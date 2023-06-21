namespace ic.Comp.Devices;

class ICMemory
{
    private long[] memory;
    public ICMemory(IEnumerable<long> code)
    {
        memory = code.ToArray();
    }

    public long this[long address]
    {
        get
        {
            if(address < 0) throw new ApplicationException();
            if (address < memory.Length)
                return memory[address];
            return 0;
        }
        set
        {
            if (address < 0 ) throw new ApplicationException();
            if (address >= memory.Length)
            {
                memory = Expand(memory, address);
            }
            try{
            memory[address] = value;
            }catch(Exception)
            {
                throw new ApplicationException($"Address {address}");
            }
        }
    }

    private static long[] Expand(long[] memory, long newCapacity)
    {
        long newSize = ((newCapacity >> 10) + 1) << 10;
        long[] newMemory = new long[newSize];
        memory.CopyTo(newMemory.AsMemory());
        return newMemory;
    }

    public void Dump(TextWriter writer)
    {
        for (int i = 0; i < memory.Length; ++i)
        {
            if (i > 0) writer.Write(',');
            writer.Write(Convert.ToString(memory[i]));
        }
        writer.WriteLine();
    }

}
