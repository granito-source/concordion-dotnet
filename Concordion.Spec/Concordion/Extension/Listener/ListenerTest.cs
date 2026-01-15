using System.Globalization;
using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Extension.Listener;

[ConcordionFixture]
public class ListenerTest : AbstractExtensionTestCase {
    public void addLoggingExtension()
    {
        Extension = new LoggingExtension(LogWriter);
    }

    public string sqrt(string num)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        return Math.Sqrt(Convert.ToDouble(num)).ToString("N1");
    }

    public bool isPositive(int num)
    {
        return num > 0;
    }
}
