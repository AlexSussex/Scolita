using System;
using System.Collections.Generic;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// A word in one language (<c>ro-mama</c>): its text, its letters (letter ids, as a group like
    /// <c>ch</c> is one letter), its syllables, its picture, whether it is only heard, its twin and
    /// false friend in the other language and, by the naming rule, its voice lines. One row of
    /// <c>words.csv</c>.
    /// </summary>
    public sealed class Word : IContentItem
    {
        /// <summary>Creates a word; called by <see cref="ContentTables"/>, the word asset and tests.</summary>
        public Word(ContentId id, Language language, string text, IReadOnlyList<ContentId> letters,
            IReadOnlyList<string> syllables, ContentId picture, bool listenOnly, ContentId twin, ContentId falseFriend,
            bool retired, ContentSource source)
        {
            Id = id;
            Language = language;
            Text = text ?? string.Empty;
            Letters = letters ?? new ContentId[0];
            Syllables = syllables ?? new string[0];
            Picture = picture;
            ListenOnly = listenOnly;
            Twin = twin;
            FalseFriend = falseFriend;
            Retired = retired;
            Source = source;
        }

        /// <inheritdoc/>
        public ContentId Id { get; }

        /// <summary>The word's one language.</summary>
        public Language Language { get; }

        /// <summary>The word as written (<c>mama</c>, <c>tam-tam</c>, <c>Ana</c>).</summary>
        public string Text { get; }

        /// <summary>Its letters, as letter ids, in order; may be empty for a listen-only word.</summary>
        public IReadOnlyList<ContentId> Letters { get; }

        /// <summary>Its syllables (Romanian, as school splits them) or beats (English), in order.</summary>
        public IReadOnlyList<string> Syllables { get; }

        /// <summary>Its picture; not set for a word with none (a reading-only word such as <c>am</c>).</summary>
        public ContentId Picture { get; }

        /// <summary>
        /// Whether it is only heard and pictured, never shown to read (<c>docs/curriculum.md</c> §3):
        /// its letters needn't be taught.
        /// </summary>
        public bool ListenOnly { get; }

        /// <summary>The other language's word that is alike (<c>mop</c> / <c>mop</c>); not set for none.</summary>
        public ContentId Twin { get; }

        /// <summary>The other language's word spelled the same with another meaning; not set for none.</summary>
        public ContentId FalseFriend { get; }

        /// <inheritdoc/>
        public bool Retired { get; }

        /// <inheritdoc/>
        public ContentSource Source { get; }

        /// <summary>
        /// The part of its id after its language (<c>mama</c> for <c>ro-mama</c>), which its voice
        /// lines are named by; the whole id when it doesn't start with its language (a content error).
        /// </summary>
        public string Key
        {
            get
            {
                string prefix = LanguageCodes.CodeOf(Language) + "-";
                string id = Id.ToString();
                return id.StartsWith(prefix, StringComparison.Ordinal) && id.Length > prefix.Length
                    ? id.Substring(prefix.Length)
                    : id;
            }
        }

        /// <summary>The word said normally (<c>ro-word-mama</c>); called by the content rules and the activities.</summary>
        public VoiceLineId WordLine => VoiceLineIds.For(Language, VoiceLineKind.Word, Key);

        /// <summary>The word said in syllables (<c>ro-syllables-mama</c>); called by the content rules and the activities.</summary>
        public VoiceLineId SyllablesLine => VoiceLineIds.For(Language, VoiceLineKind.Syllables, Key);

        /// <summary>
        /// The word said sound by sound (<c>ro-slow-mama</c>); optional, as <c>voice</c> may play its
        /// letters' sounds instead. Called by the activities.
        /// </summary>
        public VoiceLineId SlowLine => VoiceLineIds.For(Language, VoiceLineKind.Slow, Key);
    }
}
