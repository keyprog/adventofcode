namespace ic;
enum ParamMode { Ref, Val, Rel }
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

    public static Param Ld(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
    {
        return new Param(memory, cpu, cpu.Ld(memory), modes.Ld());
    }

    public static (Param, Param) Ld2(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
    {
        return (Ld(memory, cpu, modes), Ld(memory, cpu, modes));
    }

    public static (Param, Param, Param) Ld3(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
    {
        return (Ld(memory, cpu, modes), Ld(memory, cpu, modes), Ld(memory, cpu, modes));
    }

    public long Get()
    {
        return mode switch
        {
            ParamMode.Ref => memory[paramValue],
            ParamMode.Val => paramValue,
            ParamMode.Rel => memory[paramValue + cpu.RB],
            _ => throw new NotSupportedException()
        };
    }

    public void Set(long value)
    {
        switch (mode)
        {
            case ParamMode.Val:
                throw new ApplicationException("Set doesn't support value mode");
            case ParamMode.Ref:
                memory[paramValue] = value;
                break;
            case ParamMode.Rel:
                memory[paramValue + cpu.RB] = value;
                break;
            default:
                throw new NotSupportedException();
        }
    }
}

static class ParamModeExtensions
{
    public static ParamMode Ld(this Queue<ParamMode> q)
    {
        if (q.TryDequeue(out ParamMode mode))
            return mode;
        return ParamMode.Ref;
    }
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
    public void Exec(ICComp c)
    {
        c.Cpu.State = ICCompState.Terminated;
    }
}

record InputOp(Param destAddr) : ICOperation
{
    public void Exec(ICComp c) => destAddr.Set(c.Input.Read());
}

record OutputOp(Param srcAddr) : ICOperation
{
    public void Exec(ICComp c)
    {
        c.Output.Write(srcAddr.Get());
    }
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