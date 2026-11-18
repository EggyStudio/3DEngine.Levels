using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// A door's travel from its keys and its brushes, the way it opens by its size that way less its
/// lip, in meters with Y up.
/// </summary>
public sealed class LevelDoorTests
{
    private const float Close = 1e-5f;

    // A door 64 units along X, 8 deep and 96 high, at 32 units a meter.
    private static LevelEntity Door(params (string Key, string Value)[] keys)
    {
        var door = new LevelEntity();
        door["classname"] = "func_door";
        foreach (var (key, value) in keys) door[key] = value;
        door.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 8, 96), "proto/door"));
        return door;
    }

    private static void Near(Vector3 expected, Vector3? actual) =>
        Assert.True(actual is { } a && Vector3.Distance(expected, a) < Close, $"{actual} where {expected} is meant");

    [Fact]
    public void ADoorOpensAlongItsAngleByItsSizeThatWayLessItsLip()
    {
        Near(new Vector3(56f / 32, 0, 0), Door().Travel(32));
        Near(new Vector3(-56f / 32, 0, 0), Door(("angle", "180")).Travel(32));
        Near(new Vector3(2, 0, 0), Door(("lip", "0")).Travel(32));
    }

    [Fact]
    public void AnAngleOfMinusOneOpensUpAndMinusTwoDown()
    {
        Near(new Vector3(0, 88f / 32, 0), Door(("angle", "-1")).Travel(32));
        Near(new Vector3(0, -88f / 32, 0), Door(("angle", "-2")).Travel(32));
    }

    [Fact]
    public void ADoorOpeningAcrossItsDepthLessThanItsLipStaysShut()
    {
        Near(Vector3.Zero, Door(("angle", "90")).Travel(32));
    }

    [Fact]
    public void ADoorOpeningOnADiagonalTravelsItsSizeAlongTheWay()
    {
        // Along 45 degrees the door's 64 by 8 spans 72 over the root of 2, less the lip of 8, the
        // file's Y north being meters' minus Z.
        var span = (64 + 8) / Math.Sqrt(2) - 8;
        var way = new Vector3(1, 0, -1) / MathF.Sqrt(2);

        Near(way * (float)(span / 32), Door(("angle", "45")).Travel(32));
    }

    [Fact]
    public void AnEntityOfAnotherClassHasNoTravel()
    {
        var wall = Door();
        wall["classname"] = "func_wall";

        Assert.Null(wall.Travel(32));
    }

    [Fact]
    public void ALipOrAnAngleThatDoesNotReadIsRefused()
    {
        Assert.Throws<FormatException>(() => Door(("lip", "some")).Travel(32));
        Assert.Throws<FormatException>(() => Door(("angle", "east")).Travel(32));
    }
}
