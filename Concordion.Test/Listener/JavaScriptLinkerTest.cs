/*
 * Copyright 2026 Alexei Yashkov
 * Copyright 2010-2015 concordion.org
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

using System.Xml.Linq;
using Concordion.Api;
using Concordion.Internal.Listener;
using Concordion.Test.Support;

namespace Concordion.Test.Listener;

[TestFixture]
public class JavaScriptLinkerTest {
    private static readonly Resource? NotNeededParameter = null;

    [Test]
    public void XmlOutputContainsAnExplicitEndTagForScriptElement()
    {
        var javaScriptLinker = new JavaScriptLinker(NotNeededParameter);
        var html = new XElement("html");
        var head = new XElement("head");

        html.Add(head);

        javaScriptLinker.BeforeParsing(new XDocument(html));

        Assert.That(
            new HtmlUtil().RemoveWhitespaceBetweenTags(head.ToString()),
            Is.EqualTo("<head><script type=\"text/javascript\"></script></head>"));
    }
}
