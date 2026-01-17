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

using System.Xml.Linq;
using Concordion.Internal.Util;

namespace Concordion.Internal;

public class SpecificationConfigParser(SpecificationConfig config) {
    /// <summary>
    /// Parses the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    public void Parse(TextReader reader)
    {
        LoadConfiguration(XDocument.Load(reader));
    }

    /// <summary>
    /// Loads the configuration.
    /// </summary>
    /// <param name="document">The document.</param>
    private void LoadConfiguration(XDocument document)
    {
        var configElement = document.Root;

        Check.NotNull(configElement, "root element may not be null");

        if (configElement.Name != "Specification")
            return;

        LoadBaseInputDirectory(configElement);
        LoadBaseOutputDirectory(configElement);
        LoadConcordionExtensions(configElement);
        LoadSpecificationSuffixes(configElement);
    }

    /// <summary>
    /// Loads the base output directory.
    /// </summary>
    /// <param name="element">The element.</param>
    private void LoadBaseOutputDirectory(XElement element)
    {
        var baseOutputDirectory = element.Element("BaseOutputDirectory");
        var pathAttribute = baseOutputDirectory?.Attribute("path");

        if (pathAttribute != null)
            config.BaseOutputDirectory = pathAttribute.Value;
    }

    /// <summary>
    /// Loads the base input directory.
    /// </summary>
    /// <param name="element">The element.</param>
    private void LoadBaseInputDirectory(XElement element)
    {
        var baseInputDirectory = element.Element("BaseInputDirectory");
        var pathAttribute = baseInputDirectory?.Attribute("path");

        if (pathAttribute != null)
            config.BaseInputDirectory = pathAttribute.Value;
    }

    private void LoadConcordionExtensions(XElement element)
    {
        var extensions = element.Element("ConcordionExtensions");

        if (extensions == null)
            return;

        config.ConcordionExtensions.Clear();

        foreach (var extensionDefinition in extensions.Elements("Extension")) {
            var type = extensionDefinition.Attribute("type");
            var assembly = extensionDefinition.Attribute("assembly");

            if (assembly != null && type != null)
                config.ConcordionExtensions.Add(type.Value, assembly.Value);
        }
    }

    private void LoadSpecificationSuffixes(XElement element)
    {
        var suffixes = element.Element("SpecificationFileExtensions");

        if (suffixes == null)
            return;

        config.SpecificationFileExtensions.Clear();

        foreach (var suffix in suffixes.Elements("FileExtension")) {
            var name = suffix.Attribute("name");

            if (name != null)
                config.SpecificationFileExtensions.Add(name.Value);
        }
    }
}
