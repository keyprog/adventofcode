using ic.Comp;

const string sourceFile = "day17_input.txt";

long[] code = File.ReadAllText(sourceFile).Split(',').Select(long.Parse).ToArray();

ICComp comp = ICComp.Load(code, input: 2);
comp.Execute();
comp.Output.DumpAsAscii(Console.Out);

long[] output = comp.Output.ToArray();
int width = Array.IndexOf(output, 10);
int height = output.Length / width;
Console.WriteLine($"Field {width}x{height}");

long total = 0;
for (int row = 1; row < height - 1; ++row)
    for (int col = 1; col < width - 1; ++col)
    {
        if (output[col + row * width] == '#'
            && output[col - 1 + row * width] == '#' && output[col + 1 + row * width] == '#'
            && output[col + (row - 1) * width] == '#' && output[col + (row + 1) * width] == '#')
        {
            total += col * row;
            Console.WriteLine($"{col} {row} {total}");
        }

    }

Console.WriteLine(total);



