using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Css;

public class CssLinkedExtension : ConcordionExtension {
    public void AddTo(ConcordionExtender concordionExtender)
    {
        concordionExtender.WithLinkedCss(CssExtensionTest.SourcePath,
            new Api.Resource("/css/my.css"));
    }
}
