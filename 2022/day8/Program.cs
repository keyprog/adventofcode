
const string Input = "input2.txt";
var arr = ParseInput(File.ReadAllLines(Input));
Console.WriteLine($"Field size: {arr.GetLength(0)}x{arr.GetLength(1)}");
// Part I
CalculateVisibility(arr);

int innerVisible = 0;
for (int i = 1; i < arr.GetLength(0) - 1; ++i)
    for (int j = 1; j < arr.GetLength(1) - 1; ++j)
        if (arr[i, j].IsVisible) innerVisible++;

Console.WriteLine($"Inner visible: " + innerVisible);
Console.WriteLine($"Total visible: " + (innerVisible + (arr.GetLength(0) + arr.GetLength(1)) * 2 - 4));

// Part II
var scores = CalculateScenicScores(arr);
int maxScore = 0;
foreach (var score in scores)
    if (score > maxScore) maxScore = score;

Console.WriteLine("Top score: " + maxScore);

int[,] CalculateScenicScores(Location[,] field)
{
    int rows = field.GetLength(0);
    int cols = field.GetLength(1);
    var view = new int[rows, cols];
    for (int r = 1; r < rows - 1; ++r)
        for (int c = 1; c < cols - 1; ++c)
        {
            int left = 1, right = 1, top = 1, bottom = 1;
            for (int cc = c - 1; cc > 0 && field[r, cc].TreeHeight < field[r, c].TreeHeight; cc--)
                left++;
            for (int cc = c + 1; cc < cols - 1 && field[r, cc].TreeHeight < field[r, c].TreeHeight; cc++)
                right++;
            for (int rr = r - 1; rr > 0 && field[rr, c].TreeHeight < field[r, c].TreeHeight; rr--)
                top++;
            for (int rr = r + 1; rr < rows - 1 && field[rr, c].TreeHeight < field[r, c].TreeHeight; rr++)
                bottom++;
            view[r, c] = left * right * top * bottom;
        };
    return view;
}

void CalculateVisibility(Location[,] field)
{
    int rows = field.GetLength(0);
    int cols = field.GetLength(1);
    for (int r = 1; r < rows - 1; ++r)
    {
        for (int c = 1; c < cols - 1; ++c)
        {
            // calculating from top left
            field.Do(r, c, (ref Location l) =>
            {
                var left = field[r, c - 1];
                l.MaxLeft = Math.Max(left.TreeHeight, left.MaxLeft);
                var top = field[r - 1, c];
                l.MaxTop = Math.Max(top.MaxTop, top.TreeHeight);
            });
            // calculating from bottom right
            field.Do(rows - r - 1, cols - c - 1, (ref Location l) =>
            {
                var right = field[rows - r - 1, cols - c];
                l.MaxRight = Math.Max(right.MaxRight, right.TreeHeight);
                var bottom = field[rows - r, cols - c - 1];
                l.MaxBottom = Math.Max(bottom.MaxBottom, bottom.TreeHeight);
            });
        }

    }
}
Location[,] ParseInput(IEnumerable<string> lines)
{
    int width = 0;
    int height = 0;
    foreach (var l in lines)
    {
        width = l.Length > width ? l.Length : width;
        height++;
    }

    var arr = new Location[width, height];
    int row = 0;
    foreach (var l in lines)
    {
        for (int i = 0; i < l.Length; ++i)
            arr[row, i] = new Location((byte)(l[i] - '0'));
        row++;
    }
    return arr;
}

record struct Location(byte TreeHeight)
{
    public byte MaxLeft { get; set; }
    public byte MaxRight { get; set; }
    public byte MaxTop { get; set; }
    public byte MaxBottom { get; set; }
    public bool IsVisible { get { return MaxLeft < TreeHeight || MaxRight < TreeHeight || MaxTop < TreeHeight || MaxBottom < TreeHeight; } }
}

static class Extensions
{
    public delegate void DoDelegate<T>(ref T element);
    public static void Do<T>(this T[,] array, int row, int col, DoDelegate<T> action)
    {
        T element = array[row, col];
        action(ref element);
        array[row, col] = element;
        Console.WriteLine($"[{row},{col}] = {element}");
    }
}
