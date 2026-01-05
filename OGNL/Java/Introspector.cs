namespace OGNL.Java;

/// <summary>
/// Introspector
/// </summary>
public class Introspector {
    private Introspector()
    {
    }

    /// <summary>
    /// Include int indexer.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static PropertyDescriptor[] getPropertyDescriptors(Type type)
    {
        var ps = type.GetProperties();
        var pda = new PropertyDescriptor[ps.Length];
        var count = 0;

        for (var i = 0; i < ps.Length; i++) {
            var p = ps[i];
            PropertyDescriptor pd = null;
            var ips = p.GetIndexParameters();

            if (ips == null || ips.Length <= 0)
                pd = new PropertyDescriptor(p);
            else if (ips.Length > 1)
                // TODO: not support multidimensional indexer.
                continue;
            else
                // check int indexer.
            if (ips[0].ParameterType != typeof(int))
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
