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
using Concordion.Api;
using Concordion.Api.Listener;
using Concordion.Internal.Util;

namespace Concordion.Internal;

public class DocumentParser(CommandFactory commandFactory) {
    private readonly List<DocumentParsingListener> listeners = [];

    public void AddDocumentParsingListener(DocumentParsingListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveDocumentParsingListener(DocumentParsingListener listener)
    {
        listeners.Remove(listener);
    }

    private void AnnounceBeforeParsing(XDocument document)
    {
        foreach (var listener in listeners)
            listener.BeforeParsing(document);
    }

    public Specification Parse(XDocument document, Resource resource)
    {
        AnnounceBeforeParsing(document);

        var rootElement = document.Root;

        Check.NotNull(rootElement, "root element may not be null");

        var rootCommandCall = new CommandCall(CreateSpecificationCommand(),
            new Element(rootElement), "", resource);

        GenerateCommandCallTree(rootElement, rootCommandCall, resource);

        return new XmlSpecification(rootCommandCall);
    }

    private Command CreateSpecificationCommand()
    {
        var specCmd = CreateCommand("", "specification");

        return specCmd;
    }

    private Command CreateCommand(string namespaceUri, string commandName)
    {
        return commandFactory.CreateCommand(namespaceUri, commandName);
    }

    private void GenerateCommandCallTree(XElement element,
        CommandCall parentCommandCall, Resource resource)
    {
        var isCommandAssigned = false;

        foreach (var attribute in element.Attributes()) {
            var namespaceUri = attribute.Name.Namespace.NamespaceName;

            if (attribute.IsNamespaceDeclaration ||
                string.IsNullOrEmpty(namespaceUri))
                continue;

            var commandName = attribute.Name.LocalName;
            var command = CreateCommand(namespaceUri, commandName);

            Check.IsFalse(isCommandAssigned,
                "Multiple commands per element is currently not supported.");

            isCommandAssigned = true;

            var expression = attribute.Value;
            var commandCall = new CommandCall(command,
                new Element(element), expression, resource);

            parentCommandCall.AddChild(commandCall);
            parentCommandCall = commandCall;
        }

        foreach (var child in element.Elements())
            GenerateCommandCallTree(child, parentCommandCall, resource);
    }
}
