using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.VerifyRows.Results;

[ConcordionFixture]
public class MissingRowsTest {
    private readonly List<Person> people = [];

    public void addPerson(string firstName, string lastName, int birthYear)
    {
        people.Add(new Person(firstName, lastName, birthYear));
    }

    public string getOutputFragment(string inputFragment)
    {
        var document = new TestRig()
            .WithFixture(this)
            .ProcessFragment(inputFragment)
            .GetXDocument();
        var tables = document.Descendants("table");

        // stops loop after first entry, simulating the java code.
        foreach (var table in tables)
            return table.ToString().Replace("\u00A0", "&#160;");

        return string.Empty;
    }

    public ICollection<Person> getPeople()
    {
        return people;
    }

    public class Person(string firstName, string lastName, int birthYear) {
        public string firstName = firstName;

        public string lastName = lastName;

        public int birthYear = birthYear;
    }
}
