using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A lesson step as stored in a <see cref="LessonAsset"/>: its kind, its languages, its letters and
    /// words, and its options (<c>key=value</c>), as codes and ids in text.
    /// </summary>
    [Serializable]
    public sealed class StepData
    {
        [SerializeField] private string kind;
        [SerializeField] private string language;
        [SerializeField] private string[] letters;
        [SerializeField] private string[] words;
        [SerializeField] private string[] options;

        /// <summary>An empty step, for Unity's serialization.</summary>
        public StepData()
        {
        }

        /// <summary>The stored form of <paramref name="step"/>; called by <see cref="LessonAsset.Fill"/>.</summary>
        public StepData(Step step)
        {
            kind = ContentCodes.CodeOf(step.Kind);
            language = LanguageCodes.CodeOf(step.Languages);
            letters = step.Letters.Select(letter => letter.Value).ToArray();
            words = step.Words.Select(word => word.Value).ToArray();
            options = step.Options.Select(option => option.ToString()).ToArray();
        }

        /// <summary>The step as Core reads it, in the lesson <paramref name="lessonId"/>; called by <see cref="LessonAsset"/>.</summary>
        public StepEntry ToEntry(ContentId lessonId, AssetFields fields)
        {
            return new StepEntry(lessonId, fields.Kind(kind), fields.LanguageMixOf(language), fields.Ids(letters, "letter"),
                fields.Ids(words, "word"), Options(fields), fields.Source);
        }

        private IReadOnlyList<StepOption> Options(AssetFields fields)
        {
            List<StepOption> read = new List<StepOption>();
            foreach (string option in options ?? new string[0])
            {
                string[] pair = option.Split('=');
                if (pair.Length != 2)
                {
                    fields.Problem("A step's option '" + option + "' is not key=value.");
                    continue;
                }

                read.Add(new StepOption(pair[0], pair[1]));
            }

            return read;
        }
    }
}
