using ic.Comp.Operations;

namespace ic.Comp.Devices;

class ICDecoder
{
    public ICOperation Decode(ICCpu c, ICMemory m)
    {
        (int opCode, ParamMode[] paramModes) = ParseOp(c.Ld(m));
        var p = new Queue<ParamMode>(paramModes);

        return opCode switch
        {
            1 => new SumOp(ICOpParam.Load3(m, c, p)),
            2 => new MultiplyOp(ICOpParam.Load3(m, c, p)),
            3 => new InputOp(ICOpParam.Load(m, c, p)),
            4 => new OutputOp(ICOpParam.Load(m, c, p)),
            5 => new JumpIfTrueOp(ICOpParam.Load2(m, c, p)),
            6 => new JumpIfFalseOp(ICOpParam.Load2(m, c, p)),
            7 => new LessThanOp(ICOpParam.Load3(m, c, p)),
            8 => new EqualsOp(ICOpParam.Load3(m, c, p)),
            9 => new RelativeBaseOffsetOp(ICOpParam.Load(m, c, p)),
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
