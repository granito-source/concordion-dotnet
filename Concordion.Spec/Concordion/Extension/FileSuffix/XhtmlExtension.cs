using Concordion.Api.Extension;
using Concordion.Internal;

namespace Concordion.Spec.Concordion.Extension.FileSuffix;

public class XhtmlExtension : ConcordionExtension {
    public void AddTo(ConcordionExtender concordionExtender)
    {
        concordionExtender.WithSpecificationLocator(
            new ClassNameBasedSpecificationLocator("xhtml"));
    }
}
