using Xunit;

namespace Levels.Tests;

/// <summary>A point's arithmetic, which a binding reads a face's plane with.</summary>
public sealed class LevelPointTests
{
    [Fact]
    public void TheCrossProductIsRightHandedInTheFilesAxes()
    {
        Assert.Equal(new LevelPoint(0, 0, 1), LevelPoint.Cross(new LevelPoint(1, 0, 0), new LevelPoint(0, 1, 0)));
        Assert.Equal(new LevelPoint(-3, 6, -3), LevelPoint.Cross(new LevelPoint(1, 2, 3), new LevelPoint(4, 5, 6)));
    }

    [Fact]
    public void TheDotProductAndTheLengthAreTheFilesUnits()
    {
        Assert.Equal(32, LevelPoint.Dot(new LevelPoint(1, 2, 3), new LevelPoint(4, 5, 6)));
        Assert.Equal(13, new LevelPoint(3, 4, 12).Length);
    }

    [Fact]
    public void AFacesNormalIsItsPointsCrossProductAtALengthOfOne()
    {
        var face = LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "floor").Faces[0];
        var cross = LevelPoint.Cross(face.First - face.Second, face.Third - face.Second);

        Assert.Equal(cross * (1 / cross.Length), face.Normal);
    }
}
