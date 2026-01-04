using System.Xml.Linq;
using Concordion.Api;
using Concordion.Api.Extension;
using Concordion.Api.Listener;

namespace Concordion.Spec.Concordion.Extension.Configuration;

public class FakeExtensionBase : IConcordionExtension, IDocumentParsingListener {
    public static readonly string FakeExtensionAttrName = "fake.extensions";

    private readonly string m_Text;

    public FakeExtensionBase()
    {
        m_Text = GetType().Name;
    }

    public FakeExtensionBase(string text)
    {
        m_Text = text;
    }

    public void BeforeParsing(XDocument document)
    {
        var root = document.Root;

        if (root == null)
            return;

        var rootElement = new Element(root);
        var existingValue = rootElement.GetAttributeValue(FakeExtensionAttrName);
        var newValue = m_Text;

        if (existingValue != null)
            newValue = existingValue + ", " + newValue;

        rootElement.AddAttribute(FakeExtensionAttrName, newValue);
    }

    public void AddTo(IConcordionExtender concordionExtender)
    {
        concordionExtender.WithDocumentParsingListener(this);
    }
}
