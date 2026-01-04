using Concordion.Api;

namespace Concordion.Internal;

public class ListEntry
{
    #region Properties

    public Element Element
    {
        get;
        private set;
    }

    public bool IsItem 
    {
        get
        {
            return Element.IsNamed("li");
        }
    }

    public bool IsList
    {
        get
        {
            return (Element.IsNamed("ul") || Element.IsNamed("ol"));
        }
    }

    #endregion

    #region Constructors

    public ListEntry(Element element)
    {
        Element = element;
    }

    #endregion
}