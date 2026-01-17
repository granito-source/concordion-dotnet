namespace Concordion.Api.Listener;

public class ExceptionCaughtEvent(Exception exception, Element element,
    string? expression) {
    public Exception CaughtException { get; } = exception;

    public Element Element { get; } = element;

    public string? Expression { get; } = expression;
}
