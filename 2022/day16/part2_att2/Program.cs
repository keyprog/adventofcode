using System.Text.RegularExpressions;
const string InputFile = "input2.txt";
ulong pathsChecked = 0;
const int MaxTime = 26;
var valvesMap = File.ReadLines(InputFile).Select(Valve.Parse).ToDictionary(v => v.Name);
var allValves = valvesMap.Values.ToArray();

var allActiveValves = valvesMap.Values.Where(v => v.Rate > 0 || v.Name == "AA").ToArray();
var index = allActiveValves.Select((k, i) => (k, i)).ToDictionary(kv => kv.k.Name, kv => kv.i);

int[,] shortestPaths = new int[allActiveValves.Length, allActiveValves.Length];
for (int i = 0; i < allActiveValves.Length; ++i)
    for (int j = i + 1; j < allActiveValves.Length; ++j)
    {
        int distance = GetDistance(allActiveValves[i], allActiveValves[j].Name);
        shortestPaths[i, j] = distance;
        shortestPaths[j, i] = distance;
        //  Console.WriteLine($"{allActiveValves[i].Name}=>{allActiveValves[j].Name}: {distance}");
    }

//int pressure = GetBest("AA", allActiveValves.Select(v => v.Name).Where(v => v != "AA").ToArray(), MaxTime);
var cache = new Dictionary<string, int>();
//var cache2 = 
int pressure = GetBestFor2("AA", new string[0], allActiveValves.Select(v => v.Name).Where(v => v != "AA").ToArray(), MaxTime);


int GetBestFor2(string from, string[] valves1, string[] valves2, int time)
{
    if (valves1.Length > valves2.Length)
        return 0;

    int maxPressure = GetBest(from, valves1, time) + GetBest(from, valves2, time);
    foreach (var v in valves2)
    {
        if (valves1.Length == 0)
        {
            pathsChecked++;
            Console.WriteLine($"Checking {pathsChecked}/{valves2.Length}");
        }
        var arrV = new[] { v };
        int pressure = GetBestFor2(from, valves1.Concat(arrV).Order().ToArray(), valves2.Except(arrV).Order().ToArray(), time);
        if (pressure > maxPressure)
            maxPressure = pressure;
    }
    return maxPressure;
}

int GetBest(string from, string[] valves, int time)
{
    int maxPressure = 0;
    if (valves.Length == 0 || time <= 0)
        return 0;

    string key = string.Concat(from, time, string.Concat(valves));
    if (cache.TryGetValue(key, out int mp))
        return mp;
    foreach (string v in valves)
    {
        var timeLeft = time - shortestPaths[index[from], index[v]];
        int pressure = timeLeft * valvesMap[v].Rate + GetBest(v, valves.Where(vv => vv != v).ToArray(), timeLeft);
        if (pressure > maxPressure)
            maxPressure = pressure;
    }
    cache[key] = maxPressure;
    return maxPressure;
}

Console.WriteLine(pressure);
return;

int GetDistance(Valve from, string to)
{
    Queue<(string name, int distance)> next = new();
    foreach (var c in from.ConnectedValves)
        next.Enqueue((c, 1));
    while (next.TryDequeue(out var node))
    {
        if (node.name == to)
            return node.distance + 1;
        foreach (var c in valvesMap![node.name].ConnectedValves)
            next.Enqueue((c, node.distance + 1));
    }
    return -1;
}

var pathsCache = new Dictionary<((string, string, string), int length), Path>();
var emptyPathCache = new Dictionary<(string, string, string), int>();

//var effValves = valves.Values.Where(v => v.Rate > 0).Count();

var startNode = new Node("AA", 0, Action.Move);

//var bestPath = GetBestPath(new NodePair(startNode, startNode), MaxTime + 1, new OpenValvesTracker());
//bestPath = new Path(MaxTime, bestPath.Nodes[1..]);
//bestPath.PrintLog(Console.Out);
//Console.WriteLine("Path: " + bestPath);

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

    var yFromValve = valvesMap[from.yNode.Valve];
    var eFromValve = valvesMap[from.eNode.Valve];
    openValves = openValves.With(from);

    List<Path> paths = new();
    foreach (var yTo in yFromValve.GetNextNodes(openValves, valvesMap))
    {
        var ov = openValves.With(yTo);
        foreach (var eTo in eFromValve.GetNextNodes(ov, valvesMap))
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