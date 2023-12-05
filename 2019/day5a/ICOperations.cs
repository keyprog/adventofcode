namespace ic;
enum ParamMode { Ref, Val }
struct Param
{
    private readonly ICMemory memory;
    private readonly int addr;
    private readonly ParamMode mode;
    public Param(ICMemory memory, int addr, ParamMode mode)
    {
        this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
        this.addr = addr;
        this.mode = mode;
    }

    public static Param Ld(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
    {
        return new Param(memory, cpu.Ld(memory), modes.Ld());
    }

    public static (Param, Param) Ld2(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
    {
        return (Ld(memory, cpu, modes), Ld(memory, cpu, modes));
    }

    public static (Param, Param, Param) Ld3(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
    {
        return (Ld(memory, cpu, modes), Ld(memory, cpu, modes), Ld(memory, cpu, modes));
    }

    public int Get()
    {
        return mode switch
        {
            ParamMode.Ref => memory[addr],
            ParamMode.Val => addr,
            _ => throw new NotSupportedException()
        };
    }

    public void Set(int value)
    {
        if (mode != ParamMode.Ref)
            throw new ApplicationException("Set only support Ref mode");
        memory[addr] = value;
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
    public void Exec(ICComp c) =>  pars.result.Set(pars.left.Get() == pars.right.Get() ? 1 : 0);    
}
