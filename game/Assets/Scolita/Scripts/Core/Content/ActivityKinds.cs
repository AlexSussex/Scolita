using System;
using System.Collections.Generic;

namespace Scolita.Core.Content
{
    /// <summary>
    /// What a step of each <see cref="ActivityKind"/> needs from the content
    /// (<c>docs/curriculum.md</c> §4.2): the one place a kind's brief changes when its kind needs
    /// more (another option, its hint lines).
    /// </summary>
    public static class ActivityKinds
    {
        private static readonly IReadOnlyList<VoiceLineKind> NoWordLines = new VoiceLineKind[0];
        private static readonly IReadOnlyList<ActivityOption> NoOptions = new ActivityOption[0];
        private static readonly IReadOnlyList<string> NoLines = new string[0];

        private static readonly IReadOnlyList<ActivityKindRequirements> Table = new[]
        {
            Kind(ActivityKind.MeetLetter, Range(1, 1), Range(1, 1), Range(2, 2), ActivityItemRules.WordStartsWithALetter,
                ActivityWordUse.Pictured, NoWordLines, false, false, NoOptions,
                Lines("meet-new", "meet-listen", "meet-tap-letter", "meet-tap-picture"), NoLines),
            Kind(ActivityKind.ListenAndPick, Range(0, 4), Range(0, 4), Range(2, 4), ActivityItemRules.LettersOrWords,
                ActivityWordUse.Pictured, NoWordLines, false, true, NoOptions,
                Lines("pick-letter", "pick-letter-x", "pick-picture"),
                Lines("listen-again", "pick-hint-other", "pick-hint-shape", "pick-hint-show", "show-answer")),
            Kind(ActivityKind.LetterHunt, Range(2, 3), Range(0, 0), Range(2, 3), ActivityItemRules.None,
                ActivityWordUse.Heard, NoWordLines, false, true, NoOptions, Lines("hunt", "hunt-capitals"),
                Lines("hunt-hint-twinkle", "hunt-hint-capital", "hunt-hint-more", "hunt-hint-not", "pick-hint-show")),
            Kind(ActivityKind.FirstSound, Range(2, 4), Range(1, 1), Range(3, 5), ActivityItemRules.WordStartsWithALetter,
                ActivityWordUse.Pictured, new[] { VoiceLineKind.Stretched }, false, true, NoOptions,
                Lines("first-sound", "first-sound-tap"), Lines("first-hint-stretch", "first-hint-which", "pick-hint-show")),
            Kind(ActivityKind.TraceLetter, Range(1, 1), Range(0, 0), Range(1, 1), ActivityItemRules.None,
                ActivityWordUse.Heard, NoWordLines, false, false, new[]
                {
                    new ActivityOption("form", new[] { "print", "handwritten" }, true),
                    new ActivityOption("case", new[] { "lower", "upper" }, true)
                }, Lines("trace-start"), NoLines),
            Kind(ActivityKind.TraceWord, Range(0, 0), Range(1, 1), Range(1, 1), ActivityItemRules.None,
                ActivityWordUse.Read, NoWordLines, false, false, NoOptions, Lines("word-write"), NoLines),
            Kind(ActivityKind.Syllables, Range(0, 0), Range(1, 1), Range(1, 1), ActivityItemRules.None,
                ActivityWordUse.Pictured, new[] { VoiceLineKind.Syllables }, false, false, NoOptions, Lines("syll"),
                NoLines),
            Kind(ActivityKind.BuildWord, Range(0, 2), Range(1, 1), Range(1, 3), ActivityItemRules.None,
                ActivityWordUse.Read, NoWordLines, false, false, NoOptions, Lines("build"), NoLines),
            Kind(ActivityKind.ReadAndMatch, Range(0, 0), Range(2, 3), Range(2, 3), ActivityItemRules.None,
                ActivityWordUse.ReadAndPictured, NoWordLines, false, false, NoOptions, Lines("read"), NoLines),
            Kind(ActivityKind.TwoSounds, Range(1, 1), Range(4, 6), Range(5, 7), ActivityItemRules.SortsByTheLettersTwoSounds,
                ActivityWordUse.Pictured, NoWordLines, true, false, NoOptions, Lines("two-same-letter", "two-tap", "two"),
                Lines("two-hint-again", "two-hint-start", "two-hint-badges", "two-hint-show")),
            Kind(ActivityKind.WordTwins, Range(0, 0), Range(2, 8), Range(2, 8), ActivityItemRules.None,
                ActivityWordUse.Pictured, NoWordLines, true, false, NoOptions, Lines("twins"), NoLines)
        };

        /// <summary>
        /// What a step of <paramref name="kind"/> needs. Called by <see cref="CurriculumBuilder"/>
        /// (a bridge kind's languages), the step rule and <c>ActivityKindsTests</c>.
        /// </summary>
        public static ActivityKindRequirements Of(ActivityKind kind)
        {
            foreach (ActivityKindRequirements requirements in Table)
            {
                if (requirements.Kind == kind)
                {
                    return requirements;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(kind), kind, "No requirements for this kind.");
        }

        private static ActivityKindRequirements Kind(ActivityKind kind, ItemRange letters, ItemRange words,
            ItemRange items, ActivityItemRules itemRules, ActivityWordUse wordUse, IReadOnlyList<VoiceLineKind> wordLines,
            bool isBridge, bool lettersAreChoices, IReadOnlyList<ActivityOption> options,
            IReadOnlyList<string> instructionLines, IReadOnlyList<string> hintLines)
        {
            return new ActivityKindRequirements(kind, letters, words, items, itemRules, wordUse, wordLines, isBridge,
                lettersAreChoices, options, instructionLines, hintLines);
        }

        private static IReadOnlyList<string> Lines(params string[] keys)
        {
            return keys;
        }

        private static ItemRange Range(int min, int max)
        {
            return new ItemRange(min, max);
        }
    }
}
