Hand[] hands = InputParser.Parse(File.ReadAllLines("input.txt")).ToArray();

Array.Sort(hands, new HandsComparer());

int result = hands.Select((h, i) => (i + 1) * h.Bid).Sum();
Console.WriteLine(result);

enum HandType
{
    FiveOfAKind = 7, // where all five cards have the same label: AAAAA
    FourOfAKind = 6, // where four cards have the same label and one card has a different label: AA8AA
    FullHouse = 5, // where three cards have the same label, and the remaining two cards share a different label: 23332
    ThreeOfAKind = 4, // where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
    TwoPair = 3, // where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
    OnePair = 2, // where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
    HighCard = 1, // where all cards' labels are distinct: 23456
}

class HandsComparer : IComparer<Hand>
{
    public int Compare(Hand? x, Hand? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        if (x.Type != y.Type)
            return x.Type.CompareTo(y.Type);

        for (int i = 0; i < x.Cards.Length; ++i)
        {
            int diff = x.GetCardStrength(i).CompareTo(y.GetCardStrength(i));
            if (diff != 0)
                return diff;
        }
        return 0;
    }
}
class InputParser
{
    public static IEnumerable<Hand> Parse(string[] lines)
    {
        foreach (ReadOnlySpan<char> line in lines)
        {
            string cards = line[..5].ToString();
            yield return new Hand
            (
                Cards: line[..5].ToString(),
                Bid: int.Parse(line[6..]),
                Type: GetHandTypeWithWildcard(cards.ToCharArray())
            );
        }
    }

    private static HandType GetHandTypeWithWildcard(char[] cards, char wildcard = 'J')
    {
        HandType bestType = GetHandType(cards);
        if (bestType == HandType.FiveOfAKind || cards.All(c => c != wildcard))
            return bestType;

        foreach (char c in cards.Distinct().Where(c => c != wildcard))
        {
            HandType type = GetHandType(string.Concat(cards).Replace(wildcard, c).ToCharArray());
            if (type == HandType.FiveOfAKind) return type;
            if (type > bestType)
                bestType = type;
        }
        return bestType;
    }

    private static HandType GetHandType(char[] cards)
    {
        var grouped = cards.GroupBy(c => c).ToArray();
        int maxOfAKind = grouped.Max(g => g.Count());
        return (grouped.Length, maxOfAKind) switch
        {
            (1, _) => HandType.FiveOfAKind,
            (2, 4) => HandType.FourOfAKind,
            (2, _) => HandType.FullHouse,
            (3, 3) => HandType.ThreeOfAKind,
            (3, _) => HandType.TwoPair,
            (4, _) => HandType.OnePair,
            _ => HandType.HighCard
        };
    }

}

record Hand(string Cards, int Bid, HandType Type)
{
    public int GetCardStrength(int i) => GetCardStrength(Cards[i]);

    public static int GetCardStrength(char c)
    => c switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => 1,
        'T' => 10,
        '9' => 9,
        '8' => 8,
        '7' => 7,
        '6' => 6,
        '5' => 5,
        '4' => 4,
        '3' => 3,
        '2' => 2,
        _ => throw new NotSupportedException()
    };

}