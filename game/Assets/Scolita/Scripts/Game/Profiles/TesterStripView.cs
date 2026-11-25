using UnityEngine;

namespace Scolita.Game.Profiles
{
    /// <summary>
    /// The tester's strip (PRD §3.8), on the map and in a lesson, only for the tester's profile: a "TEST" badge, a line of
    /// test information (the version, the lesson and step ids, the voice line being said) and, in a lesson, "Sari peste
    /// pas". Drawn by <c>Scolita.UI.Profiles.TesterStrip</c> on <c>Prefabs/Profiles/TesterStrip.prefab</c>; hidden for a
    /// child.
    /// </summary>
    public abstract class TesterStripView : MonoBehaviour
    {
        /// <summary>Sends "Sari peste pas" to <paramref name="input"/>; called once by the Lesson scene's composition root.</summary>
        public abstract void Bind(ITesterInput input);

        /// <summary>
        /// Shows the strip (<paramref name="shown"/>), with the skip button when <paramref name="canSkip"/>; hidden for a
        /// child. Called once by the Map and Lesson scenes' composition roots.
        /// </summary>
        public abstract void Show(bool shown, bool canSkip);

        /// <summary>Writes <paramref name="text"/> in the strip; called by the composition roots when what it says changes.</summary>
        public abstract void ShowInfo(string text);
    }
}
