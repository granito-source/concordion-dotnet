using System.Xml.Linq;

namespace Concordion.Api.Listener;

public interface DocumentParsingListener {
    void BeforeParsing(XDocument document);
}
