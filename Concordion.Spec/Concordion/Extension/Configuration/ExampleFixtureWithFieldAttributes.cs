using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Configuration;

public class ExampleFixtureWithFieldAttributes {
    [Extension]
    public FakeExtension1 extension = new();

    [Extension]
    public FakeExtension2 extension2 = new();
}
