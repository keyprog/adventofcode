using ic;

const string sourceFile = "program2.txt";
RunTests();
RunProgram(sourceFile);

void RunProgram(string sourceFile)
{
    int[] code = File.ReadAllText(sourceFile).Split(',').Select(int.Parse).ToArray();

    ICComp comp = ICComp.Load(code, new ICStream(new[] { 5 }));
    comp.Execute();

    Console.WriteLine(string.Join(',', comp.Output.ToArray()));
    Console.WriteLine(comp.Output.ToArray().Last());
}
void RunTests()
{
    ICComp comp = ICComp.Load(new []{3,9,8,9,10,9,4,9,99,-1,8}, new ICStream(new[] {1})).Execute();
    comp.Output.Dump(Console.Out);

    comp = ICComp.Load(new []{3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99}, new ICStream(new[] {8})).Execute();
    comp.Output.Dump(Console.Out);


}