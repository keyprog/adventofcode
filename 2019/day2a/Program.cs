const string sourceFile = "program2.txt";
var memory = new MemorySpace(File.ReadAllText(sourceFile).Split(',').Select(int.Parse));
memory.Dump(Console.Out);
memory[1] = 12;
memory[2] = 2;
IntCodeInterpreter interpreter = new();

while (interpreter.Decode(memory).Exec(memory))
{

}
Console.WriteLine($"Answer: {memory[0]}");
memory.Dump(Console.Out);

class MemorySpace
{
    private readonly List<int> memory;
    public MemorySpace(IEnumerable<int> program)
    {
        memory = new List<int>(program);
    }

    public int this[int offset]
    {
        get
        {
            if (offset < memory.Count)
                return memory[offset];
            return 0;
        }
        set
        {
            if (offset > memory.Count)
                memory.AddRange(Enumerable.Repeat(0, (offset - memory.Count >> 10) << 10));
            memory[offset] = value;
        }
    }

    public int InstructionsOffset { get; set; } = 0;

    public void Dump(TextWriter writer)
    {
        for (int i = 0; i < memory.Count; ++i)
        {
            if (i > 0)
                writer.Write(',');
            writer.Write(memory[i].ToString());

        }
        writer.WriteLine();
    }

}

interface IOperation
{
    bool Exec(MemorySpace memory);
}

record Sum(int add1Offset, int add2Offset, int resultOffset) : IOperation
{
    public bool Exec(MemorySpace memory)
    {
        memory[resultOffset] = memory[add1Offset] + memory[add2Offset];
        return true;
    }
}

record Multiply(int fact1Offset, int fact2Offset, int productOffset) : IOperation
{
    public bool Exec(MemorySpace memory)
    {
        memory[productOffset] = memory[fact1Offset] * memory[fact2Offset];
        return true;
    }
}

record Halt : IOperation
{
    public bool Exec(MemorySpace memory) => false;
}



class IntCodeInterpreter
{
    public IOperation Decode(MemorySpace memory)
    {
        int offset = memory.InstructionsOffset;
        int opCode = memory[offset];
        switch (opCode)
        {
            case 1:
                memory.InstructionsOffset += 4;
                return new Sum(memory[offset + 1], memory[offset + 2], memory[offset + 3]);
            case 2:
                memory.InstructionsOffset += 4;
                return new Multiply(memory[offset + 1], memory[offset + 2], memory[offset + 3]);
            case 99:
                memory.InstructionsOffset += 1;
                return new Halt();
            default:
                throw new NotSupportedException();
        };
    }
}
