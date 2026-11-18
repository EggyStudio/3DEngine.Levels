using Xunit;

namespace Levels.Tests;

/// <summary>
/// A light's style, the letter pattern its brightness follows, ten letters a second, a to z from
/// none to about twice its strength with m its strength, and the presets a style's number names.
/// </summary>
public sealed class LevelLightStyleTests
{
    private static LevelEntity Light(params (string Key, string Value)[] keys)
    {
        var light = new LevelEntity();
        light["classname"] = "light";
        foreach (var (key, value) in keys) light[key] = value;
        return light;
    }

    [Fact]
    public void APatternsLettersAreTenASecondFromNoneAtAToItsStrengthAtM()
    {
        var light = Light(("pattern", "amz")).Light(32)!;

        Assert.Equal("amz", light.Pattern);
        Assert.Equal(0, light.StyleAt(0), 9);
        Assert.Equal(1, light.StyleAt(0.15), 9);
        Assert.Equal(25.0 / 12, light.StyleAt(0.25), 9);
        Assert.Equal(0, light.StyleAt(0.3), 9);
    }

    [Fact]
    public void AStylesNumberNamesAPresetAndNoneOrZeroIsSteady()
    {
        Assert.Equal("abcdefghijklmnopqrstuvwxyzyxwvutsrqponmlkjihgfedcba", Light(("style", "2")).Light(32)!.Pattern);
        Assert.Equal("mamamamamama", Light(("style", "4")).Light(32)!.Pattern);
        Assert.Null(Light(("style", "0")).Light(32)!.Pattern);
        Assert.Null(Light().Light(32)!.Pattern);
        Assert.Equal(1, Light().Light(32)!.StyleAt(12.34));
    }

    [Fact]
    public void APatternComesBeforeAStyle()
    {
        Assert.Equal("az", Light(("style", "2"), ("pattern", "az")).Light(32)!.Pattern);
    }

    [Theory]
    [InlineData("pattern", "aB")]
    [InlineData("pattern", "a1")]
    [InlineData("style", "12")]
    [InlineData("style", "-1")]
    public void APatternOfOtherThanLettersOrAStyleOfNoPresetIsRefused(string key, string value)
    {
        var light = Light((key, value));

        Assert.Throws<FormatException>(() => light.Light(32));
        Assert.Equal([key], new Level { Entities = { light } }.Check().Select(problem => problem.Key));
    }

    [Fact]
    public void AStyleIsWrittenBackAsAPatternAndUneditedAsItWas()
    {
        var styled = Light(("style", "4"));
        styled.SetLight(styled.Light(32)!, 32);
        Assert.Equal("4", styled["style"]);
        Assert.Null(styled["pattern"]);

        styled.SetLight(styled.Light(32)! with { Pattern = "mmaa" }, 32);
        Assert.Equal("mmaa", styled["pattern"]);
        Assert.Null(styled["style"]);

        styled.SetLight(styled.Light(32)! with { Pattern = null }, 32);
        Assert.Null(styled["pattern"]);
        Assert.Null(styled.Light(32)!.Pattern);
    }
}
