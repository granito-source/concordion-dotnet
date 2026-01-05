using System.Reflection;

namespace OGNL;

/// <summary>
/// PropertyDescriptor
/// </summary>
public class PropertyDescriptor {
    protected readonly PropertyInfo? propertyInfo;

    public PropertyDescriptor(PropertyInfo propertyInfo)
    {
        if (propertyInfo == null)
            throw new ArgumentException("PropertyInfo is NULL!");
        this.propertyInfo = propertyInfo;
    }

    protected PropertyDescriptor()
    {
    }

    public virtual MethodInfo? getReadMethod()
    {
        return propertyInfo?.GetGetMethod();
    }

    public virtual string? getName()
    {
        return propertyInfo?.Name;
    }

    public virtual MethodInfo? getWriteMethod()
    {
        return propertyInfo?.GetSetMethod();
    }

    public virtual Type? getPropertyType()
    {
        return propertyInfo?.PropertyType;
    }
}
