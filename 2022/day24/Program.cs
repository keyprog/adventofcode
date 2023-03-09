const string Input = "input2.txt";
string[] lines = File.ReadAllLines(Input);
int width = lines[0].Length;
int height = lines.Length;
var start = new Position(0, 1);
var destination = new Position(width - 2, height - 1);
List<Blizzard> blizzards = Parser.Parse(lines).ToList();

int steps1 = Navigator.GetStepsCount(blizzards, start, destination, width, height);
int steps2 = Navigator.GetStepsCount(blizzards, destination, start, width, height);
int steps3 = Navigator.GetStepsCount(blizzards, start, destination, width, height);


Console.WriteLine($"Steps: {steps1 + steps2 + steps3 + 2}");

class Navigator
{
    public static int GetStepsCount(List<Blizzard> blizzards, Position start, Position destination, int width, int height)
    {

        List<Position> positions = new();

        positions.Add(start);
        int minute = 0;
        while (true)
        {
            List<Position> newPositions = new(positions.Count * 2);
            minute++;
            blizzards.ForEach(b => b.Move());
            foreach (var p in positions)
            {
                if (p == destination)
                {
                    return minute - 1;
                }
                newPositions.AddRange(p.GetNext(blizzards, start, destination, width, height));
            }
            positions = newPositions.Distinct().ToList();
            if (minute % 100 == 0)
                Console.WriteLine($"Step {minute}, Positions {positions.Count}");
        }
    }
}

record struct Position(int X, int Y)
{
    public IEnumerable<Position> GetNext(IEnumerable<Blizzard> blizzards, Position start, Position end, int width, int height)
    {
        var nextPositions = (new[] { new Position(X, Y), new Position(X - 1, Y), new Position(X + 1, Y), new Position(X, Y - 1), new Position(X, Y + 1) })
                            .Where(p => p == start || p == end || (p.X > 0 && p.X < width - 1 && p.Y > 0 && p.Y < height - 1))
                            .Select(p => (p.X, p.Y)).ToHashSet();

        foreach (var b in blizzards)
        {
            if (nextPositions.Contains((b.X, b.Y)))
            {
                nextPositions.Remove((b.X, b.Y));
                if (nextPositions.Count == 0)
                    return Array.Empty<Position>();
            }
        }

        return nextPositions.Select(p => new Position(p.X, p.Y));
    }
}

record class Blizzard(int MapWidth, int MapHeight, char Direction)
{
    public int X { get; set; }
    public int Y { get; set; }

    public void Move()
    {
        switch (Direction)
        {
            case '>':
                X++;
                if (X == MapWidth - 1)
                    X = 1;
                break;
            case '<':
                X--;
                if (X == 0)
                    X = MapWidth - 2;
                break;
            case '^':
                Y--;
                if (Y == 0)
                    Y = MapHeight - 2;
                break;
            case 'v':
                Y++;
                if (Y == MapHeight - 1)
                    Y = 1;
                break;
            default:
                throw new ApplicationException();
        }
    }
}

class Parser
{
    public static IEnumerable<Blizzard> Parse(string[] lines)
    {
        int mapWidth = lines[0].Length;
        int mapHeight = lines.Length;
        for (int row = 1; row < mapHeight - 1; ++row)
            for (int col = 1; col < mapWidth - 1; ++col)
                if (lines[row][col] != '.')
                    yield return new Blizzard(mapWidth, mapHeight, lines[row][col]) { X = col, Y = row };
    }
}