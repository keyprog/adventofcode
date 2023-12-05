namespace ic;

enum ICCompState { Running, Terminated };
class ICCpu
{
    public int IP { get; set; } = 0;
    public ICCompState State { get; set; }

    /// Loads value from Memory
    public int Ld(ICMemory memory) => memory[IP++];
}
