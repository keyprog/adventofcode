string inputFile = "input2.txt";
var rr = File.ReadLines(inputFile).Select(l => ParseLine(l));

Console.WriteLine("Contains: " + rr.Count(r => Range.Contain(r.Item1, r.Item2)));
Console.WriteLine("Overlaps: " + rr.Count(r => Range.Overlap(r.Item1, r.Item2)));

(Range, Range) ParseLine(System.ReadOnlySpan<char> span)
{
    int sepInd = span.IndexOf(',');
    if (sepInd <= 0 || sepInd == span.Length - 1) throw new ApplicationException("Invalid input");
    return (Parse(span[..sepInd]), Parse(span[++sepInd..]));
}

Range Parse(System.ReadOnlySpan<char> span)
{
    int sepInd = span.IndexOf('-');
    if (sepInd <= 0 || sepInd == span.Length - 1) throw new ApplicationException("Invalid input");
    return new Range(int.Parse(span[..sepInd]), int.Parse(span[++sepInd..]));
}

record struct Range(int Start, int End)
{
    public bool Contains(Range otherRange) => this.Start <= otherRange.Start && this.End >= otherRange.End;

    public static bool Contain(Range r1, Range r2) => r1.Contains(r2) || r2.Contains(r1);

    public bool Overlaps(Range otherRange)
    {
        return this.Start <= otherRange.Start && this.End >= otherRange.Start;
    }

    public static bool Overlap(Range r1, Range r2) => r1.Overlaps(r2) || r2.Overlaps(r1);
}