using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The root component of an activity kind's view prefab: an <see cref="IActivityView"/> that
    /// Unity can serialize (it can't serialize an interface), listed in <see cref="ActivityRegistry"/>.
    /// The views themselves are in <c>Scolita.UI.Lesson</c>.
    /// </summary>
    public abstract class ActivityView : MonoBehaviour, IActivityView
    {
        /// <inheritdoc/>
        public abstract void Show(ActivityRound round, ActivityViewContext context);

        /// <inheritdoc/>
        public abstract void ShowFeedback(in StepFeedback feedback);

        /// <inheritdoc/>
        public abstract void Hide();
    }
}
