using System.Reflection;

namespace OGNL;

internal class IndexedPropertyDescriptor(PropertyInfo propertyInfo) :
    PropertyDescriptor(propertyInfo);
