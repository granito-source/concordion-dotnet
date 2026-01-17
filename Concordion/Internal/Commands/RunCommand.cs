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
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Commands;

public class RunCommand : AbstractCommand {
    public Dictionary<string, Api.Runner> Runners { get; } = new();

    private readonly List<RunListener> listeners = [];

    public void AddRunListener(RunListener runListener)
    {
        listeners.Add(runListener);
    }

    public void RemoveRunListener(RunListener runListener)
    {
        listeners.Remove(runListener);
    }

    private void AnnounceIgnored(Element element)
    {
        foreach (var listener in listeners)
            listener.IgnoredReported(new RunIgnoreEvent(element));
    }

    private void AnnounceSuccess(Element element)
    {
        foreach (var listener in listeners)
            listener.SuccessReported(new RunSuccessEvent(element));
    }

    private void AnnounceFailure(Element element)
    {
        foreach (var listener in listeners)
            listener.FailureReported(new RunFailureEvent(element));
    }

    private void AnnounceError(Exception exception, Element element,
        string? expression)
    {
        foreach (var listener in listeners)
            listener.ExceptionCaught(
                new ExceptionCaughtEvent(exception, element, expression));
    }

    public override void Execute(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
        Check.IsFalse(commandCall.HasChildCommands,
            "Nesting commands inside a 'run' is not supported");

        var element = commandCall.Element;
        var href = element.GetAttributeValue("href");

        Check.NotNull(href,
            "The 'href' attribute must be set for an element containing concordion:run");

        var runnerType = commandCall.Expression;
        var expression = element.GetAttributeValue("params", "concordion");

        if (expression != null)
            evaluator.Evaluate(expression);

        try {
            Runners.TryGetValue(runnerType, out var concordionRunner);

            // TODO - re-check this.
            Check.NotNull(concordionRunner,
                $"The runner '{runnerType}' cannot be found. Choices: (1) Use 'concordion' as your runner (2) Ensure that the 'concordion.runner.{runnerType}' System property is set to a name of an IRunner implementation (3) Specify an assembly fully qualified class name of an IRunner implementation");

            var result = concordionRunner.Execute(evaluator.Fixture,
                commandCall.Resource, href).Result;

            if (result == Result.Success) {
                resultRecorder.Success();
                AnnounceSuccess(element);
            } else if (result == Result.Ignored) {
                resultRecorder.Ignore();
                AnnounceIgnored(element);
            } else {
                resultRecorder.Failure($"test {href} failed",
                    commandCall.Element.ToXml());
                AnnounceFailure(element);
            }
        } catch (Exception ex) {
            resultRecorder.Error(ex);
            AnnounceError(ex, element, expression);
        }
    }
}
