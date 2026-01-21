using Concordion.Api;
using Concordion.Api.Listener;
using Concordion.Internal.Listener;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Results.Exception;

[ConcordionFixture]
public class ExceptionTest {
    private readonly List<string> stackTraceElements = [];

    public void addStackTraceElement(string declaringClassName,
        string methodName, string filename, int lineNumber)
    {
        stackTraceElements.Add(string.Format("at {0}.{1} in {2}:line {3}",
            declaringClassName, methodName, filename, lineNumber));
    }

    public string markAsException(string fragment, string expression,
        string errorMessage)
    {
        var exception = new StackTraceSettingException(errorMessage);

        exception.StackTraceElements.AddRange(stackTraceElements);

        var document = new TestRig()
            .ProcessFragment(fragment)
            .GetXDocument();

        var element = new Element(document.Descendants("p").ToArray()[0]);

        new ExceptionRenderer().ExceptionCaught(
            new ExceptionCaughtEvent(exception, element, expression));

        return element.ToXml();
    }
}
