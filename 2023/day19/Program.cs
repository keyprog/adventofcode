Sorter sorter = new(File.ReadAllLines("input.txt"));

sorter.Parts.Where(sorter.IsPartAccepted).Sum(p => p.Rating).Print();
sorter.CalcTotalAccepted().Print();

class Sorter
{
    public Dictionary<string, Workflow> Workflows { get; } = [];
    public List<Part> Parts { get; } = [];

    public Sorter(string[] lines)
    {
        bool isWorkflow = true;
        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                isWorkflow = false;
                continue;
            }

            if (isWorkflow)
            {
                var workflow = ParseWorkflow(line);
                Workflows[workflow.Name] = workflow;
            }
            else
            {
                Parts.Add(ParsePart(line));
            }
        }
    }

    public bool IsPartAccepted(Part part)
    {
        var workflow = Workflows["in"];
        while (true)
        {
            string result = workflow.Evaluate(part);
            switch (result)
            {
                case "A": return true;
                case "R": return false;
                default:

                    workflow = Workflows[result];
                    break;
            }
        }
    }

    public decimal CalcTotalAccepted()
    {
        var root = this.Workflows["in"];
        Queue<(Workflow, Part min, Part max)> q = new(new[] { (root, Part.One, Part.FourK) });
        decimal accepted = 0;
        while (q.Count > 0)
        {
            var (wf, min, max) = q.Dequeue();
            foreach (var rule in wf.Rules)
            {
                (Range match, (min, max)) = rule.SplitIntoRanges(min, max);
                if (rule.Success == "A") // leaf
                {
                    accepted += Part.CalcPermutations(match.Min, match.Max);
                }
                else if (rule.Success != "R")
                {
                    if (Part.CalcPermutations(match.Min, match.Max) > 0)
                        q.Enqueue((this.Workflows[rule.Success], match.Min, match.Max));
                }
            }
        }
        return accepted;
    }

    public static Workflow ParseWorkflow(string line)
    {
        // px{a<2006:qkq,m>2090:A,rfg}
        int ruleStart = line.IndexOf('{');
        string name = line[0..ruleStart];
        string[] expr = line[(ruleStart + 1)..^1].Split(',');
        Rule[] rules = expr.Select(ParseRule).ToArray();
        return new Workflow(name, rules);
    }

    private static readonly char[] RuleOps = ['<', '>'];
    public static Rule ParseRule(string exp)
    {
        ReadOnlySpan<char> expression = exp;
        int opIndex = expression.IndexOfAny(RuleOps);
        if (opIndex < 0)
            return Rule.Always(exp);
        char category = expression[0];
        int next = expression.IndexOf(':');
        return new Rule(category, expression[1], int.Parse(expression[2..next]), expression[(next + 1)..].ToString());
    }

    public static Part ParsePart(string line)
    {
        // {x=787,m=2655,a=1222,s=2876}        
        string[] tokens = line[1..^1].Split(',');
        return tokens.Aggregate(Part.Zero, (p, t) => p.With(t[0], int.Parse(t.AsSpan()[2..])));
    }
}
record Part(int X, int M, int A, int S)
{
    public static Part Zero = new(0, 0, 0, 0);
    public static Part One = new(1, 1, 1, 1);
    public static Part FourK = new(4000, 4000, 4000, 4000);
    public static decimal CalcPermutations(Part min, Part max)
    {
        decimal total = (decimal)(max.A - min.A + 1) * (max.M - min.M + 1) * (max.S - min.S + 1) * (max.X - min.X + 1);
        return total > 0 ? total : 0;
    }

    public Part With(char category, int value)
        => category switch
        {
            'x' => this with { X = value },
            'm' => this with { M = value },
            's' => this with { S = value },
            'a' => this with { A = value },
            _ => throw new NotSupportedException()
        };

    public int Get(char category)
        => category switch
        {
            'x' => X,
            'm' => M,
            's' => S,
            'a' => A,
            _ => throw new NotSupportedException()
        };
    public int Rating { get => X + M + A + S; }
}

record Range(Part Min, Part Max);

record Workflow(string Name, Rule[] Rules)
{
    public string Evaluate(Part part)
    {
        foreach (var rule in Rules)
        {
            string? res = rule.Evaluate(part);
            if (res is not null) return res;
        }
        throw new ApplicationException($"Missing rules in workflow {Name}");
    }
}

record Rule(char Category, char Op, int Value, string Success)
{
    public static Rule Always(string success) => new Rule('a', '*', 0, success);
    public string? Evaluate(Part part)
    {
        int partValue = part.Get(Category);
        bool isSuccess = Op switch
        {
            '<' => partValue < Value,
            '>' => partValue > Value,
            _ => true,
        };
        return isSuccess ? Success : null;
    }

    public (Range Match, Range NoMatch) SplitIntoRanges(Part min, Part max)
        => this.Op switch
        {
            '<' => (new Range(min, max.With(Category, Value - 1)), new Range(min.With(Category, Value), max)),
            '>' => (new Range(min.With(Category, Value + 1), max), new Range(min, max.With(Category, Value))),
            _ => (new Range(min, max), new Range(Part.Zero, Part.Zero))
        };

}
