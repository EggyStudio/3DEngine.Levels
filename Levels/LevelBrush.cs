namespace Levels;

/// <summary>
/// A brush of a level file, a convex solid, what lies behind every one of its faces' planes.
/// </summary>
/// <remarks>
/// A brush is kept as its planes, as Quake's tools, Hammer and TrenchBroom keep one, and its
/// corners are found where three planes meet within the rest (<see cref="Polygons"/>), so a brush
/// stays convex whatever moves its planes and two of them never share a corner by accident.
/// </remarks>
public sealed partial class LevelBrush
{
    // How far, in the file's units, a corner may stand outside a plane and still be within it, and
    // how near two corners may be and be one. Planes through integer points meet at corners exact
    // in double precision, and a millionth of a unit is far below a texel and far above the
    // rounding where planes through points TrenchBroom writes to seventeen digits meet.
    private const double Within = 1e-6;
    private const double Same = 1e-5;

    // How far out along an axis a face's point may stand. A double resolves a hundred-thousandth
    // of a unit out to 2 to the 35th, some 34 billion units, and a plane's arithmetic multiplies
    // coordinates together and loses more, so past this a corner is not found where its planes
    // meet.
    private const double Reach = 1e9;

    /// <summary>The faces.</summary>
    public List<LevelFace> Faces { get; } = [];

    /// <summary>
    /// Each face's polygon, its corners wound clockwise seen from outside as the file's points are,
    /// in the file's units, or nothing for a face whose plane only touches the solid.
    /// </summary>
    /// <returns>A polygon a face, in the order of <see cref="Faces"/>.</returns>
    /// <exception cref="InvalidOperationException">A face's point stands more than a billion units
    /// out along an axis, a face's three points lie on one line, the planes close nothing, or the
    /// brush is flat and has no volume.</exception>
    /// <remarks>
    /// The corners are the points where three planes meet that stand behind every other plane, each
    /// found once and shared by the faces whose planes it lies on, so the faces of a brush meet
    /// edge to edge with no gap a ray could pass. Each face's corners are ordered round their
    /// middle. A brush with several faces in one plane keeps the polygon for the first and none for
    /// the rest.
    /// </remarks>
    public LevelPoint[]?[] Polygons() => Build(out _);

    // The polygons and the corners they share, each corner once, built with no list or closure a
    // face, so a level of many brushes allocates its polygons and little else.
    internal LevelPoint[]?[] Build(out List<LevelPoint> corners)
    {
        var count = Faces.Count;
        var normals = new LevelPoint[count];
        var distances = new double[count];
        for (var face = 0; face < count; face++)
        {
            var (first, second, third) = (Faces[face].First, Faces[face].Second, Faces[face].Third);
            var far = Math.Max(Math.Max(Farthest(first), Farthest(second)), Farthest(third));
            if (!(far <= Reach))
                throw new InvalidOperationException($"The brush's face {face} has a point {LevelPoint.Number(far)} units out along an axis, past the {LevelPoint.Number(Reach)} within which its corners are found.");
            normals[face] = Faces[face].Normal;
            if (normals[face] == default) throw new InvalidOperationException($"The brush's face {face} has three points on one line, which make no plane.");
            distances[face] = Faces[face].Distance;
        }

        // Every three planes that meet at one point within the rest give a corner, kept once.
        corners = new List<LevelPoint>(8);
        for (var a = 0; a < count; a++)
        for (var b = a + 1; b < count; b++)
        for (var c = b + 1; c < count; c++)
        {
            if (Meet(normals[a], distances[a], normals[b], distances[b], normals[c], distances[c]) is not { } point) continue;
            var inside = true;
            for (var other = 0; other < count && inside; other++) inside = LevelPoint.Dot(normals[other], point) - distances[other] <= Within * Math.Max(1, Math.Abs(distances[other]));
            if (!inside) continue;
            var known = false;
            for (var k = 0; k < corners.Count && !known; k++) known = (corners[k] - point).Length < Same;
            if (!known) corners.Add(point);
        }

        if (corners.Count < 4) throw new InvalidOperationException($"The brush's {count} planes close no solid.");

        // A solid stands behind each of its planes, so a brush whose corners all lie in one of them
        // is flat, two of its planes facing each other in one place, and has no volume.
        for (var face = 0; face < count; face++)
        {
            var behind = false;
            for (var k = 0; k < corners.Count && !behind; k++) behind = LevelPoint.Dot(normals[face], corners[k]) - distances[face] < -Same;
            if (!behind) throw new InvalidOperationException($"The brush's corners all lie in its face {face}'s plane, so it has no volume.");
        }

        // Which corners lie in which face's plane, a row a face.
        var on = new bool[count * corners.Count];
        var total = 0;
        for (var face = 0; face < count; face++)
        for (var k = 0; k < corners.Count; k++)
        {
            if (Math.Abs(LevelPoint.Dot(normals[face], corners[k]) - distances[face]) > Same) continue;
            on[face * corners.Count + k] = true;
            total++;
        }

        // Each face's corners round their middle, clockwise seen from outside along the normal,
        // and every edge kept by its two corners' indices.
        var polygons = new LevelPoint[]?[count];
        var order = new int[corners.Count];
        var angles = new double[corners.Count];
        var (edgeFrom, edgeTo) = (new int[total], new int[total]);
        var edges = 0;
        for (var face = 0; face < count; face++)
        {
            var mine = 0;
            var middle = default(LevelPoint);
            for (var k = 0; k < corners.Count; k++)
            {
                if (!on[face * corners.Count + k]) continue;
                order[mine++] = k;
                middle += corners[k];
            }

            if (mine < 3 || SharesAPlane(polygons, normals, distances, face)) continue;
            middle *= 1.0 / mine;
            var normal = normals[face];
            var across = Math.Abs(normal.Z) < 0.9 ? new LevelPoint(0, 0, 1) : new LevelPoint(1, 0, 0);
            var right = LevelPoint.Cross(across, normal);
            right *= 1 / right.Length;
            var up = LevelPoint.Cross(normal, right);
            for (var m = 0; m < mine; m++)
            {
                var point = corners[order[m]] - middle;
                angles[m] = Math.Atan2(LevelPoint.Dot(point, up), LevelPoint.Dot(point, right));
            }

            // The greatest angle first, ties kept in the corners' order, by insertion.
            for (var m = 1; m < mine; m++)
            for (var n = m; n > 0 && angles[n] > angles[n - 1]; n--)
            {
                (angles[n], angles[n - 1]) = (angles[n - 1], angles[n]);
                (order[n], order[n - 1]) = (order[n - 1], order[n]);
            }

            var polygon = polygons[face] = new LevelPoint[mine];
            for (var m = 0; m < mine; m++)
            {
                polygon[m] = corners[order[m]];
                (edgeFrom[edges], edgeTo[edges]) = (order[m], order[(m + 1) % mine]);
                edges++;
            }
        }

        // A solid's every edge runs one way round one face and back round the face beside it, and
        // planes that leave the solid open on a side give faces whose edges run round one alone.
        for (var e = 0; e < edges; e++)
        {
            var back = false;
            for (var f = 0; f < edges && !back; f++) back = edgeFrom[f] == edgeTo[e] && edgeTo[f] == edgeFrom[e];
            if (!back) throw new InvalidOperationException($"The brush's {count} planes close no solid.");
        }

        return polygons;
    }

    private static double Farthest(LevelPoint point) => Math.Max(Math.Max(Math.Abs(point.X), Math.Abs(point.Y)), Math.Abs(point.Z));

    // Whether an earlier face with a polygon lies in the same plane, which keeps the polygon for
    // itself.
    private static bool SharesAPlane(LevelPoint[]?[] polygons, LevelPoint[] normals, double[] distances, int face)
    {
        for (var earlier = 0; earlier < face; earlier++)
            if (polygons[earlier] is not null && LevelPoint.Dot(normals[earlier], normals[face]) > 1 - 1e-9 && Math.Abs(distances[earlier] - distances[face]) < Same)
                return true;
        return false;
    }

    /// <summary>
    /// How far along a ray it first meets the brush's solid, or nothing where it meets none within
    /// its reach.
    /// </summary>
    /// <param name="from">Where the ray starts, in the file's units.</param>
    /// <param name="way">The way it runs, of any length but none.</param>
    /// <param name="reach">How far it runs, in lengths of <paramref name="way"/>.</param>
    /// <returns>
    /// The distance in lengths of <paramref name="way"/>, the file's units where its length is one,
    /// and zero for a ray that starts inside.
    /// </returns>
    /// <remarks>
    /// The ray is cut by each plane in turn, where it enters the half behind the plane and where it
    /// leaves, so the solid is met at the last entering and missed where that comes after the first
    /// leaving. A ray along a plane outside it misses the brush.
    /// </remarks>
    /// <exception cref="ArgumentException">The way has no length, or the reach is below zero or not
    /// a number.</exception>
    public double? Hit(LevelPoint from, LevelPoint way, double reach)
    {
        if (!(way.Length > 0)) throw new ArgumentException("A ray's way has a length.", nameof(way));
        if (!(reach >= 0)) throw new ArgumentException($"A ray's reach is {reach}, where zero or more is meant.", nameof(reach));
        var (enter, leave) = (0.0, reach);
        foreach (var face in Faces)
        {
            var (normal, distance) = (face.Normal, face.Distance);
            var (along, outside) = (LevelPoint.Dot(normal, way), LevelPoint.Dot(normal, from) - distance);
            if (Math.Abs(along) < 1e-12 * way.Length)
            {
                if (outside > Within) return null;
                continue;
            }

            var cut = -outside / along;
            if (along < 0) enter = Math.Max(enter, cut);
            else leave = Math.Min(leave, cut);
            if (enter > leave) return null;
        }

        return enter;
    }

    /// <summary>Where three planes meet, or nothing where two of them run side by side.</summary>
    private static LevelPoint? Meet(LevelPoint n1, double d1, LevelPoint n2, double d2, LevelPoint n3, double d3)
    {
        var cross23 = LevelPoint.Cross(n2, n3);
        var denominator = LevelPoint.Dot(n1, cross23);
        if (Math.Abs(denominator) < 1e-9) return null;
        return (cross23 * d1 + LevelPoint.Cross(n3, n1) * d2 + LevelPoint.Cross(n1, n2) * d3) * (1 / denominator);
    }
}
