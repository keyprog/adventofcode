using static Dir;
var city = new FactoryCity(File.ReadAllLines("input.txt"));
city.FindMinHeatLoss().Print();

public class FactoryCity(string[] lines)
{
    private readonly string[] heatLossMap = lines;
    public int Rows { get; } = lines.Length;
    public int Cols { get; } = lines[0].Length;

    private readonly int[,] minHeatLossMatrix = new int[lines.Length, lines[0].Length];
    private readonly Dictionary<(int Row, int Col, Dir Dir, int DirCount), int> visitedPaths = [];

    private int GetHeatLoss(int row, int col)
        => row < 0 || row >= Rows || col < 0 || col >= Cols ? -1 : heatLossMap[row][col] - '0';

    public int FindMinHeatLoss()
    {
        foreach (var (row, col) in minHeatLossMatrix.ForEachIndex())
            minHeatLossMatrix[row, col] = int.MaxValue;
        minHeatLossMatrix[0, 0] = 0;

        Queue<Path> q = new(
            new[] { new Path(0, 1, heatLossMap[0][1] - '0', R, 1),
            new Path(1, 0, heatLossMap[1][0] - '0', D, 1) });
        FindPathInternal(q);
        return minHeatLossMatrix[Rows - 1, Cols - 1];
    }

    private bool IsValid(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Cols;
    const int MaxSameDir = 10;
    const int MinSameDir = 4;

    private Path[] allDirections = new Path[4];
    private IEnumerable<Path> GetOutPaths(Path inPath)
    {
        allDirections[0] = new Path(inPath.Row, inPath.Col - 1, 0, L, inPath.GetDirectionCount(L) + 1);
        allDirections[1] = new Path(inPath.Row, inPath.Col + 1, 0, R, inPath.GetDirectionCount(R) + 1);
        allDirections[2] = new Path(inPath.Row - 1, inPath.Col, 0, U, inPath.GetDirectionCount(U) + 1);
        allDirections[3] = new Path(inPath.Row + 1, inPath.Col, 0, D, inPath.GetDirectionCount(D) + 1);

        foreach (var outPath in allDirections)
        {
            if (IsValid(outPath.Row, outPath.Col) && outPath.SameDirectionCount <= MaxSameDir && outPath.Direction != inPath.OppositeDirection() && (inPath.SameDirectionCount >= MinSameDir || inPath.Direction == outPath.Direction))
            {
                yield return outPath with { HeatLoss = inPath.HeatLoss + GetHeatLoss(outPath.Row, outPath.Col) };
            }
        }
    }

    private void FindPathInternal(Queue<Path> queue)
    {
        while (queue.Count > 0)
        {
            Path inEdge = queue.Dequeue();

            if (minHeatLossMatrix[inEdge.Row, inEdge.Col] > inEdge.HeatLoss)
            {
                if (inEdge.SameDirectionCount >= MinSameDir || inEdge.Row != Rows - 1 || inEdge.Col != Cols - 1)
                    minHeatLossMatrix[inEdge.Row, inEdge.Col] = inEdge.HeatLoss;
            }

            foreach (var pp in GetOutPaths(inEdge))
            {
                if (!visitedPaths.TryGetValue((pp.Row, pp.Col, pp.Direction, pp.SameDirectionCount), out int dist) || dist > pp.HeatLoss)
                {
                    visitedPaths[(pp.Row, pp.Col, pp.Direction, pp.SameDirectionCount)] = pp.HeatLoss;
                    queue.Enqueue(pp);
                }
            }
        }
    }
}

record struct Path(int Row, int Col, int HeatLoss, Dir Direction, int SameDirectionCount)
{
    public Dir OppositeDirection() => Direction.GetOpposite();

    public int GetDirectionCount(Dir direction) => direction == this.Direction ? SameDirectionCount : 0;
}

enum Dir { L = 1, R = 2, U = 4, D = 8 }

static class DirExtensions
{
    public static Dir GetOpposite(this Dir dir)
    => dir switch
    {
        L => R,
        R => L,
        U => D,
        D => U,
        _ => 0
    };
}