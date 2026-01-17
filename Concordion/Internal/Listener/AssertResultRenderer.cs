using Concordion.Api;
using Concordion.Api.Listener;

namespace Concordion.Internal.Listener;

internal class AssertResultRenderer : AssertEqualsListener,
    AssertTrueListener, AssertFalseListener
{
    public void SuccessReported(AssertSuccessEvent successEvent)
    {
        successEvent.Element
            .AddStyleClass("success")
            .AppendNonBreakingSpaceIfBlank();
    }

    public void FailureReported(AssertFailureEvent failureEvent)
    {
        var element = failureEvent.Element;

        element.AddStyleClass("failure");

        var spanExpected = new Element("del");

        spanExpected.AddStyleClass("expected");
        element.MoveChildrenTo(spanExpected);
        element.AppendChild(spanExpected);
        spanExpected.AppendNonBreakingSpaceIfBlank();

        var spanActual = new Element("ins");

        spanActual.AddStyleClass("actual");
        spanActual.AppendText(failureEvent.Actual?.ToString() ?? "(null)");
        spanActual.AppendNonBreakingSpaceIfBlank();

        element.AppendText("\n");
        element.AppendChild(spanActual);
    }
}
