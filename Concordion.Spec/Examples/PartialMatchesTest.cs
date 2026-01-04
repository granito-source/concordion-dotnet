using Concordion.Integration;

namespace Concordion.Spec.Examples;

[ConcordionTest]
public class PartialMatchesTest {
    private List<string> usernamesInSystem = [];

    public void setUpUser(string username)
    {
        usernamesInSystem.Add(username);
    }

    public List<string> getSearchResultsFor(string searchString)
    {
        var matchSet = new List<string>();
        var matches = from username in usernamesInSystem
            where username.Contains(searchString)
            select username;

        foreach (var match in matches)
            matchSet.Add(match);

        return matchSet;
    }
}
