using System.Reflection;

namespace OGNL.Java;

/// <summary>
/// IndexedPropertyDescriptor ��ժҪ˵����
/// </summary>
public class IndexedPropertyDescriptor : PropertyDescriptor {
    public IndexedPropertyDescriptor(PropertyInfo p) : base(p)
    {
    }

    public MethodInfo getIndexedReadMethod()
    {
        return p.GetGetMethod(false);
    }

    public MethodInfo getIndexedWriteMethod()
    {
        return p.GetSetMethod(false);
    }
}
