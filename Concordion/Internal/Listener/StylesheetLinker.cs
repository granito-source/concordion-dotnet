using System.Xml.Linq;
using Concordion.Api;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public class StylesheetLinker(Resource stylesheet) :
    DocumentParsingListener, SpecificationProcessingListener {
    private XElement? link;

    public void BeforeParsing(XDocument document)
    {
        var html = document.Root;

        Check.NotNull(html, "root element may not be null");

        var head = html.Element("head");

        Check.NotNull(head, "<head> section is missing from document");

        link = new XElement("link");
        link.SetAttributeValue(XName.Get("type"), "text/css");
        link.SetAttributeValue(XName.Get("rel"), "stylesheet");
        link.SetValue("");
        head.Add(link);
    }

    public void BeforeProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
        var resource = processingEvent.Resource;
        var javaScriptPath = resource
            .GetRelativePath(stylesheet)
            .Replace(Path.DirectorySeparatorChar, '/');

        link?.SetAttributeValue(XName.Get("href"), javaScriptPath);
    }

    public void AfterProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
    }
}
