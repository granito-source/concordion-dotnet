using System.Xml.Linq;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public class JavaScriptEmbedder(string javaScript) : DocumentParsingListener {
    public void BeforeParsing(XDocument document)
    {
        var head = document.Root?.Element("head");

        Check.NotNull(head, "<head> section is missing from document");

        var script = new XElement("script");

        script.SetAttributeValue(XName.Get("type"), "text/javascript");
        script.Add(javaScript);
        head.AddFirst(script);
    }
}
