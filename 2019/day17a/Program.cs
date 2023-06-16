﻿using ic.Comp;

const string sourceFile = "day17_input.txt";

long[] code = File.ReadAllText(sourceFile).Split(',').Select(long.Parse).ToArray();

ICComp comp = ICComp.Load(code, input: 2);
comp.Execute();
comp.Output.DumpAsAscii(Console.Out);

Console.WriteLine(Calibrate(comp.Output.ToArray()));

long Calibrate(long[] output)
{
    int width = Array.IndexOf(output, '\n') + 1;
    int height = output.Length / width;

    char Ch(int row, int col) => (char)output[col + row * width];

    long total = 0;
    for (int row = 1; row < height - 1; ++row)
        for (int col = 1; col < width - 2; ++col)
        {
            if (Ch(row, col) == '#'
                && Ch(row - 1, col) == '#' && Ch(row + 1, col) == '#'
                && Ch(row, col - 1) == '#' && Ch(row, col + 1) == '#')
            {
                total += row * col;
                Console.WriteLine($"{row + 1} {col + 1} {total}");
            }

        }
    return total;
}


