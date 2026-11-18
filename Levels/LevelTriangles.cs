using System.Numerics;

namespace Levels;

/// <summary>
/// An entity's triangles as one shape, in meters with Y up about the place its meshes were given
/// about, which an engine makes a static body of, as <see cref="LevelGeometry.Triangles"/> gives
/// them.
/// </summary>
/// <param name="Positions">The corners, each mesh's after the ones before.</param>
/// <param name="Indices">Three corners a triangle, counter-clockwise seen from outside the
/// brush.</param>
public sealed record LevelTriangles(Vector3[] Positions, uint[] Indices);
