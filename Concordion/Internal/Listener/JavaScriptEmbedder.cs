using System.Xml.Linq;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public class JavaScriptEmbedder : DocumentParsingListener
{
    #region Fields

    private readonly string m_JavaScript;

    #endregion

    #region Constructors

    public JavaScriptEmbedder(string javaScript)
    {
        m_JavaScript = javaScript;
    }

    #endregion

    #region IDocumentParsingListener Members

    public void BeforeParsing(XDocument document)
    {
        var html = document.Root;
        var head = html.Element("head");

        Check.NotNull(head, "<head> section is missing from document");

        var script = new XElement("script");

        script.SetAttributeValue(XName.Get("type"), "text/javascript");
        script.Add(m_JavaScript);
        head.AddFirst(script);
    }

    #endregion
}
