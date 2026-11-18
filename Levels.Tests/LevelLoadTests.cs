using System.Text;
using Xunit;

namespace Levels.Tests;

/// <summary>A level read through an engine's files, as its pictures are.</summary>
public sealed class LevelLoadTests
{
    private sealed class Files(Dictionary<string, byte[]> files) : ILevelFiles
    {
        public bool Exists(string path) => files.ContainsKey(path);

        public byte[] ReadAllBytes(string path) => files[path];
    }

    [Fact]
    public void ALevelIsReadThroughTheEnginesFilesAsTheFileReadsIt()
    {
        var text = Level.Parse(LevelTests.Room).Write();
        var files = new Files(new() { ["levels/room.map"] = Encoding.UTF8.GetBytes(text) });

        Assert.Equal(text, Level.Load(files, "levels/room.map").Write());
    }

    [Fact]
    public void AByteOrderMarkBeforeTheTextIsSkipped()
    {
        var text = Level.Parse(LevelTests.Room).Write();
        var files = new Files(new() { ["room.map"] = [.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes(text)] });

        var level = Level.Load(files, "room.map");

        Assert.Equal(text, level.Write());
        Assert.Equal("Generic", level.Game);
    }

    [Fact]
    public void AMissingLevelIsRefusedByItsPath()
    {
        var fault = Assert.Throws<FileNotFoundException>(() => Level.Load(new Files([]), "levels/none.map"));

        Assert.Equal("levels/none.map", fault.FileName);
    }

    [Fact]
    public void ALevelThatIsNotMapTextIsRefusedAtItsPathAndLine()
    {
        var files = new Files(new() { ["levels/bad.map"] = Encoding.UTF8.GetBytes("{\n\"classname\" \"worldspawn\"\nwad\n}\n") });

        var fault = Assert.Throws<FormatException>(() => Level.Load(files, "levels/bad.map"));

        Assert.StartsWith("levels/bad.map, line 3: ", fault.Message);
    }
}
