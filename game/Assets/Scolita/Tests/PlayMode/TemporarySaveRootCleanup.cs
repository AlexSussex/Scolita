#if UNITY_EDITOR
using Scolita.Game.Saving;
using UnityEditor;

namespace Scolita.Tests.PlayMode
{
    /// <summary>
    /// Deletes the PlayMode run's own save root (<see cref="SaveRoot.StopUsingTemporaryFolder"/>) when the Editor is back
    /// in Edit mode after it, so the scenes' composition roots, which save as they close, have saved into it before it goes.
    /// Editor only; subscribed whenever the Editor loads the scripts.
    /// </summary>
    [InitializeOnLoad]
    public static class TemporarySaveRootCleanup
    {
        static TemporarySaveRootCleanup()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredEditMode && SaveRoot.IsUsingTemporaryFolder)
            {
                SaveRoot.StopUsingTemporaryFolder();
            }
        }
    }
}
#endif
