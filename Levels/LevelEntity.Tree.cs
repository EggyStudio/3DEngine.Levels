namespace Levels;

public sealed partial class LevelEntity
{
    /// <summary>
    /// The tree the entity places, read from its keys, or nothing where its class is not
    /// <c>tree</c>.
    /// </summary>
    /// <remarks>
    /// The keys are <c>species</c>, a species' name, <see cref="LevelTree.DefaultSpecies"/> where
    /// it is missing or empty, and <c>seed</c>, a whole number, zero where it is missing. The tree
    /// stands where <see cref="Placement"/> says, turned by its angles.
    /// </remarks>
    /// <exception cref="FormatException">The seed is not a whole number of zero or
    /// more.</exception>
    public LevelTree? Tree()
    {
        if (ClassName != LevelTree.Class) return null;
        var species = this["species"] is { Length: > 0 } named ? named : LevelTree.DefaultSpecies;
        return new LevelTree(species, WholeNumber("seed") ?? 0);
    }
}
