using Concordion.Api.Extension;
using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Extension.FileSuffix;

[ConcordionFixture]
[Extensions(typeof(XhtmlExtension))]
public class FileSuffixExtensionsTest {
    public bool hasBeenProcessed()
    {
        return true;
    }
}
