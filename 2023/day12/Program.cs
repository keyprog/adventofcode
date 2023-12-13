using static System.Linq.Enumerable;

Parser.Repeats = 5;
decimal total = File.ReadLines("input.txt").Select(Parser.ParseLine).Sum(r => r.CountArrangements());
Console.WriteLine("Total: " + total);

static class Parser
{
    public static int Repeats {get;set;} = 1;
    public static Record ParseLine(string line) => ParseLine(line.AsSpan());
    public static Record ParseLine(ReadOnlySpan<char> line)
    {
        int sp = line.IndexOf(' ');
        string template = line[0..sp].ToString();
        template = string.Join('?', Repeat(template, Repeats));
        string broken = line[(sp + 1)..].ToString();
        broken = string.Join(',', Repeat(broken, Repeats));
        return new Record(template, broken.Split(',').Select(int.Parse).ToArray());
    }
}

class Record(string template, int[] broken)
{
    public string Template { get; } = template;
    public int[] Broken { get; } = broken;

    public long CountArrangements()
    {
        int[] operational = new int[Broken.Length];
        Array.Fill(operational, 1, 1, operational.Length - 1);

        int free = Template.Length - Broken.Sum() - operational.Sum();
        if (free == 0) return 1;
        return CountArrangements(operational, free, 0, 0, new());
    }

    private long CountArrangements(int[] operational, int free, int index, int start, Dictionary<(int, int), long> memo)
    {
        if (memo.TryGetValue((start,index), out long arrangements)) 
            return arrangements;

        if (index == operational.Length)        
            return IsOperational(start, Template.Length) ? 1 : 0;       

        for (int n = 0; n <= free; ++n)
        {
            operational[index] += n;
            if (IsOperational(start, start + operational[index])
                && IsBroken(start + operational[index], start + operational[index] + Broken[index]))
            {
                arrangements += CountArrangements(operational, free - n, index + 1, start + operational[index] + Broken[index], memo);
            }
            operational[index] -= n;
        }
        memo[(start,index)] = arrangements;
        return arrangements;
    }

    private bool IsOperational(int start, int end)
    => IsRangeOf(Template, start, end, '.');
    
    private bool IsBroken(int start, int end)
    => IsRangeOf(Template, start, end, '#');
    
    private static bool IsRangeOf(string template, int start, int end, char target)
    {
        for (int j = start; j < end; j++)
        {
            char c = template[j];
            if (c != '?' && c != target)
                return false;
        }
        return true;
    }
}