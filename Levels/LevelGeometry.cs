using System.Buffers.Binary;
using System.Numerics;

namespace Levels;

/// <summary>
/// An entity's brushes as the triangles an engine draws, a mesh a material, and as the convex hulls
/// it collides with, in meters with Y up about a place, and the texel math and the pictures' sizes
/// they are made with.
/// </summary>
/// <remarks>
/// <para>
/// A brush's faces are the polygons where its planes meet (<see cref="LevelBrush.Polygons"/>), each
/// wound clockwise seen from outside as the file winds them, and taken backwards here, so they run
/// counter-clockwise from outside once their points are turned into meters with Y up. A face's
/// texture coordinates are its points along its axes, scaled and moved as the file says, over the
/// texels its material's picture spans, so a material stays aligned to the world across every face
/// it is on, as TrenchBroom aligns it.
/// </para>
/// <para>
/// What the module gives out is arrays and not an engine's types, and the engine that binds it
/// builds its own meshes and bodies from them and answers for its materials' pictures through
/// <see cref="ILevelMaterials"/> (N 2.1).
/// </para>
/// </remarks>
public static class LevelGeometry
{
    /// <summary>
    /// An entity's brushes as triangles, a mesh each material in the order the faces first name
    /// them.
    /// </summary>
    /// <param name="source">The entity, the world's brushes or a brush entity's.</param>
    /// <param name="unitsPerMeter">How many of the file's units make a meter,
    /// <see cref="Level.UnitsPerMeter"/>.</param>
    /// <param name="origin">The place in meters the corners are given about, the entity's own place
    /// for a brush entity.</param>
    /// <param name="materials">The engine's pictures, each material's size in texels, the level's
    /// units a meter where none is given.</param>
    /// <returns>The meshes, none for an entity with no brushes.</returns>
    public static IReadOnlyList<LevelMesh> Meshes(LevelEntity source, double unitsPerMeter, Vector3 origin, ILevelMaterials? materials = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        var built = new Dictionary<string, (List<Vector3> Positions, List<Vector3> Normals, List<Vector2> Uvs, List<uint> Indices)>(StringComparer.Ordinal);
        var order = new List<string>();
        var sizes = new Dictionary<string, (double Width, double Height)>(StringComparer.Ordinal);
        (double Width, double Height) SizeOf(string material) =>
            sizes.TryGetValue(material, out var known) ? known : sizes[material] = materials?.PictureSize(material) ?? (unitsPerMeter, unitsPerMeter);

        foreach (var brush in source.Brushes)
        {
            var polygons = brush.Polygons();
            for (var f = 0; f < brush.Faces.Count; f++)
            {
                if (polygons[f] is not { } polygon) continue;
                var face = brush.Faces[f];
                if (!built.TryGetValue(face.Material, out var mesh))
                {
                    built[face.Material] = mesh = ([], [], [], []);
                    order.Add(face.Material);
                }

                var (width, height) = SizeOf(face.Material);
                var normal = Vector3.Normalize(face.Normal.Meters(1));
                var first = (uint)mesh.Positions.Count;
                for (var i = 0; i < polygon.Length; i++)
                {
                    var point = polygon[polygon.Length - 1 - i];
                    mesh.Positions.Add(point.Meters(unitsPerMeter) - origin);
                    mesh.Normals.Add(normal);
                    mesh.Uvs.Add(Texel(face, point, width, height));
                }

                // A convex face is a fan of triangles from its first corner.
                for (var i = 1; i + 1 < polygon.Length; i++)
                {
                    mesh.Indices.Add(first);
                    mesh.Indices.Add(first + (uint)i);
                    mesh.Indices.Add(first + (uint)i + 1);
                }
            }
        }

        return [.. order.Select(material => built[material]).Select((mesh, at) =>
            new LevelMesh(order[at], [.. mesh.Positions], [.. mesh.Normals], [.. mesh.Uvs], [.. mesh.Indices]))];
    }

    /// <summary>An entity's brushes as convex hulls, a hull each brush with a solid.</summary>
    /// <param name="source">The entity, the world's brushes or a brush entity's.</param>
    /// <param name="unitsPerMeter">How many of the file's units make a meter.</param>
    /// <param name="origin">The place in meters the corners are given about.</param>
    /// <returns>The hulls, a brush whose planes enclose nothing left out.</returns>
    public static IReadOnlyList<LevelHull> Hulls(LevelEntity source, double unitsPerMeter, Vector3 origin)
    {
        ArgumentNullException.ThrowIfNull(source);
        var hulls = new List<LevelHull>();
        foreach (var brush in source.Brushes)
        {
            // Each corner once, in the order the polygons first give it.
            var seen = new List<LevelPoint>(8);
            foreach (var polygon in brush.Polygons())
            {
                if (polygon is null) continue;
                foreach (var corner in polygon)
                    if (!seen.Contains(corner)) seen.Add(corner);
            }

            if (seen.Count < 4) continue;
            var meters = new Vector3[seen.Count];
            for (var k = 0; k < seen.Count; k++) meters[k] = seen[k].Meters(unitsPerMeter) - origin;
            hulls.Add(new LevelHull(meters));
        }

        return hulls;
    }

    /// <summary>
    /// How far along a ray it first meets any of the brushes, or nothing where it meets none within
    /// its reach, as <see cref="LevelBrush.Hit"/> reads each.
    /// </summary>
    /// <param name="brushes">The brushes, a level's or an entity's.</param>
    /// <param name="from">Where the ray starts, in the file's units.</param>
    /// <param name="way">The way it runs, of any length but none.</param>
    /// <param name="reach">How far it runs, in lengths of <paramref name="way"/>.</param>
    /// <exception cref="ArgumentException">The way has no length, or the reach is below zero or not
    /// a number.</exception>
    public static double? Hit(IEnumerable<LevelBrush> brushes, LevelPoint from, LevelPoint way, double reach)
    {
        ArgumentNullException.ThrowIfNull(brushes);
        double? nearest = null;
        foreach (var brush in brushes)
            if (brush.Hit(from, way, reach) is { } at && (nearest is null || at < nearest)) nearest = at;
        return nearest;
    }

    /// <summary>
    /// Whether a point in the file's units lies inside a brush, behind every one of its faces'
    /// planes.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <param name="point">The point, in the file's units with Z up.</param>
    /// <remarks>
    /// A point on a face, within a millionth of a unit of its plane, lies inside where the face's
    /// way out looks toward lesser X, or along no X toward lesser Y, or along neither toward lesser
    /// Z, the file's axes, and outside where it looks toward greater, so a point on the face two
    /// touching brushes share lies in exactly one of them, as a half-open box holds its low sides
    /// and not its high ones.
    /// </remarks>
    public static bool Inside(LevelBrush brush, LevelPoint point)
    {
        ArgumentNullException.ThrowIfNull(brush);
        foreach (var face in brush.Faces)
        {
            var normal = face.Normal;
            var outside = LevelPoint.Dot(normal, point) - face.Distance;
            if (outside > 1e-6) return false;
            if (outside >= -1e-6 && !TowardLesser(normal)) return false;
        }

        return brush.Faces.Count > 0;
    }

    // Whether a way's first part along the file's X, Y and Z that is not none is below none.
    private static bool TowardLesser(LevelPoint way) =>
        Math.Abs(way.X) > 1e-9 ? way.X < 0 : Math.Abs(way.Y) > 1e-9 ? way.Y < 0 : way.Z < 0;

    /// <summary>
    /// The cells of a grid in meters with Y up a brush fills, each where its middle lies inside the
    /// brush, with the material most of its outer sides are covered by, in order along X, then Y,
    /// then Z.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <param name="cellMeters">A cell's size in meters along each axis, the grid's cells standing
    /// from the level's origin.</param>
    /// <param name="unitsPerMeter">How many of the file's units make a meter,
    /// <see cref="Level.UnitsPerMeter"/>.</param>
    /// <remarks>
    /// <para>
    /// A cell's middle lies inside as <see cref="Inside"/> says. A side of a filled cell is outer
    /// where the cell it faces is not filled by the brush, and each side is covered by the brush's
    /// face whose way out is nearest the side's own, the earlier face in the brush's order where
    /// two are as near.
    /// </para>
    /// <para>
    /// A cell takes the material that covers the most of its outer sides, and of materials that
    /// cover as many the one of the side met first in the order up, down, plus X, minus X, plus Z,
    /// minus Z, so a block on a floor's top is the floor's top and one on its edge the edge's. A
    /// cell with no outer side takes the material of its top side.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">The cell's size or the units a meter is not
    /// above zero.</exception>
    public static IReadOnlyList<LevelCell> Cells(LevelBrush brush, double cellMeters, double unitsPerMeter)
    {
        ArgumentNullException.ThrowIfNull(brush);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cellMeters);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitsPerMeter);
        var polygons = brush.Polygons();
        var (low, high) = (new Vector3(float.MaxValue), new Vector3(float.MinValue));
        foreach (var polygon in polygons)
        {
            if (polygon is null) continue;
            foreach (var corner in polygon)
            {
                var meters = corner.Meters(unitsPerMeter);
                (low, high) = (Vector3.Min(low, meters), Vector3.Max(high, meters));
            }
        }

        // The file's point at a place in meters, in double precision.
        LevelPoint File(double x, double y, double z) => new(x * unitsPerMeter, -z * unitsPerMeter, y * unitsPerMeter);

        var filled = new HashSet<(int, int, int)>();
        var (from, to) = (Index(low, cellMeters, Math.Floor), Index(high, cellMeters, Math.Ceiling));
        for (var x = from.X; x < to.X; x++)
        for (var y = from.Y; y < to.Y; y++)
        for (var z = from.Z; z < to.Z; z++)
            if (Inside(brush, File((x + 0.5) * cellMeters, (y + 0.5) * cellMeters, (z + 0.5) * cellMeters)))
                filled.Add((x, y, z));

        // The cell's six sides in the order a tie is settled by, each by its step to the cell it
        // faces and its way out in the file's axes.
        (int, int, int, LevelPoint)[] sides =
        [
            (0, 1, 0, new LevelPoint(0, 0, 1)), (0, -1, 0, new LevelPoint(0, 0, -1)),
            (1, 0, 0, new LevelPoint(1, 0, 0)), (-1, 0, 0, new LevelPoint(-1, 0, 0)),
            (0, 0, 1, new LevelPoint(0, -1, 0)), (0, 0, -1, new LevelPoint(0, 1, 0)),
        ];
        var cover = new string[sides.Length];
        for (var s = 0; s < sides.Length; s++)
        {
            var (best, near) = (0, double.MinValue);
            for (var f = 0; f < brush.Faces.Count; f++)
            {
                var along = LevelPoint.Dot(brush.Faces[f].Normal, sides[s].Item4);
                if (along > near + 1e-9) (best, near) = (f, along);
            }

            cover[s] = brush.Faces[best].Material;
        }

        var cells = new List<LevelCell>(filled.Count);
        foreach (var (x, y, z) in filled.Order())
        {
            string? chosen = null;
            var most = 0;
            for (var s = 0; s < sides.Length; s++)
            {
                var (dx, dy, dz, _) = sides[s];
                if (filled.Contains((x + dx, y + dy, z + dz))) continue;
                var count = 0;
                for (var t = 0; t < sides.Length; t++)
                {
                    var (ex, ey, ez, _) = sides[t];
                    if (!filled.Contains((x + ex, y + ey, z + ez)) && cover[t] == cover[s]) count++;
                }

                if (count > most) (chosen, most) = (cover[s], count);
            }

            cells.Add(new LevelCell(x, y, z, chosen ?? cover[0]));
        }

        return cells;
    }

    private static (int X, int Y, int Z) Index(Vector3 meters, double cellMeters, Func<double, double> round) =>
        ((int)round(meters.X / cellMeters), (int)round(meters.Y / cellMeters), (int)round(meters.Z / cellMeters));

    /// <summary>
    /// The cells of a grid in meters with Y up an entity's brushes fill, as
    /// <see cref="Cells(LevelBrush, double, double)"/> gives each brush's, a cell two brushes fill
    /// taking the first brush's in the entity's order, in order along X, then Y, then Z.
    /// </summary>
    /// <param name="source">The entity, the world's brushes or a brush entity's.</param>
    /// <param name="cellMeters">A cell's size in meters along each axis.</param>
    /// <param name="unitsPerMeter">How many of the file's units make a meter.</param>
    /// <exception cref="ArgumentOutOfRangeException">The cell's size or the units a meter is not
    /// above zero.</exception>
    public static IReadOnlyList<LevelCell> Cells(LevelEntity source, double cellMeters, double unitsPerMeter)
    {
        ArgumentNullException.ThrowIfNull(source);
        var taken = new Dictionary<(int, int, int), LevelCell>();
        foreach (var brush in source.Brushes)
        foreach (var cell in Cells(brush, cellMeters, unitsPerMeter))
            taken.TryAdd((cell.X, cell.Y, cell.Z), cell);
        return [.. taken.OrderBy(pair => pair.Key).Select(pair => pair.Value)];
    }

    /// <summary>An entity's meshes as one shape of triangles, for a static body.</summary>
    /// <param name="meshes">The meshes, as <see cref="Meshes"/> gives them.</param>
    /// <remarks>
    /// A level collides as its triangles, the shape drawn, and needs no renderer to be built, so a
    /// level collides the same with one and without and a headless test walks it. An engine whose
    /// body takes several shapes may take <see cref="Hulls"/> in its place.
    /// </remarks>
    public static LevelTriangles Triangles(IReadOnlyList<LevelMesh> meshes)
    {
        ArgumentNullException.ThrowIfNull(meshes);
        var positions = new List<Vector3>();
        var indices = new List<uint>();
        foreach (var mesh in meshes)
        {
            var first = (uint)positions.Count;
            positions.AddRange(mesh.Positions);
            indices.AddRange(mesh.Indices.Select(index => first + index));
        }

        return new LevelTriangles([.. positions], [.. indices]);
    }

    /// <summary>
    /// Where a point of a face falls on its material, its texel along each axis, scaled and moved,
    /// over the texels its picture spans each way.
    /// </summary>
    /// <param name="face">The face, its axes, offsets and scales.</param>
    /// <param name="point">The point, in the file's units.</param>
    /// <param name="width">The texels the material's picture spans across.</param>
    /// <param name="height">The texels it spans down.</param>
    public static Vector2 Texel(LevelFace face, LevelPoint point, double width, double height)
    {
        ArgumentNullException.ThrowIfNull(face);
        return new((float)((LevelPoint.Dot(point, face.UAxis) / face.ScaleU + face.UOffset) / width),
            (float)((LevelPoint.Dot(point, face.VAxis) / face.ScaleV + face.VOffset) / height));
    }

    /// <summary>
    /// A PNG's or a JPEG's width and height from its header, or nothing for other bytes.
    /// </summary>
    /// <param name="bytes">The file's bytes, its header at least.</param>
    /// <remarks>
    /// Read from the header alone, so an engine sizing its pictures for a level's texels reads no
    /// picture whole.
    /// </remarks>
    public static (int Width, int Height)? PictureSize(ReadOnlySpan<byte> bytes)
    {
        // A PNG's header chunk holds its width and height as big-endian four byte numbers.
        ReadOnlySpan<byte> png = [0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A];
        if (bytes.Length >= 24 && bytes[..8].SequenceEqual(png))
            return (BinaryPrimitives.ReadInt32BigEndian(bytes[16..]), BinaryPrimitives.ReadInt32BigEndian(bytes[20..]));

        // A JPEG's size is in its start of frame, after the segments before it, each a marker and a
        // length.
        if (bytes.Length < 4 || bytes[0] != 0xFF || bytes[1] != 0xD8) return null;
        for (var at = 2; at + 9 < bytes.Length;)
        {
            if (bytes[at] != 0xFF) return null;
            var marker = bytes[at + 1];
            var length = bytes[at + 2] << 8 | bytes[at + 3];
            if (marker is >= 0xC0 and <= 0xCF and not (0xC4 or 0xC8 or 0xCC)) return (bytes[at + 7] << 8 | bytes[at + 8], bytes[at + 5] << 8 | bytes[at + 6]);
            at += 2 + length;
        }

        return null;
    }
}
