string[] lines = File.ReadAllLines("input.txt");
var map = new Map(Parser.Parse(lines).ToArray(), lines.Length, lines[0].Length);
Console.WriteLine($"Rocks count: {map.Rocks.Length}");
Console.WriteLine($"Map {lines.Length}x{lines[0].Length}");
int rounds = 1000000000;
List<long> states = [];
HashSet<long> loads = [];
int cycleStart = -1;
for (int i = 0; i < rounds; ++i)
{
    map.MoveNorth();
    map.MoveWest();
    map.MoveSouth();
    map.MoveEast();
    long state = map.GetStateHash();
    if (cycleStart < 0)
    {
        loads.Add(map.GetLoad());
        cycleStart = states.IndexOf(state);
        if (cycleStart >= 0)
        {
            int interval = states.Count - cycleStart;
            int skip = (rounds - i) / interval * interval;
            Console.WriteLine($"Cycle start {cycleStart}, interval {interval}, will skip {skip}");
            i += skip;
        }
    }
    states.Add(state);
}

Console.WriteLine(map.GetLoad());
loads.Order().Print();

public static class Parser
{
    public static IEnumerable<Rock> Parse(string[] lines)
    {
        foreach (var (row, col, val) in lines.ForEach())
        {
            if (val == 'O')
                yield return new Rock(row, col, RockType.Rounded);
            else if (val == '#')
                yield return new Rock(row, col, RockType.Cube);
        }
    }
}

public class Map(Rock[] rocks, int mapRows, int mapCols)
{
    public Rock[] Rocks { get; } = rocks;
    public int MapRows { get; } = mapRows;
    public int MapCols { get; } = mapCols;

    public void MoveNorth()
    {
        Span<int> colsMaxRow = stackalloc int[MapCols];
        Array.Sort(Rocks, (a, b) => a.Row - b.Row);
        foreach (ref Rock rock in Rocks.AsSpan())
        {
            if (rock.Type == RockType.Cube)
                colsMaxRow[rock.Col] = rock.Row + 1;
            else
            {
                rock.Row = colsMaxRow[rock.Col];
                colsMaxRow[rock.Col]++;
            }
        }
    }

    public void MoveSouth()
    {
        Span<int> colsMinRow = stackalloc int[MapCols];
        Array.Sort(Rocks, (a, b) => b.Row - a.Row);
        colsMinRow.Fill(MapRows - 1);
        foreach (ref Rock rock in Rocks.AsSpan())
        {
            if (rock.Type == RockType.Cube)
                colsMinRow[rock.Col] = rock.Row - 1;
            else
            {
                rock.Row = colsMinRow[rock.Col];
                colsMinRow[rock.Col]--;
            }
        }
    }

    public void MoveWest()
    {
        Span<int> rowsMaxRow = stackalloc int[MapRows];
        Array.Sort(Rocks, (a, b) => a.Col - b.Col);

        foreach (ref Rock rock in Rocks.AsSpan())
        {
            if (rock.Type == RockType.Cube)
                rowsMaxRow[rock.Row] = rock.Col + 1;
            else
            {
                rock.Col = rowsMaxRow[rock.Row];
                rowsMaxRow[rock.Row]++;
            }
        }
    }

    public void MoveEast()
    {
        Span<int> rowsMinCol = stackalloc int[MapRows];
        Array.Sort(Rocks, (a, b) => b.Col - a.Col);
        rowsMinCol.Fill(MapCols - 1);

        foreach (ref Rock rock in Rocks.AsSpan())
        {
            if (rock.Type == RockType.Cube)
                rowsMinCol[rock.Row] = rock.Col - 1;
            else
            {
                rock.Col = rowsMinCol[rock.Row];
                rowsMinCol[rock.Row]--;
            }
        }

    }
    public long GetLoad()
        => Rocks.Where(r => r.Type == RockType.Rounded).Sum(r => MapRows - r.Row);

    public long GetStateHash()
        => Rocks.Where(r => r.Type == RockType.Rounded).Sum(r => r.Row * MapRows + r.Col);
}

public record struct Rock(int Row, int Col, RockType Type);

public enum RockType { Cube, Rounded };