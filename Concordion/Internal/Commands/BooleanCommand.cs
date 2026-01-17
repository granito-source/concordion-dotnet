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

using System.Data;
using Concordion.Api;
using Concordion.Api.Listener;

namespace Concordion.Internal.Commands;

public abstract class BooleanCommand : AbstractCommand {
    private readonly List<AssertListener> listeners = [];

    public void AddAssertListener(AssertListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveAssertListener(AssertListener listener)
    {
        listeners.Remove(listener);
    }

    protected void AnnounceSuccess(Element element)
    {
        foreach (var assertListener in listeners)
            assertListener.SuccessReported(new AssertSuccessEvent(element));
    }

    protected void AnnounceFailure(Element element, string expected,
        object actual)
    {
        foreach (var assertListener in listeners)
            assertListener.FailureReported(
                new AssertFailureEvent(element, expected, actual));
    }

    protected abstract void ProcessFalseResult(CommandCall commandCall,
        ResultRecorder resultRecorder);

    protected abstract void ProcessTrueResult(CommandCall commandCall,
        ResultRecorder resultRecorder);

    public override void Verify(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
        var childCommands = commandCall.Children;

        childCommands.SetUp(evaluator, resultRecorder);
        childCommands.Execute(evaluator, resultRecorder);
        childCommands.Verify(evaluator, resultRecorder);

        var expression = commandCall.Expression;
        var result = evaluator.Evaluate(expression);

        if (result is not bool boolResult)
            throw new InvalidExpressionException(
                $"Expression '{expression}' did not produce a boolean result (needed for assertTrue).");

        if (boolResult)
            ProcessTrueResult(commandCall, resultRecorder);
        else
            ProcessFalseResult(commandCall, resultRecorder);
    }
}
