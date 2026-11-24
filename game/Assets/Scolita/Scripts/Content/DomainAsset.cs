using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A domain (<c>reading</c>) as an asset (<c>Content/Domains/&lt;id&gt;.asset</c>) with its units in
    /// teaching order, written by the content importer and never edited by hand.
    /// </summary>
    public sealed class DomainAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private List<UnitAsset> units = new List<UnitAsset>();

        /// <summary>The domain's id.</summary>
        public string Id => id;

        /// <summary>Its units, in teaching order.</summary>
        public IReadOnlyList<UnitAsset> Units => units;

        /// <summary>Writes the domain <paramref name="domainId"/> and its <paramref name="domainUnits"/>; called by the content importer.</summary>
        public void Fill(string domainId, IEnumerable<UnitAsset> domainUnits)
        {
            id = domainId;
            units = domainUnits.ToList();
        }

        /// <summary>
        /// Adds the domain's units and lessons to <paramref name="builder"/>; throws an
        /// <see cref="System.InvalidOperationException"/> if an asset can't be read. Called by
        /// <see cref="CurriculumAssets.Build"/>.
        /// </summary>
        public void AddTo(CurriculumBuilder builder)
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            fields.ThrowIfAny();
            foreach (UnitAsset unit in units)
            {
                unit.AddTo(builder);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            return fields.Problems;
        }

        private void Read(AssetFields fields)
        {
            fields.OwnId(id);
            if (units == null || units.Any(unit => unit == null))
            {
                fields.Problem("A unit of it is missing.");
            }
        }
    }
}
