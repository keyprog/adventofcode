using ic;

const string sourceFile = "program2.txt";

RunProgram(sourceFile);

void RunProgram(string sourceFile)
{
    long[] code = File.ReadAllText(sourceFile).Split(',').Select(long.Parse).ToArray();

    ICComp comp = ICComp.Load(code, input: 2);
    comp.Execute();

    Console.WriteLine(string.Join(',', comp.Output.ToArray()));
    Console.WriteLine(comp.Output.ToArray().Last());
}
