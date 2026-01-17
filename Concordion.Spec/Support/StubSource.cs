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

namespace Concordion.Spec.Support;

internal class StubSource : Source {
    private readonly Dictionary<Resource, string> resources = new();

    public void AddResource(Resource resource, string content)
    {
        resources[resource] = content;
    }

    public Stream CreateStream(Resource resource)
    {
        resources.TryGetValue(resource, out var content);

        if (content == null)
            throw new InvalidOperationException(
                $"Cannot open the resource {resource.Path}");

        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    public bool CanFind(Resource resource)
    {
        return resources.ContainsKey(resource);
    }
}
