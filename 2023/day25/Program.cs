var data = new Input(File.ReadAllLines("test.txt"));
data.Cut().Print();
//Graph.ToJson(nodes.Select(n => new Graph.Node(n)), edges).Print();

Console.WriteLine($"Total Edges: " + data.GetEdgesCount(data.EdgesMatrix));
Console.WriteLine("Total Nodes: " +data.Nodes.Count);

class Input
{
    public List<Edge> Edges { get; } = [];
    public Dictionary<string, Node> Nodes { get; } = [];
    public List<Node> NodesList { get; } = [];

    public int[,] EdgesMatrix;

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
    int nodeid = 0;
    public Node GetOrCreate(string name)
    {
        if (!Nodes.TryGetValue(name, out Node? n))
        {
            n = new Node(nodeid++, name);
            Nodes.Add(name, n);
            NodesList.Add(n);
        }
        return n;
    }

    public Input(string[] lines)
    {
        var edges = Parse(lines);
        foreach (var (n1, n2) in edges)
        {
            GetOrCreate(n1);
            GetOrCreate(n2);
        }

        EdgesMatrix = new int[Nodes.Count, Nodes.Count];
        foreach (var (n1, n2) in edges)
        {
            Node nn1 = GetOrCreate(n1);
            Node nn2 = GetOrCreate(n2);
            EdgesMatrix[nn1.Id, nn2.Id] = 1;
            EdgesMatrix[nn2.Id, nn1.Id] = 1;
        }
    }

    public (int, int) Cut()
    {
        List<Node> nodes;
        while (true)
        {
            int[,] m = new int[Nodes.Count, Nodes.Count];
            foreach (var (i, j) in m.ForEachIndex())
            {
                m[i, j] = EdgesMatrix[i, j];
                if(i == j && EdgesMatrix[i,j] == 1)
                    throw new ApplicationException();
            }

            nodes = Nodes.Values.ToList();
            foreach (var n in nodes) n.Count = 1;
            while (nodes.Count > 2)
            {
                ContractRand(nodes, m);
            }
            int edgesCount = m.ForEach().Count(n => n.Value == 1 && n.Col != n.Row);
            if (edgesCount == 6)
            {
                foreach (var (r, c, _) in m.ForEach().Where(n => n.Value == 1))
                {
                    Console.WriteLine($"{NodesList[r].Name}/{NodesList[c].Name}");
                }
                break;
            }
        }

        return (nodes[0].Count, nodes[1].Count);

    }

    Random rnd = new Random((int)DateTime.UtcNow.Ticks);
    public int GetEdgesCount(int[,] matrix) => matrix.ForEach().Count(n => n.Value == 1);
    public void ContractRand(List<Node> nodes, int[,] edges)
    {
        //edges.Print();
        int nodeIndex = rnd.Next(nodes.Count);
        Node node = nodes[nodeIndex];
        nodes.RemoveAt(nodeIndex);
        var adjacentNodes = Enumerable.Range(0, edges.GetLength(1)).Where(c => edges[node.Id, c] == 1).ToArray();
        Node node2 = NodesList[adjacentNodes[rnd.Next(adjacentNodes.Length)]];
        edges[node.Id, node2.Id] = 0;
        edges[node2.Id, node.Id] = 0;

        node2.Count += node.Count;
        //Console.WriteLine($"Removing: {node.Name}-{node2.Name}"); 
        for (int c = 0; c < edges.GetLength(0); ++c)
        {
            if (edges[node.Id, c] == 1)
            {
                edges[node.Id, c] = 0;
                edges[c, node.Id] = 0;
                //if (c != node2.Id)
                {
                    edges[node2.Id, c] = 1;
                    edges[c, node2.Id] = 1;
                }
            }
        }
    }
}

record Edge(string Node1, string Node2);
class Node(int id, string name)
{
    public string Name { get; } = name;
    public int Id { get; } = id;

    public int Count { get; set; } = 1;

}