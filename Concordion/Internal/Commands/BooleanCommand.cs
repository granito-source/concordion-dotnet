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

public abstract class BooleanCommand : AbstractCommand
{
    private readonly List<IAssertListener> m_Listeners = [];

    #region Methods

    public void AddAssertListener(IAssertListener listener)
    {
        m_Listeners.Add(listener);
    }

    public void RemoveAssertListener(IAssertListener listener)
    {
        m_Listeners.Remove(listener);
    }

    protected void AnnounceSuccess(Element element)
    {
        foreach (var assertListener in m_Listeners)
        {
            assertListener.SuccessReported(new AssertSuccessEvent(element));
        }
    }

    protected void AnnounceFailure(Element element, string expected, object actual)
    {
        foreach (var assertListener in m_Listeners)
        {
            assertListener.FailureReported(new AssertFailureEvent(element, expected, actual));
        }
    }

    protected abstract void ProcessFalseResult(CommandCall commandCall,IResultRecorder resultRecorder);

    protected abstract void ProcessTrueResult(CommandCall commandCall, IResultRecorder resultRecorder);

    #endregion

    #region ICommand Members

    public override void Verify(CommandCall commandCall, IEvaluator evaluator, IResultRecorder resultRecorder)
    {
        var childCommands = commandCall.Children;

        childCommands.SetUp(evaluator, resultRecorder);
        childCommands.Execute(evaluator, resultRecorder);
        childCommands.Verify(evaluator, resultRecorder);

        var expression = commandCall.Expression;
        var result = evaluator.Evaluate(expression);

        if (result != null && result is bool)
        {
            if ((bool) result)
            {
                ProcessTrueResult(commandCall, resultRecorder);
            }
            else
            {
                ProcessFalseResult(commandCall, resultRecorder);
            }
        }
        else
        {
            throw new InvalidExpressionException("Expression '" + expression + "' did not produce a boolean result (needed for assertTrue).");
        }
    }

    #endregion
}
