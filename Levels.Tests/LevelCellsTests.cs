using Xunit;

namespace Levels.Tests;

/// <summary>
/// The cells of a grid a level's brushes fill, as a reader that stands on blocks lays them, each
/// with its material, by rules two readers follow alike.
/// </summary>
public sealed class LevelCellsTests
{
    private static readonly LevelBrush Box = LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "stone");

    private static LevelBrush Retextured(LevelBrush brush, string material, Func<LevelPoint, bool> facing)
    {
        var copy = new LevelBrush();
        foreach (var face in brush.Faces) copy.Faces.Add(facing(face.Normal) ? face with { Material = material } : face);
        return copy;
    }

    [Fact]
    public void APointInsideABrushIsInsideAndOneOutsideIsNot()
    {
        Assert.True(LevelGeometry.Inside(Box, new LevelPoint(32, 32, 16)));
        Assert.False(LevelGeometry.Inside(Box, new LevelPoint(32, 32, 40)));
        Assert.False(LevelGeometry.Inside(Box, new LevelPoint(-1, 32, 16)));
    }

    [Fact]
    public void APointOnAFaceIsInsideWhereTheFaceLooksTowardLesserXThenYThenZ()
    {
        Assert.True(LevelGeometry.Inside(Box, new LevelPoint(0, 32, 16)));
        Assert.False(LevelGeometry.Inside(Box, new LevelPoint(64, 32, 16)));
        Assert.True(LevelGeometry.Inside(Box, new LevelPoint(32, 0, 16)));
        Assert.False(LevelGeometry.Inside(Box, new LevelPoint(32, 64, 16)));
        Assert.True(LevelGeometry.Inside(Box, new LevelPoint(32, 32, 0)));
        Assert.False(LevelGeometry.Inside(Box, new LevelPoint(32, 32, 32)));
        Assert.True(LevelGeometry.Inside(Box, new LevelPoint(0, 0, 0)));
        Assert.False(LevelGeometry.Inside(Box, new LevelPoint(64, 0, 0)));
    }

    [Fact]
    public void ABoxFillsTheCellsWhoseMiddlesItHolds()
    {
        // 64 by 64 by 32 units at 32 a meter, two meters along X, two along Z, one up.
        var cells = LevelGeometry.Cells(Box, 1, 32);

        Assert.Equal(
            [new LevelCell(0, 0, -2, "stone"), new LevelCell(0, 0, -1, "stone"), new LevelCell(1, 0, -2, "stone"), new LevelCell(1, 0, -1, "stone")],
            cells);
    }

    [Fact]
    public void ACellTakesTheMaterialMostOfItsOuterSidesAreCoveredByAndUpWinsATie()
    {
        // A slab three cells across and one high, its top grass and its bottom dirt, its sides
        // stone: the middle cell's outer sides are its top and its bottom, a tie up wins; a cell on
        // an edge has one of each, up winning again; and a corner's two stone sides outnumber its
        // top and its bottom.
        var slab = LevelBrush.Box(new LevelPoint(0, -96, 0), new LevelPoint(96, 0, 32), "stone");
        slab = Retextured(slab, "grass", normal => normal.Z > 0.5);
        slab = Retextured(slab, "dirt", normal => normal.Z < -0.5);

        var cells = LevelGeometry.Cells(slab, 1, 32).ToDictionary(cell => (cell.X, cell.Z), cell => cell.Material);

        Assert.Equal(9, cells.Count);
        Assert.Equal("grass", cells[(1, 1)]);
        Assert.Equal("stone", cells[(0, 0)]);
        Assert.Equal("grass", cells[(1, 0)]);
    }

    [Fact]
    public void AWedgeFillsTheCellsUnderItsSlope()
    {
        // A ramp 4 meters along X rising 2, 2 deep: a cell's middle lies under the slope where its
        // height is at most half its distance along X.
        var wedge = LevelBrush.Wedge(new LevelPoint(0, 0, 0), new LevelPoint(128, 64, 64), "ramp");

        var cells = LevelGeometry.Cells(wedge, 1, 32);

        Assert.Equal([(1, 0), (2, 0), (3, 0), (3, 1)], cells.Select(cell => (cell.X, cell.Y)).Distinct().Order());
        Assert.Equal(8, cells.Count);
        Assert.All(cells, cell => Assert.Equal("ramp", cell.Material));
    }

    [Fact]
    public void TwoTouchingBrushesNeverFillOneCellTwice()
    {
        var entity = new LevelEntity();
        entity.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "a"));
        entity.Brushes.Add(LevelBrush.Box(new LevelPoint(64, 0, 0), new LevelPoint(128, 64, 32), "b"));

        var cells = LevelGeometry.Cells(entity, 1, 32);
        Assert.Equal(8, cells.Count);
        Assert.Equal(cells.Count, cells.Select(cell => (cell.X, cell.Y, cell.Z)).Distinct().Count());
        Assert.All(cells, cell => Assert.Equal(cell.X < 2 ? "a" : "b", cell.Material));

        // A cell of 4 meters whose middle stands on the face two taller brushes share lies in the
        // brush whose face there looks toward lesser X.
        var tall = new LevelEntity();
        tall.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 128, 128), "a"));
        tall.Brushes.Add(LevelBrush.Box(new LevelPoint(64, 0, 0), new LevelPoint(128, 128, 128), "b"));
        Assert.Equal([new LevelCell(0, 0, -1, "b")], LevelGeometry.Cells(tall, 4, 32));
    }

    [Fact]
    public void ACellTwoBrushesFillTakesTheFirstBrushs()
    {
        var entity = new LevelEntity();
        entity.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "first"));
        entity.Brushes.Add(LevelBrush.Box(new LevelPoint(0, 0, 0), new LevelPoint(64, 64, 32), "second"));

        Assert.All(LevelGeometry.Cells(entity, 1, 32), cell => Assert.Equal("first", cell.Material));
    }
}
