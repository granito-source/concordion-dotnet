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

public class DocumentParser
{
    private readonly List<IDocumentParsingListener> m_Listeners = new List<IDocumentParsingListener>();

    #region Properties

    private ICommandFactory CommandFactory
    {
        get;
        set;
    }

    #endregion

    #region Constructors

    public DocumentParser(ICommandFactory commandFactory)
    {
        CommandFactory = commandFactory;
    }

    #endregion

    #region Methods

    public void AddDocumentParsingListener(IDocumentParsingListener listener)
    {
        m_Listeners.Add(listener);
    }

    public void RemoveDocumentParsingListener(IDocumentParsingListener listener)
    {
        m_Listeners.Remove(listener);
    }

    private void AnnounceBeforeParsing(XDocument document)
    {
        foreach (var listener in m_Listeners)
        {
            listener.BeforeParsing(document);
        }
    }

    public ISpecification Parse(XDocument document, Resource resource)
    {
        AnnounceBeforeParsing(document);
        var rootElement = document.Root;
        var rootCommandCall = new CommandCall(CreateSpecificationCommand(), new Element(rootElement), "", resource);
        GenerateCommandCallTree(rootElement, rootCommandCall, resource);
        return new XmlSpecification(rootCommandCall);
    }

    private ICommand CreateSpecificationCommand()
    {
        var specCmd = CreateCommand("", "specification");
        return specCmd;
    }

    private ICommand CreateCommand(string namespaceURI, string commandName)
    {
        return CommandFactory.CreateCommand(namespaceURI, commandName);
    }

    private void GenerateCommandCallTree(XElement element, CommandCall parentCommandCall, Resource resource)
    {
        var isCommandAssigned = false;

        foreach (var attribute in element.Attributes())
        {
            var namespaceURI = attribute.Name.Namespace.NamespaceName;

            if (!attribute.IsNamespaceDeclaration && !string.IsNullOrEmpty(namespaceURI))
            {
                var commandName = attribute.Name.LocalName;
                var command = CreateCommand(namespaceURI, commandName);
                if (command != null)
                {
                    Check.IsFalse(isCommandAssigned, "Multiple commands per element is currently not supported.");
                    isCommandAssigned = true;
                    var expression = attribute.Value;
                    var commandCall = new CommandCall(command, new Element(element), expression, resource);
                    parentCommandCall.AddChild(commandCall);
                    parentCommandCall = commandCall;
                }
            }
        }

        foreach (var child in element.Elements())
        {
            GenerateCommandCallTree(child, parentCommandCall, resource);
        }
    }

    #endregion
}