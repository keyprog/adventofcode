using System.Text.Json;

var sequences = File.ReadAllLines("test.txt").Select(l => new Sequence(l));
Console.WriteLine(sequences.Sum(s => s.GetNextValue()));

class Helper
{
public static string Visualize(int[][] data)
{
    return JsonSerializer.Serialize(
        new
        {
            kind = new { plotly = true},
            data =  data.Select((d, i) =>
            new {
                y = d
            }).ToArray()
        }
    );
}
}

class Sequence(string input)
{
    private readonly List<int[]> data = [input.Split(' ').Select(n => int.Parse(n)).ToArray()];

    public long GetNextValue()
    {
        while (data[^1].Any(n => n != 0))
        {
            int[] prevLevel = data[^1];
            int[] nextLevel = new int[data[^1].Length - 1];
            for (int i = 0; i < nextLevel.Length; ++i)
            {
                nextLevel[i] = prevLevel[i + 1] - prevLevel[i];
            }
            data.Add(nextLevel);
        }

        int last = 0;
        for (int i = data.Count - 1; i >= 0; i--)
        {
            last = data[i][0] - last;
        }

        return last;
    }

    public List<int[]> Data {get => data; }


}