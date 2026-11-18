using System.Globalization;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// A level written and read back whole, and a level map text cannot hold refused rather than
/// written as text that reads back as another.
/// </summary>
public sealed class LevelWritingTests
{
    [Fact]
    public void ALevelOfEveryShapeWrittenReadsBackAsItWas()
    {
        var level = new Level { Game = "Generic" };
        var world = new LevelEntity();
        world["classname"] = "worldspawn";
        world["message"] = "A \"quoted\" word and a back\\slash";
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(-64, -64, -16), new LevelPoint(64, 64, 0), "proto/floor"));
        world.Brushes.Add(LevelBrush.Wedge(new LevelPoint(0, 0, 0), new LevelPoint(64, 32, 48), "proto/wall", 0.25));
        world.Brushes.AddRange(Level.Doorway(new LevelPoint(-96, 64, 0), new LevelPoint(96, 80, 128), 64, 96, "wall"));
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(16, 16, 16), "crate").Turned(30, default));
        level.Entities.Add(world);
        var door = new LevelEntity();
        door.Keys.Add(new("classname", "func_door"));
        door.Keys.Add(new("OnOpen", "a"));
        door.Keys.Add(new("OnOpen", "b"));
        door.Keys.Add(new("note", "two\nlines"));
        level.Entities.Add(door);

        var read = Level.Parse(level.Write());

        Assert.Equal(level.Game, read.Game);
        Assert.Equal(level.Entities.Count, read.Entities.Count);
        for (var e = 0; e < level.Entities.Count; e++)
        {
            Assert.Equal(level.Entities[e].Keys, read.Entities[e].Keys);
            Assert.Equal(level.Entities[e].Brushes.Count, read.Entities[e].Brushes.Count);
            for (var b = 0; b < level.Entities[e].Brushes.Count; b++)
                Assert.Equal(level.Entities[e].Brushes[b].Faces, read.Entities[e].Brushes[b].Faces);
        }

        Assert.Equal(level.Write(), read.Write());
    }

    public static TheoryData<string, Action<Level>> Unwritable() => new()
    {
        { "a value ending in a backslash", level => level.Entities[0]["path"] = "C:\\maps\\" },
        { "a key ending in a backslash", level => level.Entities[0]["key\\"] = "1" },
        { "a material with a space", level => Retexture(level, "my wall") },
        { "an empty material", level => Retexture(level, "") },
        { "a material read as a comment", level => Retexture(level, "//wall") },
        { "a material with a bracket", level => Retexture(level, "wall[") },
        { "a material with a quote", level => Retexture(level, "wall\"") },
        { "a scale of no number", level => Rescale(level, double.NaN) },
        { "a scale of no end", level => Rescale(level, double.PositiveInfinity) },
        { "a game of two lines", level => level.Game = "Generic\n{" },
    };

    private static void Retexture(Level level, string material)
    {
        var brush = level.Entities[0].Brushes[0];
        brush.Faces[0] = brush.Faces[0] with { Material = material };
    }

    private static void Rescale(Level level, double scale)
    {
        var brush = level.Entities[0].Brushes[0];
        brush.Faces[0] = brush.Faces[0] with { ScaleU = scale };
    }

    [Theory]
    [MemberData(nameof(Unwritable))]
    public void ALevelMapTextCannotHoldIsRefusedRatherThanWrittenWrong(string what, Action<Level> spoil)
    {
        var level = Level.Parse(LevelTests.Room);
        spoil(level);

        var fault = Assert.Throws<InvalidOperationException>(level.Write);
        Assert.False(string.IsNullOrEmpty(fault.Message), what);
    }

    [Fact]
    public void ANumberIsWrittenAsTheShortestTextThatReadsBackTheSame()
    {
        var random = new Random(11);
        for (var trial = 0; trial < 10_000; trial++)
        {
            var value = (random.NextDouble() - 0.5) * Math.Pow(10, random.Next(-12, 12));
            var text = LevelPoint.Number(value);
            Assert.Equal(value, double.Parse(text, CultureInfo.InvariantCulture));
        }

        Assert.Equal("0", LevelPoint.Number(-0.0));
    }
}
