using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// A button's travel and a rotating part's spin, read from the keys Levels.fgd names.
/// </summary>
public sealed class LevelMoversTests
{
    private const float Close = 1e-5f;

    private static LevelEntity Mover(string className, params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        entity["classname"] = className;
        foreach (var (key, value) in keys) entity[key] = value;
        entity.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 8, 96), "proto/door"));
        return entity;
    }

    [Fact]
    public void AButtonTravelsAsADoorDoesWithALipOfFour()
    {
        var travel = Mover("func_button").Travel(32)!.Value;
        Assert.True(Vector3.Distance(new Vector3(60f / 32, 0, 0), travel) < Close, $"{travel}");
        Assert.True(Vector3.Distance(new Vector3(56f / 32, 0, 0), Mover("func_door").Travel(32)!.Value) < Close);
    }

    [Fact]
    public void ARotatingPartTurnsAboutTheVerticalAtAHundredDegreesASecond()
    {
        Assert.Equal(new LevelSpin(Vector3.UnitY, 100), Mover("func_rotating").Spin(32));
        Assert.Equal(new LevelSpin(Vector3.UnitY, 30), Mover("func_rotating", ("speed", "30")).Spin(32));
    }

    [Theory]
    [InlineData("4", 1, 0, 0, 100)]
    [InlineData("8", 0, 0, -1, 100)]
    [InlineData("2", 0, 1, 0, -100)]
    [InlineData("6", 1, 0, 0, -100)]
    [InlineData("1", 0, 1, 0, 100)]
    public void ItsFlagsTurnItAboutTheFilesXOrYOrTheOtherWay(string flags, float x, float y, float z, double speed)
    {
        Assert.Equal(new LevelSpin(new Vector3(x, y, z), speed), Mover("func_rotating", ("spawnflags", flags)).Spin(32));
    }

    [Fact]
    public void AnEntityOfAnotherClassHasNoSpinAndNoTravel()
    {
        Assert.Null(Mover("func_wall").Spin(32));
        Assert.Null(Mover("func_rotating").Travel(32));
    }

    [Theory]
    [InlineData("speed", "fast")]
    [InlineData("spawnflags", "-4")]
    public void ASpeedOrFlagsThatDoNotReadAreRefused(string key, string value)
    {
        var part = Mover("func_rotating", (key, value));

        Assert.Throws<FormatException>(() => part.Spin(32));
        Assert.Equal([key], new Level { Entities = { part } }.Check().Select(problem => problem.Key));
    }
}
