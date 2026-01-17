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

using Concordion.Api;
using Concordion.Internal.Util;

namespace Concordion.Spec.Support;

internal class StubTarget : Target {
    private readonly Dictionary<Resource, string> resources = new();

    private readonly List<Resource> copiedResources = [];

    public void Write(Resource target, string content)
    {
        resources.Add(target, content);
    }

    public void CopyTo(Resource target, Stream source)
    {
        copiedResources.Add(target);
    }

    public string ResolvedPathFor(Resource resource)
    {
        return "";
    }

    public string GetWrittenString(Resource resource)
    {
        Check.IsTrue(resources.ContainsKey(resource),
            "Expected resource '" + resource.Path +
            "' was not written to target");

        return resources[resource];
    }

    public bool HasCopiedResource(Resource resource)
    {
        return copiedResources.Contains(resource);
    }
}
