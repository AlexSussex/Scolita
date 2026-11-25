using Scolita.Core.Content;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// What a finished lesson gave, for its end screen: its stars (always), a sticker and a trail (sometimes), and
    /// whether it finished its planet. Returned by <see cref="RewardGiver.LessonFinished"/>.
    /// </summary>
    public sealed class LessonRewards
    {
        /// <summary>Creates the rewards; called by <see cref="RewardGiver"/> and the tests.</summary>
        public LessonRewards(Lesson lesson, int stars, Sticker sticker, Trail trail, bool finishedPlanet)
        {
            Lesson = lesson;
            Stars = stars;
            Sticker = sticker;
            Trail = trail;
            FinishedPlanet = finishedPlanet;
        }

        /// <summary>The lesson finished.</summary>
        public Lesson Lesson { get; }

        /// <summary>The stars it gave.</summary>
        public int Stars { get; }

        /// <summary>The sticker it gave; null for none.</summary>
        public Sticker Sticker { get; }

        /// <summary>The trail it gave; null for none.</summary>
        public Trail Trail { get; }

        /// <summary>Whether it was the lesson that finished its planet, the first time.</summary>
        public bool FinishedPlanet { get; }
    }
}
