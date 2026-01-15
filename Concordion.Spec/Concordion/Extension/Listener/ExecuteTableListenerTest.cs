using System.Globalization;
using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Extension.Listener;

[ConcordionFixture]
public class ExecuteTableListenerTest : AbstractExtensionTestCase {
    public void addLoggingExtension()
    {
        Extension = new LoggingExtension(LogWriter);
    }

    public string sqrt(string num)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        return Math.Sqrt(Convert.ToDouble(num)).ToString("N1");
    }

    public string generateUsername(string fullName)
    {
        return fullName.Replace(" ", "").ToLower();
    }
}
