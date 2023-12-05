namespace ic;

class ICStream
{
    private readonly Queue<int> data;

    public ICStream() : this(Array.Empty<int>()) { }
    public ICStream(int[] data)
    {
        this.data = new Queue<int>(data);
    }

    public int[] ToArray() => data.ToArray();

    public void Write(int value) => data.Enqueue(value);

    public int Read() => data.Dequeue();

    public void Dump(TextWriter writer)
    {
        writer.WriteLine(string.Join(',', data));
    }
}