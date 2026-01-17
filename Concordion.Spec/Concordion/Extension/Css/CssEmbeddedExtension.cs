using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Css;

public class CssEmbeddedExtension : ConcordionExtension {
    public void AddTo(ConcordionExtender concordionExtender)
    {
        concordionExtender.WithEmbeddedCss(CssExtensionTest.TestCss);
    }
}
