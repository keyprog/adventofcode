const string Input = "input2.txt";
string[] lines = File.ReadAllLines(Input);

int min = FindAllStarts(lines, (c) => c == 'a' || c == 'S')
            .Select(start => FindShortestPath(lines, start.Row, start.Col))
            .Min(p => p?.Distance ?? int.MaxValue);

Console.WriteLine("Distance: " + min);

IEnumerable<(int Row, int Col)> FindAllStarts(string[] lines, Func<char, bool> isStart)
{
    for (int r = 0; r < lines.Length; ++r)
        for (int c = 0; c < lines[r].Length; ++c)
            if (isStart(lines[r][c]))
                yield return new(r, c);
}

Position? FindShortestPath(string[] lines, int startRow, int startCol)
{
    Queue<Position> posToCheck = new();
    posToCheck.Enqueue(new Position(Row: startRow, Col: startCol, Code: 'a', Distance: 0));
    int rows = lines.Length;
    int cols = lines[0].Length;

    var visited = new bool[rows, cols];
    visited[startRow, startCol] = true;

    while (posToCheck.TryDequeue(out Position? pos))
    {
        if (pos.Code == 'E')
            return pos;

        ReadOnlySpan<ValueTuple<int, int>> adj = stackalloc[] {
            (pos.Row - 1, pos.Col),
            (pos.Row, pos.Col - 1),
            (pos.Row + 1, pos.Col),
            (pos.Row, pos.Col + 1)
        };
        foreach (var (r, c) in adj)
        {
            if (r >= 0 && r < rows && c >= 0 && c < cols // bounds check
                && !visited[r, c]
                && CanMove(pos.Code, lines[r][c]))
            {
                posToCheck.Enqueue(new Position(r, c, Code: lines[r][c], pos.Distance + 1));
                visited[r, c] = true;
            }
        }
    }
    return null;
}

const int MaxElevation = 1;
bool CanMove(char from, char to)
{
    int elevation = to switch
    {
        'E' => 'z' - from,
        'S' => 'a' - from,
        _ => to - from
    };
    return elevation <= MaxElevation;
}

record class Position(int Row, int Col, char Code, int Distance)
{
}
