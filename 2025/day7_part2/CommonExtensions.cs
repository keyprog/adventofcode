using System.Text.Json;
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
        => PrintCollection<T>(collection, '\n');

    public static IEnumerable<T> PrintCollection<T>(this IEnumerable<T> collection)
        => PrintCollection<T>(collection, ',');

    public static IEnumerable<T> PrintCollection<T>(this IEnumerable<T> collection, char separator)
    {
        Console.WriteLine(string.Join(separator, collection));
        return collection;
    }
}

static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue[]> Print<TKey, TValue>(this Dictionary<TKey, TValue[]> dictionary) where TKey: notnull
    {
        foreach ((TKey key, TValue[] value) in dictionary)
            Console.WriteLine($"{key} => {string.Join(", ", value)}");
        return dictionary;
    }

    public static Dictionary<TKey, TValue> Print<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        foreach ((TKey key, TValue value) in dictionary)
            Console.WriteLine($"{key} => {value}");
        return dictionary;
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

    public static char[,] ToCharArray2D(this string[] collection)
    {
        char[,] array = new char[collection.Length, collection[0].Length];
        foreach (var (r, c, y) in collection.ScanRows())
            array[r, c] = y;
        return array;
    }

    public static int[,] ToIntArray2D(this string[] collection)
    {
        int width = collection[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
        int[,] array = new int[collection.Length, width];
        for (int i = 0; i < collection.Length; ++i)
        {
            var tokens = collection[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < width; ++j)
            {
                array[i, j] = int.Parse(tokens[j]);
            }
        }
        return array;
    }
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
                Console.Write(' ');
            }
            Console.WriteLine();
        }
        return collection;
    }

    public static IEnumerable<T> ScanColumn<T>(this T[,] collection, int col)
    {
        for(int row = 0; row < collection.GetLength(0); ++row)
            yield return collection[row, col];
    }
}

static class MatrixExtensions
{
    public static char[,] Print(this char[,] collection)
    {
        for (int r = 0; r < collection.GetLength(0); ++r)
        {
            for (int c = 0; c < collection.GetLength(1); ++c)
            {
                Console.Write(collection[r, c]);
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

public static class Graph
{
    // https://hediet.github.io/visualization/?darkTheme=1&theme=light    
    public static string ToJson(IEnumerable<Node> nodes, IEnumerable<Edge> edges)
        => JsonSerializer.Serialize(new GraphVis(nodes.ToArray(), edges.ToArray()), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

    public class GraphVis(Node[] nodes, Edge[] edges)
    {
        public Kind Kind { get; } = new Kind();
        public Node[] Nodes { get; } = nodes;
        public Edge[] Edges { get; } = edges;
    }

    public class Kind
    {
        public bool Graph { get => true; }
    }

    public class Node(string name, string color = "green")
    {
        public string Id { get; } = name;
        public string Label { get; } = name;
        public string Color { get; } = color;
    }

    public record Edge(string From, string To);
}