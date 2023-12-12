//long[] time = [34908986];
//long[] distance = [204171312101780];

int[] time = [7, 15, 30];
int[] distance = [9, 40, 200];

long result = 1;
for (int i = 0; i < time.Length; ++i)
{
    var (min, max) = GetWinningRound(time[i], distance[i]);
    long waysCount = max - min + 1;
    Console.WriteLine($"Time {time[i]}, distance {distance[i]}, min {min}, max {max}, total {waysCount}");
    result *= waysCount;
}

Console.WriteLine(result);

static (long min, long max) GetWinningRound(long time, long distance)
{
    double sq = Math.Sqrt(time * time - 4 * distance);
    double min = (time - sq) / 2d;
    double max = (time + sq) / 2d;
    return ((long)Math.Floor(min) + 1, (long)Math.Ceiling(max) - 1);
}
