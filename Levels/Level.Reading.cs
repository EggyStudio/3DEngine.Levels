using System.Globalization;
using System.Text;

namespace Levels;

// Reading map text, entities in braces holding quoted keyvalues and brushes, brushes in braces
// holding a face a line, and comments from two slashes to the end of a line, the whole read token
// by token so a writer that puts two braces on a line is read as one that does not. A token is
// where it stands in the text, a number is read from there, and a string is made only for a key, a
// value or a material, one string a material however many faces name it, so a large level's
// reading holds the text and the level and little else.
public sealed partial class Level
{
    /// <summary>
    /// The game TrenchBroom's first comment names, which a file written again keeps, or
    /// <c>Generic</c> where it names none.
    /// </summary>
    public string Game { get; set; } = "Generic";

    /// <summary>Reads a level from map text.</summary>
    /// <param name="text">The text, in the Valve 220 format.</param>
    /// <param name="name">What the text is called in an error, as its file's path.</param>
    /// <exception cref="FormatException">The text is not map text, the line it fails on
    /// named.</exception>
    /// <remarks>
    /// A face is read as Valve 220 writes it, three points in parentheses, a material, two axes in
    /// brackets, each with its offset, a rotation and two scales. A face in Quake's older format,
    /// with no axes, is refused, since its mapping depends on rules the module does not follow;
    /// TrenchBroom converts a level to the Valve format from its map menu.
    /// </remarks>
    public static Level Parse(string text, string name = "the level")
    {
        ArgumentNullException.ThrowIfNull(text);
        return Read(new Reader(text, name));
    }

    /// <summary>
    /// Reads a level from a stream of map text, through a window of it, so a large file is never
    /// held as one text.
    /// </summary>
    /// <param name="text">The text, UTF-8 where it has no byte order mark saying otherwise.</param>
    /// <param name="name">What the text is called in an error, as its file's path.</param>
    /// <exception cref="FormatException">The text is not map text, the line it fails on
    /// named.</exception>
    public static Level Read(Stream text, string name = "the level")
    {
        ArgumentNullException.ThrowIfNull(text);
        using var reader = new StreamReader(text, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1 << 16, leaveOpen: true);
        return Read(new Reader(reader, name));
    }

    private static Level Read(Reader reader)
    {
        var level = new Level();
        if (reader.Game is { } game) level.Game = game;

        while (reader.Next() is { } token)
        {
            if (!reader.Is(token, '{')) throw reader.Fault(token, $"an entity's opening brace is meant, where it reads {reader.Text(token)}");
            level.Entities.Add(ReadEntity(reader));
        }

        return level;
    }

    private static LevelEntity ReadEntity(Reader reader)
    {
        var entity = new LevelEntity();
        while (true)
        {
            var token = reader.Next() ?? throw reader.Fault(null, "the file ends inside an entity");
            if (reader.Is(token, '}')) return entity;
            if (reader.Is(token, '{'))
            {
                entity.Brushes.Add(ReadBrush(reader));
                continue;
            }

            if (!token.Quoted) throw reader.Fault(token, $"a quoted key, a brush or the entity's closing brace is meant, where it reads {reader.Text(token)}");
            var value = reader.Next() ?? throw reader.Fault(null, $"the file ends after the key {reader.Text(token)}");
            if (!value.Quoted) throw reader.Fault(value, $"the key {reader.Text(token)} is given {reader.Text(value)}, where a quoted value is meant");
            entity.Keys.Add(new(reader.Text(token), reader.Text(value)));
        }
    }

    private static LevelBrush ReadBrush(Reader reader)
    {
        var brush = new LevelBrush();
        while (true)
        {
            var token = reader.Next() ?? throw reader.Fault(null, "the file ends inside a brush");
            if (reader.Is(token, '}')) return brush;
            if (!reader.Is(token, '(')) throw reader.Fault(token, $"a face's first point or the brush's closing brace is meant, where it reads {reader.Text(token)}");
            brush.Faces.Add(ReadFace(reader, token.Line));
        }
    }

    private static LevelFace ReadFace(Reader reader, int line)
    {
        var first = ReadPoint(reader, opened: true);
        var second = ReadPoint(reader, opened: false);
        var third = ReadPoint(reader, opened: false);
        var material = reader.Next() ?? throw reader.Fault(null, "the file ends before a face's material");
        if (reader.Is(material, '{') || reader.Is(material, '}') || reader.Is(material, '(') || reader.Is(material, '[') || material.Length == 0)
            throw reader.Fault(material, $"a face's material is meant, where it reads {reader.Text(material)}");

        // Its name is taken now, since an unquoted token's place in a stream's window lasts only
        // until the next is read.
        var name = reader.Material(material);

        var opening = reader.Next();
        if (opening is not { } open || !reader.Is(open, '['))
            throw reader.Fault(opening, "a face's two axes in brackets are meant after its material, as the Valve 220 format writes them, so this is Quake's older format, which TrenchBroom converts to the Valve format from its map menu");
        var (uAxis, uOffset) = ReadAxis(reader);
        Expect(reader, '[', "a face's second axis");
        var (vAxis, vOffset) = ReadAxis(reader);
        var rotation = reader.Number("a face's rotation");
        var scaleU = reader.Number("a face's first scale");
        var scaleV = reader.Number("a face's second scale");
        if (reader.Peek() is { Quoted: false } extra && extra.Line == line && !reader.Is(extra, '(') && !reader.Is(extra, '}'))
            throw reader.Fault(extra, $"a face ends at its two scales, where it goes on with {reader.Text(extra)}");

        return new LevelFace(first, second, third, name, uAxis, uOffset, vAxis, vOffset, rotation, scaleU, scaleV);
    }

    private static LevelPoint ReadPoint(Reader reader, bool opened)
    {
        if (!opened) Expect(reader, '(', "a face's next point");
        var point = new LevelPoint(reader.Number("a point's x"), reader.Number("a point's y"), reader.Number("a point's z"));
        Expect(reader, ')', "a point's closing parenthesis");
        return point;
    }

    private static (LevelPoint Axis, double Offset) ReadAxis(Reader reader)
    {
        var axis = new LevelPoint(reader.Number("an axis's x"), reader.Number("an axis's y"), reader.Number("an axis's z"));
        var offset = reader.Number("an axis's offset");
        Expect(reader, ']', "an axis's closing bracket");
        return (axis, offset);
    }

    private static void Expect(Reader reader, char mark, string what)
    {
        var token = reader.Next();
        if (token is not { } read || !reader.Is(read, mark))
            throw reader.Fault(token, $"{what}, {mark}, is meant, where it reads {(token is { } t ? reader.Text(t) : "the file's end")}");
    }

    /// <summary>
    /// A token of map text, where it stands in the reader's window, whether it was quoted, its
    /// line, and a quoted text's string, an unquoted token's place good until the next is read.
    /// </summary>
    private readonly record struct Token(int Start, int Length, bool Quoted, int Line, string? Unescaped = null);

    /// <summary>
    /// Reads map text token by token, keeping the line each began on, from a whole text in place or
    /// from a stream through a window that keeps the token being read as it moves on.
    /// </summary>
    private sealed class Reader
    {
        private const int Window = 1 << 16;

        private readonly string? _whole;
        private readonly TextReader? _source;
        private char[] _buffer = [];
        private int _end;
        private readonly string _name;
        private readonly Dictionary<string, string> _materials = new(StringComparer.Ordinal);
        private readonly StringBuilder _quoted = new();
        private int _at;
        private int _keep;
        private int _line = 1;
        private Token? _peeked;

        public Reader(string text, string name)
        {
            (_whole, _name) = (text, name);
            _end = text.Length;
            Game = GameOf(text);
        }

        public Reader(TextReader source, string name)
        {
            (_source, _name) = (source, name);
            _buffer = new char[Window];
            _end = source.ReadBlock(_buffer, 0, _buffer.Length);
            Game = GameOf(_buffer.AsSpan(0, _end));
        }

        public string? Game { get; }

        private ReadOnlySpan<char> Span(int start, int length) =>
            _whole is not null ? _whole.AsSpan(start, length) : _buffer.AsSpan(start, length);

        private char Char(int at) => _whole is not null ? _whole[at] : _buffer[at];

        // TrenchBroom's first comment names the game the level was drawn for.
        private static string? GameOf(ReadOnlySpan<char> text)
        {
            const string said = "// Game: ";
            var first = text.TrimStart('\uFEFF');
            if (!first.StartsWith(said, StringComparison.Ordinal)) return null;
            var rest = first[said.Length..];
            var end = rest.IndexOf('\n');
            return (end < 0 ? rest : rest[..end]).Trim().ToString();
        }

        // Whether the text holds a character ahead of where the reader stands, a stream's window
        // moved on to hold it with what is kept of the token being read, and grown where a token is
        // longer than the window.
        private bool Has(int ahead = 0)
        {
            if (_at + ahead < _end) return true;
            if (_source is null) return false;
            var kept = _end - _keep;
            if (_keep > 0) Array.Copy(_buffer, _keep, _buffer, 0, kept);
            (_at, _end, _keep) = (_at - _keep, kept, 0);
            if (_end == _buffer.Length) Array.Resize(ref _buffer, _buffer.Length * 2);
            var read = _source.ReadBlock(_buffer, _end, _buffer.Length - _end);
            _end += read;
            return _at + ahead < _end;
        }

        public Token? Peek() => _peeked ??= Read();

        public Token? Next()
        {
            var token = Peek();
            _peeked = null;
            return token;
        }

        /// <summary>Whether a token is one of the format's marks, unquoted.</summary>
        public bool Is(Token token, char mark) => !token.Quoted && token.Length == 1 && Char(token.Start) == mark;

        /// <summary>A token's text, made as a string.</summary>
        public string Text(Token token) => token.Unescaped ?? new string(Span(token.Start, token.Length));

        /// <summary>A material's name, one string for every face that names it.</summary>
        public string Material(Token token)
        {
            if (token.Unescaped is { } quoted) return quoted;
            var lookup = _materials.GetAlternateLookup<ReadOnlySpan<char>>();
            if (lookup.TryGetValue(Span(token.Start, token.Length), out var known)) return known;
            var name = Text(token);
            _materials[name] = name;
            return name;
        }

        public double Number(string what)
        {
            var token = Next();
            if (token is not { Quoted: false } read
                || !double.TryParse(Span(read.Start, read.Length), NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                || !double.IsFinite(value))
                throw Fault(token, $"{what} is meant as a number, where it reads {(token is { } t ? Text(t) : "the file's end")}");
            return value;
        }

        public FormatException Fault(Token? token, string said) =>
            new($"{_name}, line {token?.Line ?? _line}: {Capital(said)}.");

        private static string Capital(string said) => said.Length > 0 ? char.ToUpperInvariant(said[0]) + said[1..] : said;

        private Token? Read()
        {
            while (true)
            {
                _keep = _at;
                if (!Has()) return null;
                var c = Char(_at);
                if (c == '\n') _line++;
                if (char.IsWhiteSpace(c) || c == '\uFEFF')
                {
                    _at++;
                    continue;
                }

                if (c == '/' && Has(1) && Char(_at + 1) == '/')
                {
                    while (Has() && Char(_at) != '\n')
                    {
                        _keep = _at;
                        _at++;
                    }

                    continue;
                }

                if (c is '{' or '}' or '(' or ')' or '[' or ']')
                    return new Token(_at++, 1, false, _line);

                if (c == '"') return Quoted();

                while (Has() && !char.IsWhiteSpace(Char(_at)) && Char(_at) is not ('"' or '(' or ')' or '[' or ']' or '{' or '}')) _at++;
                return new Token(_keep, _at - _keep, false, _line);
            }
        }

        // A quoted string, a quote inside it written with a backslash before it, as TrenchBroom
        // writes one, read out as a string as it is read.
        private Token Quoted()
        {
            var line = _line;
            _quoted.Clear();
            _at++;
            while (true)
            {
                _keep = _at;
                if (!Has()) throw new FormatException($"{_name}, line {line}: A quoted text is never closed.");
                var c = Char(_at++);
                if (c == '"') return new Token(0, _quoted.Length, true, line, _quoted.ToString());
                if (c == '\\' && Has() && Char(_at) == '"')
                {
                    _quoted.Append('"');
                    _at++;
                    continue;
                }

                if (c == '\n') _line++;
                _quoted.Append(c);
            }
        }
    }
}
