namespace Concordion.Api.Listener;

public class ConcordionBuildEvent
{
    #region Properties

    public ITarget Target
    {
        get;
        private set;
    }

    #endregion

    #region Constructors

    public ConcordionBuildEvent(ITarget target)
    {
        Target = target;
    }

    #endregion
}
