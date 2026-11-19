using System.Numerics;

namespace Levels;

/// <summary>
/// The world's sky and its light where no light reaches, as <see cref="LevelEntity.Ambient"/>
/// reads them from the <c>worldspawn</c>'s keys.
/// </summary>
/// <param name="Sky">The sky's name, which the engine resolves, or nothing for none.</param>
/// <param name="Color">The ambient light's color in linear sRGB, each channel from 0 to 1.</param>
/// <param name="Lux">The lux it gives every surface, 0 for none.</param>
public readonly record struct LevelAmbient(string? Sky, Vector3 Color, double Lux);
