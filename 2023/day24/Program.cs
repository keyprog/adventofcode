#define TEST
var stones = File.ReadLines("input.txt")
            .Select(l => l.Split([",", "@"], StringSplitOptions.TrimEntries).Select(decimal.Parse).ToArray())
            .Select(t => new Stone(t[0], t[1], t[2], t[3], t[4], t[5]))
            .ToArray();
#if TEST
double LowerBound = 7;
double UpperBound = 27;
#else
double LowerBound = 200_000_000_000_000;
double UpperBound = 400_000_000_000_000;
#endif

int count = 0;
for (int i = 0; i < stones.Length - 1; ++i)
    for (int j = i + 1; j < stones.Length; ++j)
        if (stones[i].CrossWith(stones[j]) is var inter
            && inter.Item1
            && inter.x >= LowerBound && inter.x <= UpperBound
            && inter.y >= LowerBound && inter.y <= UpperBound)
        {
            count++;
        }

count.Print();

foreach(var p in stones)
{
    Console.WriteLine($"y = {p.Slope}*x+{p.Offset}");
}

record Stone(decimal X, decimal Y, decimal Z, decimal VX, decimal VY, decimal VZ)
{
    public decimal Slope { get; } = GetSlope((X, Y, Z), (X + VX, Y + VY, Z + VZ));
    public decimal Offset { get; } = Y - GetSlope((X, Y, Z), (X + VX, Y + VY, Z + VZ)) * X;
    public (bool, double? x, double? y) CrossWith(Stone b) => Cross(this, b);

    public static decimal GetSlope((decimal X, decimal Y, decimal Z) p1, (decimal X, decimal Y, decimal Z) p2) => (decimal)(p2.Y - p1.Y) / (p2.X - p1.X);

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
        /* a * x1 + b * y1 + c = 0;
         a * x2 + b * y2 + c = 0;
         a * (x2 - x1) + b * (y2 - y1) = 0;
         a * (x2 -x1) = b * (y1 _ y2);
         (x2 - x1) / (y1 - y2) = b/a;
         b/a = (x2 - x1) / (y1 - y2);

         a * x + b * y = -c;

         y1 = k*x1 + a;
         y2 = k*x2 + a;

         y1 - y2 = k * (x1 - x2);
         k = (x1 - x2) / (y1 - y2);

         a = y - k*x;

         k1 * x + a1 = k2 * x + a2;

         k1 * x - k2 * x +a1 - a2 = 0;
         x (k1 - k2) + a1 - a2 = 0;
         x = (a2 - a1) / (k1 - k2);
         */
    }
}