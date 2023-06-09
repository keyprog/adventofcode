namespace ic;

class ICDecoder
{
    public ICOperation Decode(ICCpu c, ICMemory m)
    {
        (int opCode, ParamMode[] paramModes) = ParseOp(c.Ld(m));
        var p = new Queue<ParamMode>(paramModes);

        return opCode switch
        {
            1 => new SumOp(Param.Load3(m, c, p)),
            2 => new MultiplyOp(Param.Load3(m, c, p)),
            3 => new InputOp(Param.Load(m, c, p)),
            4 => new OutputOp(Param.Load(m, c, p)),
            5 => new JumpIfTrueOp(Param.Load2(m, c, p)),
            6 => new JumpIfFalseOp(Param.Load2(m, c, p)),
            7 => new LessThanOp(Param.Load3(m, c, p)),
            8 => new EqualsOp(Param.Load3(m, c, p)),
            9 => new RelativeBaseOffsetOp(Param.Load(m, c, p)),
            99 => new HaltOp(),
            _ => throw new NotSupportedException($"Unsupported opcode {opCode} at {c.IP - 1}")
        };
    }

    public static (int opCode, ParamMode[] paramModes) ParseOp(long op)
    {
        int opCode = (int)(op % 100);
        long modes = op / 100;
        if (modes == 0)
            return (opCode, Array.Empty<ParamMode>());

        List<ParamMode> paramModes = new List<ParamMode>(3);
        while (modes > 0)
        {
            paramModes.Add((ParamMode)(modes % 10));
            modes /= 10;
        }
        return (opCode, paramModes.ToArray());
    }

}
