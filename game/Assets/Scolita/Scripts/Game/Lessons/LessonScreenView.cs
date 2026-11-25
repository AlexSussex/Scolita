using Scolita.Game.Rewards;
using Scolita.Game.Voice;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The lesson screen around the activity (the step dots, the mascot and its bubble, the end of the
    /// lesson), as <see cref="LessonRunner"/> sees it: drawn by <c>Scolita.UI.Lesson.LessonScreen</c>
    /// on <c>Prefabs/Lesson/LessonScreen.prefab</c>.
    /// </summary>
    public abstract class LessonScreenView : MonoBehaviour
    {
        /// <summary>Where the activity views are placed.</summary>
        public abstract RectTransform ActivityArea { get; }

        /// <summary>The mascot's bubble, which shows every line said, in its words.</summary>
        public abstract SpeechBubbleView Bubble { get; }

        /// <summary>The reward screens over the lesson (its sticker book, opened from the end).</summary>
        public abstract RewardsView Rewards { get; }

        /// <summary>Sends the screen's taps to <paramref name="input"/>; called once by <see cref="LessonRunner"/>.</summary>
        public abstract void Bind(ILessonScreenInput input);

        /// <summary>
        /// Shows the step dots: <paramref name="current"/> of <paramref name="count"/> (from 0; the steps
        /// before it done). Called by <see cref="LessonRunner"/> when a step begins and at the end.
        /// </summary>
        public abstract void ShowProgress(int current, int count);

        /// <summary>Shows the mascot in <paramref name="mood"/>; called by <see cref="LessonRunner"/>.</summary>
        public abstract void ShowMood(GuideMood mood);

        /// <summary>Moves the mascot's mouth while it speaks; called every frame by <see cref="LessonRunner"/>.</summary>
        public abstract void SetTalking(bool talking);

        /// <summary>
        /// Shows the end of the lesson (design screen 6) with <paramref name="end"/>'s stars, sticker and trail, its
        /// "Mai departe", and "În cartea mea" going to <paramref name="rewardsInput"/>; called by <see cref="LessonRunner"/>.
        /// </summary>
        public abstract void ShowEnd(LessonEnd end, IRewardsInput rewardsInput);
    }
}
