using System.Diagnostics;
using Levels;

// A level of 10,000 brushes in one world, a third each boxes, ramps and boxes turned by 30
// degrees, on a grid of 100 by 100, read, written and built into meshes and hulls. Each step runs
// once untimed, so the compiler's work is not counted, and then five times, of which the best is
// printed with the bytes it allocated.
var level = new Level();
var world = new LevelEntity();
world["classname"] = "worldspawn";
for (var i = 0; i < 10_000; i++)
{
    var low = new LevelPoint(i % 100 * 128, i / 100 * 128, 0);
    var high = low + new LevelPoint(64, 64, 32 + i % 7 * 16);
    world.Brushes.Add((i % 3) switch
    {
        0 => LevelBrush.Box(low, high, $"mat{i % 20}"),
        1 => LevelBrush.Wedge(low, high, $"mat{i % 20}"),
        _ => LevelBrush.Box(low, high, $"mat{i % 20}").Turned(30, low),
    });
}

level.Entities.Add(world);

var text = "";
var read = level;
IReadOnlyList<LevelMesh> meshes = [];
var rows = new List<(string Name, double Milliseconds, long Bytes)>();

void Time(string name, Action step)
{
    step();
    var best = double.MaxValue;
    long bytes = 0;
    for (var run = 0; run < 5; run++)
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

    rows.Add((name, best, bytes));
}

Time("Level.Write", () => text = level.Write());
Time("Level.Parse", () => read = Level.Parse(text));
Time("LevelBrush.Polygons, every brush", () => { foreach (var brush in read.World!.Brushes) brush.Polygons(); });
Time("LevelGeometry.Meshes", () => meshes = LevelGeometry.Meshes(read.World!, 32, default));
Time("LevelGeometry.Hulls", () => LevelGeometry.Hulls(read.World!, 32, default));
Time("LevelGeometry.Triangles", () => LevelGeometry.Triangles(meshes));

Console.WriteLine($"{text.Length / 1e6:F2} million characters, {read.World!.Brushes.Sum(brush => brush.Faces.Count)} faces, "
    + $"{meshes.Sum(mesh => mesh.Indices.Length) / 3} triangles");
foreach (var (name, milliseconds, bytes) in rows)
    Console.WriteLine($"{name,-36} {milliseconds,9:F1} ms {bytes / 1048576.0,9:F1} MiB allocated");

// One brush of many faces, a prism of n sides and its two ends, since a brush's corners are found
// from every three of its planes and every other plane, which grows as the faces' count to the
// fourth.
foreach (var sides in new[] { 16, 32, 64, 128 })
{
    var brush = Prism(sides);
    brush.Polygons();
    var watch = Stopwatch.StartNew();
    brush.Polygons();
    Console.WriteLine($"a brush of {sides + 2} faces {watch.Elapsed.TotalMilliseconds,9:F1} ms");
}

Console.WriteLine($"peak working set {Process.GetCurrentProcess().PeakWorkingSet64 / 1048576.0:F0} MiB");

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
