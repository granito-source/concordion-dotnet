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

using System.Text.RegularExpressions;
using System.Xml.Linq;
using Concordion.Api;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal.Listener;

public partial class BreadCrumbRenderer(Source source) :
    SpecificationProcessingListener {
    [GeneratedRegex("([0-9a-z])([A-Z])")]
    private static partial Regex DeCamelRegex();

    [GeneratedRegex("\\.[a-z]+")]
    private static partial Regex ExtensionRegex();

    private static Element GetDocumentBody(Element rootElement)
    {
        var body = rootElement.GetFirstDescendantNamed("body");

        if (body != null)
            return body;

        body = new Element("body");
        rootElement.AppendChild(body);

        return body;
    }

    private static void PrependBreadcrumb(Element span, Element breadcrumb)
    {
        if (span.HasChildren)
            span.PrependText(" ");

        span.PrependText(" >");
        span.PrependChild(breadcrumb);
    }

    private static string GetIndexPageName(Resource resource)
    {
        return Capitalize(resource.Name) + ".html";
    }

    private static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;

        return s[..1].ToUpper() + s[1..];
    }

    private static string GetBreadcrumbWording(Element rootElement,
        Resource resource)
    {
        var title = rootElement.GetFirstDescendantNamed("title");

        if (title != null && !string.IsNullOrEmpty(title.Text))
            return title.Text;

        var headings = rootElement.GetDescendantElements("h1");

        foreach (var h1 in headings)
            if (!string.IsNullOrEmpty(h1.Text))
                return h1.Text;

        var heading = resource.Name;

        if (string.IsNullOrEmpty(heading))
            return "(Up)";

        heading = StripExtension(heading);
        heading = Capitalize(heading);
        heading = DeCamelCase(heading);

        return heading;
    }

    private static string StripExtension(string s)
    {
        return ExtensionRegex().Replace(s, string.Empty);
    }

    private static string DeCamelCase(string s)
    {
        return DeCamelRegex().Replace(s, "$1 $2");
    }

    public void BeforeProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
        // No action needed beforehand
    }

    public void AfterProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
        var breadcrumbs = new Element("span").AddStyleClass("breadcrumbs");

        AppendBreadcrumbsTo(breadcrumbs, processingEvent.Resource);

        if (breadcrumbs.HasChildren)
            GetDocumentBody(processingEvent.RootElement).PrependChild(breadcrumbs);
    }

    private void AppendBreadcrumbsTo(Element breadcrumbs, Resource document)
    {
        var parent = document.Parent;

        while (parent != null) {
            var indexPage = parent
                .GetRelativeResource(GetIndexPageName(parent));

            if (!indexPage.Equals(document) && source.CanFind(indexPage))
                PrependBreadcrumb(breadcrumbs,
                    CreateBreadcrumbElement(document, indexPage));

            parent = parent.Parent;
        }
    }

    private Element CreateBreadcrumbElement(Resource document,
        Resource indexPage)
    {
        using var inputStream = source.CreateStream(indexPage);
        var root = XDocument.Load(inputStream).Root;

        Check.NotNull(root, "root may not be null");

        var breadcrumbWording = GetBreadcrumbWording(new Element(root),
            indexPage);
        var a = new Element("a");

        a.AddAttribute("href",
            document.GetRelativePath(indexPage));
        a.AppendText(breadcrumbWording);

        return a;
    }
}
