// Multiplayer summary: connects to the host TCP server and forwards inbound and outbound multiplayer messages.
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp_121_FinalProjectShell.Networking;

public sealed class GameClient : IAsyncDisposable
{
    private readonly TcpClient _client = new();
    private StreamReader? _reader;
    private StreamWriter? _writer;

    public event Func<NetworkMessage, Task>? MessageReceived;
    public event Func<Task>? Disconnected;

    public bool IsConnected => _client.Connected;

    public async Task ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
    {
        await _client.ConnectAsync(host, port, cancellationToken);
        NetworkStream stream = _client.GetStream();
        _reader = MessageCodec.CreateReader(stream);
        _writer = MessageCodec.CreateWriter(stream);
    }

    public async Task SendAsync(NetworkMessage message, CancellationToken cancellationToken = default)
    {
        if (_writer is null)
        {
            throw new InvalidOperationException("The client is not connected.");
        }

        await MessageCodec.WriteMessageAsync(_writer, message, cancellationToken);
    }

    public async Task ReceiveLoopAsync(CancellationToken cancellationToken = default)
    {
        if (_reader is null)
        {
            throw new InvalidOperationException("The client is not connected.");
        }

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                NetworkMessage? message = await MessageCodec.ReadMessageAsync(_reader, cancellationToken);
                if (message is null)
                {
                    break;
                }

                if (MessageReceived is not null)
                {
                    await MessageReceived.Invoke(message);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (Disconnected is not null)
            {
                await Disconnected.Invoke();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_writer is not null)
        {
            await _writer.DisposeAsync();
        }

        _reader?.Dispose();
        _client.Dispose();
    }
}
