int dialNumber = 50;
int code = 0;

var instructions = from l in File.ReadLines("/home/alexp/dev/adventofcode/2025/day1/input.txt")
                    select
                    (
                        Dir: l[0],
                        Dist:  int.Parse(l[1..])
                    );
instructions.PrintCollection();

foreach((char dir, int dist) in instructions)
{
    int prevDialNumber = dialNumber;
    dialNumber = dir switch
    {
        'L' => dialNumber - dist,
        'R' => dialNumber + dist
    };

    (code,dialNumber) = (prevDialNumber, dialNumber) switch
    {
        (0, >=-100 and <0) => (code + dist / 100, dialNumber + 100),
        (0, <-100 ) => (code + dist / 100, dialNumber + (100 * (dist/100 + 1))), 
        (_, <0) => (code + dist / 100 + 1, dialNumber + (100 * (dist/100 + 1))),
        (_, 0) => (code+1, dialNumber),
        (_, > 99) => (code + dialNumber / 100, dialNumber % 100),
        _ => (code, dialNumber)
    };

    Console.WriteLine($"Code: {code}, dial {dialNumber}");
       
}

