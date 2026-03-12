using System.Text;

namespace ConsoleApp_121_FinalProjectShell.Core;

public static class RoomTextService
{
    public static string GetDescription(Room room, GameProgress progress)
    {
        if (room.GetId() == 4 && progress.ForgePrepared)
        {
            return "Lava flows through the channels dug into the rock around a vacant smith's shop.\nThe forge and its tools stand complete.";
        }

        if (room.GetId() == 6 && progress.GateOpen)
        {
            return "The castle gate stands tall and imposing as before. Now however,\na large hole has been hacked through to the other side.";
        }

        if (room.GetId() == 1 && progress.SwampCleared)
        {
            return "Your boots catch in the stiff and stinking muck of the swamp.\nThe large log lies in pieces now, revealing a hidden path.";
        }

        if (room.GetId() == 8 && progress.SwordPlaced)
        {
            return "Sunlight filters through the treetops into the solitary grove.\nA derelict altar stands at its center, now bearing a shining sword.";
        }

        return room.GetDescription();
    }

    public static string GetExitString(Room room, GameProgress progress)
    {
        var exitString = new StringBuilder("Exits:");

        foreach (string exit in room.GetExits().Keys)
        {
            if (exit != "grove" || progress.SwampCleared)
            {
                exitString.Append(" ").Append(exit);
            }
        }

        return exitString.ToString();
    }

    public static string GetLongDescription(Room room, GameProgress progress)
    {
        var builtDescription = new StringBuilder(GetDescription(room, progress));
        builtDescription.Append("\n");

        if (room.GetItemsCount() > 0)
        {
            builtDescription.Append(room.itemsText()).Append(".\n");
        }

        builtDescription.Append(GetExitString(room, progress));
        return builtDescription.ToString();
    }
}