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
    private readonly HashSet<Element> rootElementsWithScript = [];

    private int buttonId;

    public void ExceptionCaught(ExceptionCaughtEvent exceptionEvent)
    {
        var element = exceptionEvent.Element;

        buttonId++;
        element.AppendChild(ExpectedSpan(element));

        // Special handling for <a> tags to avoid the stack-trace being
        // inside the link text
        if (element.IsNamed("a")) {
            var div = new Element("div");

            element.AppendSister(div);
            element = div;
        }

        element.AppendChild(ExceptionMessage(
            exceptionEvent.CaughtException.Message));
        element.AppendChild(StackTraceTogglingButton());
        element.AppendChild(StackTrace(exceptionEvent.CaughtException,
            exceptionEvent.Expression));

        EnsureDocumentHasTogglingScript(element);
    }

    private void EnsureDocumentHasTogglingScript(Element element)
    {
        var rootElement = element.GetRootElement();

        if (!rootElementsWithScript.Add(rootElement))
            return;

        var head = rootElement.GetFirstDescendantNamed("head");

        if (head == null)
            Console.WriteLine(rootElement.ToXml());

        Check.NotNull(head, "Document <head> section is missing");

        var script = new Element("script")
            .AddAttribute("type", "text/javascript");

        head.PrependChild(script);
        script.AppendText(ConcordionBuilder.AssemblySource
            .ReadResourceAsString(ConcordionBuilder.TogglingScript));
    }

    private Element ExpectedSpan(Element element)
    {
        var spanExpected = new Element("del").AddStyleClass("expected");

        element.MoveChildrenTo(spanExpected);
        spanExpected.AppendNonBreakingSpaceIfBlank();

        var spanFailure = new Element("span").AddStyleClass("failure");

        spanFailure.AppendChild(spanExpected);

        return spanFailure;
    }

    private Element ExceptionMessage(string exceptionMessage)
    {
        return new Element("span")
            .AddStyleClass("exceptionMessage")
            .AppendText(exceptionMessage);
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

    private Element StackTrace(Exception exception, string? expression)
    {
        var stackTrace = new Element("div").AddStyleClass("stackTrace");

        stackTrace.SetId("stackTrace" + buttonId);

        var p = new Element("p").AppendText("While evaluating expression: ");

        p.AppendChild(new Element("code").AppendText(expression ?? "(null)"));
        stackTrace.AppendChild(p);

        RecursivelyAppendStackTrace(exception, stackTrace);

        return stackTrace;
    }

    private void RecursivelyAppendStackTrace(Exception exception,
        Element stackTrace)
    {
        var stackTraceExceptionMessage = new Element("div")
            .AddStyleClass("stackTraceExceptionMessage")
            .AppendText(exception.GetType().Name + ": " + exception.Message);

        stackTrace.AppendChild(stackTraceExceptionMessage);

        var stackTraceText = exception.StackTrace ?? "";
        var stackTraceItems = stackTraceText.Split(['\r', '\n'],
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var item in stackTraceItems)
            stackTrace.AppendChild(StackTraceElement(item));

        if (exception.InnerException != null)
            RecursivelyAppendStackTrace(exception.InnerException, stackTrace);
    }

    private Element StackTraceElement(string stackTraceText)
    {
        return new Element("span")
            .AddStyleClass("stackTraceEntry")
            .AppendText(stackTraceText);
    }
}
