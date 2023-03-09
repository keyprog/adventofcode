using System.Text.RegularExpressions;
const string InputFile = "input2.txt";
const int MaxTime = 26;
var valves = File.ReadLines(InputFile).Select(Valve.Parse).ToDictionary(v => v.Name);

var pathsCache = new Dictionary<((string, string, string), int length), Path>();
var emptyPathCache = new Dictionary<(string, string, string), int>();

var effValves = valves.Values.Where(v => v.Rate > 0).Count();

var startNode = new Node("AA", 0, Action.Move);

var bestPath = GetBestPath(new NodePair(startNode, startNode), MaxTime + 1, new OpenValvesTracker());
bestPath = new Path(MaxTime, bestPath.Nodes[1..]);
bestPath.PrintLog(Console.Out);
Console.WriteLine("Path: " + bestPath);

Path GetBestPath(NodePair from, int maxTime, OpenValvesTracker openValves)
{
    if (maxTime == 0)
        return Path.Empty;

    //if (openValves.Count == effValves) // all needed valves already open
    //  return Path.Empty;
    //if (maxTime < (10 - openValves.Count))
    //  return Path.Empty;

    var key = (from.yNode.Valve, from.eNode.Valve, openValves.Key);
    if (emptyPathCache.TryGetValue(key, out int emptyLengh))
    {
        if (emptyLengh >= maxTime)
            return Path.Empty;
    }

    var pathKey = (key, maxTime);
    if (pathsCache.TryGetValue(pathKey, out Path path))
        return path;

    var yFromValve = valves[from.yNode.Valve];
    var eFromValve = valves[from.eNode.Valve];
    openValves = openValves.With(from);

    List<Path> paths = new();
    foreach (var yTo in yFromValve.GetNextNodes(openValves, valves))
    {
        var ov = openValves.With(yTo);
        foreach (var eTo in eFromValve.GetNextNodes(ov, valves))
        {
            paths.Add(new Path(maxTime, from, GetBestPath(new NodePair(yTo, eTo), maxTime - 1, openValves).Nodes));
        }
    }

    Path bestPath = paths.MaxBy(p => p.GetTotalPressure());
    if (bestPath.GetTotalPressure() == 0)
    {
        emptyPathCache[key] = maxTime;
        return Path.Empty;
    }

    //if (maxTime > 10)
        pathsCache[pathKey] = bestPath;

    return bestPath;
}

record class Valve(string Name, int Rate, string[] ConnectedValves)
{
    const string pattern = "Valve (?'Name'[A-Z]+) has flow rate=(?'Rate'[0-9]+); tunnel[s]* lead[s]* to valve[s]* (?'Connected'.*)";
    static readonly Regex rx = new Regex(pattern);
    public static Valve Parse(string line)
    {
        Match m = rx.Match(line);
        if (!m.Success) throw new ApplicationException("Unable to parse input");
        return new Valve(m.Groups["Name"].Value, int.Parse(m.Groups["Rate"].ValueSpan), m.Groups["Connected"].Value.Split(", "));
    }

    public IEnumerable<Node> GetNextNodes(OpenValvesTracker tracker, Dictionary<string, Valve> valves)
    {
        if (Rate > 0 && !tracker.Contains(Name) && Rate > 0)
            yield return new Node(Name, Rate, Action.Open);

        foreach (var cn in ConnectedValves)
            yield return new Node(cn, 0, Action.Move);
    }

    public override string ToString() => $"{Name} Rate={Rate} => {string.Join(", ", ConnectedValves)}";
}

enum Action { Move, Open };

record struct NodePair(Node yNode, Node eNode);
record struct Node(string Valve, int Rate, Action Action);
record struct Path
{
    public static Path Empty { get; } = new Path(0, Array.Empty<NodePair>());
    public int MaxTime { get; init; }
    public NodePair[] Nodes { get; init; }
    public Path(int maxTime, NodePair[] nodes)
    {
        MaxTime = maxTime;
        Nodes = nodes;
    }
    public Path(int maxTime, NodePair node1, NodePair[] path)
    {
        MaxTime = maxTime;
        Nodes = new NodePair[path.Length + 1];
        Nodes[0] = node1;
        path.CopyTo(Nodes, 1);
    }
    public Path(int maxTime, NodePair node1, NodePair node2, NodePair[] path)
    {
        MaxTime = maxTime;
        Nodes = new NodePair[path.Length + 2];
        Nodes[0] = node1;
        Nodes[1] = node2;
        path.CopyTo(Nodes, 2);
    }
    public ulong GetTotalPressure()
    {
        ulong total = 0;
        int timeLeft = MaxTime;

        foreach (var node in Nodes)
        {
            timeLeft--;
            if (node.yNode.Action == Action.Open)
                total += (ulong)(node.yNode.Rate * timeLeft);
            if (node.eNode.Action == Action.Open)
                total += (ulong)(node.eNode.Rate * timeLeft);
        }

        return total;
    }
    public override string ToString() => GetTotalPressure() + " => " +
        string.Join(Environment.NewLine, Nodes.Select(v => $"Y:{v.yNode.Valve}:{(v.yNode.Action == Action.Move ? 'M' : 'O')} E:{v.eNode.Valve}:{(v.eNode.Action == Action.Move ? 'M' : 'O')}"));

    public void PrintLog(TextWriter writer)
    {
        int minute = 0;
        List<(string, int)> openValves = new();
        var nodesIt = Nodes.ToList().GetEnumerator();
        nodesIt.MoveNext();
        while (minute < MaxTime)
        {
            minute++;
            writer.WriteLine($"== Minute {minute} ==");
            if (openValves.Count == 0) writer.WriteLine("No valves are open.");
            else writer.WriteLine($"Valves {string.Join(',', openValves.Select(v => v.Item1))} open, releasing {openValves.Sum(v => v.Item2)} pressure.");
            if (nodesIt.MoveNext())
            {
                PrintAction(writer, openValves, "You", nodesIt.Current.yNode);
                PrintAction(writer, openValves, "The elephant", nodesIt.Current.eNode);
            }
            Console.WriteLine();
        }
    }

    private static void PrintAction(TextWriter writer, List<(string, int)> openValves, string who, Node node)
    {
        switch (node.Action)
        {
            case Action.Move:
                writer.WriteLine($"{who} move to valve {node.Valve}");
                break;
            case Action.Open:
                openValves.Add((node.Valve, node.Rate));
                openValves.Sort((v1, v2) => string.Compare(v1.Item1, v2.Item1));
                writer.WriteLine($"{who} open valve {node.Valve}");
                break;
        }
    }
}

record struct OpenValvesTracker
{
    private readonly string openValvesSet;

    public OpenValvesTracker()
    {
        openValvesSet = string.Empty;
    }

    public OpenValvesTracker(string openValves, string? valve = null)
    {
        if (valve == null)
        {
            openValvesSet = openValves;
            return;
        }

        if (openValves.Length == 0)
        {
            openValvesSet = valve;
            return;
        }
        Span<char> newSet = stackalloc char[openValves.Length + 3]; //separator and value
        ReadOnlySpan<char> oldSet = openValves.AsSpan();
        int insertionIndex = oldSet.Length + 1;
        for (int i = 0; i < oldSet.Length; i += 3)
        {
            if (oldSet[i] > valve[0] || (oldSet[i] == valve[0] && oldSet[i + 1] > valve[1]))
            {
                insertionIndex = i;
                break;
            }
        }
        valve.CopyTo(newSet[insertionIndex..]);
        if (insertionIndex > 0)
            oldSet[0..(insertionIndex - 1)].CopyTo(newSet[0..]);
        if (insertionIndex < oldSet.Length)
            oldSet[insertionIndex..].CopyTo(newSet[(insertionIndex + 3)..]);

        if (insertionIndex == oldSet.Length + 1) newSet[insertionIndex - 1] = ' '; else newSet[insertionIndex + 2] = ' ';
        openValvesSet = String.Intern(new string(newSet));
        //Console.WriteLine(openValvesSet);
    }

    public int Count { get { return openValvesSet.Length == 0 ? 0 : openValvesSet.Length / 3 + 1; } }

    public bool Contains(string valve)
    {
        bool c = openValvesSet.Contains(valve);
        return c;
    }

    public OpenValvesTracker With(Node node)
    {
        return node.Action switch
        {
            Action.Move => this,
            Action.Open => new OpenValvesTracker(openValvesSet, node.Valve),
            _ => throw new ArgumentException()
        };
    }

    public OpenValvesTracker With(NodePair node)
    {
        return (node.yNode.Action, node.eNode.Action) switch
        {
            (Action.Move, Action.Move) => this,
            (Action.Open, Action.Move) => this.With(node.yNode),
            (Action.Move, Action.Open) => this.With(node.eNode),
            (Action.Open, Action.Open) => this.With(node.yNode).With(node.eNode),
            _ => throw new ArgumentException()
        };
    }

    public string Key { get { return openValvesSet; } }
}