using System.Text.RegularExpressions;
const string InputFile = "input1.txt";
const int MaxTime = 30;
var valvesList = File.ReadLines(InputFile).Select(Valve.Parse).ToList();
var valves = valvesList.ToDictionary(v => v.Name);
var pathsCache = new Dictionary<(string from, int length, string open), Path>();

List<Node> nodes = new ();
foreach(var v in valves)

var bestPath = GetBestPath("AA", MaxTime, Array.Empty<string>());
ulong tp = bestPath.GetTotalPressure();
bestPath.PrintLog(Console.Out);
Console.WriteLine("Path: " + bestPath);



Node[,] graph = new Node[valves.Count * 2, valves.Count * 2];
int i = 0, j = 0;
foreach (var (k, vFrom) in valves)
{
    foreach (var (kk, vTo) in valves)
    {
        graph[i, j] = new Node(v.Name, 0, Action.Move)
        j+=2;
    }
    i += 2;
}

Path GetBestPath(string from, int maxTime, string[] openValves)
{
    if (maxTime <= 2 || openValves.Length == valves.Count)
    {
        return new Path(0, Array.Empty<Node>());
    }

    var pathKey = (from, maxTime, string.Concat(openValves));
    if (pathsCache.TryGetValue(pathKey, out Path path))
        return path;

    var fromValve = valves[from];
    var fromOff = new[] { new Node(from, 0, Action.Move) };
    var paths = fromValve.ConnectedValves
                .Select(node => GetBestPath(node, maxTime - 1, openValves))
                .Select(p => new Path(maxTime, fromOff, p.Nodes));

    if (!openValves.Contains(from) && fromValve.Rate > 0)
    {
        var openValvesWithOpenFrom = openValves.Union(new[] { from }).ToArray();
        Array.Sort(openValvesWithOpenFrom);
        var fromOpen = new[] { new Node(from, 0, Action.Move), new Node(from, fromValve.Rate, Action.Open) };
        paths = paths.Union(fromValve.ConnectedValves.Select(node => GetBestPath(node, maxTime - 2, openValvesWithOpenFrom))
                .Select(p => new Path(maxTime, fromOpen, p.Nodes)));
    }

    Path bestPath = paths.MaxBy(p => p.GetTotalPressure());
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
}

enum Action { Move, Open };
record struct Node(string Valve, int Rate, Action Action);
record struct Path
{
    public int maxTime { get; init; }
    public Node[] Nodes { get; init; }

    public Path(int maxTime, Node[] nodes)
    {
        this.maxTime = maxTime;
        this.Nodes = nodes;
    }

    public Path(int maxTime, IEnumerable<Node> path1, IEnumerable<Node> path2)
    {
        this.maxTime = maxTime;
        Nodes = path1.Concat(path2).ToArray();
    }

    public ulong GetTotalPressure()
    {
        ulong total = 0;
        int timeLeft = maxTime;
        foreach (var node in Nodes.Skip(1))
        {
            switch (node.Action)
            {
                case Action.Move:
                    timeLeft--;
                    break;
                case Action.Open:
                    timeLeft--;
                    total += (ulong)(node.Rate * timeLeft);
                    break;
            }
        }
        return total;
    }
    public override string ToString() => GetTotalPressure() + " => " + string.Join(",", Nodes.Select(v => v.Valve + ':' + (v.Action == Action.Move ? 'M' : 'O')));

    public void PrintLog(TextWriter writer)
    {
        int minute = 0;
        List<(string, int)> openValves = new();
        var nodesIt = Nodes.ToList().GetEnumerator();
        nodesIt.MoveNext();
        while (minute < maxTime)
        {
            minute++;
            writer.WriteLine($"== Minute {minute} ==");
            if (openValves.Count == 0) writer.WriteLine("No valves are open.");
            else writer.WriteLine($"Valves {string.Join(',', openValves.Select(v => v.Item1))} open, releasing {openValves.Sum(v => v.Item2)} pressure.");
            if (nodesIt.MoveNext())
            {
                switch (nodesIt.Current.Action)
                {
                    case Action.Move:
                        writer.WriteLine($"You move to valve {nodesIt.Current.Valve}");
                        break;
                    case Action.Open:
                        openValves.Add((nodesIt.Current.Valve, nodesIt.Current.Rate));
                        openValves.Sort((v1, v2) => string.Compare(v1.Item1, v2.Item1));
                        writer.WriteLine($"You open valve {nodesIt.Current.Valve}");
                        break;
                }
            }
            Console.WriteLine();
        }
    }
}