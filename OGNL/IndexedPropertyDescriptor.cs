using System.Reflection;

namespace OGNL;

/// <summary>
/// IndexedPropertyDescriptor
/// </summary>
internal class IndexedPropertyDescriptor(PropertyInfo propertyInfo) :
    PropertyDescriptor(propertyInfo) {
    public MethodInfo? GetIndexedReadMethod()
    {
        return PropertyInfo.GetGetMethod(false);
    }

    public MethodInfo? GetIndexedWriteMethod()
    {
        return PropertyInfo.GetSetMethod(false);
    }
}
