using System.Linq.Expressions;

const string InputFile = "input2.txt";
const int TotalRounds = 10000;

List<Monkey> monkeys = InputParser.ParseLines(File.ReadLines(InputFile));

uint commonMultiple = 1; // to keep up with large worry, we will adjust it based on lcm
monkeys.ForEach(m => commonMultiple *= m.ThrowDivisibleTo);

for (int i = 0; i < TotalRounds; ++i)
{
    foreach (var monkey in monkeys)
    {
        foreach (var (item, throwTo) in monkey.Inspect(commonMultiple))
        {
            monkeys[throwTo].AddItem(item);
        }
    }
    Console.WriteLine($"Round {i + 1}");
    monkeys.ForEach(Console.WriteLine);
}

ulong monkeyBusiness = 1;
foreach (var m in monkeys.OrderByDescending(m => m.TotalInspected).Take(2))
    monkeyBusiness = monkeyBusiness * (ulong)m.TotalInspected;
Console.WriteLine("Monkey business: " + monkeyBusiness);

class InputParser
{
    private const string MonkeySetCmd = "Monkey ";
    private const string StartingItemsCmd = "  Starting items: ";
    private const string WorryLevelOperationCmd = "  Operation: new = ";
    private const string ThrowTestCmd = "  Test: divisible by ";
    private const string ThrowTestIfTrue = "    If true: throw to monkey ";
    private const string ThrowTestIfFalse = "    If false: throw to monkey ";

    public static List<Monkey> ParseLines(IEnumerable<string> lines)
    {
        List<Monkey> monkeys = new();
        foreach (var line in lines)
        {
            var monkey = InputParser.ParseLine(line, monkeys.LastOrDefault());
            if (monkey != null)
                monkeys.Add(monkey);
        }
        return monkeys;
    }
    public static Monkey? ParseLine(string line, Monkey? current)
    {
        switch (line)
        {
            case { } when line.StartsWith(MonkeySetCmd):
                int monkeyIndex = int.Parse(line[MonkeySetCmd.Length..line.IndexOf(':')]);
                return new Monkey(monkeyIndex);
            case { } when line.StartsWith(StartingItemsCmd):
                var items = line[StartingItemsCmd.Length..].Split(", ").Select(uint.Parse);
                foreach (var item in items)
                    current?.AddItem(item);
                break;
            case { } when line.StartsWith(WorryLevelOperationCmd):
                if (current == null)
                    throw new ApplicationException("Unexpected input");

                string[] tokens = line[WorryLevelOperationCmd.Length..].Split(' ');

                Expression body = tokens[1] switch
                {
                    "+" => Expression.AddChecked(ParseArg(tokens[0]), ParseArg(tokens[2])),
                    "*" => Expression.MultiplyChecked(ParseArg(tokens[0]),ParseArg(tokens[1])),
                    _ => throw new ArgumentException("Not supported yet")
                };
                current.WorryLevelOperation =Expression.Lambda<Func<ulong, ulong>>(body, oldArgExpr).Compile();
                break;
            case { } when line.StartsWith(ThrowTestCmd):
                if (current == null)
                    throw new ApplicationException("Unexpected input");
                current.ThrowDivisibleTo = uint.Parse(line[ThrowTestCmd.Length..]);
                break;
            case { } when line.StartsWith(ThrowTestIfTrue):
                if (current == null)
                    throw new ApplicationException("Unexpected input");
                current.ThrowToIfTrue = int.Parse(line[ThrowTestIfTrue.Length..]);
                break;
            case { } when line.StartsWith(ThrowTestIfFalse):
                if (current == null)
                    throw new ApplicationException("Unexpected input");
                current.ThrowToIfFalse = int.Parse(line[ThrowTestIfFalse.Length..]);
                break;
            case "":
                break;
            default:
                throw new ArgumentException("Unexpected input line: " + line);

        }
        return null;
    }


    private const string OldArg = "old";
    private static readonly ParameterExpression oldArgExpr = Expression.Parameter(typeof(ulong), OldArg);
    private static Expression ParseArg(string token)
    {
        return token == OldArg ? oldArgExpr : Expression.Constant(ulong.Parse(token));
    }
}

record class Monkey(int MonkeyIndex)
{
    private readonly List<uint> items = new();
    public int TotalInspected { get; set; } = 0;

    public void AddItem(uint item) => items.Add(item);
    public Func<ulong, ulong> WorryLevelOperation { get; set; } = (w) => w;

    public uint ThrowDivisibleTo { get; set; }
    public int ThrowToIfTrue { get; set; }
    public int ThrowToIfFalse { get; set; }

    public IEnumerable<(uint, int)> Inspect(uint adjustmentFactor)
    {
        checked
        {
            for (int i = 0; i < items.Count; ++i)
            {
                uint item = items[i];
                TotalInspected++;
                item = (uint)(WorryLevelOperation(item) % adjustmentFactor);
                yield return (item % ThrowDivisibleTo == 0) ? (item, ThrowToIfTrue) : (item, ThrowToIfFalse);
            }
        }
        items.Clear();
    }

    public override int GetHashCode() => MonkeyIndex;

    public override string ToString()
    {
        return $"Monkey [{MonkeyIndex}], inspected {TotalInspected}: {string.Join(", ", items)}";
    }

}