using System.Numerics;

var banks = File.ReadAllLines("/home/alexp/dev/adventofcode/2025/day3/input.txt");
BigInteger maxJoltage = 0;
foreach(BigInteger v in banks.Select(b => GetMaxJoltage(b.AsSpan())))
{
    maxJoltage += v;
}
maxJoltage.Print();

BigInteger GetMaxJoltage(ReadOnlySpan<char> bank)
{
    int totalCharacters = 12;    
    int start = 0;
    int[] maxChars = new int[totalCharacters];

    for(int charNum = 0; charNum < totalCharacters; ++charNum)
    {
        maxChars[charNum] = GetIndexOfMax(bank[start..^(totalCharacters - charNum - 1)]) + start;
        start = maxChars[charNum] + 1;
    
    }
    BigInteger joltage = 0;
    BigInteger power = 1;
    for(int i = 0; i < totalCharacters; ++i)
    {
        joltage+=(bank[maxChars[totalCharacters - i - 1]] - '0') * power;
        power*=10;
        Console.Write(bank[maxChars[totalCharacters - i - 1]] );
    }
    Console.WriteLine();
    return joltage;
}
int GetIndexOfMax(ReadOnlySpan<char> span)
{
    int maxIndex = 0;
    char maxChar = span[0];
    for(int i = 1; i < span.Length; ++i)
    {
        if (span[i] > maxChar)
        {
            maxIndex = i;
            maxChar = span[i];
        }
    }
    return maxIndex;
}
