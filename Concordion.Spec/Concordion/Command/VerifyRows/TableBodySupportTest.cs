using Concordion.Integration;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.VerifyRows;

[ConcordionTest]
public class TableBodySupportTest {
    private List<string> names = [];

    public void setUpNames(string namesAsCSV)
    {
        foreach (var name in namesAsCSV.Split([',', ' '],
            StringSplitOptions.RemoveEmptyEntries)) {
            names.Add(name);
        }
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
