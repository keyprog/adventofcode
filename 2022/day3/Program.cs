using System.Text;
string inputFile = "input2.txt";
var sacks = File.ReadLines(inputFile);

Console.WriteLine("Total priority:" + sacks.Select(GetSackPriority).Sum());

var badges = from g in sacks.Chunk(3)
                 select g.Length == 3 ? GetGroupBadge(g[0], g[1], g[2]) : throw new ApplicationException("Not enough lines");

Console.WriteLine("Total badges: " + badges.Sum());

int GetGroupBadge(string e1, string e2, string e3)
{
    ulong c1 = GetSackItemsFlags(e1);
    ulong c2 = GetSackItemsFlags(e2);
    ulong c3 = GetSackItemsFlags(e3);
    ulong common = c1 & c2 & c3;
    return GetTotalPriority(common);
}

// Bit flags, if bit is set, means the item with priority is in the sack
ulong GetSackItemsFlags(ReadOnlySpan<char> sack)
{
    ulong flags = 0;
    for (int i = 0; i < sack.Length; ++i)
        SetFlag(ref flags, GetItemPriority(sack[i]) - 1);
    return flags;
}

void SetFlag(ref ulong flags, int bitIndex) => flags |= (1ul << bitIndex);

int GetSackPriority(string backpack)
{
    int backpackSize = backpack.Length;
    if (backpackSize % 2 != 0)
        throw new ArgumentException("Unevent backpack size");

    ulong comp1 = GetSackItemsFlags(backpack.AsSpan(0, backpackSize / 2));
    ulong comp2 = GetSackItemsFlags(backpack.AsSpan(backpackSize / 2));
    ulong comps = comp1 & comp2;
    //PrintBits(comps);
    //Console.WriteLine("Priority: " + GetTotalPriority(comps));
    return GetTotalPriority(comps);
}

int GetItemPriority(char itemCode)
{
    if (itemCode >= 'a' && itemCode <= 'z')
        return itemCode - 'a' + 1;
    return itemCode - 'A' + 27;
}

void PrintBits(ulong flags)
{
    StringBuilder sb = new StringBuilder(512);
    for (int i = 0; i < 64; ++i)
        sb.Append($"[{i},{((flags & (1ul << i)) > 0 ? 1 : 0)}],");
    sb.Length -= 1;
    Console.WriteLine(sb);
}

int GetTotalPriority(ulong itemsFlags)
{
    int totalPriority = 0;
    for (int i = 0; i < 52; ++i)
        totalPriority += (itemsFlags & 1ul << i) == 0 ? 0 : i + 1;
    return totalPriority;
}
