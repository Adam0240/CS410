using System.Text.Json;

namespace ConsoleApp_121_FinalProjectShell.Core.Persistence;

public static class GameSaveSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static string ToJson(GameSaveState state) =>
        JsonSerializer.Serialize(state, Options);

    public static GameSaveState? FromJson(string json) =>
        JsonSerializer.Deserialize<GameSaveState>(json, Options);
}