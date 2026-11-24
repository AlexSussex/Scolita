using System;
using System.Collections.Generic;

namespace Scolita.Core.Content
{
    /// <summary>
    /// The items of one kind (letters, words, pictures or voice lines), in the order read, found by
    /// their stable id. Made by <see cref="CurriculumBuilder"/>; an id appears once.
    /// </summary>
    public sealed class ItemCatalog<TItem> where TItem : class, IContentItem
    {
        private readonly IReadOnlyList<TItem> items;
        private readonly Dictionary<ContentId, TItem> byId;

        /// <summary>
        /// Creates a catalog of <paramref name="items"/>; throws an <see cref="ArgumentException"/>
        /// on a repeated id (the builder reports and drops those first). Called by
        /// <see cref="CurriculumBuilder"/>.
        /// </summary>
        public ItemCatalog(IReadOnlyList<TItem> items)
        {
            this.items = items;
            byId = new Dictionary<ContentId, TItem>(items.Count);
            foreach (TItem item in items)
            {
                byId.Add(item.Id, item);
            }
        }

        /// <summary>Every item, in the order read.</summary>
        public IReadOnlyList<TItem> All => items;

        /// <summary>Finds the item <paramref name="id"/>; false when there is none. Called by the rules and the activities.</summary>
        public bool TryGet(ContentId id, out TItem item)
        {
            return byId.TryGetValue(id, out item);
        }

        /// <summary>
        /// The item <paramref name="id"/>; throws a <see cref="KeyNotFoundException"/> when there is
        /// none (validated content always has it). Called by the activities.
        /// </summary>
        public TItem Get(ContentId id)
        {
            if (!byId.TryGetValue(id, out TItem item))
            {
                throw new KeyNotFoundException("No " + typeof(TItem).Name + " with the id '" + id + "'.");
            }

            return item;
        }

        /// <summary>Whether there is an item <paramref name="id"/>; called by the rules.</summary>
        public bool Contains(ContentId id)
        {
            return byId.ContainsKey(id);
        }
    }
}
