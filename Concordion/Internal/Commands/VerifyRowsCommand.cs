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

using System.Collections;
using System.Text.RegularExpressions;
using Concordion.Api;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Commands;

public class VerifyRowsCommand : Command {
    private readonly List<VerifyRowsListener> listeners = [];

    public void AddVerifyRowsListener(VerifyRowsListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveVerifyRowsListener(VerifyRowsListener listener)
    {
        listeners.Remove(listener);
    }

    private void AnnounceExpressionEvaluated(Element element)
    {
        foreach (var listener in listeners)
            listener.ExpressionEvaluated(
                new ExpressionEvaluatedEvent(element));
    }

    private void AnnounceMissingRow(Element element)
    {
        foreach (var listener in listeners)
            listener.MissingRow(new MissingRowEvent(element));
    }

    private void AnnounceSurplusRow(Element element)
    {
        foreach (var listener in listeners)
            listener.SurplusRow(new SurplusRowEvent(element));
    }

    public void Setup(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
    }

    public void Execute(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
    }

    public void Verify(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
        var pattern = new Regex("(#.+?) *: *(.+)");
        var matcher = pattern.Match(commandCall.Expression);

        if (!matcher.Success)
            throw new InvalidOperationException(
                "The expression for a \"verifyRows\" should be of the form: #var : collectionExpr");

        var loopVariableName = matcher.Groups[1].Value;
        var iterableExpression = matcher.Groups[2].Value;
        var obj = evaluator.Evaluate(iterableExpression);

        Check.NotNull(obj, "Expression returned null (should be an IEnumerable).");
        Check.IsTrue(obj is IEnumerable, obj.GetType() + " is not IEnumerable");
        Check.IsTrue(!(obj is IDictionary), obj.GetType() + " does not have a predictable iteration order");

        var iterable = (IEnumerable)obj;
        var tableSupport = new TableSupport(commandCall);
        var detailRows = tableSupport.GetDetailRows();

        AnnounceExpressionEvaluated(commandCall.Element);

        var index = 0;

        foreach (var loopVar in iterable) {
            evaluator.SetVariable(loopVariableName, loopVar);

            Row detailRow;

            if (detailRows.Count > index)
                detailRow = detailRows[index];
            else {
                detailRow = tableSupport.AddDetailRow();
                AnnounceSurplusRow(detailRow.RowElement);
            }

            tableSupport.CopyCommandCallsTo(detailRow);
            commandCall.Children.Verify(evaluator, resultRecorder);
            index++;
        }

        for (; index < detailRows.Count; index++) {
            var detailRow = detailRows[index];

            resultRecorder.Failure($"missing row {detailRow}",
                commandCall.Element.ToXml());
            AnnounceMissingRow(detailRow.RowElement);
        }
    }
}
