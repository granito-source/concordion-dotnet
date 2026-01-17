namespace Concordion.Api.Extension;

/**
 * <summary>
 * Creates an extension for use within Concordion. This factory mechanism
 * is typically only used for extensions that require customization.
 * <p>
 * To use an extension factory, include the fully-qualified class name of
 * the factory in the comma-separated list of extensions and extension
 * factories specified by the system property
 * <code>concordion.extensions</code>.
 * </p>
 * <p>
 * <see cref="ConcordionExtension"/>
 * </p>
 * </summary>
 */
public interface ConcordionExtensionFactory {
    ConcordionExtension CreateExtension();
}
