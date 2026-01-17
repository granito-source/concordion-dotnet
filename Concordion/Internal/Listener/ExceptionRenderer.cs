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
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public class ExceptionRenderer : ExceptionCaughtListener {
    private long buttonId;

    private readonly HashSet<Element> rootElementsWithScript = [];

    private static Element ExpectedSpan(Element element)
    {
        var expected = new Element("del").AddStyleClass("expected");

        element.MoveChildrenTo(expected);
        expected.AppendNonBreakingSpaceIfBlank();

        var failure = new Element("span").AddStyleClass("failure");

        failure.AppendChild(expected);

        return failure;
    }

    private static Element ExceptionMessage(string exceptionMessage)
    {
        return new Element("span")
            .AddStyleClass("exceptionMessage")
            .AppendText(exceptionMessage);
    }

    private static Element StackTraceElement(string stackTraceText)
    {
        return new Element("span")
            .AddStyleClass("stackTraceEntry")
            .AppendText(stackTraceText);
    }

    public void ExceptionCaught(ExceptionCaughtEvent caughtEvent)
    {
        buttonId++;

        var element = caughtEvent.Element;

        element.AppendChild(ExpectedSpan(element));

        // Special handling for <a> tags to avoid the stack-trace being
        // inside the link text
        if (element.IsNamed("a")) {
            var div = new Element("div");

            element.AppendSister(div);
            element = div;
        }

        var expression = caughtEvent.Expression ?? "(null)";

        element.AppendChild(
            ExceptionMessage(caughtEvent.CaughtException.Message));
        element.AppendChild(StackTraceTogglingButton());
        element.AppendChild(
            StackTrace(caughtEvent.CaughtException, expression));
        EnsureDocumentHasTogglingScript(element);
    }

    private Element StackTraceTogglingButton()
    {
        return new Element("input")
            .AddStyleClass("stackTraceButton")
            .SetId("stackTraceButton" + buttonId)
            .AddAttribute("type", "button")
            .AddAttribute("onclick",
                $"javascript:toggleStackTrace('{buttonId}')")
            .AddAttribute("value", "View Stack");
    }

    private Element StackTrace(Exception exception, string expression)
    {
        var span = new Element("span").AddStyleClass("stackTrace");

        span.SetId("stackTrace" + buttonId);

        var p = new Element("p")
            .AppendText("While evaluating expression: ");

        p.AppendChild(new Element("code").AppendText(expression));
        span.AppendChild(p);

        var stackTrace = exception.StackTrace ?? "";
        var stackTraceItems = new List<string> {
            $"{exception.GetType()}: {exception.Message}"
        };

        stackTraceItems.AddRange(stackTrace.Split(['\r', '\n'],
            StringSplitOptions.RemoveEmptyEntries));

        foreach (var item in stackTraceItems)
            span.AppendChild(StackTraceElement(item));

        return span;
    }

    private void EnsureDocumentHasTogglingScript(Element element)
    {
        var root = element.GetRootElement();

        if (!rootElementsWithScript.Add(root))
            return;

        var head = root.GetFirstDescendantNamed("head");

        if (head == null)
            Console.WriteLine(root.ToXml());

        Check.NotNull(head, "Document <head> section is missing");

        var script = new Element("script")
            .AddAttribute("type", "text/javascript");

        head.PrependChild(script);
        script.AppendText(HtmlFramework.TOGGLING_SCRIPT_RESOURCE);
    }
}
