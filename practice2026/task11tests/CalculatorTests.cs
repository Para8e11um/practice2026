using task11;
using System;
using Xunit;
public class CalculatorTests
{
    private readonly ICalculator _calculator;

    public CalculatorTests()
    {
        _calculator = CalculatorGenerator.Create();
    }
    [Fact]
    public void Add()
    {
        Assert.Equal(5, _calculator.Add(2, 3));
        Assert.Equal(-1, _calculator.Add(-4,3));
    }
    [Fact]
    public void Minus()
    {
        Assert.Equal(5, _calculator.Minus(10, 5));
        Assert.Equal(-7, _calculator.Minus(-4,3));
    }
    [Fact]
    public void Mul()
    {
        Assert.Equal(6, _calculator.Mul(2, 3));
        Assert.Equal(-12, _calculator.Mul(-4, 3));
    }
    [Fact]
    public void Div()
    {
        Assert.Equal(2, _calculator.Div(6, 3));
        Assert.Equal(-2, _calculator.Div(-5, 2));
    }
    [Fact]
    public void DivideByZero()
    {
        Assert.Throws<DivideByZeroException>(() => _calculator.Div(1, 0));
    }
}
