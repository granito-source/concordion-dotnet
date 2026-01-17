using Concordion.Api.Listener;

namespace Concordion.Api.Extension;

public interface ConcordionExtender {
    /**
     * <summary>
     * Adds a command to Concordion.
     * </summary>
     * <param name="namespaceUri">the URI to be used for the namespace
     * of the command.  Must not be <code>concordion.org</code>.</param>
     * <param name="commandName">the name to be used for the command.
     * The fully qualified name composed of the <code>namespaceURI</code>
     * and <code>commandName</code> must be used to reference the command
     * in the Concordion specification.</param>
     * <param name="command">the command to be executed</param>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithCommand(string namespaceUri, string commandName,
        Command command);

    /**
     * <summary>
     * Adds a listener to <code>concordion:assertEquals</code> commands.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithAssertEqualsListener(AssertEqualsListener listener);

    /**
     * Adds a listener to <code>concordion:assertTrue</code> commands.
     * @param listener
     * @return this
     */
    ConcordionExtender WithAssertTrueListener(AssertTrueListener listener);

    /**
     * Adds a listener to <code>concordion:assertFalse</code> commands.
     * @param listener
     * @return this
     */
    ConcordionExtender WithAssertFalseListener(AssertFalseListener listener);

    /**
     * Adds a listener to <code>concordion:execute</code> commands.
     * @param executeListener
     * @return this
     */
    ConcordionExtender WithExecuteListener(ExecuteListener listener);

    /**
     * Adds a listener to <code>concordion:run</code> commands.
     * @param listener
     * @return this
     */
    ConcordionExtender WithRunListener(RunListener listener);

    /**
     * Adds a listener to <code>concordion:verifyRows</code> commands.
     * @param listener
     * @return this
     */
    ConcordionExtender WithVerifyRowsListener(VerifyRowsListener listener);

    /**
     * <summary>
     * Adds a listener that is invoked when an uncaught {@link Throwable}
     * is thrown by a command, including commands that have been added
     * using {@link #withCommand(String, String, Command)}.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithExceptionListener(ExceptionCaughtListener listener);

    /**
     * <summary>
     * Adds a listener that is invoked when Concordion parses the
     * specification document, providing access to the parsed document.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithDocumentParsingListener(
        DocumentParsingListener listener);

    /**
     * <summary>
     * Adds a listener that is invoked before and after Concordion has
     * processed the specification, providing access to the specification
     * resource and root element.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithSpecificationProcessingListener(
        SpecificationProcessingListener listener);

    /**
     * <summary>
     * Adds a listener that is invoked when a Concordion instance is
     * built, providing access to the {@link Target}to which resources
     * can be written.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithBuildListener(ConcordionBuildListener listener);

    /**
     * <summary>
     * Copies a resource to the Concordion output.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithResource(string sourcePath, Resource targetResource);

    /**
     * <summary>
     * Embeds the given CSS in the Concordion output.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithEmbeddedCss(string css);

    /**
     * <summary>
     * Copies the given CSS file to the Concordion output folder, and
     * adds a link to the CSS in the &lt;head&gt; section of the
     * Concordion HTML.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithLinkedCss(string cssPath, Resource targetResource);

    /**
     * <summary>
     * Embeds the given JavaScript in the Concordion output.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithEmbeddedJavaScript(string javaScript);

    /**
     * <summary>
     * Copies the given JavaScript file to the Concordion output folder,
     * and adds a link to the JavaScript in the &lt;head&gt; section of
     * the Concordion HTML.
     * </summary>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithLinkedJavaScript(string jsPath, Resource targetResource);

    /**
     * Overrides the source that the Concordion specifications are read
     * from.
     * @param source the new source
     * @return this
     */
    ConcordionExtender WithSource(Source source);

    /**
     * <summary>
     * Overrides the target that the Concordion specifications are written
     * to.
     * </summary>
     * <param name="target">the new target</param>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithTarget(Target target);

    /**
     * <summary>
     * Overrides the locator for Concordion specifications.
     * </summary>
     * <param name="locator">the new specification locator</param>
     * <returns>this - to enable fluent interfaces</returns>
     */
    ConcordionExtender WithSpecificationLocator(SpecificationLocator locator);

    ConcordionExtender WithEvaluatorFactory(EvaluatorFactory evaluatorFactory);

    /**
     * <summary>
     * Factory method to create an instance of the Concordion class to
     * process the active specification.
     * </summary>
     * <returns>the Concordion instance to run the active specification</returns>
     */
    Concordion Build();
}
