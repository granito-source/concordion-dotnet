namespace Concordion.Api.Listener;

public interface IConcordionBuildListener
{
    void ConcordionBuilt(ConcordionBuildEvent buildEvent);
}
