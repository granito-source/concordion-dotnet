/*
 * Copyright 2026 Alexei Yashkov
 * Copyright 2010-2015 concordion.org
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

using System.Reflection;
using Concordion.Api;

namespace Concordion.Internal;

public class EmbeddedResourceSource(Assembly assembly) : Source {
    private static string ConvertPathToNamespace(string path)
    {
        var name = path.Replace(Path.DirectorySeparatorChar, '.');

        if (name[0] == '.')
            name = name.Remove(0, 1);

        return name;
    }

    public Stream CreateStream(Resource resource)
    {
        var name = ConvertPathToNamespace(resource.Path);

        return assembly.GetManifestResourceStream(name) ??
            throw new InvalidOperationException(
                $"Cannot open the resource {name}");
    }

    public bool CanFind(Resource resource)
    {
        var name = ConvertPathToNamespace(resource.Path);

        return assembly.GetManifestResourceInfo(name) != null;
    }
}
