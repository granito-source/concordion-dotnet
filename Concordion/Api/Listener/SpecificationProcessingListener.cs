namespace Concordion.Api.Listener;

public interface SpecificationProcessingListener {
    void BeforeProcessingSpecification(
        SpecificationProcessingEvent processingEvent);

    void AfterProcessingSpecification(
        SpecificationProcessingEvent processingEvent);
}
