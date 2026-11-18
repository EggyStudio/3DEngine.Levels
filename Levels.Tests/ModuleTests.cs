using System.Reflection;
using Tools;
using Xunit;

namespace Levels.Tests;

/// <summary>
/// The checks every module keeps, from the Tools submodule, that the library references no engine,
/// window or graphics API, names no engine in a public member or its documentation and has every
/// public type named in the README, and that the README's binding block is the host's code.
/// </summary>
public sealed class ModuleTests
{
    // The library's assembly, the checkout's root and the source of the host that runs the
    // README's binding block, the three a module sets.
    private const string Library = "3DEngine.Levels";
    private const string Host = "Levels.Tests/Host/LevelBinding.cs";

    private static string Root
    {
        get
        {
            var at = new DirectoryInfo(AppContext.BaseDirectory);
            while (at is not null && !File.Exists(Path.Combine(at.FullName, "Levels.slnx")))
                at = at.Parent;
            return at?.FullName
                ?? throw new DirectoryNotFoundException("No Levels.slnx above the tests.");
        }
    }

    [Fact]
    public void TheLibraryIsOneAnyEngineTakes()
    {
        var library = Assembly.Load(Library);
        var deps = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".deps.json");
        var readme = File.ReadAllText(Path.Combine(Root, "README.md"));
        Assert.Empty(ModuleChecks.Refused(library, deps));
        Assert.Empty(ModuleChecks.EngineNames(library));
        Assert.Empty(ModuleChecks.Unnamed(library, readme));
        Assert.Null(ModuleChecks.BindingBlock(readme, File.ReadAllText(Path.Combine(Root, Host))));
    }
}
