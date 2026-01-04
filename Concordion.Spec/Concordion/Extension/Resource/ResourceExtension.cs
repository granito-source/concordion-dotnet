using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Resource;

public class ResourceExtension : IConcordionExtension {
    public const string SourcePath = "/test/concordion/o.png";

    public void AddTo(IConcordionExtender concordionExtender)
    {
        concordionExtender.WithResource(
            SourcePath,
            new Api.Resource(("/images/o.png")));
    }
}
