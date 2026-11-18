using System.Globalization;
using System.Numerics;

namespace Levels;

public sealed partial class LevelEntity
{
    /// <summary>A key's value as a number, or nothing where the entity has no such key.</summary>
    /// <param name="key">The key, as <c>angle</c>.</param>
    /// <exception cref="FormatException">The value is not a finite number.</exception>
    public double? Number(string key) => this[key] is { } text ? Parse(text, key, text) : null;

    /// <summary>
    /// A key's value as a whole number of zero or more, or nothing where the entity has no such
    /// key.
    /// </summary>
    /// <param name="key">The key, as <c>seed</c>.</param>
    /// <exception cref="FormatException">The value is not a whole number of zero or
    /// more.</exception>
    public uint? WholeNumber(string key)
    {
        if (this[key] is not { } text) return null;
        return uint.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
            ? value
            : throw Refused(key, text, "a whole number of zero or more");
    }

    /// <summary>
    /// A key's value as three numbers, a point in the file's units, or nothing where the entity has
    /// no such key.
    /// </summary>
    /// <param name="key">The key, as <c>origin</c>.</param>
    /// <exception cref="FormatException">The value is not three finite numbers.</exception>
    public LevelPoint? Point(string key)
    {
        if (this[key] is not { } text) return null;
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3) throw Refused(key, text, "three numbers");
        return new LevelPoint(Parse(parts[0], key, text), Parse(parts[1], key, text), Parse(parts[2], key, text));
    }

    /// <summary>
    /// Where the entity stands in meters with Y up and how it is turned, as an engine places what
    /// it spawns for it.
    /// </summary>
    /// <param name="unitsPerMeter">How many of the file's units make a meter,
    /// <see cref="Level.UnitsPerMeter"/>.</param>
    /// <remarks>
    /// <para>
    /// A point entity stands at its <c>origin</c>, turned by its <c>angle</c>, the yaw in degrees,
    /// or its <c>angles</c>, the pitch, the yaw and the roll, so that its forward, minus Z, looks
    /// the way the file's angles face, where TrenchBroom draws its arrow. A pitch above zero looks
    /// down, as the Quake format has it.
    /// </para>
    /// <para>
    /// A brush entity stands unturned, since its brushes are where the file puts them whatever its
    /// angles, at its <c>origin</c> where it has one, which a door turns about, and at the middle
    /// of the box round its brushes otherwise. The <c>worldspawn</c> stands at the level's origin.
    /// </para>
    /// </remarks>
    /// <exception cref="FormatException">The <c>origin</c>, <c>angle</c> or <c>angles</c> is not
    /// the numbers it is read as.</exception>
    /// <exception cref="InvalidOperationException">A brush closes no solid.</exception>
    public LevelPlacement Placement(double unitsPerMeter)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitsPerMeter);
        var position = Point("origin") is { } origin ? origin.Meters(unitsPerMeter)
            : Brushes.Count > 0 && ClassName != "worldspawn" ? Middle(unitsPerMeter)
            : Vector3.Zero;
        return new LevelPlacement(position, Brushes.Count > 0 ? Quaternion.Identity : Turn());
    }

    /// <summary>
    /// The turn the entity's angles give, the file's turn carried onto Y up and then a quarter
    /// about Y, so the entity's minus Z looks along the file's X turned by the angles, since a map
    /// file faces an entity at no angle along X.
    /// </summary>
    private Quaternion Turn()
    {
        static float Radians(double degrees) => (float)(degrees * Math.PI / 180);
        double pitch = 0, yaw = 0, roll = 0;
        if (Point("angles") is { } angles) (pitch, yaw, roll) = (angles.X, angles.Y, angles.Z);
        else if (Number("angle") is { } angle) yaw = angle;

        // The file's yaw about Z, pitch about Y and roll about X are, Y up, a yaw about Y, a pitch
        // about minus Z and a roll about X.
        var turned = Quaternion.CreateFromAxisAngle(Vector3.UnitY, Radians(yaw))
            * Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, Radians(pitch))
            * Quaternion.CreateFromAxisAngle(Vector3.UnitX, Radians(roll));
        return turned * Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2);
    }

    /// <summary>The middle of the box round the entity's brushes, in meters.</summary>
    private Vector3 Middle(double unitsPerMeter)
    {
        var corners = Brushes.SelectMany(brush => brush.Polygons()).OfType<LevelPoint[]>()
            .SelectMany(polygon => polygon).ToList();
        if (corners.Count == 0) return Vector3.Zero;
        var low = new LevelPoint(corners.Min(p => p.X), corners.Min(p => p.Y), corners.Min(p => p.Z));
        var high = new LevelPoint(corners.Max(p => p.X), corners.Max(p => p.Y), corners.Max(p => p.Z));
        return ((low + high) * 0.5).Meters(unitsPerMeter);
    }

    private double Parse(string part, string key, string text) =>
        double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
        && double.IsFinite(value)
            ? value
            : throw Refused(key, text, text == part ? "a number" : "three numbers");

    private FormatException Refused(string key, string text, string meant) =>
        new($"The {(ClassName.Length > 0 ? ClassName : "entity of no class")} has the {key} {text}, "
            + $"where {meant} is meant.");

    /// <summary>
    /// How far and which way a door or a button moves from shut to open or from out to in, in
    /// meters with Y up, or nothing where the entity is no <c>func_door</c> or <c>func_button</c>.
    /// </summary>
    /// <param name="unitsPerMeter">How many of the file's units make a meter,
    /// <see cref="Level.UnitsPerMeter"/>.</param>
    /// <remarks>
    /// The keys are Levels.fgd's for <c>func_door</c> and <c>func_button</c>: <c>angle</c>, the way
    /// it moves in degrees about the vertical from the file's X, -1 for up and -2 for down, 0 where
    /// it is missing, and <c>lip</c>, the units it stays short of its full travel, 8 for a door and
    /// 4 for a button where it is missing. It moves the
    /// way it opens by the size of the box round its brushes that way, the box's sides each taken
    /// by how much of the way runs along it, less the lip, and stays shut where that is less than
    /// none, as a level editor's doors have it.
    /// </remarks>
    /// <exception cref="FormatException">The angle or the lip is not a number.</exception>
    /// <exception cref="InvalidOperationException">A brush closes no solid.</exception>
    public Vector3? Travel(double unitsPerMeter)
    {
        if (ClassName is not ("func_door" or "func_button")) return null;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitsPerMeter);
        var angle = Number("angle") ?? 0;
        var lip = Number("lip") ?? (ClassName == "func_door" ? 8 : 4);
        var way = angle switch
        {
            -1 => new LevelPoint(0, 0, 1),
            -2 => new LevelPoint(0, 0, -1),
            _ => new LevelPoint(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180), 0),
        };

        var corners = Brushes.SelectMany(brush => brush.Polygons()).OfType<LevelPoint[]>().SelectMany(polygon => polygon).ToList();
        if (corners.Count == 0) return Vector3.Zero;
        var size = new LevelPoint(
            corners.Max(p => p.X) - corners.Min(p => p.X),
            corners.Max(p => p.Y) - corners.Min(p => p.Y),
            corners.Max(p => p.Z) - corners.Min(p => p.Z));
        var span = Math.Abs(way.X) * size.X + Math.Abs(way.Y) * size.Y + Math.Abs(way.Z) * size.Z - lip;
        return span > 0 ? (way * span).Meters(unitsPerMeter) : Vector3.Zero;
    }

    /// <summary>
    /// How a <c>func_rotating</c> turns about an axis through its <see cref="Placement"/>, or
    /// nothing for another class.
    /// </summary>
    /// <param name="unitsPerMeter">How many of the file's units make a meter.</param>
    /// <remarks>
    /// The keys are Levels.fgd's: <c>speed</c>, degrees a second, 100 where it is missing, and
    /// <c>spawnflags</c>, the level-editor convention's flags, 4 for the file's X axis, 8 for its Y
    /// and neither for the vertical, and 2 to turn the other way. The axis is given in meters with
    /// Y up.
    /// </remarks>
    /// <exception cref="FormatException">The speed is not a number or the flags not a whole
    /// number.</exception>
    public LevelSpin? Spin(double unitsPerMeter)
    {
        if (ClassName != "func_rotating") return null;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitsPerMeter);
        var speed = Number("speed") ?? 100;
        var flags = WholeNumber("spawnflags") ?? 0;
        var axis = (flags & 4) != 0 ? new LevelPoint(1, 0, 0) : (flags & 8) != 0 ? new LevelPoint(0, 1, 0) : new LevelPoint(0, 0, 1);
        return new LevelSpin(axis.Meters(1), (flags & 2) != 0 ? -speed : speed);
    }
}
