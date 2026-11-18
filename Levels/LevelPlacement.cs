using System.Numerics;

namespace Levels;

/// <summary>
/// Where a level's entity stands and how it is turned, in meters with Y up about the level's
/// origin, as <see cref="LevelEntity.Placement"/> gives it.
/// </summary>
/// <param name="Position">The place, in meters with Y up.</param>
/// <param name="Rotation">The turn, which takes minus Z to the way the entity faces.</param>
public readonly record struct LevelPlacement(Vector3 Position, Quaternion Rotation);
