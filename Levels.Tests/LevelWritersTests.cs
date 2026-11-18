using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// An entity's place, light and tree written back to its keys, so a tool saves what it loaded:
/// unedited, the level writes back to the byte, and an edit reads back as it was made.
/// </summary>
public sealed class LevelWritersTests
{
    private const float Close = 1e-4f;

    private static void Near(Vector3 expected, Vector3 actual) =>
        Assert.True(Vector3.Distance(expected, actual) < Close, $"{actual} where {expected} is meant");

    private static LevelEntity Point(string className, params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        entity["classname"] = className;
        foreach (var (key, value) in keys) entity[key] = value;
        return entity;
    }

    [Fact]
    public void EveryClassOfTheExampleWrittenBackUneditedWritesTheSameLevel()
    {
        var level = ExampleLevelTests.Generate();
        var text = level.Write();

        foreach (var entity in level.Entities)
        {
            entity.Place(entity.Placement(level.UnitsPerMeter), level.UnitsPerMeter);
            if (entity.Light(level.UnitsPerMeter) is { } light) entity.SetLight(light, level.UnitsPerMeter);
            if (entity.Tree() is { } tree) entity.SetTree(tree);
        }

        Assert.Equal(text, level.Write());
    }

    [Fact]
    public void APointEntityMovedAndTurnedReadsBackWhereItWasPut()
    {
        var start = Point("info_player_start", ("origin", "0 0 0"), ("angle", "0"));
        start.Place(new LevelPlacement(new Vector3(1, 2, 3), start.Placement(32).Rotation * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2)), 32);

        Assert.Equal("32 -96 64", start["origin"]);
        Assert.Equal("90", start["angle"]);
        Assert.Null(start["angles"]);
        Near(new Vector3(1, 2, 3), start.Placement(32).Position);
    }

    [Fact]
    public void ATurnWithAPitchOrARollIsWrittenAsTheThreeAngles()
    {
        var spot = Point("light_spot", ("angle", "45"));
        var wanted = Point("light_spot", ("angles", "30 120 15")).Placement(32);

        spot.Place(wanted, 32);

        Assert.Equal("30 120 15", spot["angles"]);
        Assert.Null(spot["angle"]);
        Assert.True(Math.Abs(Quaternion.Dot(wanted.Rotation, spot.Placement(32).Rotation)) > 1 - 1e-6);
    }

    [Fact]
    public void ALightsEditsAreWrittenAndItsDefaultsLeftUnwritten()
    {
        var light = Point("light", ("origin", "0 0 64"));
        var read = light.Light(32)!;

        light.SetLight(read with { Intensity = 1500, Color = LevelLight.Linear(255, 128, 10), Position = new Vector3(1, 3, -2) }, 32);

        Assert.Equal("1500", light["intensity"]);
        Assert.Equal("255 128 10", light["_color"]);
        Assert.Equal("32 64 96", light["origin"]);
        Assert.Null(light["range"]);
        var back = light.Light(32)!;
        Assert.Equal(1500, back.Intensity);
        Near(new Vector3(1, 3, -2), back.Position);
    }

    [Fact]
    public void ASpotTurnedReadsBackFacingTheWayItWasTurned()
    {
        var spot = Point("light_spot", ("origin", "0 0 96"), ("angles", "90 0 0"));
        var way = Vector3.Normalize(new Vector3(1, -1, 0));

        spot.SetLight(spot.Light(32)! with { Direction = way, OuterCone = 45 }, 32);

        Near(way, spot.Light(32)!.Direction);
        Assert.Equal("45", spot["_cone"]);
    }

    [Fact]
    public void ATreeOfNoSpeciesLosesTheKeyAndASeedIsWritten()
    {
        var tree = Point("tree", ("species", "example-species"), ("seed", "3"));

        tree.SetTree(new LevelTree(null, 9));

        Assert.Null(tree["species"]);
        Assert.Equal("9", tree["seed"]);
        Assert.Equal(new LevelTree(null, 9), tree.Tree());
    }

    [Fact]
    public void ABrushEntityTurnedOrTheWorldMovedIsRefused()
    {
        var door = Point("func_door");
        door.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(8, 64, 96), "proto/door"));
        var world = Point("worldspawn");

        Assert.Throws<ArgumentException>(() =>
            door.Place(new LevelPlacement(door.Placement(32).Position, Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1)), 32));
        Assert.Throws<ArgumentException>(() => world.Place(new LevelPlacement(Vector3.One, Quaternion.Identity), 32));
    }

    [Fact]
    public void AnyTurnWrittenReadsBackAsTheSameRotation()
    {
        var random = new Random(5);
        double[] pitches = [90, -90, 0, 45];
        for (var trial = 0; trial < 500; trial++)
        {
            var pitch = trial < pitches.Length ? pitches[trial] : random.NextDouble() * 180 - 90;
            var (yaw, roll) = (random.NextDouble() * 360 - 180, random.NextDouble() * 360 - 180);
            var wanted = Point("info_null", ("angles", $"{pitch:R} {yaw:R} {roll:R}")).Placement(32).Rotation;
            var entity = Point("info_null", ("angle", "0"));

            entity.Place(new LevelPlacement(Vector3.Zero, wanted), 32);

            var back = entity.Placement(32).Rotation;
            Assert.True(Math.Abs(Quaternion.Dot(wanted, back)) > 1 - 1e-6, $"{pitch} {yaw} {roll} read back as {entity["angles"] ?? entity["angle"]}");
        }
    }

    [Fact]
    public void EveryChannelOfTheFilesColorIsWrittenBackAsTheNumberItWasRead()
    {
        for (var value = 0; value < 256; value++)
        {
            var light = Point("light", ("_color", "255 255 255"));

            light.SetLight(light.Light(32)! with { Color = LevelLight.Linear(value, 255 - value, value / 2) }, 32);

            Assert.Equal($"{value} {255 - value} {value / 2}", light["_color"]);
        }
    }
}
