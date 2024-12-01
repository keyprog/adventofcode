using MathNet.Numerics.LinearAlgebra;

var cloud = new Cloud(File.ReadAllLines("input.txt"));
Stone[] t1 = cloud.Stones;
var result = Solver.Solve(cloud.Stones[200..]);
Console.WriteLine(result);
Console.WriteLine(result.X + result.Y + result.Z);

/*for (int i = -1000; i < 1000; ++i)
for(int j = -1000; j < 1000; ++j)
//for(int k = 0; k < 1000; ++k)
{
    Stone s = new Stone((decimal)Math.Round(result.X) + i, (decimal)Math.Round(result.Y ) + j, (decimal)Math.Round(result.Z),
        (decimal)Math.Round(result.VX), (decimal)Math.Round(result.VY), (decimal)Math.Round(result.VZ));

    if (t1.All(s2 => s.CrossWith(s2).Item1))
        s.Print();
}*/
class Solver
{
    public static (double X, double Y, double Z, double VX, double VY, double VZ) Solve(Stone[] stones)
    {
        var (x, y, vx, vy) = SolvePlain(stones.Select(s => (s.X, s.Y, s.VX, s.VY)).ToArray());
        var (x1, z, vx1, vz) = SolvePlain(stones.Select(s => (s.X, s.Z, s.VX, s.VZ)).ToArray());
        var (y1, z1, vy1, vz1) = SolvePlain(stones.Select(s => (s.Y, s.Z, s.VY, s.VZ)).ToArray());
        return (x, y, z, vx, vy, vz);
    }

    private static (double X, double Y, double VX, double DY) SolvePlain((decimal x, decimal y, decimal dx, decimal dy)[] d)
    {
        var a = new double[4, 4];
        var b = new double[4];
        for (int i = 0; i < a.GetLength(0); ++i)
        {
            int i0 = i * 2;
            int i1 = i0 + 1;

            a[i, 0] = (double)(d[i1].dy - d[i0].dy);
            a[i, 1] = (double)(d[i0].dx - d[i1].dx);
            a[i, 2] = (double)(d[i0].y - d[i1].y);
            a[i, 3] = (double)(d[i1].x - d[i0].x);
            b[i] = (double)(d[i1].x * d[i1].dy - d[i1].y * d[i1].dx - d[i0].x * d[i0].dy + d[i0].y * d[i0].dx);
        };


        var aa = Matrix<double>.Build.DenseOfArray(a);
        var bb = Vector<double>.Build.Dense(b);

        var x = aa.Solve(bb);
        return (x[0], x[1], x[2], x[3]);
    }
}

class Cloud(string[] data)
{
    public Stone[] Stones { get; } = data
            .Select(l => l.Split([",", "@"], StringSplitOptions.TrimEntries).Select(decimal.Parse).ToArray())
            .Select(t => new Stone(t[0], t[1], t[2], t[3], t[4], t[5]))
            .ToArray();


    public static IEnumerable<((decimal DX, decimal DY, decimal DZ) Distance, Stone a, Stone b)> GetAllDistances(Stone[] t1, Stone[] t2)
    {
        for (int i = 0; i < t1.Length; ++i)
            for (int j = 0; j < t2.Length; ++j)
                if (i != j)
                {
                    yield return (t1[i].GetDistanceTo(t2[j]), t1[i], t2[j]);
                }

    }
}

record Stone(decimal X, decimal Y, decimal Z, decimal VX, decimal VY, decimal VZ)
{
    public Stone Move(decimal times) => new Stone(X + VX * times, Y + VY * times, Z + VZ * times, VX, VY, VZ);

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