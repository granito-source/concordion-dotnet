/*
 * Copyright 2026 Alexei Yashkov
 * Copyright 2009 Jeffrey Cameron
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Concordion.Api;
using Concordion.Api.Extension;
using Concordion.Api.Listener;
using Concordion.Internal.Commands;
using Concordion.Internal.Listener;
using Concordion.Internal.Runner;
using Concordion.Internal.Util;

namespace Concordion.Internal;

public class ConcordionBuilder : ConcordionExtender {
    private bool builtAlready;

    private ExceptionRenderer exceptionRenderer;

    private Source source;

    private Target target;

    private SpecificationLocator specificationLocator;

    private readonly CommandRegistry commandRegistry;

    private readonly DocumentParser documentParser;

    private EvaluatorFactory evaluatorFactory;

    private readonly SpecificationCommand specificationCommand;

    private readonly AssertEqualsCommand assertEqualsCommand;

    private readonly AssertTrueCommand assertTrueCommand;

    private readonly AssertFalseCommand assertFalseCommand;

    private readonly ExecuteCommand executeCommand;

    private readonly RunCommand runCommand;

    private readonly VerifyRowsCommand verifyRowsCommand;

    private readonly EchoCommand echoCommand;

    private readonly List<ConcordionBuildListener> buildListeners;

    private readonly List<SpecificationProcessingListener> specificationProcessingListeners;

    private readonly List<ExceptionCaughtListener> exceptionListeners;

    private readonly Dictionary<string, Resource> resourceToCopyMap;

    public ConcordionBuilder()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        source = new FileSource(currentDirectory);
        target = new FileTarget(currentDirectory);
        resourceToCopyMap = new Dictionary<string, Resource>();
        buildListeners = [];
        specificationProcessingListeners = [];
        exceptionListeners = [];
        specificationLocator = new ClassNameBasedSpecificationLocator();
        commandRegistry = new CommandRegistry();
        documentParser = new DocumentParser(commandRegistry);
        evaluatorFactory = new SimpleEvaluatorFactory();
        specificationCommand = new SpecificationCommand();
        assertEqualsCommand = new AssertEqualsCommand();
        assertTrueCommand = new AssertTrueCommand();
        assertFalseCommand = new AssertFalseCommand();
        executeCommand = new ExecuteCommand();
        runCommand = new RunCommand();
        verifyRowsCommand = new VerifyRowsCommand();
        echoCommand = new EchoCommand();
        exceptionRenderer = new ExceptionRenderer();
        WithExceptionListener(exceptionRenderer);

        // Set up the commands
        commandRegistry.Register("", "specification", specificationCommand);

        // Wire up the command listeners
        var assertResultRenderer = new AssertResultRenderer();

        WithAssertEqualsListener(assertResultRenderer);
        WithAssertTrueListener(assertResultRenderer);
        WithAssertFalseListener(assertResultRenderer);
        WithVerifyRowsListener(new VerifyRowResultRenderer());
        WithRunListener(new RunResultRenderer());
        WithDocumentParsingListener(new DocumentStructureImprover());
        WithDocumentParsingListener(new MetadataCreator());
        WithEmbeddedCss(HtmlFramework.EMBEDDED_STYLESHEET_RESOURCE);
    }

    public ConcordionExtender WithSource(Source aSource)
    {
        source = aSource;

        return this;
    }

    public ConcordionExtender WithTarget(Target aTarget)
    {
        target = aTarget;

        return this;
    }

    public ConcordionExtender WithSpecificationLocator(
        SpecificationLocator locator)
    {
        specificationLocator = locator;

        return this;
    }

    public ConcordionExtender WithEvaluatorFactory(EvaluatorFactory factory)
    {
        evaluatorFactory = factory;

        return this;
    }

    public ConcordionBuilder WithExceptionRenderer(ExceptionRenderer renderer)
    {
        exceptionRenderer = renderer;

        return this;
    }

    public ConcordionExtender WithAssertEqualsListener(
        AssertEqualsListener listener)
    {
        assertEqualsCommand.AddAssertEqualsListener(listener);

        return this;
    }

    public ConcordionExtender WithAssertTrueListener(
        AssertTrueListener listener)
    {
        assertTrueCommand.AddAssertListener(listener);

        return this;
    }

    public ConcordionExtender WithAssertFalseListener(
        AssertFalseListener listener)
    {
        assertFalseCommand.AddAssertListener(listener);

        return this;
    }

    private ConcordionBuilder WithApprovedCommand(string namespaceUri,
        string commandName, Command command)
    {
        var decoratedCommand = new ExceptionCatchingDecorator(
            new LocalTextDecorator(command));

        exceptionListeners.ForEach(decoratedCommand.AddExceptionListener);
        commandRegistry.Register(namespaceUri, commandName, decoratedCommand);

        return this;
    }

    public ConcordionExtender WithVerifyRowsListener(
        VerifyRowsListener listener)
    {
        verifyRowsCommand.AddVerifyRowsListener(listener);

        return this;
    }

    public ConcordionExtender WithRunListener(RunListener listener)
    {
        runCommand.AddRunListener(listener);

        return this;
    }

    public ConcordionExtender WithExecuteListener(ExecuteListener listener)
    {
        executeCommand.AddExecuteListener(listener);

        return this;
    }

    public ConcordionExtender WithDocumentParsingListener(
        DocumentParsingListener listener)
    {
        documentParser.AddDocumentParsingListener(listener);

        return this;
    }

    public ConcordionExtender WithSpecificationProcessingListener(
        SpecificationProcessingListener listener)
    {
        specificationProcessingListeners.Add(listener);

        return this;
    }

    public ConcordionExtender WithBuildListener(
        ConcordionBuildListener listener)
    {
        buildListeners.Add(listener);

        return this;
    }

    public ConcordionExtender WithCommand(string namespaceUri,
        string commandName, Command command)
    {
        Check.NotEmpty(namespaceUri, "Namespace URI is mandatory");
        Check.NotEmpty(commandName, "Command name is mandatory");
        Check.NotNull(command, "Command is null");
        Check.IsFalse(namespaceUri.StartsWith("Concordion"),
            $"The namespace URI for user-contributed command '{commandName}' must not start with 'Concordion'. Use your own domain name instead.");

        return WithApprovedCommand(namespaceUri, commandName, command);
    }

    public ConcordionExtender WithResource(string sourcePath,
        Resource targetResource)
    {
        resourceToCopyMap.Add(sourcePath, targetResource);

        return this;
    }

    public ConcordionExtender WithEmbeddedCss(string css)
    {
        var embedder = new StylesheetEmbedder(css);

        WithDocumentParsingListener(embedder);

        return this;
    }

    public ConcordionExtender WithLinkedCss(string cssPath,
        Resource targetResource)
    {
        WithResource(cssPath, targetResource);

        var cssLinker = new StylesheetLinker(targetResource);

        WithDocumentParsingListener(cssLinker);
        WithSpecificationProcessingListener(cssLinker);

        return this;
    }

    public ConcordionExtender WithEmbeddedJavaScript(string javaScript)
    {
        var embedder = new JavaScriptEmbedder(javaScript);

        WithDocumentParsingListener(embedder);

        return this;
    }

    public ConcordionExtender WithLinkedJavaScript(string jsPath,
        Resource targetResource)
    {
        WithResource(jsPath, targetResource);

        var javaScriptLinker = new JavaScriptLinker(targetResource);

        WithDocumentParsingListener(javaScriptLinker);
        WithSpecificationProcessingListener(javaScriptLinker);

        return this;
    }

    public Concordion Build()
    {
        Check.IsFalse(builtAlready,
            "ConcordionBuilder currently does not support calling Build() twice");

        builtAlready = true;

        WithApprovedCommand(HtmlFramework.NAMESPACE_CONCORDION_2007,
            "run", runCommand);
        WithApprovedCommand(HtmlFramework.NAMESPACE_CONCORDION_2007,
            "execute", executeCommand);
        WithApprovedCommand(HtmlFramework.NAMESPACE_CONCORDION_2007,
            "set", new SetCommand());
        WithApprovedCommand(HtmlFramework.NAMESPACE_CONCORDION_2007,
            "assertEquals", assertEqualsCommand);
        WithApprovedCommand(HtmlFramework.NAMESPACE_CONCORDION_2007,
            "assertTrue", assertTrueCommand);
        WithApprovedCommand(HtmlFramework.NAMESPACE_CONCORDION_2007,
            "assertFalse", assertFalseCommand);
        WithApprovedCommand(HtmlFramework.NAMESPACE_CONCORDION_2007,
            "verifyRows", verifyRowsCommand);
        WithApprovedCommand(HtmlFramework.NAMESPACE_CONCORDION_2007,
            "echo", echoCommand);

        SetAllRunners();

        specificationCommand.AddSpecificationListener(new BreadCrumbRenderer(source));
        specificationCommand.AddSpecificationListener(new PageFooterRenderer());
        specificationCommand.AddSpecificationListener(new SpecificationRenderer(target));

        CopyResources();
        AddSpecificationListeners();

        foreach (var concordionBuildListener in buildListeners)
            concordionBuildListener.ConcordionBuilt(new ConcordionBuildEvent(target));

        return new Concordion(specificationLocator,
            new XmlSpecificationReader(source, documentParser),
            evaluatorFactory);
    }

    private void AddSpecificationListeners()
    {
        foreach (var listener in specificationProcessingListeners)
            specificationCommand.AddSpecificationListener(listener);
    }

    private void CopyResources()
    {
        foreach (var (sourcePath, targetResource) in resourceToCopyMap) {
            using var stream = source.CreateStream(new Resource(sourcePath));

            target.CopyTo(targetResource, stream);
        }
    }

    private void SetAllRunners()
    {
        runCommand.Runners.Add("concordion", new DefaultConcordionRunner(source, target));

        var config = new ConcordionConfig().Load();

        foreach (var runner in config.Runners)
            runCommand.Runners.Add(runner.Key, runner.Value);
    }

    public ConcordionExtender WithExceptionListener(ExceptionCaughtListener listener)
    {
        exceptionListeners.Add(listener);

        return this;
    }
}
