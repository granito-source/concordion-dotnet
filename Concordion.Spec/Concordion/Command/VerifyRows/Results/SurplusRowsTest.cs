using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Command.VerifyRows.Results;

[ConcordionFixture]
public class SurplusRowsTest : MissingRowsTest {
    public void addPerson(string firstName, string lastName)
    {
        base.addPerson(firstName, lastName, 1973);
    }
}
