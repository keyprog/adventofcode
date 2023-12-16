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
    public static IEnumerable<(int Row, int Col, char Value)> ForEach(this string[] collection)
        => from r in Range(0, collection.Length)
           from c in Range(0, collection[0].Length)
           select (r, c, collection[r][c]);
}

static class Collections2DExtensions
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
    public static T[,] Print<T>(this T[,] collection)
    {
        for (int r = 0; r < collection.GetLength(0); ++r)
        {
            for (int c = 0; c < collection.GetLength(1); ++c)
            {
                Console.Write(collection[r, c]);
                Console.Write(' ');
            }
            Console.WriteLine();
        }
        return collection;
    }
}
partial class JaggedArray<T>(T[][] input)
{
    private T[][] data = input;
    public int RowsCount { get; } = input.Length;
    public int ColsCount { get; } = input[0].Length;

    public void Print() => data.Print();
}
