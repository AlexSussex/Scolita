namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// Follows a reference from one content item to another (a word's picture, a step's letters),
    /// reporting a reference to an id that doesn't exist or is retired. Used by every rule.
    /// </summary>
    public static class References
    {
        /// <summary>
        /// The item <paramref name="id"/> of <paramref name="catalog"/>, or null (with an error at
        /// <paramref name="source"/>) when there is none, or when it is retired: a retired item is
        /// kept for its id, never used. <paramref name="what"/> names it in the message
        /// (<c>letter</c>), and <paramref name="file"/> names its CSV. Called by the rules.
        /// </summary>
        public static TItem InUse<TItem>(ItemCatalog<TItem> catalog, ContentId id, ContentSource source, string what,
            string file, ContentProblems problems) where TItem : class, IContentItem
        {
            if (!catalog.TryGet(id, out TItem item))
            {
                problems.Error(source, "the " + what + " " + id + " is not in " + file + ".");
                return null;
            }

            if (item.Retired)
            {
                problems.Error(source, "the " + what + " " + id + " is retired (" + item.Source
                    + "): use another, or bring it back by emptying its retired cell.");
                return null;
            }

            return item;
        }
    }
}
