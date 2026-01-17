namespace Concordion.Api.Extension;

/**
 * <summary>
 * Supplements Concordion behaviour by adding new commands, listeners or
 * output enhancements.
 * <p>
 * To use extensions, set the system property
 * <code>concordion.extensions</code> to a comma-separated list containing:
 * </p>
 * <ul>
 * <li>the fully-qualified class name of extensions to be installed, and/or</li>
 * <li>the fully-qualified class name of extension factories that will
 * create an extension.</li>
 * </ul>
 * <p>
 * If an extension is specified, it will be instantiated by Concordion.
 * </p>
 * <p>
 * All extensions and/or extension factories must be present on the classpath.
 * Extensions must implement <see cref="ConcordionExtension"/>.
 * Extension factories must implement <see cref="ConcordionExtensionFactory"/>.
 * </p>
 * </summary>
 */
public interface ConcordionExtension {
    void AddTo(ConcordionExtender concordionExtender);
}
