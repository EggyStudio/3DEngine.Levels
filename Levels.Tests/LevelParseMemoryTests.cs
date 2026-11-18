using Xunit;

namespace Levels.Tests;

/// <summary>
/// A level's text read with no copy of it held as tokens, so a large level's reading allocates a
/// few bytes a character, most of them the level it reads.
/// </summary>
public sealed class LevelParseMemoryTests
{
    // Bytes a character of the text, the level it is read into among them. A level of 10,000
    // brushes keeps some 2 bytes a character of faces, points and lists, as the bench's row for the
    // level Level.Parse keeps reads (dotnet run -c Release --project build/bench), so a bound of 4
    // leaves the reading itself under 2.
    private const double BytesACharacter = 4;

    [Fact]
    public void ALevelOfTenThousandBrushesIsReadInAFewBytesACharacter()
    {
        var level = new Level();
        var world = new LevelEntity();
        world["classname"] = "worldspawn";
        for (var i = 0; i < 10_000; i++)
        {
            var low = new LevelPoint(i % 100 * 128, i / 100 * 128, 0);
            world.Brushes.Add(LevelBrush.Box(low, low + new LevelPoint(64, 64, 32 + i % 7 * 16), $"mat{i % 20}"));
        }

        level.Entities.Add(world);
        var text = level.Write();
        Level.Parse(text);

        var before = GC.GetAllocatedBytesForCurrentThread();
        var read = Level.Parse(text);
        var bytes = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(10_000, read.World!.Brushes.Count);
        Assert.True(bytes <= BytesACharacter * text.Length,
            $"Reading {text.Length:N0} characters allocated {bytes:N0} bytes, {(double)bytes / text.Length:F1} a character, past {BytesACharacter}.");
    }
}
