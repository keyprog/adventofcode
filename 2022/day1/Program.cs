var elfs = ReadElfCalories(File.ReadLines("data2.txt")).ToArray();
Console.WriteLine("Elfs' most calories: " + elfs.Max(e => e.TotalCalories));

var top3 = elfs.OrderByDescending(e => e.TotalCalories).Take(3);
Console.WriteLine("Top 3 carry:" + top3.Sum(e => e.TotalCalories));

IEnumerable<ElfCalories> ReadElfCalories(IEnumerable<string> lines)
{
    int elfNumber = 1;
    int elfCalories = 0;
    foreach (var line in lines)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            elfCalories += int.Parse(line);
        }
        else // empty line - elfs separator
        {
            yield return new ElfCalories(elfNumber, elfCalories);
            elfNumber++;
            elfCalories = 0;
        }
    }
    if (elfCalories > 0)
        yield return new ElfCalories(elfNumber, elfCalories);
}
record struct ElfCalories(int Number, int TotalCalories);