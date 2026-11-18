using Xunit;

namespace Levels.Tests;

/// <summary>
/// Tool materials, faces marked for what they do: clip and nodraw collide unseen, trigger faces
/// are their entity's volume, and skip faces are left out of everything.
/// </summary>
public sealed class LevelToolsTests
{
    // A box of six faces, the first two drawn, then one of each tool material.
    private static LevelEntity Box()
    {
        var box = LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 64), "brick");
        string[] materials = ["brick", "brick", LevelTools.Clip, LevelTools.Nodraw, LevelTools.Trigger, LevelTools.Skip];
        var marked = new LevelBrush();
        for (var f = 0; f < box.Faces.Count; f++) marked.Faces.Add(box.Faces[f] with { Material = materials[f] });
        var entity = new LevelEntity();
        entity["classname"] = "worldspawn";
        entity.Brushes.Add(marked);
        return entity;
    }

    [Fact]
    public void OnlyFacesOfOtherMaterialsAreDrawn()
    {
        var mesh = Assert.Single(LevelGeometry.Meshes(Box(), 32, default));

        Assert.Equal("brick", mesh.Material);
        Assert.Equal(2 * 6, mesh.Indices.Length);
    }

    [Fact]
    public void ClipAndNodrawCollideWithTheDrawnFacesAndTriggerAndSkipDoNot()
    {
        Assert.Equal(4 * 6, LevelGeometry.Collision(Box(), 32, default).Indices.Length);
    }

    [Fact]
    public void ABrushAllTriggerIsAVolumeAndNoHull()
    {
        var trigger = new LevelEntity();
        trigger["classname"] = "trigger_once";
        trigger.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 64), LevelTools.Trigger));
        var clip = new LevelEntity();
        clip.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 64), LevelTools.Clip));

        Assert.Empty(LevelGeometry.Hulls(trigger, 32, default));
        Assert.Empty(LevelGeometry.Meshes(trigger, 32, default));
        Assert.Single(LevelGeometry.Hulls(clip, 32, default));
        Assert.Empty(LevelGeometry.Meshes(clip, 32, default));
    }

    [Theory]
    [InlineData("brick", true, true)]
    [InlineData(LevelTools.Clip, false, true)]
    [InlineData(LevelTools.Nodraw, false, true)]
    [InlineData(LevelTools.Trigger, false, false)]
    [InlineData(LevelTools.Skip, false, false)]
    public void EachMaterialIsDrawnAndCollidesAsItsNameSays(string material, bool drawn, bool collides)
    {
        Assert.Equal((drawn, collides), (LevelTools.Drawn(material), LevelTools.Collides(material)));
    }
}
