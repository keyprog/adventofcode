const string sourceFile = "program2.txt";
const int ExpectedOutput = 19690720;

var code = File.ReadAllText(sourceFile).Split(',').Select(int.Parse);
int noun = 0;
int verb = 0;

IntCodeInterpreter interpreter = new();
for (noun = 0; noun < 100; ++noun)
{
    for (verb = noun; verb < 100; ++verb)
    {
        MemorySpace mem = new(code);
        mem[1] = noun;
        mem[2] = verb;
        foreach (var op in interpreter.DecodeAll(mem)) op.Exec(mem);

        if (mem[0] == ExpectedOutput)
        {
            Console.WriteLine($"Answer: {100 * noun + verb}");
            return;
        }
    }
}
Console.WriteLine("Unable to find expected output");

class MemorySpace
{
    private int[] memory;
    public MemorySpace(IEnumerable<int> program)
    {
        memory = program.ToArray();
    }

    public int this[int address]
    {
        get
        {
            if (address < memory.Length)
                return memory[address];
            return 0;
        }
        set
        {
            if (address > memory.Length)
            {
                memory = Expand(memory, address);
            }
            memory[address] = value;
        }
    }

    private static int[] Expand(int[] memory, int newCapacity)
    {
        int newSize = ((newCapacity >> 10) + 1) << 10;
        int[] newMemory = new int[newSize];
        memory.CopyTo(newMemory.AsMemory());
        return newMemory;
    }

    public int InstrPtr { get; set; } = 0;

    public void Dump(TextWriter writer)
    {
        for (int i = 0; i < memory.Length; ++i)
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
    public IEnumerable<IOperation> DecodeAll(MemorySpace memory)
    {
        while (true)
        {
            IOperation op = Decode(memory);
            if (op is Halt)
                yield break;
            yield return op;
        }
    }

    public IOperation Decode(MemorySpace memory)
    {
        int offset = memory.InstrPtr;
        int opCode = memory[offset];
        switch (opCode)
        {
            case 1:
                memory.InstrPtr += 4;
                return new Sum(memory[offset + 1], memory[offset + 2], memory[offset + 3]);
            case 2:
                memory.InstrPtr += 4;
                return new Multiply(memory[offset + 1], memory[offset + 2], memory[offset + 3]);
            case 99:
                memory.InstrPtr += 1;
                return new Halt();
            default:
                throw new NotSupportedException();
        };
    }
}
