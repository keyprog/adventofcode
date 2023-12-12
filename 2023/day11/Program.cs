using static System.Linq.Enumerable;
using static System.Math;

var points = Universe.GetPoints(File.ReadAllLines("input.txt")).ToArray();

var pairs = points.SelectMany((p1, i) => points[(i + 1)..].Select(p2 => (p1, p2)));
pairs.Print();

var totalLength = pairs.Sum(pp => Abs(pp.p1.Row - pp.p2.Row) + Abs(pp.p1.Col - pp.p2.Col));
Console.WriteLine(totalLength);

class Universe
{
    private const long Expand = 1000000 - 1;
    public static IEnumerable<Point> GetPoints(string[] lines)
    {
        int rows = lines.Length;
        int cols = lines[0].Length;
        bool[] emptyRows = Range(0, rows).Select(r => lines[r].All(c => c == '.')).ToArray();
        bool[] emptyCols = Range(0, cols).Select(c => lines.All(l => l[c] == '.')).ToArray();

        long colExpand = 0;
        for (long c = 0; c < cols; ++c)
        {
            if (emptyCols[c])
            {
                colExpand += Expand;
                continue;
            }

            long rowExpand = 0;
            for (long r = 0; r < rows; ++r)
            {
                if (emptyRows[r])
                    rowExpand += Expand;
                else if (lines[r][(int)c] == '#')
                    yield return new Point(r + rowExpand, c + colExpand);
            }
        }
    }
}

record struct Point(long Row, long Col);