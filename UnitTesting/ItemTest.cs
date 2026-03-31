using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Items;
using Xunit;
using Bogus;

namespace UnitTesting
{
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
        public void Constructor_SetsName_NotNull() => Assert.NotNull(_testItem.GetName());

        [Fact]
        public void Constructor_SetsDesc_NotNull() => Assert.NotNull(_testItem.GetDesc());

        [Fact]
        public void GetName_ReturnsCorrectName() => Assert.Equal("Sword", _testItem.GetName());

        [Fact]
        public void GetDesc_ReturnsCorrectDescription() => Assert.Equal("A sharp blade", _testItem.GetDesc());

        [Fact]
        public void GetWeight_ReturnsCorrectWeight() => Assert.Equal(10, _testItem.GetWeight());

        [Fact]
        public void GetID_ReturnsCorrectID() => Assert.Equal(5, _testItem.GetID());

        [Fact]
        public void Constructor_WithBogusData_CreatesValidItem()
        {
            // Make results repeatable 
            Randomizer.Seed = new Random(123);

            // Create fake data 
            var faker = new Faker();

            for (int i = 0; i < 5; i++) // run multiple times
            {
                string name = faker.Commerce.ProductName();
                string desc = faker.Lorem.Sentence();
                int weight = faker.Random.Int(1, 100);
                int id = faker.Random.Int(0, 5); 

                Item item = ItemFactory.Create(name, desc, weight, id);

                // Assertions (this is what tests)
                Assert.NotNull(item);
                Assert.Equal(name, item.GetName());
                Assert.Equal(desc, item.GetDesc());
                Assert.Equal(weight, item.GetWeight());
                Assert.Equal(id, item.GetID());
            }
        }
    }


}