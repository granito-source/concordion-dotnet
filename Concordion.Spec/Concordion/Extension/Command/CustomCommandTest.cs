using Concordion.Integration;

namespace Concordion.Spec.Concordion.Extension.Command;

[ConcordionTest]
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
