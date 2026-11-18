using Xunit;

namespace Levels.Tests;

/// <summary>
/// Cells made into boxes, so a tool that builds a level of blocks saves it as brushes, and the
/// brushes fill the cells they were made of.
/// </summary>
public sealed class LevelCellBrushesTests
{
    private static List<LevelCell> Block(int x0, int x1, int y0, int y1, int z0, int z1, string material) =>
        [.. from x in Enumerable.Range(x0, x1 - x0) from y in Enumerable.Range(y0, y1 - y0) from z in Enumerable.Range(z0, z1 - z0)
            select new LevelCell(x, y, z, material)];

    private static IReadOnlyList<LevelCell> Back(IReadOnlyList<LevelBrush> brushes, double size)
    {
        var entity = new LevelEntity();
        entity.Brushes.AddRange(brushes);
        return LevelGeometry.Cells(entity, size, 32);
    }

    [Fact]
    public void ASolidBlockOfOneMaterialIsOneBox()
    {
        var brushes = LevelGeometry.Brushes(Block(-2, 2, 0, 2, -3, 0, "stone"), 1, 32);

        var box = Assert.Single(brushes);
        Assert.All(box.Faces, face => Assert.Equal("stone", face.Material));
        Assert.Equal(Block(-2, 2, 0, 2, -3, 0, "stone"), Back(brushes, 1));
    }

    [Fact]
    public void ARoomOfAFloorAndFourWallsFillsTheCellsItWasMadeOf()
    {
        var cells = Block(0, 8, -1, 0, 0, 8, "floor")
            .Concat(Block(0, 8, 0, 4, 0, 1, "wall")).Concat(Block(0, 8, 0, 4, 7, 8, "wall"))
            .Concat(Block(0, 1, 0, 4, 1, 7, "wall")).Concat(Block(7, 8, 0, 4, 1, 7, "wall"))
            .ToList();

        var brushes = LevelGeometry.Brushes(cells, 0.5, 32);

        Assert.Equal(cells.OrderBy(cell => (cell.X, cell.Y, cell.Z)), Back(brushes, 0.5));
        Assert.Equal(5, brushes.Count);
    }

    [Fact]
    public void AStaircaseFillsTheCellsItWasMadeOf()
    {
        var cells = Enumerable.Range(0, 5).SelectMany(step => Block(step, step + 1, 0, step + 1, 0, 2, "stairs")).ToList();

        var brushes = LevelGeometry.Brushes(cells, 1, 32);

        Assert.Equal(cells.OrderBy(cell => (cell.X, cell.Y, cell.Z)), Back(brushes, 1));
        Assert.Equal(5, brushes.Count);
    }

    [Fact]
    public void TwoMaterialsAreTwoBoxesAndACellGivenTwiceTakesTheFirst()
    {
        var cells = Block(0, 2, 0, 1, 0, 1, "a").Concat(Block(2, 4, 0, 1, 0, 1, "b")).Append(new LevelCell(0, 0, 0, "b")).ToList();

        var brushes = LevelGeometry.Brushes(cells, 1, 32);

        Assert.Equal(2, brushes.Count);
        Assert.Equal(Block(0, 2, 0, 1, 0, 1, "a").Concat(Block(2, 4, 0, 1, 0, 1, "b")), Back(brushes, 1));
    }

    [Fact]
    public void NoCellsAreNoBrushes()
    {
        Assert.Empty(LevelGeometry.Brushes([], 1, 32));
    }
}
