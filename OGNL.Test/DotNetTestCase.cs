using OGNL.Test.Objects;
using static OGNL.Util;

namespace OGNL.Test;

/// <summary>
/// NumberTestCase
/// </summary>
[TestFixture]
public class DotNetTestCase {
    [Test]
    public void testHexString()
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
        /*
        Assert.IsTrue (Convert.ToBoolean ("True"));
        Assert.IsFalse (Convert.ToBoolean ("False"));
        Assert.IsFalse (Convert.ToBoolean ("aaaa"));
        */
        Assert.IsTrue(Convert.ToBoolean(1));
        Assert.IsFalse(Convert.ToBoolean(0));

        Assert.IsTrue(Convert.ToBoolean(0.001));
        Assert.IsFalse(Convert.ToBoolean(0));
    }

    [Test]
    public void TestIndexer()
    {
        var type = typeof(Indexer);
        var ps = type.GetProperties();

        for (var i = 0; i < ps.Length; i++) {
            var p = ps[i];

            Console.Out.WriteLine("p.MemberType = {0}", p.MemberType);
            Console.Out.WriteLine("p.GetMethod = {0}", p.GetGetMethod());
            Console.Out.WriteLine("p.SetMethod = {0}", p.GetSetMethod());
            Console.Out.WriteLine("p.Name = {0}", p.Name);
            if (p.GetIndexParameters().Length > 0)
                Console.Out.WriteLine("p.Index = {0}", p.GetIndexParameters()[0].ParameterType);
        }
    }

    [Test]
    public void testULong()
    {
        var l = unchecked((long)0xf800000000000000L);
        var u1 = 0xf800000000000000L;
        var u2 = (ulong)l;
        Assert.That(u2, Is.EqualTo(u1));

        // No oct supported.
        // Assert.AreEqual (63 , 077);
        Assert.That(81 & 63, Is.EqualTo(17));
        Assert.That(1L << (17), Is.EqualTo(131072));
    }

    [Test]
    public void testLoadType()
    {
        var t = Type.GetType("org.ognl.test.objects.Simple,Test");
        Assert.IsNotNull(t);

        // t = Type.GetType ("System.Collections.Specialized.ListDictionary,System") ;
        // Assert.IsNotNull (t);
    }

    [Test]
    public void testEnum()
    {
        Assert.That(Enum.Parse<SomeEnum>("Item1", true), Is.EqualTo(SomeEnum.Item1));
        Assert.That(Enum.Parse<SomeEnum>("item2", true), Is.EqualTo(SomeEnum.Item2));
    }
}

public class Indexer {
    public object Item0 {
        get { return new object(); }

        set { ; }
    }

    public string this[int index] {
        get { return index.ToString(); }

        set { }
    }
}
