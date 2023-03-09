const string Input = "input2.txt";
string[] lines = File.ReadAllLines(Input);
int totalElves = InputParser.Parse(lines, 0).Count();
int MapPadding = totalElves * 2;
int[,] map = new int[lines[0].Length + MapPadding * 2, lines.Length + MapPadding * 2];
Dictionary<(int, int), int> moves = new();
List<Elf> elves = InputParser.Parse(lines, MapPadding).ToList();

map.MapElves(elves);
int round = 0;
while (true)
{
    round++;

    foreach (var e in elves.Where(e => e.ConsiderMove(map)))
    {
        if (moves.TryGetValue((e.ConsX, e.ConsY), out int competitor))
        {
            e.Cancel();
            elves[competitor - 1].Cancel();
        }
        else
        {
            moves.Add((e.ConsX, e.ConsY), e.Name);
        }
    }
    map.Reset(elves);
    elves.ForEach(e => e.Move());

    map.MapElves(elves);
    if (round % 100 == 0)
        Console.WriteLine(round);

    bool finished = moves.Count == 0;
    if (finished)
    {
        Console.WriteLine($"Rounds: {round}");
        return;
    }
    moves.Clear();
}

/*int minX = int.MaxValue;
int minY = int.MaxValue;
int maxX = int.MinValue;
int maxY = int.MinValue;
foreach (var e in elves)
{
    if (e.X < minX) minX = e.X;
    if (e.X > maxX) maxX = e.X;
    if (e.Y < minY) minY = e.Y;
    if (e.Y > maxY) maxY = e.Y;
}

int area = (maxX - minX + 1) * (maxY - minY + 1) - elves.Count;
Console.WriteLine("Area: " + area);
*/

record class Elf
{
    private readonly Queue<char> directions = new Queue<char>(new[] { 'N', 'S', 'W', 'E' });

    public void MapPosition(int[,] map) => map[X, Y] = Name;
    public int Name { get; init; }
    public int X { get; set; }
    public int Y { get; set; }

    public int ConsX { get; set; } = -1;
    public int ConsY { get; set; } = -1;

    public bool Move()
    {
        if ((ConsX, ConsY) == (-1, -1))
            return false;

        X = ConsX;
        Y = ConsY;
        return true;
    }
    public void Cancel() => (ConsX, ConsY) = (-1, -1);


    public (bool CanMove, int TakenBy) VerifyConsideredPosition(int[,] consMap)
    {
        if ((ConsX, ConsY) == (-1, -1))
            return (false, 0);

        if (consMap[ConsX, ConsY] == 0)
        {
            consMap[ConsX, ConsY] = Name;
            return (true, Name);
        }
        else
        {
            return (false, consMap[ConsX, ConsY]);
        }
    }

    public bool CanMoveNorth(int[,] map) => map[X - 1, Y - 1] == 0 && map[X, Y - 1] == 0 && map[X + 1, Y - 1] == 0;
    public bool CanMoveSouth(int[,] map) => map[X - 1, Y + 1] == 0 && map[X, Y + 1] == 0 && map[X + 1, Y + 1] == 0;
    public bool CanMoveWest(int[,] map) => map[X - 1, Y - 1] == 0 && map[X - 1, Y] == 0 && map[X - 1, Y + 1] == 0;
    public bool CanMoveEast(int[,] map) => map[X + 1, Y - 1] == 0 && map[X + 1, Y] == 0 && map[X + 1, Y + 1] == 0;

    public bool AllDirectionsFree(int[,] map) => CanMoveNorth(map) && CanMoveSouth(map) && CanMoveWest(map) && CanMoveEast(map);

    public bool ConsiderMove(int[,] map)
    {
        bool noNeedToMove = AllDirectionsFree(map);
        if (noNeedToMove)
        {
            directions.Enqueue(directions.Dequeue());
            return false;
        }

        foreach (var d in directions)
        {
            (ConsX, ConsY) = d switch
            {
                'N' when CanMoveNorth(map) => (X, Y - 1),
                'S' when CanMoveSouth(map) => (X, Y + 1),
                'W' when CanMoveWest(map) => (X - 1, Y),
                'E' when CanMoveEast(map) => (X + 1, Y),
                _ => (-1, -1)
            };
            if (ConsX != -1 && ConsY != -1)
                break;
        }

        directions.Enqueue(directions.Dequeue());
        return ConsX != -1 && ConsY != -1;
    }
}

static class MapExtensions
{
    public static void Print(this int[,] map, TextWriter writer)
    {
        for (int y = 0; y < map.GetLength(1); ++y)
        {
            for (int x = 0; x < map.GetLength(0); ++x)
            {
                writer.Write(map[x, y] == 0 ? '.' : '#');
            }
            writer.WriteLine();
        }
    }

    public static void Reset(this int[,] map, IEnumerable<Elf> elves)
    {
        foreach (var e in elves)
            map[e.X, e.Y] = 0;
    }

    public static void Reset(this int[,] map)
    {
        for (int y = 0; y < map.GetLength(1); ++y)
            for (int x = 0; x < map.GetLength(0); ++x)
                map[x, y] = 0;
    }

    public static void MapElves(this int[,] map, IEnumerable<Elf> elves)
    {
        foreach (var elf in elves)
            elf.MapPosition(map);
    }

}

class InputParser
{
    const char ElfChar = '#';
    public static IEnumerable<Elf> Parse(string[] lines, int padding)
    {
        int width = lines[0].Length;
        int height = lines.Length;
        int elfName = 1;

        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
                if (lines[y][x] == ElfChar)
                {
                    yield return new Elf { Name = elfName++, X = x + padding, Y = y + padding };
                }
    }
}