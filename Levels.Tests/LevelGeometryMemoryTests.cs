using Xunit;

namespace Levels.Tests;

/// <summary>
/// A large level's brushes built into polygons, meshes and hulls in a few kilobytes a brush, most
/// of them what is given out.
/// </summary>
public sealed class LevelGeometryMemoryTests
{
    // Bytes a brush. A box's six polygons of four corners are some 800 bytes, its meshes' four
    // arrays some 900 and their lists' growth as much again, and its hull's eight corners some 120,
    // so each bound leaves the work itself a kilobyte or two.
    private const int Polygons = 3 * 1024;
    private const int Meshes = 8 * 1024;
    private const int Hulls = 4 * 1024;

    private static readonly LevelEntity World = Made();

    private static LevelEntity Made()
    {
        var world = new LevelEntity();
        world["classname"] = "worldspawn";
        for (var i = 0; i < 10_000; i++)
        {
            var low = new LevelPoint(i % 100 * 128, i / 100 * 128, 0);
            var high = low + new LevelPoint(64, 64, 32 + i % 7 * 16);
            world.Brushes.Add((i % 3) switch
            {
                0 => LevelBrush.Box(low, high, $"mat{i % 20}"),
                1 => LevelBrush.Wedge(low, high, $"mat{i % 20}"),
                _ => LevelBrush.Box(low, high, $"mat{i % 20}").Turned(30, low),
            });
        }

        return world;
    }

    private static long Allocated(Action step)
    {
        step();
        var before = GC.GetAllocatedBytesForCurrentThread();
        step();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private static void Within(string what, long bytes, int bound)
    {
        var each = (double)bytes / World.Brushes.Count;
        Assert.True(each <= bound, $"{what} allocated {each:F0} bytes a brush, past {bound}.");
    }

    [Fact]
    public void EveryBrushsPolygonsAreBuiltInAFewKilobytesABrush() =>
        Within("Polygons", Allocated(() => { foreach (var brush in World.Brushes) brush.Polygons(); }), Polygons);

    [Fact]
    public void TheMeshesAreBuiltInAFewKilobytesABrush() =>
        Within("Meshes", Allocated(() => LevelGeometry.Meshes(World, 32, default)), Meshes);

    [Fact]
    public void TheHullsAreBuiltInAFewKilobytesABrush() =>
        Within("Hulls", Allocated(() => LevelGeometry.Hulls(World, 32, default)), Hulls);
}
