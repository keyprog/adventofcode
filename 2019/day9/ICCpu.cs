namespace ic;

enum ICCompState { Running, Terminated };
class ICCpu
{
    public long IP { get; set; } = 0;
    public long RB { get; set; } = 0;
    public ICCompState State { get; set; }

    /// Loads value from Memory
    public long Ld(ICMemory memory) => memory[IP++];
}
