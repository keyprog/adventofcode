using static System.Linq.Enumerable;

static class ConsoleExtensions
{
    public static T Print<T>(this T value)
    {
        Console.WriteLine(value);
        return value;
    }
}

static class CollectionsExtensions
{
    public static IEnumerable<T> PrintLines<T>(this IEnumerable<T> collection)
        => Print<T>(collection, '\n');

    public static IEnumerable<T> Print<T>(this IEnumerable<T> collection)
        => Print<T>(collection, ',');

    public static IEnumerable<T> Print<T>(this IEnumerable<T> collection, char separator)
    {
        Console.WriteLine(string.Join(separator, collection));
        return collection;
    }
}

static class StringsCollectionExtensions
{
    public static IEnumerable<(int Row, int Col, char Value)> ScanRows(this string[] collection)
        => from r in Range(0, collection.Length)
           from c in Range(0, collection[0].Length)
           select (r, c, collection[r][c]);
    public static IEnumerable<(int Row, int Col, char Value)> ScanRowsReverse(this string[] collection)
        => from r in Range(0, collection.Length)
           from c in Range(0, collection[0].Length)
           let rr = collection.Length - r - 1
           let cc = collection[0].Length - c - 1
           select (rr, cc, collection[rr][cc]);
}

static class Collections2DExtensions
{
    public static IEnumerable<(int Row, int Col, T Value)> ForEach<T>(this T[,] collection)
    => from r in Range(0, collection.GetLength(0))
       from c in Range(0, collection.GetLength(1))
       select (r, c, collection[r, c]);

    public static IEnumerable<(int Row, int Col)> ForEachIndex<T>(this T[,] collection)
    => from r in Range(0, collection.GetLength(0))
       from c in Range(0, collection.GetLength(1))
       select (r, c);

    public static T[,] Print<T>(this T[,] collection)
    {
        for (int r = 0; r < collection.GetLength(0); ++r)
        {
            for (int c = 0; c < collection.GetLength(1); ++c)
            {
                Console.Write(collection[r, c]);
                //Console.Write(' ');
            }
            Console.WriteLine();
        }
        return collection;
    }
}

public static class JaggedArrayExtensions
{
    public static IEnumerable<(int Row, int Col, T Value)> ForEach<T>(this T[][] collection)
        => from r in Range(0, collection.Length)
           from c in Range(0, collection[0].Length)
           select (r, c, collection[r][c]);

    public static T[][] Print<T>(this T[][] collection)
    {
        for (int r = 0; r < collection.Length; ++r)
        {
            for (int c = 0; c < collection[0].Length; ++c)
            {
                Console.Write(collection[r][c]);
                Console.Write(' ');
            }
            Console.WriteLine();
        }
        return collection;
    }
}

public static class AreaExtensions
{
    public static decimal CalculatePolygonArea(this (long Row, long Col)[] points)
    {
        int count = points.Length;
        decimal area = 0;
        decimal perimeter = 0;

        for (int i = 0; i < count; i++)
        {
            int j = (i + 1) % count;

            area += (points[i].Row + points[j].Row) * (points[i].Col - points[j].Col);
            perimeter += (decimal)Math.Sqrt((points[i].Row - points[j].Row) * (points[i].Row - points[j].Row)
            + (points[i].Col - points[j].Col) * (points[i].Col - points[j].Col));
        }
        area = Math.Abs(area) / 2m + perimeter / 2m + 1;
        return area;
    }
}