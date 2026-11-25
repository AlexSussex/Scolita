using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="Core.Rewards.Constellation"/> as an asset (<c>Content/Rewards/Constellations/&lt;id&gt;.asset</c>): its
    /// planet, its stars in its frame and its lines; written by the content importer and never edited by hand.
    /// </summary>
    public sealed class ConstellationAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string unit;
        [SerializeField] private Vector2[] stars;
        [SerializeField] private Vector2Int[] lines;
        [SerializeField] private bool retired;

        /// <summary>The constellation's id.</summary>
        public string Id => id;

        /// <summary>Writes <paramref name="constellation"/> into this asset; called by the content importer.</summary>
        public void Fill(Constellation constellation)
        {
            id = constellation.Id.Value;
            unit = constellation.UnitId.Value;
            stars = constellation.Stars.Select(star => new Vector2(star.X, star.Y)).ToArray();
            lines = constellation.Lines.Select(line => new Vector2Int(line.From, line.To)).ToArray();
            retired = constellation.Retired;
        }

        /// <summary>
        /// The constellation for Core; throws an <see cref="System.InvalidOperationException"/> if the asset can't be
        /// read. Called by <see cref="RewardContentAssets.Build"/>.
        /// </summary>
        public Constellation ToConstellation()
        {
            AssetFields fields = new AssetFields(name);
            Constellation constellation = Read(fields);
            fields.ThrowIfAny();
            return constellation;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            return fields.Problems;
        }

        private Constellation Read(AssetFields fields)
        {
            ContentId ownId = fields.OwnId(id);
            ContentId unitId = fields.Id(unit, "unit");
            Vector2[] readStars = stars ?? new Vector2[0];
            Vector2Int[] readLines = lines ?? new Vector2Int[0];
            if (readStars.Length < RewardContentRules.FewestStars)
            {
                fields.Problem("It has " + readStars.Length + " stars; a figure needs at least " + RewardContentRules.FewestStars + ".");
            }

            if (readLines.Any(line => line.x < 0 || line.y < 0 || line.x >= readStars.Length || line.y >= readStars.Length))
            {
                fields.Problem("A line joins a star it doesn't have.");
            }

            return new Constellation(ownId, unitId,
                readStars.Select(star => new StarPoint(star.x, star.y)).ToArray(),
                readLines.Select(line => new StarLine(line.x, line.y)).ToArray(),
                retired, fields.Source);
        }
    }
}
