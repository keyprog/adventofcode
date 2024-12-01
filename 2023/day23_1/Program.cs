var map = new Map(File.ReadAllLines("input.txt"));

P start = new P(0, 1);
P dest = new(map.Rows - 1, map.Data[^1].IndexOf('.'));
HashSet<Node> visited = [];
Node graph = map.BuildGraph(start);
var edges = new List<Graph.Edge>();
var nodes = new List<Graph.Node>();
int destDistance = 0;
HashSet<(P, P)> visited2 = [];
calcPath(graph);
visited.Clear();
printGraph(graph, nodes, edges);
Console.WriteLine(Graph.ToJson(nodes, edges));

destDistance.Print();

void calcPath(Node parent, int distance = 0)
{
    if (parent.Name == dest && distance > destDistance)
    {
        Console.WriteLine(distance);
        destDistance = distance;
    }
    if (!visited.Add(parent))
        return;
    foreach (var (n, d) in parent.Next)
    {
        calcPath(n, distance + d);
    }
    visited.Remove(parent);
}

void printGraph(Node parent, List<Graph.Node> nodes, List<Graph.Edge> edges, int indent = 0)
{
    if (!visited.Add(parent))
        return;
    nodes.Add(new Graph.Node(parent.Name.ToString()));
    foreach (var (n, d) in parent.Next)
    {
        edges.Add(new(parent.Name.ToString(), n.Name.ToString(), d.ToString()));
        printGraph(n, nodes, edges, indent + 1);
    }
}

class Map(string[] lines)
{
    public int Rows = lines.Length;
    public int Cols = lines[0].Length;

    public string[] Data = lines;

    public void Print(P p)
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine();
        for (int r = 0; r < Rows; ++r)
        {
            for (int c = 0; c < Cols; ++c)
            {
                Console.Write(r == p.Row && c == p.Col ? 'O' : Data[r][c]);
            }
            Console.WriteLine();
        }
    }

    public static P[] GetAllDir(P f) => [new(f.Row - 1, f.Col), new(f.Row + 1, f.Col), new(f.Row, f.Col - 1), new(f.Row, f.Col + 1)];
    public Node BuildGraph(P start)
    {
        Dictionary<P, Node> nodesLookup = [];
        HashSet<(P, P)> visited = [];

        Node root = new Node(start);
        nodesLookup[start] = root;
        BuildGraphInternal(root, start, new P(0, 0), visited, nodesLookup);
        return root;
    }

    private void BuildGraphInternal(Node parent, P from, P from2, HashSet<(P, P)> visited, Dictionary<P, Node> nodeLookup)
    {
        if (!visited.Add((from, from2)))
            return;

        P[] directions = GetDirections(from, from2).Where(p => !visited.Contains((from, p))).ToArray();
        switch (directions.Length)
        {
            case 0:
                break;
            case 1:
                {
                    var end = BuildStraightPath(directions[0], from, 1, visited);

                    if (!nodeLookup.TryGetValue(end.From, out var node))
                    {
                        node = new Node(end.From);
                        nodeLookup.Add(end.From, node);
                    }
                    parent.AddEdgeTo(node, end.Distance);
                    node.AddEdgeTo(parent, end.Distance);
                    BuildGraphInternal(node, end.From, end.From2, visited, nodeLookup);
                }
                break;
            default:
                foreach (var f in directions)
                {

                    var end = BuildStraightPath(f, from, 1, visited);
                    if (!nodeLookup.TryGetValue(end.From, out var node))
                    {
                        node = new Node(end.From);
                        nodeLookup.Add(end.From, node);
                    }
                    parent.AddEdgeTo(node, end.Distance);
                    node.AddEdgeTo(parent, end.Distance);
                    BuildGraphInternal(node, end.From, end.From2, visited, nodeLookup);
                }
                break;
        }
    }

    private (P From, P From2, int Distance) BuildStraightPath(P from, P from2, int distance, HashSet<(P, P)> visited)
    {
        P[] dir = GetDirections(from, from2);
        if (dir.Length == 0)
            return (from, from2, distance);
        if (dir.Length == 1)
        {



            return BuildStraightPath(dir[0], from, distance + 1, visited);
        }
        return (from, from2, distance); // dir.Select(d => (d, from, distance + 1)).ToArray();
    }

    private P[] GetDirections(P from, P from2)
    {
        return GetAllDir(from).Where(d => d.Row >= 0 && d.Row < Rows && d.Col >= 0 && d.Col < Cols && Data[d.Row][d.Col] != '#' && d != from2).ToArray();
    }

}

record P(int Row, int Col)
{
    public override string ToString()
    {
        return (Row + 1) + "-" + (Col + 1);
    }
}
class Node(P name)
{
    public P Name { get; } = name;
    public int Dist { get; set; }
    public List<(Node, int)> Next { get; set; } = [];

    public void AddEdgeTo(Node node, int distance)
    {
        if (Next.IndexOf((node, distance)) < 0)
            Next.Add((node, distance));
    }
}

