const string Input = "input2.txt";
int[][] cubes = File.ReadLines(Input).Select(l => l.Split(',')).Select(p => p.Select(pp => int.Parse(pp)).ToArray()).ToArray();
cubes = Utils.IncludeNonReachableCubes(cubes).ToArray();


int adjTotal = 0;
foreach (var per in Utils.GetPermutations(new[] { 0, 1, 2 }))
{
    adjTotal += cubes.Select(c => Utils.Num(c[per[0]], c[per[1]], c[per[2]])).Order().GetAdjacentTotal();//.ForEach(c => Console.WriteLine($"{c.Item1},{c.Item2},{c.Item3}"));
}

Console.WriteLine("Adjacents: " + adjTotal);
int total = cubes.Length * 6;
Console.WriteLine(total);
Console.WriteLine(total - adjTotal);


static class Utils
{
    public static IEnumerable<int[]> GetPermutations(IEnumerable<int> arr)
    {
        return arr.Select(a => new[] { a }.Concat(arr.Where(ee => ee != a)).ToArray());
    }
    public static int Num(int d0, int d1, int d2) => d0 + d1 * 1000 + d2 * 1000000;
    public static int GetAdjacentTotal(this IEnumerable<int> sorted)
    {
        int adjacent = 0;
        int p = sorted.First();
        foreach (var c in sorted.Skip(1))
        {
            if (c - p == 1)
                adjacent += 2;

            p = c;
        }
        return adjacent;
    }

    public static IEnumerable<int[]> IncludeNonReachableCubes(IEnumerable<int[]> cubes)
    {
        int maxOrd = cubes.SelectMany(c => c).Max() + 1;
        var dropMap = new char[maxOrd, maxOrd, maxOrd];
        foreach (var c in cubes)
            dropMap[c[0], c[1], c[2]] = '*';

        var q = new Queue<(int, int, int)>();
        q.Enqueue((0, 0, 0));
        while (q.TryDequeue(out var p))
        {
            (int x, int y, int z) = p;
            if (x < 0 || x >= maxOrd || y < 0 || y >= maxOrd || z < 0 || z >= maxOrd || dropMap[x, y, z] != 0)
                continue;
            dropMap[x, y, z] = 'x';
            var next = new[] { (x - 1, y, z), (x + 1, y, z), (x, y - 1, z), (x, y + 1, z), (x, y, z + 1), (x, y, z - 1) };
            foreach (var n in next)
                q.Enqueue(n);
        }

        for (int x = 0; x < maxOrd; ++x)
            for (int y = 0; y < maxOrd; ++y)
                for (int z = 0; z < maxOrd; ++z)
                    if (dropMap[x, y, z] != 'x')
                        yield return new[] { x, y, z };


    }

}