//Game.Testing.cs
//Removes some of the testing logic out of Game.cs

using System.Collections.Generic;
using ConsoleApp_121_FinalProjectShell.Items;

namespace ConsoleApp_121_FinalProjectShell.Core;

public partial class Game
{
    // Test-only helper: keeps createRooms() readable by pushing tracking code out of Game.cs
    private void TrackTestArtifacts(IEnumerable<Room> rooms, IEnumerable<Item?> items)
    {
        if (!isTestInstance)
            return;

        // Safety: in case someone changes constructor init later
        allRooms ??= new List<Room>();
        allItems ??= new List<Item?>();

        allRooms.AddRange(rooms);
        allItems.AddRange(items);
    }
}
