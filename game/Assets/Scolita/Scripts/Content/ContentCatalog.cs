using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A catalog of content assets of one kind, in the order of their CSV (<see cref="LetterCatalog"/>,
    /// <see cref="WordCatalog"/>, <see cref="PictureCatalog"/>, <see cref="VoiceLineCatalog"/>,
    /// <see cref="DomainCatalog"/>), written by the content importer and never edited by hand.
    /// </summary>
    public abstract class ContentCatalog<TAsset> : ScriptableObject, IValidatableContent where TAsset : ScriptableObject
    {
        [SerializeField] private List<TAsset> items = new List<TAsset>();

        /// <summary>Its assets, in order.</summary>
        public IReadOnlyList<TAsset> Items => items;

        /// <summary>Writes <paramref name="assets"/> into the catalog, in order; called by the content importer.</summary>
        public void Fill(IEnumerable<TAsset> assets)
        {
            items = assets.ToList();
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            List<string> problems = new List<string>();
            if (items == null || items.Any(item => item == null))
            {
                problems.Add("An item of it is missing.");
                return problems;
            }

            foreach (IGrouping<string, TAsset> repeated in items.GroupBy(IdOf).Where(group => group.Count() > 1))
            {
                problems.Add("The id " + repeated.Key + " is in it " + repeated.Count() + " times.");
            }

            return problems;
        }

        /// <summary>The id of <paramref name="asset"/>; called by <see cref="Validate"/>.</summary>
        protected abstract string IdOf(TAsset asset);
    }
}
