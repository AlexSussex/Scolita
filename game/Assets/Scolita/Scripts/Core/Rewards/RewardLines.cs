using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// What the rewards say, by the voice lines' naming rule (<see cref="VoiceLineIds"/>), so the content
    /// holds the words: a constellation's stars at a lesson's end (<c>&lt;lang&gt;-line-stars-&lt;id&gt;</c>) and its
    /// name in the sky book (<c>constellation-&lt;id&gt;</c>), a new sticker, a new trail, and the lines that open
    /// the sky book, the sticker book and the trail picker.
    /// </summary>
    public static class RewardLines
    {
        /// <summary>The key of the line said when a sticker comes out of its box.</summary>
        public const string StickerKey = "sticker";

        /// <summary>The key of the line said when a planet gives a new trail.</summary>
        public const string TrailKey = "trail";

        /// <summary>The key of the line said when the sky book opens.</summary>
        public const string SkyBookKey = "sky-book";

        /// <summary>The key of the line said when the sticker book opens.</summary>
        public const string StickerBookKey = "sticker-book";

        /// <summary>The key of the line said when the trail picker opens.</summary>
        public const string TrailPickerKey = "trail-picker";

        private const string StarsKeyPrefix = "stars-";
        private const string NameKeyPrefix = "constellation-";

        /// <summary>
        /// The keys of the lines every language of the lessons needs, whatever the constellations. Called by
        /// <see cref="RewardContentRules"/> and <c>RewardsSession</c>.
        /// </summary>
        public static IReadOnlyList<string> CommonKeys { get; } = System.Array.AsReadOnly(new[]
        {
            StickerKey, TrailKey, SkyBookKey, StickerBookKey, TrailPickerKey
        });

        /// <summary>
        /// "Bravo! Ai aprins 3 stele în constelația …": the stars a lesson lit in <paramref name="constellation"/>,
        /// in <paramref name="language"/>. Called by <c>RewardsSession</c> and the content rules.
        /// </summary>
        public static VoiceLineId Stars(Constellation constellation, Language language)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, StarsKeyPrefix + constellation.Id.Value);
        }

        /// <summary>
        /// "Aceasta este constelația …": <paramref name="constellation"/>'s name in <paramref name="language"/>,
        /// said when she taps it in the sky book. Called by <c>RewardsSession</c> and the content rules.
        /// </summary>
        public static VoiceLineId Name(Constellation constellation, Language language)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, NameKeyPrefix + constellation.Id.Value);
        }

        /// <summary>
        /// The line <paramref name="key"/> (one of <see cref="CommonKeys"/>) in <paramref name="language"/>; called
        /// by <c>RewardsSession</c> and the content rules.
        /// </summary>
        public static VoiceLineId Common(string key, Language language)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, key);
        }
    }
}
