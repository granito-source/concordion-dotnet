using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Configuration;

public class FakeExtension2Factory : ConcordionExtensionFactory {
    public ConcordionExtension CreateExtension()
    {
        return new FakeExtension2("FakeExtension2FromFactory");
    }
}
