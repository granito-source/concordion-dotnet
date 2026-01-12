using System.Drawing;
using Concordion.Api;
using Concordion.Internal.Util;

namespace Concordion.Spec.Support;

internal class StubTarget : Target {
    private readonly Dictionary<Resource, string> writtenStrings = new();

    private readonly List<Resource> copiedResources = [];

    public string GetWrittenString(Resource resource)
    {
        Check.IsTrue(writtenStrings.ContainsKey(resource),
            "Expected resource '" + resource.Path +
            "' was not written to target");

        return writtenStrings[resource];
    }

    public void Write(Resource target, string content)
    {
        writtenStrings.Add(target, content);
    }

    public void Write(Resource target, Bitmap image)
    {
        // Do nothing
    }

    public void CopyTo(Resource target, Stream source)
    {
        copiedResources.Add(target);
    }

    public bool HasCopiedResource(Resource resource)
    {
        return copiedResources.Contains(resource);
    }

    public string ResolvedPathFor(Resource resource)
    {
        return "";
    }
}
