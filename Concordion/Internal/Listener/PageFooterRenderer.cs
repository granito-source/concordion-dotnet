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

namespace Concordion.Internal.Listener;

public class PageFooterRenderer(Target target) : SpecificationProcessingListener {
    private const string ConcordionWebsiteUrl = "https://concordion.org/";

    private static readonly Resource TargetLogoResource =
        new("image/concordion-logo.png");

    public void BeforeProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
    }

    public void AfterProcessingSpecification(
        SpecificationProcessingEvent processingEvent)
    {
        try {
            CopyLogoToTarget();
            AddFooterToDocument(processingEvent.RootElement,
                processingEvent.Resource);
        } catch (Exception e) {
            Console.WriteLine(@"Failed to write page footer. {0}", e.Message);
        }
    }

    private void AddFooterToDocument(Element rootElement, Resource resource)
    {
        var body = rootElement.GetFirstChildElement("body");

        if (body == null)
            return;

        var footer = new Element("div");

        footer.AddStyleClass("footer");
        footer.AppendText("Powered by ");

        var link = new Element("a");

        link.AddAttribute("href", ConcordionWebsiteUrl);
        footer.AppendChild(link);

        var img = new Element("img");

        img.AddAttribute("src", resource.GetRelativePath(TargetLogoResource));
        img.AddAttribute("alt", "Concordion");
        img.AddAttribute("border", "0");
        link.AppendChild(img);
        body.AppendChild(footer);
    }

    private void CopyLogoToTarget()
    {
        // XXX: disable for now, will review
        // Target.Write(TargetLogoResource, HtmlFramework.SOURCE_LOGO_RESOURCE_PATH);
    }
}
