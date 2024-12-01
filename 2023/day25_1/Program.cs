using System.Collections.Immutable;

var data = new Input(File.ReadAllLines("input.txt"));
var result = data.Cut().Print();
Console.WriteLine(result.Item1 * result.Item2);
//Graph.ToJson(nodes.Select(n => new Graph.Node(n)), edges).Print();

Console.WriteLine($"Total Edges: " + data.Edges.Count);
//Console.WriteLine("Total Nodes: " + data.Nodes.Count);

class Input
{
    public List<Edge> Edges { get; } = [];

    public Input(string[] lines)
    {
        Edges = Parse(lines).Select(e => new Edge(e.Item1, e.Item2)).ToList();
    }

    public static Dictionary<string, Node> GetNodes(IEnumerable<Edge> edges)
    {
        Dictionary<string, Node> nodes = [];
        foreach (var e in edges)
        {
            if (!nodes.TryGetValue(e.Node1, out var node))
            {
                node = new Node(0, e.Node1);
                nodes.Add(e.Node1, node);
            }

            node.Edges.Add(e);
            if (!nodes.TryGetValue(e.Node2, out node))
            {
                node = new Node(0, e.Node2);
                nodes.Add(e.Node2, node);
            }
            node.Edges.Add(e);
        }
        return nodes;
    }

    public IEnumerable<(string, string)> Parse(string[] lines)
    {

        foreach (var l in lines)
        {
            int separator = l.IndexOf(':');
            string name = l.Substring(0, separator);
            string[] dep = l.Substring(separator + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var n2 in dep)
                yield return name.CompareTo(n2) < 0 ? (name, n2) : (n2, name);
        }
    }

    public (int, int) Cut()
    {
        Dictionary<string, Node> nodes;
        List<Edge> edges;
        while (true)
        {
            edges = Edges.Select(e => new Edge(e.Node1, e.Node2)).ToList();
            nodes = GetNodes(edges);
            while (nodes.Count > 2)
            {
                ContractRand(nodes, edges);
            }
            int edgesCount = edges.Count;
            if (edgesCount == 3)
            {
                foreach (var e in edges)
                {
                    Console.WriteLine($"{e.Node1}/{e.Node2}");
                }
                break;
            }
        }

        var nn = nodes.ToArray();
        return (nn[0].Value.Count, nn[1].Value.Count);

    }

    Random rnd = new Random((int)DateTime.UtcNow.Ticks);
    public int GetEdgesCount(int[,] matrix) => matrix.ForEach().Count(n => n.Value == 1);
    public void ContractRand(Dictionary<string, Node> nodes, List<Edge> edges)
    {
        int edgeIndex = rnd.Next(edges.Count);
        Edge cutEdge = edges[edgeIndex];
        edges.RemoveAt(edgeIndex);

        var (node1, node2) = (nodes[cutEdge.Node1], nodes[cutEdge.Node2]);

        node1.Edges.Remove(cutEdge);
        node2.Edges.Remove(cutEdge);

        foreach(var e in node1.Edges)
        {
            if (e.Node1 == cutEdge.Node1)
                e.Node1 = cutEdge.Node2; 
            if (e.Node2 == cutEdge.Node1)
                e.Node2 = cutEdge.Node2;
        }
        node2.Edges.AddRange(node1.Edges);
        node2.Edges.RemoveAll(e => e.Node1 == e.Node2);
        edges.RemoveAll(e => e.Node1 == e.Node2);

        node2.Count += node1.Count;
        nodes.Remove(cutEdge.Node1);
    }
}

class Edge(string node1, string node2)
{
    public string Node1 {get;set;} = node1;
    public string Node2 {get;set;} = node2;
}

class Node(int id, string name)
{
    public string Name { get; } = name;
    public int Id { get; } = id;
    public int Count { get; set; } = 1;
    public List<Edge> Edges = [];
}