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

namespace Concordion.Internal;

/// <summary>
/// Parses the Concordion.config file and stores the results in
/// a <see cref="ConcordionConfig"/> object.
/// </summary>
public class ConcordionConfigParser(ConcordionConfig config) {
    /// <summary>
    /// Parses the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    public void Parse(TextReader reader)
    {
        LoadConfiguration(XDocument.Load(reader));
    }

    private void LoadConfiguration(XDocument document)
    {
        var root = document.Root;

        if (root == null)
            return;

        if (root.Name == "Concordion")
            LoadRunners(root);
    }

    private void LoadRunners(XElement element)
    {
        var runners = element.Element("Runners");

        if (runners == null)
            return;

        foreach (var runner in runners.Elements("Runner")) {
            var alias = runner.Attribute("alias");
            var type = runner.Attribute("type");

            if (alias == null || type == null)
                continue;

            var runnerType = Type.GetType(type.Value);

            if (runnerType == null)
                continue;

            var runnerObject = Activator.CreateInstance(runnerType);

            if (runnerObject is Api.Runner apiRunner)
                config.Runners.Add(alias.Value, apiRunner);
        }
    }
}
