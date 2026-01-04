namespace Concordion.Api.Listener;

public interface ISpecificationProcessingListener
{
    void BeforeProcessingSpecification(SpecificationProcessingEvent processingEvent);

    void AfterProcessingSpecification(SpecificationProcessingEvent processingEvent);
}
