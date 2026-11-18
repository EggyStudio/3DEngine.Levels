namespace Levels;

/// <summary>
/// A cell of a grid in meters with Y up that a level's brushes fill, by its index along each axis
/// from the level's origin, and the material it is laid with, as
/// <see cref="LevelGeometry.Cells(LevelEntity, double, double)"/> gives it.
/// </summary>
/// <param name="X">Its index along X, the cell spanning X times the cell's size to one
/// more.</param>
/// <param name="Y">Its index up.</param>
/// <param name="Z">Its index along Z.</param>
/// <param name="Material">The material of the brush's face most of its outer sides are covered
/// by.</param>
public readonly record struct LevelCell(int X, int Y, int Z, string Material);
