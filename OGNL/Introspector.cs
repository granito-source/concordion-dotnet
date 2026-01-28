namespace OGNL;

/// <summary>
/// Introspector
/// </summary>
internal static class Introspector {
    /// <summary>
    /// Include int indexer.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static PropertyDescriptor[] GetPropertyDescriptors(Type type)
    {
        var properties = type.GetProperties();
        var descriptors = new PropertyDescriptor[properties.Length];
        var index = 0;

        foreach (var p in properties) {
            var ips = p.GetIndexParameters();

            PropertyDescriptor descriptor;

            switch (ips.Length) {
                case 0:
                    descriptor = new PropertyDescriptor(p);

                    break;
                case 1 when ips[0].ParameterType == typeof(int):
                    descriptor = new IndexedPropertyDescriptor(p);

                    break;
                case 1:
                    descriptor = new ObjectIndexedPropertyDescriptor(p);

                    break;
                default:
                    // TODO: not support multidimensional indexer.
                    continue;
            }

            descriptors[index++] = descriptor;
        }

        if (index != descriptors.Length) {
            // do array copy ;
            var tmp = new PropertyDescriptor[index];

            Array.Copy(descriptors, 0, tmp, 0, index);
            descriptors = tmp;
        }

        return descriptors;
    }
}
