using Xunit;

namespace Levels.Tests;

/// <summary>
/// An entity's outputs, keyvalues of five fields an event sends another entity's input by, and the
/// entities a target name names.
/// </summary>
public sealed class LevelOutputsTests
{
    private static LevelEntity Entity(string className, params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        entity["classname"] = className;
        foreach (var (key, value) in keys) entity.Keys.Add(new(key, value));
        return entity;
    }

    [Fact]
    public void AnOutputIsAKeyvalueOfTargetInputParameterDelayAndTimes()
    {
        var door = Level.Parse(LevelTests.Room).Entities.Single(entity => entity.ClassName == "func_door");

        Assert.Equal(
            [new LevelOutput("OnOpen", "lamp", "TurnOn", "", 0, -1), new LevelOutput("OnOpen", "chime", "Play", "", 0.5, 1)],
            door.Outputs());
    }

    [Fact]
    public void AnOutputsFieldsMayBeSplitByTheEscapeCharacter()
    {
        var button = Entity("func_button", ("OnPressed", "door\u001bOpen\u001bfast\u001b1.5\u001b2"));

        Assert.Equal([new LevelOutput("OnPressed", "door", "Open", "fast", 1.5, 2)], button.Outputs());
    }

    [Fact]
    public void AKeyvalueOfOtherThanFiveFieldsIsNoOutput()
    {
        var light = Entity("light", ("origin", "0 0 64"), ("_color", "255 255 255"), ("message", "a, b, c"), ("note", "one,two,three,four,five,six"));

        Assert.Empty(light.Outputs());
    }

    [Theory]
    [InlineData("lamp,TurnOn,,soon,-1", "soon")]
    [InlineData("lamp,TurnOn,,0,twice", "twice")]
    public void AnOutputWhoseDelayOrTimesDoesNotReadIsRefusedByItsKey(string value, string bad)
    {
        var door = Entity("func_door", ("OnOpen", value));

        var fault = Assert.Throws<FormatException>(() => door.Outputs());
        Assert.Contains("OnOpen", fault.Message);
        Assert.Contains(bad, fault.Message);
    }

    [Fact]
    public void ATargetNameNamesTheEntitiesThatBearItInTheLevelsOrder()
    {
        var level = new Level();
        level.Entities.Add(Entity("light", ("targetname", "lamp")));
        level.Entities.Add(Entity("light", ("targetname", "lamps")));
        level.Entities.Add(Entity("light_spot", ("targetname", "lamp")));

        Assert.Equal(["light", "light_spot"], level.Targeted("lamp").Select(entity => entity.ClassName));
        Assert.Empty(level.Targeted("Lamp"));
        Assert.Empty(level.Targeted("!self"));
    }
}
