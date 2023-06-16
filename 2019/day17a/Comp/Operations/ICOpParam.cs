using ic.Comp.Devices;

namespace ic.Comp.Operations;

enum ParamMode { Reference = 0, Value = 1, Relative = 2 }

struct ICOpParam
{
    private readonly ICMemory memory;
    private readonly long paramValue;
    private readonly ParamMode mode;
    private readonly ICCpu cpu;
    public ICOpParam(ICMemory memory, ICCpu cpu, long value, ParamMode mode)
    {
        this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
        this.cpu = cpu ?? throw new ArgumentNullException(nameof(cpu));
        this.paramValue = value;
        this.mode = mode;
    }

    public static ICOpParam Load(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
        => new ICOpParam(memory, cpu, cpu.Ld(memory), modes.Load());

    public static (ICOpParam, ICOpParam) Load2(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
        => (Load(memory, cpu, modes), Load(memory, cpu, modes));

    public static (ICOpParam, ICOpParam, ICOpParam) Load3(ICMemory memory, ICCpu cpu, Queue<ParamMode> modes)
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

