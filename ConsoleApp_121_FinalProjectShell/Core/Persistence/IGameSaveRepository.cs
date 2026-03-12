namespace ConsoleApp_121_FinalProjectShell.Core.Persistence;

public interface IGameSaveRepository
{
    void Initialize();
    void SaveJson(string saveJson);
    string? LoadJson();
}