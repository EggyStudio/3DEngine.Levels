using Xunit;

namespace Levels.Tests;

/// <summary>
/// A level read from text that is not whole or not right, refused by a named exception and never
/// read without end.
/// </summary>
public sealed class LevelInputTests
{
    private static readonly string Written = Level.Parse(LevelTests.Room).Write();

    // Reads a text as an engine would, the level, then every brush's polygons, meshes and hulls,
    // and gives the exception's type where one is thrown.
    private static Type? Fault(string text)
    {
        try
        {
            var level = Level.Parse(text, "the text");
            foreach (var entity in level.Entities)
            {
                foreach (var brush in entity.Brushes) brush.Polygons();
                LevelGeometry.Meshes(entity, level.UnitsPerMeter, default);
                LevelGeometry.Hulls(entity, level.UnitsPerMeter, default);
            }

            return null;
        }
        catch (Exception fault)
        {
            return fault.GetType();
        }
    }

    [Fact]
    public void AFileCutShortAnywhereIsReadOrRefusedAsMapText()
    {
        for (var end = 0; end < Written.Length; end++)
        {
            var fault = Fault(Written[..end]);
            Assert.True(fault is null || fault == typeof(FormatException) || fault == typeof(InvalidOperationException),
                $"cut at {end}, {fault}");
        }
    }

    [Fact]
    public void AFileWithACharacterChangedIsReadOrRefusedAsMapText()
    {
        var random = new Random(7);
        char[] marks = ['{', '}', '(', ')', '[', ']', '"', '\\', '/', ' ', '\n', '-', 'e', '9', '.'];
        for (var trial = 0; trial < 3000; trial++)
        {
            var text = Written.ToCharArray();
            for (var change = 0; change < 1 + trial % 3; change++)
                text[random.Next(text.Length)] = marks[random.Next(marks.Length)];

            var fault = Fault(new string(text));
            Assert.True(fault is null || fault == typeof(FormatException) || fault == typeof(InvalidOperationException),
                $"trial {trial}, {fault}");
        }
    }

    [Theory]
    [InlineData("1e999")]
    [InlineData("-1e400")]
    [InlineData("Infinity")]
    [InlineData("NaN")]
    [InlineData("1" + "0000000000000000000000000000000000000000000000000000000000000000000000000000000"
        + "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
        + "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
        + "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    public void ANumberOfNoEndIsRefused(string number)
    {
        var text = $"{{\n{{\n( 0 0 {number} ) ( 0 1 0 ) ( 0 0 1 ) floor [ 0 1 0 0 ] [ 0 0 -1 0 ] 0 1 1\n}}\n}}";

        var fault = Assert.Throws<FormatException>(() => Level.Parse(text, "broken.map"));
        Assert.Contains("line 3", fault.Message);
    }

    [Fact]
    public void ABrushTooFarOutForItsPlanesToMeetIsRefused()
    {
        var far = LevelBrush.Box(new LevelPoint(1e200, 1e200, 1e200), new LevelPoint(2e200, 2e200, 2e200), "floor");

        Assert.Throws<InvalidOperationException>(() => far.Polygons());
    }

    [Fact]
    public void AFaceOfThreePointsOnALineIsRefusedByItsNumber()
    {
        var brush = LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "floor");
        brush.Faces[2] = brush.Faces[2] with { Third = brush.Faces[2].First * 2 - brush.Faces[2].Second };

        var fault = Assert.Throws<InvalidOperationException>(() => brush.Polygons());
        Assert.Contains("face 2", fault.Message);
    }

    [Fact]
    public void ABrushOfNoVolumeIsRefused()
    {
        // A box whose top is moved down onto its floor, the two planes one, which encloses a
        // rectangle and no solid.
        var brush = LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "floor");
        var top = brush.Faces.FindIndex(face => face.Normal.Z > 0.5);
        var down = new LevelPoint(0, 0, 32);
        brush.Faces[top] = brush.Faces[top] with
        {
            First = brush.Faces[top].First - down,
            Second = brush.Faces[top].Second - down,
            Third = brush.Faces[top].Third - down,
        };

        Assert.Throws<InvalidOperationException>(() => brush.Polygons());
        var world = new LevelEntity();
        world.Brushes.Add(brush);
        Assert.Throws<InvalidOperationException>(() => LevelGeometry.Hulls(world, 32, default));
    }
}
