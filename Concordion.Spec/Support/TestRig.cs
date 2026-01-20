using Concordion.Api;
using Concordion.Api.Extension;
using Concordion.Internal;
using Concordion.Internal.Extension;

namespace Concordion.Spec.Support;

public class TestRig {
    public SpecificationConfig Configuration { get; init; } = new();

    private readonly StubSource source = new();

    private EvaluatorFactory evaluatorFactory =
        new SimpleEvaluatorFactory();

    private object? fixture;

    private StubTarget? target;

    private ConcordionExtension? extension;

    public bool HasCopiedResource(Resource resource)
    {
        return target != null && target.HasCopiedResource(resource);
    }

    public TestRig WithFixture(object fixture)
    {
        this.fixture = fixture;

        return this;
    }

    public TestRig WithExtension(ConcordionExtension extension)
    {
        this.extension = extension;

        return this;
    }

    public TestRig WithResource(Resource resource, string html)
    {
        source.AddResource(resource, html);

        return this;
    }

    public TestRig WithStubbedEvaluationResult(object? evaluationResult)
    {
        evaluatorFactory = new StubEvaluator(fixture)
            .WithStubbedResult(evaluationResult);

        return this;
    }

    public ProcessingResult Process(Resource resource)
    {
        var eventRecorder = new EventRecorder();

        target = new StubTarget();

        var concordionBuilder = new ConcordionBuilder()
            .WithEvaluatorFactory(evaluatorFactory)
            .WithSource(source)
            .WithTarget(target)
            .WithAssertEqualsListener(eventRecorder)
            .WithExceptionListener(eventRecorder);

        if (fixture != null)
            new ExtensionLoader(Configuration)
                .AddExtensions(fixture, concordionBuilder);

        extension?.AddTo(concordionBuilder);

        var concordion = concordionBuilder.Build();

        try {
            var resultSummary = concordion.Process(resource, fixture);
            var xml = target.GetWrittenString(resource);

            return new ProcessingResult(resultSummary, eventRecorder, xml);
        } catch (Exception e) {
            throw new Exception("Test rig failed to process specification", e);
        }
    }

    public ProcessingResult Process(string html)
    {
        var resource = new Resource("/testrig");

        WithResource(resource, html);

        return Process(resource);
    }

    public ProcessingResult ProcessFragment(string fragment)
    {
        return Process(WrapFragment(fragment));
    }

    private string WrapFragment(string fragment)
    {
        var wrappedFragment = "<body><fragment>" +
            fragment +
            "</fragment></body>";

        return WrapWithNamespaceDeclaration(wrappedFragment);
    }

    private string WrapWithNamespaceDeclaration(string fragment)
    {
        return "<html xmlns:concordion='" +
            ConcordionBuilder.ConcordionNamespace + "'>" +
            fragment +
            "</html>";
    }
}
