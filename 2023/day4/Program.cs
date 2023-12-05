bool isTest = false;
var cardsTable = new CardsTable(File.ReadAllLines(isTest ? "input_test.txt" : "input.txt"));
Card[] cards = cardsTable.GetCards().ToArray();

foreach(var card in cards)
    Console.WriteLine($"{card.Id} => {string.Join(',',card.WinningNumbers)} | {string.Join(',', card.Numbers)}, score {card.GetScore()}");
int totalScore = cardsTable.GetCards().Sum(c => c.GetScore());
Console.WriteLine($"Part 1: {totalScore}");

// part 2
int[] copies = Enumerable.Repeat(1, cards.Length).ToArray();
for(int i = 0; i < copies.Length; ++i)
{
    int winCount = cards[i].GetWinCount();
    for(int j = i + 1; winCount > 0 && j < copies.Length; ++j)
    {
        copies[j] += copies[i];
        winCount--;
    }
}
int totalCards = copies.Sum();
Console.WriteLine($"Part 2: {totalCards}");