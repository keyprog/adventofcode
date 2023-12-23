using Point = (int x, int y);
var bricks = Parser.Parse(File.ReadAllLines("input.txt")).ToArray();
int BricksCount = bricks.Length;

//bricks.PrintLines();
Array.Sort(bricks, (b1, b2)=>b1.Start.Z.CompareTo(b2.Start.Z));

for(int i=0; i< bricks.Length; ++i)
{
    while (bricks[i].CanMoveDown(bricks.AsSpan()[0..i]))
    {
        bricks[i].MoveDown();
    }
}

Array.Sort(bricks, (b1, b2)=>b1.Start.Z.CompareTo(b2.Start.Z));
bricks.PrintLines();
Dictionary<int, StatItem> bricksRelations = Enumerable.Range(0, bricks.Length).ToDictionary(i => i, i=> new StatItem());

for(int i=0;i<bricks.Length; ++i)
{
    int z = bricks[i].End.Z;
    for(int j=i+1;j<bricks.Length;++j)
    {
        if(bricks[j].Start.Z > z + 1)
            break;

        bool isUnder =  bricks[i].IsUnder(bricks[j]);
        if(isUnder)
        {
            bricksRelations[i].Above.Add(j);
            bricksRelations[j].Below.Add(i);
        }        
    }
}

int count = 0;
for(int i=0;i<bricks.Length;++i)
{
    if(bricksRelations[i].Above.All(b => bricksRelations[b].Below.Count > 1))
        count++;
}

Console.WriteLine("Part 1: " + count);

foreach(var id in Enumerable.Range(0, bricks.Length))
{
    Console.WriteLine($"{id} => {string.Join(',', bricksRelations[id].Above)}");
}

count = 0;
HashSet<int> fall = [];
for(int i = bricks.Length - 1; i>=0; i--)
{
    fall.Add(i); 
    foreach(var a in bricksRelations[i].Above)  
        IterateOnce(a, fall);

    count+=fall.Count-1;
    fall.Clear();
}
Console.WriteLine("Part 2: " + count);

void IterateOnce(int from, HashSet<int> toFall)
{
    if(bricksRelations[from].Below.All(b => toFall.Contains(b)))
    {
        toFall.Add(from);

        foreach(var a in bricksRelations[from].Above)
        {
             IterateOnce(a, toFall);
        }
    }
}

public static class Parser
{
    public static IEnumerable<Brick> Parse(string[] lines)
    {
        foreach (var line in lines)        
            yield return ParseBrick(line);        
    }

    public static Brick ParseBrick(ReadOnlySpan<char> line)
    {
        int tilde = line.IndexOf('~');
        Span<Range> start = stackalloc Range[3];
        Span<Range> end = stackalloc Range[3];

        line[0..tilde].Split(start, ',');
        var b2 = line[(tilde + 1)..];
        b2.Split(end, ',');

        var s = new BrickEnd(int.Parse(line[start[0]]), int.Parse(line[start[1]]), int.Parse(line[start[2]]));
        var e = new BrickEnd(int.Parse(b2[end[0]]), int.Parse(b2[end[1]]), int.Parse(b2[end[2]]));
        if (s.Z > e.Z)
            (s, e) = (e, s);
        return new Brick(s, e);
    }

}

public class Brick(BrickEnd start, BrickEnd end)
{
    public BrickEnd Start {get; set;} = start;
    public BrickEnd End {get;set;} = end;

    public void MoveDown() => MoveDown(1);
    public void MoveDown(int places)
    {
        Start = Start with { Z = Start.Z - places };
        End = End with { Z = End.Z - places };
    }

    public bool CanMoveDown(ReadOnlySpan<Brick> sorted)
    {
        if (this.Start.Z == 1) return false;
        foreach(var b in sorted)
        {
            
            bool isBelow = b != this && b.End.Z == this.Start.Z - 1;
            if (isBelow)
            {

                if(b.IntersectsWith(this))  
                    return false;
            }
        }
        return true;
    }

    public bool IsUnder(Brick other)
    {
        return other.Start.Z == End.Z + 1 && other.IntersectsWith(this);
    }

    public bool IntersectsWith(Brick other)
    {
        return MathExtensions.DoIntersect((Start.X, Start.Y), (End.X, End.Y), (other.Start.X, other.Start.Y), (other.End.X, other.End.Y));


    }

    public override string ToString()
    {
        return $"Start: {Start}, End: {End}";
    }
}

public record BrickEnd(int X, int Y, int Z);

public class StatItem
{
    public List<int> Above {get;} = new();
    public List<int> Below {get;} = new();
}

public static class MathExtensions
{
    // Given three collinear points p, q, r, the function checks if 
// point q lies on line segment 'pr' 
public static bool OnSegment(Point p, Point q, Point r) 
{ 
    if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) && 
        q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y)) 
    return true; 
  
    return false; 
} 
  
// To find orientation of ordered triplet (p, q, r). 
// The function returns following values 
// 0 --> p, q and r are collinear 
// 1 --> Clockwise 
// 2 --> Counterclockwise 
static int orientation(Point p, Point q, Point r) 
{ 
    // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
    // for details of below formula. 
    int val = (q.y - p.y) * (r.x - q.x) - 
            (q.x - p.x) * (r.y - q.y); 
  
    if (val == 0) return 0; // collinear 
  
    return (val > 0)? 1: 2; // clock or counterclock wise 
} 
  
// The main function that returns true if line segment 'p1q1' 
// and 'p2q2' intersect. 
public static bool DoIntersect(Point p1, Point q1, Point p2, Point q2) 
{ 
    // Find the four orientations needed for general and 
    // special cases 
    int o1 = orientation(p1, q1, p2); 
    int o2 = orientation(p1, q1, q2); 
    int o3 = orientation(p2, q2, p1); 
    int o4 = orientation(p2, q2, q1); 
  
    // General case 
    if (o1 != o2 && o3 != o4) 
        return true; 
  
    // Special Cases 
    // p1, q1 and p2 are collinear and p2 lies on segment p1q1 
    if (o1 == 0 && OnSegment(p1, p2, q1)) return true; 
  
    // p1, q1 and q2 are collinear and q2 lies on segment p1q1 
    if (o2 == 0 && OnSegment(p1, q2, q1)) return true; 
  
    // p2, q2 and p1 are collinear and p1 lies on segment p2q2 
    if (o3 == 0 && OnSegment(p2, p1, q2)) return true; 
  
    // p2, q2 and q1 are collinear and q1 lies on segment p2q2 
    if (o4 == 0 && OnSegment(p2, q1, q2)) return true; 
  
    return false; // Doesn't fall in any of the above cases 
} 
}