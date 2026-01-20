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

using System.Text;
using Concordion.Api;

namespace Concordion.Internal;

public class FileSource(string baseDirectory) : Source {
    private readonly string baseDirectory = Path.GetFullPath(baseDirectory);

    public Stream CreateStream(Resource resource)
    {
        var path = ExistingFilePath(resource);

        if (path == null)
            throw new InvalidOperationException(
                $"Cannot open the resource {resource.Path}");


        return new FileStream(path, FileMode.Open);
    }

    public bool CanFind(Resource resource)
    {
        return ExistingFilePath(resource) != null;
    }

    public string ReadResourceAsString(Resource resource)
    {
        using var reader = new StreamReader(CreateStream(resource),
            Encoding.UTF8);

        return reader.ReadToEnd();
    }

    private string? ExistingFilePath(Resource resource)
    {
        var path = Path.Combine(baseDirectory, resource.Path[1..]);

        return File.Exists(path) ? path : null;
    }
}
