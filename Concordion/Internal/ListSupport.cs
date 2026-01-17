using Concordion.Api;

namespace Concordion.Internal;

public class ListSupport {
    private readonly CommandCall listCommandCall;

    public ListSupport(CommandCall listCommandCall)
    {
        if (!(listCommandCall.Element.IsNamed("ol") ||
                listCommandCall.Element.IsNamed("ul")))
            throw new ArgumentException(
                @"This strategy can only work on list elements",
                nameof(listCommandCall));

        this.listCommandCall = listCommandCall;
    }

    public IList<Element> GetListItemElements()
    {
        return listCommandCall
            .Element
            .GetDescendantElements("li")
            .ToList();
    }

    public IList<Element> GetListElements()
    {
        return listCommandCall
            .Element
            .GetDescendantElements("ul")
            .Concat(listCommandCall.Element.GetDescendantElements("ol"))
            .ToList();
    }

    public IList<ListEntry> GetListEntries()
    {
        return (
            from listElement in listCommandCall.Element.GetChildElements()
            where listElement.IsNamed("li") || listElement.IsNamed("ul") ||
                listElement.IsNamed("ol")
            select new ListEntry(listElement)
        ).ToList();
    }
}
