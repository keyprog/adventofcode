
string[] lines = File.ReadAllLines("/home/alexp/dev/adventofcode/2025/day6/input.txt");
var problems = Parse(lines);

long result = 0;

foreach(var (values, operation) in problems)
{
    long res = operation switch
    {
        "+" => values.Aggregate((agg, v) => checked(agg + v)),
        "*" => values.Aggregate((agg, v) => checked(agg * v)),
        _ => throw new InvalidOperationException(),
    };
    checked { result += res; }
}


result.Print();

static IEnumerable<(long[] values, string operation)> Parse(string[] lines)
{
    List<long> values = [];
    System.Text.StringBuilder valueBuilder = new();
    for(int col = lines[0].Length - 1; col >= 0; --col)
    {
        for(int row = 0; row < lines.Length; ++row)
        {
            char c = lines[row][col];
            if (char.IsDigit(c))
                valueBuilder.Append(c);
            else if (c is '+' or '*')
            {
                values.Add(long.Parse(valueBuilder.ToString()));
                yield return (values.ToArray(), c.ToString());
                values.Clear();
                valueBuilder.Clear();
            }
            
        }
        if (valueBuilder.Length > 0)
            values.Add(long.Parse(valueBuilder.ToString()));
        valueBuilder.Clear();        
    }
}

