using NUnit;
using NUnit.Framework;
[TestFixture]
class MoveToNextMapTests2
{
    private MapManager mm;
    [SetUp]
    public void Setup()
    {
        const string Input = "/home/alexp/dev/dotnet/adventofcode/2022/day22/part2/input0.txt";
        var (mapLines, path) = InputParser.Parse(File.ReadAllLines(Input));
        var maps = MapsParser.Parse(mapLines, 4);
        mm = new MapManager(maps, MapsExtensions.MoveToNextMap);
    }

    [Test]
    [TestCase(1, 11, 10, 15)]
    [TestCase(5, 11, 8, 14)]
    [TestCase(8, 15, 3, 11)]
    [TestCase(8, 11, 8, 12)]
    public void MoveRight(int row, int col, int exprow, int expcol)
    {
        var (actrow, actcol, dir) = mm.Move(row, col, 1, 'R');
        Assert.AreEqual((exprow, expcol), (actrow, actcol));
    }

    [Test]
    [TestCase(5, 10, 10, 14, 5)]
    public void MoveRight2(int row, int col, int exprow, int expcol, int steps)
    {
        var (actrow, actcol, dir) = mm.Move(row, col, steps, 'R');
        Assert.AreEqual((exprow, expcol), (actrow, actcol));
    }


    [Test]
    [TestCase(1, 8, 4, 5)]
    [TestCase(4, 0, 11, 15)]
    [TestCase(8, 8, 7, 7)]
    [TestCase(10, 12, 10, 11)]
    public void MoveLeft(int row, int col, int exprow, int expcol)
    {
        var (actrow, actcol, dir) = mm.Move(row, col, 1, 'L');
        Assert.AreEqual((exprow, expcol), (actrow, actcol));
    }

    [Test]
    [TestCase(0, 10, 4, 1)]
    [TestCase(4, 1, 0, 10)]
    [TestCase(4, 4, 0, 8)]
    [TestCase(8, 12, 7, 11)]
    [TestCase(8, 8, 7, 8)]
    public void MoveUp(int row, int col, int exprow, int expcol)
    {
        var (actrow, actcol, dir) = mm.Move(row, col, 1, 'U');
        Assert.AreEqual((exprow, expcol), (actrow, actcol));
    }


    [Test]
    [TestCase(7, 0, 11, 11)]
    [TestCase(7, 4, 11, 8)]
    [TestCase(11, 8, 7, 3)]
    [TestCase(11, 12, 7, 0)]
    [TestCase(3, 8, 4, 8)]
    public void MoveDown(int row, int col, int exprow, int expcol)
    {
        var (actrow, actcol, dir) = mm.Move(row, col, 1, 'D');
        Assert.AreEqual((exprow, expcol), (actrow, actcol));
    }

}