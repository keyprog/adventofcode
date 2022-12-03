string inputFile = "input2.txt";

var lines = from l in File.ReadLines(inputFile)
            let s = l.Length == 3 ? Decoder.Decode(l[0], l[2]) : throw new ApplicationException("Unable to parse line")
            select Rules.Match(s.s1, s.s2);

Console.WriteLine($"My Result: {lines.Sum(s => s.score2)}");

public enum Shape { Rock = 1, Paper = 2, Scissors = 3 };
public enum Result { Win = 6, Lost = 0, Draw = 3 };

static class Decoder
{
    public static (Shape s1, Shape s2) Decode(char c1, char c2)
    {
        var s1 = GetShape(c1);
        return c2 switch
        {
            'X' => (s1, s1.GetLooser()),
            'Y' => (s1, s1),
            'Z' => (s1, s1.GetWinner()),
            _ => throw new ArgumentException("Unexpected code " + c2)
        };
    }

    public static Shape GetShape(char code)
    {
        return code switch
        {
            'A' => Shape.Rock,
            'B' => Shape.Paper,
            'C' => Shape.Scissors,
            'X' => Shape.Rock,
            'Y' => Shape.Paper,
            'Z' => Shape.Scissors,
            _ => throw new ArgumentException($"Unexpected input '{code}'")
        };
    }
    public static Shape GetWinner(this Shape s)
    {
        int winner = (int)s + 1;
        if (winner > 3)
            winner = 1;
        return (Shape)winner;
    }

    public static Shape GetLooser(this Shape s)
    {
        int looser = (int)s - 1;
        if (looser == 0)
            looser = 3;
        return (Shape)looser;
    }
}

static class Rules
{
    public static (int score1, int score2) Match(char code1, char code2) => Match(Decoder.GetShape(code1), Decoder.GetShape(code2));
    public static (int score1, int score2) Match(Shape a, Shape b)
    {
        var results = (a, b) switch
        {
            (Shape.Rock, Shape.Scissors) or
            (Shape.Paper, Shape.Rock) or
            (Shape.Scissors, Shape.Paper) => (Result.Win, Result.Lost),
            (var c1, var c2) when c1 == c2 => (Result.Draw, Result.Draw),
            _ => (Result.Lost, Result.Win)
        };
        return (a.GetScore(results.Item1), b.GetScore(results.Item2));
    }

    public static int GetScore(this Shape shape, Result result) => (int)shape + (int)result;

}
