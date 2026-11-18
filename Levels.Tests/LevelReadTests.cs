using Xunit;

namespace Levels.Tests;

/// <summary>
/// A level's entities handed to an engine one by one, a fault the level's text causes named by the
/// entity it stands in.
/// </summary>
public sealed class LevelReadTests
{
    [Fact]
    public void EachEntityIsHandedOnInTheLevelsOrderWithItsIndex()
    {
        var level = Level.Parse(LevelTests.Room);
        var read = new List<(int, string)>();

        level.ReadEntities((index, entity) => read.Add((index, entity.ClassName)));

        Assert.Equal(level.Entities.Select((entity, index) => (index, entity.ClassName)), read);
    }

    [Fact]
    public void AMalformedKeyIsRefusedByTheEntityItStandsIn()
    {
        var level = Level.Parse(LevelTests.Room);
        level.Entities[1]["origin"] = "0 0";

        var fault = Assert.Throws<FormatException>(() => level.ReadEntities((_, entity) => entity.Placement(level.UnitsPerMeter)));

        Assert.StartsWith("The level's entity 1, info_player_start, cannot be read. ", fault.Message);
        Assert.Contains("the origin 0 0", fault.Message);
        Assert.IsType<FormatException>(fault.InnerException);
    }

    [Fact]
    public void ABrushThatClosesNoSolidIsRefusedByTheEntityItStandsIn()
    {
        var level = Level.Parse(LevelTests.Room);
        var door = level.Entities.FindIndex(entity => entity.ClassName == "func_door");
        level.Entities[door].Brushes[0].Faces.RemoveAt(0);

        var fault = Assert.Throws<FormatException>(() =>
            level.ReadEntities((_, entity) => LevelGeometry.Meshes(entity, level.UnitsPerMeter, default)));

        Assert.StartsWith($"The level's entity {door}, door, cannot be read. ", fault.Message);
        Assert.IsType<InvalidOperationException>(fault.InnerException);
    }

    [Fact]
    public void AnEnginesOwnFaultGoesOnAsItIs()
    {
        var level = Level.Parse(LevelTests.Room);

        Assert.Throws<NotSupportedException>(() => level.ReadEntities((_, _) => throw new NotSupportedException()));
    }
}
