namespace HierarchySearch
{
    static class StringMatchHelper
    {
        public static bool DoesNameMatch(string target, string searchTerm, bool caseSensitive, bool matchWholeWord)
        {
            target = caseSensitive ? target : target.ToLowerInvariant();
            searchTerm = caseSensitive ? searchTerm : searchTerm.ToLowerInvariant();
            return matchWholeWord ? target == searchTerm : target.Contains(searchTerm);
        }
    }
}
