using Xunit;

namespace Levels.Tests;

/// <summary>
/// A ray against a level's brushes, which a test or a game reads alike in either engine.
/// </summary>
public sealed class LevelHitTests
{
    private static readonly LevelBrush Floor = LevelBrush.Box(new LevelPoint(-128, -128, -16), new LevelPoint(128, 128, 0), "proto/floor");

    [Fact]
    public void ARayDownMeetsTheFloorsTop()
    {
        Assert.Equal(64, Floor.Hit(new LevelPoint(0, 0, 64), new LevelPoint(0, 0, -1), 1000));
        Assert.Equal(32, Floor.Hit(new LevelPoint(0, 0, 64), new LevelPoint(0, 0, -2), 1000));
    }

    [Fact]
    public void ARayThatPassesBesideOrStopsShortMissesIt()
    {
        Assert.Null(Floor.Hit(new LevelPoint(200, 0, 64), new LevelPoint(0, 0, -1), 1000));
        Assert.Null(Floor.Hit(new LevelPoint(0, 0, 64), new LevelPoint(0, 0, -1), 63));
        Assert.Null(Floor.Hit(new LevelPoint(0, 0, 64), new LevelPoint(0, 0, 1), 1000));
    }

    [Fact]
    public void ARayAlongAPlaneOutsideItMissesAndOneInsideMeetsAtOnce()
    {
        Assert.Null(Floor.Hit(new LevelPoint(-500, 0, 8), new LevelPoint(1, 0, 0), 1000));
        Assert.Equal(0, Floor.Hit(new LevelPoint(0, 0, -8), new LevelPoint(1, 0, 0), 1000));
    }

    [Fact]
    public void ARayMeetsATurnedBrushOnItsSlope()
    {
        var ramp = LevelBrush.Ramp(new LevelPoint(0, 0, 0), new LevelPoint(128, 0, 0), 64, 64, "proto/stairs");

        var at = ramp.Hit(new LevelPoint(64, 0, 200), new LevelPoint(0, 0, -1), 1000);

        Assert.NotNull(at);
        Assert.True(Math.Abs(at!.Value - (200 - 32)) < 1e-6, $"{at}");
    }

    [Fact]
    public void TheNearestOfManyBrushesIsMet()
    {
        var box = LevelBrush.Box(new LevelPoint(-16, -16, 0), new LevelPoint(16, 16, 32), "crate");

        Assert.Equal(32, LevelGeometry.Hit([Floor, box], new LevelPoint(0, 0, 64), new LevelPoint(0, 0, -1), 1000));
        Assert.Null(LevelGeometry.Hit([Floor, box], new LevelPoint(500, 0, 64), new LevelPoint(0, 0, -1), 1000));
    }

    [Fact]
    public void ARayOfNoWayOrABackwardReachIsRefused()
    {
        Assert.Throws<ArgumentException>(() => Floor.Hit(default, default, 10));
        Assert.Throws<ArgumentException>(() => Floor.Hit(default, new LevelPoint(1, 0, 0), -1));
    }
}
