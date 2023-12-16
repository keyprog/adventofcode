var hashmap = new HashMap(256);

foreach (var (box, lense, op) in Parser.ParseCommands(File.ReadAllText("input.txt")))
{
    switch (op)
    {
        case Operation.Add:
            hashmap.Add(box, lense);
            break;
        case Operation.Remove:
            hashmap.Remove(box, lense.Label);
            break;
    }
}

hashmap.Print()
    .GetCheckSum().Print();

class Parser()
{
    public static IEnumerable<(int Box, Lense Lense, Operation Op)> ParseCommands(string input)
    {
        char[] ops = ['-', '='];
        foreach (var token in input.Split(','))
        {
            int opIndex = token.IndexOfAny(ops);
            string label = token[0..opIndex];
            Operation operation = token[opIndex] switch
            {
                '=' => Operation.Add,
                '-' => Operation.Remove,
                _ => throw new NotSupportedException()
            };
            int hashcode = GetHashCodeFor(label);
            int focus = operation == Operation.Add ? int.Parse(token[(opIndex + 1)..]) : 0;
            yield return (hashcode, new Lense(label, focus), operation);
        }
    }

    public static int GetHashCodeFor(string word)
    {
        int current = 0;
        for (int i = 0; i < word.Length; ++i)
        {
            //Determine the ASCII code for the current character of the string.
            byte b = (byte)word[i];
            //Increase the current value by the ASCII code you just determined.
            current += b;
            //Set the current value to itself multiplied by 17.
            current *= 17;
            //Set the current value to the remainder of dividing itself by 256.
            current %= 256;
        }
        return current;
    }
}

class HashMap
{
    private readonly List<Lense>[] boxes;

    public HashMap(int bucketsSize)
    {
        boxes = new List<Lense>[bucketsSize];
        for (int i = 0; i < boxes.Length; ++i)
            boxes[i] = [];
    }

    public void Add(int boxNum, Lense lense)
    {
        List<Lense> lenses = boxes[boxNum];
        int ind = lenses.FindIndex(l => l.Label == lense.Label);
        if (ind >= 0)
        {
            lenses[ind] = lense;
        }
        else
        {
            lenses.Add(lense);
        }
    }

    public void Remove(int boxNum, string label)
    {
        List<Lense> lenses = boxes[boxNum];
        int index = lenses.FindIndex(l => l.Label == label);
        if (index >= 0)
            lenses.RemoveAt(index);
    }

    public long GetCheckSum()
    {
        long checksum = 0;
        for (int i = 0; i < boxes.Length; ++i)
        {
            for (int l = 0; l < boxes[i].Count; ++l)
            {
                checksum += (i + 1) * (l + 1) * boxes[i][l].Focus;
            }
        }
        return checksum;
    }

    public HashMap Print()
    {
        for (int i = 0; i < boxes.Length; ++i)
        {
            if (boxes[i].Count > 0)
                Console.WriteLine($"Box {i}: " + string.Join(' ', boxes[i].Select(l => $"[{l.Label} {l.Focus}]")));
        }
        return this;
    }
}

record struct Lense(string Label, int Focus);

enum Operation { Add, Remove }
