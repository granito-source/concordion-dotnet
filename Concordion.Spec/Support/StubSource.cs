using System.Text;
using Concordion.Api;
using Concordion.Internal.Util;

namespace Concordion.Spec.Support;

class StubSource : ISource {
    private readonly Dictionary<Resource, string> resources = new();

    public void AddResource(string resourceName, string content)
    {
        AddResource(new Resource(resourceName), content);
    }

    public void AddResource(Resource resource, string content)
    {
        if (!resources.ContainsKey(resource))
            resources.Add(resource, content);
        else {
            resources.Remove(resource);
            resources.Add(resource, content);
        }
    }

    public Stream CreateInputStream(Resource resource)
    {
        Check.IsTrue(CanFind(resource),
            "No such resource exists in simulator: " + resource.Path);

        return new MemoryStream(Encoding.UTF8.GetBytes(resources[resource]));
    }

    public TextReader CreateReader(Resource resource)
    {
        Check.IsTrue(CanFind(resource),
            "No such resource exists in simulator: " + resource.Path);

        return new StreamReader(
            new MemoryStream(Encoding.UTF8.GetBytes(resources[resource])));
    }

    public bool CanFind(Resource resource)
    {
        return resources.ContainsKey(resource);
    }
}
