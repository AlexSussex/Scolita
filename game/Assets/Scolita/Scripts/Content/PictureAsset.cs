using System.Collections.Generic;
using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="Picture"/> as an asset (<c>Content/Pictures/&lt;id&gt;.asset</c>) with its sprite
    /// (<c>&lt;id&gt;.png</c> beside it: the art from <c>content/pictures/</c>, or the placeholder the
    /// importer draws), written by the content importer and never edited by hand.
    /// </summary>
    public sealed class PictureAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string art;
        [SerializeField] private bool retired;
        [SerializeField] private Sprite sprite;

        /// <summary>The picture's id.</summary>
        public string Id => id;

        /// <summary>What the activities show: the art, or its placeholder.</summary>
        public Sprite Sprite => sprite;

        /// <summary>Writes <paramref name="picture"/> and its <paramref name="pictureSprite"/> into this asset; called by the content importer.</summary>
        public void Fill(Picture picture, Sprite pictureSprite)
        {
            id = picture.Id.Value;
            art = picture.Art;
            retired = picture.Retired;
            sprite = pictureSprite;
        }

        /// <summary>
        /// The picture for Core; throws an <see cref="System.InvalidOperationException"/> if the asset
        /// can't be read. A missing sprite is never fatal on a tablet: it logs a warning and the
        /// activities show a placeholder (the validator fails on it before a release). Called by
        /// <see cref="CurriculumAssets.Build"/>.
        /// </summary>
        public Picture ToPicture()
        {
            AssetFields fields = new AssetFields(name);
            Picture picture = Read(fields);
            fields.ThrowIfAny();
            if (sprite == null)
            {
                Debug.LogWarning("The picture " + name + " has no sprite: its activities show a placeholder.");
            }

            return picture;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            if (sprite == null)
            {
                fields.Problem("It has no sprite (run \u0218coli\u021Ba \u2192 Import content).");
            }

            return fields.Problems;
        }

        private Picture Read(AssetFields fields)
        {
            return new Picture(fields.OwnId(id), art, retired, fields.Source);
        }
    }
}
