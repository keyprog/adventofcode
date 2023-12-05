readonly record struct Card(int Id, int[] WinningNumbers, int[] Numbers)
{
    public int GetScore()
    {
        int winCount = GetWinCount();
        int score = winCount == 0 ? 0 : 1 << (winCount - 1);
        return score;
    }

    public int GetWinCount()
    {
        var winning = new HashSet<int>(WinningNumbers);
        int winningCount = Numbers.Count(n => winning.Contains(n));
        return winningCount;
    }
}