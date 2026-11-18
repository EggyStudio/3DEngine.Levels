using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// What a light's range means, the light falling as the inverse square of the distance and brought
/// to nothing at the range by glTF's window, held on the light's own numbers.
/// </summary>
public sealed class LevelLightRangeTests
{
    private static LevelLight Light(LevelLightKind kind, double lumens, double range, double outer = 0) =>
        new(kind, Vector3.Zero, -Vector3.UnitY, Vector3.One, lumens, range, 0, outer);

    // glTF's KHR_lights_punctual window, 1 minus (d over the range) to the fourth, held to 0 and 1.
    private static double Window(double meters, double range) =>
        Math.Clamp(1 - Math.Pow(meters / range, 4), 0, 1);

    [Theory]
    [InlineData(1)]
    [InlineData(2.5)]
    [InlineData(5)]
    [InlineData(9)]
    public void APointLightFallsAsTheSquareOfTheDistanceWithinItsWindow(double meters)
    {
        var bulb = Light(LevelLightKind.Point, LevelLight.DefaultLumens, LevelLight.DefaultRange);

        // 800 lumens over the whole sphere are 800 over 4 pi candelas, and a candela gives a lux at
        // a meter.
        var expected = LevelLight.DefaultLumens / (4 * Math.PI) / (meters * meters) * Window(meters, LevelLight.DefaultRange);
        Assert.Equal(expected, bulb.IlluminanceAt(meters), 1e-9);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(12)]
    public void AtItsRangeAndPastItALightGivesNothing(double meters)
    {
        Assert.Equal(0, Light(LevelLightKind.Point, 800, 10).IlluminanceAt(meters));
        Assert.Equal(0, Light(LevelLightKind.Spot, 800, 10, 30).IlluminanceAt(meters));
    }

    [Fact]
    public void ASpotsLumensOverItsOuterConeGiveItsCandelasAlongItsAxis()
    {
        var spot = Light(LevelLightKind.Spot, 800, 10, LevelLight.DefaultOuterCone);
        var cone = 2 * Math.PI * (1 - Math.Cos(LevelLight.DefaultOuterCone * Math.PI / 180));

        Assert.Equal(800 / cone / 4 * Window(2, 10), spot.IlluminanceAt(2), 1e-9);
    }

    [Fact]
    public void TheSunGivesItsLuxAtAnyDistance()
    {
        var sun = Light(LevelLightKind.Directional, LevelLight.DefaultLux, 0);

        Assert.Equal(LevelLight.DefaultLux, sun.IlluminanceAt(0));
        Assert.Equal(LevelLight.DefaultLux, sun.IlluminanceAt(1e6));
    }

    [Fact]
    public void ALightNearerThanACentimeterGivesWhatItGivesAtOne()
    {
        var bulb = Light(LevelLightKind.Point, 800, 10);

        Assert.Equal(bulb.IlluminanceAt(0.01), bulb.IlluminanceAt(0));
    }
}
