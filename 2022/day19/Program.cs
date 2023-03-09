using System.Text.RegularExpressions;

const string Input = "input2.txt";
int maxTime = 32;
var blueprints = BluePrintParser.Parse(File.ReadLines(Input)).ToArray();

int total = 1;
int totalQuality = 0;
foreach (var bp in blueprints.Take(3))
{
    Console.WriteLine(bp);
    var factory = new Factory(bp);
    var root = new FactoryLine() { Time = 0 };
    int max = BuildGraph(factory, maxTime, root);

    Console.WriteLine(max);
    total *= max;
    totalQuality += max * bp.Name;
}
Console.WriteLine($"Quality: {totalQuality}, Part2: {total} ");

int BuildGraph(Factory f, int maxTime, FactoryLine top)
{
    int maxGeodes = 0;
    int nodesChecked = 0;
    var q = new Stack<FactoryLine>();
    q.Push(top);

    var dict = new HashSet<(int, N, N)>();

    var nextNodes = new N?[4];

    while (q.TryPop(out FactoryLine node))
    {
        nodesChecked++;
        if (nodesChecked % 10000000 == 0)
        {
            Console.WriteLine($"Checked: {nodesChecked}, max geodes: {maxGeodes}");
        }

        var p = node.Robots;
        nextNodes[0] = f.EnoughOre(p) ? null : p with { Ore = p.Ore + 1 };
        nextNodes[1] = (p.Ore == 0 || f.EnoughClay(p)) ? null : p with { Clay = p.Clay + 1 };
        nextNodes[2] = (p.Clay == 0 || f.EnoughObs(p)) ? null : p with { Obsidian = p.Obsidian + 1 };
        nextNodes[3] = (p.Obsidian == 0) ? null : p with { Geode = p.Geode + 1 };

        foreach (N? n1 in nextNodes)
        {
            if (!n1.HasValue)
                continue;
            var nextNode = n1.Value;
            var fl = f.Produce(node, nextNode.Subtract(p));
            if (fl.Time > maxTime)
                continue;

            if (fl.Time < maxTime - 2)
            {
                var key = (fl.Time, fl.Robots, fl.Minerals);

                if (dict.Contains(key))
                    continue;
                dict.Add(key);
            }

            int totalGeodes = fl.GetTotalGeodes(maxTime);
            if (totalGeodes > maxGeodes)
            {
                maxGeodes = totalGeodes;
                if (maxGeodes > 60)
                    Console.WriteLine(fl.GetTotalGeodes(maxTime) + " " + fl);
            }
            if (fl.Time < maxTime - 1)
                q.Push(fl);
        }
    }
    return maxGeodes;
}


record struct N(int Ore, int Clay, int Obsidian, int Geode);

record class Factory
{
    public Blueprint Blueprint { get; init; }
    private N maxCost;
    public Factory(Blueprint blueprint)
    {
        this.Blueprint = blueprint;
        maxCost = blueprint.MaxCost();
    }

    public bool EnoughOre(N robots) => robots.Ore >= maxCost.Ore;

    public bool EnoughClay(N robots) => robots.Clay >= maxCost.Clay;

    public bool EnoughObs(N robots) => robots.Obsidian >= maxCost.Obsidian;

    public FactoryLine Produce(FactoryLine f, N create)
    {
        if (f.Time == int.MaxValue || f.Time < 0)
            throw new ArgumentException();
        var cost = Blueprint.GetByMask(create);
        var mineralsRequired = cost.Subtract(f.Minerals);
        var time = mineralsRequired.Do(f.Robots, (m, r) => m == 0 ? 0 : r == 0 ? throw new ArgumentException() : (int)Math.Ceiling((double)m / (double)r));
        int timeRequired = time.Max();
        if (timeRequired < 0)
            timeRequired = 0;
        if (timeRequired == int.MaxValue)
            return f with { Time = int.MaxValue };

        timeRequired++; // including robot build itself
        f = f with
        {
            Robots = f.Robots.Add(create),
            Minerals = f.Minerals.Add(f.Robots.Do(r => r * timeRequired)).Subtract(cost),
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

    public N Minerals
    {
        get => new N(MatOre, MatClay, MatObsidian, MatGeode);
        set => (MatOre, MatClay, MatObsidian, MatGeode) = value;

    }
    public N Robots
    {
        get => new N(RobotOre, RobotClay, RobotObsidian, RobotGeode);
        set => (RobotOre, RobotClay, RobotObsidian, RobotGeode) = value;
    }

    public int GetTotalGeodes(int maxTime) => MatGeode + (RobotGeode * (maxTime - Time));
}

static class Extensions
{
    public static N ToTuple(this Dictionary<string, int> dict)
    {
        return new N(
            dict.GetValueOrDefault("ore", 0),
            dict.GetValueOrDefault("clay", 0),
            dict.GetValueOrDefault("obsidian", 0),
            dict.GetValueOrDefault("geode", 0)
        );
    }

    public static N Subtract(this N v1, N v2)
    {
        return new N(v1.Ore - v2.Ore, v1.Clay - v2.Clay, v1.Obsidian - v2.Obsidian, v1.Geode - v2.Geode);
    }

    public static N Add(this N v1, N v2)
    {
        return new N(v1.Ore + v2.Ore, v1.Clay + v2.Clay, v1.Obsidian + v2.Obsidian, v1.Geode + v2.Geode);
    }

    public static N Do(this N v, Func<int, int> func)
    {
        return new N(func(v.Ore), func(v.Clay), func(v.Obsidian), func(v.Geode));
    }


    public static N Do(this N v1, N v2, Func<int, int, int> func)
    {
        return new N(func(v1.Ore, v2.Ore), func(v1.Clay, v2.Clay), func(v1.Obsidian, v2.Obsidian), func(v1.Geode, v2.Geode));
    }

    public static int Max(this N v)
    {
        int max = v.Ore;
        if (v.Clay > max) max = v.Clay;
        if (v.Obsidian > max) max = v.Obsidian;
        if (v.Geode > max) max = v.Geode;
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
    public N OreRobot { get; init; }
    public N ClayRobot { get; set; }
    public N ObsidianRobot { get; set; }
    public N GeodeRobot { get; set; }

    public N GetByName(string name)
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
    public N GetByMask(N mask)
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


    public N MaxCost()
    {
        return new N(
            new N(OreRobot.Ore, ClayRobot.Ore, ObsidianRobot.Ore, GeodeRobot.Ore).Max(),
            new N(OreRobot.Clay, ClayRobot.Clay, ObsidianRobot.Clay, GeodeRobot.Clay).Max(),
            new N(OreRobot.Obsidian, ClayRobot.Obsidian, ObsidianRobot.Obsidian, GeodeRobot.Obsidian).Max(),
            new N(OreRobot.Geode, ClayRobot.Geode, ObsidianRobot.Geode, GeodeRobot.Geode).Max()
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