using Concordion.Api.Extension;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Extension;

public class AbstractExtensionTestCase {
    protected IConcordionExtension Extension { get; set; }

    protected TestRig? TestRig { get; set; }

    protected ProcessingResult? ProcessingResult { get; set; }

    public TextWriter LogWriter { get; set; } = new StringWriter();

    public void processAnything()
    {
        process("<p>anything..</p>");
    }

    public void process(string fragment)
    {
        TestRig = new TestRig();
        ConfigureTestRig();
        ProcessingResult = TestRig.WithFixture(this)
            .WithExtension(Extension)
            .ProcessFragment(fragment);
    }

    protected virtual void ConfigureTestRig()
    {
    }

    public List<string> getEventLog()
    {
        LogWriter.Flush();

        var loggedEvents = LogWriter
            .ToString()
            .Split([LogWriter.NewLine], StringSplitOptions.None);
        var eventLog = loggedEvents.ToList();

        eventLog.Remove("");

        return eventLog;
    }

    public bool isAvailable(string resourcePath)
    {
        return TestRig.HasCopiedResource(new Api.Resource(resourcePath));
    }
}
