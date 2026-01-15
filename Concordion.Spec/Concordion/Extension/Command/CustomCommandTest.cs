using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Extension.Command;

[ConcordionFixture]
public class CustomCommandTest : AbstractExtensionTestCase {
    public void addCommandExtension()
    {
        Extension = new CommandExtension(LogWriter);
    }

    public List<string> getOutput()
    {
        return getEventLog();
    }
}
