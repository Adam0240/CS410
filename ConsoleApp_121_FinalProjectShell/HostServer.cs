// Multiplayer summary: hosts the single-client TCP server, accepts one joiner, and dispatches inbound messages.
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp_121_FinalProjectShell.Networking;

public sealed class HostServer : IAsyncDisposable
{
    private readonly TcpListener _listener;
    private TcpClient? _client;
    private StreamReader? _reader;
    private StreamWriter? _writer;

    public HostServer(IPAddress ipAddress, int port)
    {
        _listener = new TcpListener(ipAddress, port);
    }

    public event Func<NetworkMessage, Task>? MessageReceived;
    public event Func<Task>? ClientDisconnected;

    public bool IsClientConnected => _client?.Connected == true;

    public void Start()
    {
        _listener.Start();
    }

    public async Task<TcpClient> AcceptClientAsync(CancellationToken cancellationToken = default)
    {
        _client = await _listener.AcceptTcpClientAsync(cancellationToken);
        NetworkStream stream = _client.GetStream();
        _reader = MessageCodec.CreateReader(stream);
        _writer = MessageCodec.CreateWriter(stream);
        return _client;
    }

    public async Task SendAsync(NetworkMessage message, CancellationToken cancellationToken = default)
    {
        if (_writer is null)
        {
            throw new InvalidOperationException("No connected client is available.");
        }

        await MessageCodec.WriteMessageAsync(_writer, message, cancellationToken);
    }

    public async Task ReceiveLoopAsync(CancellationToken cancellationToken = default)
    {
        if (_reader is null)
        {
            throw new InvalidOperationException("No connected client is available.");
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
            if (ClientDisconnected is not null)
            {
                await ClientDisconnected.Invoke();
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
        _client?.Dispose();
        _listener.Stop();
    }
}
