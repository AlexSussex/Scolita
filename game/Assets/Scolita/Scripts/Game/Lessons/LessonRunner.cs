using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The Lesson scene's composition root: it will build a lesson's run from the chosen lesson and
    /// the profile (<c>lesson-engine</c>). For now it keeps the screen awake while the Lesson scene
    /// is open, and only then, so the tablet can sleep on the map when she walks away.
    /// </summary>
    public sealed class LessonRunner : MonoBehaviour
    {
        private void OnEnable()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void OnDisable()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}
