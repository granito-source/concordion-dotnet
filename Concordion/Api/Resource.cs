/*
 * Copyright 2026 Alexei Yashkov
 * Copyright 2010-2015 concordion.org
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

using Concordion.Internal;
using static System.IO.Path;

namespace Concordion.Api;

/// <summary>
/// Represents a physical file on the filesystem.
/// </summary>
public class Resource(string path, string assemblyName = "") {
    public string Path { get; } = path;

    public string Name => GetFileName(TrimEndingDirectorySeparator(Path));

    public string ReducedPath => Path.RemoveFirst(
        assemblyName.Replace('.', DirectorySeparatorChar) +
        DirectorySeparatorChar);

    public Resource? Parent {
        get {
            if (Path.Equals(GetPathRoot(Path)))
                return null;

            var parent = GetDirectoryName(
                TrimEndingDirectorySeparator(Path));

            if (!EndsInDirectorySeparator(parent))
                parent += DirectorySeparatorChar;

            return new Resource(parent, assemblyName);
        }
    }

    /// <summary>
    /// Gets a resource relative to this one based on the path
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
    /// <returns></returns>
    public Resource GetRelativeResource(string relativePath)
    {
        var package = EndsInDirectorySeparator(Path) ? this : Parent;
        var relative = GetFullPath(Combine(package!.Path, relativePath));

        return new Resource(relative, assemblyName);
    }

    public string GetRelativePath(Resource resource)
    {
        var root = GetPathRoot(Path);
        var dir = Path.Equals(root) ? root : GetDirectoryName(Path);

        return System.IO.Path.GetRelativePath(dir!, resource.Path);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj)
            return true;

        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (Resource)obj;

        return Path.Equals(other.Path);
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }
}
