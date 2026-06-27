using Xunit;
using task04;
public class SpaceshipTests
{
    [Fact]
    public void Test1()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50,cruiser.Speed);
        Assert.Equal(100,cruiser.FirePower);
    }
    [Fact]
    public void Test2()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }
    [Fact]
    public void Test3()
    {
        ISpaceship fighter = new Fighter();
        Assert.Equal(100, fighter.Speed);
        Assert.Equal(50, fighter.FirePower);
    }
}
