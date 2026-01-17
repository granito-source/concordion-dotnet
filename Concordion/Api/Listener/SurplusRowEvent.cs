namespace Concordion.Api.Listener;

public class SurplusRowEvent(Element rowElement) {
    public Element RowElement { get; } = rowElement;
}
