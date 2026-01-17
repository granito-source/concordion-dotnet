using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.JavaScript;

class JavaScriptLinkedExtension : ConcordionExtension {
    public void AddTo(ConcordionExtender concordionExtender)
    {
        concordionExtender.WithLinkedJavaScript(
            JavaScriptExtensionTest.SourcePath,
            new Api.Resource("/js/my.js"));
    }
}
