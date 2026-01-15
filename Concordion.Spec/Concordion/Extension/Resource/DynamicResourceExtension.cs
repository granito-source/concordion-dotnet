using Concordion.Api;
using Concordion.Api.Extension;
using Concordion.Api.Listener;

namespace Concordion.Spec.Concordion.Extension.Resource;

public class DynamicResourceExtension : IConcordionExtension,
    IConcordionBuildListener {
    private Target? target;

    public void AddTo(IConcordionExtender concordionExtender)
    {
        concordionExtender.WithBuildListener(this);
    }

    public void ConcordionBuilt(ConcordionBuildEvent buildEvent)
    {
        target = buildEvent.Target;

        // NOTE: normally this would be done during specification
        // processing, e.g. in an AssertEqualsListener
        CreateResourceInTarget();
    }

    private void CreateResourceInTarget()
    {
        using var input = new MemoryStream("success"u8.ToArray());

        target?.CopyTo(new Api.Resource("/resource/my.txt"), input);
    }
}
