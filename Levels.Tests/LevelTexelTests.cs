using Xunit;

namespace Levels.Tests;

/// <summary>
/// Which way a face's texture coordinates run, a picture's top left at (0, 0) and V growing down
/// it, as TrenchBroom draws a material, so an engine that samples from the bottom left knows to
/// turn V over.
/// </summary>
public sealed class LevelTexelTests
{
    private static readonly LevelBrush Box = LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "wall", 1);

    private static LevelFace Facing(double x, double y, double z) =>
        Box.Faces.Single(face => (face.Normal - new LevelPoint(x, y, z)).Length < 1e-9);

    [Fact]
    public void OnAWallUGrowsAlongItAndVGrowsDownIt()
    {
        // The wall facing minus Y, seen from outside with X to the right.
        var wall = Facing(0, -1, 0);
        var topLeft = LevelGeometry.Texel(wall, new LevelPoint(0, 0, 32), 32, 32);
        var topRight = LevelGeometry.Texel(wall, new LevelPoint(64, 0, 32), 32, 32);
        var bottomLeft = LevelGeometry.Texel(wall, new LevelPoint(0, 0, 0), 32, 32);

        Assert.Equal(2, topRight.X - topLeft.X, 5);
        Assert.Equal(1, bottomLeft.Y - topLeft.Y, 5);
    }

    [Fact]
    public void OnAFloorSeenFromAboveNorthIsThePicturesTop()
    {
        // The floor's top, seen from above with X to the right and the file's Y, north, up.
        var top = Facing(0, 0, 1);
        var north = LevelGeometry.Texel(top, new LevelPoint(0, 64, 32), 32, 32);
        var south = LevelGeometry.Texel(top, new LevelPoint(0, 0, 32), 32, 32);
        var east = LevelGeometry.Texel(top, new LevelPoint(64, 64, 32), 32, 32);

        Assert.Equal(2, south.Y - north.Y, 5);
        Assert.Equal(2, east.X - north.X, 5);
    }
}
