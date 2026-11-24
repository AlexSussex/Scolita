using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="Lesson"/> and its steps as an asset (<c>Content/Domains/Lessons/&lt;id&gt;.asset</c>),
    /// written by the content importer from <c>lessons.csv</c> and <c>steps.csv</c> and never edited
    /// by hand.
    /// </summary>
    public sealed class LessonAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string unit;
        [SerializeField] private string language;
        [SerializeField] private string introduces;
        [SerializeField] private bool retired;
        [SerializeField] private List<StepData> steps = new List<StepData>();

        /// <summary>The lesson's id.</summary>
        public string Id => id;

        /// <summary>Writes <paramref name="lesson"/> and its steps into this asset; called by the content importer.</summary>
        public void Fill(Lesson lesson)
        {
            id = lesson.Id.Value;
            unit = lesson.UnitId.Value;
            language = LanguageCodes.CodeOf(lesson.Languages);
            introduces = lesson.Introduces.ToString();
            retired = lesson.Retired;
            steps = lesson.Steps.Select(step => new StepData(step)).ToList();
        }

        /// <summary>
        /// Adds the lesson and its steps to <paramref name="builder"/>; throws an
        /// <see cref="System.InvalidOperationException"/> if the asset can't be read. Called by
        /// <see cref="UnitAsset.AddTo"/>.
        /// </summary>
        public void AddTo(CurriculumBuilder builder)
        {
            AssetFields fields = new AssetFields(name);
            LessonEntry lesson = Read(fields, out IReadOnlyList<StepEntry> lessonSteps);
            fields.ThrowIfAny();
            builder.Add(lesson);
            foreach (StepEntry step in lessonSteps)
            {
                builder.Add(step);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields, out IReadOnlyList<StepEntry> _);
            return fields.Problems;
        }

        private LessonEntry Read(AssetFields fields, out IReadOnlyList<StepEntry> lessonSteps)
        {
            ContentId lessonId = fields.OwnId(id);
            lessonSteps = (steps ?? new List<StepData>())
                .Select(step => step.ToEntry(lessonId, fields))
                .ToArray();
            return new LessonEntry(lessonId, fields.Id(unit, "unit"), fields.LanguageMixOf(language),
                fields.OptionalId(introduces, "introduces"), retired, fields.Source);
        }
    }
}
