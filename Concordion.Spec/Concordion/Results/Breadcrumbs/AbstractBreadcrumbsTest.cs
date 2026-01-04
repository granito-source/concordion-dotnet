using System.Xml.Linq;
using Concordion.Api;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Results.Breadcrumbs;

public abstract class AbstractBreadcrumbsTest {
    private TestRig testRig = new();

    public virtual void setUpResource(string resourceName, string content)
    {
        testRig.WithResource(new Resource(resourceName), content);
    }

    public virtual Result getBreadcrumbsFor(string resourceName)
    {
        var spanElements = testRig
            .Process(new Resource(resourceName))
            .GetXDocument()
            .Root?
            .Descendants("span");
        var result = new Result();

        foreach (var span in spanElements)
            if ("breadcrumbs" == span.Attribute("class").Value) {
                result.html = span.ToString(SaveOptions.DisableFormatting);
                result.text = span.Value;
            }

        return result;
    }

    public class Result {
        public string text = "";

        public string html = "";
    }
}
