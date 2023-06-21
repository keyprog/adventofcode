using ic;
using ic.Comp;
using ic.Comp.Devices;

const string sourceFile = "day17_code.txt";

long[] code = File.ReadAllText(sourceFile).Split(',').Select(long.Parse).ToArray();

ICComp comp = ICComp.Load(code);
comp.Execute();
//comp.Output.DumpAsAscii(Console.Out);

ScaffoldMap map = new(comp.Output.ToArray());
map.DumpAsAscii(Console.Out);

RoboVac robot = RoboVac.FindOnMap(map);
string[] route = robot.Traverse(map).ToArray();
Console.WriteLine(string.Join(',', route));

char[] robotCode = RoboVacCompiler.Compile(route, mode: CompilerMode.Release);
Console.WriteLine("RoboVac Code:\n" + new string(robotCode));

code[0] = 2; // to wake up the vacuum robot
comp = ICComp.Load(code, input: new ICStream(robotCode.Select(c => (long)c)));
comp.Execute();
//comp.Output.DumpAsAscii(Console.Out);
Console.WriteLine("Resut: " + comp.Output.ToArray().Last());
