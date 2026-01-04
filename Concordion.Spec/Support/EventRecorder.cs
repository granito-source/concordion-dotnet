using Concordion.Api.Listener;

namespace Concordion.Spec.Support;

public class EventRecorder : IAssertEqualsListener, IExceptionCaughtListener {
    private readonly List<object> m_Events;

    public EventRecorder()
    {
        m_Events = [];
    }

    public object? GetLast(Type eventType)
    {
        object? lastMatch = null;

        foreach (var anEvent in m_Events.Where(eventType.IsInstanceOfType))
            lastMatch = anEvent;

        return lastMatch;
    }

    public void ExceptionCaught(ExceptionCaughtEvent caughtEvent)
    {
        m_Events.Add(caughtEvent);
    }

    public void SuccessReported(AssertSuccessEvent successEvent)
    {
        m_Events.Add(successEvent);
    }

    public void FailureReported(AssertFailureEvent failureEvent)
    {
        m_Events.Add(failureEvent);
    }
}
