namespace OGNL;

/// <summary>
/// Introspector
/// </summary>
public static class Introspector {
    /// <summary>
    /// Include int indexer.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static PropertyDescriptor[] GetPropertyDescriptors(Type type)
    {
        var ps = type.GetProperties();
        var pda = new PropertyDescriptor[ps.Length];
        var count = 0;

        foreach (var p in ps) {
            PropertyDescriptor pd;
            var ips = p.GetIndexParameters();

            if (ips.Length <= 0)
                pd = new PropertyDescriptor(p);
            else if (ips.Length > 1)
                // TODO: not support multidimensional indexer.
                continue;
            else if (ips[0].ParameterType != typeof(int))
                pd = new ObjectIndexedPropertyDescriptor(p);
            else
                pd = new IndexedPropertyDescriptor(p);

            pda[count++] = pd;
        }

        if (count != pda.Length) {
            // do array copy ;
            var tmp = new PropertyDescriptor[count];

            Array.Copy(pda, 0, tmp, 0, count);
            pda = tmp;
        }

        return pda;
    }
}
