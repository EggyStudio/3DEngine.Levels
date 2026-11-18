namespace Levels;

// The brushes a level is blocked out with, each a convex outline across Y stretched from one Y to
// another, its faces given the world-aligned mapping a new face takes, so a level built in code
// reads in TrenchBroom as one drawn there does.
public sealed partial class LevelBrush
{
    /// <summary>
    /// The scale a face made here takes, a sixteenth, which TrenchBroom's game config beside the
    /// engine's levels gives a new face, so a prototype's picture of 512 texels spans 32 units, a
    /// meter at the default units a meter.
    /// </summary>
    public const double DefaultScale = 1.0 / 16;

    /// <summary>A box between two corners.</summary>
    /// <param name="low">The corner with the least of each axis, in the file's units.</param>
    /// <param name="high">The corner with the most of each.</param>
    /// <param name="material">What every face is drawn with.</param>
    /// <param name="scale">The faces' scale, <see cref="DefaultScale"/> unless given.</param>
    /// <exception cref="ArgumentException">The box has no size along an axis.</exception>
    public static LevelBrush Box(LevelPoint low, LevelPoint high, string material, double scale = DefaultScale)
    {
        Sized(low, high);
        return Prism([(low.X, low.Z), (high.X, low.Z), (high.X, high.Z), (low.X, high.Z)], low.Y, high.Y, material, scale);
    }

    /// <summary>
    /// A ramp between two corners, its slope rising along X from the low corner's height at its low
    /// X to the high corner's at its high X.
    /// </summary>
    /// <param name="low">The corner with the least of each axis, in the file's units.</param>
    /// <param name="high">The corner with the most of each.</param>
    /// <param name="material">What every face is drawn with.</param>
    /// <param name="scale">The faces' scale, <see cref="DefaultScale"/> unless given.</param>
    /// <remarks>A ramp rising another way is this one turned (<see cref="Turned"/>).</remarks>
    /// <exception cref="ArgumentException">The ramp has no size along an axis.</exception>
    public static LevelBrush Wedge(LevelPoint low, LevelPoint high, string material, double scale = DefaultScale)
    {
        Sized(low, high);
        return Prism([(low.X, low.Z), (high.X, low.Z), (high.X, high.Z)], low.Y, high.Y, material, scale);
    }

    /// <summary>
    /// An arch standing across X, a half ring or a part of one cut into straight segments, a brush
    /// each, every segment spanning an equal share of the arch's angle.
    /// </summary>
    /// <param name="center">The middle of the ring, the arch standing on its X axis and its Y the
    /// middle of its depth.</param>
    /// <param name="radius">The radius of the ring's inside, the opening the arch spans.</param>
    /// <param name="thickness">How far its outside stands out from its inside.</param>
    /// <param name="depth">How deep it is along Y.</param>
    /// <param name="material">What every face is drawn with.</param>
    /// <param name="segments">How many straight pieces the ring is cut into, three or more.</param>
    /// <param name="degrees">How much of the ring it is, from its right side up and over, 180 for a
    /// half ring.</param>
    /// <param name="scale">The faces' scale, <see cref="DefaultScale"/> unless given.</param>
    /// <returns>The segments, from the right to the left.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A measure is not above zero, the segments are
    /// fewer than three, or the arch is more than a whole ring.</exception>
    public static IReadOnlyList<LevelBrush> Arch(LevelPoint center, double radius, double thickness, double depth, string material, int segments = 8, double degrees = 180, double scale = DefaultScale)
    {
        if (!(radius > 0)) throw new ArgumentOutOfRangeException(nameof(radius), radius, "An arch's radius is above zero.");
        if (!(thickness > 0)) throw new ArgumentOutOfRangeException(nameof(thickness), thickness, "An arch's thickness is above zero.");
        if (!(depth > 0)) throw new ArgumentOutOfRangeException(nameof(depth), depth, "An arch's depth is above zero.");
        ArgumentOutOfRangeException.ThrowIfLessThan(segments, 3);
        if (!(degrees > 0 && degrees <= 360)) throw new ArgumentOutOfRangeException(nameof(degrees), degrees, "An arch is more than nothing of its ring and no more than all of it.");

        var brushes = new List<LevelBrush>();
        var step = degrees * Math.PI / 180 / segments;
        for (var piece = 0; piece < segments; piece++)
        {
            var (a, b) = (piece * step, (piece + 1) * step);
            (double X, double Z) At(double angle, double reach) => (center.X + reach * Math.Cos(angle), center.Z + reach * Math.Sin(angle));
            brushes.Add(Prism([At(a, radius), At(a, radius + thickness), At(b, radius + thickness), At(b, radius)], center.Y - depth / 2, center.Y + depth / 2, material, scale));
        }

        return brushes;
    }

    /// <summary>
    /// The brush turned about the vertical line through a point, its faces' mapping taken again
    /// from the world, so its material lies on it as on one drawn where it stands.
    /// </summary>
    /// <param name="degrees">The turn, counter-clockwise seen from above.</param>
    /// <param name="about">A point of the line it turns about.</param>
    /// <remarks>
    /// The faces' offsets, rotations and scales are kept, and their axes are the ones a face facing
    /// its new way takes.
    /// </remarks>
    public LevelBrush Turned(double degrees, LevelPoint about)
    {
        // A quarter turn's sine and cosine taken whole, where the library's give six parts in a
        // hundred quadrillion for a zero, which a file would carry as dust on every point.
        var quarters = degrees / 90;
        var (cos, sin) = quarters == Math.Round(quarters)
            ? (((int)Math.Round(quarters) % 4 + 4) % 4) switch { 0 => (1.0, 0.0), 1 => (0.0, 1.0), 2 => (-1.0, 0.0), _ => (0.0, -1.0) }
            : (Math.Cos(degrees * Math.PI / 180), Math.Sin(degrees * Math.PI / 180));
        LevelPoint Turn(LevelPoint point)
        {
            var (x, y) = (point.X - about.X, point.Y - about.Y);
            return new LevelPoint(Kept(about.X + x * cos - y * sin), Kept(about.Y + x * sin + y * cos), point.Z);
        }

        var turned = new LevelBrush();
        foreach (var face in Faces)
        {
            var moved = face with { First = Turn(face.First), Second = Turn(face.Second), Third = Turn(face.Third) };
            var (u, v) = Paraxial(moved.Normal);
            turned.Faces.Add(moved with { UAxis = u, VAxis = v });
        }

        return turned;
    }

    /// <summary>
    /// The brush moved, its faces' mapping taken again from the world, so its material stays where
    /// the world puts it rather than going with the brush, as a moved brush's does in TrenchBroom
    /// with its texture lock off.
    /// </summary>
    /// <param name="by">How far it moves along each of the file's axes, in its units.</param>
    /// <remarks>
    /// With <see cref="Turned"/>, a wall or a ramp made along X at the origin is stood anywhere
    /// facing any way. The faces' offsets, rotations and scales are kept.
    /// </remarks>
    public LevelBrush Moved(LevelPoint by)
    {
        LevelPoint Move(LevelPoint point) => new(Kept(point.X + by.X), Kept(point.Y + by.Y), Kept(point.Z + by.Z));

        var moved = new LevelBrush();
        foreach (var face in Faces) moved.Faces.Add(face with { First = Move(face.First), Second = Move(face.Second), Third = Move(face.Third) });
        return moved;
    }

    /// <summary>
    /// A brush of a convex outline across Y, its points as (X, Z) in either winding, stretched from
    /// one Y to another, each face's three points wound as the file winds them and its axes the
    /// world's.
    /// </summary>
    /// <param name="outline">The outline's corners as (X, Z) in the file's units, three or more,
    /// round it either way.</param>
    /// <param name="from">One end along Y.</param>
    /// <param name="to">The other end, either side of the first.</param>
    /// <param name="material">What every face is drawn with.</param>
    /// <param name="scale">The faces' scale, <see cref="DefaultScale"/> unless given.</param>
    /// <remarks>
    /// A box, a ramp and an arch's segment are prisms of four, three and four corners. Corners less
    /// than a millionth of a unit apart are one.
    /// </remarks>
    /// <exception cref="ArgumentException">The outline has fewer than three corners, its corners
    /// lie on one line or it is not convex, the two ends are one, or the material holds a
    /// space.</exception>
    public static LevelBrush Prism(IReadOnlyList<(double X, double Z)> outline, double from, double to, string material, double scale = DefaultScale) =>
        Extruded(outline, from, to, (corner, y) => new LevelPoint(corner.X, y, corner.Y), "Y", material, scale);

    /// <summary>
    /// A brush of a convex outline on the ground, its points as (X, Y) in either winding, stretched
    /// up from one height to another, as a floor's, a ceiling's or a platform's slab.
    /// </summary>
    /// <param name="outline">The outline's corners as (X, Y) in the file's units, three or more,
    /// round it either way.</param>
    /// <param name="from">One height along Z, the file's up.</param>
    /// <param name="to">The other, above or below the first.</param>
    /// <param name="material">What every face is drawn with.</param>
    /// <param name="scale">The faces' scale, <see cref="DefaultScale"/> unless given.</param>
    /// <exception cref="ArgumentException">The outline has fewer than three corners, its corners
    /// lie on one line or it is not convex, the two heights are one, or the material holds a
    /// space.</exception>
    public static LevelBrush Slab(IReadOnlyList<(double X, double Y)> outline, double from, double to, string material, double scale = DefaultScale) =>
        Extruded(outline, from, to, (corner, z) => new LevelPoint(corner.X, corner.Y, z), "Z", material, scale);

    /// <summary>
    /// A wall from one point of the ground to another, centered on the line between them, standing
    /// up from the first point's height.
    /// </summary>
    /// <param name="from">Where it starts, its height the wall's foot, in the file's units.</param>
    /// <param name="to">Where it ends, whose height is not read.</param>
    /// <param name="height">How high it stands.</param>
    /// <param name="thickness">How thick it is across the line.</param>
    /// <param name="material">What every face is drawn with.</param>
    /// <param name="scale">The faces' scale, <see cref="DefaultScale"/> unless given.</param>
    /// <exception cref="ArgumentException">The two points are one on the ground, or the height or
    /// the thickness is not above zero.</exception>
    public static LevelBrush Wall(LevelPoint from, LevelPoint to, double height, double thickness, string material, double scale = DefaultScale)
    {
        var length = GroundLength(from, to);
        return Box(new LevelPoint(0, -thickness / 2, 0), new LevelPoint(length, thickness / 2, height), material, scale).Along(from, to);
    }

    /// <summary>
    /// A ramp from one point of the ground to another, as wide as given and centered on the line
    /// between them, rising from the first point's height by its rise at the second.
    /// </summary>
    /// <param name="from">Its foot, in the file's units.</param>
    /// <param name="to">Its top's end, whose height is not read.</param>
    /// <param name="width">How wide it is across the line.</param>
    /// <param name="rise">How far it rises from its foot to its top.</param>
    /// <param name="material">What every face is drawn with.</param>
    /// <param name="scale">The faces' scale, <see cref="DefaultScale"/> unless given.</param>
    /// <exception cref="ArgumentException">The two points are one on the ground, or the width or
    /// the rise is not above zero.</exception>
    public static LevelBrush Ramp(LevelPoint from, LevelPoint to, double width, double rise, string material, double scale = DefaultScale)
    {
        var length = GroundLength(from, to);
        return Wedge(new LevelPoint(0, -width / 2, 0), new LevelPoint(length, width / 2, rise), material, scale).Along(from, to);
    }

    /// <summary>
    /// The brush, made along X from the file's origin, turned to run from one point of the ground
    /// toward another and moved to start at the first, its height raised by the first's.
    /// </summary>
    /// <param name="from">Where the brush's origin goes.</param>
    /// <param name="to">The way its X runs, whose height is not read.</param>
    /// <remarks>
    /// A doorway or an arch between two points is <see cref="Level.Doorway"/> or <see cref="Arch"/>
    /// made along X with each brush put along the line. The faces take their mapping from the
    /// world again, as <see cref="Turned"/> and <see cref="Moved"/> give it.
    /// </remarks>
    /// <exception cref="ArgumentException">The two points are one on the ground.</exception>
    public LevelBrush Along(LevelPoint from, LevelPoint to)
    {
        GroundLength(from, to);
        var degrees = Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI;
        return Turned(degrees, default).Moved(from);
    }

    // How far apart two points stand on the ground, refusing two that are one.
    private static double GroundLength(LevelPoint from, LevelPoint to)
    {
        var length = Math.Sqrt((to.X - from.X) * (to.X - from.X) + (to.Y - from.Y) * (to.Y - from.Y));
        return length > 0 ? length : throw new ArgumentException($"A shape from {from} to {to} has no length on the ground.", nameof(to));
    }

    // A convex outline stretched along an axis between two ends, each corner placed by where it
    // stands on the outline and how far along the axis.
    private static LevelBrush Extruded(IReadOnlyList<(double X, double Y)> outline, double from, double to, Func<(double X, double Y), double, LevelPoint> place, string axis, string material, double scale)
    {
        ArgumentException.ThrowIfNullOrEmpty(material);
        if (material.Any(char.IsWhiteSpace)) throw new ArgumentException("A material is a name with no spaces.", nameof(material));

        // Points a hair apart are one, so an arch's first segment, whose inside and outside meet at
        // the ring's foot, is the triangle it is.
        var points = new List<(double X, double Y)>();
        foreach (var raw in outline)
        {
            (double X, double Y) point = (Kept(raw.X), Kept(raw.Y));
            if (points.Count == 0 || Math.Abs(points[^1].X - point.X) + Math.Abs(points[^1].Y - point.Y) > 1e-9) points.Add(point);
        }

        if (points.Count > 1 && Math.Abs(points[0].X - points[^1].X) + Math.Abs(points[0].Y - points[^1].Y) <= 1e-9) points.RemoveAt(points.Count - 1);
        if (points.Count < 3) throw new ArgumentException("A brush's outline has three points or more.", nameof(outline));
        if (!(Math.Abs(to - from) > 0)) throw new ArgumentException($"A brush from {from} to {to} along {axis} has no depth.", nameof(to));

        // Each corner turns the outline the same way, or not at all, round a convex outline, and
        // one that turns both ways bends in, which no brush's planes can hold.
        var (left, right) = (false, false);
        for (var i = 0; i < points.Count; i++)
        {
            var (a, b, c) = (points[i], points[(i + 1) % points.Count], points[(i + 2) % points.Count]);
            var turn = (b.X - a.X) * (c.Y - b.Y) - (b.Y - a.Y) * (c.X - b.X);
            if (turn > 1e-9) left = true;
            if (turn < -1e-9) right = true;
        }

        if (left == right)
            throw new ArgumentException(left ? "A brush's outline is convex, where this one bends in." : "A brush's outline has corners off one line.", nameof(outline));

        var (low, high) = (Math.Min(from, to), Math.Max(from, to));
        var inside = place((points.Average(p => p.X), points.Average(p => p.Y)), (low + high) / 2);
        var brush = new LevelBrush();
        for (var i = 0; i < points.Count; i++)
        {
            var (a, b) = (points[i], points[(i + 1) % points.Count]);
            brush.Faces.Add(Face(place(a, low), place(b, low), place(a, high), inside, material, scale));
        }

        var (p0, p1, p2) = (points[0], points[1], points[2]);
        brush.Faces.Add(Face(place(p0, low), place(p1, low), place(p2, low), inside, material, scale));
        brush.Faces.Add(Face(place(p0, high), place(p1, high), place(p2, high), inside, material, scale));
        return brush;
    }

    /// <summary>
    /// A face through three points, wound so its normal looks away from a point inside the brush,
    /// mapped as a new face is.
    /// </summary>
    private static LevelFace Face(LevelPoint a, LevelPoint b, LevelPoint c, LevelPoint inside, string material, double scale)
    {
        var face = new LevelFace(a, b, c, material, default, 0, default, 0, 0, scale, scale);
        if (LevelPoint.Dot(face.Normal, inside - b) > 0) face = face with { First = c, Third = a };
        var (u, v) = Paraxial(face.Normal);
        return face with { UAxis = u, VAxis = v };
    }

    /// <summary>
    /// The axes a new face's material runs along, those of the plane of the world's axes the face
    /// is nearest, as the Valve format's editors give one, a floor's or a ceiling's along X and
    /// minus Y and a wall's along X or Y and down.
    /// </summary>
    internal static (LevelPoint U, LevelPoint V) Paraxial(LevelPoint normal)
    {
        var (x, y, z) = (Math.Abs(normal.X), Math.Abs(normal.Y), Math.Abs(normal.Z));
        if (z >= x && z >= y) return (new(1, 0, 0), new(0, -1, 0));
        return x >= y ? (new(0, 1, 0), new(0, 0, -1)) : (new(1, 0, 0), new(0, 0, -1));
    }

    /// <summary>
    /// A coordinate kept to a millionth of a unit, so a point a turn or an arch works out is
    /// written as a few digits rather than seventeen and the dust of a sine, and reads the same on
    /// a system whose sine differs in its last digit. A millionth of a unit is far below a texel.
    /// </summary>
    internal static double Kept(double value)
    {
        var kept = Math.Round(value, 6, MidpointRounding.AwayFromZero);
        return kept == 0 ? 0 : kept;
    }

    private static void Sized(LevelPoint low, LevelPoint high)
    {
        if (!(high.X > low.X && high.Y > low.Y && high.Z > low.Z)) throw new ArgumentException($"A brush from {low} to {high} has no size along an axis; the second corner has the most of each.", nameof(high));
    }
}
