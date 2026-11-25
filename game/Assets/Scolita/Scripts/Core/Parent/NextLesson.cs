using Scolita.Core.Content;

namespace Scolita.Core.Parent
{
    /// <summary>A lesson still to do, as the parent area lists it: its planet and its new letter, if any.</summary>
    public sealed class NextLesson
    {
        /// <summary>
        /// <paramref name="lesson"/> on planet <paramref name="planetNumber"/>, bringing <paramref name="letter"/>
        /// (null for a lesson that practises). Called by <see cref="ParentReport"/>.
        /// </summary>
        public NextLesson(Lesson lesson, int planetNumber, Letter letter)
        {
            Lesson = lesson;
            PlanetNumber = planetNumber;
            Letter = letter;
        }

        /// <summary>The lesson.</summary>
        public Lesson Lesson { get; }

        /// <summary>Its planet's number on the map, from 1.</summary>
        public int PlanetNumber { get; }

        /// <summary>The letter it brings; null when it practises what she has met (words, capitals, a party).</summary>
        public Letter Letter { get; }
    }
}
