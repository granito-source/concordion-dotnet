// Copyright 2009 Jeffrey Cameron
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Reflection;
using Concordion.Api.Extension;

namespace Concordion.Internal;

/// <summary>
/// Loads the configuration file for a specification assembly
/// </summary>
public class SpecificationConfig {
    #region Properties

    /// <summary>
    /// Gets or sets the base input directory.
    /// </summary>
    /// <value>The base input directory.</value>
    public string? BaseInputDirectory { get; set; } =
        null; //a value of null indicates that specifications are embedded in DLL file

    /// <summary>
    /// Gets or sets the base output directory.
    /// </summary>
    /// <value>The base output directory.</value>
    public string? BaseOutputDirectory { get; set; } =
        Environment.GetEnvironmentVariable("TEMP");

    /// <summary>
    /// Gets or sets names of extensions.
    /// </summary>
    /// <seealso cref="IConcordionExtension"/>
    /// <value>Qualified type names together with assembly names of Concordion extensions.</value>
    public IDictionary<string, string> ConcordionExtensions { get; set; } =
        new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the suffix to be used for specification files.
    /// </summary>
    /// <value>The file suffix of specification documents (e.g. "html").</value>
    public List<string> SpecificationFileExtensions { get; set; } = ["html"];

    #endregion

    #region Methods

    /// <summary>
    /// Loads the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public SpecificationConfig Load(Type type)
    {
        Load(type.Assembly);

        return this;
    }

    /// <summary>
    /// Loads the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns></returns>
    public SpecificationConfig Load(Assembly assembly)
    {
        var assemblyCodebase = new Uri(assembly.CodeBase);

        if (assemblyCodebase.IsFile)
            Load(assemblyCodebase.LocalPath);

        return this;
    }

    /// <summary>
    /// Loads the specified path to assembly.
    /// </summary>
    /// <param name="pathToAssembly">The path to assembly.</param>
    /// <returns></returns>
    private SpecificationConfig Load(string pathToAssembly)
    {
        var configFileName = Path.ChangeExtension(pathToAssembly, ".config");

        if (File.Exists(configFileName)) {
            var specificationConfigParser = new SpecificationConfigParser(this);

            specificationConfigParser.Parse(new StreamReader(configFileName));
        }

        return this;
    }

    #endregion
}
