using UnityEngine.SceneManagement;

namespace Scolita.Game.Scenes
{
    /// <summary>
    /// Moves between the app's scenes by their <see cref="AppScene"/> name, so no scene name is
    /// written as text anywhere else.
    /// </summary>
    public static class SceneNavigator
    {
        /// <summary>
        /// Replaces the open scene with <paramref name="scene"/>, from the next frame on.
        /// Called by <see cref="Boot.BootLoader"/>, the Map's Go button and the Lesson's home button.
        /// </summary>
        public static void Load(AppScene scene)
        {
            SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
        }
    }
}
