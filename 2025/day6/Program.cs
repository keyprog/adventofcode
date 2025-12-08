
using System.Numerics;

string[] lines = File.ReadAllLines("/home/alexp/dev/adventofcode/2025/day6/input.txt");
int[,] problems = lines[..^1].ToIntArray2D();
string[] operation = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

long result = 0;

for(int col = 0; col < problems.GetLength(1); ++col)
{
    long res = operation[col] switch
    {
        "+" => problems.ScanColumn(col).Select(v => (long)v).Aggregate((agg, v) => checked(agg + v)),
        "*" => problems.ScanColumn(col).Select(v => (long)v).Aggregate((agg, v) => checked(agg * v)),
    };
    checked { result += res; }
}


result.Print();

