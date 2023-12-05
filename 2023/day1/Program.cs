using System.Linq;

string[] lines = File.ReadAllLines("input.txt");

int sum = lines.Sum(GetDigits);

Console.WriteLine(sum);

static int GetDigits(string line)
{
    List<int> digits = [];
    for (int i = 0; i < line.Length; ++i)
    {
        if (char.IsDigit(line[i]))
        {
            digits.Add(line[i] - '0');
        }
        else if (Line2Num(line.AsSpan(i)) is int c)
        {
            digits.Add(c);
        }
    }
    int num = digits[0] * 10 + digits[^1];
    Console.WriteLine($"{line} => {num}");
    return num;
}

static int? Line2Num(ReadOnlySpan<char> line)
        => line switch
        {
        ['o', 'n', 'e', ..] => 1,
        ['t', 'w', 'o', ..] => 2,
        ['t', 'h', 'r', 'e', 'e', ..] => 3,
        ['f', 'o', 'u', 'r', ..] => 4,
        ['f', 'i', 'v', 'e', ..] => 5,
        ['s', 'i', 'x', ..] => 6,
        ['s', 'e', 'v', 'e', 'n', ..] => 7,
        ['e', 'i', 'g', 'h', 't', ..] => 8,
        ['n', 'i', 'n', 'e', ..] => 9,
        ['z', 'e', 'r', 'o', ..] => 0,
            _ => null
        };

