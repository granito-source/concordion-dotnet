using System.Drawing;
using Concordion.Api;
using Concordion.Internal.Util;

namespace Concordion.Spec.Support;

class StubTarget : ITarget {
    private readonly Dictionary<Resource, string> writtenStrings;

    private readonly List<Resource> m_CopiedResources = [];

    public StubTarget()
    {
        writtenStrings = new Dictionary<Resource, string>();
    }

    public string GetWrittenString(Resource resource)
    {
        Check.IsTrue(writtenStrings.ContainsKey(resource),
            "Expected resource '" + resource.Path +
            "' was not written to target");

        return writtenStrings[resource];
    }

    #region ITarget Members

    public void Write(Resource resource, string s)
    {
        writtenStrings.Add(resource, s);
    }

    public void Write(Resource resource, Bitmap image)
    {
        // Do nothing
    }

    public void CopyTo(Resource resource, string destination)
    {
    }

    public void CopyTo(Resource resource, TextReader inputReader)
    {
        m_CopiedResources.Add(resource);
    }

    public bool HasCopiedResource(Resource resource)
    {
        return m_CopiedResources.Contains(resource);
    }

    public void Delete(Resource resource)
    {
    }

    public string ResolvedPathFor(Resource resource)
    {
        return "";
    }

    #endregion
}
