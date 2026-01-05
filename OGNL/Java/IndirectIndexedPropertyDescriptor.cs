using System.Reflection;

namespace OGNL.Java;

/// <summary>
/// IndirectIndexedPropertyDescriptor ��ժҪ˵����
/// </summary>
public class IndirectIndexedPropertyDescriptor : IndexedPropertyDescriptor {
    public IndirectIndexedPropertyDescriptor(PropertyInfo p) : base(p)
    {
    }
}
