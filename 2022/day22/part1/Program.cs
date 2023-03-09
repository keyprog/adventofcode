const string Input = "input2.txt";
var (mapLines, path) = InputParser.Parse(File.ReadAllLines(Input));
var maps = MapsParser.Parse(mapLines);
var mm = new MapManager(maps);

var start = maps[0].Rows[0].Segments[0].Start;
var player = new Player { Row = 0, Column = start };
var pos = player.Travel(new MapManager(maps), path);
//Console.WriteLine(player.Travel(mm, "10R3L3"));

Console.WriteLine($"Last position ({pos.row + 1},{pos.col + 1})");
Console.WriteLine($"Password: " + (1000 * (pos.row + 1) + 4 * (pos.col + 1)));

record class Player
{
    public int Row { get; set; }
    public int Column { get; set; }

    private const string Directions = "RDLU";
    private int direction { get; set; } = 0;


    public (int row, int col) Travel(MapManager mapManager, string path)
    {
        foreach (var command in Parse(path))
        {
            switch (command)
            {
                case MoveCommand mv:
                    Console.WriteLine($"{Directions[direction]}{mv.Steps} {(Row, Column)}");
                    (Row, Column) = mapManager.Move(Row, Column, mv.Steps, Directions[direction]);
                    break;

                case TurnCommand turn:
                    switch (turn.Direction)
                    {
                        case 'R':
                            direction++;
                            if (direction == Directions.Length)
                                direction = 0;
                            break;
                        case 'L':
                            direction--;
                            if (direction < 0)
                                direction = Directions.Length - 1;
                            break;
                    }

                    break;

                default: throw new ApplicationException();
            }
        }
        return (Row, Column);
    }

    private MoveCommand ParseCurrent(ReadOnlySpan<char> p, int start, int end)
    {
        return new MoveCommand(int.Parse(p[start..end]));
    }

    private IEnumerable<Command> Parse(string path)
    {
        int curr = 0;

        for (int i = 0; i < path.Length; ++i)
        {
            switch (path[i])
            {
                case 'R':
                case 'L':
                    yield return ParseCurrent(path, curr, i);
                    yield return new TurnCommand(path[i]);
                    curr = i + 1;
                    break;

            }
        }
        yield return ParseCurrent(path, curr, path.Length);
    }
}

interface Command { };
record struct MoveCommand(int Steps) : Command;
record struct TurnCommand(char Direction) : Command;

static class InputParser
{
    public static (string[] Map, string Path) Parse(string[] lines)
    {
        return (lines.TakeWhile(l => l.Length != 0).ToArray(), lines[lines.Length - 1]);
    }
}

record class MapsParser
{
    public static Map[] Parse(string[] rowLines)
    {
        var colLines = Matrix.Transpose(rowLines);
        List<Map> maps = new();

        var lastColsRange = GetColumnsBounds(rowLines[0]);
        int firstRow = 0;

        for (int row = 0; row < rowLines.Length; ++row)
        {
            string line = rowLines[row];
            var colsRange = GetColumnsBounds(line);
            if (lastColsRange != colsRange)
            {
                maps.Add(Map.Parse(rowLines, colLines, (firstRow, row - 1), lastColsRange));
                firstRow = row;
                lastColsRange = colsRange;
            }
        }
        maps.Add(Map.Parse(rowLines, colLines, (firstRow, rowLines.Length - 1), lastColsRange));

        return maps.ToArray();
    }

    static readonly char[] MapSurface = new[] { '.', '#' };
    private static (int start, int end) GetColumnsBounds(string line)
    {
        int start = line.IndexOfAny(MapSurface);
        if (start < 0)
            return (-1, -1);
        int end = line.IndexOf(' ', start);
        if (end == -1) end = line.Length - 1;
        return (start, end);
    }
}

class MapManager
{
    private readonly Map[] maps;
    public MapManager(Map[] maps)
    {
        this.maps = maps;
    }

    public (int newRow, int newColumn) Move(int row, int col, int steps, char direction)
    {
        var map = FindMap(row, col) ?? throw new ApplicationException();
        while (steps != 0)
        {
            switch (direction)
            {
                case 'R':
                    if (col == map.EndCol)
                    {
                        map = FindMap(row, col + 1) ?? maps.ForRow(row).First();
                        if (!map.CanMoveTo(row, map.StartCol))
                            return (row, col);
                        steps--;
                        col = map.StartCol;
                    }
                    (row, col, steps) = map.MoveHoriz(row, col, steps);
                    break;
                case 'L':
                    if (col == map.StartCol)
                    {
                        map = FindMap(row, col - 1) ?? maps.ForRow(row).Last();
                        if (!map.CanMoveTo(row, map.EndCol))
                            return (row, col);
                        steps--;
                        col = map.EndCol;
                    }
                    (row, col, steps) = map.MoveHoriz(row, col, -steps);
                    steps = -steps;
                    break;
                case 'U':
                    if (row == map.StartRow)
                    {
                        map = FindMap(row - 1, col) ?? maps.ForCol(col).Last();
                        if (!map.CanMoveTo(map.EndRow, col))
                            return (row, col);
                        steps--;
                        row = map.EndRow;
                    }
                    (row, col, steps) = map.MoveVert(row, col, -steps);
                    steps = -steps;
                    break;
                case 'D':
                    if (row == map.EndRow)
                    {
                        map = FindMap(row + 1, col) ?? maps.ForCol(col).First();
                        if (!map.CanMoveTo(map.StartRow, col))
                            return (row, col);
                        steps--;
                        row = map.StartRow;
                    }
                    (row, col, steps) = map.MoveVert(row, col, steps);
                    break;
                default:
                    throw new ApplicationException();
            };
        }
        return (row, col);
    }

    private Map? FindMap(int row, int column)
    {
        return maps.ForRow(row).ForCol(column).SingleOrDefault();
    }

}

static class MapsExtensions
{
    public static IEnumerable<Map> ForRow(this IEnumerable<Map> maps, int row)
    {
        return maps.Where(m => m.StartRow <= row && m.EndRow >= row);
    }

    public static IEnumerable<Map> ForCol(this IEnumerable<Map> maps, int col)
    {
        return maps.Where(m => m.StartCol <= col && m.EndCol >= col);
    }
}

record class Map(int StartRow, int EndRow, int StartCol, int EndCol, Line[] Rows, Line[] Columns)
{
    public static Map Parse(string[] rowLines, string[] colLines, (int start, int end) rowsRange, (int start, int end) colsRange)
    {
        Line[] rows = rowLines.Skip(rowsRange.start).Take(rowsRange.end - rowsRange.start + 1).Select(l => Line.Parse(l, colsRange.start, colsRange.end)).ToArray();
        Line[] columns = colLines.Skip(colsRange.start).Take(colsRange.end - colsRange.start + 1).Select(l => Line.Parse(l, rowsRange.start, rowsRange.end)).ToArray();
        return new Map(rowsRange.start, rowsRange.end, colsRange.start, colsRange.end, rows, columns);
    }

    public bool CanMoveTo(int row, int col)
    {
        var r = Rows[row - StartRow];
        return r.GetSegmentForPosition(col) != Segment.None;
    }

    public (int row, int col, int offset) MoveHoriz(int row, int col, int offset)
    {
        var r = Rows[row - StartRow];
        (col, offset) = r.Move(col, offset);
        return (row, col, offset);
    }

    public bool CanMoveVert(int row, int col, int offset)
    {
        return MoveVert(row, col, offset) != (row, col, 0);
    }

    public (int row, int col, int offset) MoveVert(int row, int col, int offset)
    {
        var c = Columns[col - StartCol];
        (row, offset) = c.Move(row, offset);
        return (row, col, offset);
    }
}
static class Matrix
{
    public static string[] Transpose(string[] lines)
    {
        List<string> transposed = new();
        int rowsCount = lines.Length;
        int colsCount = lines.Max(l => l.Length);

        var line = new char[rowsCount];
        for (int c = 0; c < colsCount; ++c)
        {
            for (int r = 0; r < rowsCount; ++r)
            {
                line[r] = lines[r].Length > c ? lines[r][c] : ' ';
            }
            transposed.Add(new string(line));
        }
        return transposed.ToArray();
    }
}

record struct Line(int Start, int End, Segment[] Segments)
{
    const char Tile = '.';
    static readonly char[] SegmentSeparators = new char[] { '#', ' ' };
    public static Line Parse(string line, int start, int end)
    {
        List<Segment> segments = new();
        var (segStart, segEnd) = (-1, start - 1);

        while (true)
        {
            segEnd++;
            segStart = segEnd <= end ? line.IndexOf(Tile, segEnd, end - segEnd + 1) : -1;
            if (segStart == -1)
                return new Line(start, end, segments.ToArray());
            segEnd = line.IndexOfAny(SegmentSeparators, segStart, end - segStart + 1);
            if (segEnd == -1)
                segEnd = end + 1;
            segments.Add(new Segment(segStart, segEnd - 1));
        }
    }

    public (int pos, int offset) Move(int pos, int offset)
    {
        var s = GetSegmentForPosition(pos);
        if (s == Segment.None)
            return (pos, 0);

        // moving within one segment
        (pos, offset) = s.Move(pos, offset);
        if ((offset > 0 && pos != End) || (offset < 0 && pos != Start))
            offset = 0; // reached a wall

        return (pos, offset);
    }

    public Segment GetSegmentForPosition(int position)
    {
        foreach (var s in Segments)
            if (s.Start <= position && s.End >= position)
                return s;
        return Segment.None;
    }
}

record struct Segment(int Start, int End)
{
    public static readonly Segment None = new Segment(-1, -1);

    public (int pos, int offset) Move(int pos, int offset)
    {
        int newPos = pos + offset;
        if (newPos < Start) newPos = Start;
        if (newPos > End) newPos = End;
        return (newPos, offset - (newPos - pos));
    }
}