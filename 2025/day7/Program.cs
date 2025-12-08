var lines = File.ReadAllLines("/home/alexp/dev/adventofcode/2025/day7/input.txt");
int beam = lines[0].IndexOf('S');

int count = CountSplits(new HashSet<int>([beam]), new Queue<string>(lines[1..]));
count.Print();

static int CountSplits(HashSet<int> beams, Queue<string> rows)
{
    if (!rows.TryDequeue(out string? row))
        return 0;
    
    for(int i=0;i<row.Length;++i)
    {
        Console.Write(beams.Contains(i) ? '|' : '.');            
    }
    Console.WriteLine();    
    Console.WriteLine(row);

    int splitsCount = 0;
    foreach(var beam in beams.ToArray())
    {
        if (row[beam] == '^')
        {
            splitsCount++;
            beams.Add(beam - 1);
            beams.Add(beam + 1);
            beams.Remove(beam);
        }
    }
    return splitsCount + CountSplits(beams, rows);
}

