using System.Numerics;

namespace Levels;

/// <summary>
/// How a rotating part of a level turns, about an axis through its place, as
/// <see cref="LevelEntity.Spin"/> reads it.
/// </summary>
/// <param name="Axis">The axis in meters with Y up, a length of one.</param>
/// <param name="DegreesPerSecond">How fast it turns about the axis, counterclockwise seen from the
/// axis's tip, below none the other way.</param>
public readonly record struct LevelSpin(Vector3 Axis, double DegreesPerSecond);
