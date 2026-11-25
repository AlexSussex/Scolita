using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="Core.Rewards.Trail"/> as an asset (<c>Content/Rewards/Trails/&lt;id&gt;.asset</c>) with its picture,
    /// <c>Art/Map/trail-&lt;id&gt;</c>; written by the content importer and never edited by hand.
    /// </summary>
    public sealed class TrailAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private bool retired;
        [SerializeField] private Sprite sprite;

        /// <summary>The trail's id.</summary>
        public string Id => id;

        /// <summary>Its picture, a ribbon repeated behind the rocket.</summary>
        public Sprite Sprite => sprite;

        /// <summary>Writes <paramref name="trail"/> and its <paramref name="picture"/> into this asset; called by the content importer.</summary>
        public void Fill(Trail trail, Sprite picture)
        {
            id = trail.Id.Value;
            retired = trail.Retired;
            sprite = picture;
        }

        /// <summary>
        /// The trail for Core; throws an <see cref="System.InvalidOperationException"/> if the asset can't be read.
        /// Called by <see cref="RewardContentAssets.Build"/>.
        /// </summary>
        public Trail ToTrail()
        {
            AssetFields fields = new AssetFields(name);
            Trail trail = Read(fields);
            fields.ThrowIfAny();
            return trail;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            if (sprite == null)
            {
                fields.Problem("It has no picture (run Școlița → Import content).");
            }

            return fields.Problems;
        }

        private Trail Read(AssetFields fields)
        {
            return new Trail(fields.OwnId(id), retired, fields.Source);
        }
    }
}
