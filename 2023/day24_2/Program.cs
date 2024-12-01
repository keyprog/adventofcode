﻿var cloud = new Cloud(File.ReadAllLines("input.txt"));
Dictionary<(decimal,decimal,decimal), int> distCounts = [];
Stone[] t1 = cloud.Stones;
for(int i =0; i<100000; i++)
{
    Stone[] t2 = cloud.Stones.Select(s => s.Move(i)).ToArray();
    var distances = Cloud.GetAllDistances(t1, t2);//.OrderBy(d => Math.Abs(d.DX) + Math.Abs(d.DY) + Math.Abs(d.DZ));
    t1 = t2;
    foreach(var (d, _, __) in distances)
    {
        if (!distCounts.TryGetValue(d, out int count))
        {
            distCounts[d] = 1;
        }else
        {
            distCounts[d]++;
            if (distCounts[d] > 3)
            {
                Console.WriteLine($"{d} => {distCounts[d]}");
            }
        }
    }
}

var top = distCounts.MaxBy(v => v.Value);

foreach(var p in distCounts.OrderByDescending(v => v.Value).Take(10))
{
    Console.WriteLine($"{p.Key} => {p.Value}");
}

var t = Cloud.GetAllDistances(cloud.Stones, cloud.Stones.Select(s => s.Move(1)).ToArray()).Where(d => d.Distance == top.Key).ToArray();
t.Length.Print();

foreach(var (d, s,_) in t)
{
    Console.WriteLine(s.X - d.DX + s.Y - d.DY + s.Z - d.DZ);
}


class Cloud(string[] data)
{
    public Stone[] Stones {get;} = data
            .Select(l => l.Split([",", "@"], StringSplitOptions.TrimEntries).Select(decimal.Parse).ToArray())
            .Select(t => new Stone(t[0], t[1], t[2], t[3], t[4], t[5]))
            .ToArray();


    public static IEnumerable<((decimal DX, decimal DY, decimal DZ) Distance, Stone a, Stone b)> GetAllDistances(Stone[] t1, Stone[] t2)
    {
        for(int i =0; i<t1.Length ; ++i)
         for(int j = 0;j<t2.Length; ++j)
            if (i != j)
            {
                yield return (t1[i].GetDistanceTo(t2[j]), t1[i], t2[j]);
            }

    }
}

record Stone(decimal X, decimal Y, decimal Z, decimal VX, decimal VY, decimal VZ)
{
    public Stone Move(int times) => new Stone(X + VX * times, Y + VY * times, Z + VZ * times, VX, VY, VZ);

    public (decimal DX, decimal DY, decimal DZ) GetDistanceTo(Stone other)
        => (other.X - X, other.Y - Y, other.Z - Z); 

    public decimal Slope { get; } = GetSlope((X, Y, Z), (X + VX, Y + VY, Z + VZ));
    public decimal Offset { get; } = Y - GetSlope((X, Y, Z), (X + VX, Y + VY, Z + VZ)) * X;
    public (bool, double? x, double? y) CrossWith(Stone b) => Cross(this, b);

    public static decimal GetSlope((decimal X, decimal Y, decimal Z) p1, (decimal X, decimal Y, decimal Z) p2) 
        => (decimal)(p2.Y - p1.Y) / (p2.X - p1.X);

    public bool IsInFuture(double x, double y) => IsGreater((double)X, x, VX) && IsGreater((double)Y, y, VY);

    public static bool IsGreater(double c1, double c2, decimal v) => v >= 0 ? c2 > c1 : c2 < c1;
    public (bool, double? X, double? Y) Cross(Stone a, Stone b)
    {
        if (a.Slope == b.Slope)
        {
            // && a.Offset != b.Offset) return (false, null, null);
            Console.WriteLine(a.Offset + " " + b.Offset);
            return (false, null, null);
        }

        double x = (double)(b.Offset - a.Offset) / (double)(a.Slope - b.Slope);
        double y = (double)a.Slope * (double)x + (double)a.Offset;

        return (a.IsInFuture(x, y) && b.IsInFuture(x, y), x, y);
    }
}