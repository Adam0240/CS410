// See https://aka.ms/new-console-template for more information

using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Core.Persistence;

//Save State Edit 20
string savePath = Path.Combine(AppContext.BaseDirectory, "data", "savegame.db");
//Save State Edit 21
IGameSaveRepository saveRepository = new SqliteGameSaveRepository(savePath);
//Save State Edit 22
Game game = new Game(false, saveRepository);
game.Play();
