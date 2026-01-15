using System.Xml.Linq;
using Concordion.Internal.Util;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.VerifyRows;

[ConcordionFixture]
public class VerifyRowsTest {
    public ICollection<string> usernames = new List<string>();

    public string processFragment(string fragment, string csv)
    {
        usernames = csvToCollection(csv);

        var document = new TestRig()
            .WithFixture(this)
            .ProcessFragment(fragment)
            .GetXDocument();
        var result = string.Empty;
        var table = document.Descendants("table").ToList()[0];
        var rows = table.Elements("tr").ToList();

        for (var index = 1; index < rows.Count; index++) {
            var row = rows.ToArray()[index];

            if (!string.IsNullOrEmpty(result))
                result += ", ";

            result += categorize(row);
        }

        return result;
    }

    private string categorize(XElement row)
    {
        var cssClass = row.Attribute("class");

        if (cssClass == null) {
            var cell = row.Element("td");

            cssClass = cell?.Attribute("class");
        }

        Check.NotNull(cssClass, "cssClass is null");

        return cssClass!.Value.ToUpper();
    }

    private static ICollection<string> csvToCollection(string csv)
    {
        var c = new List<string>();

        foreach (var s in csv.Split([',', ' '],
                     StringSplitOptions.RemoveEmptyEntries))
            c.Add(s);

        return c;
    }
}
