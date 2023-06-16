using NUnit.Framework;
using ic.Comp;
using ic.Comp.Devices;

namespace ic;

[TestFixture]
public class Tests
{
    [Test]
    public void TestBigNumberOp()
    {
        var comp = ICComp.Load(new long[] { 104, 1125899906842624, 99 }).Execute();
        //comp.Output.Dump("Big number", Console.Out);
        Assert.AreEqual(1125899906842624, comp.Output.ToArray()[0]);
    }

    [Test]
    public void TestJumpIfFalseOp()
    {
        var comp = ICComp.Load(new long[] { 1006, 101, 3, 99 }).Execute();
        Assert.AreEqual(4, comp.Cpu.IP);

        comp = ICComp.Load(new long[] { 109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 }).Execute();
        comp.Output.Dump(Console.Out);

        comp = ICComp.Load(new long[] { 109, 1, 204, -1, 1001, 1, 1, 100, 99 }).Execute();
        //comp.Memory.Dump(Console.Out);
        comp.Output.Dump(Console.Out);
    }

    [Test]
    public void TestEqualsOp()
    {

        //var comp = ICComp.Load(new long[] { 1008,100,16,101,1006,101,0,99 }).Execute();
        //comp.Output.Dump(Console.Out);
    }
    [Test]
    public void BasicExecution()
    {
        ICComp comp = ICComp.Load(new long[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 }, new ICStream(new long[] { 1 })).Execute();
        comp.Output.Dump(Console.Out);
    }

    [Test]
    public void AnotherTest()
    {
        var comp = ICComp.Load(new long[]{3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99}, new ICStream(new long[] { 8 })).Execute();
        comp.Output.Dump(Console.Out);
    }

    [Test]
    public void MoreTests()
    {
        var comp = ICComp.Load(new long[] { 1102, 34915192, 34915192, 7, 4, 7, 99, 0 }).Execute();
        comp.Output.Dump(Console.Out);
    }
}