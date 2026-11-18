using System.Globalization;
using System.Text;

namespace Levels;

/// <summary>
/// A level file, its brushes and entities, in the Valve 220 map format TrenchBroom reads and
/// writes.
/// </summary>
/// <remarks>
/// <para>
/// A level is blocked out as brushes, convex solids each a set of planes with a material and a
/// world-aligned mapping a face, and populated with entities, each a class and its keyvalues, as
/// Quake's, Half-Life's and Source's levels are. The file is the map text TrenchBroom writes in its
/// Valve format, so a level is drawn in TrenchBroom and loaded here, and what the engine writes
/// TrenchBroom opens. The engine that binds this module builds it into its scene.
/// </para>
/// <para>
/// Its coordinates are TrenchBroom's, Z up and in units, and the <c>worldspawn</c> entity's
/// <c>_units_per_meter</c> key says how many units make a meter, <see cref="DefaultUnitsPerMeter"/>
/// where it says none, so a 32 unit grid is a meter's. The engine turns them into meters with Y up
/// (<see cref="LevelPoint.Meters"/>).
/// </para>
/// <para>
/// <see cref="Write"/> writes the canonical text, as TrenchBroom lays it out, so a file the module
/// wrote reads and writes back byte for byte, and one TrenchBroom wrote reads back the same
/// brushes, entities and keyvalues in the same order, its numbers written as the shortest text that
/// reads back as the same value.
/// </para>
/// </remarks>
public sealed partial class Level
{
    /// <summary>
    /// How many of a file's units make a meter where its <c>worldspawn</c> says none.
    /// </summary>
    public const double DefaultUnitsPerMeter = 32;

    /// <summary>
    /// The <c>worldspawn</c> key saying how many of the file's units make a meter.
    /// </summary>
    public const string UnitsPerMeterKey = "_units_per_meter";

    /// <summary>The extension a level file is named with.</summary>
    public const string Extension = ".map";

    /// <summary>The entities, <c>worldspawn</c> first where the file has one.</summary>
    public List<LevelEntity> Entities { get; } = [];

    /// <summary>
    /// The <c>worldspawn</c> entity, which holds the level's own brushes, or nothing where there is
    /// none.
    /// </summary>
    public LevelEntity? World => Entities.FirstOrDefault(entity => entity.ClassName == "worldspawn");

    /// <summary>
    /// How many of the file's units make a meter, the <c>worldspawn</c>'s
    /// <see cref="UnitsPerMeterKey"/> or <see cref="DefaultUnitsPerMeter"/>.
    /// </summary>
    /// <exception cref="FormatException">The key is not a number above zero.</exception>
    public double UnitsPerMeter
    {
        get
        {
            if (World?[UnitsPerMeterKey] is not { } said) return DefaultUnitsPerMeter;
            return double.TryParse(said, NumberStyles.Float, CultureInfo.InvariantCulture, out var units) && units > 0 && double.IsFinite(units)
                ? units
                : throw new FormatException($"The level's {UnitsPerMeterKey} is {said}, where a number of units above zero is meant.");
        }
    }

    /// <summary>
    /// Hands each entity to the engine in the level's order with its index, a fault the level's
    /// text causes named by the entity it stands in.
    /// </summary>
    /// <param name="read">What the engine makes of an entity, given its index and the
    /// entity.</param>
    /// <remarks>
    /// A malformed key or a brush that closes no solid is found as the engine reads the entity,
    /// where the reader knows the class and the key but not the entity's place in the file, so a
    /// <see cref="FormatException"/> or an <see cref="InvalidOperationException"/> the reading
    /// throws is given again as a <see cref="FormatException"/> naming the entity's index and
    /// <see cref="LevelEntity.Name"/>, the first fault inside it. Any other exception is the
    /// engine's and goes on as it is.
    /// </remarks>
    /// <exception cref="FormatException">An entity's keys or brushes are not what the reading
    /// takes them as.</exception>
    public void ReadEntities(Action<int, LevelEntity> read)
    {
        ArgumentNullException.ThrowIfNull(read);
        for (var index = 0; index < Entities.Count; index++)
        {
            var entity = Entities[index];
            try
            {
                read(index, entity);
            }
            catch (Exception fault) when (fault is FormatException or InvalidOperationException)
            {
                throw new FormatException($"The level's entity {index}, {entity.Name(index)}, cannot be read. {fault.Message}", fault);
            }
        }
    }

    /// <summary>Reads a level from a file.</summary>
    /// <param name="path">The file, found as the file system finds it.</param>
    /// <remarks>
    /// An engine that reads its levels from its own assets reads them through
    /// <see cref="Load(ILevelFiles, string)"/>, as it gives a level's pictures.
    /// </remarks>
    /// <exception cref="FileNotFoundException">There is no such file.</exception>
    /// <exception cref="FormatException">The file is not map text, the line it fails on
    /// named.</exception>
    public static Level Load(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        if (!File.Exists(path)) throw new FileNotFoundException($"There is no level at {path}.", path);
        return Parse(File.ReadAllText(path), path);
    }

    /// <summary>Reads a level from an engine's files, as its pictures are read.</summary>
    /// <param name="files">The engine's files.</param>
    /// <param name="path">The level's path among them, relative and with forward slashes, which
    /// a fault names.</param>
    /// <remarks>
    /// The bytes are read as UTF-8, as TrenchBroom writes a map, a byte order mark before them
    /// skipped.
    /// </remarks>
    /// <exception cref="FileNotFoundException">The engine has no file at the path.</exception>
    /// <exception cref="FormatException">The file is not map text, the line it fails on
    /// named.</exception>
    public static Level Load(ILevelFiles files, string path)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentException.ThrowIfNullOrEmpty(path);
        if (!files.Exists(path)) throw new FileNotFoundException($"There is no level at {path}.", path);
        return Parse(System.Text.Encoding.UTF8.GetString(files.ReadAllBytes(path)), path);
    }

    /// <summary>
    /// Writes the level to a file, which <see cref="Load(string)"/> reads back the same.
    /// </summary>
    /// <param name="path">The file, found as the file system finds it.</param>
    /// <returns>The file's full path.</returns>
    /// <remarks>
    /// Written under a name of its own and moved into place, so a game or an editor reading it
    /// while it is written reads the old file or the new one whole.
    /// </remarks>
    public string Save(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        var full = Path.GetFullPath(path);
        var folder = Path.GetDirectoryName(full);
        if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);

        var part = full + ".part";
        File.WriteAllText(part, Write(), new UTF8Encoding(false));
        File.Move(part, full, overwrite: true);
        return full;
    }
}
