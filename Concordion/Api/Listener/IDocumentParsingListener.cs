using System.Xml.Linq;

namespace Concordion.Api.Listener;

public interface IDocumentParsingListener
{
    void BeforeParsing(XDocument document);
}
