using Xunit;

namespace Levels.Tests;

/// <summary>
/// The library's and the tests' sources, held to the width the repository's prose is wrapped at
/// (NORM.md, N 4.11).
/// </summary>
public sealed class SourceTests
{
    private const int Width = 100;

    private static string Root
    {
        get
        {
            // The repository's root is the folder above the tests that holds the solution.
            var at = new DirectoryInfo(AppContext.BaseDirectory);
            while (at is not null && !File.Exists(Path.Combine(at.FullName, "Levels.slnx")))
                at = at.Parent;
            return at?.FullName
                ?? throw new DirectoryNotFoundException("No Levels.slnx above the tests.");
        }
    }

    [Fact]
    public void EveryCommentLineHoldsAHundredColumns()
    {
        var root = Root;
        var over = new List<string>();
        foreach (var project in new[] { "Levels", "Levels.Tests" })
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, project), "*.cs"))
            {
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].TrimStart();
                    if (!line.StartsWith("//") || lines[i].Length <= Width) continue;

                    // A line holding an address alone is left whole, since a broken address opens
                    // nothing.
                    var text = line.TrimStart('/').Trim();
                    if (text.Contains("://") && !text.Contains(' ')) continue;
                    over.Add($"{Path.GetRelativePath(root, file)}:{i + 1} ({lines[i].Length})");
                }
            }
        }

        Assert.True(over.Count == 0, $"{over.Count} comment lines past {Width} columns:\n"
            + string.Join("\n", over));
    }
}
