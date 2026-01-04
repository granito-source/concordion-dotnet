namespace Concordion.Api.Listener;

public interface IExecuteListener
{
    void ExecuteCompleted(ExecuteEvent executeEvent);
}
