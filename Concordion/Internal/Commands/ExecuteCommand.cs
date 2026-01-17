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

namespace Concordion.Internal.Commands;

public class ExecuteCommand : Command {
    private readonly List<ExecuteListener> listeners = [];

    public void AddExecuteListener(ExecuteListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveExecuteListener(ExecuteListener listener)
    {
        listeners.Remove(listener);
    }

    public void AnnounceExecuteCompleted(Element element)
    {
        foreach (var listener in listeners)
            listener.ExecuteCompleted(new ExecuteEvent(element));
    }

    public void Setup(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
    }

    public void Execute(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
        ExecuteStrategy strategy;

        if (commandCall.Element.IsNamed("table"))
            strategy = new TableExecuteStrategy();
        else if (commandCall.Element.IsNamed("ol") ||
            commandCall.Element.IsNamed("ul"))
            strategy = new ListExecuteStrategy();
        else
            strategy = new DefaultExecuteStrategy(this);

        strategy.Execute(commandCall, evaluator, resultRecorder);
    }

    public void Verify(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
    }
}
