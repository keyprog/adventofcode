
var (instr, map) = Parser.Parse(File.ReadAllLines("input.txt"));

int steps = 0;
string current = "AAA";
while(current != "ZZZ")
{
    current = instr.GetNext() == 'L' ? map.GetLeft(current) : map.GetRight(current);
    steps++;
}

    Console.WriteLine(steps);


class Parser
{
    public static (Instructions, Map) Parse(string[] lines)
    {
        return  (new Instructions(lines[0]), new Map(lines[2..]));
    }

}
class Map
{
    private readonly Dictionary<string, (string,string)> map = new ();

    public Map(IEnumerable<string> lines)
    {
        foreach(var l in lines)
        {
            map.Add(l[..3], (l[7..10], l[12..15]));
        }
    }

    public string GetLeft(string from) => map[from].Item1;
    public string GetRight(string from) => map[from].Item2;


}
class Instructions(string instructions)
{
    private int current = 0;

    public char GetNext()
    {
        char c = instructions[current];
        current++;
        if (current == instructions.Length)
            current = 0;
        return c;
    }

}