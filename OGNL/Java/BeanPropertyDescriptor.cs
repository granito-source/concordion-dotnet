using System.Reflection;

namespace OGNL.Java;

/// <summary>
/// BeanPropertyDescriptor ��ժҪ˵����
/// </summary>
public class BeanPropertyDescriptor : PropertyDescriptor {
    string name;

    Type propertyType;

    MethodInfo reader;

    MethodInfo writer;

    public BeanPropertyDescriptor(string name, Type propertyType, MethodInfo reader, MethodInfo writer)
    {
        this.name = name;
        this.propertyType = propertyType;
        this.reader = reader;
        this.writer = writer;
    }

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
