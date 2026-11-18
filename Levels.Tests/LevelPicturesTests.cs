using Xunit;

namespace Levels.Tests;

/// <summary>
/// A level's materials found by the module's names for them, over files an engine gives.
/// </summary>
public sealed class LevelPicturesTests
{
    private sealed class Files(Dictionary<string, byte[]> files) : ILevelFiles
    {
        public List<string> Read { get; } = [];

        public bool Exists(string path) => files.ContainsKey(path);

        public byte[] ReadAllBytes(string path)
        {
            Read.Add(path);
            return files[path];
        }
    }

    // A PNG's header of 256 by 128 and a JPEG's of 512 by 384, the sizes their headers give.
    private static readonly byte[] Png =
        [0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 13,
         (byte)'I', (byte)'H', (byte)'D', (byte)'R', 0, 0, 1, 0, 0, 0, 0, 128];

    private static readonly byte[] Jpeg =
        [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x04, 0x00, 0x00, 0xFF, 0xC0, 0x00, 0x11, 0x08, 0x01, 0x80,
         0x02, 0x00, 0x03, 0x00, 0x00];

    [Fact]
    public void APrototypeIsItsSlotOverAPictureOfFiveHundredTwelveTexels()
    {
        var files = new Files([]);
        var pictures = new LevelPictures(files);

        Assert.True(LevelPictures.IsPrototype("proto/floor"));
        Assert.Equal("floor", LevelPictures.Slot("proto/floor"));
        Assert.Equal(new LevelPicture(null, 512, 512), pictures.Find("proto/floor"));
        Assert.Equal((512.0, 512.0), pictures.PictureSize("proto/floor"));
        Assert.Empty(files.Read);
    }

    [Theory]
    [InlineData("proto/")]
    [InlineData("protofloor")]
    [InlineData("brick")]
    public void AnyOtherNameIsItsOwnSlot(string material)
    {
        Assert.False(LevelPictures.IsPrototype(material));
        Assert.Equal(material, LevelPictures.Slot(material));
    }

    [Fact]
    public void AMaterialsPictureIsAPngThenAJpegOfItsNameUnderTextures()
    {
        var pictures = new LevelPictures(new Files(new()
        {
            ["textures/brick.png"] = Png,
            ["textures/brick.jpg"] = Jpeg,
            ["textures/stone.jpeg"] = Jpeg,
            ["textures/walls/plaster.jpg"] = Jpeg,
        }));

        Assert.Equal(new LevelPicture("textures/brick.png", 256, 128), pictures.Find("brick"));
        Assert.Equal(new LevelPicture("textures/stone.jpeg", 512, 384), pictures.Find("stone"));
        Assert.Equal(new LevelPicture("textures/walls/plaster.jpg", 512, 384), pictures.Find("walls/plaster"));
    }

    [Fact]
    public void AFileWhoseHeaderDoesNotReadIsPassedForTheNext()
    {
        var pictures = new LevelPictures(new Files(new()
        {
            ["textures/brick.png"] = [1, 2, 3],
            ["textures/brick.jpg"] = Jpeg,
            ["textures/sand.png"] = [1, 2, 3],
        }));

        Assert.Equal(new LevelPicture("textures/brick.jpg", 512, 384), pictures.Find("brick"));
        Assert.Null(pictures.Find("sand"));
        Assert.Null(pictures.PictureSize("grass"));
    }

    [Fact]
    public void AMeshesTexelsAreOverThePicturesTheFilesHold()
    {
        // A floor of 64 units square at 32 units a meter and a scale of one, so a material of 256
        // texels across spans a quarter of it and one with no picture spans the level's meter.
        var floor = new LevelEntity();
        floor.Keys.Add(new("classname", "worldspawn"));
        floor.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, -16), new LevelPoint(64, 64, 0), "brick", 1));
        var bare = new LevelEntity();
        bare.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, -16), new LevelPoint(64, 64, 0), "grass", 1));
        var pictures = new LevelPictures(new Files(new() { ["textures/brick.png"] = Png }));

        static float Across(LevelMesh mesh) => mesh.Uvs.Max(uv => uv.X) - mesh.Uvs.Min(uv => uv.X);
        Assert.Equal(64f / 256, Across(LevelGeometry.Meshes(floor, 32, default, pictures)[0]), 5);
        Assert.Equal(2f, Across(LevelGeometry.Meshes(bare, 32, default, pictures)[0]), 5);
    }
}
