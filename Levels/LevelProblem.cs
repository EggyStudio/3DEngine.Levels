namespace Levels;

/// <summary>
/// A problem of a level, where it stands and what it is, as <see cref="Level.Check"/> gives it.
/// </summary>
/// <param name="Entity">The index of the entity it stands in, among the level's.</param>
/// <param name="Key">The key it stands in, or nothing for a brush or a material.</param>
/// <param name="Brush">The index of the brush it stands in, among the entity's, or nothing for a
/// key.</param>
/// <param name="Message">What is wrong, as a sentence.</param>
public sealed record LevelProblem(int Entity, string? Key, int? Brush, string Message);
