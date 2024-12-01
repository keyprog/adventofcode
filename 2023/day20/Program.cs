using System.Text;

var conf = new ModulesConfiguration(File.ReadAllLines("input.txt"));
int low = 0;
int high = 0;

long count = 0;
//for(int i=0;i<100;++i)
  //  conf.PushButton();
while(!conf.PushButton())
{  
    count++;
    if (count % 1000000 == 0) count.Print();
}
count.Print();


class ModulesConfiguration
{
    public Dictionary<string, Module> Modules { get; } = [];
    public ModulesConfiguration(string[] lines)
    {
        foreach (var line in lines)
        {
            var module = ParseModule(line);
            Modules[module.Name] = module;
        }
        foreach(var module in Modules.Values)
        {
            foreach(string dest in module.Outputs)
                if(Modules.TryGetValue(dest, out var destModule))
                    destModule.Inputs.Add(module.Name);
        }
    }

    public bool PushButton()
    {
        int low = 0;
        int high = 0;
        StringBuilder sb = new StringBuilder();
        Queue<Pulse> pulses = new Queue<Pulse>();
        pulses.Enqueue(new Pulse("button", "broadcaster", false));
        while (pulses.Count > 0)
        {
            Pulse pulse = pulses.Dequeue();
            if (pulse.isHigh) high++; else low++;
            //Console.WriteLine($"""{pulse.Source} -{(pulse.isHigh ? "high":"low")} -> {pulse.Destination}""");
            sb.Append($"{pulse.Destination}:{(pulse.isHigh?1:0)};");
            if(pulse.Destination == "rx" && !pulse.isHigh)
            {
//        Console.WriteLine(sb);
                return true;
            }

            if (Modules.TryGetValue(pulse.Destination, out var destModule))
                destModule.Handle(pulse, pulses);
        }
        //Modules.Values.Select(m => ${m.Name} {m})
//        Console.WriteLine(sb);
        return false;
      }


    private const string OutputToken = " -> ";
    private Module ParseModule(string line)
    {
        int sep = line.IndexOf(OutputToken);
        string n = line[0..sep];
        string[] outputs = line[(sep + OutputToken.Length)..].Split(',').Select(o => o.Trim()).ToArray();
        
        return (n, n[0]) switch
        {
            ("broadcaster", _) => new BroadcasterModule(outputs),
            (_, '%') => new FlipFlopModule(n[1..], outputs),
            (_, '&') => new ConjunctionModule(n[1..], outputs),
            (_, _) => throw new NotSupportedException(n)
        };
    }
}

abstract class Module(string name, string[] outputs)
{
    public string Name { get; init; } = name;
    public string[] Outputs { get; init; } = outputs;
    public List<string> Inputs {get;} = [];
    public abstract void Handle(Pulse pulse, Queue<Pulse> bus);

    protected void SendToAll(string[] dest, bool isHigh, Queue<Pulse> bus)
    {
        foreach(var d in dest)
            bus.Enqueue(new Pulse(Name, d, isHigh));
    }

}

class FlipFlopModule(string name, string[] outputs) : Module(name, outputs)
{
    private bool isOn = false;
    public override void Handle(Pulse pulse, Queue<Pulse> bus)
    {
        if (!pulse.isHigh)
        {
            isOn = !isOn;
            SendToAll(Outputs, isOn, bus);
        }
    }
}
class ConjunctionModule(string name, string[] outputs) : Module(name, outputs)
{
    private Dictionary<string, bool> inputLastSignal = [];
    public override void Handle(Pulse pulse, Queue<Pulse> bus)
    {
        inputLastSignal[pulse.Source] = pulse.isHigh;
        bool low = inputLastSignal.Count == Inputs.Count && inputLastSignal.Values.All(s => s);
        SendToAll(Outputs, !low, bus);
    }
}

class BroadcasterModule(string[] outputs) :Module("broadcaster", outputs)
{
    public override void Handle(Pulse pulse, Queue<Pulse> bus)
    {
        SendToAll(Outputs, pulse.isHigh, bus);
    }
}

record Pulse(string Source, string Destination, bool isHigh);

