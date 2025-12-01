using System.Collections.Generic;

string inputFile = "test.txt";

var input = File.ReadAllLines(inputFile);

var map = new Map(input);
var guardPos = map.FindGuard();
Console.WriteLine($"Guars position: {guardPos}");

Guard g = new Guard(guardPos.row, guardPos.col, guardPos.dir);
HashSet<(int, int)> visited = [];

while (map.IsInside(g.Row, g.Col))
{
    visited.Add((g.Row, g.Col));
    g.Move(map);
    Console.WriteLine(g);
}

Console.WriteLine($"Visited {visited.Count} distinct positions");

class Map
{
    public Map(string[] input)
    {
        Width = input[0].Length;
        Height = input.Length;
        var data = new char[input.Length, input[0].Length];
        for(int r = 0; r < input.Length; ++r)
            for(int c = 0; c < input[0].Length; ++c)
               data[r, c] = input[r][c];
        Data = data;
    }

    public char[,] Data { get; }
    public int Width { get; }
    public int Height { get; }
    public bool HasObstacle(int row, int col) => IsInside(row, col) && Data[row,col] == '#';
    public bool IsInside(int row, int col) => row >=0 && col >=0 && col < Width && row < Height;
    public (int row, int col, Direction dir) FindGuard()
    {
        for(int row = 0; row < Height; ++row)
          for(int col = 0; col < Width; ++col)
          {
            Direction? direction = Data[row,col] switch
            {
                '^' => Direction.Up,
                '>' => Direction.Right,
                'v' => Direction.Down,
                '<' => Direction.Left,
                _ => null,
            };
            if (direction is Direction d)
                return (row, col, d);
          }

        throw new ApplicationException("Guard not found on the map");
    }
}


class Guard(int row, int col, Direction dir)
{
    public int Row {get;set;} = row;
    public int Col {get;set;} = col;
    public Direction Dir {get;set;} = dir;

    public bool Move(Map map)
    {
        var (row, col) = Dir switch
        {
            Direction.Up => (Row - 1, Col),
            Direction.Down => (Row + 1, Col),
            Direction.Left => (Row, Col - 1),
            Direction.Right => (Row, Col + 1),
            _ => throw new NotSupportedException()
        };
        if (!map.HasObstacle(row, col))        
        {
            Row = row;
            Col = col;
            return true;
        }
        TurnRight();
        return Move(map);        
    }

    public void TurnRight()
    {
        Dir = Dir switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            var a => a
        };
    }

    public override string ToString() => $"Position [{Row},{Col}], Direction: {Dir}";
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}