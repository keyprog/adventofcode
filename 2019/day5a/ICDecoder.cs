namespace ic;

class ICDecoder
{
    public IEnumerable<ICOperation> DecodeAll(ICCpu cpu, ICMemory memory)
    {
        while (cpu.State == ICCompState.Running)
        {
            yield return Decode(cpu, memory);
        }
    }

    private (int opCode, ParamMode[] paramModes) ParseOp(int op)
    {
        int opCode = op % 100;
        int modes = op / 100;
        if (modes == 0)
            return (opCode, Array.Empty<ParamMode>());

        List<ParamMode> paramModes = new List<ParamMode>();
        while (modes > 0)
        {
            paramModes.Add((ParamMode)(modes % 10));
            modes /= 10;
        }
        return (opCode, paramModes.ToArray());
    }

    public ICOperation Decode(ICCpu c, ICMemory m)
    {
        (int opCode, ParamMode[] paramModes) = ParseOp(c.Ld(m));
        var p = new Queue<ParamMode>(paramModes);

        return opCode switch
        {
            1 => new SumOp(Param.Ld3(m, c, p)),
            2 => new MultiplyOp(Param.Ld3(m, c, p)),
            3 => new InputOp(Param.Ld(m, c, p)),
            4 => new OutputOp(Param.Ld(m, c, p)),
            5 => new JumpIfTrueOp(Param.Ld2(m, c, p)),
            6 => new JumpIfFalseOp(Param.Ld2(m, c, p)),
            7 => new LessThanOp(Param.Ld3(m, c, p)),
            8 => new EqualsOp(Param.Ld3(m, c, p)),
            99 => new HaltOp(),
            _ => throw new NotSupportedException($"Unsupported opcode {opCode} at {c.IP - 1}")
        };
    }
}
