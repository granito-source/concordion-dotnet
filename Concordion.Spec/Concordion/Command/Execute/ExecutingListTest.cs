using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Execute;

[ConcordionFixture]
public class ExecutingListTest {
    private readonly Queue<ParseParameters> nodes = new();

    public void ParseNode(string text, int level)
    {
        var parseParameters = new ParseParameters {
            Name = text,
            Level = level
        };

        nodes.Enqueue(parseParameters);
    }

    public struct ParseParameters {
        public string Name;

        public int Level;
    }

    public Queue<ParseParameters> GetNodes()
    {
        return nodes;
    }

    public void process(string fragment)
    {
        new TestRig()
            .WithFixture(this)
            .ProcessFragment(fragment);
    }
}
