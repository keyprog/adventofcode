using System.Dynamic;
using System.Reflection.Metadata;

var lines = File.ReadAllLines("/home/alexp/dev/adventofcode/2025/day7/input.txt");
int beam = lines[0].IndexOf('S');
long[] beams = new long[lines[0].Length];
beams[beam] = 1;

long count = CountSplits(beams, new Queue<string>(lines[1..]));
count.Print();

static long CountSplits(long[] beams, Queue<string> rows)
{
    string? row = null;
    do
    {
        if (!rows.TryDequeue(out row))
        {
            Console.WriteLine($"{string.Concat(beams.Select(bb => bb > 0 ? bb.ToString() : "."))} {beams.Sum()}");
            return beams.Sum();
        }
    } while (row.IndexOf('^') < 0);    

    Console.WriteLine($"{string.Concat(beams.Select(bb => bb > 0 ? bb.ToString() : "."))} {beams.Sum()}");
    Console.WriteLine(row);

    long[] newBeams = new long[beams.Length];
    for (int position = 0; position < beams.Length; ++position)
    {
        long b = beams[position];
        if (b == 0)
            continue;

        if (row[position] == '^')
        {
            newBeams[position - 1] += b;
            newBeams[position + 1] += b;
        }
        else
        {
            newBeams[position] += b;
        }
    }

    
    
    return CountSplits(newBeams, rows);
}

