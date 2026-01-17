using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Resource;

public class ResourceExtension : ConcordionExtension {
    public const string SourcePath = "/test/concordion/o.png";

    public void AddTo(ConcordionExtender concordionExtender)
    {
        concordionExtender.WithResource(
            SourcePath,
            new Api.Resource(("/images/o.png")));
    }
}
