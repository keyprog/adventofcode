namespace ic;

class ICComp
{
    public ICCpu Cpu { get; private set; }
    public ICMemory Memory { get; private set; }
    public ICStream Input { get; private set; }
    public ICStream Output { get; private set; }
    private ICDecoder decoder = new();

    private ICComp(ICMemory memory, ICStream? input, ICStream? output)
    {
        this.Memory = memory ?? throw new ArgumentNullException(nameof(memory));
        this.decoder = new ICDecoder();
        this.Cpu = new ICCpu { IP = 0, State = ICCompState.Running };
        this.Input = input ?? new ICStream();
        this.Output = output ?? new ICStream();
    }

    public static ICComp Load(long[] program, ICStream? input = null, ICStream? output = null)
    {
        var memory = new ICMemory(program);
        return new ICComp(memory, input, output);
    }

    public ICComp Execute()
    {
        while (ExecuteOne() != ICCompState.Terminated) ;
        return this;
    }

    public ICCompState ExecuteOne()
    {
        if (Cpu.State == ICCompState.Terminated)
            throw new ApplicationException("IntCode computer program terminated");

        ICOperation op = decoder.Decode(Cpu, Memory);
        op.Exec(this);
        return this.Cpu.State;
    }
}