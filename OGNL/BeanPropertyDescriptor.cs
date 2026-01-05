using System.Reflection;

namespace OGNL;

/// <summary>
/// BeanPropertyDescriptor
/// </summary>
public class BeanPropertyDescriptor(string name, Type propertyType,
    MethodInfo reader, MethodInfo writer) : PropertyDescriptor {
    public override MethodInfo getReadMethod()
    {
        return reader;
    }

    public override string getName()
    {
        return name;
    }

    public override MethodInfo getWriteMethod()
    {
        return writer;
    }

    public override Type getPropertyType()
    {
        return propertyType;
    }
}
