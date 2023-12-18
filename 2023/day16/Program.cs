using static System.Linq.Enumerable;
using static Direction;

var map = new Map(File.ReadAllLines("input.txt"));

new[]{
    Range(0, map.Rows).Select(row => map.Simulate(new Beam (row, 0, R ))).Max(),
    Range(0, map.Rows).Select(row => map.Simulate(new Beam (row,  map.Cols - 1, L ))).Max(),
    Range(0, map.Cols).Select(col => map.Simulate(new Beam (0,  col, D ))).Max(),
    Range(0, map.Cols).Select(col => map.Simulate(new Beam (map.Rows - 1,  col,  U))).Max()
}.Max().Print();

class Map(string[] lines)
{
    private readonly string[] data = lines;
    public int Rows { get; } = lines.Length;
    public int Cols { get; } = lines[0].Length;
    private readonly List<Beam> beams = [];

    public int Simulate(Beam start)
    {
        beams.Add(start);

        HashSet<(int Row, int Col)> energizedTile = [];
        HashSet<(int Row, int Col, Direction Dir)> visited = [];

        while (beams.Count > 0)
        {
            foreach (var beam in beams.ToArray())
            {
                energizedTile.Add((beam.Row, beam.Col));

                char tile = data[beam.Row][beam.Col];
                switch (tile)
                {
                    case '.':
                        break;
                    case '/':
                        beam.Dir = beam.Dir switch
                        {
                            R => U,
                            L => D,
                            D => L,
                            U => R,
                            _ => throw new NotSupportedException()
                        };
                        break;
                    case '\\':
                        beam.Dir = beam.Dir switch
                        {
                            R => D,
                            L => U,
                            D => R,
                            U => L,
                            _ => throw new NotSupportedException()
                        };
                        break;
                    case '|':
                        if (beam.Dir is L or R)
                        {
                            var beam2 = new Beam(beam.Row, beam.Col, U);
                            beams.Add(beam2);
                            beam2.Move(this, beams, visited);
                            beam.Dir = D;
                        }
                        break;
                    case '-':
                        if (beam.Dir is U or D)
                        {

                            var beam2 = new Beam(beam.Row, beam.Col, L);
                            beams.Add(beam2);
                            beam2.Move(this, beams, visited);
                            beam.Dir = R;
                        }
                        break;
                }
                beam.Move(this, beams, visited);

            }
        }
        return energizedTile.Count;
    }
}

class Beam(int row, int col, Direction dir)
{
    public int Row { get; set; } = row;
    public int Col { get; set; } = col;
    public Direction Dir { get; set; } = dir;

    public void Move(Map map, List<Beam> beams, HashSet<(int, int, Direction)> visited)
    {
        bool isAlive = Dir switch
        {
            U => --Row >= 0,
            D => ++Row < map.Rows,
            L => --Col >= 0,
            R => ++Col < map.Cols,
            _ => throw new NotSupportedException()
        };
        if (!isAlive || !visited.Add((Row, Col, Dir)))
            beams.Remove(this);
    }
}

enum Direction { U, D, L, R }
