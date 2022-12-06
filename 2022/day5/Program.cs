const string input = "input2.txt";
const string MoveCommand = "move ";
const string FromArg = "from ";
const string ToArg = "to ";

var lines = File.ReadLines(input);

List<Stack<char>> stacks = new List<Stack<char>>();

foreach (var line in lines.Where(l => l.Contains('[')).Reverse())
    ParseDrawingLine(line, stacks);

Stack<char> tempStack = new Stack<char>();
foreach (var command in lines.Where(l => l.StartsWith(MoveCommand)).Select(l => ParseCommand(l)))
{
    // Part I
    //Repeat(command.Repeat, () => stacks.GetStack(command.To).Push(stacks.GetStack(command.From).Pop()));

    // Part II
    var from = stacks.GetStack(command.From);
    var to = stacks.GetStack(command.To);

    Repeat(command.Repeat, () => tempStack.Push(from.Pop()));
    Repeat(command.Repeat, () => to.Push(tempStack.Pop()));
}

stacks.PrintTop();

void Repeat(int count, Action c)
{
    while (count-- > 0) c();
}

void ParseDrawingLine(System.ReadOnlySpan<char> line, List<Stack<char>> stacks)
{
    for (int i = 0; i <= line.Length / 4; i++)
    {
        char crate = line[i * 4 + 1];
        if (crate != ' ')
            stacks.GetStack(i).Push(crate);
    }
}
Command ParseCommand(System.ReadOnlySpan<char> line)
{
    //format: "move {repeat} from {from} to {to}"
    int fromPos = line.IndexOf(FromArg);
    int toPos = line.IndexOf(ToArg);
    int repeat = int.Parse(line[MoveCommand.Length..fromPos]);
    int from = int.Parse(line[(fromPos + FromArg.Length)..toPos]) - 1;
    int to = int.Parse(line[(toPos + ToArg.Length)..]) - 1;
    return new Command(From: from, To: to, Repeat: repeat);
}
record Command(int From, int To, int Repeat);
static class StackListExtensions
{
    public static void Print<T>(this List<Stack<T>> listOfStacks)
    {
        foreach (var s in listOfStacks)
            Console.WriteLine(string.Join(',', s));
    }

    public static void PrintTop(this List<Stack<char>> listOfStacks)
    {
        Console.WriteLine(new string(listOfStacks.Select(s => s.Peek()).ToArray()));
    }

    public static Stack<T> GetStack<T>(this List<Stack<T>> listOfStacks, int index)
    {
        while (listOfStacks.Count <= index)
        {
            listOfStacks.Add(new Stack<T>());
        }

        return listOfStacks[index];
    }
}