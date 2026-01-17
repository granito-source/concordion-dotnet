using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.JavaScript;

public class JavaScriptEmbeddedExtension : ConcordionExtension {
    public void AddTo(ConcordionExtender concordionExtender)
    {
        concordionExtender.WithEmbeddedJavaScript(
            JavaScriptExtensionTest.TestJs);
    }
}
