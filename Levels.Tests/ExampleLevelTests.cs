using System.Numerics;
using Levels.Tests.Host;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// The example level, a room of every class the module reads, which a stranger opens in TrenchBroom
/// and loads through the README's binding first. Its file is what the generator below writes, and
/// the tests run with LEVELS_EXAMPLE_WRITE=1 write it again.
/// </summary>
public sealed class ExampleLevelTests
{
    private const string Path = "TrenchBroom/examples/room.map";

    private static string Root
    {
        get
        {
            var at = new DirectoryInfo(AppContext.BaseDirectory);
            while (at is not null && !File.Exists(System.IO.Path.Combine(at.FullName, "Levels.slnx")))
                at = at.Parent;
            return at?.FullName ?? throw new DirectoryNotFoundException("No Levels.slnx above the tests.");
        }
    }

    // A room of 8 by 8 meters at 32 units a meter, its floor, four walls, a doorway to the east
    // with a door in it, a ramp up to a platform, a light over its middle, a spot on the platform,
    // the sun, the player's start by the west wall facing east and a tree in the south-west corner.
    internal static Level Generate()
    {
        // The game the module's GameConfig.cfg names, so TrenchBroom opens it with Levels.fgd.
        var level = new Level { Game = "Levels" };
        var world = new LevelEntity();
        world["mapversion"] = "220";
        world["classname"] = "worldspawn";
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(-128, -128, -16), new LevelPoint(128, 128, 0), "proto/floor"));
        world.Brushes.Add(LevelBrush.Wall(new LevelPoint(-128, 136, 0), new LevelPoint(128, 136, 0), 128, 16, "proto/wall"));
        world.Brushes.Add(LevelBrush.Wall(new LevelPoint(-128, -136, 0), new LevelPoint(128, -136, 0), 128, 16, "proto/wall"));
        world.Brushes.Add(LevelBrush.Wall(new LevelPoint(-136, -144, 0), new LevelPoint(-136, 144, 0), 128, 16, "proto/wall"));
        world.Brushes.AddRange(Level.Doorway(new LevelPoint(0, -8, 0), new LevelPoint(288, 8, 128), 64, 96, "proto/wall")
            .Select(brush => brush.Along(new LevelPoint(136, -144, 0), new LevelPoint(136, 144, 0))));
        world.Brushes.Add(LevelBrush.Ramp(new LevelPoint(-32, 64, 0), new LevelPoint(32, 64, 0), 48, 32, "proto/stairs"));
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(32, 32, 0), new LevelPoint(112, 112, 32), "proto/floor"));
        level.Entities.Add(world);

        var door = new LevelEntity();
        door["classname"] = "func_door";
        door["targetname"] = "door";
        door["origin"] = "136 -32 0";
        door.Brushes.Add(LevelBrush.Box(new LevelPoint(132, -32, 0), new LevelPoint(140, 32, 96), "proto/door"));
        level.Entities.Add(door);

        level.Entities.Add(Point("light", ("origin", "0 0 112"), ("_color", "255 220 180"), ("intensity", "1200"), ("range", "12")));
        level.Entities.Add(Point("light_spot", ("origin", "72 72 120"), ("angles", "90 0 0"), ("intensity", "800"), ("_inner_cone", "20"), ("_cone", "35")));
        level.Entities.Add(Point("light_environment", ("angles", "50 30 0"), ("_color", "255 244 230"), ("intensity", "100000")));
        level.Entities.Add(Point("info_player_start", ("origin", "-96 0 0"), ("angle", "0")));
        level.Entities.Add(Point("tree", ("origin", "-96 -96 0"), ("species", "example-species"), ("seed", "1")));
        return level;
    }

    private static LevelEntity Point(string className, params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        entity["classname"] = className;
        foreach (var (key, value) in keys) entity[key] = value;
        return entity;
    }

    [Fact]
    public void TheExampleIsTheFileItsGeneratorWrites()
    {
        var file = System.IO.Path.Combine(Root, Path);
        var written = Generate().Write();
        if (Environment.GetEnvironmentVariable("LEVELS_EXAMPLE_WRITE") == "1") File.WriteAllText(file, written);

        Assert.True(File.Exists(file) && File.ReadAllText(file).ReplaceLineEndings("\n") == written,
            $"{Path} is not what its generator writes. Run the tests with LEVELS_EXAMPLE_WRITE=1 to write it again.");
    }

    [Fact]
    public void TheReadmesBindingLoadsTheExampleWithEveryClassItReads()
    {
        var kept = new Kept();

        LevelBinding.Load(System.IO.Path.Combine(Root, "TrenchBroom", "examples"), "room.map", kept);

        Assert.Equal(["door", "floor", "stairs", "wall"], kept.Drawn.Select(drawn => drawn.Slot).Distinct().Order());
        Assert.Equal(2, kept.Bodies.Count);
        Assert.Equal([LevelLightKind.Point, LevelLightKind.Spot, LevelLightKind.Directional], kept.Lights.Select(light => light.Kind));
        Assert.Equal(["worldspawn", "func_door", "info_player_start", "tree"], kept.Spawned.Select(spawned => spawned.Entity.ClassName));
        Assert.Equal(new LevelTree("example-species", 1), kept.Spawned[3].Entity.Tree());
        var start = kept.Spawned[2].Placement;
        Assert.True(Vector3.Distance(Vector3.UnitX, Vector3.Transform(-Vector3.UnitZ, start.Rotation)) < 1e-5f);
    }

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
}
