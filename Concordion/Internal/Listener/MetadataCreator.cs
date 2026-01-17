using System.Xml.Linq;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public class MetadataCreator : DocumentParsingListener {
    private static bool HasContentTypeMetadata(XElement head)
    {
        var metaChildren = head.Elements("meta");

        return metaChildren
            .Select(metaChild => metaChild.Attribute("http-equiv"))
            .Any(httpEquiv =>
                httpEquiv != null && string.Equals("content-type",
                    httpEquiv.Value, StringComparison.OrdinalIgnoreCase));
    }

    private static void AddContentTypeMetadata(XElement head)
    {
        var meta = new XElement("meta");

        meta.SetAttributeValue("http-equiv", "content-type");
        meta.SetAttributeValue("content", "text/html; charset=UTF-8");
        head.AddFirst(meta);
    }

    public void BeforeParsing(XDocument document)
    {
        var head = document.Root?.Element("head");

        Check.NotNull(head, "<head> section is missing from document");

        if (!HasContentTypeMetadata(head))
            AddContentTypeMetadata(head);
    }
}
