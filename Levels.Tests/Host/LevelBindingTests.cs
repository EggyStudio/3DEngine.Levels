using System.Numerics;
using Xunit;

namespace Levels.Tests.Host;

/// <summary>
/// The README's smallest binding run against a level and a folder of pictures with no engine, a
/// world that keeps what it is given, so the build and the suite hold the guide to the module.
/// </summary>
public sealed class LevelBindingTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"levels-binding-{Guid.NewGuid():N}");

    private sealed class Kept : IWorld
    {
        public List<(LevelMesh Mesh, string Slot, LevelPicture? Picture, Vector3 At)> Drawn { get; } = [];
        public List<(LevelTriangles Triangles, Vector3 At)> Bodies { get; } = [];
        public List<LevelLight> Lights { get; } = [];
        public List<(LevelEntity Entity, LevelPlacement Placement)> Spawned { get; } = [];

        public void Draw(LevelMesh mesh, string slot, LevelPicture? picture, Vector3 at) => Drawn.Add((mesh, slot, picture, at));
        public void Collide(LevelTriangles triangles, Vector3 at) => Bodies.Add((triangles, at));
        public void Light(LevelLight light) => Lights.Add(light);
        public void Spawn(LevelEntity entity, LevelPlacement placement) => Spawned.Add((entity, placement));
    }

    public LevelBindingTests()
    {
        // A level of a floor of a picture and a wall of a prototype, a door, a spot, a player's
        // start and a tree, and the floor's picture, a PNG's header of 256 by 128.
        var level = new Level();
        var world = new LevelEntity();
        world["classname"] = "worldspawn";
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(-128, -128, -16), new LevelPoint(128, 128, 0), "brick"));
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(-128, 112, 0), new LevelPoint(128, 128, 96), "proto/wall"));
        level.Entities.Add(world);
        var door = new LevelEntity();
        door["classname"] = "func_door";
        door["origin"] = "-32 108 0";
        door.Brushes.Add(LevelBrush.Box(new LevelPoint(-32, 104, 0), new LevelPoint(32, 112, 96), "proto/door"));
        level.Entities.Add(door);
        level.Entities.Add(Point("light_spot", ("origin", "0 0 96"), ("angles", "90 0 0")));
        level.Entities.Add(Point("info_player_start", ("origin", "0 -64 0"), ("angle", "90")));
        level.Entities.Add(Point("tree", ("origin", "96 -96 0"), ("species", "weeping-willow"), ("seed", "3")));

        Directory.CreateDirectory(Path.Combine(_root, "levels"));
        Directory.CreateDirectory(Path.Combine(_root, "textures"));
        level.Save(Path.Combine(_root, "levels", "room.map"));
        File.WriteAllBytes(Path.Combine(_root, "textures", "brick.png"),
            [0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 13,
             (byte)'I', (byte)'H', (byte)'D', (byte)'R', 0, 0, 1, 0, 0, 0, 0, 128]);
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    private static LevelEntity Point(string className, params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        entity["classname"] = className;
        foreach (var (key, value) in keys) entity[key] = value;
        return entity;
    }

    [Fact]
    public void TheBindingMakesTheLevelsMeshesBodiesLightsAndEntities()
    {
        var kept = new Kept();

        LevelBinding.Load(_root, "levels/room.map", kept);

        Assert.Equal(["brick", "wall", "door"], kept.Drawn.Select(drawn => drawn.Slot));
        Assert.Equal(new LevelPicture("textures/brick.png", 256, 128), kept.Drawn[0].Picture);
        Assert.Equal(new LevelPicture(null, 512, 512), kept.Drawn[1].Picture);
        Assert.Equal(2, kept.Bodies.Count);
        Assert.Equal(kept.Drawn[2].At, kept.Bodies[1].At);
        Assert.True(Vector3.Distance(new Vector3(-1, 0, -3.375f), kept.Bodies[1].At) < 1e-5f);

        var spot = Assert.Single(kept.Lights);
        Assert.Equal(LevelLightKind.Spot, spot.Kind);
        Assert.True(Vector3.Distance(-Vector3.UnitY, spot.Direction) < 1e-5f);

        Assert.Equal(["worldspawn", "func_door", "info_player_start", "tree"],
            kept.Spawned.Select(spawned => spawned.Entity.ClassName));
        Assert.Equal(new LevelTree("weeping-willow", 3), kept.Spawned[3].Entity.Tree());
    }

    [Fact]
    public void TheReadmesBindingIsTheHostsCode()
    {
        var at = new DirectoryInfo(AppContext.BaseDirectory);
        while (at is not null && !File.Exists(Path.Combine(at.FullName, "Levels.slnx"))) at = at.Parent;
        var readme = File.ReadAllText(Path.Combine(at!.FullName, "README.md")).ReplaceLineEndings("\n");
        var host = File.ReadAllText(Path.Combine(at.FullName, "Levels.Tests", "Host", "LevelBinding.cs"))
            .ReplaceLineEndings("\n");

        var code = host.Split("// README begin\n")[1].Split("// README end\n")[0];
        Assert.Contains("```csharp\n" + code + "```\n", readme);
    }

    [Fact]
    public void EveryPublicTypeIsInTheBindingSectionOrARowOfAReadmeTable()
    {
        // A stranger finds every type the library offers from the README alone, by its name in
        // code, whole, as `Level` or `Level.Parse`, and not as the start of `LevelEntity`.
        var at = new DirectoryInfo(AppContext.BaseDirectory);
        while (at is not null && !File.Exists(Path.Combine(at.FullName, "Levels.slnx"))) at = at.Parent;
        var readme = File.ReadAllText(Path.Combine(at!.FullName, "README.md")).ReplaceLineEndings("\n");
        var start = readme.IndexOf("## Binding it in your engine", StringComparison.Ordinal);
        Assert.True(start >= 0, "the README has no section Binding it in your engine");
        var end = readme.IndexOf("\n## ", start + 1, StringComparison.Ordinal);
        var section = readme[start..(end < 0 ? readme.Length : end)];
        var rows = string.Join("\n", readme.Split('\n').Where(line => line.StartsWith('|')));

        var missing = typeof(Level).Assembly.GetExportedTypes()
            .Select(type => type.Name)
            .Where(name => !Named(section, name) && !Named(rows, name))
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(missing.Count == 0,
            $"the README names {string.Join(", ", missing)} neither in Binding it in your engine nor in a table");
    }

    private static bool Named(string text, string name) =>
        System.Text.RegularExpressions.Regex.IsMatch(text, $@"`{name}([`.(<\[ ])");
}
