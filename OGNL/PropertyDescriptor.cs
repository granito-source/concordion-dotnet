using System.Reflection;

namespace OGNL;

public class PropertyDescriptor {
    private readonly PropertyInfo propertyInfo = null!;

    public PropertyDescriptor(PropertyInfo propertyInfo)
    {
        this.propertyInfo = propertyInfo;
    }

    protected PropertyDescriptor()
    {
    }

    public virtual string Name => propertyInfo.Name;

    public virtual MethodInfo? ReadMethod => propertyInfo.GetGetMethod();

    public virtual MethodInfo? WriteMethod => propertyInfo.GetSetMethod();

    public virtual Type PropertyType => propertyInfo.PropertyType;
}
