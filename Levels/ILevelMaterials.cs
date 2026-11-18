namespace Levels;

/// <summary>
/// What an engine says of a level's materials, the size of the picture each is drawn with, which a
/// face's texture coordinates are worked out over (N 2.1).
/// </summary>
/// <remarks>
/// <see cref="LevelPictures"/> answers by the module's names for a level's materials, over
/// the files an engine gives it, so a level repeats its materials alike in either. An engine that
/// keeps its pictures otherwise answers by its own folders and its own prototypes.
/// </remarks>
public interface ILevelMaterials
{
    /// <summary>
    /// How many texels the picture of a material spans each way, or nothing where the engine has
    /// none for it, which <see cref="LevelGeometry.Meshes"/> then takes as the level's units a
    /// meter, so a face at a scale of one repeats its material once a meter.
    /// </summary>
    /// <param name="material">The material's name, as the level file writes it.</param>
    (double Width, double Height)? PictureSize(string material);
}
