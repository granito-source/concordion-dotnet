using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Configuration;

class ExampleDerivedFixtureWithFieldAttributes : ExampleFixtureBaseWithFieldAttributes {
    [Extension]
    public readonly FakeExtension1 extension = new("ExampleExtension");
}
