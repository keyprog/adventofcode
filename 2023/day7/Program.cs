Hand[] hands = File.ReadAllLines("input.txt").Select(InputParser.ParseHand).ToArray();

Array.Sort(hands, (x, y) => x.Type != y.Type ? x.Type.CompareTo(y.Type) : x.Strength.CompareTo(y.Strength));

int result = hands.Select((hand, i) => (i + 1) * hand.Bid).Sum();
Console.WriteLine(result);


record Hand(string Cards, int Bid, HandType Type, int Strength);

enum HandType { FiveOfAKind = 7, FourOfAKind = 6, FullHouse = 5, ThreeOfAKind = 4, TwoPair = 3, OnePair = 2, HighCard = 1 }

class InputParser
{
    public static Hand ParseHand(string line)
    {
        string cards = line[..5];
        return new Hand
        (
            Cards: cards,
            Bid: int.Parse(line.AsSpan()[6..]),
            Type: GetHandTypeWithWildcard(cards.ToCharArray()),
            Strength: GetStrength(cards)
        );
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

    private static int GetStrength(string cards) => cards.Aggregate(0, (s, c) => (s << 4) + GetCardStrength(c));

    public static int GetCardStrength(char c)
    => c switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => 1,
        'T' => 10,
        var num => num - '0'
    };
}
