namespace Concordion.Api.Listener;

public interface IExceptionCaughtListener
{
    void ExceptionCaught(ExceptionCaughtEvent caughtEvent);
}
