using OGNL.Test.Objects;
using static OGNL.Util;

namespace OGNL.Test;

[TestFixture]
public class DotNetTestCase {
    [Test]
    public void TestHexString()
    {
        var i = 123;

        Console.WriteLine(i.ToString("x4"));
        Console.Write(typeof(float));
    }

    [Test]
    public void TestLong()
    {
        var l = 0x12aDL;
        var s = "12aD";

        Assert.That(ParseLong(s, 16), Is.EqualTo(l));

        l = 44;
        s = "54";

        Assert.That(ParseLong(s, 8), Is.EqualTo(l));

        l = -l;
        s = "-54";

        Assert.That(ParseLong(s, 8), Is.EqualTo(l));
    }

    [Test]
    public void TestConvert()
    {
        Assert.That(Convert.ToBoolean(1), Is.True);
        Assert.That(Convert.ToBoolean(0), Is.False);

        Assert.That(Convert.ToBoolean(0.001), Is.True);
        Assert.That(Convert.ToBoolean(0), Is.False);
    }

    [Test]
    public void TestIndexer()
    {
        var type = typeof(Indexer);
        var ps = type.GetProperties();

        foreach (var p in ps) {
            Console.Out.WriteLine("p.MemberType = {0}", p.MemberType);
            Console.Out.WriteLine("p.GetMethod = {0}", p.GetGetMethod());
            Console.Out.WriteLine("p.SetMethod = {0}", p.GetSetMethod());
            Console.Out.WriteLine("p.Name = {0}", p.Name);

            if (p.GetIndexParameters().Length > 0)
                Console.Out.WriteLine("p.Index = {0}",
                    p.GetIndexParameters()[0].ParameterType);
        }
    }

    [Test]
    public void TestULong()
    {
        var l = unchecked((long)0xf800000000000000L);
        var u1 = 0xf800000000000000L;
        var u2 = (ulong)l;

        using (Assert.EnterMultipleScope()) {
            Assert.That(u2, Is.EqualTo(u1));
            Assert.That(81 & 63, Is.EqualTo(17));
            Assert.That(1L << 17, Is.EqualTo(131072));
        }
    }

    [Test]
    public void TestLoadType()
    {
        var t = Type.GetType("OGNL.Test.Objects.Simple,OGNL.Test");

        Assert.That(t, Is.Not.Null);
    }

    [Test]
    public void TestEnum()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.That(Enum.Parse<SomeEnum>("Item1", true),
                Is.EqualTo(SomeEnum.Item1));
            Assert.That(Enum.Parse<SomeEnum>("item2", true),
                Is.EqualTo(SomeEnum.Item2));
        }
    }
}

public class Indexer {
    public object Item0 {
        get { return new object(); }

        set { }
    }

    public string this[int index] {
        get { return index.ToString(); }

        set { }
    }
}
