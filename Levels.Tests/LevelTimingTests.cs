using Xunit;

namespace Levels.Tests;

/// <summary>A door's and a button's speed and wait, read from the keys Levels.fgd names.</summary>
public sealed class LevelTimingTests
{
    private static LevelEntity Mover(string className, params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        entity["classname"] = className;
        foreach (var (key, value) in keys) entity[key] = value;
        return entity;
    }

    [Fact]
    public void ADoorMovesAHundredUnitsASecondAndWaitsThreeSecondsAndAButtonFortyAndOne()
    {
        Assert.Equal(new LevelTiming(100.0 / 32, 3), Mover("func_door").Timing(32));
        Assert.Equal(new LevelTiming(40.0 / 32, 1), Mover("func_button").Timing(32));
    }

    [Fact]
    public void ItsSpeedIsUnitsASecondAndAWaitBelowNoneStays()
    {
        var timing = Mover("func_door", ("speed", "64"), ("wait", "-1")).Timing(32)!.Value;

        Assert.Equal(2, timing.MetersPerSecond);
        Assert.True(timing.Stays);
        Assert.False(Mover("func_door").Timing(32)!.Value.Stays);
    }

    [Fact]
    public void AnEntityOfAnotherClassHasNoTiming()
    {
        Assert.Null(Mover("func_rotating").Timing(32));
    }

    [Theory]
    [InlineData("speed", "0")]
    [InlineData("speed", "quick")]
    [InlineData("wait", "long")]
    public void ASpeedOfNoneOrAKeyThatDoesNotReadIsRefusedAndNamed(string key, string value)
    {
        var door = Mover("func_door", (key, value));

        Assert.Throws<FormatException>(() => door.Timing(32));
        Assert.Equal([key], new Level { Entities = { door } }.Check().Select(problem => problem.Key));
    }
}
