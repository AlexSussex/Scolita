using System;
using Scolita.Core.Content;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The lesson in progress, or the one Go opens next, saved as <c>lesson-run.json</c> in the
    /// profile's folder through <see cref="SaveStore.ForProfile"/> (AGENTS.md, seam 5): its id, the
    /// lesson's own step number to resume at (from 1), and the signature of the lesson's steps when it
    /// was saved, so a lesson whose steps changed starts again from its first step. The map writes the
    /// lesson it opens here; a profile from before the map (0.9.0 and earlier) had only this file, so
    /// her progress is made from it then (<c>MapProgress.Before</c>).
    /// </summary>
    [Serializable]
    public sealed class LessonRunData : ISaveData<LessonRunData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "lesson-run";

        private const int LatestVersion = 1;
        private const int FirstStep = 1;

        [SerializeField] private int version;
        [SerializeField] private string lessonId;
        [SerializeField] private int stepNumber;
        [SerializeField] private string stepsSignature;

        /// <summary>
        /// Creates the data for the lesson <paramref name="lesson"/>, resuming at
        /// <paramref name="step"/>, its steps' signature being <paramref name="signature"/>. Called by
        /// <see cref="LessonRunner"/> and the tests.
        /// </summary>
        public LessonRunData(ContentId lesson, int step, string signature)
            : this(lesson.IsSet ? lesson.Value : string.Empty, step, signature)
        {
        }

        private LessonRunData(string lessonCode, int step, string signature)
        {
            version = LatestVersion;
            lessonId = lessonCode ?? string.Empty;
            stepNumber = Math.Max(FirstStep, step);
            stepsSignature = signature ?? string.Empty;
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>The step number to resume at, from 1.</summary>
        public int StepNumber => stepNumber;

        /// <summary>The signature of the lesson's steps when it was saved.</summary>
        public string StepsSignature => stepsSignature;

        /// <summary>
        /// No lesson yet: Go opens the first lesson. Called by <see cref="LessonRunner"/> and the tests.
        /// </summary>
        public static LessonRunData CreateDefaults()
        {
            return new LessonRunData(string.Empty, FirstStep, string.Empty);
        }

        /// <summary>
        /// The saved lesson's id; false for none, or for an id that isn't one (kept as it is, read as
        /// none). Called by <see cref="LessonRunner"/> and the tests.
        /// </summary>
        public bool TryGetLessonId(out ContentId id)
        {
            return ContentId.TryCreate(lessonId, out id);
        }

        /// <summary>
        /// Returns this data in the current shape: a step number below 1 becomes 1, a missing text
        /// becomes empty, and an unknown lesson id is kept (and ignored, <see cref="TryGetLessonId"/>).
        /// Called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public LessonRunData Upgraded()
        {
            return new LessonRunData(lessonId, stepNumber, stepsSignature);
        }
    }
}
