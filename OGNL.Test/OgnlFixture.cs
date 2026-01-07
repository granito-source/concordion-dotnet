namespace OGNL.Test;

public abstract class OgnlFixture {
    protected readonly OgnlContext context = Ognl.createDefaultContext(null);

    protected object? Get(string expression)
    {
        return Ognl.getValue(expression, context, this);
    }

    protected void Set(string expression, object? value)
    {
        Ognl.setValue(expression, context, this, value);
    }
}
