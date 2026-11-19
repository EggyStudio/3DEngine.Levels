using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// A trigger's volume, the hulls of its brushes whatever their material, and whether a place stands
/// in it, so every engine fires a trigger alike.
/// </summary>
public sealed class LevelVolumeTests
{
    // A trigger of two boxes of trigger faces, 0 to 64 and 64 to 128 along X, 64 deep and 64 high,
    // at 32 units a meter.
    private static LevelEntity Trigger()
    {
        var trigger = new LevelEntity();
        trigger["classname"] = "trigger_once";
        trigger.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 64), LevelTools.Trigger));
        trigger.Brushes.Add(LevelBrush.Box(new LevelPoint(64, 0, 0), new LevelPoint(128, 64, 64), LevelTools.Trigger));
        return trigger;
    }

    [Fact]
    public void ATriggersVolumeIsAHullABrushThoughItIsNoBody()
    {
        var volume = LevelGeometry.Volume(Trigger(), 32, default);

        Assert.Equal(2, volume.Count);
        Assert.All(volume, hull => Assert.Equal(8, hull.Corners.Length));
        Assert.Empty(LevelGeometry.Hulls(Trigger(), 32, default));
    }

    [Theory]
    [InlineData(1, 1, -1, true)]
    [InlineData(3, 1, -1, true)]
    [InlineData(2, 1, -1, true)]
    [InlineData(5, 1, -1, false)]
    [InlineData(1, 3, -1, false)]
    [InlineData(1, 1, 1, false)]
    public void APlaceInMetersIsInsideWhereItStandsInAnyOfItsBrushes(float x, float y, float z, bool inside)
    {
        Assert.Equal(inside, LevelGeometry.InsideVolume(Trigger(), new Vector3(x, y, z), 32));
    }

    [Fact]
    public void AnEntityOfNoBrushesHasNoVolume()
    {
        var point = new LevelEntity();
        point["classname"] = "info_null";

        Assert.Empty(LevelGeometry.Volume(point, 32, default));
        Assert.False(LevelGeometry.InsideVolume(point, Vector3.Zero, 32));
    }
}
