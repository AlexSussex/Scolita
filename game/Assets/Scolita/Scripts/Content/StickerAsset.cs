using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="Core.Rewards.Sticker"/> as an asset (<c>Content/Rewards/Stickers/&lt;id&gt;.asset</c>) with its picture,
    /// <c>Art/Stickers/sticker-&lt;id&gt;</c>; written by the content importer and never edited by hand.
    /// </summary>
    public sealed class StickerAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private bool retired;
        [SerializeField] private Sprite sprite;

        /// <summary>The sticker's id.</summary>
        public string Id => id;

        /// <summary>Its picture.</summary>
        public Sprite Sprite => sprite;

        /// <summary>Writes <paramref name="sticker"/> and its <paramref name="picture"/> into this asset; called by the content importer.</summary>
        public void Fill(Sticker sticker, Sprite picture)
        {
            id = sticker.Id.Value;
            retired = sticker.Retired;
            sprite = picture;
        }

        /// <summary>
        /// The sticker for Core; throws an <see cref="System.InvalidOperationException"/> if the asset can't be read.
        /// Called by <see cref="RewardContentAssets.Build"/>.
        /// </summary>
        public Sticker ToSticker()
        {
            AssetFields fields = new AssetFields(name);
            Sticker sticker = Read(fields);
            fields.ThrowIfAny();
            return sticker;
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

        private Sticker Read(AssetFields fields)
        {
            return new Sticker(fields.OwnId(id), retired, fields.Source);
        }
    }
}
