namespace ic;

enum ParamMode { Reference = 0, Value = 1, Relative = 2 }

struct Param
{
    private readonly ICMemory memory;
    private readonly long paramValue;
    private readonly ParamMode mode;
    private readonly ICCpu cpu;
    public Param(ICMemory memory, ICCpu cpu, long value, ParamMode mode)
    {
        this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
        this.cpu = cpu ?? throw new ArgumentNullException(nameof(cpu));
        this.paramValue = value;
        this.mode = mode;
    }

    public static Param Load(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
        => new Param(memory, cpu, cpu.Ld(memory), modes.Load());

    public static (Param, Param) Load2(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
        => (Load(memory, cpu, modes), Load(memory, cpu, modes));

    public static (Param, Param, Param) Load3(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
        => (Load(memory, cpu, modes), Load(memory, cpu, modes), Load(memory, cpu, modes));

    public long Get()
    {
        return mode switch
        {
            ParamMode.Reference => memory[paramValue],
            ParamMode.Value => paramValue,
            ParamMode.Relative => memory[paramValue + cpu.RB],
            _ => throw new NotSupportedException()
        };
    }

    public void Set(long value)
    {
        switch (mode)
        {
            case ParamMode.Value:
                throw new ApplicationException("Set doesn't support value mode");
            case ParamMode.Reference:
                memory[paramValue] = value;
                break;
            case ParamMode.Relative:
                memory[paramValue + cpu.RB] = value;
                break;
            default:
                throw new NotSupportedException();
        }
    }
}

static class ParamModeExtensions
{
    public static ParamMode Load(this Queue<ParamMode> q)
       => q.TryDequeue(out ParamMode mode) ? mode : ParamMode.Reference;
}

interface ICOperation
{
    void Exec(ICComp comp);
}

record SumOp((Param p1, Param p2, Param resAddr) pars) : ICOperation
{
    public void Exec(ICComp c)
    {
        pars.resAddr.Set(pars.p1.Get() + pars.p2.Get());
    }
}

record MultiplyOp((Param p1, Param p2, Param resAddr) pars) : ICOperation
{
    public void Exec(ICComp c)
    {
        pars.resAddr.Set(pars.p1.Get() * pars.p2.Get());
    }
}

record HaltOp : ICOperation
{
    public void Exec(ICComp c) => c.Cpu.State = ICCompState.Terminated;
}

record InputOp(Param destAddr) : ICOperation
{
    public void Exec(ICComp c) => destAddr.Set(c.Input.Read());
}

record OutputOp(Param srcAddr) : ICOperation
{
    public void Exec(ICComp c) => c.Output.Write(srcAddr.Get());
}

record JumpIfTrueOp((Param flag, Param jumpTo) pars) : ICOperation
{
    public void Exec(ICComp c)
    {
        if (pars.flag.Get() != 0)
            c.Cpu.IP = pars.jumpTo.Get();
    }
}

record JumpIfFalseOp((Param flag, Param jumpTo) pars) : ICOperation
{
    public void Exec(ICComp c)
    {
        if (pars.flag.Get() == 0)
            c.Cpu.IP = pars.jumpTo.Get();
    }
}

record LessThanOp((Param left, Param right, Param result) pars) : ICOperation
{
    public void Exec(ICComp c) => pars.result.Set(pars.left.Get() < pars.right.Get() ? 1 : 0);
}

record EqualsOp((Param left, Param right, Param result) pars) : ICOperation
{
    public void Exec(ICComp c) => pars.result.Set(pars.left.Get() == pars.right.Get() ? 1 : 0);
}

record RelativeBaseOffsetOp(Param offset) : ICOperation
{
    public void Exec(ICComp c) => c.Cpu.RB += offset.Get();
}