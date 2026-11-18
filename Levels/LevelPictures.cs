namespace Levels;

/// <summary>
/// A level's materials by the names both engines read them by, a prototype under
/// <see cref="PrototypeFolder"/> and any other material a picture of its name under
/// <see cref="PictureFolder"/>, so a face repeats over the same texels in either engine.
/// </summary>
/// <remarks>
/// <para>
/// A material <c>proto/floor</c> is drawn as the engine's <c>floor</c> prototype over a picture of
/// <see cref="PrototypePixels"/> a side, which TrenchBroom shows at the same size, so a face of a
/// sixteenth's scale repeats once a meter in both. A material with a PNG or a JPEG of its name
/// under <see cref="PictureFolder"/> is drawn with it over its own pixels. Any other material has
/// no picture, and <see cref="LevelGeometry.Meshes"/> takes the level's units a meter for it, so at
/// a scale of one it repeats once a meter.
/// </para>
/// <para>
/// An engine draws a material by <see cref="Find"/>, a picture's path or a prototype's slot
/// (<see cref="Slot"/>), and gives this to <see cref="LevelGeometry.Meshes"/> as its
/// <see cref="ILevelMaterials"/>.
/// </para>
/// </remarks>
/// <param name="files">The engine's files, which the pictures are searched for in.</param>
public sealed class LevelPictures(ILevelFiles files) : ILevelMaterials
{
    /// <summary>
    /// The folder of a level's materials a prototype stands in, so a face of <c>proto/floor</c> is
    /// drawn as the <c>floor</c> prototype.
    /// </summary>
    public const string PrototypeFolder = "proto/";

    /// <summary>
    /// How many texels a side a prototype's picture has, a meter's cell of the grid at a face scale
    /// of a sixteenth and 32 units a meter.
    /// </summary>
    public const int PrototypePixels = 512;

    /// <summary>
    /// The folder a material's picture is searched for in, relative to the engine's root.
    /// </summary>
    public const string PictureFolder = "textures/";

    /// <summary>The kinds of picture searched for, in the order they are tried.</summary>
    public static IReadOnlyList<string> PictureExtensions { get; } = [".png", ".jpg", ".jpeg"];

    /// <summary>
    /// Whether a material is a prototype, a name under <see cref="PrototypeFolder"/>.
    /// </summary>
    /// <param name="material">The material's name, as the level file writes it.</param>
    public static bool IsPrototype(string material) =>
        material.StartsWith(PrototypeFolder, StringComparison.Ordinal)
        && material.Length > PrototypeFolder.Length;

    /// <summary>
    /// The slot a material is drawn as, a prototype's name without <see cref="PrototypeFolder"/>
    /// and any other name as it is.
    /// </summary>
    /// <param name="material">The material's name, as the level file writes it.</param>
    public static string Slot(string material) =>
        IsPrototype(material) ? material[PrototypeFolder.Length..] : material;

    /// <summary>
    /// The material's picture, a prototype's with no path, the first PNG or JPEG of its name whose
    /// header reads, or nothing.
    /// </summary>
    /// <param name="material">The material's name, as the level file writes it.</param>
    public LevelPicture? Find(string material)
    {
        ArgumentNullException.ThrowIfNull(material);
        if (IsPrototype(material)) return new LevelPicture(null, PrototypePixels, PrototypePixels);

        foreach (var extension in PictureExtensions)
        {
            var path = PictureFolder + material + extension;
            if (files.Exists(path) && LevelGeometry.PictureSize(files.ReadAllBytes(path)) is { } size)
                return new LevelPicture(path, size.Width, size.Height);
        }

        return null;
    }

    /// <inheritdoc/>
    public (double Width, double Height)? PictureSize(string material) =>
        Find(material) is { } picture ? (picture.Width, picture.Height) : null;
}

/// <summary>A material's picture as <see cref="LevelPictures.Find"/> gives it.</summary>
/// <param name="Path">The picture's path among the engine's files, or nothing for a prototype's,
/// which the engine draws itself.</param>
/// <param name="Width">Its width in texels.</param>
/// <param name="Height">Its height in texels.</param>
public readonly record struct LevelPicture(string? Path, int Width, int Height);
