using System.Xml.Linq;
using Concordion.Api;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public class JavaScriptLinker(Resource? javaScriptResource)
    : IDocumentParsingListener, ISpecificationProcessingListener {
    private XElement? m_Script;

    public void BeforeParsing(XDocument document)
    {
        var html = document.Root;
        var head = html?.Element("head");

        Check.NotNull(head, "<head> section is missing from document");

        m_Script = new XElement("script");
        m_Script.SetAttributeValue(XName.Get("type"), "text/javascript");
        m_Script.SetValue("");
        head!.Add(m_Script);
    }

    public void BeforeProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
        if (javaScriptResource == null)
            return;

        var resource = processingEvent.Resource;
        var javaScriptPath = resource
            .GetRelativePath(javaScriptResource)
            .Replace("\\", "/");

        m_Script?.SetAttributeValue(XName.Get("src"), javaScriptPath);
    }

    public void AfterProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
    }
}
