using static Direction;
using static System.Linq.Enumerable;
Maze maze = new(File.ReadAllLines("input.txt"));

Pipe start = maze.FindStart();

Dictionary<Position, int> positions = new(1024);
positions.Add(start.Position, 0);
var q = new Queue<Pipe>([start]);

while (q.Count > 0)
{
    var pipe = q.Dequeue();
    foreach (var direction in pipe.Directions)
    {
        if (maze.TryGetPipeWithConnection(pipe.Position.Move(direction), direction.Reverse(), out var nextPipe)
            && !positions.ContainsKey(nextPipe.Position))
        {
            q.Enqueue(nextPipe);
            positions.Add(nextPipe.Position, positions[pipe.Position] + 1);
        }
    }
}

maze.Print(positions);
Console.WriteLine($"Part 1: {positions.Values.Max()}");

// Part 2
int total = 0;
maze.TraversePipeline(positions, (dir, pipe) =>
{
    //Console.WriteLine($"{nextPipe.Position} => {nextDirection}");
    if (dir == North || (dir == West && pipe.Type == PipeType.SouthWest))
    {
        for (int c = pipe.Position.Col + 1; !positions.ContainsKey(new Position(pipe.Position.Row, c)); ++c)
            total++;
    }
});
Console.WriteLine($"Part 2: {total}");

class Maze(string[] input)
{
    private readonly string[] maze = input;
    public int RowsCount { get; } = input.Length;
    public int ColsCount { get; } = input[0].Length;

    public IEnumerable<Position> ForAll()
        => from r in Range(0, RowsCount) from c in Range(0, ColsCount) select new Position(r, c);

    public Pipe FindStart()
        => ForAll().Where(p => maze[p.Row][p.Col] == (char)PipeType.Start).Select(p => new Pipe(PipeType.Start, p.Row, p.Col)).First();

    public Pipe FindTopLeft(Dictionary<Position, int> positions)
        => ForAll().Where(p => positions.ContainsKey(p)).Select(p => (Pipe)this.GetPipe(p)!).First();

    public bool TryGetPipe(Position position, out Pipe pipe)
    {
        Pipe? p = GetPipe(position);
        pipe = p ?? new Pipe();
        return p.HasValue;
    }

    public Pipe? GetPipe(Position p) => GetPipe(p.Row, p.Col);
    public Pipe? GetPipe(int row, int col)
    {
        if (row < 0 || row >= RowsCount || col < 0 || col >= ColsCount) return null;
        PipeType tile = (PipeType)maze[row][col];
        return IsValidPipe(tile) ? new Pipe(tile, row, col) : null;
    }

    public bool TryGetPipeWithConnection(Position position, Direction direction, out Pipe pipe)
        => TryGetPipe(position, out pipe) && pipe.ConnectsTo(direction);

    public static bool IsValidPipe(PipeType tile)
        => tile is PipeType.NorthSouth or PipeType.EastWest
        or PipeType.NorthEast or PipeType.NorthWest
        or PipeType.SouthEast or PipeType.SouthWest
        or PipeType.Start;

    public void TraversePipeline(Dictionary<Position, int> positions, Action<Direction, Pipe> onMove)
    {
        static Pipe? GetNext(Maze maze, Position curPipe, Direction curDirection)
            => maze.TryGetPipe(curPipe.Move(curDirection), out var pipe)
                && pipe.ConnectsTo(curDirection.Reverse()) ? pipe : null;

        var topLeft = FindTopLeft(positions);
        var curDirection = East;
        var curPipe = (Pipe)GetNext(this, topLeft.Position, curDirection)!;
        while (curPipe.Position != topLeft.Position)
        {
            onMove(curDirection, curPipe);
            Pipe nextPipe = (Pipe)GetNext(this, curPipe.Position, curDirection)!;
            var nextDirection = Pipe.GetDirections(nextPipe.Type).First(d => GetNext(this, nextPipe.Position, d) != null && d != curDirection.Reverse());

            curPipe = nextPipe;
            curDirection = nextDirection;
        }
    }

    public void Print(Dictionary<Position, int> positions)
    {
        for (int r = 0; r < RowsCount; r++)
        {
            for (int c = 0; c < ColsCount; c++)
            {
                Console.Write(positions.TryGetValue(new Position(r, c), out int dist) ? '*' : '.');
                Console.Write(' ');
            }
            Console.WriteLine();
        }
    }
}

public readonly record struct Pipe(PipeType Type, int Row, int Col)
{
    public readonly Position Position { get => new Position(Row, Col); }
    public bool ConnectsTo(Direction direction) => Array.IndexOf(Directions, direction) >= 0;

    public Direction[] Directions { get => GetDirections(Type); }
    public static Direction[] GetDirections(PipeType pipeType)
    {
        return pipeType switch
        {
            PipeType.EastWest => [East, West],
            PipeType.NorthSouth => [North, South],
            PipeType.NorthEast => [North, East],
            PipeType.NorthWest => [North, West],
            PipeType.SouthEast => [South, East],
            PipeType.SouthWest => [South, West],
            PipeType.Start => [North, South, West, East],
            _ => []
        };
    }
}

public readonly record struct Position(int Row, int Col)
{
    public Position Move(Direction direction)
        => direction switch
        {
            Direction.East => new Position(Row, Col + 1),
            Direction.North => new Position(Row - 1, Col),
            Direction.South => new Position(Row + 1, Col),
            Direction.West => new Position(Row, Col - 1),
            _ => throw new Exception()
        };
}

public enum Direction { North, South, East, West }
public static class DirectionExtensions
{
    public static Direction Reverse(this Direction direction)
    => direction switch
    {
        East => West,
        North => South,
        West => East,
        South => North,
        _ => throw new ArgumentException()
    };
}

public enum PipeType
{
    NorthSouth = '|',
    EastWest = '-',
    NorthEast = 'L',
    NorthWest = 'J',
    SouthWest = '7',
    SouthEast = 'F',
    Ground = '.',
    Start = 'S'
}
