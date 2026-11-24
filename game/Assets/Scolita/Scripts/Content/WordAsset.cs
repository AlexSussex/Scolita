using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="Word"/> as an asset (<c>Content/Words/&lt;id&gt;.asset</c>), written by the content
    /// importer from <c>words.csv</c> and never edited by hand.
    /// </summary>
    public sealed class WordAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string language;
        [SerializeField] private string text;
        [SerializeField] private string[] letters;
        [SerializeField] private string[] syllables;
        [SerializeField] private string picture;
        [SerializeField] private bool listenOnly;
        [SerializeField] private string twin;
        [SerializeField] private string falseFriend;
        [SerializeField] private bool retired;

        /// <summary>The word's id.</summary>
        public string Id => id;

        /// <summary>Writes <paramref name="word"/> into this asset; called by the content importer.</summary>
        public void Fill(Word word)
        {
            id = word.Id.Value;
            language = LanguageCodes.CodeOf(word.Language);
            text = word.Text;
            letters = word.Letters.Select(letter => letter.Value).ToArray();
            syllables = word.Syllables.ToArray();
            picture = word.Picture.ToString();
            listenOnly = word.ListenOnly;
            twin = word.Twin.ToString();
            falseFriend = word.FalseFriend.ToString();
            retired = word.Retired;
        }

        /// <summary>
        /// The word for Core; throws an <see cref="System.InvalidOperationException"/> if the asset
        /// can't be read. Called by <see cref="CurriculumAssets.Build"/>.
        /// </summary>
        public Word ToWord()
        {
            AssetFields fields = new AssetFields(name);
            Word word = Read(fields);
            fields.ThrowIfAny();
            return word;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            return fields.Problems;
        }

        private Word Read(AssetFields fields)
        {
            if (string.IsNullOrEmpty(text))
            {
                fields.Problem("It has no text.");
            }

            return new Word(fields.OwnId(id), fields.OneLanguage(language), text, fields.Ids(letters, "letter"),
                syllables, fields.OptionalId(picture, "picture"), listenOnly, fields.OptionalId(twin, "twin"),
                fields.OptionalId(falseFriend, "false friend"), retired, fields.Source);
        }
    }
}
