const string Input = "input2.txt";
var paths = File.ReadLines(Input).Select(l => Path.Parse(l)).ToArray();
var map = new Map(paths, new Point(500, 0));

int i = 0;
map.PrintMap(Console.Out);
while (map.PourSandWithFloor())
{
    //map.PrintMap(Console.Out);
    ++i;
    Console.WriteLine("Units: " + i);
}

record class Map
{
    private const int FloorLevel = 2;
    private const char Stone = '#';
    private const char Air = '.';
    private const char Sand = 'o';
    private const char PouringPoint = '+';
    private char[,] map;
    private int mapWidth;
    private int mapHeight;
    private int offsetX = 0;
    private Point pouringPoint;
    public Map(Path[] paths, Point pouringPoint)
    {
        this.pouringPoint = pouringPoint;
        var (topLeft, bottomRight) = GetBorders(paths);
        offsetX = topLeft.X;
        mapWidth = bottomRight.X - topLeft.X + 1;
        mapHeight = bottomRight.Y + FloorLevel + 1;
        map = new char[mapWidth, mapHeight];
        FillMap(map, '.');
        foreach (var path in paths)
            DrawPath(path);
        map[pouringPoint.X - offsetX, pouringPoint.Y] = '+';
    }

    public bool PourSandWithFloor()
    {
        var (x, y) = (pouringPoint.X - offsetX, pouringPoint.Y);
        while (true)
        {
            if (x == 0)
            {
                Expand(1, 0);
                x++;
            }
            if (x == mapWidth - 1)
            {
                Expand(0, 1);
            }

            if (map[x, y + 1] == Air)
                ++y;
            else if (map[x - 1, y + 1] == Air)
            {
                --x; ++y;
            }
            else if (map[x + 1, y + 1] == Air)
            {
                ++x; ++y;
            }
            else
            {
                if ((x, y) != (pouringPoint.X - offsetX, pouringPoint.Y))
                {
                    map[x, y] = Sand;
                    return true;
                }
                else return false;
            }
        }
    }


    public bool PourSand()
    {
        var (x, y) = (pouringPoint.X - offsetX, pouringPoint.Y);
        while (true)
        {
            if (y >= mapHeight - 1 || x < 0 || x >= mapWidth)
                return false;


            if (map[x, y + 1] == Air)
                ++y;
            else if (x == 0) return false;
            else if (map[x - 1, y + 1] == Air)
            {
                --x; ++y;
            }
            else if (x == mapWidth - 1) return false;
            else if (map[x + 1, y + 1] == Air)
            {
                ++x; ++y;
            }
            else
            {
                map[x, y] = Sand;
                return true;
            }
        }
    }

    private void FillMap(char[,] map, char ch)
    {
        for (int i = 0; i < mapWidth; ++i)
            for (int j = 0; j < mapHeight; ++j)
                map[i, j] = ch;
        for (int x = 0; x < mapWidth; ++x)
            map[x, mapHeight - 1] = Stone;
    }

    public void Expand(int left, int right)
    {
        mapWidth = mapWidth + left + right;
        offsetX -= left;
        var newMap = new char[mapWidth, mapHeight];
        FillMap(newMap, Air);
        for (int y = 0; y < mapHeight; ++y)
            for (int x = 0; x < mapWidth - left - right; ++x)
                newMap[x + left, y] = map[x, y];
        map = newMap;
    }

    public void DrawPath(Path path)
    {
        Point start = path.Points[0];
        for (int i = 1; i < path.Points.Length; ++i)
        {
            Point end = path.Points[i];
            DrawPath(start, end);
            start = end;
        }
    }

    public void DrawPath(Point start, Point end)
    {
        if (start.Y == end.Y)
        {
            DrawHorizontalPath(start.X, end.X, start.Y);
        }
        else if (start.X == end.X)
        {
            DrawVerticalPath(start.Y, end.Y, start.X);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public void PrintMap(TextWriter writer)
    {
        int rowLength = map.GetLength(0);
        char[] row = new char[rowLength];
        for (int i = 0; i < map.GetLength(1); ++i)
        {
            for (int j = 0; j < rowLength; ++j)
                row[j] = map[j, i];
            writer.WriteLine(row);
        }
    }

    public void DrawHorizontalPath(int startX, int endX, int y)
    {
        if (startX > endX) (startX, endX) = (endX, startX);

        for (int i = startX; i <= endX; ++i)
            map[i - offsetX, y] = '#';
    }

    public void DrawVerticalPath(int startY, int endY, int x)
    {
        if (startY > endY) (startY, endY) = (endY, startY);
        for (int i = startY; i <= endY; ++i)
            map[x - offsetX, i] = '#';
    }

    public static (Point topLeft, Point bottomRight) GetBorders(Path[] paths)
    {
        int maxX = 0, maxY = 0;
        int minX = int.MaxValue, minY = int.MaxValue;
        foreach (var (topLeft, bottomRight) in paths.Select(p => p.GetBorders()))
        {
            if (topLeft.X < minX) minX = topLeft.X;
            if (topLeft.Y < minY) minY = topLeft.Y;
            if (bottomRight.X > maxX) maxX = bottomRight.X;
            if (bottomRight.Y > maxY) maxY = bottomRight.Y;
        }
        return (new Point(minX, minY), new Point(maxX, maxY));
    }
}

record struct Path(Point[] Points)
{
    private const string Separator = " -> ";
    public static Path Parse(string line)
    {
        List<Point> points = new List<Point>();
        return new Path(line.Split(Separator).Select(t => Point.Parse(t)).ToArray());
    }

    public (Point, Point) GetBorders()
    {
        int maxX = 0, maxY = 0;
        int minX = int.MaxValue, minY = 0;
        foreach (var p in Points)
        {
            if (p.X > maxX)
                maxX = p.X;
            if (p.Y > maxY)
                maxY = p.Y;
            if (p.X < minX)
                minX = p.X;
        }
        return (new Point(minX, minY), new Point(maxX, maxY));
    }

}
record struct Point(int X, int Y)
{
    public static Point Parse(ReadOnlySpan<char> token)
    {
        int comma = token.IndexOf(',');
        return new Point(int.Parse(token[0..comma]), int.Parse(token[(comma + 1)..]));
    }
}


