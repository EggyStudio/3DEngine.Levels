using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// No public type or member of the library, nor its documentation, names either engine, so a
/// stranger reads the library as one any engine takes; the engines appear in the README alone, as
/// two examples.
/// </summary>
public sealed partial class EngineNameTests
{
    // Each engine's name and its API's prefix, as a word of its own.
    [GeneratedRegex(@"3DEngine|\bBevy(CSharp)?\b|\bE3D\b", RegexOptions.IgnoreCase)]
    private static partial Regex Engine();

    [Fact]
    public void NoPublicTypeOrMemberIsNamedForAnEngine()
    {
        var library = typeof(Level).Assembly;
        var names = library.GetExportedTypes().SelectMany(type =>
            type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(member => $"{type.FullName}.{member.Name}")
                .Prepend(type.FullName!));

        Assert.DoesNotContain(names, name => Engine().IsMatch(name));
    }

    [Fact]
    public void NoSummaryOrRemarkNamesAnEngine()
    {
        var library = typeof(Level).Assembly;
        var file = Path.Combine(AppContext.BaseDirectory, library.GetName().Name + ".xml");

        // The documentation's members, its assembly's own name apart, which the package carries.
        var named = XDocument.Load(file).Descendants("member")
            .Where(member => Engine().IsMatch(member.Value))
            .Select(member => member.Attribute("name")!.Value)
            .ToList();

        Assert.True(named.Count == 0, $"An engine is named in {string.Join(", ", named)}.");
    }
}
