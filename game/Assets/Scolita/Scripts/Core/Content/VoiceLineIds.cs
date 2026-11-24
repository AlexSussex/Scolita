using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// The naming rule of voice lines (AGENTS.md → "Audio"): <c>&lt;language&gt;-&lt;kind&gt;-&lt;key&gt;</c>,
    /// as <c>ro-sound-m</c>, <c>en-word-pat</c>, <c>ro-line-try-again</c>. A letter, a word or an
    /// activity kind names the lines it speaks by this rule, so none needs a column per line.
    /// </summary>
    public static class VoiceLineIds
    {
        /// <summary>
        /// The id of the <paramref name="kind"/> line of <paramref name="key"/> in
        /// <paramref name="language"/>. Throws an <see cref="System.ArgumentException"/> if the result
        /// isn't a valid id. Called by <see cref="Letter"/>, <see cref="Word"/>, the content rules and
        /// <c>VoiceLineIdsTests</c>.
        /// </summary>
        public static VoiceLineId For(Language language, VoiceLineKind kind, string key)
        {
            string text = LanguageCodes.CodeOf(language) + "-" + ContentCodes.CodeOf(kind) + "-" + key;
            return new VoiceLineId(ContentId.Create(text));
        }

        /// <summary>
        /// Reads the language and the kind out of <paramref name="id"/>; false when it doesn't
        /// start with a language code, then a kind code, then a non-empty key. Called by the voice
        /// line rule (an id must match its <c>language</c> and <c>kind</c> columns) and
        /// <c>VoiceLineIdsTests</c>.
        /// </summary>
        public static bool TryParse(ContentId id, out Language language, out VoiceLineKind kind)
        {
            language = default;
            kind = default;
            string[] parts = id.ToString().Split(new[] { '-' }, 3);
            return parts.Length == 3
                && parts[2].Length > 0
                && LanguageCodes.TryParseLanguage(parts[0], out language)
                && ContentCodes.TryParseVoiceLineKind(parts[1], out kind);
        }
    }
}
