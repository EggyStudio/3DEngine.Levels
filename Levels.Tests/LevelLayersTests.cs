using Xunit;

namespace Levels.Tests;

/// <summary>
/// TrenchBroom's layers and groups, entities of the class func_group whose brushes are the
/// world's, read as the world they stand in, and a layer omitted from export left out whole.
/// </summary>
public sealed class LevelLayersTests
{
    private static LevelEntity Entity(string className, params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        entity["classname"] = className;
        foreach (var (key, value) in keys) entity[key] = value;
        return entity;
    }

    private static LevelEntity WithBox(LevelEntity entity, double x)
    {
        entity.Brushes.Add(LevelBrush.Box(new LevelPoint(x, 0, 0), new LevelPoint(x + 32, 32, 32), "proto/wall"));
        return entity;
    }

    // A level as TrenchBroom writes one with a layer, a group in it, a door in the group, and a
    // layer omitted from export holding a light, a group and an entity in that group.
    private static Level Layered()
    {
        var level = new Level();
        level.Entities.Add(WithBox(Entity("worldspawn"), 0));
        level.Entities.Add(WithBox(Entity("func_group", ("_tb_type", "_tb_layer"), ("_tb_name", "Rooms"), ("_tb_id", "1")), 64));
        level.Entities.Add(WithBox(Entity("func_group", ("_tb_type", "_tb_group"), ("_tb_name", "Hall"), ("_tb_id", "2"), ("_tb_layer", "1")), 128));
        level.Entities.Add(Entity("light", ("origin", "0 0 64"), ("_tb_layer", "1")));
        level.Entities.Add(WithBox(Entity("func_door", ("_tb_group", "2")), 192));
        level.Entities.Add(WithBox(Entity("func_group", ("_tb_type", "_tb_layer"), ("_tb_name", "Notes"), ("_tb_id", "3"), ("_tb_layer_omit_from_export", "1")), 256));
        level.Entities.Add(Entity("light", ("origin", "0 0 128"), ("_tb_layer", "3")));
        level.Entities.Add(WithBox(Entity("func_group", ("_tb_type", "_tb_group"), ("_tb_id", "4"), ("_tb_layer", "3")), 320));
        level.Entities.Add(Entity("info_null", ("_tb_group", "4")));
        return level;
    }

    [Fact]
    public void ALayersAndAGroupsBrushesAreTheWorlds()
    {
        var flat = Layered().Flattened();

        Assert.Equal([0.0, 64, 128], flat.World!.Brushes.Select(brush => brush.Faces.Min(face => face.First.X)).Order());
        Assert.Equal(["worldspawn", "light", "func_door"], flat.Entities.Select(entity => entity.ClassName));
        Assert.Single(flat.Entities[2].Brushes);
    }

    [Fact]
    public void ALayerOmittedFromExportIsLeftOutWithEverythingInIt()
    {
        var flat = Layered().Flattened();

        Assert.DoesNotContain(flat.Entities, entity => entity["origin"] == "0 0 128" || entity.ClassName == "info_null");
        Assert.DoesNotContain(flat.World!.Brushes, brush => brush.Faces.Min(face => face.First.X) >= 256);
    }

    [Fact]
    public void ALevelOfLayersWritesBackWithThemSinceFlattenedIsAViewForABinding()
    {
        var level = Layered();
        var text = level.Write();

        level.Flattened();

        Assert.Equal(text, level.Write());
    }

    [Fact]
    public void ALevelOfNoLayersFlattensToItsOwnEntities()
    {
        var level = ExampleLevelTests.Generate();

        Assert.Equal(level.Write(), level.Flattened().Write());
    }

    [Fact]
    public void AWorldOmittedFromExportKeepsItsKeysAndLosesItsBrushes()
    {
        var level = Layered();
        level.World!["_tb_layer_omit_from_export"] = "1";

        Assert.Equal([64.0, 128], level.Flattened().World!.Brushes.Select(brush => brush.Faces.Min(face => face.First.X)).Order());
    }

    [Fact]
    public void ALevelOfNoWorldGainsNoneAndTheWorldKeepsItsPlace()
    {
        var points = new Level();
        points.Entities.Add(Entity("light"));
        Assert.Equal(["light"], points.Flattened().Entities.Select(entity => entity.ClassName));

        var late = new Level();
        late.Entities.Add(Entity("light"));
        late.Entities.Add(WithBox(Entity("worldspawn"), 0));
        Assert.Equal(["light", "worldspawn"], late.Flattened().Entities.Select(entity => entity.ClassName));
    }
}
