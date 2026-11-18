using System.Text;

namespace Levels;

// Writing map text as TrenchBroom lays it out, the game and the format in the first two comments,
// each entity and each brush after a comment numbering it, a keyvalue and a face a line, and each
// number as the shortest text that reads back as the same double, so what this writes reads and
// writes back byte for byte.
public sealed partial class Level
{
    /// <summary>
    /// The level as map text in the Valve 220 format, as TrenchBroom lays it out.
    /// </summary>
    /// <exception cref="InvalidOperationException">The level holds what map text cannot, a key or a
    /// value ending in a backslash, a material that is not one word of the format, a number that is
    /// not finite or a game of more than one line, so the text would read back as another
    /// level.</exception>
    public string Write()
    {
        Writable();
        var text = new StringBuilder();
        text.Append("// Game: ").Append(Game).Append('\n');
        text.Append("// Format: Valve\n");
        for (var e = 0; e < Entities.Count; e++)
        {
            var entity = Entities[e];
            text.Append("// entity ").Append(e).Append("\n{\n");
            entity.AppendKeys(text);
            for (var b = 0; b < entity.Brushes.Count; b++)
            {
                text.Append("// brush ").Append(b).Append("\n{\n");
                foreach (var face in entity.Brushes[b].Faces) Line(text, face);
                text.Append("}\n");
            }

            text.Append("}\n");
        }

        return text.ToString();
    }

    private static void Line(StringBuilder text, LevelFace face)
    {
        static string N(double value) => LevelPoint.Number(value);
        text.Append("( ").Append(face.First).Append(" ) ( ").Append(face.Second).Append(" ) ( ").Append(face.Third).Append(" ) ")
            .Append(face.Material)
            .Append(" [ ").Append(face.UAxis).Append(' ').Append(N(face.UOffset)).Append(" ] [ ")
            .Append(face.VAxis).Append(' ').Append(N(face.VOffset)).Append(" ] ")
            .Append(N(face.Rotation)).Append(' ').Append(N(face.ScaleU)).Append(' ').Append(N(face.ScaleV)).Append('\n');
    }

    // Map text quotes a key and a value with a backslash before a quote inside them, so one ending
    // in a backslash would end in an escaped quote; a material is a word up to a space or a mark of
    // the format, and one that begins with two slashes is a comment; a number is written as digits.
    private void Writable()
    {
        if (Game.Contains('\n') || Game.Contains('\r'))
            throw new InvalidOperationException($"The level's game {Game} is more than one line, which its comment cannot hold.");
        for (var e = 0; e < Entities.Count; e++)
        {
            foreach (var (key, value) in Entities[e].Keys)
            {
                if (key.EndsWith('\\') || value.EndsWith('\\'))
                    throw new InvalidOperationException($"The level's entity {e} has the key {key} or its value {value} ending in a backslash, which map text reads as a quote.");
            }

            for (var b = 0; b < Entities[e].Brushes.Count; b++)
            for (var f = 0; f < Entities[e].Brushes[b].Faces.Count; f++)
            {
                var face = Entities[e].Brushes[b].Faces[f];
                var where = $"The level's entity {e}, brush {b}, face {f}";
                if (face.Material.Length == 0 || face.Material.StartsWith("//", StringComparison.Ordinal)
                    || face.Material.Any(c => char.IsWhiteSpace(c) || c is '"' or '(' or ')' or '[' or ']' or '{' or '}'))
                    throw new InvalidOperationException($"{where} has the material \"{face.Material}\", which is not one word of map text.");
                double[] numbers =
                [
                    face.First.X, face.First.Y, face.First.Z, face.Second.X, face.Second.Y, face.Second.Z,
                    face.Third.X, face.Third.Y, face.Third.Z, face.UAxis.X, face.UAxis.Y, face.UAxis.Z,
                    face.UOffset, face.VAxis.X, face.VAxis.Y, face.VAxis.Z, face.VOffset, face.Rotation,
                    face.ScaleU, face.ScaleV,
                ];
                if (!numbers.All(double.IsFinite))
                    throw new InvalidOperationException($"{where} has a number that is not finite, which map text cannot read.");
            }
        }
    }

    internal static string Quote(string text) => "\"" + text.Replace("\"", "\\\"", StringComparison.Ordinal) + "\"";
}
