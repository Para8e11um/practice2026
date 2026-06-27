using Xunit;
using task01;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        string input = "А роза упала на лапу Азора";
        Assert.True(input.IsPalindrome());
    }
    [Fact]
    public void Test2()
    {
        string input = "Hello";
        Assert.False(input.IsPalindrome());
    }
    [Fact]
    public void Test3()
    {
        string input = "ab:,..:,ba";
        Assert.True(input.IsPalindrome());
    }
    [Fact]
    public void Test4()
    {
        string input = "ab,.,.:::ab";
        Assert.False(input.IsPalindrome());
    }
    [Fact]
    public void Test5()
    {
        string input = "AbcbA";
        Assert.True(input.IsPalindrome());
    }
    [Fact]
    public void Test6()
    {
        string input = "AbcCBa";
        Assert.True(input.IsPalindrome());
    }
    [Fact]
    public void Test7()
    {
        string input = "A";
        Assert.True(input.IsPalindrome());
    }
    [Fact]
    public void Test8()
    {
        string input = "";
        Assert.False(input.IsPalindrome());
    }
    [Fact]
    public void Test9()
    {
        string input = "ab aa b a";
        Assert.True(input.IsPalindrome());
    }
    [Fact]
    public void Test10()
    {
        string input = "123321";
        Assert.True(input.IsPalindrome());
    }
    public void Test11()
    {
        string input = "123123";
        Assert.False(input.IsPalindrome());
    }
}
