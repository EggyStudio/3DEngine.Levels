namespace Levels;

/// <summary>
/// The files an engine reads a level and its pictures from, by paths relative to its own root,
/// which <see cref="Level.Load(ILevelFiles, string)"/> and <see cref="LevelPictures"/> read
/// (N 2.1).
/// </summary>
/// <remarks>
/// A path is written with forward slashes, as <c>textures/brick.png</c>. The engine maps it onto
/// its own folders or onto what a build carries, so the module opens no file itself.
/// </remarks>
public interface ILevelFiles
{
    /// <summary>Whether the engine has a file at the path.</summary>
    /// <param name="path">The path, relative and with forward slashes.</param>
    bool Exists(string path);

    /// <summary>The file's bytes.</summary>
    /// <param name="path">The path, relative and with forward slashes, one that exists.</param>
    byte[] ReadAllBytes(string path);
}
