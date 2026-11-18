using Xunit;

namespace Levels.Tests;

/// <summary>An entity's name, which both engines and their consoles show alike.</summary>
public sealed class LevelNameTests
{
    [Fact]
    public void AnEntityIsCalledByItsTargetNameElseItsClassElseItsIndex()
    {
        var level = Level.Parse(LevelTests.Room);
        var door = level.Entities.Single(entity => entity.ClassName == "func_door");
        var nameless = new LevelEntity();

        Assert.Equal("door", door.TargetName);
        Assert.Equal("door", door.Name(2));
        Assert.Equal("", level.World!.TargetName);
        Assert.Equal("worldspawn", level.World.Name(0));
        Assert.Equal("entity 7", nameless.Name(7));
    }

    [Fact]
    public void AnEmptyTargetNameGivesWayToTheClass()
    {
        var light = new LevelEntity();
        light["classname"] = "light";
        light["targetname"] = "";

        Assert.Equal("light", light.Name(3));
    }
}
