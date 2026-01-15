using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.VerifyRows;

[ConcordionFixture]
public class TableBodySupportTest {
    private List<string> names = [];

    public void setUpNames(string namesAsCSV)
    {
        var parsed = namesAsCSV.Split([',', ' '],
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var name in parsed)
            names.Add(name);
    }

    public List<string> getNames()
    {
        return names;
    }

    public string? process(string inputFragment)
    {
        var document = new TestRig()
            .WithFixture(this)
            .ProcessFragment(inputFragment)
            .GetXDocument();
        var table = document
            .Element("html")?
            .Element("body")?
            .Element("fragment")?
            .Element("table");

        return table?.ToString().Replace("\u00A0", "&#160;");
    }
}
