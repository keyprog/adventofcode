var map = new Map(File.ReadAllLines("test.txt"));

P start = new P(0, 1);
P dest = new(map.Rows - 1, map.Data[^1].IndexOf('.'));

int destLength = 0;

HashSet<P> visited = new();
Dictionary<(P, P), (int Dist, bool HitTail, bool DeadEnd)> dist = new();
Dictionary<(P,P), ((P,P), int)> fastPath = [];
int saved = 0;

GetMaxLength(start, new(0, 0), 0, (start, new(0,0), 0));

Console.WriteLine("Saved: " + saved);
//Console.WriteLine(string.Join('\n', fastPath.Select(kv => kv.Key + "=>" + kv.Value)));
//Console.WriteLine(string.Join('\n', dist.Values));

//Console.WriteLine(dist.Where(k =>k.Key.Item1 == dest).Single().Value);
Console.WriteLine(destLength);
Node root = new Node(start);
Node destNode = new Node(dest);

//BuildGraph(root, start, 0);
int count = 0;
//TraverseAll(root);
//Console.WriteLine(count);
//destNode.Dist.Print();

void TraverseAll(Node root)
{
    Queue<Node> nodes = new();
    nodes.Enqueue(root);
    while (nodes.Count > 0)
    {
        count++;
        var n = nodes.Dequeue();
        Console.WriteLine(n.Name);
        foreach (var nn in n.Next)
            nodes.Enqueue(nn);
    }
}

Dictionary<P, Node> nodesMap = [];
bool BuildGraph(Node fromNode, P from, int curDist)
{
    if (from == dest)
    {
        if (curDist > destNode.Dist)
            destNode.Dist = curDist;
        fromNode.Next.Add(destNode);
        return true;
    }

    var next = GetAllDir(from)
        .Where(pp => pp.Row >= 0 && pp.Row < map.Rows && pp.Col >= 0
                  && pp.Col < map.Cols
                  && map.Data[pp.Row][pp.Col] != '#'
                  && pp != from && !visited.Contains(pp))
        .ToArray();

    if (next.Length == 0)
        return false;

    visited.Add(from);
    if (next.Length == 1)
    {
        BuildGraph(fromNode, next[0], curDist + 1);
    }
    else
    {
        // fork
        foreach (var n in next)
        {

            Node nn = new Node(n) { Dist = curDist };
            if (BuildGraph(nn, n, curDist + 1))
                fromNode.Next.Add(nn);

        }
    }
    visited.Remove(from);

    return fromNode.Next.Any();
}


(bool HitTail, bool DeadEnd) GetMaxLength(P from, P from2, int curDist, (P,P,int)? sp)
{
    if (from == dest)
    {
        if (destLength < curDist)
        {
            destLength = curDist;
            Console.WriteLine(destLength);
        }//map.Print(from);
        return (false, false);
    }

    if(fastPath.TryGetValue((from, from2), out var fp) )
    {
        saved += fp.Item2;
        visited.Add(from);
        visited.Add(fp.Item1.Item2);
        var rr= GetMaxLength(fp.Item1.Item1, fp.Item1.Item2, curDist + fp.Item2, null);
        visited.Remove(fp.Item1.Item2);
        visited.Remove(from);
        return rr;
    }

    if (dist.TryGetValue((from, from2), out var state) && (state.DeadEnd || state.Dist >= curDist))
        return (false, state.DeadEnd);

    var next = GetAllDir(from)
        .Where(pp => pp.Row >= 0 && pp.Row < map.Rows && pp.Col >= 0 && pp.Col < map.Cols && map.Data[pp.Row][pp.Col] != '#'
            && pp != from2).ToArray();

    if (next.Length > 2)
    {
        //Console.WriteLine(from + "=>" + next.Length);
    }
    if (next.Length == 0 && from != dest)
    {
        dist.Add((from, from2), (0, false, true));
        return (false, true);
    }

    bool isStraightPath = next.Length == 1 && !visited.Contains(next[0]);
    if (!isStraightPath && sp is not null)
    {
        fastPath[(sp.Value.Item1, sp.Value.Item2)] = ((from, from2), curDist - sp.Value.Item3);
        sp = null;
    }
    if(isStraightPath && sp is null)
    {
        sp = new (from, from2, curDist);
    }

    visited.Add(from);

    bool hitTail = false;
    bool deadend = true;


    foreach (var n in next)
    {
        if (visited.Contains(n))
        {
            hitTail = true;
            continue;
        }
        var res = GetMaxLength(n, from, curDist + 1, sp);
        if (!res.DeadEnd)
            deadend = false;
        if (res.HitTail)
            hitTail = true;
    }

    visited.Remove(from);


    if (hitTail)
        return (true, false);

    if (!dist.TryGetValue((from, from2), out state) || state.Item1 < curDist)
        dist[(from, from2)] = (curDist, false, deadend);
    return (false, deadend);
}

P[] GetAllDir(P f)
{
    char tile = map.Data[f.Row][f.Col];
    return tile switch
    {
        //'<' => [new (f.Row, f.Col - 1)],
        // '>' => [new (f.Row, f.Col + 1)],
        //'^' => [new (f.Row - 1, f.Col)],
        //'v' => [new (f.Row + 1, f.Col)],
        _ => [new(f.Row - 1, f.Col), new(f.Row + 1, f.Col), new(f.Row, f.Col - 1), new(f.Row, f.Col + 1)]
    };
}

class Node(P name)
{
    public P Name { get; } = name;
    public int Dist { get; set; }
    public List<Node> Next { get; } = [];
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
}

record P(int Row, int Col)
{

}