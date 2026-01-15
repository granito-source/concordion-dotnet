using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Results.Breadcrumbs;

[ConcordionFixture]
public class DeterminingBreadcrumbsTest : BreadcrumbsBase {
    public string getBreadcrumbTextFor(string resourceName)
    {
        return base.getBreadcrumbsFor(resourceName).text;
    }

    public void setUpResource(string resourceName)
    {
        base.setUpResource(resourceName, "<html />");
    }
}
