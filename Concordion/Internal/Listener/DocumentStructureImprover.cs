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

using System.Xml.Linq;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public class DocumentStructureImprover : DocumentParsingListener
{
    private static bool HasHeadSection(XElement html)
    {
        return html.Element(XName.Get("head", "")) != null;
    }

    private static void CopyNodesBeforeBodyIntoHead(XElement html,
        XElement head)
    {
        foreach (var child in NodesBeforeBody(html))
        {
            child.Remove();
            head.Add(child);
        }
    }

    private static List<XElement> NodesBeforeBody(XElement html)
    {
        return html
            .Elements()
            .TakeWhile(child => !IsBodySection(child))
            .ToList();
    }

    private static bool IsBodySection(XElement child)
    {
        return child.Name.LocalName == "body";
    }

    public void BeforeParsing(XDocument document)
    {
        var html = document.Root;

        Check.NotNull(html, "document root may not be null");
        Check.IsTrue("html".Equals(html.Name.LocalName),
            $"Only <html> documents are supported (<{html.Name.LocalName}> is not)");

        if (HasHeadSection(html))
            return;

        var head = new XElement("head");

        CopyNodesBeforeBodyIntoHead(html, head);
        html.AddFirst(head);
    }
}
