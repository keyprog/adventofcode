const string InputFile = "input2.txt";
const int DecryptionKey = 811589153;
const int Repeats = 10;
LinkedListNode<long>[] nodes = File.ReadLines(InputFile)
            .Select(v => long.Parse(v) * DecryptionKey)
            .Select(v => new LinkedListNode<long>(v)).ToArray();
int length = nodes.Length;
var linkedList = new LinkedList<long>();
foreach (var node in nodes)
    linkedList.AddLast(node);

Console.WriteLine(string.Join(',', linkedList));
for (int i = 0; i < Repeats; ++i)
    foreach (var node in nodes)
    {
        MoveNode(node, (int)(node.Value % (long)(length - 1)));
    }
Console.WriteLine(string.Join(", ", linkedList));

var zero = linkedList.Find(0)!;
var values = new[] {
        GetAfter(zero, 1000 % length),
        GetAfter(zero, 2000 % length),
        GetAfter(zero, 3000 % length)
        };

Console.WriteLine(string.Join(',', values));
Console.WriteLine("Sum: " + values.Select(v => (long)v).Sum());

T GetAfter<T>(LinkedListNode<T> node, long offset)
{
    var n = node;
    while (offset > 0)
    {
        n = GetNext(n);
        offset--;
    }
    return n.Value;
}

LinkedListNode<T> GetNext<T>(LinkedListNode<T> n) => n.Next == null ? n.List!.First! : n.Next;
void MoveNode<T>(LinkedListNode<T> node, int offset)
{
    if (offset == 0)
        return;
    if (offset < 0)
        offset += length - 1;

    var list = node.List!;
    offset = offset > 0 ? offset : (offset * -1) + 1;
    LinkedListNode<T> insertAfter = GetNext(node);
    offset--;
    list.Remove(node);
    while (offset > 0)
    {
        insertAfter = GetNext(insertAfter);
        offset--;
    }
    list.AddAfter(insertAfter, node);
}