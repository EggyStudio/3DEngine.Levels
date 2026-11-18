using Xunit;

namespace Levels.Tests;

/// <summary>
/// The shapes placed by points on the ground, a slab of an outline, a wall and a ramp between two
/// points, and any brush made along X put along a line.
/// </summary>
public sealed class LevelPlacedShapesTests
{
    private static List<LevelPoint> Corners(LevelBrush brush) =>
        [.. brush.Polygons().SelectMany(polygon => polygon!).Distinct().OrderBy(point => (point.X, point.Y, point.Z))];

    private static void Same(LevelBrush expected, LevelBrush actual)
    {
        var (a, b) = (Corners(expected), Corners(actual));
        Assert.Equal(a.Count, b.Count);
        for (var i = 0; i < a.Count; i++) Assert.True((a[i] - b[i]).Length < 1e-6, $"{b[i]} where {a[i]} is meant");
    }

    [Fact]
    public void ASlabIsAGroundOutlineStretchedUp()
    {
        var slab = LevelBrush.Slab([(0, 0), (128, 0), (128, 64), (0, 64)], 0, -16, "proto/floor");

        Same(LevelBrush.Box(new LevelPoint(0, 0, -16), new LevelPoint(128, 64, 0), "proto/floor"), slab);
        Assert.Equal(6, slab.Polygons().Length);
    }

    [Fact]
    public void ASlabOfAnOutlineThatBendsInIsRefused()
    {
        Assert.Throws<ArgumentException>(() =>
            LevelBrush.Slab([(0, 0), (64, 0), (64, 64), (32, 16), (0, 64)], 0, 16, "proto/floor"));
    }

    [Fact]
    public void AWallStandsCenteredOnTheLineBetweenItsTwoPoints()
    {
        var north = LevelBrush.Wall(new LevelPoint(0, 0, 8), new LevelPoint(0, 128, 99), 96, 16, "proto/wall");

        Same(LevelBrush.Box(new LevelPoint(-8, 0, 8), new LevelPoint(8, 128, 104), "proto/wall"), north);
    }

    [Fact]
    public void AWallOnADiagonalHasItsCornersWithinItsThicknessOfTheLine()
    {
        var wall = LevelBrush.Wall(new LevelPoint(0, 0, 0), new LevelPoint(96, 96, 0), 64, 16, "proto/wall");
        var corners = Corners(wall);
        var length = Math.Sqrt(2) * 96;

        Assert.Equal(8, corners.Count);
        Assert.All(corners, corner =>
        {
            var along = (corner.X + corner.Y) / Math.Sqrt(2);
            var across = (corner.Y - corner.X) / Math.Sqrt(2);
            Assert.True(Math.Abs(Math.Abs(across) - 8) < 1e-5 && (Math.Abs(along) < 1e-5 || Math.Abs(along - length) < 1e-5));
            Assert.True(Math.Abs(corner.Z) < 1e-9 || Math.Abs(corner.Z - 64) < 1e-9);
        });
    }

    [Fact]
    public void ARampRisesFromItsFootAtTheFirstPointToItsTopAtTheSecond()
    {
        var ramp = LevelBrush.Ramp(new LevelPoint(0, 0, 16), new LevelPoint(128, 0, 16), 64, 32, "proto/stairs");

        Same(LevelBrush.Wedge(new LevelPoint(0, -32, 16), new LevelPoint(128, 32, 48), "proto/stairs"), ramp);

        var back = LevelBrush.Ramp(new LevelPoint(128, 0, 0), new LevelPoint(0, 0, 0), 64, 32, "proto/stairs");
        Assert.Contains(Corners(back), corner => corner.X == 0 && corner.Z == 32);
        Assert.DoesNotContain(Corners(back), corner => corner.X == 128 && corner.Z == 32);
    }

    [Fact]
    public void ADoorwayMadeAlongXIsPutAlongALine()
    {
        var doorway = Level.Doorway(new LevelPoint(0, -8, 0), new LevelPoint(192, 8, 160), 64, 128, "proto/wall")
            .Select(brush => brush.Along(new LevelPoint(64, 64, 0), new LevelPoint(64, 512, 0)))
            .ToList();

        Assert.All(doorway, brush => Assert.All(Corners(brush), corner =>
            Assert.True(corner.X >= 56 - 1e-6 && corner.X <= 72 + 1e-6 && corner.Y >= 64 - 1e-6 && corner.Y <= 256 + 1e-6)));
    }

    [Fact]
    public void TwoPointsThatAreOneOnTheGroundAreRefused()
    {
        Assert.Throws<ArgumentException>(() => LevelBrush.Wall(new LevelPoint(5, 5, 0), new LevelPoint(5, 5, 64), 64, 16, "proto/wall"));
        Assert.Throws<ArgumentException>(() =>
            LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(8, 8, 8), "x").Along(new LevelPoint(1, 1, 0), new LevelPoint(1, 1, 0)));
    }
}
