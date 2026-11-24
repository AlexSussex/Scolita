using System;
using System.Collections.Generic;
using System.Linq;

namespace Scolita.Core.Content
{
    /// <summary>
    /// The stable codes the content CSVs and the content assets write for <see cref="ActivityKind"/>
    /// (<c>MeetLetter</c>), <see cref="LetterGroup"/> (<c>shared-close</c>) and
    /// <see cref="VoiceLineKind"/> (<c>sound</c>), and the <c>yes</c> of the yes/no columns, so
    /// renaming an enum value never changes a file. Languages use <c>LanguageCodes</c>.
    /// </summary>
    public static class ContentCodes
    {
        /// <summary>The code of "yes" in a yes/no column (<c>retired</c>, <c>listen_only</c>); "no" is empty.</summary>
        public const string Yes = "yes";

        private static readonly IReadOnlyList<KeyValuePair<string, ActivityKind>> Kinds = new[]
        {
            Pair("MeetLetter", ActivityKind.MeetLetter),
            Pair("ListenAndPick", ActivityKind.ListenAndPick),
            Pair("LetterHunt", ActivityKind.LetterHunt),
            Pair("FirstSound", ActivityKind.FirstSound),
            Pair("TraceLetter", ActivityKind.TraceLetter),
            Pair("TraceWord", ActivityKind.TraceWord),
            Pair("Syllables", ActivityKind.Syllables),
            Pair("BuildWord", ActivityKind.BuildWord),
            Pair("ReadAndMatch", ActivityKind.ReadAndMatch),
            Pair("TwoSounds", ActivityKind.TwoSounds),
            Pair("WordTwins", ActivityKind.WordTwins)
        };

        private static readonly IReadOnlyList<KeyValuePair<string, LetterGroup>> Groups = new[]
        {
            Pair("shared-close", LetterGroup.SharedClose),
            Pair("vowel-differs", LetterGroup.VowelDiffers),
            Pair("sound-differs", LetterGroup.SoundDiffers),
            Pair("romanian-only", LetterGroup.RomanianOnly),
            Pair("english-only", LetterGroup.EnglishOnly)
        };

        private static readonly IReadOnlyList<KeyValuePair<string, VoiceLineKind>> LineKinds = new[]
        {
            Pair("sound", VoiceLineKind.Sound),
            Pair("name", VoiceLineKind.Name),
            Pair("word", VoiceLineKind.Word),
            Pair("slow", VoiceLineKind.Slow),
            Pair("syllables", VoiceLineKind.Syllables),
            Pair("line", VoiceLineKind.Line)
        };

        /// <summary>The code of <paramref name="kind"/>; called by the content assets and <see cref="ActivityKinds"/>.</summary>
        public static string CodeOf(ActivityKind kind)
        {
            return CodeIn(Kinds, kind);
        }

        /// <summary>The code of <paramref name="group"/>; called by the content assets.</summary>
        public static string CodeOf(LetterGroup group)
        {
            return CodeIn(Groups, group);
        }

        /// <summary>The code of <paramref name="kind"/>; called by <see cref="VoiceLineIds"/> and the content assets.</summary>
        public static string CodeOf(VoiceLineKind kind)
        {
            return CodeIn(LineKinds, kind);
        }

        /// <summary>Reads an activity kind from its exact code; called by <see cref="ContentTables"/> and the content assets.</summary>
        public static bool TryParseKind(string code, out ActivityKind kind)
        {
            return TryParseIn(Kinds, code, out kind);
        }

        /// <summary>Reads a letter group from its exact code; called by <see cref="ContentTables"/> and the content assets.</summary>
        public static bool TryParseGroup(string code, out LetterGroup group)
        {
            return TryParseIn(Groups, code, out group);
        }

        /// <summary>Reads a voice line kind from its exact code; called by <see cref="ContentTables"/> and the content assets.</summary>
        public static bool TryParseVoiceLineKind(string code, out VoiceLineKind kind)
        {
            return TryParseIn(LineKinds, code, out kind);
        }

        /// <summary>Every activity kind's code, for messages; called by <see cref="ContentTables"/>.</summary>
        public static string AllKindCodes()
        {
            return string.Join(", ", Kinds.Select(pair => pair.Key));
        }

        /// <summary>Every letter group's code, for messages; called by <see cref="ContentTables"/>.</summary>
        public static string AllGroupCodes()
        {
            return string.Join(", ", Groups.Select(pair => pair.Key));
        }

        /// <summary>Every voice line kind's code, for messages; called by <see cref="ContentTables"/>.</summary>
        public static string AllVoiceLineKindCodes()
        {
            return string.Join(", ", LineKinds.Select(pair => pair.Key));
        }

        private static KeyValuePair<string, T> Pair<T>(string code, T value)
        {
            return new KeyValuePair<string, T>(code, value);
        }

        private static string CodeIn<T>(IReadOnlyList<KeyValuePair<string, T>> table, T value) where T : struct, Enum
        {
            foreach (KeyValuePair<string, T> pair in table)
            {
                if (EqualityComparer<T>.Default.Equals(pair.Value, value))
                {
                    return pair.Key;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(value), value, "No code for this value.");
        }

        private static bool TryParseIn<T>(IReadOnlyList<KeyValuePair<string, T>> table, string code, out T value)
            where T : struct, Enum
        {
            foreach (KeyValuePair<string, T> pair in table)
            {
                if (string.Equals(pair.Key, code, StringComparison.Ordinal))
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
