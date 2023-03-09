const string Input = "input2.txt";
var dict = File.ReadLines(Input).Select(InputParser.ParseLine).ToDictionary(v => v.name, v => v.Monkey);
MathMonkey root = (MathMonkey)dict["root"];
Console.WriteLine("Root: " + root.GetValue(dict));
var pathToHumn = GetPathTo("humn", root).Skip(1).ToList();
pathToHumn.ForEach(v => Console.WriteLine(v.Name));
long eq1 = dict[root.Monkey1 == pathToHumn[0].Name ? root.Monkey2 : root.Monkey1].GetValue(dict);
long humnValue = pathToHumn[0].GetReverseValue(dict, new Queue<IMonkey>(pathToHumn.Skip(1)), eq1);

Console.WriteLine("Humn: " + humnValue);

IEnumerable<IMonkey> GetPathTo(string name, IMonkey from)
{
    if (from.Name == name)
        return new[] { from };

    if (from is MathMonkey mm)
    {
        var subpath = GetPathTo(name, dict[mm.Monkey1]).Concat(GetPathTo(name, dict[mm.Monkey2])).ToArray();
        if (subpath.Length > 0)
            return new[] { from }.Concat(subpath);
    }
    return Array.Empty<IMonkey>();
}

abstract record class IMonkey(string Name)
{
    public abstract long GetValue(Dictionary<string, IMonkey> map);
    public abstract long GetReverseValue(Dictionary<string, IMonkey> map, Queue<IMonkey> path, long value);
}

record class NumberMonkey(string Name, long Number) : IMonkey(Name)
{
    public override long GetValue(Dictionary<string, IMonkey> map) => Number;


    public override long GetReverseValue(Dictionary<string, IMonkey> map, Queue<IMonkey> path, long value) => value;
}

record class MathMonkey(string Name, string Monkey1, string Monkey2, char Op) : IMonkey(Name)
{
    long? value;
    public override long GetValue(Dictionary<string, IMonkey> map)
    {
        checked
        {
            if (value.HasValue)
                return value.Value;
            long val1 = map[Monkey1].GetValue(map);
            long val2 = map[Monkey2].GetValue(map);
            value = Op switch
            {
                '+' => val1 + val2,
                '-' => val1 - val2,
                '*' => val1 * val2,
                '/' => val1 / val2,
                _ => throw new NotSupportedException($"Invalid operation '{Op}'")
            };
        }
        return value.Value;
    }


    public override long GetReverseValue(Dictionary<string, IMonkey> map, Queue<IMonkey> path, long value)
    {
        if (path.Count == 0)
            return value;

        var next = path.Dequeue();
        var (total, val2) = next.Name == Monkey1 ? (value, map[Monkey2].GetValue(map)) : (value, map[Monkey1].GetValue(map));
        value = Op switch
        {
            '+' => total - val2,
            '-' when next.Name == Monkey1 => total + val2,
            '-' when next.Name == Monkey2 => val2 - total,
            '*' => total / val2,
            '/' when next.Name == Monkey1 => total * val2,
            '/' when next.Name == Monkey2 => val2 / total,
            _ => throw new ApplicationException()
        };
        return map[next.Name].GetReverseValue(map, path, value);

    }
}
static class InputParser
{
    private static (string, string, string) ToTuple(this string[] opArr) => (opArr[0], opArr[1], opArr[2]);
    public static (string name, IMonkey Monkey) ParseLine(string line)
    {
        int nameSeparatorInd = line.IndexOf(':');
        string name = line.Substring(0, nameSeparatorInd);
        return line.Substring(nameSeparatorInd + 2).Split(' ') switch
        {
            [var num] => (name, new NumberMonkey(name, int.Parse(num))),
            [var v1, var op, var v2] => ((name, new MathMonkey(name, v1, v2, op[0]))),
            _ => throw new ApplicationException()
        };
    }
}
