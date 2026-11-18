using System.Reflection;
using Xunit;

namespace Levels.Tests;

public class LibraryTests
{
    [Fact]
    public void The_Library_Loads()
    {
        // By its assembly's name, which an engine's project and package see, while its types stand
        // in the namespace Levels.
        var library = Assembly.Load("3DEngine.Levels");
        Assert.Equal("3DEngine.Levels", library.GetName().Name);
        Assert.Equal(library, typeof(Level).Assembly);
        Assert.Equal("Levels", typeof(Level).Namespace);
    }
}
