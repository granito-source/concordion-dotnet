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

using Concordion.Api;

namespace Concordion.Internal.Runner;

public class DefaultConcordionRunner(Source source, Target target) : Api.Runner {
    private static object GetFixture(Resource resource, string href,
        object callingFixture)
    {
        var fixtureName = GetNameOfFixtureToRun(resource, href);
        var assembly = callingFixture.GetType().Assembly;
        var type = assembly.GetType(fixtureName, false, true) ??
            assembly.GetType(fixtureName + "Test", false, true) ??
            assembly.GetType(fixtureName + "Fixture", false, true);

        if (type != null) {
            var fixture = Activator.CreateInstance(type);

            if (fixture != null)
                return fixture;
        }

        throw new InvalidOperationException(
            $"Cannot create a fixture instance for {href}");
    }

    private static string GetNameOfFixtureToRun(Resource resource,
        string href)
    {
        var hrefResource = resource.GetRelativeResource(href);
        var fixturePath = hrefResource.Path;
        var fixtureFullyQualifiedPath = fixturePath
            .Replace(Path.DirectorySeparatorChar, '.');
        var fixtureName = fixtureFullyQualifiedPath.Replace(".html", "");

        if (fixtureName.StartsWith('.'))
            fixtureName = fixtureName.Remove(0, 1);

        return fixtureName;
    }

    public RunnerResult Execute(object callingFixture, Resource resource,
        string href)
    {
        var runnerFixture = GetFixture(resource, href, callingFixture);
        var concordion = new ConcordionBuilder()
            .WithSource(source)
            .WithTarget(target)
            .Build();
        var results = concordion.Process(runnerFixture);
        var result = results.HasFailures ?
            Result.Failure :
            results.HasExceptions ? Result.Exception : Result.Success;

        return new RunnerResult(result);
    }
}
