
bool isTest = false;
Almanac almanac = File.ReadAllLines(isTest ? "input_test.txt" : "input.txt");

var (seed, location) = FindSeedWithMinLocation(almanac);
Console.WriteLine($"Seed {seed}, Location: {location}");

static (long minSeed, long minLocation) FindSeedWithMinLocation(Almanac almanac)
{
    long minSeed = 0;
    long minLocation = long.MaxValue;
    foreach (var (start, end) in almanac.GetSeedsRanges())
    {
        Console.WriteLine($"Range {start}-{end} length {end - start}");

        for (long seed = start; seed < end;)
        {
            long num = seed;
            long rangeLength = long.MaxValue;
            foreach (var map in almanac.Maps)
            {
                (num, long len) = map.Next(num);
                rangeLength = Math.Min(rangeLength, len);
            }
            if (minLocation > num)
            {
                minLocation = num;
                minSeed = seed;
            }
            seed += rangeLength;
        }
    }

    return (minSeed, minLocation);
}

class Almanac
{
    public long[] Seeds { get; init; } = [];
    public Map[] Maps { get; init; } = [];

    public Almanac(string[] lines)
    {
        (Seeds, Maps) = Parse(lines);
    }

    public static implicit operator Almanac(string[] lines) => new(lines);

    private static (long[] seeds, Map[] maps) Parse(string[] lines)
    {
        long[] seeds = [];
        List<Map> maps = [];

        foreach (string line in lines.Where(l => l.Length > 0))
        {
            if (line.StartsWith("seeds: "))
                seeds = line["seeds: ".Length..].Split(' ').Select(long.Parse).ToArray();
            else if (line.EndsWith(" map:"))
                maps.Add(new Map());
            else if (line.Split(' ').Select(long.Parse).ToArray() is [long dest, long src, long length])
            {
                maps[^1].Ranges.Add(src, new MapRange(dest, src, length));
            }
        }
        return (seeds, maps.ToArray());
    }

    public IEnumerable<(long start, long end)> GetSeedsRanges()
    {
        for (int i = 0; i < Seeds.Length; i += 2)
        {
            long rangeStart = Seeds[i];
            long rangeEnd = rangeStart + Seeds[i + 1];
            yield return (rangeStart, rangeEnd);
        }
    }
}

class Map
{
    public SortedList<long, MapRange> Ranges { get; } = [];

    public (long num, long length) Next(long num)
    {
        foreach (var (_, r) in Ranges)
        {
            if (num < r.SourceStart)
                return (num, r.SourceStart - num);

            long offset = num - r.SourceStart;
            if (offset < r.Length)
                return (r.DestinationStart + offset, r.Length - offset);
        }
        return (num, long.MaxValue - num);
    }
}
readonly record struct MapRange(long DestinationStart, long SourceStart, long Length);