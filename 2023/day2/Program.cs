var games = File.ReadAllLines("input.txt").Select(ParseGameString);

foreach (var g in games)
  Console.WriteLine($"Game {g.Num} => {string.Join(',', g.Sets)}");

Func<Set, bool> isPossibleGame = s => s.Red <= 12 && s.Green <= 13 && s.Blue <= 14;
var possibleGames = games.Where(g => g.Sets.All(isPossibleGame));

Console.WriteLine($"Part 1: {possibleGames.Sum(g => g.Num)}");
Console.WriteLine($"Part 2: {games.Select(g => g.GetMinSet()).Sum(s => s.Blue * s.Red * s.Green)}");

static Game ParseGameString(string line)
{
    var span = line.AsSpan();
    int idStart = span.IndexOf(' ') + 1;
    int idEnd = span.IndexOf(':');
    Game game = new();
    game.Num = int.Parse(span[idStart..idEnd]);
    ParseSets(span[(idEnd + 1)..], game.Sets);

    return game;
}

static void ParseSets(ReadOnlySpan<char> line, List<Set> sets)
{
    Span<Range> tokens = stackalloc Range[line.Length / 2];
    int setsCount = line.Split(tokens, ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    for (int i = 0; i < setsCount; ++i)
    {
        sets.Add(ParseSet(line[tokens[i]]));
    }
}

static Set ParseSet(ReadOnlySpan<char> line)
{
    Set s = new();
    Span<Range> tokens = stackalloc Range[line.Length];
    int tokensCount = line.Split(tokens, ',');
    for (int i = 0; i < tokensCount; ++i)
    {
        var token = line[tokens[i]].Trim();
        int space = token.IndexOf(' ');
        int n = int.Parse(token[..space]);
        var name = token[(space + 1)..];
        switch (name)
        {
            case "blue":
                s.Blue = n;
                break;
            case "green":
                s.Green = n;
                break;
            case "red":
                s.Red = n;
                break;
        }
    }
    return s;
}

record Game
{
    public int Num { get; set; }
    public List<Set> Sets { get; } = [];

    public Set GetMinSet()
    {
        var minSet = new Set();
        foreach (var s in Sets)
        {
            if (s.Blue > minSet.Blue) minSet.Blue = s.Blue;
            if (s.Red > minSet.Red) minSet.Red = s.Red;
            if (s.Green > minSet.Green) minSet.Green = s.Green;
        }
        return minSet;
    }
}

record struct Set(int Blue, int Red, int Green);