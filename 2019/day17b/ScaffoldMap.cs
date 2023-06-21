namespace ic;

struct ScaffoldMap
{
    private readonly int[,] map;

    public int Width { get; init; }
    public int Height { get; init; }

    public ScaffoldMap(long[] data)
    {
        Width = GetMapWidth(data);
        Height = GetMapHeight(data);
        map = new int[Height, Width];
        for (int row = 0; row < Height; ++row)
        {
            for (int col = 0; col < Width; ++col)
            {
                map[row, col] = (int)data[row * (Width + 1) + col];
            }
        }
    }

    public char this[int row, int col]
    {
        get
        {
            if (col >= 0 && col < Width && row >= 0 && row < Height)
                return (char)map[row, col];
            return ' ';
        }
    }

    public void DumpAsAscii(TextWriter writer)
    {
        for (int row = 0; row < Height; ++row)
        {
            for (int col = 0; col < Width; ++col)
            {
                writer.Write((char)map[row, col]);
            }
            writer.WriteLine();
        }
    }

    public static int GetMapWidth(long[] data) => Array.IndexOf(data, '\n');

    public static int GetMapHeight(long[] data) => data.Length / (GetMapWidth(data) + 1); // including nl
}
