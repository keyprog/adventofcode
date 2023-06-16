namespace ic;

interface ICOperation
{
    void Exec(ICComp comp);
}

record SumOp((ICOpParam p1, ICOpParam p2, ICOpParam resAddr) pars) : ICOperation
{
    public void Exec(ICComp c)
    {
        pars.resAddr.Set(pars.p1.Get() + pars.p2.Get());
    }
}

record MultiplyOp((ICOpParam p1, ICOpParam p2, ICOpParam resAddr) pars) : ICOperation
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

record InputOp(ICOpParam destAddr) : ICOperation
{
    public void Exec(ICComp c) => destAddr.Set(c.Input.Read());
}

record OutputOp(ICOpParam srcAddr) : ICOperation
{
    public void Exec(ICComp c) => c.Output.Write(srcAddr.Get());
}

record JumpIfTrueOp((ICOpParam flag, ICOpParam jumpTo) pars) : ICOperation
{
    public void Exec(ICComp c)
    {
        if (pars.flag.Get() != 0)
            c.Cpu.IP = pars.jumpTo.Get();
    }
}

record JumpIfFalseOp((ICOpParam flag, ICOpParam jumpTo) pars) : ICOperation
{
    public void Exec(ICComp c)
    {
        if (pars.flag.Get() == 0)
            c.Cpu.IP = pars.jumpTo.Get();
    }
}

record LessThanOp((ICOpParam left, ICOpParam right, ICOpParam result) pars) : ICOperation
{
    public void Exec(ICComp c) => pars.result.Set(pars.left.Get() < pars.right.Get() ? 1 : 0);
}

record EqualsOp((ICOpParam left, ICOpParam right, ICOpParam result) pars) : ICOperation
{
    public void Exec(ICComp c) => pars.result.Set(pars.left.Get() == pars.right.Get() ? 1 : 0);
}

record RelativeBaseOffsetOp(ICOpParam offset) : ICOperation
{
    public void Exec(ICComp c) => c.Cpu.RB += offset.Get();
}