// Multiplayer Change 1:
// Startup now offers single-player, host, and join modes so multiplayer can be launched from the console menu.
using System.Net;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Core.Persistence;
using ConsoleApp_121_FinalProjectShell.Networking;

string savePath = Path.Combine(AppContext.BaseDirectory, "data", "savegame.db");
IGameSaveRepository saveRepository = new SqliteGameSaveRepository(savePath);
Game splashGame = new(false, saveRepository);
splashGame.ShowSplashScreen();

Console.WriteLine("Select a mode:");
Console.WriteLine("1. Single-player");
Console.WriteLine("2. Host multiplayer");
Console.WriteLine("3. Join multiplayer");
Console.Write("> ");
string mode = (Console.ReadLine() ?? "1").Trim();

switch (mode)
{
    case "2":
        await RunHostAsync(saveRepository);
        break;
    case "3":
        await RunClientAsync(saveRepository);
        break;
    default:
        new Game(false, saveRepository).Play();
        break;
}

static async Task RunHostAsync(IGameSaveRepository saveRepository)
{
    // Multiplayer Change 2:
    // The host owns the authoritative game state and listens for exactly one joining client.
    Game game = new(false, saveRepository);
    game.ConfigureAsHost();

    int port = ReadPortWithDefault(5000);
    using CancellationTokenSource cts = new();
    await using HostServer server = new(IPAddress.Any, port);

    // Multiplayer Change 3:
    // Host-side inbound messages either accept the client or execute the client's commands authoritatively.
    server.MessageReceived += async message =>
    {
        switch (message.Type)
        {
            case NetworkMessageType.JoinRequest:
                game.SetPlayerConnected(2, true);
                Console.WriteLine($"{message.PlayerName ?? "Player 2"} joined.");
                await server.SendAsync(new NetworkMessage
                {
                    Type = NetworkMessageType.JoinAccepted,
                    PlayerId = 2,
                    PlayerName = "Player 2",
                    Text = "Connected to host."
                }, cts.Token);
                await server.SendAsync(new NetworkMessage
                {
                    Type = NetworkMessageType.PlayerConnected,
                    PlayerId = 2,
                    PlayerName = "Player 2",
                    Text = "Player 2 joined."
                }, cts.Token);
                await server.SendAsync(new NetworkMessage
                {
                    Type = NetworkMessageType.FullSnapshotSync,
                    Snapshot = game.CaptureMultiplayerState(),
                    Text = game.GetLocationInfoText(2)
                }, cts.Token);
                break;

            case NetworkMessageType.CommandSubmission when message.PlayerId == 2 && !string.IsNullOrWhiteSpace(message.Text):
                CommandExecutionResult remoteResult = game.ExecuteAuthoritativeCommand(2, game.ParseCommandText(message.Text));
                await SendStateToClientAsync(server, remoteResult, cts.Token, includePlayerText: true);
                break;
        }
    };

    // Multiplayer Change 4:
    // Disconnect handling updates multiplayer connection state and keeps the host console stable.
    server.ClientDisconnected += async () =>
    {
        game.SetPlayerConnected(2, false);
        Console.WriteLine("Player 2 disconnected.");
        await Task.CompletedTask;
    };

    server.Start();
    Console.WriteLine($"Hosting multiplayer on port {port}. Waiting for one client...");
    // Multiplayer Change 5:
    // Network receive handling runs separately from local input so players do not wait on turns.
    Task acceptTask = Task.Run(async () =>
    {
        await server.AcceptClientAsync(cts.Token);
        Console.WriteLine("Client connected. Waiting for join request...");
        await server.ReceiveLoopAsync(cts.Token);
    }, cts.Token);

    Console.WriteLine(game.GetLocationInfoText(1));

    while (!cts.IsCancellationRequested)
    {
        Console.Write("> ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            continue;
        }

        Command command = game.ParseCommandText(input);
        if (command.GetCommandWord() == CommandWord.QUIT)
        {
            if (server.IsClientConnected)
            {
                await server.SendAsync(new NetworkMessage
                {
                    Type = NetworkMessageType.SystemMessage,
                    Text = "Host shut down the multiplayer session."
                }, cts.Token);
            }

            break;
        }

        // Multiplayer Change 6:
        // Host-entered commands run directly on the authoritative game so interactive commands like trade
        // still print immediately on the host console.
        LocalCommandExecutionResult result = game.ExecuteLocalAuthoritativeCommand(1, command);

        if (result.ShouldQuit)
        {
            break;
        }

        if (server.IsClientConnected)
        {
            await server.SendAsync(new NetworkMessage
            {
                Type = NetworkMessageType.StateUpdate,
                Snapshot = result.Snapshot
            }, cts.Token);
        }
    }

    cts.Cancel();
    await Task.WhenAny(acceptTask, Task.Delay(500));
}

static async Task RunClientAsync(IGameSaveRepository saveRepository)
{
    // Multiplayer Change 7:
    // The client keeps a synced local view from the host and only sends player intent across the network.
    Game game = new(false, saveRepository);
    game.ConfigureAsClient();

    Console.Write("Host IP: ");
    string host = (Console.ReadLine() ?? string.Empty).Trim();
    int port = ReadPortWithDefault(5000);

    using CancellationTokenSource cts = new();
    await using GameClient client = new();

    // Multiplayer Change 8:
    // Client message handling applies host snapshots and prints only messages intended for Player 2.
    client.MessageReceived += async message =>
    {
        switch (message.Type)
        {
            case NetworkMessageType.JoinAccepted:
                Console.WriteLine(message.Text ?? "Joined host.");
                break;

            case NetworkMessageType.PlayerConnected:
            case NetworkMessageType.PlayerDisconnected:
            case NetworkMessageType.SystemMessage:
                if (!string.IsNullOrWhiteSpace(message.Text))
                {
                    Console.WriteLine(message.Text);
                }
                break;

            case NetworkMessageType.FullSnapshotSync:
            case NetworkMessageType.StateUpdate:
                if (message.Snapshot is not null)
                {
                    game.ApplyMultiplayerState(message.Snapshot);
                }
                break;

            case NetworkMessageType.RoomUpdate:
                if (!string.IsNullOrWhiteSpace(message.Text))
                {
                    Console.WriteLine(message.Text);
                }
                break;
        }

        await Task.CompletedTask;
    };

    // Multiplayer Change 9:
    // Client disconnect handling shuts down cleanly instead of leaving the session in a bad state.
    client.Disconnected += async () =>
    {
        Console.WriteLine("Disconnected from host.");
        cts.Cancel();
        await Task.CompletedTask;
    };

    try
    {
        await client.ConnectAsync(host, port, cts.Token);
        Console.WriteLine($"Connected to {host}:{port}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection failed: {ex.Message}");
        return;
    }

    // Multiplayer Change 10:
    // The client explicitly sends a join request before gameplay commands begin.
    await client.SendAsync(new NetworkMessage
    {
        Type = NetworkMessageType.JoinRequest,
        PlayerId = 2,
        PlayerName = "Player 2"
    }, cts.Token);

    Task receiveTask = client.ReceiveLoopAsync(cts.Token);

    while (!cts.IsCancellationRequested)
    {
        Console.Write("> ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            continue;
        }

        Command command = game.ParseCommandText(input);
        if (command.GetCommandWord() == CommandWord.QUIT)
        {
            break;
        }

        // Multiplayer Change 11:
        // Remote trade is blocked for now because the original trade command is an interactive console loop
        // and does not yet have a request/response multiplayer protocol.
        if (command.GetCommandWord() == CommandWord.TRADE)
        {
            Console.WriteLine("Trade is unavailable for the joining player in multiplayer.");
            continue;
        }

        await client.SendAsync(new NetworkMessage
        {
            Type = NetworkMessageType.CommandSubmission,
            PlayerId = 2,
            Text = input
        }, cts.Token);
    }

    cts.Cancel();
    await Task.WhenAny(receiveTask, Task.Delay(500));
}

static int ReadPortWithDefault(int defaultPort)
{
    Console.Write($"Port [{defaultPort}]: ");
    string? portInput = Console.ReadLine();
    return int.TryParse(portInput, out int parsedPort) ? parsedPort : defaultPort;
}

static async Task SendStateToClientAsync(
    HostServer server,
    CommandExecutionResult result,
    CancellationToken cancellationToken,
    bool includePlayerText)
{
    // Multiplayer Change 12:
    // World snapshots are always synced, while detailed command text is forwarded only when it belongs on the client's console.
    await server.SendAsync(new NetworkMessage
    {
        Type = NetworkMessageType.StateUpdate,
        Snapshot = result.Snapshot,
        Text = includePlayerText ? result.OutputText : null
    }, cancellationToken);

    if (!includePlayerText)
    {
        return;
    }

    await server.SendAsync(new NetworkMessage
    {
        Type = NetworkMessageType.RoomUpdate,
        PlayerId = 2,
        Text = string.IsNullOrWhiteSpace(result.OutputText)
            ? result.RoomText
            : $"{result.OutputText}{result.RoomText}"
    }, cancellationToken);
}

static void PrintResult(CommandExecutionResult result)
{
    if (!string.IsNullOrWhiteSpace(result.OutputText))
    {
        Console.Write(result.OutputText);
    }
}
