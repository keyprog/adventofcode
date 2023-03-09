using System.Text.RegularExpressions;

const string Input = "input1.txt";
int maxTime = 24;
var blueprints = BluePrintParser.Parse(File.ReadLines(Input)).ToArray();

int total = 0;
foreach (var bp in blueprints)
{
    Console.WriteLine(bp);
    var factory = new Factory(bp);
    var root = new Node(new N(1, 0, 0, 0));
    root.AddFL(new FactoryLine() { Time = 0 });
    var dict = new Dictionary<N, Node> { { root.N, root } };
    BuildGraph(factory, maxTime, root, dict, 7);

    var max = dict.Values.MaxBy(v => v.GetTotalGeodes(maxTime));
    var quality = max.GetTotalGeodes(maxTime) * bp.Name;
    Console.WriteLine(quality);
    total += quality;
}
Console.WriteLine(total);
//Reverse(max).ToList().ForEach(n => Console.WriteLine(n.Print(maxTime)));



void Compute(Node n, Factory f, int maxTime)
{
    if (n.Completed)
        return;

    foreach (var prev in n.Prev)
    {
        if (!prev.Completed)
            throw new ApplicationException("out of order");
        //Compute(prev, f, maxTime);
        if (prev.NotReachable)
            continue;

        foreach (var prevFL in prev.FactoryLines)
        {
            var fl = f.Produce(prevFL, n.N.ToTuple().Subtract(prev.N.ToTuple()));

            if (fl.Time >= maxTime)
                continue;

            if (fl.Time == maxTime - 1 && n.N.Geode == 0)
                continue;

            n.AddFL(fl);
        }
    }
    n.Prev.Clear();
}

void BuildGraph(Factory f, int maxTime, Node root, Dictionary<N, Node> index, int max)
{
    var q = new Stack<Node>();
    q.Push(root);

    var nextNodes = new N?[4];

    while (q.TryPop(out Node node))
    {
        //index.Remove(node.N);
        if (!node.Completed)
            Compute(node, f, maxTime);

        if (node.NotReachable)
        {
            continue;
        }

        var p = node.N;
        nextNodes[0] = f.EnoughOre(p) ? null : p with { Ore = p.Ore + 1 };
        nextNodes[1] = (p.Ore == 0 || f.EnoughClay(p)) ? null : p with { Clay = p.Clay + 1 };
        nextNodes[2] = (p.Clay == 0 || f.EnoughObs(p)) ? null : p with { Obsidian = p.Obsidian + 1 };
        nextNodes[3] = (p.Obsidian == 0 || p.Geode > max) ? null : p with { Geode = p.Geode + 1 };

        foreach (N? n1 in nextNodes)
        {
            if (!n1.HasValue)
                continue;
            var nextNode = n1.Value;

            //if (n.Ore <= max && n.Clay <= max && n.Obsidian <= max && n.Geode <= max)
            {

                if (!index.TryGetValue(nextNode, out Node? nn))
                {
                    nn = new Node(nextNode);
                    index[nextNode] = nn;
                }
                //node.AddNext(nn);
                nn.AddPrev(node);
                q.Enqueue(nn);
            }
        }
    }


}

void BuildGraph2(Node root, (int Ore, int Clay, int Obsidian, int Geode) max, Dictionary<N, Node> index)
{
    var q = new Queue<Node>();
    q.Enqueue(root);

    var next = new N[4];

    while (q.TryDequeue(out Node node))
    {
        var p = node.N;
        next[0] = p with { Ore = p.Ore + 1 };
        next[1] = p with { Clay = p.Clay + 1 };
        next[2] = p with { Obsidian = p.Obsidian + 1 };
        next[3] = p with { Geode = p.Geode + 1 };

        foreach (var n in next)
        {
            if (n.Ore <= max.Ore && n.Clay <= max.Clay && n.Obsidian <= max.Obsidian && n.Geode <= max.Geode)
            {
                if (!index.TryGetValue(n, out Node? nn))
                {
                    nn = new Node(n);
                    index[n] = nn;
                }
                //node.AddNext(nn);
                nn.AddPrev(node);
                q.Enqueue(nn);
            }
        }
    }
}

record struct N(int Ore, int Clay, int Obsidian, int Geode)
{
    public (int, int, int, int) ToTuple() => (Ore, Clay, Obsidian, Geode);
    public static string Diff(N prev, N next)
    {
        if (next.Ore - prev.Ore == 1) return "ore";
        if (next.Clay - prev.Clay == 1) return "clay";
        if (next.Obsidian - prev.Obsidian == 1) return "obsidian";
        if (next.Geode - prev.Geode == 1) return "geode";
        throw new ArgumentException();
    }

    public IEnumerable<N> ReachGeode()
    {
        var cur = this;
        if (cur.Ore == 0)
        {
            cur = cur with { Ore = 1 };
            yield return cur;
        }
        if (cur.Clay == 0)
        {
            cur = cur with { Clay = 1 };
            yield return cur;
        }
        if (cur.Obsidian == 0)
        {
            cur = cur with { Obsidian = 1 };
            yield return cur;
        }
        if (cur.Geode == 0)
        {
            yield return cur with { Geode = 1 };
        }
    }
}
record class Node(N N, FactoryLine FactoryLine)
{
    //private FactoryLine factoryLines;
    //public IEnumerable<FactoryLine> FactoryLines { get { return factoryLines == null ? Array.Empty<FactoryLine>() : factoryLines; } }


    static readonly (Func<FactoryLine, (int, int, int)>, Func<FactoryLine, int>)[] dupConditions =
        {
            ((cfl) => (cfl.MatClay, cfl.MatObsidian, cfl.MatGeode), (cfl) => cfl.MatOre),
            ((cfl) => (cfl.MatOre, cfl.MatObsidian, cfl.MatGeode), (cfl) => cfl.MatClay),
            ((cfl) => (cfl.MatOre, cfl.MatClay, cfl.MatGeode), (cfl) => cfl.MatObsidian),
            ((cfl) => (cfl.MatOre, cfl.MatClay, cfl.MatObsidian), (cfl) => cfl.MatGeode),

        };
    /*public void AddFL(FactoryLine fl)
    {
        if (factoryLines == null)
            factoryLines = new();

        var ff = factoryLines.Where(f => f.Time == fl.Time).ToArray();

        foreach (var (filter, selector) in dupConditions)
        {
            var dups = ff.Where(f => filter(f) == filter(fl));
            if (dups.Any())
            {
                var d = dups.First();
                if (selector(fl) > selector(d))
                {
                    factoryLines.Remove(d);
                    factoryLines.Add(fl);
                }
                return;
            }

            if (factoryLines.Count > 0 && factoryLines.Count % 1000 == 0)
            {
                Console.WriteLine(factoryLines.Count);

                var grouped = factoryLines.GroupBy(ff => (ff.Time, ff.MatClay, ff.MatGeode, ff.MatObsidian)).ToArray();
                var gg = grouped.Where(g => g.Count() > 1).ToArray();
                Console.WriteLine(grouped.Length);
            }
        }

        factoryLines.Add(fl);
    }*/
    //public List<Node> Next { get; } = new();

    /*public void AddNext(Node node)
    {
        if (!Next.Contains(node))
            Next.Add(node);
    }*/
    //public List<Node> Prev { get; } = new();
    /*public void AddPrev(Node node)
    {
        if (!Prev.Contains(node))
            Prev.Add(node);
    }*/

    //public bool Completed { get { return Prev.Count == 0; } }
    ///public bool NotReachable { get { return !FactoryLines.Any(); } }

    //public Node? PrevMin { get; set; }

    public int GetTotalGeodes(int maxTime) =>  FactoryLine.MatGeode + (N.Geode * (maxTime - FactoryLine.Time));

    public string Print(int maxTime)
    {
        return this.ToString() + " geodes: " + GetTotalGeodes(maxTime);
    }

    public override string ToString()
    {
        return string.Join(", ", FactoryLines.Select(fl => fl + " time: " + fl.Time));
    }
}

record class Factory
{
    public Blueprint Blueprint { get; init; }
    private (int ore, int clay, int obsidian, int _) maxCost;
    public Factory(Blueprint blueprint)
    {
        this.Blueprint = blueprint;
        maxCost = blueprint.MaxCost();
    }

    public bool EnoughOre(N robots) => robots.Ore >= maxCost.ore;

    public bool EnoughClay(N robots) => robots.Clay >= maxCost.clay;

    public bool EnoughObs(N robots) => robots.Obsidian >= maxCost.obsidian;

    public FactoryLine Produce(FactoryLine f, (int, int, int, int) create)
    {
        if (f.Time == int.MaxValue)
            throw new ArgumentException();
        var cost = Blueprint.GetByMask(create);
        var mineralsRequired = cost.Subtract(f.Minerals);
        var time = mineralsRequired.Do(f.Robots, (m, r) => m == 0 ? 0 : r == 0 ? int.MaxValue : (int)Math.Ceiling((float)m / (float)r));
        int timeRequired = time.Max();
        if (timeRequired == int.MaxValue)
            return f with { Time = int.MaxValue };

        timeRequired++; // including robot build itself
        f = f with
        {
            MatOre = f.MatOre + f.RobotOre * timeRequired,
            MatClay = f.MatClay + f.RobotClay * timeRequired,
            MatObsidian = f.MatObsidian + f.RobotObsidian * timeRequired,
            MatGeode = f.MatGeode + f.RobotGeode * timeRequired,
            Robots = f.Robots.Add(create),
            Time = f.Time + timeRequired
        };
        return f;
    }

}

record struct FactoryLine
{
    public FactoryLine()
    {
        RobotOre = 1;
    }
    public int RobotOre { get; set; }
    public int RobotClay { get; set; }
    public int RobotObsidian { get; set; }
    public int RobotGeode { get; set; }
    public int MatOre { get; set; }
    public int MatClay { get; set; }
    public int MatObsidian { get; set; }
    public int MatGeode { get; set; }
    public int Time { get; set; } = int.MaxValue;

    public (int, int, int, int) Minerals
    {
        get => (MatOre, MatClay, MatObsidian, MatGeode);
        set
        {
            (MatOre, MatClay, MatObsidian, MatGeode) = value;
        }
    }
    public (int, int, int, int) Robots
    {
        get => (RobotOre, RobotClay, RobotObsidian, RobotGeode);
        set
        {
            (RobotOre, RobotClay, RobotObsidian, RobotGeode) = value;
        }
    }
}

static class Extensions
{
    public static (int, int, int, int) ToTuple(this Dictionary<string, int> dict)
    {
        return (
            dict.GetValueOrDefault("ore", 0),
            dict.GetValueOrDefault("clay", 0),
            dict.GetValueOrDefault("obsidian", 0),
            dict.GetValueOrDefault("geode", 0)
        );
    }

    public static (int, int, int, int) Subtract(this (int, int, int, int) v1, (int, int, int, int) v2)
    {
        return (v1.Item1 - v2.Item1, v1.Item2 - v2.Item2, v1.Item3 - v2.Item3, v1.Item4 - v2.Item4);
    }

    public static (int, int, int, int) Add(this (int, int, int, int) v1, (int, int, int, int) v2)
    {
        return (v1.Item1 + v2.Item1, v1.Item2 + v2.Item2, v1.Item3 + v2.Item3, v1.Item4 + v2.Item4);
    }

    public static (int, int, int, int) Do(this (int, int, int, int) v, Func<int, int> func)
    {
        return (func(v.Item1), func(v.Item2), func(v.Item3), func(v.Item4));
    }


    public static (int, int, int, int) Do(this (int, int, int, int) v1, (int, int, int, int) v2, Func<int, int, int> func)
    {
        return (func(v1.Item1, v2.Item1), func(v1.Item2, v2.Item2), func(v1.Item3, v2.Item3), func(v1.Item4, v2.Item4));
    }



    public static Dictionary<string, int> ToDict(this (int, int, int, int) v)
    {
        var dict = new Dictionary<string, int>(4);
        dict["ore"] = v.Item1;
        dict["clay"] = v.Item2;
        dict["obsidian"] = v.Item3;
        dict["geode"] = v.Item4;
        return dict;
    }

    public static int Max(this (int, int, int, int) v)
    {
        int max = v.Item1;
        if (v.Item2 > max)
            max = v.Item2;
        if (v.Item3 > max)
            max = v.Item3;
        if (v.Item4 > max)
            max = v.Item4;
        return max;
    }

    public static (int, int, int, int) Set(this (int, int, int, int) v, string name, int value)
    {
        return name switch
        {
            "ore" => v with { Item1 = value },
            "clay" => v with { Item2 = value },
            "obsidian" => v with { Item3 = value },
            "geode" => v with { Item4 = value },
            _ => throw new NotSupportedException()
        };
    }

}
record class Blueprint
{
    public Blueprint(int name, Dictionary<string, Dictionary<string, int>> input)
    {
        Name = name;
        OreRobot = input["ore"].ToTuple();
        ClayRobot = input["clay"].ToTuple();
        ObsidianRobot = input["obsidian"].ToTuple();
        GeodeRobot = input["geode"].ToTuple();
    }

    public int Name { get; init; }
    public (int, int, int, int) OreRobot { get; init; }
    public (int, int, int, int) ClayRobot { get; set; }
    public (int, int, int, int) ObsidianRobot { get; set; }
    public (int, int, int, int) GeodeRobot { get; set; }

    public (int Ore, int Clay, int Obsidian, int Geode) GetByName(string name)
    {
        return name switch
        {
            "ore" => OreRobot,
            "clay" => ClayRobot,
            "obsidian" => ObsidianRobot,
            "geode" => GeodeRobot,
            _ => throw new NotSupportedException()
        };
    }
    public (int Ore, int Clay, int Obsidian, int Geode) GetByMask((int, int, int, int) mask)
    {
        return mask switch
        {
            (1, 0, 0, 0) => OreRobot,
            (0, 1, 0, 0) => ClayRobot,
            (0, 0, 1, 0) => ObsidianRobot,
            (0, 0, 0, 1) => GeodeRobot,
            _ => throw new NotSupportedException()
        };
    }

    public IEnumerable<(int, int, int, int)> GetRobots() => new[] { OreRobot, ClayRobot, ObsidianRobot, GeodeRobot };

    public (int, int, int, int) MaxCost()
    {
        return (
                (OreRobot.Item1, ClayRobot.Item1, ObsidianRobot.Item1, GeodeRobot.Item1).Max(),
                (OreRobot.Item2, ClayRobot.Item2, ObsidianRobot.Item2, GeodeRobot.Item2).Max(),
                (OreRobot.Item3, ClayRobot.Item3, ObsidianRobot.Item3, GeodeRobot.Item3).Max(),
                (OreRobot.Item4, ClayRobot.Item4, ObsidianRobot.Item4, GeodeRobot.Item4).Max()
        );
    }
}

class BluePrintParser
{
    private const string BlueprintToken = "Blueprint ";
    private const string RobotCostPattern = "(\\s)*Each (?<name>[a-z]+) robot costs (?<cost>.*)";
    private static readonly Regex RobotCostRx = new Regex(RobotCostPattern, RegexOptions.Compiled);

    public static IEnumerable<Blueprint> Parse(IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            int name = 0;
            var dict = new Dictionary<string, Dictionary<string, int>>(4);
            foreach (var l in line.Split(new char[] { ':', '.' }))
            {
                switch (l)
                {
                    case var ll when ll.StartsWith(BlueprintToken):
                        name = int.Parse(l.AsSpan()[BlueprintToken.Length..]);
                        break;
                    case "": break;
                    default:
                        var (robotName, minerals) = RobotCostRx.Matches(l).Select(m => (m.Groups["name"].Value, m.Groups["cost"].Value)).Single();
                        dict[robotName] = minerals.Split(" and ")
                                                .Select(token => token.Split(' '))
                                                .ToDictionary(cost2Mineral => cost2Mineral[1], v => int.Parse(v[0]));

                        break;
                }
            }
            yield return new Blueprint(name, dict);
        }
    }
}