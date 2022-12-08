const string FileName = "input3.txt";
byte[] stream = File.ReadAllBytes(FileName);
Console.WriteLine("Stream starts @ " + FindStreamStart(stream, tokenLength: 14));

int FindStreamStart(ReadOnlySpan<byte> stream, int tokenLength = 4)
{
    if (stream.Length < tokenLength)
        return -1;

    for (int i = tokenLength; i < stream.Length; ++i)
    {
        switch (FindDup(stream[(i - tokenLength)..i]))
        {
            case < 0: return i; // no dups, found the stream start token
            case var dupPos: i += dupPos; break;
        }
    }
    return -1;
}

/// Returns index of first byte which has a duplicate
int FindDup(ReadOnlySpan<byte> arr)
{
    for (int i = 0; i < arr.Length - 1; ++i)
    {
        for (int j = i + 1; j < arr.Length; ++j)
        {
            if (arr[i] == arr[j])
                return i;
        }
    }
    return -1;
}
