using System.Reflection;

namespace OGNL;

/// <summary>
/// IndexedPropertyDescriptor
/// </summary>
public class IndexedPropertyDescriptor(PropertyInfo propertyInfo) :
    PropertyDescriptor(propertyInfo) {
    public MethodInfo? getIndexedReadMethod()
    {
        return propertyInfo?.GetGetMethod(false);
    }

    public MethodInfo? getIndexedWriteMethod()
    {
        return propertyInfo?.GetSetMethod(false);
    }
}
