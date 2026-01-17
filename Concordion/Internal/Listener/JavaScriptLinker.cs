using System.Xml.Linq;
using Concordion.Api;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public class JavaScriptLinker(Resource? javaScriptResource) :
    DocumentParsingListener, SpecificationProcessingListener {
    private XElement? script;

    public void BeforeParsing(XDocument document)
    {
        var head = document.Root?.Element("head");

        Check.NotNull(head, "<head> section is missing from document");

        script = new XElement("script");
        script.SetAttributeValue(XName.Get("type"), "text/javascript");
        script.SetValue("");
        head.Add(script);
    }

    public void BeforeProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
        if (javaScriptResource == null)
            return;

        var resource = processingEvent.Resource;
        var javaScriptPath = resource
            .GetRelativePath(javaScriptResource)
            .Replace(Path.DirectorySeparatorChar, '/');

        script?.SetAttributeValue(XName.Get("src"), javaScriptPath);
    }

    public void AfterProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
    }
}
