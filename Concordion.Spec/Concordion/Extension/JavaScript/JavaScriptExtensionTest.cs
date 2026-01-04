using Concordion.Integration;

namespace Concordion.Spec.Concordion.Extension.JavaScript;

[ConcordionTest]
public class JavaScriptExtensionTest : AbstractExtensionTestCase {
    public const string SourcePath = "/test/concordion/my.js";

    public const string TestJs = "/* My test JS */";

    public void addLinkedJavaScriptExtension()
    {
        Extension = new JavaScriptLinkedExtension();
    }

    public void addEmbeddedJavaScriptExtension()
    {
        Extension = new JavaScriptEmbeddedExtension();
    }

    protected override void ConfigureTestRig()
    {
        TestRig.WithResource(new Api.Resource(SourcePath), TestJs);
    }

    public bool hasJavaScriptDeclaration(string cssFilename)
    {
        return ProcessingResult.HasJavaScriptDeclaration(cssFilename);
    }

    public bool hasEmbeddedTestJavaScript()
    {
        return ProcessingResult.HasEmbeddedJavaScript(TestJs);
    }
}
