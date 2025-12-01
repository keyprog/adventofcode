using System.ComponentModel;
using System.Runtime.CompilerServices;

string[] map = File.ReadAllLines("/home/alexp/dev/adventofcode/2024/day8_part2/input.txt");
AntiNodesCollection antinodes = new(map);



Dictionary<char, (int,int)[]> antennaes = map.ScanRows()
                                                    .Where(s => s.Value is not '.' and not '#')
                                                    .GroupBy(s => s.Value)
                                                    .ToDictionary(g => g.Key, g => g.Select(gg => (gg.Row, gg.Col)).ToArray());
                                                    
antennaes.Print();

foreach((char a, (int Row, int Col)[] coors) in antennaes)
{
    if (coors.Length > 1)
    {
    for(int i = 0; i<coors.Length-1;++i)
        for(int j = i+1; j<coors.Length; ++j)
        {
            int dr = coors[j].Row - coors[i].Row;
            int dc = coors[j].Col - coors[i].Col;

            var (r, c) = (coors[i].Row, coors[i].Col);
            while(antinodes.IsInside(r,c))
            {
                antinodes.Add(r, c);
                (r, c) = (r + dr, c + dc);                    
            }
            (r, c) = (coors[i].Row, coors[i].Col);
            while(antinodes.IsInside(r,c))
            {
                antinodes.Add(r, c);
                (r, c) = (r - dr, c - dc);                    
            }

        }
    }
}

antinodes.Antinodes.Order().PrintCollection();
Console.WriteLine($"Total: {antinodes.Antinodes.Count}");

class AntiNodesCollection(string[] map)
{
    private readonly int Rows = map.Length;
    private readonly int Cols = map[0].Length;
    public readonly HashSet<(int,int)> Antinodes = [];
    public bool IsInside(int r, int c) => r >= 0 && c >= 0 && r < Rows && c < Cols;    

    public void Add(int row, int col)
    {
        if (IsInside(row, col))        
            Antinodes.Add((row, col));
    }

}

