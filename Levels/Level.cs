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
    /// Every problem of the level at once, each named by the entity it stands in and its key or
    /// its brush, so an editor or a loader shows them all where reading stops at the first.
    /// </summary>
    /// <param name="materials">The engine's materials, whose pictures are looked for, or nothing
    /// to leave the materials unchecked.</param>
    /// <returns>The problems in the level's order, an entity's keys in their order before its
    /// brushes, and none for a level its readers read whole.</returns>
    /// <remarks>
    /// A problem is what a reader of the module refuses: the world's units a meter, an origin, an
    /// angle or angles, a light's color, strength, range and cones, a tree's seed and an output
    /// that do not read, one problem a key, and a brush whose planes close no solid. A material the
    /// engine gives no picture for is named once, at the first brush that has it, since its faces
    /// repeat once a meter where the level may have meant a picture.
    /// </remarks>
    public IReadOnlyList<LevelProblem> Check(ILevelMaterials? materials = null)
    {
        var problems = new List<LevelProblem>();
        var named = new HashSet<string>(StringComparer.Ordinal);
        for (var index = 0; index < Entities.Count; index++)
        {
            var entity = Entities[index];
            if (entity == World && entity[UnitsPerMeterKey] is not null)
            {
                try
                {
                    _ = UnitsPerMeter;
                }
                catch (FormatException fault)
                {
                    problems.Add(new LevelProblem(index, UnitsPerMeterKey, null, fault.Message));
                }
            }

            foreach (var (key, message) in entity.KeyProblems())
                problems.Add(new LevelProblem(index, key, null, message));

            for (var b = 0; b < entity.Brushes.Count; b++)
            {
                var brush = entity.Brushes[b];
                try
                {
                    brush.Polygons();
                }
                catch (InvalidOperationException fault)
                {
                    problems.Add(new LevelProblem(index, null, b, fault.Message));
                    continue;
                }

                if (materials is null) continue;
                foreach (var face in brush.Faces)
                {
                    if (!named.Add(face.Material) || materials.PictureSize(face.Material) is not null) continue;
                    problems.Add(new LevelProblem(index, null, b,
                        $"The brush's material {face.Material} has no picture the engine gives, so its faces repeat once a meter."));
                }
            }
        }

        return problems;
    }

    /// <summary>
    /// The entities whose <c>targetname</c> is a name, as an output's target names them, in the
    /// level's order.
    /// </summary>
    /// <param name="targetName">The name, matched whole and with its case.</param>
    /// <remarks>
    /// A name beginning with <c>!</c>, as <c>!self</c> or <c>!activator</c>, names an entity by
    /// its part in an event rather than by its name, which the engine resolves, so it names none
    /// here, nor does an empty one.
    /// </remarks>
    public IReadOnlyList<LevelEntity> Targeted(string targetName)
    {
        ArgumentNullException.ThrowIfNull(targetName);
        if (targetName.Length == 0 || targetName[0] == '!') return [];
        return [.. Entities.Where(entity => entity.TargetName == targetName)];
    }

    /// <summary>
    /// The level as a binding reads it, TrenchBroom's layers and groups read as the world they
    /// stand in and a layer omitted from export left out whole.
    /// </summary>
    /// <remarks>
    /// <para>
    /// TrenchBroom writes a layer and a group as an entity of the class <c>func_group</c> whose
    /// <c>_tb_type</c> is <c>_tb_layer</c> or <c>_tb_group</c>, its <c>_tb_id</c> naming it, its
    /// brushes the world's, and an entity in one carries the layer's or the group's id in its
    /// <c>_tb_layer</c> or <c>_tb_group</c>, a group in a layer or another group as well. The
    /// view's world holds the <c>worldspawn</c>'s keys and brushes and every layer's and group's
    /// brushes, and its other entities are the level's but the layers and the groups.
    /// </para>
    /// <para>
    /// A layer whose <c>_tb_layer_omit_from_export</c> is <c>1</c> is left out with every brush,
    /// group and entity in it, and the <c>worldspawn</c>'s own brushes where it says so of the
    /// layer it holds. The view shares the level's entities and brushes, and the level is left as
    /// it was, so a level read with layers writes back with them.
    /// </para>
    /// </remarks>
    public Level Flattened()
    {
        static bool Is(LevelEntity entity, string type) => entity.ClassName == "func_group" && entity["_tb_type"] == type;
        var groups = new Dictionary<string, LevelEntity>(StringComparer.Ordinal);
        var omitted = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entity in Entities)
        {
            if (!Is(entity, "_tb_layer") && !Is(entity, "_tb_group")) continue;
            if (entity["_tb_id"] is { } id) groups[(Is(entity, "_tb_layer") ? "layer " : "group ") + id] = entity;
            if (Is(entity, "_tb_layer") && entity["_tb_layer_omit_from_export"] == "1" && entity["_tb_id"] is { } layer) omitted.Add(layer);
        }

        // Whether an entity stands in an omitted layer, through the groups it stands in.
        bool Omitted(LevelEntity entity)
        {
            var seen = new HashSet<LevelEntity>();
            for (var at = entity; at is not null && seen.Add(at);)
            {
                if (Is(at, "_tb_layer")) return at["_tb_id"] is { } own && omitted.Contains(own);
                if (at["_tb_layer"] is { } layer) return omitted.Contains(layer);
                at = at["_tb_group"] is { } group ? groups.GetValueOrDefault("group " + group) : null;
            }

            return false;
        }

        // The world stands where the worldspawn does, or first where a layer's brushes have none.
        var flat = new Level { Game = Game };
        var world = new LevelEntity();
        if (World is { } own)
        {
            world.Keys.AddRange(own.Keys);
            if (own["_tb_layer_omit_from_export"] != "1") world.Brushes.AddRange(own.Brushes);
        }
        else
        {
            world["classname"] = "worldspawn";
        }

        foreach (var entity in Entities)
        {
            if (entity == World) flat.Entities.Add(world);
            else if (Omitted(entity)) continue;
            else if (Is(entity, "_tb_layer") || Is(entity, "_tb_group")) world.Brushes.AddRange(entity.Brushes);
            else flat.Entities.Add(entity);
        }

        if (World is null && world.Brushes.Count > 0) flat.Entities.Insert(0, world);
        return flat;
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
        using var file = File.OpenRead(path);
        return Read(file, path);
    }

    /// <summary>Reads a level from an engine's files, as its pictures are read.</summary>
    /// <param name="files">The engine's files.</param>
    /// <param name="path">The level's path among them, relative and with forward slashes, which
    /// a fault names.</param>
    /// <remarks>
    /// The file is read through <see cref="ILevelFiles.OpenRead"/> as UTF-8, as TrenchBroom writes
    /// a map, a byte order mark before it skipped, and never held as one text.
    /// </remarks>
    /// <exception cref="FileNotFoundException">The engine has no file at the path.</exception>
    /// <exception cref="FormatException">The file is not map text, the line it fails on
    /// named.</exception>
    public static Level Load(ILevelFiles files, string path)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentException.ThrowIfNullOrEmpty(path);
        if (!files.Exists(path)) throw new FileNotFoundException($"There is no level at {path}.", path);
        using var file = files.OpenRead(path);
        return Read(file, path);
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
