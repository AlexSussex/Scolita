using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="Unit"/> (a planet) as an asset (<c>Content/Domains/Units/&lt;id&gt;.asset</c>) with
    /// its lessons in teaching order, written by the content importer from <c>units.csv</c> and never
    /// edited by hand.
    /// </summary>
    public sealed class UnitAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string domain;
        [SerializeField] private bool retired;
        [SerializeField] private List<LessonAsset> lessons = new List<LessonAsset>();

        /// <summary>The unit's id.</summary>
        public string Id => id;

        /// <summary>Its lessons, in teaching order.</summary>
        public IReadOnlyList<LessonAsset> Lessons => lessons;

        /// <summary>Writes <paramref name="unit"/> and its <paramref name="unitLessons"/> into this asset; called by the content importer.</summary>
        public void Fill(Unit unit, IEnumerable<LessonAsset> unitLessons)
        {
            id = unit.Id.Value;
            domain = unit.DomainId.Value;
            retired = unit.Retired;
            lessons = unitLessons.ToList();
        }

        /// <summary>
        /// Adds the unit and its lessons to <paramref name="builder"/>; throws an
        /// <see cref="System.InvalidOperationException"/> if the asset can't be read. Called by
        /// <see cref="DomainAsset.AddTo"/>.
        /// </summary>
        public void AddTo(CurriculumBuilder builder)
        {
            AssetFields fields = new AssetFields(name);
            UnitEntry unit = Read(fields);
            fields.ThrowIfAny();
            builder.Add(unit);
            foreach (LessonAsset lesson in lessons)
            {
                lesson.AddTo(builder);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            return fields.Problems;
        }

        private UnitEntry Read(AssetFields fields)
        {
            if (lessons == null || lessons.Any(lesson => lesson == null))
            {
                fields.Problem("A lesson of it is missing.");
            }

            return new UnitEntry(fields.OwnId(id), fields.Id(domain, "domain"), retired, fields.Source);
        }
    }
}
