using System.Numerics;

namespace Levels;

// Writing a place, a light and a tree back to an entity's keys, the readers' inverses. Each key is
// written only where the value given differs from what the reader reads from the keys now, so an
// entity written back unedited keeps its keys, their order and their text, and a key left at its
// default stays absent. A place in meters and a turn in single precision are written to a
// thousandth of a unit and of a degree, which reads back the numbers a level is drawn with.
public sealed partial class LevelEntity
{
    // How near a place in meters or a way is to the one the keys give and counts as it, and how
    // small a turn in radians between two counts as none, under the thousandth of a degree the
    // angles are written to and above a single-precision turn's rounding.
    private const float SamePlace = 1e-5f;
    private const double SameTurn = 1e-5;

    /// <summary>
    /// Writes where the entity stands and how it is turned to its <c>origin</c> and its
    /// <c>angle</c> or <c>angles</c>, as <see cref="Placement"/> reads them.
    /// </summary>
    /// <param name="placement">The place in meters with Y up and the turn, minus Z the way it
    /// faces.</param>
    /// <param name="unitsPerMeter">How many of the file's units make a meter,
    /// <see cref="Level.UnitsPerMeter"/>.</param>
    /// <remarks>
    /// A turn of no pitch and no roll is written as the <c>angle</c> where the entity has no
    /// <c>angles</c>, and any other as the three <c>angles</c> in place of the <c>angle</c>. A
    /// brush entity's place is its <c>origin</c> alone, since its brushes stand where the file puts
    /// them.
    /// </remarks>
    /// <exception cref="ArgumentException">A brush entity is given a turn, or the world a place
    /// off the level's origin.</exception>
    public void Place(LevelPlacement placement, double unitsPerMeter)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitsPerMeter);
        var now = Placement(unitsPerMeter);
        var turned = Between(placement.Rotation, now.Rotation) > SameTurn;
        if (ClassName == "worldspawn")
        {
            if (placement.Position.Length() > SamePlace || turned)
                throw new ArgumentException("The world stands at the level's origin, unturned.", nameof(placement));
            return;
        }

        if (Vector3.Distance(placement.Position, now.Position) > SamePlace)
            this["origin"] = Text(LevelPoint.FromMeters(placement.Position, unitsPerMeter));

        if (!turned) return;
        if (Brushes.Count > 0)
            throw new ArgumentException("A brush entity stands unturned, its brushes where the file puts them; turn the brushes.", nameof(placement));
        var (pitch, yaw, roll) = Angles(placement.Rotation);
        Turn(pitch, yaw, roll);
    }

    /// <summary>
    /// Writes a light to the entity's keys, its class, place, way, color, strength, range and
    /// cones, as <see cref="Light"/> reads them.
    /// </summary>
    /// <param name="light">The light.</param>
    /// <param name="unitsPerMeter">How many of the file's units make a meter,
    /// <see cref="Level.UnitsPerMeter"/>.</param>
    /// <remarks>
    /// A spot's or the sun's way is written as a pitch and a yaw, the entity's roll kept. The color
    /// is written in sRGB from 0 to 255 a channel, each the nearest whole number, which reads back
    /// as the color read. A point light's way and cones and the sun's range and cones are not read
    /// and not written.
    /// </remarks>
    /// <exception cref="ArgumentException">The light's way has no length.</exception>
    public void SetLight(LevelLight light, double unitsPerMeter)
    {
        ArgumentNullException.ThrowIfNull(light);
        var className = light.Kind switch
        {
            LevelLightKind.Point => LevelLight.PointClass,
            LevelLightKind.Spot => LevelLight.SpotClass,
            _ => LevelLight.DirectionalClass,
        };
        if (ClassName != className) this["classname"] = className;

        var now = Light(unitsPerMeter)!;
        if (light.Kind != LevelLightKind.Directional && Vector3.Distance(light.Position, now.Position) > SamePlace)
            this["origin"] = Text(LevelPoint.FromMeters(light.Position, unitsPerMeter));

        if (light.Kind != LevelLightKind.Point && Vector3.Distance(Vector3.Normalize(light.Direction), now.Direction) > SamePlace)
        {
            if (!(light.Direction.Length() > 0)) throw new ArgumentException("A light's way has a length.", nameof(light));
            var way = Vector3.Normalize(light.Direction);
            var pitch = Math.Asin(Math.Clamp(-way.Y, -1, 1)) * 180 / Math.PI;
            var yaw = Math.Atan2(-way.Z, way.X) * 180 / Math.PI;
            var roll = Point("angles")?.Z ?? 0;
            Turn(pitch, yaw, roll);
        }

        if (Vector3.Distance(light.Color, now.Color) > SamePlace)
            this["_color"] = $"{Srgb(light.Color.X)} {Srgb(light.Color.Y)} {Srgb(light.Color.Z)}";
        if (light.Intensity != now.Intensity) this["intensity"] = Number(light.Intensity);
        if (light.Kind == LevelLightKind.Directional) return;
        if (light.Range != now.Range) this["range"] = Number(light.Range);
        if (light.Kind != LevelLightKind.Spot) return;
        if (light.InnerCone != now.InnerCone) this["_inner_cone"] = Number(light.InnerCone);
        if (light.OuterCone != now.OuterCone) this["_cone"] = Number(light.OuterCone);
    }

    /// <summary>
    /// Writes a tree to the entity's keys, its class, its <c>species</c> and its <c>seed</c>, as
    /// <see cref="Tree"/> reads them.
    /// </summary>
    /// <param name="tree">The tree; a species of nothing takes the <c>species</c> key away.</param>
    public void SetTree(LevelTree tree)
    {
        if (ClassName != LevelTree.Class) this["classname"] = LevelTree.Class;
        var now = Tree()!.Value;
        if (tree.Species != now.Species)
        {
            if (tree.Species is { Length: > 0 } species) this["species"] = species;
            else Keys.RemoveAll(pair => pair.Key == "species");
        }

        if (tree.Seed != now.Seed) this["seed"] = Number(tree.Seed);
    }

    // The angle where a turn has no pitch and no roll and the entity no angles, else the angles.
    private void Turn(double pitch, double yaw, double roll)
    {
        (pitch, yaw, roll) = (Thousandth(pitch), Thousandth(yaw), Thousandth(roll));
        if (pitch == 0 && roll == 0 && this["angles"] is null)
        {
            this["angle"] = Number(yaw);
            return;
        }

        this["angles"] = $"{Number(pitch)} {Number(yaw)} {Number(roll)}";
        Keys.RemoveAll(pair => pair.Key == "angle");
    }

    // The pitch, the yaw and the roll a turn gives, the inverse of Placement's: the quarter turn
    // about Y taken off, the forward's pitch and yaw read, and the roll as the up's turn about the
    // forward from the up of no roll.
    private static (double Pitch, double Yaw, double Roll) Angles(Quaternion rotation)
    {
        var turned = Quaternion.Normalize(rotation) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2);
        var forward = Vector3.Transform(Vector3.UnitX, turned);
        var pitch = Math.Asin(Math.Clamp(-forward.Y, -1, 1));
        var yaw = Math.Atan2(-forward.Z, forward.X);
        var unrolled = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)yaw) * Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, (float)pitch);
        var level = Vector3.Transform(Vector3.UnitY, unrolled);
        var up = Vector3.Transform(Vector3.UnitY, turned);
        var roll = Math.Atan2(Vector3.Dot(up, Vector3.Cross(forward, level)), Vector3.Dot(up, level));
        return (pitch * 180 / Math.PI, yaw * 180 / Math.PI, roll * 180 / Math.PI);
    }

    // The angle in radians of the turn from one rotation to another, from the vector part of the
    // turn between them, which grows as the angle does where the dot product hardly moves.
    private static double Between(Quaternion a, Quaternion b)
    {
        var turn = Quaternion.Conjugate(Quaternion.Normalize(b)) * Quaternion.Normalize(a);
        var half = new Vector3(turn.X, turn.Y, turn.Z).Length();
        return 2 * Math.Atan2(half, Math.Abs(turn.W));
    }

    private static double Thousandth(double value)
    {
        var kept = Math.Round(value, 3, MidpointRounding.AwayFromZero);
        return kept == 0 ? 0 : kept;
    }

    private static string Number(double value) => LevelPoint.Number(Thousandth(value));

    private static string Text(LevelPoint point) => $"{Number(point.X)} {Number(point.Y)} {Number(point.Z)}";

    private static int Srgb(float linear)
    {
        var c = Math.Clamp((double)linear, 0, 1);
        var encoded = c <= 0.0031308 ? c * 12.92 : 1.055 * Math.Pow(c, 1 / 2.4) - 0.055;
        return (int)Math.Round(encoded * 255, MidpointRounding.AwayFromZero);
    }
}
