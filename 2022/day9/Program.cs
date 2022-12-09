const string Input = "input2.txt";
const int KnotsTotal = 10;

Position head = new();
var knots = new Position[KnotsTotal - 1]; // keeps current positions for all knots (except head)
var tailPositions = new HashSet<Position>(); // keeps track of all visited positions by the tail
var input = File.ReadLines(Input).Select(l => ParseLine(l));

foreach (var (code, repeats) in input)
{
    for (int rep = 0; rep < repeats; ++rep)
    {
        // moving head
        head = code switch
        {
            'R' => head.MoveRight(),
            'L' => head.MoveLeft(),
            'U' => head.MoveUp(),
            'D' => head.MoveDown(),
            _ => throw new ArgumentException("Not expected command")
        };
        // moving all knots
        knots[0] = knots[0].Follow(head);
        for (int k = 1; k < knots.Length; ++k)
        {
            knots[k] = knots[k].Follow(knots[k - 1]);
        }

        // recording position of last knot
        tailPositions.Add(knots[knots.Length - 1]);

        //Console.WriteLine($"Head {head}, tail {knots[knots.Length - 1]}");
    }
}

Console.WriteLine($"Tail visited { tailPositions.Count } positions");

/// Parses command input in format '<code> <repeat>', e.g. R 123 
(char code, int repeat) ParseLine(ReadOnlySpan<char> line) => (line[0], int.Parse(line[2..]));

record struct Position(int X, int Y)
{
    public Position MoveLeft() => this with { X = this.X - 1 };
    public Position MoveRight() => this with { X = this.X + 1 };
    public Position MoveUp() => this with { Y = this.Y - 1 };
    public Position MoveDown() => this with { Y = this.Y + 1 };

    public Position Follow(Position head)
    {
        int dx = head.X - X;
        int dy = head.Y - Y;

        return (dx, dy) switch
        {
            _ when (Math.Abs(dx) <= 1 && Math.Abs(dy) <= 1) => this,
            (_, 0) => this with { X = X + Math.Sign(dx) },
            (0, _) => this with { Y = Y + Math.Sign(dy) },
            (_, _) => this with { X = X + Math.Sign(dx), Y = Y + Math.Sign(dy) }
        };
    }
}