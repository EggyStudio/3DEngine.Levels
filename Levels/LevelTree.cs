namespace Levels;

/// <summary>
/// A tree a level places, by the name of its species and its seed, as
/// <see cref="LevelEntity.Tree"/> reads it from a <c>tree</c> entity's keys.
/// </summary>
/// <param name="Species">The species' name, as <c>black-tupelo</c>.</param>
/// <param name="Seed">The seed, which picks its variant and turns and sizes it.</param>
/// <remarks>
/// The values are plain, so the module knows no species. An engine grows the tree through the
/// species it has registered under the name, and refuses a name it has none for.
/// </remarks>
public readonly record struct LevelTree(string Species, uint Seed)
{
    /// <summary>The class of a tree.</summary>
    public const string Class = "tree";

    /// <summary>
    /// The species of a tree with no <c>species</c>, the one TrenchBroom shows a new tree with.
    /// </summary>
    public const string DefaultSpecies = "black-tupelo";
}
