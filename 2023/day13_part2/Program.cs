using System.Reflection.Emit;

var patterns = Parser.Parse(File.ReadLines("input.txt"));

int res = patterns.Select(p => p.GetSymmetry()).Sum();
Console.WriteLine(res);

class Parser
{
    public static IEnumerable<Pattern> Parse(IEnumerable<string> lines)
    {
        List<string> pattern = new();
        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                yield return new Pattern(pattern.ToArray());
                pattern.Clear();
            }
            else
            {
                pattern.Add(line);
            }
        }
        yield return new Pattern(pattern.ToArray());
    }
}

class Pattern(string[] Data)
{
    public int ColsCount { get; } = Data[0].Length;
    public int RowsCount { get; } = Data.Length;

    public int GetSymmetry()
    {
        int? vert = FindVertSymmetry();
        if (vert.HasValue) return vert.Value;
        int? horz = FindHorsSymmetry();
        return horz!.Value * 100;
    }

    public int? FindVertSymmetry()
    {
        for (int c = 0; c < ColsCount - 1; ++c)
        {
            if(IsVertSymmetry(Data, c))
                return c + 1;
        }
        return null;
    }

    private bool IsVertSymmetry(string[] Data, int c)
    {
        int smugdeCount = 0;
        int checkWidth = Math.Min(c + 1, ColsCount - c - 1);
        for (int cc = 0; cc < checkWidth; cc++)
        {
            for (int r = 0; r < RowsCount; ++r)
            {
                if (Data[r][c - cc] != Data[r][c + 1 + cc])
                {
                    smugdeCount++;
                    if(smugdeCount > 1)
                    return false;
                }
            }
        }

        return smugdeCount==1;
    }

    public int? FindHorsSymmetry()
    {
        for (int r = 0; r < RowsCount - 1; ++r)
        {
            if(IsHorsSymmetry(Data, r))
                return r + 1;
        }
        return null;
    }

    private bool IsHorsSymmetry(string[] Data, int r)
    {
        int smugdeCount = 0;
        int checkHeight = Math.Min(r + 1, RowsCount - r - 1);
        for (int rr = 0; rr < checkHeight; ++rr)
            for (int c = 0; c < ColsCount; ++c)
            {
                if (Data[r - rr][c] != Data[r + rr + 1][c])
                {
                    smugdeCount++;
                    if (smugdeCount > 1)
                    return false;
                }
            }

        return smugdeCount == 1;
    }
}