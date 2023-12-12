(string instructions, Map map) = Parser.Parse(File.ReadAllLines("input.txt"));
List<Ghost> ghosts = map.GetAllStartPositions()
                        .Select(startPos => new Ghost(instructions, startPos, map))
                        .ToList();

ghosts.ForEach(g => g.MoveToNextExit());

long maxSteps = ghosts.Max(g => g.Steps);
while (ghosts.Any(g => g.Steps != maxSteps))
{
    var minGhost = ghosts.MinBy(g => g.Steps);
    while (minGhost!.Steps < maxSteps)
        minGhost!.MoveToNextExit();
    maxSteps = minGhost.Steps;
}

Console.WriteLine(maxSteps);

class Parser
{
    public static (string, Map) Parse(string[] lines) => (lines[0], new Map(lines.Skip(2)));
}

class Map(IEnumerable<string> lines)
{
    private readonly Dictionary<string, (string Left, string Right)> map = lines.ToDictionary(l => l[..3], l => (l[7..10], l[12..15]));

    public string Get(string from, char instruction)
        => instruction == 'L' ? map[from].Left : map[from].Right;

    public string[] GetAllStartPositions()
        => map.Keys.Where(k => k.EndsWith('A')).ToArray();
}

class Ghost(string instructions, string startLocation, Map map)
{
    private int currentInstr = 0;
    private readonly Dictionary<(string curPos, int curInst), (string, int, long)> memo = [];

    public void MoveNext()
    {
        char instr = GetNextInstruction();
        CurrentPosition = map.Get(CurrentPosition, instr);
        Steps++;
    }

    public void MoveToNextExit()
    {
        var initial = (CurrentPosition, currentInstr);
        if (memo.TryGetValue(initial, out (string newPosition, int newInstr, long steps) values))
        {
            CurrentPosition = values.newPosition;
            currentInstr = values.newInstr;
            Steps += values.steps;
            return;
        }

        int steps = 0;
        do
        {
            MoveNext();
            steps++;
        } while (!CurrentPosition.EndsWith('Z'));

        memo[initial] = (CurrentPosition, currentInstr, steps);

        Console.WriteLine($"{initial.CurrentPosition}-{CurrentPosition}");
    }

    public string CurrentPosition { get; private set; } = startLocation;

    public long Steps { get; private set; }

    private char GetNextInstruction()
    {
        char c = instructions[currentInstr];
        currentInstr++;
        if (currentInstr == instructions.Length)
            currentInstr = 0;
        return c;
    }
}