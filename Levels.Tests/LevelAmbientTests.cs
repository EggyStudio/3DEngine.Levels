using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>The world's sky name and its ambient light, read from the worldspawn's keys.</summary>
public sealed class LevelAmbientTests
{
    private static LevelEntity World(params (string Key, string Value)[] keys)
    {
        var world = new LevelEntity();
        world["classname"] = "worldspawn";
        foreach (var (key, value) in keys) world[key] = value;
        return world;
    }

    [Fact]
    public void AWorldOfNoKeysHasNoSkyAndNoAmbientLight()
    {
        Assert.Equal(new LevelAmbient(null, Vector3.One, 0), World().Ambient());
        Assert.Equal(new LevelAmbient(null, Vector3.One, 0), World(("_sky", "")).Ambient());
    }

    [Fact]
    public void ItsSkyIsANameAndItsAmbientAColorAndALux()
    {
        var ambient = World(("_sky", "dusk"), ("_ambient", "255 128 10"), ("_ambient_lux", "40")).Ambient()!.Value;

        Assert.Equal("dusk", ambient.Sky);
        Assert.True(Vector3.Distance(LevelLight.Linear(255, 128, 10), ambient.Color) < 1e-6f);
        Assert.Equal(40, ambient.Lux);
    }

    [Fact]
    public void AnEntityOtherThanTheWorldHasNone()
    {
        var light = new LevelEntity();
        light["classname"] = "light";

        Assert.Null(light.Ambient());
    }

    [Theory]
    [InlineData("_ambient", "grey")]
    [InlineData("_ambient_lux", "-3")]
    public void AnAmbientThatDoesNotReadIsRefusedAndNamed(string key, string value)
    {
        var world = World((key, value));

        Assert.Throws<FormatException>(() => world.Ambient());
        Assert.Equal([key], new Level { Entities = { world } }.Check().Select(problem => problem.Key));
    }
}
