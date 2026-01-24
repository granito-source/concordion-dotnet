namespace OGNL.Test;

public abstract class OgnlFixture {
    protected readonly OgnlContext Context = Ognl.CreateDefaultContext("");

    private object root;

    protected OgnlFixture()
    {
        root = this;
    }

    protected void WithRoot(object newRoot)
    {
        root = newRoot;
    }

    protected object? Get(string expression)
    {
        return Ognl.GetValue(expression, Context, root);
    }

    protected void Set(string expression, object? value)
    {
        Ognl.SetValue(expression, Context, root, value);
    }
}
