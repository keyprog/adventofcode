const string Input = "input2.txt";
Log.LogEnabled = false;
// Part I
var lines = File.ReadLines(Input).Chunk(3);
var pairs = lines.Select((p, i) => (Index: i + 1, Left: Parser.Parse(p[0]), Right: Parser.Parse(p[1])));

int sum = 0;
foreach (var p in pairs)
{
    Log.Line($"== Pair {p.Index} ==");
    if (TokensComparer.Compare(p.Left, p.Right) < 0)
        sum += p.Index;

    Log.Line("");
}
Console.WriteLine("Sum: " + sum);

// Part II
var div1 = Parser.Parse("[[2]]");
var div2 = Parser.Parse("[[6]]");
ListToken[] packets = File.ReadLines(Input)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => Parser.Parse(l))
                .Union(new[] { div1, div2 })
                .ToArray();

Array.Sort(packets, (p1, p2) => TokensComparer.Compare(p1, p2));
if (Log.LogEnabled) packets.ToList().ForEach(Console.WriteLine);

int div1Ind = Array.BinarySearch(packets, 0, packets.Length, div1) + 1;
int div2Ind = Array.BinarySearch(packets, 0, packets.Length, div2) + 1;

Console.WriteLine("Result: " + (div1Ind * div2Ind));

class Parser
{
    public static ListToken Parse(ReadOnlySpan<char> line)
    {
        if (line[0] != '[' || line[line.Length - 1] != ']')
            throw new ApplicationException("Unexpected input");
        var list = new ListToken();
        ParseList(line[1..], list);
        return list;
    }

    private static void AddValue(List<char> value, ListToken list)
    {
        if (value.Count > 0)
        {
            list.Values.Add(new ValueToken(int.Parse(value.ToArray())));
            value.Clear();
        }

    }
    private static int ParseList(ReadOnlySpan<char> line, ListToken parentList)
    {
        List<char> value = new();
        int pos = 0;
        while (pos < line.Length)
        {
            switch (line[pos])
            {
                case '[':
                    pos++;
                    var list = new ListToken();
                    parentList.Values.Add(list);
                    pos += ParseList(line[pos..], list);
                    break;
                case ']':
                    AddValue(value, parentList);
                    return ++pos;
                case ',':
                    AddValue(value, parentList);
                    ++pos;
                    break;
                default: // accumulating current value
                    value.Add(line[pos]);
                    ++pos;
                    break;

            }
        }
        return pos;
    }
}

class Log
{
    public static bool LogEnabled { get; set; }
    public static void Line(string msg)
    {
        if (!LogEnabled)
            return;
        Console.WriteLine(msg);
    }
    public static void Line(string msg, int level)
    {
        if (!LogEnabled)
            return;
        if (string.IsNullOrEmpty(msg))
            return;
        Console.WriteLine($"{new string(' ', level * 2)}- {msg}");
    }
}

class TokensComparer
{
    public static int Compare(ListToken leftList, ListToken rightList, int level = 0)
    {
        Log.Line($"Compare {leftList} vs {rightList}", level);
        var left = leftList.Values.GetEnumerator();
        var right = rightList.Values.GetEnumerator();
        while (true)
        {
            switch (left.MoveNext(), right.MoveNext())
            {
                case (false, true): return -1; // left shorter
                case (true, false): return 1; // right shorter
                case (false, false): return 0; // lists with equal lengths
                case (true, true):
                    int res = CompareTokens(left.Current, right.Current, level + 1);
                    if (res != 0) return res;
                    break;
            }
        }
    }

    private static int CompareTokens(BaseToken leftToken, BaseToken rightToken, int level) =>
    (leftToken, rightToken) switch
    {
        (ValueToken v1, ValueToken v2) => v1.Value - v2.Value,
        (ListToken l1, ListToken l2) => Compare(l1, l2, level),
        (ValueToken v1, ListToken l2) => Compare(new ListToken { Values = { v1 } }, l2, level),
        (ListToken l1, ValueToken v2) => Compare(l1, new ListToken { Values = { v2 } }, level),
        _ => throw new ApplicationException()
    };

}

record class BaseToken { }

record class ListToken : BaseToken, IComparable<ListToken>
{
    public List<BaseToken> Values { get; } = new List<BaseToken>();

    public int CompareTo(ListToken? list) => TokensComparer.Compare(this, list ?? throw new ArgumentNullException(nameof(list)));

    public override string ToString() => $"[{string.Join(',', Values)}]";
}

record class ValueToken(int Value) : BaseToken
{
    public override string ToString() => Value.ToString();
}