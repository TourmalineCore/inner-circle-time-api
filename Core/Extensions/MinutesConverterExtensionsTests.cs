using Core;
using Xunit;

[UnitTest]
public class MinutesConverterExtensionsTests
{
    [Fact]
    public void ToHoursWithoutRounding_ShouldReturnCorrectValueFor60Minutes()
    {
        int minutes = 60;

        decimal hours = minutes.ToHoursWithoutRounding();

        Assert.Equal(1m, hours);
    }

    [Fact]
    public void ToHoursWithoutRounding_ShouldReturnCorrectValueFor90Minutes()
    {
        int minutes = 90;

        decimal hours = minutes.ToHoursWithoutRounding();

        Assert.Equal(1.5m, hours);
    }

    [Fact]
    public void ToHoursWithoutRounding_ShouldReturnCorrectValueFor430Minutes()
    {
        int minutes = 430;

        decimal hours = minutes.ToHoursWithoutRounding();

        Assert.Equal(7.1666666666666666666666666667m, hours);
    }
}
