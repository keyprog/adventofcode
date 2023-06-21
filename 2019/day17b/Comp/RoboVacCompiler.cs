namespace ic;

enum CompilerMode { Release, Debug }
class RoboVacCompiler
{
    private static readonly (string[], string[], string[], string[]) NoResult = (Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
    public static char[] Compile(string[] str, CompilerMode mode = CompilerMode.Release)
    {
        var result = Refactor(str);
        if (result == NoResult)
            return Array.Empty<char>();
        var (Main, A, B, C) = result;
        var runtimeOptions = new string[] { mode == CompilerMode.Release ? "n" : "y" };
        List<char> input = new List<char>(256);
        foreach (var segment in new[] { Main, A, B, C, runtimeOptions })
        {
            input.AddRange(string.Join(',', segment));
            input.Add('\n');
        }

        return input.ToArray();
    }

    public static (string[], string[], string[], string[]) Refactor(string[] str)
    {
        for (int a = 2; a <= 11; ++a)
        {
            for (int b = 2; b <= 11; ++b)
            {
                for (int c = 2; c <= 11; ++c)
                {
                    string[] A = str[..a];
                    string[] B = FindNextSeq(str, b, A);
                    string[] C = FindNextSeq(str, c, A, B);
                    string[] main = CheckFunctions(str, ("A", A), ("B", B), ("C", C));
                    if (main.Length != 0)
                        return (main, A, B, C);
                }
            }
        }
        return NoResult;
    }

    private static string[] FindNextSeq(ReadOnlySpan<string> str, int length, params string[][] funcs)
    {
        while (length < str.Length)
        {
            foreach (var seq in funcs)
            {
                if (str.StartsWith(seq))
                    str = str[seq.Length..];
                else
                    return str[..length].ToArray();
            }
        }
        return Array.Empty<string>();
    }

    private static string[] CheckFunctions(ReadOnlySpan<string> str, params (string Name, string[] Seq)[] funcs)
    {
        List<string> mainFunc = new List<string>(11);
        //Console.Write($"A {string.Join(',', a.ToArray())} B {string.Join(',', b.ToArray())} C {string.Join(',', c.ToArray())}");
        while (str.Length > 0)
        {
            bool matched = false;
            foreach (var (name, seq) in funcs)
            {
                if (str.StartsWith(seq))
                {
                    matched = true;
                    mainFunc.Add(name);
                    str = str[seq.Length..];
                    break;
                }
            }

            if (!matched)
            {
                return Array.Empty<string>();
            }
        }

        return mainFunc.ToArray();
    }
}