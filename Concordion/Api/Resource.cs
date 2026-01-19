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

namespace Concordion.Api;

/// <summary>
/// Represents a physical file on the filesystem.
/// </summary>
public class Resource {
    private const char Separator = '/';

    private static string StripSeparator(string name)
    {
        return name.EndsWith(Separator) ? name[..^1] : name;
    }

    private readonly Uri uri;

    public string Path => uri.AbsolutePath;

    public string Name => StripSeparator(uri.Segments[^1]);

    public Resource? Parent {
        get {
            var segments = uri.Segments[..^1];

            return segments.Length > 0 ?
                new Resource(string.Join(null, segments)) :
                null;
        }
    }

    public Resource(string path)
    {
        if (!path.StartsWith(Separator))
            throw new ArgumentException(@"resource path must be absolute",
                nameof(path));

        uri = new Uri($"file://{path}");
    }

    /// <summary>
    /// Gets a resource relative to this one based on the path
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
    /// <returns></returns>
    public Resource GetRelativeResource(string relativePath)
    {
        return new Resource(new Uri(uri, relativePath).AbsolutePath);
    }

    public string GetRelativePath(Resource resource)
    {
        return uri.MakeRelativeUri(resource.uri).OriginalString;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj)
            return true;

        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (Resource)obj;

        return uri.Equals(other.uri);
    }

    public override int GetHashCode()
    {
        return uri.GetHashCode();
    }
}
