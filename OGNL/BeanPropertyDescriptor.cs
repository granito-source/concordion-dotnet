using System.Reflection;

namespace OGNL;

public class BeanPropertyDescriptor(string name, Type propertyType,
    MethodInfo reader, MethodInfo writer) : PropertyDescriptor {
    public override string Name => name;

    public override MethodInfo ReadMethod => reader;

    public override MethodInfo WriteMethod => writer;

    public override Type PropertyType => propertyType;
}
