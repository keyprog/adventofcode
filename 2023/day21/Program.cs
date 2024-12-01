using P = (int Row, int Col);

var map = new Map(File.ReadAllLines("input.txt"));
int steps = 26501365;

var start = map.Data.ScanRows().Where(s => s.Value == 'S').Select(s => (s.Row, s.Col)).First();

long p1 = 0;//map.CountTiles(steps, start, start.Row - steps * 2, start.Row + steps * 2, start.Col - steps * 2, start.Col + steps * 2, false);

long p2 = map.CountBigTiles(steps, start);


Console.WriteLine($"P1: {p1}, P2: {p2}");

class Map(string[] lines)
{
    public string[] Data { get; } = lines;
    public int Rows { get; } = lines.Length;
    public int Cols { get; } = lines[0].Length;

    public long CountTiles(int steps, (int Row, int Col) start, int minRow, int maxRow, int minCol, int maxCol, bool print = false)
    {
        if (steps < 0) return 0;
        if (steps == 0) return 1;
        int maxSteps = (maxRow - minRow) * 2 + steps % 2;
        maxSteps = steps < maxSteps ? steps : maxSteps;
        var matrix = FindPlots(maxSteps, start);

        var plots = matrix.Where(kv => kv.Value % 2 == 0).Select(kv => (kv.Key.Row, kv.Key.Col));
        var areaPlots = plots.Where(p => p.Row >= minRow && p.Row < maxRow && p.Col >= minCol && p.Col < maxCol);
        if (print)
        {
            int size = maxRow - minRow;
            char[,] mm = new char[size, size];
            foreach (var p in mm.ForEachIndex())
                mm[p.Row, p.Col] = '.';//lines[p.Row % Rows][p.Col % Cols];


            foreach (var (r, c) in areaPlots)
                mm[r - minRow, c - minCol] = 'O';

            mm.Print();
        }
        return areaPlots.Count();
    }

    public long CountBigTiles(int steps, (int Row, int Col) start)
    {
        var memo = new Dictionary<(int, int, int), long>();
        int bigSquareSize = Rows * 3; // 3x3

        int bigRows = (int)Math.Ceiling((decimal)steps / bigSquareSize) * 2 + 1;
        int bigMidRow = bigRows / 2;
        long totalPlots = 0;
        long maxEvenPlotsInSq = 0;
        long maxOddProtsInSq = 0;

        for (int bigRow = 0; bigRow < bigRows; ++bigRow)
        {
            long rowTotal = 0;
            if (bigRow % 10000 == 0)
                Console.WriteLine($"{bigRow}/{bigRows} - {totalPlots}");

            for (int bigCol = bigMidRow - bigRows - 1; bigCol <= bigMidRow + bigRows; ++bigCol)
            {
                P sqMaxPoint = FindEntryPoint(bigSquareSize, bigMidRow, bigRow, bigCol);
                var r = (bigRow - bigMidRow) * bigSquareSize - Rows;
                var c = (bigCol - bigMidRow) * bigSquareSize - Cols;
                (int Row, int Col) startPoint = (sqMaxPoint.Row + r, sqMaxPoint.Col + c);
                int sqSteps = steps - Math.Abs(start.Row - startPoint.Row) - Math.Abs(start.Row - startPoint.Col);
                if (sqSteps > bigSquareSize + bigSquareSize)
                {
                    bool isEven = sqSteps % 2 == 0;
                    if (maxEvenPlotsInSq == 0)
                    {
                        maxOddProtsInSq = CountTiles(isEven ? sqSteps + 1 : sqSteps, startPoint, r, r + bigSquareSize, c, c + bigSquareSize);
                        maxEvenPlotsInSq = CountTiles(isEven ? sqSteps : sqSteps + 1, startPoint, r, r + bigSquareSize, c, c + bigSquareSize);
                    }

                    int t = bigRows - bigCol - bigCol;
                    int even = isEven ? t / 2 + 1 : t / 2;

                    rowTotal += maxEvenPlotsInSq * even + maxOddProtsInSq * (t - even);
                    bigCol += t - 1;
                }
                else
                {
                    if (sqSteps >= 0)
                    {
                        if (memo.TryGetValue((sqMaxPoint.Row, sqMaxPoint.Col, sqSteps), out long count))
                        {
                            rowTotal += count;
                            //Console.WriteLine("Cool");
                            continue;

                        }

                        long tiles = CountTiles(sqSteps, startPoint, r, r + bigSquareSize, c, c + bigSquareSize, false);
                        //Console.WriteLine((rr, cc) + ": "  + tiles);
                        rowTotal += tiles;
                        memo[(sqMaxPoint.Row, sqMaxPoint.Col, sqSteps)] = tiles;
                        //totals[rr, cc] = ((int)tiles, sqSteps);
                    }
                    else if (rowTotal > 0)
                        break;

                }
            }
            totalPlots += rowTotal;
        }
        long p2 = totalPlots;
        return p2;

        static P FindEntryPoint(int squareSize, int mid, int bigRow, int bigCol)
        {
            int row = bigRow < mid ? squareSize - 1 : bigRow == mid ? squareSize / 2 : 0;
            /*int GetEntry(int x) => x - mid switch
            {
                0 => squareSize / 2,
                <0 =>squareSize - 1,
                >0 => 0
            };*/
            int col = bigCol < mid ? squareSize - 1 : bigCol == mid ? squareSize / 2 : 0;
            return (row, col);
        }
    }

    Queue<(int Row, int Col, int Steps)> next = new();

    Dictionary<(int Row, int Col), int> memo = [];
    public Dictionary<(int Row, int Col), int> FindPlots(int steps, (int Row, int Col) start)
    {
        next.Clear();
        memo.Clear();

        next.Enqueue((start.Row, start.Col, steps));
        memo[(start.Row, start.Col)] = steps;
        while (next.Count > 0)
        {
            var cur = next.Dequeue();

            if (cur.Steps == 0)
            {
                continue;
            }

            var directions = GetDirections(cur.Item1, cur.Item2)
                .Where(c => lines[((c.Row % Rows) + Rows) % Rows][((c.Col % Cols) + Cols) % Cols] != '#'
                   // && !memo.ContainsKey((c.Row, c.Col))
                   );

            foreach (var d in directions)
            {
                if (!memo.TryGetValue((d.Row, d.Col), out steps) || steps < cur.Item3 - 1)
                {
                    memo[(d.Row, d.Col)] = cur.Item3 - 1;
                    next.Enqueue((d.Row, d.Col, cur.Item3 - 1));
                }
            }

        }

        return memo;
    }

    private (int Row, int Col)[] GetDirections(int curRow, int curCol)
        => [(curRow - 1, curCol), (curRow + 1, curCol), (curRow, curCol - 1), (curRow, curCol + 1)];

}