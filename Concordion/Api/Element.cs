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

namespace Concordion.Api;

/// <summary>
/// A wrapper class for an XML element, usually from the specification or
/// the target of the specification.
/// </summary>
public class Element {
    private static readonly XNamespace XhtmlNs =
        "http://www.w3.org/1999/xhtml";

    private readonly XElement xElement;

    /// <summary>
    /// Gets the text value of the element
    /// </summary>
    public string Text => xElement.Value;

    /// <summary>
    /// Gets true if the element has children, false otherwise
    /// </summary>
    public bool HasChildren => xElement.HasElements;

    /// <summary>
    /// Gets true if the element's text is empty
    /// </summary>
    private bool IsBlank => string.IsNullOrEmpty(Text.Trim());

    /// <summary>
    /// Constructs a new object of the <see cref="Element"/> type
    /// </summary>
    /// <param name="name">The name of the new Element</param>
    public Element(string name) : this(new XElement(name))
    {
    }

    /// <summary>
    /// Constructs a new object of the Elem<see cref="Element"/>ent type
    /// </summary>
    /// <param name="element">The <see cref="System.Xml.Linq.XElement"/>
    /// to wrap</param>
    public Element(XElement element)
    {
        xElement = element;
    }

    /// <summary>
    /// Appends some text to the <see cref="Element"/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Element AppendText(string text)
    {
        xElement.Add(text);

        return this;
    }

    /// <summary>
    /// Appends a child <see cref="Element"/> after this one
    /// </summary>
    /// <param name="child"></param>
    public void AppendChild(Element child)
    {
        xElement.Add(child.xElement);
    }

    public void AppendSister(Element sister)
    {
        xElement.AddAfterSelf(sister.xElement);
    }

    /// <summary>
    /// Prepends a child <see cref="Element"/> before this one
    /// </summary>
    /// <param name="child"></param>
    /// <returns></returns>
    public Element PrependChild(Element child)
    {
        xElement.AddFirst(child.xElement);

        return this;
    }

    /// <summary>
    /// Determines if the <see cref="Element"/> has a name like the parameter
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsNamed(string name)
    {
        return xElement.Name.LocalName == name;
    }

    /// <summary>
    /// Gets all the descendant <see cref="Element"/> objects with a
    /// specific name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IList<Element> GetDescendantElements(string name)
    {
        return (
            from el in xElement.Descendants()
            where el.Name.LocalName == name && (el.Name.Namespace == "" ||
                el.Name.Namespace == XhtmlNs)
            select new Element(el)
        ).ToList();
    }

    public IEnumerable<Element> GetChildElements(string name)
    {
        return WrapXElements(xElement.Elements(XName.Get(name)));
    }

    /// <summary>
    /// Gets only the immediate child <see cref="Element"/> of the current one
    /// </summary>
    /// <returns>A list of child elements</returns>
    public IEnumerable<Element> GetChildElements()
    {
        return WrapXElements(xElement.Elements());
    }

    private static List<Element> WrapXElements(IEnumerable<XElement> elements)
    {
        return elements
            .Select(childElement => new Element(childElement))
            .ToList();
    }

    /// <summary>
    /// Gets the first child <see cref="Element"/> with the following name
    /// The document is searched in DOM document order
    /// </summary>
    /// <param name="elementName"></param>
    /// <returns>an <see cref="Element"/> object if found, null
    /// otherwise</returns>
    public Element? GetFirstChildElement(string elementName)
    {
        return (
            from descendant in xElement.Descendants()
            where descendant.Name.LocalName == elementName
            select new Element(descendant)
        ).FirstOrDefault();
    }

    /// <summary>
    /// Gets the value of an attribute of the <see cref="Element"/>.
    /// </summary>
    /// <param name="attributeName">The name of the attribute</param>
    /// <returns>A string with the text of the attribute value, null if
    /// the attribute does not exist</returns>
    public string? GetAttributeValue(string attributeName)
    {
        return xElement.Attribute(XName.Get(attributeName))?.Value;
    }

    /// <summary>
    /// Gets the value of an attribute of the <see cref="Element"/> in
    /// the specified namespace.
    /// </summary>
    /// <param name="attributeName">The name of the attribute</param>
    /// <param name="namespaceName">The name of the XML namespace</param>
    /// <returns>A string with the test of the attribute value, null if
    /// the attribute does not exist</returns>
    public string? GetAttributeValue(string attributeName, string namespaceName)
    {
        return xElement
            .Attribute(XName.Get(attributeName, namespaceName))?.Value;
    }

    /// <summary>
    /// Applies a CSS class to the following element.
    /// </summary>
    /// <param name="style">The name of the style to apply</param>
    /// <returns>This object with the style class applied</returns>
    public Element AddStyleClass(string style)
    {
        var currentClass = GetAttributeValue("class");
        var styleClass = style;

        if (currentClass != null)
            styleClass = currentClass + " " + styleClass;

        AddAttribute("class", styleClass);

        return this;
    }

    /// <summary>
    /// Adds an attribute to the element.
    /// </summary>
    /// <param name="localName">The name of the attribute</param>
    /// <param name="value">The value of the attribute</param>
    /// <returns>This object with the attribute added</returns>
    public Element AddAttribute(string localName, string value)
    {
        xElement.SetAttributeValue(XName.Get(localName, ""), value);

        return this;
    }

    /// <summary>
    /// Gets the first descendant that matches the name.
    /// </summary>
    /// <param name="name">The name to find</param>
    /// <returns>The <see cref="Element"/> if found, null
    /// otherwise</returns>
    public Element? GetFirstDescendantNamed(string name)
    {
        return GetDescendantElements(name).FirstOrDefault();
    }

    /// <summary>
    /// Moves all the children of this <see cref="Element"/> to another
    /// element.
    /// </summary>
    /// <param name="destinationElement">The destination element</param>
    public void MoveChildrenTo(Element destinationElement)
    {
        destinationElement.xElement.Add(GetChildNodes());
        xElement.RemoveNodes();
    }

    /// <summary>
    /// Gets all child <see cref="System.Xml.Linq.XNode"/>
    /// </summary>
    /// <returns></returns>
    private IEnumerable<XNode> GetChildNodes()
    {
        return xElement.Nodes();
    }

    /// <summary>
    /// If the <see cref="Element"/> has no text then a <![CDATA[&nbsp;]]>
    /// element is appended.
    /// </summary>
    /// <returns>This object with the NonBreakingSpace appended</returns>
    public Element AppendNonBreakingSpaceIfBlank()
    {
        if (IsBlank)
            AppendText("\u00A0");

        return this;
    }

    /// <summary>
    /// Sets the id of the current element
    /// </summary>
    /// <param name="id">The id to set</param>
    /// <returns>This object with the id set</returns>
    public Element SetId(string id)
    {
        AddAttribute("id", id);

        return this;
    }

    /// <summary>
    /// Gets the root element of the <see cref="System.Xml.Linq.XDocument"/>
    /// that this <see cref="Element"/> is contained within.
    /// </summary>
    /// <returns>The root <see cref="Element"/> object of the document</returns>
    public Element GetRootElement()
    {
        var root = xElement.Document?.Root;

        return root != null ? new Element(root) :
            throw new NullReferenceException("Root element is null");
    }

    /// <summary>
    /// Outputs the <see cref="Element"/> to a string as XML.
    /// </summary>
    /// <returns>A string of XML</returns>
    public string ToXml()
    {
        return xElement.ToString();
    }

    /// <summary>
    /// Adds some text to the first of the text of this
    /// <see cref="Element"/>.
    /// </summary>
    /// <param name="text"></param>
    public void PrependText(string text)
    {
        xElement.AddFirst(new XText(text));
    }

    /// <summary>
    /// Gets a hashcode of the object.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return xElement.GetHashCode();
    }

    /// <summary>
    /// Determines if another object equals this one
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (this == obj)
            return true;

        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (Element)obj;

        return xElement.Equals(other.xElement);
    }
}
