using ic;

const string sourceFile = "program2.txt";

long[] code = File.ReadAllText(sourceFile).Split(',').Select(long.Parse).ToArray();

ICComp comp = ICComp.Load(code, input: 2);
comp.Execute();
comp.Output.Dump("Output:", Console.Out);