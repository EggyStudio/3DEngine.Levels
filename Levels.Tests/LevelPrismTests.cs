using Xunit;

namespace Levels.Tests;

/// <summary>A brush of any convex outline across Y, as a level is blocked out in code.</summary>
public sealed class LevelPrismTests
{
    private static readonly (double X, double Z)[] Hexagon =
        [(64, 0), (32, 55), (-32, 55), (-64, 0), (-32, -55), (32, -55)];

    [Fact]
    public void AnOutlineIsABrushOfAFaceASideAndItsTwoEnds()
    {
        var brush = LevelBrush.Prism(Hexagon, 0, 32, "proto/wall");
        var polygons = brush.Polygons();

        Assert.Equal(8, brush.Faces.Count);
        Assert.Equal([4, 4, 4, 4, 4, 4, 6, 6], polygons.Select(polygon => polygon!.Length).Order());
    }

    [Fact]
    public void EitherWindingAndEitherOrderOfTheEndsMakeOneBrush()
    {
        var one = LevelBrush.Prism(Hexagon, 0, 32, "wall");
        var other = LevelBrush.Prism([.. Hexagon.Reverse()], 32, 0, "wall");

        static IEnumerable<LevelPoint> Corners(LevelBrush brush) =>
            brush.Polygons().SelectMany(polygon => polygon!).Distinct().OrderBy(point => (point.X, point.Y, point.Z));
        Assert.Equal(Corners(one), Corners(other));
    }

    [Fact]
    public void AnOutlineThatIsNotConvexIsRefused()
    {
        (double X, double Z)[] notch = [(0, 0), (64, 0), (64, 64), (32, 16), (0, 64)];

        Assert.Throws<ArgumentException>(() => LevelBrush.Prism(notch, 0, 32, "wall"));
    }

    [Theory]
    [InlineData(16, 16)]
    [InlineData(0, double.NaN)]
    public void APrismOfNoDepthIsRefused(double from, double to)
    {
        Assert.Throws<ArgumentException>(() => LevelBrush.Prism(Hexagon, from, to, "wall"));
    }

    [Fact]
    public void AnOutlineOfPointsOnOneLineIsRefused()
    {
        Assert.Throws<ArgumentException>(() => LevelBrush.Prism([(0, 0), (32, 0), (64, 0)], 0, 32, "wall"));
    }
}
