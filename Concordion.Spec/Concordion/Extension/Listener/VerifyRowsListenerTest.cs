using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Extension.Listener;

[ConcordionFixture]
public class VerifyRowsListenerTest : AbstractExtensionTestCase {
    public void addLoggingExtension()
    {
        Extension = new LoggingExtension(LogWriter);
    }

    public List<string> getGeorgeAndRingo()
    {
        var result = new[] { "George Harrison", "Ringo Starr" };

        return result.ToList();
    }
}
