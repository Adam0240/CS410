using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Core;
using Bogus;
using Xunit;

namespace UnitTesting
{
    public class RoomTextServiceTests
    {
        [Fact]
        public void GetDescription_ReturnsBaseDescription_WhenNoProgressApplies()
        {
            var room = new Room("Old forge", 4);
            var progress = new GameProgress();

            var description = RoomTextService.GetDescription(room, progress);

            Assert.Equal("Old forge", description);
        }

        [Fact]
        public void GetDescription_Changes_WhenForgePrepared()
        {
            var room = new Room("Old forge", 4);
            var progress = new GameProgress
            {
                ForgePrepared = true
            };

            var description = RoomTextService.GetDescription(room, progress);

            Assert.Contains("forge", description, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetDescription_Changes_WhenGateOpen()
        {
            var room = new Room("Castle Gate", 6);
            var progress = new GameProgress
            {
                GateOpen = true
            };

            var description = RoomTextService.GetDescription(room, progress);

            Assert.Contains("hole", description, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetDescription_Changes_WhenSwampCleared()
        {
            var room = new Room("Swamp", 1);
            var progress = new GameProgress
            {
                SwampCleared = true
            };

            var description = RoomTextService.GetDescription(room, progress);

            Assert.Contains("hidden path", description, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetDescription_Changes_WhenSwordPlaced()
        {
            var room = new Room("Grove", 8);
            var progress = new GameProgress
            {
                SwordPlaced = true
            };

            var description = RoomTextService.GetDescription(room, progress);

            Assert.Contains("altar", description, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetExitString_HidesGrove_WhenSwampNotCleared()
        {
            var room = new Room("Swamp", 1);
            room.SetExit("north", new Room("North", 2));
            room.SetExit("grove", new Room("Grove", 8));

            var progress = new GameProgress
            {
                SwampCleared = false
            };

            var exitString = RoomTextService.GetExitString(room, progress);

            Assert.Contains("north", exitString);
            Assert.DoesNotContain("grove", exitString);
        }

        [Fact]
        public void GetExitString_ShowsGrove_WhenSwampCleared()
        {
            var room = new Room("Swamp", 1);
            room.SetExit("north", new Room("North", 2));
            room.SetExit("grove", new Room("Grove", 8));

            var progress = new GameProgress
            {
                SwampCleared = true
            };

            var exitString = RoomTextService.GetExitString(room, progress);

            Assert.Contains("north", exitString);
            Assert.Contains("grove", exitString);
        }

        // BOGUS TESTS

        [Fact]
        public void GetDescription_WithGeneratedProgress_NeverReturnsEmpty()
        {
            Randomizer.Seed = new Random(123);

            var faker = new Faker<GameProgress>()
                .RuleFor(p => p.SwampCleared, f => f.Random.Bool())
                .RuleFor(p => p.ForgePrepared, f => f.Random.Bool())
                .RuleFor(p => p.SwordPlaced, f => f.Random.Bool())
                .RuleFor(p => p.GateOpen, f => f.Random.Bool())
                .RuleFor(p => p.ToldProtagGate, f => f.Random.Bool())
                .RuleFor(p => p.ToldProtagSword, f => f.Random.Bool());

            var rooms = new[]
            {
                new Room("Old forge", 4),
                new Room("Castle Gate", 6),
                new Room("Swamp", 1),
                new Room("Grove", 8)
            };

            for (int i = 0; i < 10; i++)
            {
                var progress = faker.Generate();

                foreach (var room in rooms)
                {
                    var description = RoomTextService.GetDescription(room, progress);

                    Assert.False(string.IsNullOrWhiteSpace(description));
                }
            }
        }

        [Fact]
        public void GetExitString_WithGeneratedProgress_AlwaysReturnsValidText()
        {
            Randomizer.Seed = new Random(456);

            var faker = new Faker<GameProgress>()
                .RuleFor(p => p.SwampCleared, f => f.Random.Bool())
                .RuleFor(p => p.ForgePrepared, f => f.Random.Bool())
                .RuleFor(p => p.SwordPlaced, f => f.Random.Bool())
                .RuleFor(p => p.GateOpen, f => f.Random.Bool())
                .RuleFor(p => p.ToldProtagGate, f => f.Random.Bool())
                .RuleFor(p => p.ToldProtagSword, f => f.Random.Bool());

            for (int i = 0; i < 10; i++)
            {
                var room = new Room("Swamp", 1);
                room.SetExit("north", new Room("North", 2));
                room.SetExit("south", new Room("South", 3));
                room.SetExit("grove", new Room("Grove", 8));

                var progress = faker.Generate();

                var exitString = RoomTextService.GetExitString(room, progress);

                Assert.False(string.IsNullOrWhiteSpace(exitString));
                Assert.Contains("north", exitString);
                Assert.Contains("south", exitString);

                if (progress.SwampCleared)
                {
                    Assert.Contains("grove", exitString);
                }
                else
                {
                    Assert.DoesNotContain("grove", exitString);
                }
            }
        }
    }
}