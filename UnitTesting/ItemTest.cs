using ConsoleApp_121_FinalProjectShell;
using Xunit;


public class ItemTest
{
    private readonly Item _testItem;

    public ItemTest()
    {
        // Item is abstract now, so we must construct a concrete subclass.
        // We use the factory so the test stays consistent with how the game creates items.
        _testItem = ItemFactory.Create("Sword", "A sharp blade", 10, 5);
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
    public void GetID_ReturnsCorrectID() => Assert.Equal(5, _testItem.getID());
}
