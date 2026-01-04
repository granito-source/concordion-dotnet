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
public class MetadataCreatorTest {
    private MetadataCreator metadataCreator;

    private XElement html;

    private XDocument document;

    private XElement head;

    [SetUp]
    public void Init()
    {
        html = new XElement("html");
        head = new XElement("head");
        html.Add(head);
        document = new XDocument(html);
        metadataCreator = new MetadataCreator();
    }

    [Test]
    public void AddsContentTypeMetadataIfMissing()
    {
        metadataCreator.BeforeParsing(document);

        Assert.That(
            new HtmlUtil().RemoveWhitespaceBetweenTags(html.ToString()),
            Is.EqualTo("<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" /></head></html>"));
    }

    [Test]
    public void DoesNotAddContentTypeMetadataIfAlreadyPresent()
    {
        var meta = new XElement("meta");

        meta.SetAttributeValue("http-equiv", "Content-Type");
        meta.SetAttributeValue("content", "text/html; charset=UTF-8");
        head.Add(new XElement(meta));

        Assert.That(
            new HtmlUtil().RemoveWhitespaceBetweenTags(html.ToString()),
            Is.EqualTo("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head></html>"));

        metadataCreator.BeforeParsing(document);

        Assert.That(
            new HtmlUtil().RemoveWhitespaceBetweenTags(html.ToString()),
            Is.EqualTo("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head></html>"));
    }
}
