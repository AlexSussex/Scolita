using System.Collections.Generic;
using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="Letter"/> as an asset (<c>Content/Letters/&lt;id&gt;.asset</c>), written by the
    /// content importer from <c>letters.csv</c> and never edited by hand.
    /// </summary>
    public sealed class LetterAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string lower;
        [SerializeField] private string upper;
        [SerializeField] private string group;
        [SerializeField] private string strokes;
        [SerializeField] private bool retired;

        /// <summary>The letter's id.</summary>
        public string Id => id;

        /// <summary>Writes <paramref name="letter"/> into this asset; called by the content importer.</summary>
        public void Fill(Letter letter)
        {
            id = letter.Id.Value;
            lower = letter.Lower;
            upper = letter.Upper;
            group = ContentCodes.CodeOf(letter.Group);
            strokes = letter.Strokes.ToString();
            retired = letter.Retired;
        }

        /// <summary>
        /// The letter for Core; throws an <see cref="System.InvalidOperationException"/> if the asset
        /// can't be read. Called by <see cref="CurriculumAssets.Build"/>.
        /// </summary>
        public Letter ToLetter()
        {
            AssetFields fields = new AssetFields(name);
            Letter letter = Read(fields);
            fields.ThrowIfAny();
            return letter;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            return fields.Problems;
        }

        private Letter Read(AssetFields fields)
        {
            if (string.IsNullOrEmpty(lower) || string.IsNullOrEmpty(upper))
            {
                fields.Problem("It has no lower or upper case.");
            }

            return new Letter(fields.OwnId(id), lower, upper, fields.Group(group), fields.OptionalId(strokes, "strokes"),
                retired, fields.Source);
        }
    }
}
