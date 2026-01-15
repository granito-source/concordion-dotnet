using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Extension.Css;

[ConcordionFixture]
public class CssExtensionTest : AbstractExtensionTestCase {
    public static readonly string SourcePath = "/test/concordion/my.css";

    public static readonly string TestCss = "/* My test CSS */";

    public void addLinkedCSSExtension()
    {
        Extension = new CssLinkedExtension();
    }

    public void addEmbeddedCSSExtension()
    {
        Extension = new CssEmbeddedExtension();
    }

    protected override void ConfigureTestRig()
    {
        TestRig.WithResource(new Api.Resource(SourcePath), TestCss);
    }

    public bool hasCSSDeclaration(string cssFilename)
    {
        return ProcessingResult.HasCssDeclaration(cssFilename);
    }

    public bool hasEmbeddedTestCSS()
    {
        return ProcessingResult.HasEmbeddedCss(TestCss);
    }
}
