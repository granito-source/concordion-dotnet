using System.Reflection;

namespace OGNL;

public class BeanPropertyDescriptor(string name, MethodInfo reader,
    MethodInfo writer) : PropertyDescriptor {
    public override string Name => name;

    public override MethodInfo ReadMethod => reader;

    public override MethodInfo WriteMethod => writer;
}
