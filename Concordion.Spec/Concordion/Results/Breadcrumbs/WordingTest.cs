using System.Text.RegularExpressions;
using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Results.Breadcrumbs;

[ConcordionFixture]
public class WordingTest : BreadcrumbsBase {
    public string getBreadcrumbWordingFor(string resourceName, string content)
    {
        var packageName = "/" + resourceName.Replace(".html", string.Empty) + "/";
        var otherResourceName = "Demo.html";

        setUpResource(packageName + resourceName, content);
        setUpResource(packageName + otherResourceName, "<html />");

        var breadcrumbs = getBreadcrumbsFor(packageName + otherResourceName)
            .text;

        return Regex.Replace(breadcrumbs, " *> *", string.Empty);
    }
}
