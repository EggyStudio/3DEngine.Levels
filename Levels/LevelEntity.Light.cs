using System.Numerics;

namespace Levels;

public sealed partial class LevelEntity
{
    /// <summary>
    /// The light the entity is, read from its keys, or nothing where its class is not a light's.
    /// </summary>
    /// <param name="unitsPerMeter">How many of the file's units make a meter,
    /// <see cref="Level.UnitsPerMeter"/>.</param>
    /// <remarks>
    /// <para>
    /// The classes are <c>light</c>, <c>light_spot</c> and <c>light_environment</c>, as Levels.fgd
    /// gives them to TrenchBroom. The place and the way the light faces are the entity's
    /// (<see cref="Placement"/>), so a spot or the sun faces where its arrow points.
    /// </para>
    /// <para>
    /// The keys are <c>_color</c>, three numbers from 0 to 255 in sRGB, white where it is missing;
    /// <c>intensity</c>, in lumens for a point or a spot and in lux for the sun; <c>range</c>, in
    /// meters; and a spot's <c>_inner_cone</c> and <c>_cone</c>, in degrees from its axis. A key
    /// missing takes the default of <see cref="LevelLight"/>'s constants, which the fgd shows.
    /// </para>
    /// </remarks>
    /// <exception cref="FormatException">A key is not the numbers it is read as, or a strength, a
    /// range or a cone is below zero, or the inner cone is wider than the outer.</exception>
    public LevelLight? Light(double unitsPerMeter)
    {
        LevelLightKind? kind = ClassName switch
        {
            LevelLight.PointClass => LevelLightKind.Point,
            LevelLight.SpotClass => LevelLightKind.Spot,
            LevelLight.DirectionalClass => LevelLightKind.Directional,
            _ => null,
        };
        if (kind is not { } of) return null;

        var placed = Placement(unitsPerMeter);
        var color = Point("_color") is { } c ? LevelLight.Linear(c.X, c.Y, c.Z) : Vector3.One;
        var sun = of == LevelLightKind.Directional;
        var intensity = AtLeastZero("intensity", sun ? LevelLight.DefaultLux : LevelLight.DefaultLumens);
        var range = sun ? 0 : AtLeastZero("range", LevelLight.DefaultRange);
        double inner = 0, outer = 0;
        if (of == LevelLightKind.Spot)
        {
            inner = AtLeastZero("_inner_cone", LevelLight.DefaultInnerCone);
            outer = AtLeastZero("_cone", LevelLight.DefaultOuterCone);
            if (inner > outer)
                throw new FormatException($"The {ClassName} has the _inner_cone {inner} wider than its _cone {outer}.");
        }

        var direction = Vector3.Normalize(Vector3.Transform(-Vector3.UnitZ, placed.Rotation));
        return new LevelLight(of, placed.Position, direction, color, intensity, range, inner, outer) { Pattern = Pattern() };
    }

    // The light's pattern, its pattern key's letters or the preset its style names, none steady.
    private string? Pattern()
    {
        if (this["pattern"] is { Length: > 0 } letters)
            return letters.All(c => c is >= 'a' and <= 'z') ? letters : throw Refused("pattern", letters, "letters from a to z");
        if (this["style"] is not { } style) return null;
        if (!int.TryParse(style, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var number) || number >= LevelLight.Presets.Count)
            throw Refused("style", style, $"the number of a preset from 0 to {LevelLight.Presets.Count - 1}");
        return number == 0 ? null : LevelLight.Presets[number];
    }

    private double AtLeastZero(string key, double otherwise)
    {
        var value = Number(key) ?? otherwise;
        return value >= 0 ? value : throw Refused(key, this[key]!, "a number of zero or more");
    }

    /// <summary>
    /// The world's sky and its ambient light, read from the <c>worldspawn</c>'s keys, or nothing
    /// for another class.
    /// </summary>
    /// <remarks>
    /// The keys are Levels.fgd's: <c>_sky</c>, a sky's name for the engine to resolve, none where
    /// it is missing or empty; <c>_ambient</c>, a color in sRGB from 0 to 255 a channel, white
    /// where it is missing; and <c>_ambient_lux</c>, the lux the ambient light gives every surface,
    /// 0 where it is missing.
    /// </remarks>
    /// <exception cref="FormatException">The color is not three numbers or the lux is not a
    /// number of zero or more.</exception>
    public LevelAmbient? Ambient()
    {
        if (ClassName != "worldspawn") return null;
        var sky = this["_sky"] is { Length: > 0 } name ? name : null;
        var color = Point("_ambient") is { } c ? LevelLight.Linear(c.X, c.Y, c.Z) : Vector3.One;
        return new LevelAmbient(sky, color, AtLeastZero("_ambient_lux", 0));
    }
}
