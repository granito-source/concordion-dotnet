namespace Concordion.Api.Listener;

public class ConcordionBuildEvent
{
    #region Properties

    public Target Target
    {
        get;
        private set;
    }

    #endregion

    #region Constructors

    public ConcordionBuildEvent(Target target)
    {
        Target = target;
    }

    #endregion
}
