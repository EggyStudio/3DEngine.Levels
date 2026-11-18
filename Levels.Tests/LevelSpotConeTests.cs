using System.Numerics;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// A spot's light off its axis, full inside its inner cone, nothing past its outer cone, and
/// between the two the square of a ramp in the cosine of the angle, as glTF's KHR_lights_punctual
/// states it.
/// </summary>
public sealed class LevelSpotConeTests
{
    private static LevelLight Spot(double inner, double outer) =>
        new(LevelLightKind.Spot, Vector3.Zero, -Vector3.UnitY, Vector3.One, 800, 10, inner, outer);

    private static double Cos(double degrees) => Math.Cos(degrees * Math.PI / 180);

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(25)]
    public void InsideTheInnerConeASpotGivesWhatItGivesOnItsAxis(double degrees)
    {
        var spot = Spot(LevelLight.DefaultInnerCone, LevelLight.DefaultOuterCone);

        Assert.Equal(spot.IlluminanceAt(2), spot.IlluminanceAt(2, degrees), 1e-9);
    }

    [Theory]
    [InlineData(26)]
    [InlineData(27.5)]
    [InlineData(29.5)]
    public void BetweenTheConesItFallsAsTheSquareOfARampInTheCosine(double degrees)
    {
        var spot = Spot(LevelLight.DefaultInnerCone, LevelLight.DefaultOuterCone);
        var ramp = (Cos(degrees) - Cos(30)) / (Cos(25) - Cos(30));

        Assert.Equal(spot.IlluminanceAt(2) * ramp * ramp, spot.IlluminanceAt(2, degrees), 1e-9);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(45)]
    [InlineData(180)]
    public void AtItsOuterConeAndPastItASpotGivesNothing(double degrees)
    {
        Assert.Equal(0, Spot(LevelLight.DefaultInnerCone, LevelLight.DefaultOuterCone).IlluminanceAt(2, degrees), 1e-12);
    }

    [Fact]
    public void ASpotOfOneConeHasAnEdgeAThousandthOfACosineWide()
    {
        var spot = Spot(30, 30);

        // At 30 degrees a thousandth of a cosine is some 0.11 degrees, so the light is full at 29.8
        // and gone at the cone.
        Assert.Equal(spot.IlluminanceAt(2), spot.IlluminanceAt(2, 29.8), 1e-9);
        Assert.True(spot.IlluminanceAt(2, 29.95) < spot.IlluminanceAt(2));
        Assert.Equal(0, spot.IlluminanceAt(2, 30), 1e-12);
    }

    [Fact]
    public void APointLightAndTheSunAreTheSameEveryWay()
    {
        var bulb = new LevelLight(LevelLightKind.Point, Vector3.Zero, -Vector3.UnitZ, Vector3.One, 800, 10, 0, 0);
        var sun = new LevelLight(LevelLightKind.Directional, Vector3.Zero, -Vector3.UnitY, Vector3.One, 100_000, 0, 0, 0);

        Assert.Equal(bulb.IlluminanceAt(2), bulb.IlluminanceAt(2, 120));
        Assert.Equal(sun.IlluminanceAt(2), sun.IlluminanceAt(2, 60));
    }
}
