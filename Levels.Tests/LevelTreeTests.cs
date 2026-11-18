using Xunit;

namespace Levels.Tests;

/// <summary>
/// A tree a level places, read from its keys as plain values an engine grows it from.
/// </summary>
public sealed class LevelTreeTests
{
    private static LevelEntity Entity(params (string Key, string Value)[] keys)
    {
        var entity = new LevelEntity();
        foreach (var (key, value) in keys) entity.Keys.Add(new(key, value));
        return entity;
    }

    [Fact]
    public void ATreeIsItsSpeciesAndItsSeed()
    {
        Assert.Equal(new LevelTree("weeping-willow", 7),
            Entity(("classname", "tree"), ("species", "weeping-willow"), ("seed", "7")).Tree());
    }

    [Fact]
    public void ATreeOfNoSpeciesHasNoneForTheEngineToResolve()
    {
        Assert.Equal(new LevelTree(null, 0), Entity(("classname", "tree")).Tree());
        Assert.Equal(new LevelTree(null, 3), Entity(("classname", "tree"), ("species", ""), ("seed", "3")).Tree());
    }

    [Fact]
    public void AnUnknownSpeciesIsReadAsItIsForTheEngineToRefuse()
    {
        Assert.Equal("no-such-tree", Entity(("classname", "tree"), ("species", "no-such-tree")).Tree()?.Species);
    }

    [Fact]
    public void ASeedThatIsNotAWholeNumberIsRefused()
    {
        Assert.Throws<FormatException>(() => Entity(("classname", "tree"), ("seed", "-1")).Tree());
    }

    [Fact]
    public void AnEntityOfAnotherClassIsNoTree()
    {
        Assert.Null(Entity(("classname", "light")).Tree());
    }

    [Fact]
    public void TheRoomsTreeIsRead()
    {
        var tree = Level.Parse(LevelTests.Room).Entities.Single(entity => entity.ClassName == "tree");

        Assert.Equal(new LevelTree("black-tupelo", 3), tree.Tree());
    }
}
