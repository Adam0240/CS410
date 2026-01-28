using ConsoleApp_121_FinalProjectShell;
using Xunit;

namespace ConsoleApp_121_FinalProjectShell.Tests
{
    public class ItemTest
    {
        private readonly Item _testItem;

        public ItemTest()
        {
            // This runs before each test
            _testItem = new Item("Sword", "A sharp blade", 10, 1);
        }

        [Fact]
        public void Constructor_CreatesItem_NotNull() => Assert.NotNull(_testItem);

        [Fact]
        public void Constructor_SetsName_NotNull() => Assert.NotNull(_testItem.getName());

        [Fact]
        public void Constructor_SetsDesc_NotNull() => Assert.NotNull(_testItem.getDesc());

        [Fact]
        public void GetName_ReturnsCorrectName() => Assert.Equal("Sword", _testItem.getName());

        [Fact]
        public void GetDesc_ReturnsCorrectDescription() => Assert.Equal("A sharp blade", _testItem.getDesc());

        [Fact]
        public void GetWeight_ReturnsCorrectWeight() => Assert.Equal(10, _testItem.getWeight());

        [Fact]
        public void GetID_ReturnsCorrectID() => Assert.Equal(1, _testItem.getID());
    }
}