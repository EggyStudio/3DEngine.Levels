using System.Numerics;
using System.Text.RegularExpressions;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// The classes Levels.fgd gives TrenchBroom held to the module's readers, each class with the keys
/// they read and the defaults they take, so a level drawn in the editor reads as it was drawn.
/// </summary>
public sealed partial class FgdTests
{
    private const float Close = 1e-5f;

    // The keys each class's reader reads, besides classname and origin, which every entity has.
    private static readonly Dictionary<string, string[]> Read = new()
    {
        ["worldspawn"] = [Level.UnitsPerMeterKey],
        ["info_player_start"] = ["angle"],
        [LevelLight.PointClass] = ["_color", "intensity", "range"],
        [LevelLight.SpotClass] = ["angles", "_color", "intensity", "range", "_inner_cone", "_cone"],
        [LevelLight.DirectionalClass] = ["angles", "_color", "intensity"],
        [LevelTree.Class] = ["species", "seed"],
    };

    [GeneratedRegex(@"@\w+Class[^=]*=\s*(?<name>\w+)\s*:[^\[]*\[(?<keys>[^\]]*)\]")]
    private static partial Regex ClassPattern();

    [GeneratedRegex(@"^\s*(?<key>\w+)\(\w+\)\s*:\s*""[^""]*""\s*:\s*(?:""(?<text>[^""]*)""|(?<number>[^\s:]+))\s*:", RegexOptions.Multiline)]
    private static partial Regex KeyPattern();

    private static Dictionary<string, Dictionary<string, string>> Classes()
    {
        var at = new DirectoryInfo(AppContext.BaseDirectory);
        while (at is not null && !File.Exists(Path.Combine(at.FullName, "Levels.slnx"))) at = at.Parent;
        var fgd = File.ReadAllText(Path.Combine(at!.FullName, "TrenchBroom", "Levels.fgd"));

        return ClassPattern().Matches(fgd).ToDictionary(
            match => match.Groups["name"].Value,
            match => KeyPattern().Matches(match.Groups["keys"].Value).ToDictionary(
                key => key.Groups["key"].Value,
                key => key.Groups["text"].Success ? key.Groups["text"].Value : key.Groups["number"].Value));
    }

    [Fact]
    public void TheFgdNamesTheClassesTheModuleReadsWithTheKeysItReads()
    {
        var classes = Classes();

        Assert.Equal(Read.Keys.Order(), classes.Keys.Order());
        foreach (var (name, keys) in Read)
            Assert.True(keys.Order().SequenceEqual(classes[name].Keys.Order()),
                $"{name} reads {string.Join(", ", keys)} and the fgd gives {string.Join(", ", classes[name].Keys)}");
    }

    public static TheoryData<string> ClassNames() => [.. Read.Keys];

    [Theory]
    [MemberData(nameof(ClassNames))]
    public void AnEntityOfNoKeysReadsAsOneOfTheDefaultsTheFgdShows(string name)
    {
        var bare = new LevelEntity();
        bare["classname"] = name;
        var given = new LevelEntity();
        given["classname"] = name;
        foreach (var (key, value) in Classes()[name]) given[key] = value;

        Assert.Equal(Units(bare), Units(given));
        Near(bare.Placement(32), given.Placement(32));
        Assert.Equal(bare.Tree(), given.Tree());
        if (bare.Light(32) is { } light)
        {
            var other = given.Light(32)!;
            Assert.Equal(light with { Color = default }, other with { Color = default });
            Assert.True(Vector3.Distance(light.Color, other.Color) < Close, $"{light.Color} and {other.Color}");
        }
        else
        {
            Assert.Null(given.Light(32));
        }
    }

    private static double Units(LevelEntity entity)
    {
        var level = new Level();
        level.Entities.Add(entity);
        return level.UnitsPerMeter;
    }

    private static void Near(LevelPlacement expected, LevelPlacement actual)
    {
        Assert.True(Vector3.Distance(expected.Position, actual.Position) < Close);
        Assert.True(Math.Abs(Quaternion.Dot(expected.Rotation, actual.Rotation)) > 1 - Close,
            $"{actual.Rotation} where {expected.Rotation} is meant");
    }
}
