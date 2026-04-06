// Multiplayer summary: serializes and deserializes newline-delimited JSON messages over the TCP streams.
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp_121_FinalProjectShell.Networking;

public static class MessageCodec
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string Encode(NetworkMessage message)
    {
        return JsonSerializer.Serialize(message, SerializerOptions);
    }

    public static NetworkMessage Decode(string payload)
    {
        return JsonSerializer.Deserialize<NetworkMessage>(payload, SerializerOptions)
               ?? throw new InvalidDataException("Received an invalid network message.");
    }

    public static async Task WriteMessageAsync(StreamWriter writer, NetworkMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await writer.WriteLineAsync(Encode(message));
        await writer.FlushAsync();
    }

    public static async Task<NetworkMessage?> ReadMessageAsync(StreamReader reader, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string? line = await reader.ReadLineAsync(cancellationToken);
        return line is null ? null : Decode(line);
    }

    public static StreamReader CreateReader(Stream stream)
    {
        return new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
    }

    public static StreamWriter CreateWriter(Stream stream)
    {
        return new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
    }
}
