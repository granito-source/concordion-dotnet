using Concordion.Internal;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Extension.Configuration;

[ConcordionFixture]
public class ExtensionConfigurationTest {
    private SpecificationConfig Configuration { get; } = new();

    public string Process(string fixtureNameSpace, string fixtureClassName)
    {
        var fullClassName = string.Concat(fixtureNameSpace, ".", fixtureClassName);
        var fixtureType = Type.GetType(fullClassName);
        var fixture = Activator.CreateInstance(fixtureType!);
        var testRig = new TestRig {
            Configuration = Configuration
        };
        var processingResult = testRig
            .WithFixture(fixture!)
            .ProcessFragment("<p>anything..</p>");
        var extensionNamesString = processingResult
            .GetRootElement()
            .GetAttributeValue(FakeExtensionBase.FakeExtensionAttrName);
        var extensionNames = extensionNamesString?
            .Split(',')
            .Select(extensionName => extensionName.Trim())
            .ToList();

        extensionNames?.Sort();

        return string.Join(", ", extensionNames?.ToArray() ?? []);
    }

    public void LoadConfiguration(string configContent)
    {
        new SpecificationConfigParser(Configuration).Parse(new StringReader(configContent));
    }

    public string Process()
    {
        var exampleFixtureType = typeof(ExampleFixtureWithoutExtensions);

        return Process(exampleFixtureType.Namespace!,
            exampleFixtureType.Name);
    }
}
