static class CommonExtensions
{
    public static void Print<T>(this IEnumerable<T> collection) => Console.WriteLine(string.Join(',', collection));
}