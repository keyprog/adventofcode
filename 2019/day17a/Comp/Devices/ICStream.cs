namespace ic.Comp.Devices;

class ICStream
{
    private readonly Queue<long> data;

    public ICStream() : this(Array.Empty<long>()) { }
    public ICStream(long[] data)
    {
        this.data = new Queue<long>(data);
    }

    public static implicit operator ICStream(long data) => new ICStream(new long[] { data });

    public long[] ToArray() => data.ToArray();

    public void Write(long value) => data.Enqueue(value);

    public long Read() => data.Dequeue();

    public void Dump(string message, TextWriter writer)
    {
        writer.WriteLine(message);
        Dump(writer);
    }

    public void Dump(TextWriter writer)
    {
        writer.WriteLine(string.Join(',', data));
    }

    public void DumpAsAscii(TextWriter writer)
    {
        foreach(long c in data)
        {
            writer.Write((char) c);
        }
    }
}