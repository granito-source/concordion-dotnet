namespace Concordion.Api.Listener;

public class ConcordionBuildEvent(Target target) {
    public Target Target { get; } = target;
}
