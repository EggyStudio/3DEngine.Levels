using Xunit;

namespace Levels.Tests;

/// <summary>
/// A level loaded from a file or an engine's files through a stream, so the file is never held as
/// one text beside the level it reads into.
/// </summary>
public sealed class LevelLoadMemoryTests(LevelLoadMemoryTests.LargeFile large) : IClassFixture<LevelLoadMemoryTests.LargeFile>
{
    // Bytes a character of the file, the level it is read into among them, as Level.Parse is held
    // to 4 in LevelParseMemoryTests; the file's text as one string would be 2 more.
    private const double BytesACharacter = 4;

    /// <summary>
    /// A level of 10,000 brushes saved once into a folder of its own for the tests.
    /// </summary>
    public sealed class LargeFile : IDisposable
    {
        public LargeFile()
        {
            var level = new Level();
            var world = new LevelEntity();
            world["classname"] = "worldspawn";
            for (var i = 0; i < 10_000; i++)
            {
                var low = new LevelPoint(i % 100 * 128, i / 100 * 128, 0);
                world.Brushes.Add(LevelBrush.Box(low, low + new LevelPoint(64, 64, 32 + i % 7 * 16), $"mat{i % 20}"));
            }

            level.Entities.Add(world);
            Directory.CreateDirectory(Folder);
            Text = level.Write();
            File.WriteAllText(System.IO.Path.Combine(Folder, "large.map"), Text);
        }

        public string Folder { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"levels-load-{Guid.NewGuid():N}");

        public string Text { get; }

        public string Path => System.IO.Path.Combine(Folder, "large.map");

        public void Dispose() => Directory.Delete(Folder, recursive: true);
    }

    private sealed class Files(string folder) : ILevelFiles
    {
        public bool Exists(string path) => File.Exists(System.IO.Path.Combine(folder, path));

        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(System.IO.Path.Combine(folder, path));

        public Stream OpenRead(string path) => File.OpenRead(System.IO.Path.Combine(folder, path));
    }

    private void Within(string what, Func<Level> load)
    {
        load();
        var before = GC.GetAllocatedBytesForCurrentThread();
        var read = load();
        var bytes = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(10_000, read.World!.Brushes.Count);
        Assert.True(bytes <= BytesACharacter * large.Text.Length,
            $"{what} of {large.Text.Length:N0} characters allocated {bytes:N0} bytes, {(double)bytes / large.Text.Length:F1} a character, past {BytesACharacter}.");
    }

    [Fact]
    public void ALevelLoadedFromAFileIsReadInAFewBytesACharacter() =>
        Within("Level.Load of a path", () => Level.Load(large.Path));

    [Fact]
    public void ALevelLoadedThroughAnEnginesStreamIsReadInAFewBytesACharacter() =>
        Within("Level.Load of an engine's files", () => Level.Load(new Files(large.Folder), "large.map"));

    [Fact]
    public void ALevelLoadedThroughAStreamWritesAsItsText()
    {
        Assert.Equal(large.Text, Level.Load(new Files(large.Folder), "large.map").Write());
        Assert.Equal(large.Text, Level.Load(large.Path).Write());
    }
}
