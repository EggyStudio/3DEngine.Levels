namespace Levels;

/// <summary>
/// The tool materials, faces a level editor marks for what they do rather than how they look, which
/// <see cref="LevelGeometry.Meshes"/>, <see cref="LevelGeometry.Collision"/> and
/// <see cref="LevelGeometry.Hulls"/> read by name.
/// </summary>
/// <remarks>
/// A face of <see cref="Clip"/> or <see cref="Nodraw"/> collides and is never drawn, a face of
/// <see cref="Trigger"/> is neither, its brush the volume its entity reads, and a face of
/// <see cref="Skip"/> is left out of every mesh and shape. Any other material is drawn and
/// collides.
/// </remarks>
public static class LevelTools
{
    /// <summary>A face that collides and is never drawn.</summary>
    public const string Clip = "tools/clip";

    /// <summary>A face of a volume its entity reads, neither drawn nor collided with.</summary>
    public const string Trigger = "tools/trigger";

    /// <summary>A face that collides and is never drawn, where no surface is seen.</summary>
    public const string Nodraw = "tools/nodraw";

    /// <summary>A face left out of every mesh and shape.</summary>
    public const string Skip = "tools/skip";

    /// <summary>Whether a face of a material is drawn.</summary>
    /// <param name="material">The material's name.</param>
    public static bool Drawn(string material) => material is not (Clip or Trigger or Nodraw or Skip);

    /// <summary>Whether a face of a material collides.</summary>
    /// <param name="material">The material's name.</param>
    public static bool Collides(string material) => material is not (Trigger or Skip);
}
