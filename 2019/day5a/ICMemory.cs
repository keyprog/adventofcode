namespace ic;

class ICMemory
{
    private int[] memory;
    public ICMemory(IEnumerable<int> program)
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
