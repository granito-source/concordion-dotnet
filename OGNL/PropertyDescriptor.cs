using System.Reflection;

namespace OGNL;

internal class PropertyDescriptor {
    protected readonly PropertyInfo PropertyInfo = null!;

    public PropertyDescriptor(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
    }

    protected PropertyDescriptor()
    {
    }

    public virtual string Name => PropertyInfo.Name;

    public virtual MethodInfo? ReadMethod => PropertyInfo.GetGetMethod();

    public virtual MethodInfo? WriteMethod => PropertyInfo.GetSetMethod();
}
