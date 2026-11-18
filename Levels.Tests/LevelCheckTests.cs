using Xunit;

namespace Levels.Tests;

/// <summary>
/// Every problem of a level at once, each named by where it stands, so an editor or a loader shows
/// them all where reading stops at the first.
/// </summary>
public sealed class LevelCheckTests
{
    private sealed class Pictures(params string[] known) : ILevelMaterials
    {
        public (double Width, double Height)? PictureSize(string material) =>
            known.Contains(material) ? (64, 64) : null;
    }

    private sealed class NoFiles : ILevelFiles
    {
        public bool Exists(string path) => false;

        public byte[] ReadAllBytes(string path) => throw new FileNotFoundException(path);
    }

    private static LevelEntity Entity(string className, params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        entity["classname"] = className;
        foreach (var (key, value) in keys) entity.Keys.Add(new(key, value));
        return entity;
    }

    // A level of one of each problem: the world's units a meter, a brush that closes no solid, a
    // start's origin, a light's color and its strength, a spot's cones the wrong way round, a
    // tree's seed, a door's output, and a material no picture is found for.
    private static Level Broken()
    {
        var level = new Level();
        var world = Entity("worldspawn", ("_units_per_meter", "lots"));
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "brick"));
        var open = LevelBrush.Box(new LevelPoint(64, 0, 0), new LevelPoint(128, 64, 32), "brick");
        open.Faces.RemoveAt(0);
        world.Brushes.Add(open);
        world.Brushes.Add(LevelBrush.Box(new LevelPoint(128, 0, 0), new LevelPoint(192, 64, 32), "nowhere"));
        level.Entities.Add(world);
        level.Entities.Add(Entity("info_player_start", ("origin", "0 0")));
        level.Entities.Add(Entity("light", ("_color", "red"), ("intensity", "-5")));
        level.Entities.Add(Entity("light_spot", ("_inner_cone", "40"), ("_cone", "30")));
        level.Entities.Add(Entity("tree", ("seed", "-1")));
        level.Entities.Add(Entity("func_door", ("OnOpen", "lamp,TurnOn,,soon,1")));
        return level;
    }

    [Fact]
    public void EveryProblemIsNamedByItsEntityAndItsKeyOrItsBrush()
    {
        var problems = Broken().Check(new Pictures("brick"));

        Assert.Equal(
            [(0, "_units_per_meter", (int?)null), (0, null, 1), (0, null, 2), (1, "origin", null), (2, "_color", null),
             (2, "intensity", null), (3, "_inner_cone", null), (4, "seed", null), (5, "OnOpen", null)],
            problems.Select(problem => (problem.Entity, problem.Key, problem.Brush)));
        Assert.All(problems, problem => Assert.False(string.IsNullOrWhiteSpace(problem.Message)));
        Assert.Contains("nowhere", problems.Single(problem => problem.Brush == 2).Message);
    }

    [Fact]
    public void AMaterialIsNamedOnceWhereNoPicturesAreGivenNoneIsChecked()
    {
        var level = Broken();
        level.Entities[0].Brushes.Add(LevelBrush.Box(new LevelPoint(256, 0, 0), new LevelPoint(320, 64, 32), "nowhere"));

        Assert.Single(level.Check(new Pictures("brick")), problem => problem.Message.Contains("nowhere"));
        Assert.DoesNotContain(level.Check(), problem => problem.Message.Contains("nowhere"));
    }

    [Fact]
    public void ALevelOfNoProblemHasNone()
    {
        Assert.Empty(ExampleLevelTests.Generate().Check(new LevelPictures(new NoFiles())));
        Assert.Empty(Level.Parse(LevelTests.Room).Check());
    }

    [Fact]
    public void ALevelChecksAsItReadsSoEachProblemIsOneItsReadersRefuse()
    {
        var level = Broken();

        Assert.Throws<FormatException>(() => level.UnitsPerMeter);
        Assert.Throws<InvalidOperationException>(() => level.World!.Brushes[1].Polygons());
        Assert.Throws<FormatException>(() => level.Entities[2].Light(32));
    }

    [Fact]
    public void ASpotsBadColorIsNamedByItsColorAlone()
    {
        var level = new Level();
        level.Entities.Add(Entity("light_spot", ("_color", "red")));

        Assert.Equal(["_color"], level.Check().Select(problem => problem.Key));
    }
}
