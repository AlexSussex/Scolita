using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Content
{
    /// <summary>
    /// Reads a content asset's serialized fields (ids and codes as text) back into the Core model's
    /// types, collecting every field it can't read. A content asset uses it both to check itself
    /// (<see cref="IValidatableContent.Validate"/>) and to give Core its item, where a problem is an
    /// impossible state (the importer wrote it) and throws.
    /// </summary>
    public sealed class AssetFields
    {
        private readonly string assetName;
        private readonly List<string> problems = new List<string>();

        /// <summary>Creates a reader for the asset named <paramref name="assetName"/>; called by every content asset.</summary>
        public AssetFields(string assetName)
        {
            this.assetName = assetName ?? string.Empty;
        }

        /// <summary>The problems found so far.</summary>
        public IReadOnlyList<string> Problems => problems;

        /// <summary>Where the asset's items come from, for Core's messages.</summary>
        public ContentSource Source => new ContentSource(assetName, 0);

        /// <summary>Adds a problem; called by the content assets for checks of their own.</summary>
        public void Problem(string problem)
        {
            problems.Add(problem);
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> naming every problem, if there is any.
        /// Called by the content assets before they hand Core their item.
        /// </summary>
        public void ThrowIfAny()
        {
            if (problems.Count > 0)
            {
                throw new InvalidOperationException("The content asset " + assetName + " can't be read (run "
                    + "\u0218coli\u021Ba \u2192 Import content): " + string.Join(" ", problems));
            }
        }

        /// <summary>
        /// The id in <paramref name="text"/>, which must be set and match the asset's name (an asset is
        /// named after its id). Called by the content assets.
        /// </summary>
        public ContentId OwnId(string text)
        {
            ContentId id = ReadId(text, "id");
            if (id.IsSet && id.Value != assetName)
            {
                problems.Add("Its id " + id + " is not its name " + assetName + ".");
            }

            return id;
        }

        /// <summary>The id in <paramref name="text"/> (the field <paramref name="field"/>), which must be set; called by the content assets.</summary>
        public ContentId Id(string text, string field)
        {
            return ReadId(text, field);
        }

        /// <summary>The id in <paramref name="text"/>, or the unset id when it is empty; called by the content assets.</summary>
        public ContentId OptionalId(string text, string field)
        {
            return string.IsNullOrEmpty(text) ? default : ReadId(text, field);
        }

        /// <summary>The ids in <paramref name="texts"/>; called by the content assets.</summary>
        public IReadOnlyList<ContentId> Ids(IEnumerable<string> texts, string field)
        {
            return (texts ?? Enumerable.Empty<string>())
                .Select(text => ReadId(text, field))
                .ToArray();
        }

        /// <summary>The language in <paramref name="code"/> (<c>ro</c>, <c>en</c>); called by the content assets.</summary>
        public Language OneLanguage(string code)
        {
            if (!LanguageCodes.TryParseLanguage(code, out Language language))
            {
                problems.Add("Its language '" + code + "' is not ro or en.");
            }

            return language;
        }

        /// <summary>The languages in <paramref name="code"/> (<c>ro</c>, <c>en</c>, <c>both</c>); called by the content assets.</summary>
        public LanguageMix LanguageMixOf(string code)
        {
            if (!LanguageCodes.TryParseMix(code, out LanguageMix mix))
            {
                problems.Add("Its language '" + code + "' is not ro, en or both.");
            }

            return mix;
        }

        private ContentId ReadId(string text, string field)
        {
            if (ContentId.TryCreate(text, out ContentId id))
            {
                return id;
            }

            problems.Add("Its " + field + " '" + text + "' is not an id.");
            return default;
        }

        /// <summary>The letter group in <paramref name="code"/>; called by <see cref="LetterAsset"/>.</summary>
        public LetterGroup Group(string code)
        {
            if (!ContentCodes.TryParseGroup(code, out LetterGroup group))
            {
                problems.Add("Its group '" + code + "' is not a letter group.");
            }

            return group;
        }

        /// <summary>The activity kind in <paramref name="code"/>; called by <see cref="LessonAsset"/>.</summary>
        public ActivityKind Kind(string code)
        {
            if (!ContentCodes.TryParseKind(code, out ActivityKind kind))
            {
                problems.Add("A step's kind '" + code + "' is not an activity kind.");
            }

            return kind;
        }

        /// <summary>The voice line kind in <paramref name="code"/>; called by <see cref="VoiceLineAsset"/>.</summary>
        public VoiceLineKind LineKind(string code)
        {
            if (!ContentCodes.TryParseVoiceLineKind(code, out VoiceLineKind kind))
            {
                problems.Add("Its kind '" + code + "' is not a voice line kind.");
            }

            return kind;
        }
    }
}
