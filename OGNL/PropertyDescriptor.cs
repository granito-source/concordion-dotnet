using System.Reflection;

namespace OGNL;

internal class PropertyDescriptor(PropertyInfo propertyInfo) {
    public string Name => propertyInfo.Name;

    public MethodInfo? ReadMethod => propertyInfo.GetGetMethod();

    public MethodInfo? WriteMethod => propertyInfo.GetSetMethod();
}
