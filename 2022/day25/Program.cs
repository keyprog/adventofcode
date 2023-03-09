Console.WriteLine(SNAFUConverter.ToSnafu(File.ReadLines("input2.txt").Sum(l => SNAFUConverter.FromSnafu(l))));

public class SNAFUConverter
{
    public static string ToSnafu(long value)
    {
        List<char> snafu = new();
        while (value > 0)
        {
            long digit = value % 5L;
            value = (value + 2L) / 5L;
            snafu.Add(digit switch
            {
                0 => '0',
                1 => '1',
                2 => '2',
                3 => '=',
                4 => '-',
                _ => throw new ApplicationException()
            });
        }
        snafu.Reverse();
        return new string(snafu.ToArray());
    }

    public static long FromSnafu(string value)
    {
        long number = 0;
        for (int i = 0; i < value.Length; ++i)
        {
            number += ((long)Math.Pow(5, value.Length - i - 1)) * value[i] switch
            {
                '0' => 0L,
                '1' => 1L,
                '2' => 2L,
                '-' => -1L,
                '=' => -2L,
                _ => throw new NotSupportedException()
            };
        }
        return number;
    }
}
