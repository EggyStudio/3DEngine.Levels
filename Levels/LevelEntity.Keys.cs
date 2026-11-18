using System.Text;

namespace Levels;

public sealed partial class LevelEntity
{
    /// <summary>
    /// The keyvalues as the level file writes them, a line each in their order, which an engine
    /// keeps on what it spawns for the entity and <see cref="ParseKeys"/> reads back.
    /// </summary>
    /// <remarks>
    /// An engine whose components hold text and not a list keeps every key of an entity this way,
    /// so a game reads a key the engine's own classes do not, and a key given twice stays twice.
    /// </remarks>
    public string KeysText()
    {
        var text = new StringBuilder();
        AppendKeys(text);
        return text.ToString();
    }

    /// <summary>The keyvalues of <see cref="KeysText"/>'s text, in their order.</summary>
    /// <param name="text">Keyvalue lines as the level file writes them.</param>
    /// <exception cref="FormatException">The text holds other than keyvalue lines.</exception>
    public static List<KeyValuePair<string, string>> ParseKeys(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var level = Level.Parse("{\n" + text + "\n}\n", "the keys");
        if (level.Entities is not [{ Brushes.Count: 0 } only])
            throw new FormatException("The keys hold a brush or an entity, where keyvalue lines are meant.");
        return only.Keys;
    }

    internal void AppendKeys(StringBuilder text)
    {
        foreach (var (key, value) in Keys)
            text.Append(Level.Quote(key)).Append(' ').Append(Level.Quote(value)).Append('\n');
    }
}
