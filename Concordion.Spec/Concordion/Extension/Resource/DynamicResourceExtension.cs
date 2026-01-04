using Concordion.Api;
using Concordion.Api.Extension;
using Concordion.Api.Listener;

namespace Concordion.Spec.Concordion.Extension.Resource;

public class DynamicResourceExtension : IConcordionExtension, IConcordionBuildListener {
    public static readonly string SourcePath = "/test/concordion/o.png";

    private ITarget? m_Target;

    public void AddTo(IConcordionExtender concordionExtender)
    {
        concordionExtender.WithBuildListener(this);
    }

    public void ConcordionBuilt(ConcordionBuildEvent buildEvent)
    {
        m_Target = buildEvent.Target;

        // NOTE: normally this would be done during specification
        // processing, e.g. in an AssertEqualsListener
        CreateResourceInTarget();
    }

    private void CreateResourceInTarget()
    {
        m_Target?.CopyTo(new Api.Resource("/resource/my.txt"),
            new StringReader("success"));
    }
}
