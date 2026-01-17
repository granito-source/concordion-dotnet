namespace Concordion.Api.Listener;

public class MissingRowEvent(Element rowElement) {
    public Element RowElement { get; } = rowElement;
}
