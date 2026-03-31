using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Items;
using System;
using Xunit;
using Bogus;

namespace UnitTesting
{
    public class RoomTests
    {
        private static Item CreateTestItem(string name, string desc, int weight = 1, int id = 3)
        {
            return ItemFactory.Create(name, desc, weight, id);
        }

        [Fact]
        public void Constructor_InitializesRoomCorrectly()
        {
            var room = new Room("Test room", 99);

            var id = room.GetId();
            var description = room.GetLongDesc();

            Assert.Equal(99, id);
            Assert.Contains("Test room", description);
        }

        [Fact]
        public void SetExit_AddsExitSuccessfully()
        {
            var roomA = new Room("Room A", 1);
            var roomB = new Room("Room B", 2);

            roomA.SetExit("north", roomB);

            Assert.True(roomA.GetExits().ContainsKey("north"));
            Assert.Equal(roomB, roomA.GetExits()["north"]);
        }

        [Fact]
        public void GetExitString_ShowsAllExits()
        {
            var room = new Room("Test", 1);
            room.SetExit("north", new Room("North", 2));
            room.SetExit("south", new Room("South", 3));
            room.SetExit("grove", new Room("Grove", 8));

            var exitString = room.GetExitString();

            Assert.Contains("north", exitString);
            Assert.Contains("south", exitString);
            Assert.Contains("grove", exitString);
        }

        [Fact]
        public void AddItem_ItemAppearsInRoom()
        {
            var room = new Room("Test", 1);
            var item = CreateTestItem("Key", "a rusty key");

            room.addItem(item);

            Assert.True(room.hasItemByName("key"));
            Assert.NotNull(room.getItemByName("key"));
        }

        [Fact]
        public void RemoveItemByName_RemovesCorrectItem()
        {
            var room = new Room("Test", 1);
            var item = CreateTestItem("Key", "a rusty key");
            room.addItem(item);

            room.removeItemByName("key");

            Assert.False(room.hasItemByName("key"));
            Assert.Null(room.getItemByName("key"));
        }

        [Fact]
        public void GetItemByName_IsCaseInsensitive()
        {
            var room = new Room("Test", 1);
            var item = CreateTestItem("Sword", "a sharp sword");
            room.addItem(item);

            var found = room.getItemByName("sWoRd");

            Assert.NotNull(found);
            Assert.Equal(item, found);
        }

        [Fact]
        public void GetDescription_ReturnsBaseDescription()
        {
            var room = new Room("Old forge", 4);

            var description = room.GetDescription();

            Assert.Equal("Old forge", description);
        }

        [Fact]
        public void GetRandomExit_ReturnsValidExit()
        {
            var room = new Room("Test", 1);
            room.SetExit("north", new Room("North", 2));
            room.SetExit("south", new Room("South", 3));

            var exit = room.GetRandomExit();

            Assert.NotNull(exit);
            Assert.Contains(exit, room.GetExits().Keys);
        }

        // BOGUS TESTS

        [Fact]
        public void AddAndRemoveGeneratedItem_WorksCorrectly()
        {
            Randomizer.Seed = new Random(123);

            var faker = new Faker();

            string itemName = faker.Commerce.ProductName().Replace(" ", "");
            string itemDesc = faker.Lorem.Sentence();
            int weight = faker.Random.Int(1, 10);

            var room = new Room("Generated test room", 1);
            var item = CreateTestItem(itemName, itemDesc, weight);

            room.addItem(item);

            Assert.True(room.hasItemByName(itemName));
            Assert.NotNull(room.getItemByName(itemName));

            room.removeItemByName(itemName);

            Assert.False(room.hasItemByName(itemName));
            Assert.Null(room.getItemByName(itemName));
        }

        [Fact]
        public void SetExit_WithGeneratedDirection_StoresExitCorrectly()
        {
            Randomizer.Seed = new Random(456);

            var faker = new Faker();
            string direction = faker.Random.Word();

            var roomA = new Room("Room A", 1);
            var roomB = new Room("Room B", 2);

            roomA.SetExit(direction, roomB);

            Assert.True(roomA.GetExits().ContainsKey(direction));
            Assert.Equal(roomB, roomA.GetExits()[direction]);
        }
    }
}