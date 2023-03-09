const string InputFile = "input2.txt";
//const int Y = 10;
//const int Y = 2000000;
//const int MaxCoord = 20;
const int MaxCoord = 4000000;
List<Sensor> sensors = File.ReadLines(InputFile).Select(Sensor.Parse).ToList();

for (int y = 0; y < MaxCoord; ++y)
{
    (int, int)[] lines = sensors.Select(s => s.GetCoverage(y))
                                .Where(l => l.Start <= MaxCoord && l.End >= 0)
                                .Select(l => (Math.Max(0, l.Start), Math.Min(MaxCoord, l.End)))
                                .Where(l => l != (0, 0))
                                .OrderBy(l => l.Item1).ToArray();

    //Console.WriteLine("Lines: ");
    //foreach (var l in lines)
    //  Console.WriteLine(l);

    var combined = CombineIntersected(lines);
    int total = combined.Sum(l => l.Item2 - l.Item1 + 1);
    int beaconsOnLine = sensors.Where(s => s.bY == y).Select(s => s.bX).Distinct().Count();
    //total -= beaconsOnLine;
    if (total < MaxCoord + 1){
        int x = combined.Length switch
        {
            1 => combined[0].Item1 == 0 ? MaxCoord : 0,
            2 => combined[0].Item2 + 1,
            _ => throw new ApplicationException()
        };
        checked{
            ulong res= (ulong)x*4000000ul +(ulong)y;
        Console.WriteLine($"Total {total}, x={x}, y={y}, result={res}");
        }
    foreach (var l in combined)
        Console.WriteLine(l);
    }
}
(int, int)[] CombineIntersected((int start, int end)[] lines)
{
    if (lines.Length < 2)
        return lines;

    if (lines[0].end - lines[1].start >=0 )
    {
        var combined = (lines[0].start, Math.Max(lines[0].end, lines[1].end));
        return CombineIntersected(new[] { combined }.Union(lines[2..]).ToArray());
    }

    return (new[] { lines[0] }).Union(CombineIntersected(lines[1..])).ToArray();
}

record struct Sensor(int sX, int sY, int bX, int bY)
{
    //Sensor at x=2, y=18: closest beacon is at x=-2, y=15
    const string XToken = "x=";
    const string YToken = "y=";
    public static Sensor Parse(string line)
    {
        int xPos = line.IndexOf(XToken) + XToken.Length;
        int yPos = line.IndexOf(YToken, xPos) + YToken.Length;
        int bxPos = line.IndexOf(XToken, yPos) + XToken.Length;
        int byPos = line.IndexOf(YToken, bxPos) + XToken.Length;
        return new Sensor(ParseInt(line, xPos), ParseInt(line, yPos), ParseInt(line, bxPos), ParseInt(line, byPos));
    }

    private static int ParseInt(string line, int pos)
    {
        int endPos = line.IndexOfAny(new[] { ',', ':' }, pos);
        return endPos < 0 ? int.Parse(line[pos..]) : int.Parse(line[pos..endPos]);
    }


    public (int Start, int End) GetCoverage(int y)
    {
        int coverageRadius = Math.Abs(sX - bX) + Math.Abs(sY - bY);
        int yDistance = Math.Abs(sY - y);
        int xDistance = coverageRadius - yDistance;
        return xDistance <= 0 ? (0, 0) : (sX - xDistance, sX + xDistance);
    }
}
record struct Beacon(int X, int Y);