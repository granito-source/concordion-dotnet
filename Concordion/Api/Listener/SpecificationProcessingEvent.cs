namespace Concordion.Api.Listener;

public class SpecificationProcessingEvent(Resource resource,
    Element rootElement) {
    public Element RootElement { get; } = rootElement;

    public Resource Resource { get; } = resource;
}
