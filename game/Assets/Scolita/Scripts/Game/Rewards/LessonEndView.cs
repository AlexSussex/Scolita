using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// The end of a lesson (design screen 6) as the lesson screen sees it: "Bravo!", the constellation and its flying
    /// stars, the sticker's gift box, "Mai departe" and "În cartea mea". Drawn by <c>Scolita.UI.Rewards.LessonEndScreen</c>
    /// (<c>Prefabs/Rewards/LessonEnd.prefab</c>).
    /// </summary>
    public abstract class LessonEndView : MonoBehaviour
    {
        /// <summary>Whether it is shown.</summary>
        public abstract bool IsShown { get; }

        /// <summary>
        /// Shows <paramref name="end"/>; "În cartea mea" (shown when a sticker was earned) goes to
        /// <paramref name="input"/>. Called by the lesson screen.
        /// </summary>
        public abstract void Show(LessonEnd end, IRewardsInput input);

        /// <summary>Hides it; called by the lesson screen when it opens.</summary>
        public abstract void Hide();
    }
}
