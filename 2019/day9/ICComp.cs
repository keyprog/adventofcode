namespace ic;

class ICComp
{
    public ICCpu Cpu { get; init; }
    public ICMemory Memory { get; init; }
    public ICStream Input { get; init; }
    public ICStream Output { get; init; }
    public ICDecoder Decoder { get; init; }

    private ICComp(ICCpu cpu, ICMemory memory, ICStream input, ICStream output, ICDecoder decoder)
    {
        this.Cpu = cpu;
        this.Memory = memory;
        this.Decoder = decoder;
        this.Input = input;
        this.Output = output;
    }

    public static ICComp Load(long[] code, ICStream? input = null, ICStream? output = null,
                                ICDecoder? decoder = null)
        => new ICComp(new ICCpu { IP = 0, State = ICCompState.Running }, new ICMemory(code),
                      input ?? new ICStream(), output ?? new ICStream(),
                      decoder ?? new ICDecoder());


    public ICComp Execute()
    {
        while (this.Cpu.State != ICCompState.Terminated)
            ExecuteOne();

        return this;
    }

    public ICComp ExecuteOne()
    {
        if (Cpu.State == ICCompState.Terminated)
            throw new ApplicationException("IntCode computer program terminated");

        ICOperation op = Decoder.Decode(Cpu, Memory);
        op.Exec(this);
        return this;
    }
}