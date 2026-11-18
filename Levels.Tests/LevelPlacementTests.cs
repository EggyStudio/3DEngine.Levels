using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// Where a level's entity stands and how it is turned, read from its keys as an engine places it,
/// and the keys read as numbers.
/// </summary>
public sealed class LevelPlacementTests
{
    private const float Close = 1e-5f;

    private static LevelEntity Entity(params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        foreach (var (key, value) in keys) entity.Keys.Add(new(key, value));
        return entity;
    }

    private static Vector3 Forward(LevelPlacement placement) =>
        Vector3.Transform(-Vector3.UnitZ, placement.Rotation);

    private static void Near(Vector3 expected, Vector3 actual) =>
        Assert.True(Vector3.Distance(expected, actual) < Close, $"{actual} where {expected} is meant");

    [Fact]
    public void APointEntityStandsAtItsOriginInMeters()
    {
        var start = Entity(("classname", "info_player_start"), ("origin", "64 -32 16"));

        // The file's (x, y, z) at (x, z, -y) over 32 units a meter.
        Near(new Vector3(2, 0.5f, 1), start.Placement(32).Position);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 90, 0)]
    [InlineData(0, 180, 0)]
    [InlineData(0, -135, 0)]
    [InlineData(30, 0, 0)]
    [InlineData(-45, 60, 0)]
    [InlineData(20, 200, 45)]
    public void ItsForwardLooksTheWayTheFileFacesIt(double pitch, double yaw, double roll)
    {
        var entity = Entity(("classname", "info_null"), ("angles", $"{pitch} {yaw} {roll}"));
        var p = pitch * Math.PI / 180;
        var y = yaw * Math.PI / 180;

        // The file faces an entity at no angle along X, its yaw turning it toward Y and a pitch
        // above zero looking down.
        var facing = new LevelPoint(Math.Cos(p) * Math.Cos(y), Math.Cos(p) * Math.Sin(y), -Math.Sin(p));
        Near(facing.Meters(1), Forward(entity.Placement(32)));
    }

    [Fact]
    public void AnAngleIsTheYawAndAnglesComeBeforeIt()
    {
        Near(Vector3.UnitX, Forward(Entity(("angle", "0")).Placement(32)));
        Near(-Vector3.UnitZ, Forward(Entity(("angle", "90")).Placement(32)));
        Near(Vector3.UnitZ, Forward(Entity(("angle", "90"), ("angles", "0 -90 0")).Placement(32)));
    }

    [Fact]
    public void ARollTurnsItsUpAboutItsForward()
    {
        var level = Entity(("angles", "0 0 0")).Placement(32);
        var rolled = Entity(("angles", "0 0 90")).Placement(32);

        Near(Vector3.UnitY, Vector3.Transform(Vector3.UnitY, level.Rotation));
        Near(Vector3.UnitX, Forward(rolled));

        // Up leans to the file's minus Y, the right of an entity facing along X.
        Near(new LevelPoint(0, -1, 0).Meters(1), Vector3.Transform(Vector3.UnitY, rolled.Rotation));
    }

    [Fact]
    public void ABrushEntityStandsUnturnedAtItsOriginOrItsBrushesMiddle()
    {
        var door = Entity(("classname", "func_door"), ("angle", "90"));
        door.Brushes.Add(LevelBrush.Box(new LevelPoint(-32, 104, 0), new LevelPoint(32, 112, 96), "door"));

        var middle = door.Placement(32);
        Near(new Vector3(0, 1.5f, -3.375f), middle.Position);
        Assert.Equal(Quaternion.Identity, middle.Rotation);

        door["origin"] = "-32 108 0";
        Near(new Vector3(-1, 0, -3.375f), door.Placement(32).Position);
    }

    [Fact]
    public void TheWorldStandsAtTheLevelsOrigin()
    {
        var world = Entity(("classname", "worldspawn"));
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(64, 64, 0), new LevelPoint(128, 128, 32), "floor"));

        var placed = world.Placement(32);
        Assert.Equal(Vector3.Zero, placed.Position);
        Assert.Equal(Quaternion.Identity, placed.Rotation);
    }

    [Fact]
    public void AKeyIsReadAsNumbersOrNothingWhereItIsMissing()
    {
        var tree = Entity(("classname", "tree"), ("seed", "7"), ("scale", "1.5"), ("origin", "1 2 3"));

        Assert.Equal(7u, tree.WholeNumber("seed"));
        Assert.Equal(1.5, tree.Number("scale"));
        Assert.Equal(new LevelPoint(1, 2, 3), tree.Point("origin"));
        Assert.Null(tree.Number("angle"));
        Assert.Null(tree.WholeNumber("count"));
        Assert.Null(tree.Point("angles"));
    }

    [Theory]
    [InlineData("origin", "1 2", "three numbers")]
    [InlineData("origin", "1 two 3", "three numbers")]
    [InlineData("angles", "0 90 0 0", "three numbers")]
    [InlineData("angle", "east", "a number")]
    [InlineData("angle", "Infinity", "a number")]
    public void AMalformedPlaceIsRefusedWithItsKeyNamed(string key, string text, string meant)
    {
        var entity = Entity(("classname", "info_player_start"), (key, text));

        var fault = Assert.Throws<FormatException>(() => entity.Placement(32));
        Assert.Equal($"The info_player_start has the {key} {text}, where {meant} is meant.", fault.Message);
    }

    [Fact]
    public void AWholeNumberRefusesASignOrAFraction()
    {
        Assert.Throws<FormatException>(() => Entity(("seed", "-1")).WholeNumber("seed"));
        Assert.Throws<FormatException>(() => Entity(("seed", "1.5")).WholeNumber("seed"));
    }
}
