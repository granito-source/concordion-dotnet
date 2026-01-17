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

public class AssertEqualsCommand(IComparer<object> comparer) : AbstractCommand {
    private readonly List<AssertEqualsListener> listeners = [];

    public AssertEqualsCommand() : this(new BrowserStyleWhitespaceComparer())
    {
    }

    public void AddAssertEqualsListener(AssertEqualsListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveAssertEqualsListener(AssertEqualsListener listener)
    {
        listeners.Add(listener);
    }

    private void AnnounceSuccess(Element element)
    {
        foreach (var assertEqualsListener in listeners)
            assertEqualsListener.SuccessReported(
                new AssertSuccessEvent(element));
    }

    private void AnnounceFailure(Element element, string expected,
        object? actual)
    {
        foreach (var assertEqualsListener in listeners)
            assertEqualsListener.FailureReported(
                new AssertFailureEvent(element, expected, actual));
    }

    public override void Verify(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
        Check.IsFalse(commandCall.HasChildCommands,
            "Nesting commands inside an 'assertEquals' is not supported");

        var element = commandCall.Element;
        var actual = evaluator.Evaluate(commandCall.Expression);
        var expected = element.Text;

        if (comparer.Compare(actual, expected) == 0) {
            resultRecorder.Success();
            AnnounceSuccess(element);
        } else {
            resultRecorder.Failure($"expected {expected} but was {actual}",
                element.ToXml());
            AnnounceFailure(element, expected, actual);
        }
    }
}
