sealed record CardsTable(string[] lines)
{
    private readonly string[] lines = lines;
    public int Rows { get; } = lines.Length;
    public int Cols { get; } = lines.Length;

    public IEnumerable<Card> GetCards()
    {
        foreach (var line in lines)
            yield return ParseCard(line);
    }

    private Card ParseCard(ReadOnlySpan<char> line)
    {
        int idStart = line.IndexOf(' ') + 1;
        int idEnd = line[idStart..].IndexOf(':') + idStart;
        int id = int.Parse(line[idStart..idEnd]);
        int numbersSeparator = line.IndexOf('|');
        int[] winningNumbers = ParseNumbers(line[(idEnd + 1)..numbersSeparator]);
        int[] numbers = ParseNumbers(line[(numbersSeparator + 1)..]);

        return new Card(id, WinningNumbers: winningNumbers, Numbers: numbers);
    }

    private int[] ParseNumbers(ReadOnlySpan<char> numbersString)
    {
        Span<Range> tokens = stackalloc Range[numbersString.Length / 2];
        int tokensCount = numbersString.Split(tokens, ' ', StringSplitOptions.RemoveEmptyEntries);
        int[] numbers = new int[tokensCount];
        for (int i = 0; i < tokensCount; ++i)
            numbers[i] = int.Parse(numbersString[tokens[i]]);
        return numbers;
    }
}