namespace OGNL.Test;

public abstract class OgnlFixture {
    protected readonly OgnlContext context = Ognl.CreateDefaultContext(null);

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
        return Ognl.GetValue(expression, context, root);
    }

    protected void Set(string expression, object? value)
    {
        Ognl.SetValue(expression, context, root, value);
    }
}
