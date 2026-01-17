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

using Concordion.Api;

namespace Concordion.Internal;

public class SummarizingResultRecorder : ResultRecorder, ResultSummary {
    private readonly List<ResultDetails> results = [];

    /// <summary>
    /// Gets the success count.
    /// </summary>
    /// <value>The success count.</value>
    public long SuccessCount =>
        results.LongCount(result => result.IsSuccess);

    /// <summary>
    /// Gets the failure count.
    /// </summary>
    /// <value>The failure count.</value>
    public long FailureCount =>
        results.LongCount(result => result.IsFailure);

    /// <summary>
    /// Gets the exception count.
    /// </summary>
    /// <value>The exception count.</value>
    public long ExceptionCount =>
        results.LongCount(result => result.IsError);

    /// <summary>
    /// Gets a value indicating whether this instance has exceptions.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has exceptions; otherwise, <c>false</c>.
    /// </value>
    public bool HasExceptions => ExceptionCount > 0;

    /// <summary>
    /// Gets a value indicating whether this instance has failures.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has failures; otherwise, <c>false</c>.
    /// </value>
    public bool HasFailures => FailureCount > 0;

    public List<ResultDetails> FailureDetails =>
        results.Where(result => result.IsFailure).ToList();

    public List<ResultDetails> ErrorDetails =>
        results.Where(result => result.IsError).ToList();

    public void Success()
    {
        results.Add(new ResultDetails(Result.Success));
    }

    public void Failure(string message, string stackTrace)
    {
        results.Add(new ResultDetails(Result.Failure,
            message, stackTrace));
    }

    public void Error(Exception exception)
    {
        results.Add(new ResultDetails(Result.Exception,
            exception));
    }

    public void Ignore()
    {
        results.Add(new ResultDetails(Result.Ignored));
    }

    public void AddResultDetails(List<ResultDetails> resultDetails)
    {
        results.AddRange(resultDetails);
    }

    /// <summary>
    /// Asserts the specification is satisfied.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    public void AssertIsSatisfied(object fixture)
    {
        DetermineFixtureState(fixture)
            .AssertIsSatisfied(SuccessCount, FailureCount, ExceptionCount);
    }

    /// <summary>
    /// Prints the results.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="fixture">The fixture.</param>
    public void Print(TextWriter writer, object fixture)
    {
        writer.Write("Successes: {0}, Failures: {1}", SuccessCount,
            FailureCount);

        if (HasExceptions)
            writer.Write(", Exceptions: {0}", ExceptionCount);

        writer.WriteLine();
        writer.Flush();
    }

    private FixtureState DetermineFixtureState(object fixture)
    {
        var attributes = fixture.GetType().GetCustomAttributes(false);

        if (attributes.Contains(typeof(UnimplementedAttribute)))
            return new UnimplementedFixtureState();

        if (attributes.Contains(typeof(ExpectedToFailAttribute)))
            return new ExpectedToFailFixtureState();

        return new ExpectedToPassFixtureState();
    }
}
