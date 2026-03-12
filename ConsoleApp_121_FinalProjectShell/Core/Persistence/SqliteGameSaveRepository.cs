using Microsoft.Data.Sqlite;

namespace ConsoleApp_121_FinalProjectShell.Core.Persistence;

public class SqliteGameSaveRepository : IGameSaveRepository
{
    private readonly string _connectionString;

    public SqliteGameSaveRepository(string dbPath)
    {
        var fullPath = Path.GetFullPath(dbPath);
        var dir = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _connectionString = $"Data Source={fullPath}";
    }

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS saves (
                slot_id INTEGER PRIMARY KEY,
                save_json TEXT NOT NULL,
                updated_utc TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    public void SaveJson(string saveJson)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO saves (slot_id, save_json, updated_utc)
            VALUES (1, $json, $updated)
            ON CONFLICT(slot_id) DO UPDATE SET
                save_json = excluded.save_json,
                updated_utc = excluded.updated_utc;
            """;
        command.Parameters.AddWithValue("$json", saveJson);
        command.Parameters.AddWithValue("$updated", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    public string? LoadJson()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT save_json FROM saves WHERE slot_id = 1 LIMIT 1;";

        var result = command.ExecuteScalar();
        return result as string;
    }
}