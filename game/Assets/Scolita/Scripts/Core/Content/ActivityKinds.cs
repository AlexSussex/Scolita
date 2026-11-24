using System;
using System.Collections.Generic;

namespace Scolita.Core.Content
{
    /// <summary>
    /// What a step of each <see cref="ActivityKind"/> needs from the content
    /// (<c>docs/curriculum.md</c> §4.2): the one place a kind's brief changes when its kind needs
    /// more (another option, its hints' lines).
    /// </summary>
    public static class ActivityKinds
    {
        private static readonly IReadOnlyList<VoiceLineKind> NoWordLines = new VoiceLineKind[0];
        private static readonly IReadOnlyList<ActivityOption> NoOptions = new ActivityOption[0];

        private static readonly IReadOnlyList<ActivityKindRequirements> Table = new[]
        {
            Kind(ActivityKind.MeetLetter, Range(1, 1), Range(0, 1), Range(1, 2), ActivityItemRules.WordStartsWithALetter,
                ActivityWordUse.Pictured,
                NoWordLines, false, NoOptions, "meet-new", "meet-listen"),
            Kind(ActivityKind.ListenAndPick, Range(0, 4), Range(0, 4), Range(2, 4), ActivityItemRules.LettersOrWords,
                ActivityWordUse.Pictured,
                NoWordLines, false, NoOptions, "pick-letter", "pick-picture"),
            Kind(ActivityKind.LetterHunt, Range(1, 3), Range(0, 0), Range(1, 3), ActivityItemRules.None,
                ActivityWordUse.Heard,
                NoWordLines, false, NoOptions, "hunt"),
            Kind(ActivityKind.FirstSound, Range(2, 4), Range(1, 1), Range(3, 5), ActivityItemRules.WordStartsWithALetter,
                ActivityWordUse.Pictured,
                NoWordLines, false, NoOptions, "first-sound", "first-sound-tap"),
            Kind(ActivityKind.TraceLetter, Range(1, 1), Range(0, 0), Range(1, 1), ActivityItemRules.None,
                ActivityWordUse.Heard,
                NoWordLines, false, new[]
                {
                    new ActivityOption("form", new[] { "print", "handwritten" }, true),
                    new ActivityOption("case", new[] { "lower", "upper" }, true)
                }, "trace-start"),
            Kind(ActivityKind.TraceWord, Range(0, 0), Range(1, 1), Range(1, 1), ActivityItemRules.None,
                ActivityWordUse.Read,
                NoWordLines, false, NoOptions, "word-write"),
            Kind(ActivityKind.Syllables, Range(0, 0), Range(1, 1), Range(1, 1), ActivityItemRules.None,
                ActivityWordUse.Pictured,
                new[] { VoiceLineKind.Syllables }, false, NoOptions, "syll"),
            Kind(ActivityKind.BuildWord, Range(0, 2), Range(1, 1), Range(1, 3), ActivityItemRules.None,
                ActivityWordUse.Read,
                NoWordLines, false, NoOptions, "build"),
            Kind(ActivityKind.ReadAndMatch, Range(0, 0), Range(2, 3), Range(2, 3), ActivityItemRules.None,
                ActivityWordUse.ReadAndPictured,
                NoWordLines, false, NoOptions, "read"),
            Kind(ActivityKind.TwoSounds, Range(1, 1), Range(4, 6), Range(5, 7), ActivityItemRules.None,
                ActivityWordUse.Pictured,
                NoWordLines, true, NoOptions, "two", "two-tap"),
            Kind(ActivityKind.WordTwins, Range(0, 0), Range(2, 8), Range(2, 8), ActivityItemRules.None,
                ActivityWordUse.Pictured,
                NoWordLines, true, NoOptions, "twins")
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
            bool isBridge, IReadOnlyList<ActivityOption> options, params string[] instructionLines)
        {
            return new ActivityKindRequirements(kind, letters, words, items, itemRules, wordUse, wordLines, isBridge,
                options, instructionLines);
        }

        private static ItemRange Range(int min, int max)
        {
            return new ItemRange(min, max);
        }
    }
}
