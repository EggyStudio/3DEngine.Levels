using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// A level's lights read from their classes' keys in physical units, as both engines make a light
/// of them, with what TrenchBroom is given to draw them.
/// </summary>
public sealed class LevelLightTests
{
    private const float Close = 1e-5f;

    private static LevelEntity Entity(params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        foreach (var (key, value) in keys) entity.Keys.Add(new(key, value));
        return entity;
    }

    private static void Near(Vector3 expected, Vector3 actual) =>
        Assert.True(Vector3.Distance(expected, actual) < Close, $"{actual} where {expected} is meant");

    [Fact]
    public void APointLightIsWhiteAtABulbsLumensWhereItsKeysAreMissing()
    {
        var light = Entity(("classname", "light"), ("origin", "64 -32 16")).Light(32)!;

        Assert.Equal(LevelLightKind.Point, light.Kind);
        Near(new Vector3(2, 0.5f, 1), light.Position);
        Assert.Equal(Vector3.One, light.Color);
        Assert.Equal(800, light.Intensity);
        Assert.Equal(10, light.Range);
        Assert.Equal(0, light.InnerCone);
        Assert.Equal(0, light.OuterCone);
    }

    [Fact]
    public void AColorIsDecodedFromSrgbByItsOwnCurve()
    {
        var light = Entity(("classname", "light"), ("_color", "255 128 10")).Light(32)!;

        // 128 of 255 is 0.21586 linear on sRGB's curve, and 10 of 255 falls on its straight part.
        Near(new Vector3(1, 0.2158605f, 10f / 255 / 12.92f), light.Color);
    }

    [Fact]
    public void ASpotFacesWhereItsArrowPointsWithItsCones()
    {
        var spot = Entity(("classname", "light_spot"), ("angles", "90 0 0"), ("intensity", "1200"),
            ("range", "5"), ("_inner_cone", "10"), ("_cone", "40")).Light(32)!;

        Assert.Equal(LevelLightKind.Spot, spot.Kind);
        Near(-Vector3.UnitY, spot.Direction);
        Assert.Equal(1200, spot.Intensity);
        Assert.Equal(5, spot.Range);
        Assert.Equal(10, spot.InnerCone);
        Assert.Equal(40, spot.OuterCone);

        var plain = Entity(("classname", "light_spot")).Light(32)!;
        Assert.Equal(25, plain.InnerCone);
        Assert.Equal(30, plain.OuterCone);
    }

    [Fact]
    public void TheSunShinesTheWayItsAnglesFaceAtANoonsLux()
    {
        var entity = Entity(("classname", "light_environment"), ("angles", "60 30 0"), ("range", "99"));
        var sun = entity.Light(32)!;

        Assert.Equal(LevelLightKind.Directional, sun.Kind);
        Assert.Equal(100_000, sun.Intensity);
        Assert.Equal(0, sun.Range);
        Near(Vector3.Transform(-Vector3.UnitZ, entity.Placement(32).Rotation), sun.Direction);
        Assert.True(sun.Direction.Y < 0, "A pitch above zero shines down.");
    }

    [Fact]
    public void AnEntityOfAnotherClassIsNoLight()
    {
        Assert.Null(Entity(("classname", "info_player_start")).Light(32));
        Assert.Null(new LevelEntity().Light(32));
    }

    [Theory]
    [InlineData("light", "intensity", "-1")]
    [InlineData("light", "range", "-2")]
    [InlineData("light", "_color", "255 255")]
    [InlineData("light_spot", "_cone", "-5")]
    public void AMalformedOrNegativeKeyIsRefused(string className, string key, string value)
    {
        var fault = Assert.Throws<FormatException>(() => Entity(("classname", className), (key, value)).Light(32));
        Assert.Contains($"the {key} {value}", fault.Message);
    }

    [Fact]
    public void AnInnerConeWiderThanTheOuterIsRefused()
    {
        Assert.Throws<FormatException>(() =>
            Entity(("classname", "light_spot"), ("_inner_cone", "40"), ("_cone", "30")).Light(32));
    }

    [Fact]
    public void TheFgdNamesTheThreeLightClasses()
    {
        var at = new DirectoryInfo(AppContext.BaseDirectory);
        while (at is not null && !File.Exists(Path.Combine(at.FullName, "Levels.slnx"))) at = at.Parent;
        var fgd = File.ReadAllText(Path.Combine(at!.FullName, "TrenchBroom", "Levels.fgd"));

        foreach (var name in new[] { LevelLight.PointClass, LevelLight.SpotClass, LevelLight.DirectionalClass })
            Assert.Contains($"= {name} :", fgd);
    }
}
