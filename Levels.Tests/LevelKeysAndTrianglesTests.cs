using Xunit;

namespace Levels.Tests;

/// <summary>
/// An entity's keyvalues as the text an engine keeps them in and back, and an entity's meshes as
/// one shape of triangles for a body.
/// </summary>
public sealed class LevelKeysAndTrianglesTests
{
    [Fact]
    public void AnEntitysKeysReadBackFromTheirTextInTheirOrder()
    {
        var door = Level.Parse(LevelTests.Room).Entities.Single(entity => entity.ClassName == "func_door");

        var text = door.KeysText();

        Assert.StartsWith("\"classname\" \"func_door\"\n", text);
        Assert.Equal(door.Keys, LevelEntity.ParseKeys(text));
        Assert.Equal(["lamp,TurnOn,,0,-1", "chime,Play,,0.5,1"], LevelEntity.ParseKeys(text)
            .Where(pair => pair.Key == "OnOpen").Select(pair => pair.Value));
    }

    [Fact]
    public void AKeysTextIsTheLinesTheLevelFileWritesForIt()
    {
        var level = Level.Parse(LevelTests.Room);
        var written = level.Write();

        foreach (var entity in level.Entities) Assert.Contains("{\n" + entity.KeysText(), written);
    }

    [Fact]
    public void AValueHoldingAQuoteReadsBackWhole()
    {
        var sign = new LevelEntity();
        sign["message"] = "Say \"open\" at the door";

        Assert.Equal(sign.Keys, LevelEntity.ParseKeys(sign.KeysText()));
    }

    [Fact]
    public void AnEntityOfNoKeysIsAnEmptyText()
    {
        Assert.Equal("", new LevelEntity().KeysText());
        Assert.Empty(LevelEntity.ParseKeys(""));
    }

    [Fact]
    public void ATextHoldingABrushIsRefused()
    {
        var floor = Level.Parse(LevelTests.Room).Write().Split("// brush 0\n")[1].Split("}\n")[0];

        Assert.Throws<FormatException>(() => LevelEntity.ParseKeys("{\n" + floor + "}\n"));
    }

    [Fact]
    public void AnEntitysMeshesAreOneShapeEachMeshsCornersAfterTheOnesBefore()
    {
        var world = Level.Parse(LevelTests.Room).World!;
        var meshes = LevelGeometry.Meshes(world, 32, default);

        var shape = LevelGeometry.Triangles(meshes);

        Assert.Equal(meshes.SelectMany(mesh => mesh.Positions), shape.Positions);
        Assert.Equal(meshes.Sum(mesh => mesh.Indices.Length), shape.Indices.Length);
        var first = 0;
        var at = 0;
        // Each triangle of each mesh names the same corners in the shape, by the mesh's indices
        // moved past the corners before it.
        foreach (var mesh in meshes)
        {
            foreach (var index in mesh.Indices)
                Assert.Equal(mesh.Positions[index], shape.Positions[shape.Indices[at++]]);
            first += mesh.Positions.Length;
        }

        Assert.Equal(shape.Positions.Length, first);
    }
}
