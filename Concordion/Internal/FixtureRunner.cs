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

using System.Diagnostics;
using Concordion.Api;
using Concordion.Internal.Extension;

namespace Concordion.Internal;

public class FixtureRunner(SpecificationConfig? config = null) {
    private static void AddToTestResults(ResultSummary singleResult,
        SummarizingResultRecorder resultSummary)
    {
        if (singleResult.HasExceptions)
            resultSummary.AddResultDetails(singleResult.ErrorDetails);
        else if (singleResult.HasFailures)
            resultSummary.AddResultDetails(singleResult.FailureDetails);
        else
            resultSummary.Success();
    }

    private SpecificationConfig? runningConfig = config;

    private object? runningFixture;

    private Source? runningSource;

    private FileTarget? runningTarget;

    public ResultSummary Run(object fixture)
    {
        try {
            runningFixture = fixture;
            runningConfig ??= new SpecificationConfig()
                .Load(fixture.GetType());
            runningSource = string.IsNullOrEmpty(runningConfig.BaseInputDirectory) ?
                new EmbeddedResourceSource(fixture.GetType().Assembly) :
                new FileSource(runningConfig.BaseInputDirectory);
            runningTarget = new FileTarget(runningConfig.BaseOutputDirectory);

            var fileExtensions = runningConfig.SpecificationFileExtensions;

            if (fileExtensions.Count > 1)
                return RunAllSpecifications(fileExtensions);

            if (fileExtensions.Count == 1)
                return RunSingleSpecification(fileExtensions.First());

            throw new InvalidOperationException(
                $"no specification extensions defined for: {runningConfig}");
        } catch (Exception ex) {
            Console.WriteLine(ex);

            var exceptionResult = new SummarizingResultRecorder();

            exceptionResult.Error(ex);

            return exceptionResult;
        }
    }

    private SummarizingResultRecorder RunAllSpecifications(
        IEnumerable<string> fileExtensions)
    {
        Debug.Assert(runningConfig != null, nameof(runningConfig) + " != null");
        Debug.Assert(runningFixture != null, nameof(runningFixture) + " != null");
        Debug.Assert(runningSource != null, nameof(runningSource) + " != null");

        var testSummary = new SummarizingResultRecorder();
        var anySpecExecuted = false;

        foreach (var fileExtension in fileExtensions) {
            var specLocator = new ClassNameBasedSpecificationLocator(fileExtension);
            var specResource = specLocator.LocateSpecification(runningFixture);


            if (!runningSource.CanFind(specResource))
                continue;

            var fixtureResult = RunSingleSpecification(fileExtension);

            AddToTestResults(fixtureResult, testSummary);
            anySpecExecuted = true;
        }

        if (anySpecExecuted)
            return testSummary;

        var specPath = !string.IsNullOrEmpty(runningConfig.BaseInputDirectory) ?
            $"directory {Path.GetFullPath(runningConfig.BaseInputDirectory)}" :
            $"assembly {runningFixture.GetType().Assembly.GetName().Name}";

        testSummary.Error(new AssertionErrorException(
            $"no active specification found for {runningFixture.GetType().Name} in {specPath}"));

        return testSummary;
    }

    private ResultSummary RunSingleSpecification(string fileExtension)
    {
        Debug.Assert(runningConfig != null, nameof(runningConfig) + " != null");
        Debug.Assert(runningFixture != null, nameof(runningFixture) + " != null");
        Debug.Assert(runningSource != null, nameof(runningSource) + " != null");
        Debug.Assert(runningTarget != null, nameof(runningTarget) + " != null");

        var locator = new ClassNameBasedSpecificationLocator(fileExtension);
        var concordionExtender = new ConcordionBuilder();

        concordionExtender
            .WithSource(runningSource)
            .WithTarget(runningTarget)
            .WithSpecificationLocator(locator);

        var extensionLoader = new ExtensionLoader(runningConfig);

        extensionLoader.AddExtensions(runningFixture, concordionExtender);

        var concordion = concordionExtender.Build();

        return concordion.Process(runningFixture);
    }
}
