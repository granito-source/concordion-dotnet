using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Configuration;

public class ExampleFixtureBaseWithFieldAttributes {
    [Extension]
    public FakeExtension2 extension2 = new("SuperExtension");
}
