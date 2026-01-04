using Concordion.Api;

namespace Concordion.Internal;

public class ListSupport
{
    #region Properties

    private CommandCall ListCommandCall
    {
        get;
        set;
    }

    #endregion

    #region Constructors

    public ListSupport(CommandCall listCommandCall)
    {
        if (!(listCommandCall.Element.IsNamed("ol") || listCommandCall.Element.IsNamed("ul")))
        {
            throw new ArgumentException("This strategy can only work on list elements", "listCommandCall");
        }

        ListCommandCall = listCommandCall;
    }

    #endregion

    #region Methods

    public IList<Element> GetListItemElements()
    {
        var listItemElements = new List<Element>();
        foreach (var itemElement in ListCommandCall.Element.GetDescendantElements("li"))
        {
            listItemElements.Add(itemElement);
        }
        return listItemElements;
    }

    public List<Element> GetListElements()
    {
        var listElements = new List<Element>();
        foreach (var listElement in ListCommandCall.Element.GetDescendantElements("ul"))
        {
            listElements.Add(listElement);
        }
        foreach (var listElement in ListCommandCall.Element.GetDescendantElements("ol"))
        {
            listElements.Add(listElement);
        }
        return listElements;
    }

    public List<ListEntry> GetListEntries()
    {
        var listEntries = new List<ListEntry>();
        foreach (var listElement in ListCommandCall.Element.GetChildElements())
        {
            if (listElement.IsNamed("li") ||
                listElement.IsNamed("ul") ||
                listElement.IsNamed("ol"))
            {
                listEntries.Add(new ListEntry(listElement));
            }
        }
        return listEntries;
    }

    #endregion
}