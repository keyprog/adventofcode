const string ShapesFile = "shapes.txt";
const string InputFile = "input2.txt";

string jets = File.ReadAllText(InputFile);

//Console.WriteLine(jets.Length);

var chamber = new Chamber(7, jets);
var shapes = new Shapes(Shape.Read(File.ReadLines(ShapesFile)).ToArray());

List<int> diffs = new();


ulong prev = 0;
ulong rocksCount = 1000000;

foreach (var shape in shapes.GetShapes())
{
    chamber.Push(shape);
    rocksCount--;
    if (rocksCount == 0)
        break;
    //if (rocksCount % 1000000 == 0)
    //  Console.WriteLine(rocksCount);
    //chamber.Print(Console.Out);
    //if (rocksCount % i == 0)
    {
        /*Console.WriteLine(chamber.TotalTop - prev);
        prev = chamber.TotalTop;
        chamber.Print(Console.Out);*/
        int diff = (int)(chamber.TotalTop - prev);
        diffs.Add(diff);
        prev = chamber.TotalTop;
    }
}



//chamber.Print(Console.Out);

Console.WriteLine(chamber.Top);
Console.WriteLine(chamber.TotalTop);

int interval =1745;
ulong intSum = 2778;
ulong repeats = 1000000000000ul;
ulong total = (repeats / (ulong)interval) * intSum + (ulong) diffs.Take((int)(repeats % (ulong)interval)).Sum();

Console.WriteLine(total);

//for (int i = 0; i < 35; ++i)
//{
    Console.WriteLine(string.Join(' ', diffs.Skip(5000).Chunk(interval).Select(c => c.Sum()).Take(20)));
//}

/*(int offset, int interval) FindRepeatInterval(List<int> values)
{

    /*for (int i = 1; i < 100; ++i)
    {
        int sum1 = 0, prev1 = values[0];
        foreach(var n in values.Skip(1).Take(i * 20).Chunk(i).Select(c => c.Sum()))
            sum1

    }*/
//}*/

public record class Rock
{
    private readonly Shape shape;
    private int left;
    private int top;

    public Rock(Shape shape, int left, int top)
    {
        this.shape = shape;
        this.left = left;
        this.top = top;
    }

    public void MoveRight(Chamber chamber)
    {
        if (Right == chamber.Width)
            return;

        int depth = chamber.Top - Bottom;
        if (depth > 0 && shape.Intersects(chamber.GetTopLevels(depth), left + 1))
        {
            return;
        }
        left++;
    }
    public void MoveLeft(Chamber chamber)
    {
        if (Left == 0)
            return;

        int depth = chamber.Top - Bottom;
        if (depth > 0 && shape.Intersects(chamber.GetTopLevels(depth), left - 1))
        {
            return;
        }
        left--;
    }

    public bool TryMoveDown(Chamber chamber)
    {
        if (Bottom == 0)
            return false;

        int depth = chamber.Top - Bottom + 1;
        if (depth > 0 && shape.Intersects(chamber.GetTopLevels(depth), left))
        {
            return false;
        }
        top--;
        return true;
    }

    public void CopyShape(Chamber chamber)
    {
        int levelsNeeded = this.Top - chamber.Top;
        if (levelsNeeded > 0)
            chamber.AddLevels(levelsNeeded);
        int offset = chamber.Top > this.top ? chamber.Top - this.top : 0;
        shape.CopyTo(chamber.GetTopLevels(shape.Height + offset), left);
    }

    public int Left { get => left; }
    public int Right { get => left + shape.Width; }
    public int Top { get => top; }
    public int Bottom { get => top - shape.Height; }
}

public class Chamber
{
    private LinkedList<char[]> levels = new();
    private readonly int width;
    private ulong bottomOffset = 0;
    private string jets;
    private int jetsPosition = 0;

    public Chamber(int width, string jets)
    {
        this.width = width;
        this.jets = jets;
    }

    public void AddLevels(int numLevels)
    {
        for (int i = 0; i < numLevels; ++i)
        {
            char[] level = new char[width];
            Array.Fill(level, '.');
            levels.AddFirst(level);
        }
        //if (levels.Count > 1)
        CollectUnreachableLevels();
    }

    public void CollectUnreachableLevels()
    {
        if (levels.Count == 0)
            return;
        List<char[]> reachablesList = levels.Take(100).ToList();
        if (!IsBottomReachable(reachablesList))
        {
            int reachableCount = reachablesList.Count;
            int cleanedCount = levels.Count - reachableCount;
            bottomOffset += (ulong)cleanedCount;
            levels = new LinkedList<char[]>(reachablesList);
            return;
        }
    }

    private bool IsBottomReachable(List<char[]> map)
    {
        bool[,] visited = new bool[map[0].Length, map.Count];
        Stack<(int, int)> next = new();
        next.Push((0, 0));

        while (next.TryPop(out (int x, int y) p))
        {
            if (p.y == map.Count)
                return true;
            visited[p.x, p.y] = true;
            var neighbours = new[] { (p.x - 1, p.y), (p.x + 1, p.y), (p.x, p.y - 1), (p.x, p.y + 1) };
            foreach ((int x, int y) n in neighbours)
            {
                if (n.x >= 0 && n.y >= 0 && n.x < map[0].Length && n.y < map.Count && !visited[n.x, n.y] && map[n.y][n.x] != '#')
                    next.Push(n);
            }
        }
        return false;
    }


    public void Push(Shape shape)
    {
        Rock rock = new Rock(shape, 2, Top + 3 + shape.Height);
        while (true)
        {
            switch (jets[jetsPosition])
            {
                case '<': rock.MoveLeft(this); break;
                case '>': rock.MoveRight(this); break;
            }
            jetsPosition++;
            if (jetsPosition == jets.Length)
                jetsPosition = 0;

            if (!rock.TryMoveDown(this))
            {
                rock.CopyShape(this);
                return;
            }
        }
    }

    public char[][] GetTopLevels(int levelsCount)
    {
        return levels.Take(levelsCount).ToArray();
    }

    public int Top { get => levels.Count; }

    public ulong TotalTop { get => bottomOffset + (ulong)levels.Count; }

    public int Width { get => width; }

    public void Print(TextWriter writer)
    {
        foreach (var l in levels)
            writer.WriteLine(l);
        writer.WriteLine();
    }
}

public record struct Shapes(Shape[] shapes)
{
    private int current = 0;
    public IEnumerable<Shape> GetShapes()
    {
        while (true)
        {
            yield return shapes[current];
            current++;
            if (current >= shapes.Length)
                current = 0;
        }
    }
}

public record struct Shape(string[] Body)
{
    public int Width { get => Body[0].Length; }
    public int Height { get => Body.Length; }
    public bool Intersects(char[][] Chamber, int offsetCol)
    {
        int chamberOffset = Chamber.Length > Height ? Chamber.Length - Height : 0;
        int depth = Height - Chamber.Length;
        for (int r = chamberOffset; r < Chamber.Length; ++r)
            for (int c = 0; c < Width; ++c)
                if (Body[r + depth][c] == '#' && Chamber[r][c + offsetCol] == '#')
                    return true;
        return false;
    }

    public void CopyTo(char[][] chamber, int offsetCol)
    {
        int chamberOffset = chamber.Length > Height ? chamber.Length - Height : 0;
        for (int r = 0; r < Height; ++r)
            for (int c = 0; c < Width; ++c)
                if (Body[r][c] == '#')
                    chamber[r + chamberOffset][c + offsetCol] = '#';
    }
    public static IEnumerable<Shape> Read(IEnumerable<string> lines)
    {
        return GetBlock(lines).Select(b => new Shape(b));
    }

    private static IEnumerable<string[]> GetBlock(IEnumerable<string> lines)
    {
        List<string> block = new();
        foreach (var l in lines)
        {
            if (!string.IsNullOrWhiteSpace(l))
            {
                block.Add(l);
            }
            else
            {
                yield return block.ToArray();
                block.Clear();
            }
        }
        yield return block.ToArray();
    }
}