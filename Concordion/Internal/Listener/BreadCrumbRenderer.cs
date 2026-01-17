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
        try {
            var span = new Element("span").AddStyleClass("breadcrumbs");

            AppendBreadcrumbsTo(span, processingEvent.Resource);

            if (span.HasChildren)
                GetDocumentBody(processingEvent.RootElement)
                    .PrependChild(span);
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
    }

    private void AppendBreadcrumbsTo(Element breadcrumbSpan,
        Resource documentResource)
    {
        var packageResource = documentResource.Parent;

        while (packageResource != null) {
            var indexPageResource = packageResource
                .GetRelativeResource(GetIndexPageName(packageResource));

            if (!indexPageResource.Equals(documentResource) &&
                source.CanFind(indexPageResource))
                try {
                    PrependBreadcrumb(breadcrumbSpan,
                        CreateBreadcrumbElement(documentResource,
                            indexPageResource));
                } catch (Exception e) {
                    throw new Exception("Trouble appending a breadcrumb", e);
                }

            packageResource = packageResource.Parent;
        }
    }

    private Element CreateBreadcrumbElement(Resource documentResource,
        Resource indexPageResource)
    {
        using var inputStream = source.CreateStream(indexPageResource);
        var root = XDocument.Load(inputStream).Root;

        Check.NotNull(root, "root may not be null");

        var breadcrumbWording = GetBreadcrumbWording(new Element(root),
            indexPageResource);
        var a = new Element("a");

        a.AddAttribute("href",
            documentResource.GetRelativePath(indexPageResource));
        a.AppendText(breadcrumbWording);

        return a;
    }
}
