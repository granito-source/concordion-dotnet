using System.Xml.Linq;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Results.Stylesheet;

[ConcordionFixture]
public class StylesheetTest {
    private XElement? outputDocument;

    public void processDocument(string html)
    {
        outputDocument = new TestRig()
            .Process(html)
            .GetXDocument()
            .Root;
    }

    public string getRelativePosition(string outer, string target, string sibling)
    {
        var outerElement = outputDocument?.Element(outer);
        var targetIndex = indexOfFirstChildWithName(outerElement, target);
        var siblingIndex = indexOfFirstChildWithName(outerElement, sibling);

        return targetIndex > siblingIndex ? "after" : "before";
    }

    private int indexOfFirstChildWithName(XElement? element, string name)
    {
        var index = 0;

        if (element != null)
            foreach (var e in element.Elements()) {
                if (e.Name.LocalName.Equals(name))
                    return index;

                index++;
            }

        throw new Exception("No child <" + name + "> found.");
    }

    public bool elementTextContains(string elementName, string s1, string s2)
    {
        var element = outputDocument?
            .Document?
            .Root?
            .Descendants(elementName).ToArray()[0];
        var text = element?.Value;

        return text != null && text.Contains(s1) && text.Contains(s2);
    }
}
