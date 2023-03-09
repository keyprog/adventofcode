const string Input = "input2.txt";
var (mapLines, path) = InputParser.Parse(File.ReadAllLines(Input));
var maps = MapsParser.Parse(mapLines, 50);
var mm = new MapManager(maps, MapsExtensions.MoveToNextMap2);

var start = maps[0].Rows[0].Segments[0].Start;
var player = new Player { Row = 0, Column = start };
var pos = player.Travel(mm, path);
//var player = new Player { Row = 1, Column = 11 };
//Console.WriteLine(player.Travel(mm, "1"));

Console.WriteLine($"Last position ({pos.row + 1},{pos.col + 1})");
Console.WriteLine($"Password: " + (1000 * (pos.row + 1) + 4 * (pos.col + 1) + pos.dir));

record class Player
{
    public int Row { get; set; }
    public int Column { get; set; }

    public const string Directions = "RDLU";
    private int direction { get; set; } = 0;


    public (int row, int col, int dir) Travel(MapManager mapManager, string path)
    {
        foreach (var command in Parse(path))
        {
            switch (command)
            {
                case MoveCommand mv:
                    Console.WriteLine($"{Directions[direction]}{mv.Steps} {(Row, Column)}");
                    (Row, Column, var dir) = mapManager.Move(Row, Column, mv.Steps, Directions[direction]);
                    direction = Directions.IndexOf(dir);
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
        return (Row, Column, direction);
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
    public static Map[] Parse(string[] rowLines, int mapSize)
    {
        //int mapSize = rowLines.Length / 3;
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
                for (int col = lastColsRange.start; col < lastColsRange.end; col += mapSize)
                {
                    maps.Add(Map.Parse(rowLines, colLines, (firstRow, row - 1), (col, col + mapSize - 1)));
                }
                firstRow = row;
                lastColsRange = colsRange;
            }
        }
        for (int col = lastColsRange.start; col < lastColsRange.end; col += mapSize)
        {
            maps.Add(Map.Parse(rowLines, colLines, (firstRow, rowLines.Length - 1), (col, col + mapSize - 1)));
        }

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
    public delegate (Map? map, char direction, int row, int col) MoveToNextMap(Map[] maps, Map map, char direction, int row, int col);

    private readonly Map[] maps;
    private readonly int mapSize;
    private readonly MoveToNextMap moveToNextMap;
    public MapManager(Map[] maps, MoveToNextMap moveToNextMap)
    {
        this.maps = maps;
        this.moveToNextMap = moveToNextMap;
        mapSize = maps[0].EndRow - maps[0].StartRow + 1;
    }

    public (int newRow, int newColumn, char direction) Move(int row, int col, int steps, char direction)
    {
        var map = FindMap(row, col) ?? throw new ApplicationException();
        while (steps != 0)
        {
            switch (direction)
            {
                case 'R' when (col == map.EndCol):
                    {
                        var (m, d, r, c) = moveToNextMap(maps, map, direction, row, col);
                        if (m == null)
                            return (row, col, direction);
                        steps--;
                        (map, direction, row, col) = (m, d, r, c);
                    }
                    break;
                case 'R':
                    (row, col, steps) = map.MoveHoriz(row, col, steps);
                    break;
                case 'L' when col == map.StartCol:
                    {
                        var (m, d, r, c) = moveToNextMap(maps, map, direction, row, col);
                        if (m == null)
                            return (row, col, direction);
                        steps--;
                        (map, direction, row, col) = (m, d, r, c);
                    }
                    break;
                case 'L':
                    (row, col, steps) = map.MoveHoriz(row, col, -steps);
                    steps = -steps;
                    break;
                case 'U' when row == map.StartRow:
                    {
                        var (m, d, r, c) = moveToNextMap(maps, map, direction, row, col);
                        if (m == null)
                            return (row, col, direction);
                        steps--;
                        (map, direction, row, col) = (m, d, r, c);
                    }
                    break;
                case 'U':
                    (row, col, steps) = map.MoveVert(row, col, -steps);
                    steps = -steps;
                    break;
                case 'D' when row == map.EndRow:
                    {
                        var (m, d, r, c) = moveToNextMap(maps, map, direction, row, col);
                        if (m == null)
                            return (row, col, direction);
                        steps--;
                        (map, direction, row, col) = (m, d, r, c);
                    }
                    break;
                case 'D':
                    (row, col, steps) = map.MoveVert(row, col, steps);
                    break;
                default:
                    throw new ApplicationException();
            };
        }
        return (row, col, direction);
    }

    private Map? FindMap(int row, int column)
    {
        return maps.ForRow(row).ForCol(column).SingleOrDefault();
    }
}

static class MapsExtensions
{
    public static (Map? map, char direction, int row, int col) MoveToNextMap2(this Map[] maps, Map map, char direction, int row, int col)
    {
        int mapIndex = Array.IndexOf(maps, map);
        (mapIndex, direction, row, col) = (direction, mapIndex) switch
        {
            ('R', 1) => (4, 'L', maps[4].EndRow - maps[1].RowToRel(row), maps[4].EndCol),
            ('R', 2) => (1, 'U', maps[1].EndRow, maps[1].StartCol + maps[2].RowToRel(row)),
            ('R', 4) => (1, 'L', maps[1].EndRow - maps[4].RowToRel(row), maps[1].EndCol),
            ('R', 5) => (4, 'U', maps[4].EndRow, maps[4].StartCol + maps[5].RowToRel(row)),
            ('R', _) => (mapIndex + 1, 'R', row, col + 1),
            ('L', 0) => (3, 'R', maps[3].EndRow - maps[0].RowToRel(row), maps[3].StartCol),
            ('L', 2) => (3, 'D', maps[3].StartRow, maps[3].StartCol + maps[2].RowToRel(row)),
            ('L', 3) => (0, 'R', maps[0].EndRow - maps[3].RowToRel(row), maps[0].StartCol),
            ('L', 5) => (0, 'D', maps[0].StartRow, maps[0].StartCol + maps[5].RowToRel(row)),
            ('L', _) => (mapIndex - 1, 'L', row, col - 1),
            ('U', 1) => (5, 'U', maps[5].EndRow, maps[5].StartCol + maps[1].ColToRel(col)),
            ('U', 0) => (5, 'R', maps[5].StartRow + maps[0].ColToRel(col), maps[5].StartCol),
            ('U', 3) => (2, 'R', maps[2].StartRow + maps[3].ColToRel(col), maps[2].StartCol),
            ('U', _) => (mapIndex - 2, 'U', row - 1, col),
            ('D', 1) => (2, 'L', maps[2].StartRow + maps[1].ColToRel(col), maps[2].EndCol),
            ('D', 4) => (5, 'L', maps[5].StartRow + maps[4].ColToRel(col), maps[5].EndCol),
            ('D', 5) => (1, 'D', maps[1].StartRow, maps[1].StartCol + maps[5].ColToRel(col)),
            ('D', _) => (mapIndex + 2, 'D', row + 1, col),
            _ => throw new ApplicationException()
        };

        map = maps[mapIndex];
        if (!map.CanMoveTo(row, col))
            return (null, direction, 0, 0);
        return (map, direction, row, col);
    }

    public static (Map? map, char direction, int row, int col) MoveToNextMap(this Map[] maps, Map map, char direction, int row, int col)
    {
        int mapIndex = Array.IndexOf(maps, map);
        (mapIndex, direction, row, col) = (direction, mapIndex) switch
        {
            ('R', 0) => (5, 'L', maps[5].EndRow - maps[0].RowToRel(row), maps[5].EndCol),
            ('R', 5) => (0, 'L', maps[0].EndRow - maps[5].RowToRel(row), maps[0].EndCol),
            ('R', 3) => (5, 'D', maps[5].StartRow, maps[5].EndCol - maps[3].RowToRel(row)),
            ('R', _) => (mapIndex + 1, 'R', row, col + 1),
            ('L', 1) => (5, 'U', maps[5].EndRow, maps[5].EndCol - maps[1].ColToRel(col)),
            ('L', 4) => (2, 'U', maps[2].EndRow, maps[2].EndCol - maps[4].RowToRel(row)),
            ('L', 0) => (2, 'D', maps[2].StartRow, maps[2].StartCol + maps[0].RowToRel(row)),
            ('L', _) => (mapIndex - 1, 'L', row, col - 1),
            ('U', 1) => (0, 'D', maps[0].StartRow, maps[0].EndCol - maps[1].ColToRel(col)),
            ('U', 0) => (1, 'D', maps[1].StartRow, maps[1].EndCol - maps[0].ColToRel(col)),
            ('U', 2) => (0, 'R', maps[0].StartRow + maps[2].ColToRel(col), maps[0].StartCol),
            ('U', 5) => (0, 'D', maps[0].StartRow - maps[5].ColToRel(col), maps[3].EndCol),
            ('U', _) => (mapIndex - 1, 'U', row - 1, col),
            ('D', 4) => (1, 'U', maps[1].EndRow, maps[1].EndCol - maps[4].ColToRel(col)),
            ('D', 1) => (4, 'U', maps[4].EndRow, maps[4].EndCol - maps[1].ColToRel(col)),
            ('D', 5) => (1, 'R', maps[1].EndRow - maps[5].ColToRel(col), maps[1].StartCol),
            ('D', 2) => (4, 'R', maps[4].EndRow - maps[2].ColToRel(col), maps[4].StartCol),
            ('D', 0) => (3, 'D', maps[3].StartRow, col),
            ('D', 3) => (4, 'D', row + 1, col),
            _ => throw new ApplicationException()
        };

        map = maps[mapIndex];
        if (!map.CanMoveTo(row, col))
            return (null, direction, 0, 0);
        return (map, direction, row, col);
    }

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

    public int RowToRel(int row) => row - StartRow;

    public int ColToRel(int col) => col - StartCol;
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