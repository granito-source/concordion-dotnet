using Concordion.NUnit;

namespace Concordion.Spec.Examples;

[ConcordionFixture]
public class SpikeTest {
    public string getGreetingFor(string name)
    {
        return "Hello " + name + "!";
    }

    public void doSomething()
    {
    }

    public ICollection<Person> getPeople()
    {
        return new List<Person> { new("John", "Travolta") };
    }

    public class Person {
        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName;

        public string LastName;
    }
}
