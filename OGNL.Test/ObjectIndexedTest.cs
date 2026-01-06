using System.Collections;
using System.Reflection;
using OGNL.JccGen;
using OGNL.Test.Util;

namespace OGNL.Test;

[TestFixture]
public class ObjectIndexedTest {
    protected OgnlContext context;

    public interface TestInterface {
        string getSunk(string index);

        void setSunk(string index, string sunk);
    }

    public class Test1 : TestInterface {
        public virtual string this[string index] {
            get { return "foo"; }

            set { }
        }

        public virtual string getSunk(string index)
        {
            return "foo";
        }

        public virtual void setSunk(string index, string sunk)
        {
            /* do nothing */
        }
    }

    public class Test2 : Test1 {
        public override string this[string index] {
            get { return "foo"; }

            set { }
        }

        public override string getSunk(string index)
        {
            return "foo";
        }

        public override void setSunk(string index, string sunk)
        {
            /* do nothing */
        }
    }

    public class Test3 : Test1 {
        public virtual string this[string index] {
            get { return "foo"; }

            set { }
        }

        public override string getSunk(string index)
        {
            return "foo";
        }

        public override void setSunk(string index, string sunk)
        {
            /* do nothing */
        }

        public virtual string this[object index] {
            get { return "foo"; }

            set { }
        }

        public string getSunk(object index)
        {
            return null;
        }
    }

    public class Test4 : Test1 {
        public override string getSunk(string index)
        {
            return "foo";
        }

        public override void setSunk(string index, string sunk)
        {
            /* do nothing */
        }

        public void setSunk(object index, string sunk)
        {
            /* do nothing */
        }
    }

    public class Test5 : Test1 {
        public override string getSunk(string index)
        {
            return "foo";
        }

        public override void setSunk(string index, string sunk)
        {
            /* do nothing */
        }

        public string getSunk(object index)
        {
            return null;
        }

        public void setSunk(object index, string sunk)
        {
            /* do nothing */
        }
    }

    public TestSuite suite()
    {
        // TODO: simple test.
        return null;
    }

    public ObjectIndexedTest()
    {
    }

    public ObjectIndexedTest(string name)
    {
    }

    [Ignore("Not supported")]
    public void testPropertyDescriptorReflection() // throws Exception
    {
        OgnlRuntime.getPropertyDescriptor(typeof(CollectionBase), "");

        // OgnlRuntime.getPropertyDescriptor(typeof (java.util.AbstractSequentialList), "");
        OgnlRuntime.getPropertyDescriptor(typeof(Array), "");
        OgnlRuntime.getPropertyDescriptor(typeof(ArrayList), "");

        // OgnlRuntime.getPropertyDescriptor(typeof (java.util.BitSet), "");
        OgnlRuntime.getPropertyDescriptor(typeof(DateTime), "");
        OgnlRuntime.getPropertyDescriptor(typeof(FieldInfo), "");

        // OgnlRuntime.getPropertyDescriptor(typeof (java.util.LinkedList), "");
        OgnlRuntime.getPropertyDescriptor(typeof(IList), "");
        OgnlRuntime.getPropertyDescriptor(typeof(IEnumerator), "");

        // OgnlRuntime.getPropertyDescriptor(typeof (java.lang.ThreadLocal), "");
        OgnlRuntime.getPropertyDescriptor(typeof(Uri), "");
        OgnlRuntime.getPropertyDescriptor(typeof(Stack), "");
    }

    [Ignore("Not Supported")]
    public void testObjectIndexAccess() // throws OgnlException
    {
        var expression = (SimpleNode)Ognl.parseExpression("#ka.sunk[#root]");

        context["ka"] = new Test1();
        Ognl.getValue(expression, context, "aksdj");
    }

    [Ignore("Not Supported.")]
    public void testObjectIndexInSubclass() // throws OgnlException
    {
        var expression = (SimpleNode)Ognl.parseExpression("#ka.sunk[#root]");

        context["ka"] = new Test2();
        Ognl.getValue(expression, context, "aksdj");
    }

    [Ignore("Not Supported")]
    public void testMultipleObjectIndexGetters() // throws OgnlException
    {
        var expression = (SimpleNode)Ognl.parseExpression("#ka.sunk[#root]");

        context["ka"] = new Test3();

        try {
            Ognl.getValue(expression, context, new Test3());
            Assert.Fail();
        } catch (OgnlException ex) {
            /* Should throw */
        }
    }

    [Ignore("Not Supported")]
    public void testMultipleObjectIndexSetters() // throws OgnlException
    {
        var expression = (SimpleNode)Ognl.parseExpression("#ka.sunk[#root]");

        context["ka"] = new Test4();

        try {
            Ognl.getValue(expression, context, "aksdj");
            Assert.Fail();
        } catch (OgnlException ex) {
            /* Should throw */
        }
    }

    [Ignore("Not Supported")]
    public void testMultipleObjectIndexMethodPairs() // throws OgnlException
    {
        var expression = (SimpleNode)Ognl.parseExpression("#ka.sunk[#root]");

        context["ka"] = new Test5();

        try {
            Ognl.getValue(expression, context, "aksdj");
            Assert.Fail();
        } catch (OgnlException ex) {
            /* Should throw */
        }
    }

    [SetUp]
    protected void setUp()
    {
        context = Ognl.createDefaultContext(null);
    }
}
