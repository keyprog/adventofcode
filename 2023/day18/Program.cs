using static Direction;
string[] lines = File.ReadAllLines("input.txt");

Parser.ParsePath(lines).PrintLines().ToArray().CalculatePolygonArea().Print();

enum Direction { U, D, L, R }

class Parser
{
    public static IEnumerable<(long Row, long Col)> ParsePath(string[] lines)
    {
        int row = 0;
        int col = 0;
        yield return (0, 0);
        foreach (var instruction in lines.Select(Parser.ParseColor))
        {
            (row, col) = instruction.Dir switch
            {
                R => (row, col + instruction.Size),
                L => (row, col - instruction.Size),
                U => (row - instruction.Size, col),
                D => (row + instruction.Size, col),
                _ => throw new NotImplementedException()
            };
            yield return (row, col);
        }
    }
    public static (Direction Dir, int Size) Parse(string line)
    {
        var dir = line[0] switch
        {
            'R' => R,
            'L' => L,
            'U' => U,
            'D' => D,
            _ => throw new NotImplementedException()
        };
        int sep = line.IndexOf(' ', 3);
        int size = int.Parse(line.AsSpan()[2..sep]);
        return (dir, size);
    }

    public static (Direction Dir, int Size) ParseColor(string line)
    {
        int sep = line.IndexOf(' ', 3);
        string color = line.AsSpan()[(sep + 3)..^1].ToString();
        int size = int.Parse(color.AsSpan()[0..5], System.Globalization.NumberStyles.HexNumber);
        var dir = color[5] switch
        {
            '0' => R,
            '1' => D,
            '2' => L,
            '3' => U,
            _ => throw new NotSupportedException()
        };
        return (dir, size);
    }
}
