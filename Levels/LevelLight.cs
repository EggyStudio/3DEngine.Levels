using System.Numerics;

namespace Levels;

/// <summary>The kind of a level's light, by its class.</summary>
public enum LevelLightKind
{
    /// <summary>A <c>light</c>, sending its light every way from its place.</summary>
    Point,

    /// <summary>A <c>light_spot</c>, sending its light in a cone about the way it faces.</summary>
    Spot,

    /// <summary>
    /// A <c>light_environment</c>, the sun, its light falling the way it faces across the whole
    /// level, its place read by no engine.
    /// </summary>
    Directional,
}

/// <summary>
/// A light of a level in physical units, as <see cref="LevelEntity.Light"/> reads it from a light
/// class's keys, which both engines make a light of alike.
/// </summary>
/// <param name="Kind">The kind, by the entity's class.</param>
/// <param name="Position">The place, in meters with Y up.</param>
/// <param name="Direction">The way the light faces, a length of one, where the entity's arrow
/// points in TrenchBroom.</param>
/// <param name="Color">The color in linear sRGB, each channel from 0 to 1.</param>
/// <param name="Intensity">The strength, the lumens a point light sends over the whole sphere or a
/// spot over its outer cone, or the lux the sun gives a surface facing it.</param>
/// <param name="Range">How far in meters a point or a spot reaches, zero for the sun.</param>
/// <param name="InnerCone">A spot's full strength out to this angle in degrees from its
/// axis.</param>
/// <param name="OuterCone">A spot's light out to this angle in degrees from its axis, fading
/// from the inner one.</param>
/// <remarks>
/// A spot's lumens are those it sends over its outer cone, so a spot keeps its lumens as its cone
/// narrows and grows brighter along its axis. An engine whose spot takes the lumens of a point
/// light of the same candelas gives it the lumens times 2 over (1 minus the outer cone's cosine).
/// </remarks>
public sealed record LevelLight(
    LevelLightKind Kind,
    Vector3 Position,
    Vector3 Direction,
    Vector3 Color,
    double Intensity,
    double Range,
    double InnerCone,
    double OuterCone)
{
    /// <summary>The class of a point light.</summary>
    public const string PointClass = "light";

    /// <summary>The class of a spot.</summary>
    public const string SpotClass = "light_spot";

    /// <summary>The class of the sun.</summary>
    public const string DirectionalClass = "light_environment";

    /// <summary>
    /// The lumens of a point or a spot with no <c>intensity</c>, a 60 watt bulb's.
    /// </summary>
    public const double DefaultLumens = 800;

    /// <summary>The lux of a sun with no <c>intensity</c>, a clear noon's.</summary>
    public const double DefaultLux = 100_000;

    /// <summary>The meters a point or a spot with no <c>range</c> reaches.</summary>
    public const double DefaultRange = 10;

    /// <summary>
    /// The inner cone of a spot with no <c>_inner_cone</c>, in degrees from its axis.
    /// </summary>
    public const double DefaultInnerCone = 25;

    /// <summary>The outer cone of a spot with no <c>_cone</c>, in degrees from its axis.</summary>
    public const double DefaultOuterCone = 30;

    /// <summary>
    /// A color of 0 to 255 a channel in sRGB, as TrenchBroom's color picker writes it, in linear
    /// sRGB from 0 to 1, decoded by sRGB's own curve.
    /// </summary>
    /// <param name="red">The red channel, from 0 to 255.</param>
    /// <param name="green">The green channel.</param>
    /// <param name="blue">The blue channel.</param>
    public static Vector3 Linear(double red, double green, double blue)
    {
        static float Decode(double channel)
        {
            var c = Math.Clamp(channel / 255, 0, 1);
            return (float)(c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4));
        }

        return new Vector3(Decode(red), Decode(green), Decode(blue));
    }
}
