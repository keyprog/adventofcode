const string InputFileName = "input2.txt";
const int TotalSpace = 70000000;
const int RequiredSpace = 30000000;

Dir root = ParseInput(File.ReadLines(InputFileName));
int freeSpace = TotalSpace - root.GetTotalSize();
int neededMore = RequiredSpace - freeSpace;
Console.WriteLine("Free space: " + freeSpace);

int[] folderSizes = root.TraverseAll().Select(d => d.GetTotalSize()).ToArray();
Array.Sort(folderSizes);
Console.WriteLine("Delete dir: " + folderSizes.First(s => s > neededMore));

//Console.WriteLine(root.PrintTree(new System.Text.StringBuilder()));

//Console.WriteLine("Total Size:" + root.GetTotalSize());
/*Console.WriteLine("Dirs <= 100000 total:" + 
        string.Join(",",root.TraverseAll()
            .Where(d => d.GetTotalSize() <= 100000)            
            .Sum(d => d.GetTotalSize())
            ));*/

Dir ParseInput(IEnumerable<string> lines)
{
    var iter = lines.GetEnumerator();
    var root = new Dir("/", null);
    var curDir = root;
    bool hasNext = iter.MoveNext();
    while (hasNext)
    {
        string command = iter.Current;
        switch (command.Split(' '))
        {
            case ["$", "cd", "/"]: curDir = root; iter.MoveNext(); break;
            case ["$", "cd", ".."]: curDir = curDir.Parent ?? throw new ApplicationException("No parent"); iter.MoveNext(); break;
            case ["$", "cd", var dirName]: { curDir = curDir.GetSubdir(dirName); hasNext = iter.MoveNext(); break; }
            case ["$", "ls"]:
                {
                    bool ls = true;
                    while (ls && (hasNext = iter.MoveNext()))
                    {
                        switch (iter.Current.Split(' '))
                        {
                            case ["dir", var dirName]: curDir.AddSubdir(dirName); break;
                            case ["$", _, _]: ls = false; break;
                            case [var fileSize, var fileName]: curDir.AddFile(int.Parse(fileSize)); break;
                            default: throw new ApplicationException($"Unexpected ls output: '{iter.Current}'");
                        }

                    }
                    break;
                }
            default: throw new ApplicationException($"Unexpected command: '{iter.Current}'");
        };

    }
    return root;
}

record class Dir(string Name, Dir? Parent)
{
    private List<Dir> subdirs = new List<Dir>();
    private List<int> files = new List<int>();

    public IEnumerable<Dir> Subdirs { get { return subdirs; } }

    public void AddSubdir(string name)
    {
        if (subdirs.Any(s => s.Name == name))
            return;

        subdirs.Add(new Dir(name, this));
    }

    public void AddFile(int size)
    {
        files.Add(size);
    }

    public Dir GetSubdir(string name)
    {
        var dir = subdirs.SingleOrDefault(s => s.Name == name);
        if (dir == null)
            throw new ApplicationException($"Dir {Name} doesn't have subdir with name {name}");
        return dir;
    }

    public int GetTotalSize()
    {
        return files.Sum() + subdirs.Sum(d => d.GetTotalSize());
    }

    public IEnumerable<Dir> TraverseAll()
    {
        yield return this;
        foreach (var dir in subdirs.SelectMany(s => s.TraverseAll()))
        {
            yield return dir;
        }
    }


    public System.Text.StringBuilder PrintTree(System.Text.StringBuilder sb, int level = 0)
    {
        sb.Append(' ', level).AppendLine(Name);
        level++;
        foreach (var file in files)
            sb.Append(' ', level).AppendLine(file.ToString());

        foreach (var dir in subdirs)
            dir.PrintTree(sb, level);
        return sb;
    }

    public override string ToString()
    {
        return Name + ": " + string.Join(",", files.Select(f => f.ToString()));
    }
}
