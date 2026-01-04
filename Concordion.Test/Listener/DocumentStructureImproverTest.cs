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
using Concordion.Internal.Listener;
using Concordion.Test.Support;

namespace Concordion.Test.Listener;

[TestFixture]
public class DocumentStructureImproverTest {
    private DocumentStructureImprover improver;

    private XElement html;

    private XDocument document;

    [SetUp]
    public void Init()
    {
        improver = new DocumentStructureImprover();
        html = new XElement("html");
        document = new XDocument(html);
    }

    [Test]
    public void AddsHeadIfMissing()
    {
        improver.BeforeParsing(document);

        Assert.That(
            new HtmlUtil().RemoveWhitespaceBetweenTags(html.ToString()),
            Is.EqualTo("<html><head /></html>"));

        // Check it does not add it again if we repeat the call
        improver.BeforeParsing(document);

        Assert.That(
            new HtmlUtil().RemoveWhitespaceBetweenTags(html.ToString()),
            Is.EqualTo("<html><head /></html>"));
    }

    [Test]
    public void TransfersEverythingBeforeBodyIntoNewlyCreatedHead()
    {
        var style1 = new XElement("style1");
        var style2 = new XElement("style2");

        html.Add(style1);
        html.Add(style2);

        var body = new XElement("body");

        body.SetValue("some ");

        var bold = new XElement("b");

        bold.SetValue("bold text");
        body.Add(bold);
        html.Add(body);
        improver.BeforeParsing(document);

        Assert.That(
            new HtmlUtil().RemoveWhitespaceBetweenTags(html.ToString()),
            Is.EqualTo("<html><head><style1 /><style2 /></head><body>some <b>bold text</b></body></html>"));
    }
}
