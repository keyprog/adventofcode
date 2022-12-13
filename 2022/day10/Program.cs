const string InputFile = "input2.txt";
const int StartCycle = 20;
const int CycleInterval = 40;
var lines = File.ReadLines(InputFile);

int total = 0;

char[] crt = ExecuteProgram(lines, StartCycle, CycleInterval, (register, cycle, lineNumber) =>
{
    total += (register * cycle);
    Console.WriteLine($"Line: {lineNumber}, Cycle: {cycle}, Register: {register}, Strengh: {register * cycle} Total: {total}");
});

Console.WriteLine("Result: " + total);

for (int i = 0; i < 6; ++i)
    Console.WriteLine(crt[(i * 40)..((i + 1) * 40)]);


char[] ExecuteProgram(IEnumerable<string> commands, int startCycle, int interval, Action<int, int, int> callback)
{
    int cycle = 1;
    int register = 1;
    int lineNumber = 0;
    char[] crt = new char[40 * 6];

    void DrawCRT()
    {
        int crtPos = cycle - 1;
        int crtCol = crtPos % 40;
        crt[crtPos] = Math.Abs(register - crtCol) <= 1 ? '#' : '.';
    }

    void IncrementCycle()
    {
        cycle++;
        if ((cycle - startCycle) % interval == 0)
            callback(register, cycle, lineNumber);
    }

    foreach (var line in commands)
    {
        lineNumber++;
        switch (line.Split(' '))
        {
            case ["addx", var arg]:
                DrawCRT();
                IncrementCycle();
                DrawCRT();
                register += int.Parse(arg);
                IncrementCycle();
                break;
            case ["noop"]:
                DrawCRT();
                IncrementCycle();
                break;
        }


    }
    return crt;
}


