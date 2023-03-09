using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test]
    [TestCase("1", 1)]
    [TestCase("2", 2)]
    [TestCase("1=", 3)]
    [TestCase("1-", 4)]
    [TestCase("10", 5)]
    [TestCase("11", 6)]
    [TestCase("1121-1110-1=0", 314159265)]
    public void FromSnafuTest(string input, long expected)
    {
        long actual = SNAFUConverter.FromSnafu(input);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCase(1, "1")]
    [TestCase(2,"2")]
    [TestCase(3,"1=")]
    [TestCase(4,"1-")]
    [TestCase(5,"10")]
    [TestCase(6,"11")]
    [TestCase(7,"12")]
    [TestCase(20,"1-0")]
    [TestCase(314159265,"1121-1110-1=0")]
    public void ToSnafuTest(long input, string expected)
    {
        string actual = SNAFUConverter.ToSnafu(input);
        Assert.AreEqual(expected, actual);
    }

}
