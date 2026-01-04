using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Configuration;

class ExampleDerivedFixtureWithFieldAttributes : ExampleFixtureBaseWithFieldAttributes {
    [Extension]
    public FakeExtension1 extension = new("ExampleExtension");
}
