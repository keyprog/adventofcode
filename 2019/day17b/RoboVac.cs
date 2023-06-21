namespace ic;

enum Turn { None, Left = 'L', Right = 'R', };
enum Direction { Left = (int)'<', Right = (int)'>', Up = (int)'^', Down = (int)'v' };

record struct RoboVac(int Row, int Col, Direction Direction)
{
    public static RoboVac FindOnMap(ScaffoldMap map)
    {
        for (int row = 0; row < map.Height; ++row)
        {
            for (int col = 0; col < map.Width; ++col)
            {
                char c = map[row, col];
                if (c == '<' || c == '>' || c == '^' || c == 'v')
                    return new RoboVac(row, col, (Direction)c);
            }
        }
        throw new ArgumentException("RoboVac not found on the map");
    }

    public int MoveForward(ScaffoldMap map)
    {
        int movesCount = 0;
        while (true)
        {
            (Row, Col, bool canMove) = Direction switch
            {
                Direction.Up when CanMoveUp(map) => (Row - 1, Col, true),
                Direction.Down when CanMoveDown(map) => (Row + 1, Col, true),
                Direction.Right when CanMoveRight(map) => (Row, Col + 1, true),
                Direction.Left when CanMoveLeft(map) => (Row, Col - 1, true),
                _ => (Row, Col, false)
            };
            if (canMove) movesCount++;
            else return movesCount;
        };
    }

    public IEnumerable<string> Traverse(ScaffoldMap map)
    {
        Turn turn = MakeTurn(map);
        while (turn != Turn.None)
        {
            yield return ((char)turn).ToString();
            yield return MoveForward(map).ToString();
            turn = MakeTurn(map);
        }
    }

    public Turn MakeTurn(ScaffoldMap map)
    {
        (var turn, this.Direction) = Direction switch
        {
            Direction.Up when CanMoveLeft(map) => (Turn.Left, Direction.Left),
            Direction.Up when CanMoveRight(map) => (Turn.Right, Direction.Right),
            Direction.Down when CanMoveLeft(map) => (Turn.Right, Direction.Left),
            Direction.Down when CanMoveRight(map) => (Turn.Left, Direction.Right),
            Direction.Left when CanMoveDown(map) => (Turn.Left, Direction.Down),
            Direction.Left when CanMoveUp(map) => (Turn.Right, Direction.Up),
            Direction.Right when CanMoveUp(map) => (Turn.Left, Direction.Up),
            Direction.Right when CanMoveDown(map) => (Turn.Right, Direction.Down),
            _ => (Turn.None, Direction)
        };
        return turn;
    }

    public bool CanMoveLeft(ScaffoldMap map) => map[Row, Col - 1] == '#';
    public bool CanMoveRight(ScaffoldMap map) => map[Row, Col + 1] == '#';
    public bool CanMoveUp(ScaffoldMap map) => map[Row - 1, Col] == '#';
    public bool CanMoveDown(ScaffoldMap map) => map[Row + 1, Col] == '#';
}
