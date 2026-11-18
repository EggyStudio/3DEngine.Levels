namespace Levels;

/// <summary>
/// An entity of a level file, its keyvalues in the order the file gives them and the brushes it is
/// made of, if any.
/// </summary>
/// <remarks>
/// <para>
/// The <c>classname</c> key says what it is, <c>worldspawn</c> for the level's own brushes, which
/// the first entity of a file is, and any other class for a thing placed in it, which the engine
/// that binds this module says how to spawn. An entity with brushes is a brush entity, a part of
/// the level that does something, as a door does; one without is a point entity, standing where its
/// <c>origin</c> key says.
/// </para>
/// <para>
/// A key may come more than once, as an output does when a button opens two doors, so the keys are
/// a list and not a dictionary, and a file read and written again keeps them in their order.
/// </para>
/// </remarks>
public sealed partial class LevelEntity
{
    /// <summary>
    /// The keyvalues, in the file's order, a key more than once where the file gives it so.
    /// </summary>
    public List<KeyValuePair<string, string>> Keys { get; } = [];

    /// <summary>The brushes, none for a point entity.</summary>
    public List<LevelBrush> Brushes { get; } = [];

    /// <summary>
    /// The value of its <c>classname</c> key, or an empty string where it has none.
    /// </summary>
    public string ClassName => this["classname"] ?? "";

    /// <summary>
    /// The value of its <c>targetname</c> key, by which another entity's output names it, or an
    /// empty string where it has none.
    /// </summary>
    public string TargetName => this["targetname"] ?? "";

    /// <summary>
    /// What the entity is called where an engine shows it, its <see cref="TargetName"/>, else its
    /// <see cref="ClassName"/>, else <c>entity</c> and its index in the level.
    /// </summary>
    /// <param name="index">Its index among the level's entities, which names one of no name and no
    /// class.</param>
    /// <remarks>
    /// Two entities may share a name, as two lights of one class do, so a name is for a reader and
    /// a console, and the index tells them apart.
    /// </remarks>
    public string Name(int index) =>
        TargetName.Length > 0 ? TargetName : ClassName.Length > 0 ? ClassName : $"entity {index}";

    /// <summary>The first value of a key, or nothing where the entity has no such key.</summary>
    /// <param name="key">The key, as the file writes it, its case kept.</param>
    /// <remarks>
    /// Set, the first such key takes the value, or a key is added at the end where there is none.
    /// </remarks>
    public string? this[string key]
    {
        get
        {
            foreach (var (name, value) in Keys)
                if (name == key) return value;
            return null;
        }

        set
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);
            var at = Keys.FindIndex(pair => pair.Key == key);
            if (at >= 0) Keys[at] = new(key, value);
            else Keys.Add(new(key, value));
        }
    }

    /// <summary>Every value of a key, in the file's order.</summary>
    /// <param name="key">The key.</param>
    public IEnumerable<string> All(string key) => Keys.Where(pair => pair.Key == key).Select(pair => pair.Value);
}
