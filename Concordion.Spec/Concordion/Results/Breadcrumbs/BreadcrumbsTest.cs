using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Results.Breadcrumbs;

[ConcordionFixture]
public class BreadcrumbsTest : BreadcrumbsBase {
    public override void setUpResource(string resourceName, string content)
    {
        base.setUpResource(resourceName, content);
    }

    public override Result getBreadcrumbsFor(string resourceName)
    {
        return base.getBreadcrumbsFor(resourceName);
    }
}
