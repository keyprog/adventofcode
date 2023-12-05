string[] lines = File.ReadAllLines("input_test.txt");
Map map = new(lines);
Number[] numbers = map.FindAllNumbers();

foreach (var n in numbers)
    Console.WriteLine($"{n} -> {map.HasSymbolAdjacentTo(n)}");

long part1 = numbers.Where(n => map.HasSymbolAdjacentTo(n)).Sum(n => n.Value);
Console.WriteLine($"Part 1: {part1}");

Gear[] gears = map.FindAllGears(numbers);
foreach (var g in gears)
    Console.WriteLine($"{g.Row}x{g.Col} => {g.Numbers.Count} => {g.Numbers.Aggregate(1L, (p, g) => p * g.Value)}");

long part2 = gears.Where(g => g.Numbers.Count == 2).Sum(g => g.Numbers[0].Value * g.Numbers[1].Value);
Console.WriteLine($"Part 2: {part2}");

class Map(string[] lines)
{
    private readonly string[] lines = lines;
    public int RowsCount { get; } = lines.Length;
    public int ColsCount { get; } = lines.Length > 0 ? lines[0].Length : 0;

    public Number[] FindAllNumbers()
    {
        List<Number> numbers = [];
        for (int row = 0; row < RowsCount; ++row)
        {
            Number? num = null;
            for (int col = 0; col < ColsCount; col++)
            {
                char c = lines[row][col];
                if (char.IsDigit(c))
                {
                    if (num is null)
                    {
                        num = new Number(c - '0', row, col, 1);
                    }
                    else
                    {
                        num = num with { Value = num.Value * 10 + c - '0', Length = num.Length + 1 };
                    }
                }
                else
                {
                    if (num is not null)
                    {
                        numbers.Add(num);
                        num = null;
                    }
                }
            }
            if (num is not null)
                numbers.Add(num);
        }
        return numbers.ToArray();
    }
    public Gear[] FindAllGears(IEnumerable<Number> numbers)
    {
        Dictionary<(int, int), Gear> gears = [];
        foreach (var number in numbers)
        {
            foreach (var (row, col) in number.GetAdjacent())
            {
                if (gears.TryGetValue((row, col), out Gear? gear))
                {
                    gear.Numbers.Add(number);
                }
                else if (HasGearAt(row, col))
                {
                    gear = new Gear(row, col);
                    gear.Numbers.Add(number);
                    gears.Add((row, col), gear);
                }

            }
        }
        return gears.Values.ToArray();
    }

    public bool HasSymbolAt(int row, int col)
        => IsValidCoordinate(row, col) && lines[row][col] != '.' && !char.IsDigit(lines[row][col]);

    public bool HasGearAt(int row, int col)
        => IsValidCoordinate(row, col) && lines[row][col] == '*';

    private bool IsValidCoordinate(int row, int col)
        => row >= 0 && row < RowsCount && col >= 0 && col < ColsCount;

    public bool HasSymbolAdjacentTo(Number num)
        => num.GetAdjacent().Any(c => HasSymbolAt(c.Row, c.Col));
}

record Number(long Value, int Row, int Col, int Length)
{
    public IEnumerable<(int Row, int Col)> GetAdjacent()
    {
        yield return (Row, Col - 1);
        yield return (Row, Col + Length);

        for (int i = Col - 1; i <= Col + Length; ++i)
        {
            yield return (Row - 1, i);
            yield return (Row + 1, i);
        }
    }
}
record Gear(int Row, int Col)
{
    public List<Number> Numbers { get; } = [];
}