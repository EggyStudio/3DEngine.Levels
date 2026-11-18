using System.Diagnostics;
using Levels;

// A made level at each count of brushes the command line gives, 100, 10,000 and 100,000 where it
// gives none, through every reader and writer the module holds: written and read as text, saved and
// loaded as a file, each brush's polygons, the meshes, the hulls and the triangles, and every
// entity's place, light, tree and keys. A brush in a hundred belongs to a door, and the level
// holds a light, a spot and a tree for each hundred brushes. Each step runs once untimed, so the
// compiler's work is not counted, and then up to five times within a few seconds, of which the
// best is printed with the bytes it allocated. The level read back is held to the one written, its
// text written again to the byte and its file loaded to the same text, and a difference ends the
// run.
int[] counts = args.Length > 0 ? [.. args.Select(int.Parse)] : [100, 10_000, 100_000];
var folder = Path.Combine(Path.GetTempPath(), $"levels-bench-{Environment.ProcessId}");
Directory.CreateDirectory(folder);
try
{
    Console.WriteLine("| Brushes | Step | ms | MiB allocated |");
    Console.WriteLine("|---|---|---|---|");
    foreach (var count in counts) Read(count, Path.Combine(folder, $"level-{count}.map"));
}
finally
{
    Directory.Delete(folder, recursive: true);
}

// One brush of many faces, a prism of n sides and its two ends, since a brush's corners are found
// from every three of its planes and every other plane, which grows as the faces' count to the
// fourth.
foreach (var sides in new[] { 16, 32, 64, 128 })
{
    var brush = Prism(sides);
    brush.Polygons();
    var watch = Stopwatch.StartNew();
    brush.Polygons();
    Console.WriteLine($"a brush of {sides + 2} faces {watch.Elapsed.TotalMilliseconds:F1} ms");
}

Console.WriteLine($"peak working set {Process.GetCurrentProcess().PeakWorkingSet64 / 1048576.0:F0} MiB");

static void Read(int count, string path)
{
    var level = Made(count);
    var text = "";
    var read = level;
    IReadOnlyList<LevelMesh> meshes = [];

    void Time(string name, Action step)
    {
        step();
        var best = double.MaxValue;
        long bytes = 0;
        var spent = Stopwatch.StartNew();
        for (var run = 0; run < 5 && (run == 0 || spent.Elapsed.TotalSeconds < 5); run++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var before = GC.GetAllocatedBytesForCurrentThread();
            var watch = Stopwatch.StartNew();
            step();
            watch.Stop();
            bytes = GC.GetAllocatedBytesForCurrentThread() - before;
            best = Math.Min(best, watch.Elapsed.TotalMilliseconds);
        }

        Console.WriteLine($"| {count:N0} | {name} | {best:F1} | {bytes / 1048576.0:F1} |");
    }

    Time("Level.Write", () => text = level.Write());
    Time("Level.Parse", () => read = Level.Parse(text));
    // What the level read holds, beside what reading it allocated, so the reading's own cost is
    // the difference.
    var held = GC.GetTotalMemory(forceFullCollection: true);
    var kept = Level.Parse(text);
    var keeps = GC.GetTotalMemory(forceFullCollection: true) - held;
    GC.KeepAlive(kept);
    Console.WriteLine($"| {count:N0} | the level Level.Parse keeps | | {keeps / 1048576.0:F1} |");

    Time("Level.Save", () => level.Save(path));
    Time("Level.Load", () => read = Level.Load(path));
    Time("LevelBrush.Polygons, every brush", () =>
    {
        foreach (var entity in read.Entities)
        foreach (var brush in entity.Brushes) brush.Polygons();
    });
    Time("LevelGeometry.Meshes, every entity", () =>
    {
        foreach (var entity in read.Entities) meshes = LevelGeometry.Meshes(entity, read.UnitsPerMeter, entity.Placement(read.UnitsPerMeter).Position);
    });
    Time("LevelGeometry.Hulls, every entity", () =>
    {
        foreach (var entity in read.Entities) LevelGeometry.Hulls(entity, read.UnitsPerMeter, default);
    });
    meshes = LevelGeometry.Meshes(read.World!, read.UnitsPerMeter, default);
    Time("LevelGeometry.Triangles, the world", () => LevelGeometry.Triangles(meshes));
    Time("Placement, Light and Tree, every entity", () =>
    {
        foreach (var entity in read.Entities)
        {
            entity.Placement(read.UnitsPerMeter);
            entity.Light(read.UnitsPerMeter);
            entity.Tree();
        }
    });
    Time("KeysText and ParseKeys, every entity", () =>
    {
        foreach (var entity in read.Entities) LevelEntity.ParseKeys(entity.KeysText());
    });

    if (read.Write() != text) throw new InvalidOperationException($"The level of {count} brushes reads back as another.");
    if (Level.Parse(text).Write() != text) throw new InvalidOperationException($"The text of {count} brushes writes back as another.");
    var faces = read.Entities.Sum(entity => entity.Brushes.Sum(brush => brush.Faces.Count));
    Console.WriteLine($"| {count:N0} | the level | {read.Entities.Count:N0} entities, {faces:N0} faces, "
        + $"{meshes.Sum(mesh => mesh.Indices.Length) / 3:N0} triangles in the world, {text.Length / 1e6:F2} million characters, "
        + $"{new FileInfo(path).Length / 1048576.0:F1} MiB on disk | round trip equal |");
}

// The level, on a grid of brushes 128 units apart, a third each boxes, ramps and boxes turned by 30
// degrees, every hundredth a door's, and a light, a spot and a tree for each hundred brushes.
static Level Made(int count)
{
    var level = new Level();
    var world = new LevelEntity();
    world["classname"] = "worldspawn";
    level.Entities.Add(world);
    var side = (int)Math.Ceiling(Math.Sqrt(count));
    for (var i = 0; i < count; i++)
    {
        var low = new LevelPoint(i % side * 128, i / side * 128, 0);
        var high = low + new LevelPoint(64, 64, 32 + i % 7 * 16);
        var brush = (i % 3) switch
        {
            0 => LevelBrush.Box(low, high, $"mat{i % 20}"),
            1 => LevelBrush.Wedge(low, high, $"mat{i % 20}"),
            _ => LevelBrush.Box(low, high, $"mat{i % 20}").Turned(30, low),
        };
        if (i % 100 != 99)
        {
            world.Brushes.Add(brush);
            continue;
        }

        var door = new LevelEntity();
        door["classname"] = "func_door";
        door["targetname"] = $"door {i}";
        door["OnOpen"] = $"lamp {i},TurnOn,,0,-1";
        door.Brushes.Add(brush);
        level.Entities.Add(door);
        var at = $"{low.X + 32} {low.Y + 32} {high.Z + 64}";
        level.Entities.Add(Point("light", ("origin", at), ("_color", "255 200 150"), ("intensity", "1200")));
        level.Entities.Add(Point("light_spot", ("origin", at), ("angles", "90 0 0"), ("_cone", "40")));
        level.Entities.Add(Point("tree", ("origin", $"{low.X} {low.Y - 32} 0"), ("species", "black-tupelo"), ("seed", $"{i}")));
    }

    return level;
}

static LevelEntity Point(string className, params (string Key, string Value)[] keys)
{
    var entity = new LevelEntity();
    entity["classname"] = className;
    foreach (var (key, value) in keys) entity[key] = value;
    return entity;
}

static LevelBrush Prism(int sides)
{
    var brush = new LevelBrush();
    void Add(LevelPoint outward, LevelPoint at, LevelPoint along, LevelPoint across)
    {
        var face = new LevelFace(at, at + along, at + across, "wall", new(1, 0, 0), 0, new(0, -1, 0), 0, 0, 1, 1);
        if (LevelPoint.Dot(face.Normal, outward) < 0) face = face with { First = face.Third, Third = face.First };
        brush.Faces.Add(face);
    }

    for (var i = 0; i < sides; i++)
    {
        var turn = 2 * Math.PI * i / sides;
        var outward = new LevelPoint(Math.Cos(turn), Math.Sin(turn), 0);
        Add(outward, outward * 256, new LevelPoint(-Math.Sin(turn), Math.Cos(turn), 0) * 16, new LevelPoint(0, 0, 16));
    }

    Add(new(0, 0, 1), new(0, 0, 64), new(16, 0, 0), new(0, 16, 0));
    Add(new(0, 0, -1), new(0, 0, 0), new(16, 0, 0), new(0, 16, 0));
    return brush;
}
