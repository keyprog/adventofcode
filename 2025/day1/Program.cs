int dialNumber = 50;
int code = 0;

var instructions = from l in File.ReadLines("/home/alexp/dev/adventofcode/2025/day1/input.txt")
                   select (Dir: l[0], Dist: int.Parse(l[1..]));

//instructions.PrintCollection();

foreach (var instruction in instructions)
{
    (char dir, int dist) = instruction;
    if (dist > 100)
    {
        code += dist / 100;
        dist = dist % 100;
    }

    int prevDialNumber = dialNumber;
    dialNumber = dir switch
    {
        'L' => dialNumber - dist,
        'R' => dialNumber + dist,
        _ => throw new NotSupportedException()
    };

    (code, dialNumber) = (prevDialNumber, dialNumber) switch
    {
        (0, < 0) => (code, dialNumber + 100),
        (_, < 0) => (code + 1, dialNumber + 100),
        (_, >=100) => (code + 1, dialNumber - 100),
        (_, 0) => (code + 1, dialNumber),
        (_,_) => (code, dialNumber)
    };

    Console.WriteLine($"Instruction: {instruction}, Code: {code}, dial {dialNumber}");

}

