using Scolita.Core.Content;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// Raised by <see cref="RewardGiver.RewardEarned"/> for each reward a finished lesson gives: its stars, a sticker or a
    /// trail. <c>sound-feel</c> plays its sound from it.
    /// </summary>
    public readonly struct RewardEarned
    {
        /// <summary>Creates the event; called by <see cref="RewardGiver"/>.</summary>
        public RewardEarned(RewardKind kind, ContentId lessonId, int stars, ContentId itemId)
        {
            Kind = kind;
            LessonId = lessonId;
            Stars = stars;
            ItemId = itemId;
        }

        /// <summary>What was given.</summary>
        public RewardKind Kind { get; }

        /// <summary>The lesson whose end gave it.</summary>
        public ContentId LessonId { get; }

        /// <summary>The stars given (for <see cref="RewardKind.Stars"/>), else 0.</summary>
        public int Stars { get; }

        /// <summary>The sticker's or the trail's id; unset for stars.</summary>
        public ContentId ItemId { get; }
    }
}
