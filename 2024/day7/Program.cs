
var equations = from l in File.ReadLines("/home/alexp/dev/adventofcode/2024/day7/input.txt")
                let colon = l.IndexOf(':')
                let expRes = l[..colon]
                let nums = l[(colon+1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(n => long.Parse(n))
                select (value:long.Parse(expRes), nums);

var result = equations.Where(eq => IsValid(eq.value, 0, new Stack<long>(eq.nums.Reverse())));
foreach(var r in result) Console.WriteLine(r.value);

Console.WriteLine(result.Sum(eq => eq.value));

//Console.WriteLine(IsValid(190, new Stack<int>(new int[]{10, 19}.Reverse())));

bool IsValid(long expectedResult, long cumValue, Stack<long> arguments)
{    
    if (arguments.Count == 0)    
        return cumValue == expectedResult;

    long first = arguments.Pop();
    
    if(IsValid(expectedResult, cumValue + first, arguments))
        return true;
    
    if (IsValid(expectedResult, cumValue * first, arguments))
        return true;

    // Concatenation    
    if (IsValid(expectedResult, long.Parse(cumValue.ToString() + first.ToString()), arguments))
        return true;    
    
    arguments.Push(first);
    return false;
}