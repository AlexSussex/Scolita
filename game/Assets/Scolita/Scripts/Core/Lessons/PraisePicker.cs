using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// Chooses the praise after a step, varied (<c>docs/curriculum.md</c> §4.3): one of the
    /// language's praise lines (<see cref="LessonLines.Praise"/>), never one of the last
    /// <see cref="LearningRules.PraiseNotRepeatedWithin"/> said in that language. Its lines are made
    /// when the lesson is built, so choosing one allocates nothing.
    /// </summary>
    public sealed class PraisePicker
    {
        private readonly VoiceSequence[][] lines;
        private readonly int[][] recent;
        private readonly int[] recentCount;
        private readonly int[] nextRecent;
        private readonly IRandom random;
        private readonly List<VoiceSequence> all = new List<VoiceSequence>();

        /// <summary>
        /// Prepares the praise of both languages in <paramref name="curriculum"/>. Called by
        /// <see cref="LessonRun"/> and the tests.
        /// </summary>
        public PraisePicker(Curriculum curriculum, LearningRules rules, IRandom random)
        {
            this.random = random;
            int languages = Enum.GetValues(typeof(Language)).Length;
            lines = new VoiceSequence[languages][];
            recent = new int[languages][];
            recentCount = new int[languages];
            nextRecent = new int[languages];
            foreach (Language language in (Language[])Enum.GetValues(typeof(Language)))
            {
                IReadOnlyList<VoiceLineId> praise = LessonLines.Praise(curriculum, language);
                VoiceSequence[] sequences = new VoiceSequence[praise.Count];
                for (int index = 0; index < praise.Count; index++)
                {
                    sequences[index] = VoiceSequence.Of(praise[index]);
                }

                lines[(int)language] = sequences;
                all.AddRange(sequences);
                recent[(int)language] = new int[Math.Max(0, Math.Min(rules.PraiseNotRepeatedWithin, praise.Count - 1))];
            }
        }

        /// <summary>Every praise line, in both languages, so their bubble words are prepared when the lesson is built.</summary>
        public IReadOnlyList<VoiceSequence> Lines => all;

        /// <summary>
        /// The next praise in <paramref name="language"/>; throws an <see cref="InvalidOperationException"/>
        /// when the content has none (the validator makes that impossible). Called by
        /// <see cref="LessonRun"/> when a step is done, and the tests.
        /// </summary>
        public VoiceSequence Next(Language language)
        {
            VoiceSequence[] choices = lines[(int)language];
            if (choices.Length == 0)
            {
                throw new InvalidOperationException("No praise lines in " + language + " (" + LessonLines.PraiseLine(language, 1) + ").");
            }

            int[] said = recent[(int)language];
            int count = recentCount[(int)language];
            int pick = random.NextInt(0, choices.Length - count);
            int chosen = NthNotRecent(pick, said, count);
            Remember(language, chosen);
            return choices[chosen];
        }

        private static int NthNotRecent(int nth, int[] said, int count)
        {
            for (int index = 0; ; index++)
            {
                if (IsAmong(index, said, count))
                {
                    continue;
                }

                if (nth == 0)
                {
                    return index;
                }

                nth--;
            }
        }

        private static bool IsAmong(int index, int[] said, int count)
        {
            for (int position = 0; position < count; position++)
            {
                if (said[position] == index)
                {
                    return true;
                }
            }

            return false;
        }

        private void Remember(Language language, int chosen)
        {
            int[] said = recent[(int)language];
            if (said.Length == 0)
            {
                return;
            }

            int slot = (int)language;
            if (recentCount[slot] < said.Length)
            {
                said[recentCount[slot]] = chosen;
                recentCount[slot]++;
                return;
            }

            said[nextRecent[slot]] = chosen;
            nextRecent[slot] = (nextRecent[slot] + 1) % said.Length;
        }
    }
}
