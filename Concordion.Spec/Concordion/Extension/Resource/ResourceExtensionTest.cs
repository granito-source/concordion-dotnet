using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Extension.Resource;

[ConcordionFixture]
public class ResourceExtensionTest : AbstractExtensionTestCase {
    public void addResourceExtension()
    {
        Extension = new ResourceExtension();
    }

    public void addDynamicResourceExtension()
    {
        Extension = new DynamicResourceExtension();
    }

    protected override void ConfigureTestRig()
    {
        TestRig.WithResource(new Api.Resource(ResourceExtension.SourcePath),
            "0101");
    }

    public int getMeaningOfLife()
    {
        return 42;
    }
}
