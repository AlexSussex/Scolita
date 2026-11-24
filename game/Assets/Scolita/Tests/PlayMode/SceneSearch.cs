using System.Collections;
using Scolita.Game.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scolita.Tests.PlayMode
{
    /// <summary>Helpers the PlayMode tests share: waiting for a scene and finding a component in it.</summary>
    public static class SceneSearch
    {
        /// <summary>
        /// Waits a frame (a scene asked for loads on the next one), then at most 300 frames until
        /// <paramref name="scene"/> is the active scene; used by the PlayMode tests.
        /// </summary>
        public static IEnumerator WaitForScene(AppScene scene)
        {
            const int maxFrames = 300;
            string sceneName = scene.ToString();
            yield return null;
            for (int frame = 0; frame < maxFrames && SceneManager.GetActiveScene().name != sceneName; frame++)
            {
                yield return null;
            }
        }

        /// <summary>The first <typeparamref name="T"/> under the active scene's roots, or null; used by the PlayMode tests.</summary>
        public static T FindInActiveScene<T>() where T : Component
        {
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                T found = root.GetComponentInChildren<T>(true);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
