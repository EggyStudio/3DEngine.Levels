namespace Levels;

/// <summary>
/// A tree a level places, by the name of its species and its seed, as
/// <see cref="LevelEntity.Tree"/> reads it from a <c>tree</c> entity's keys.
/// </summary>
/// <param name="Species">The species' name as the level writes it, or nothing where it names
/// none.</param>
/// <param name="Seed">The seed, which picks its variant and turns and sizes it.</param>
/// <remarks>
/// The values are plain, so the module knows no species. An engine's vegetation resolves the name,
/// and the engine says what a tree of no species or of a name it has none for becomes.
/// </remarks>
public readonly record struct LevelTree(string? Species, uint Seed)
{
    /// <summary>The class of a tree.</summary>
    public const string Class = "tree";
}
