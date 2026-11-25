using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Map;
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Tests.PlayMode.Lessons;
using Scolita.UI.Map;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scolita.Tests.PlayMode.Map
{
    /// <summary>
    /// Helpers the map PlayMode tests share: keeping and restoring the Editor's own files the map reads and
    /// writes, saving her map progress, opening the Map scene and waiting. Used by <see cref="MapFlowTests"/>
    /// and <c>SceneFlowTests</c>.
    /// </summary>
    public static class MapScene
    {
        private static readonly string[] ProfileFiles =
        {
            MapProgressData.FileName, LessonRunData.FileName, LearningSettingsData.FileName
        };

        /// <summary>The Editor's own profile files the map touches, as they are now (null for a missing one); used by the map tests.</summary>
        public static string[] KeepFiles()
        {
            return ProfileFiles.Select(LessonScene.KeepFile).ToArray();
        }

        /// <summary>Puts back the files <see cref="KeepFiles"/> kept; used by the map tests.</summary>
        public static void RestoreFiles(string[] kept)
        {
            for (int index = 0; index < ProfileFiles.Length; index++)
            {
                LessonScene.RestoreFile(ProfileFiles[index], kept[index]);
            }
        }

        /// <summary>
        /// Saves both languages, no lesson in progress, and her map progress: <paramref name="doneLessons"/> done
        /// and the rocket last shown at <paramref name="rocketStop"/> (null for never); used by the map tests.
        /// </summary>
        public static void SaveProgress(string rocketStop, params string[] doneLessons)
        {
            SaveStore store = Store();
            Assert.That(store.Save(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()), Is.True);
            File.Delete(Path.Combine(store.Folder, LessonRunData.FileName + ".json"));
            MapProgress progress = new MapProgress(doneLessons.Select(ContentId.Create),
                rocketStop == null ? default : ContentId.Create(rocketStop));
            Assert.That(MapProgressFile.Save(store, progress), Is.True);
        }

        /// <summary>Her map progress as saved now; used by the map tests.</summary>
        public static MapProgress SavedProgress()
        {
            return Store().Load(MapProgressData.FileName, MapProgressData.CreateDefaults()).Data.ToProgress();
        }

        /// <summary>Opens the Map scene and waits for its route; used by the map tests.</summary>
        public static IEnumerator Open()
        {
            SceneManager.LoadScene(AppScene.Map.ToString());
            yield return WaitForMap();
        }

        /// <summary>Waits for the Map scene and its route (after a lesson's "Mai departe"); used by the map tests.</summary>
        public static IEnumerator WaitForMap()
        {
            yield return SceneSearch.WaitForScene(AppScene.Map);
            yield return WaitFor(() => Runner() != null && Runner().Route != null);
        }

        /// <summary>The Map scene's composition root; used by the map tests.</summary>
        public static MapRunner Runner()
        {
            return SceneSearch.FindInActiveScene<MapRunner>();
        }

        /// <summary>The map screen; used by the map tests.</summary>
        public static MapScreen Screen()
        {
            return SceneSearch.FindInActiveScene<MapScreen>();
        }

        /// <summary>
        /// Waits at most <paramref name="maxSeconds"/> of real time for <paramref name="condition"/> (the trip is
        /// about real time) and fails the test if it never holds; used by the map tests.
        /// </summary>
        public static IEnumerator WaitFor(Func<bool> condition, float maxSeconds = 10f)
        {
            float until = Time.realtimeSinceStartup + maxSeconds;
            while (!condition() && Time.realtimeSinceStartup < until)
            {
                yield return null;
            }

            Assert.That(condition(), Is.True, "waited " + maxSeconds + " seconds");
        }

        private static SaveStore Store()
        {
            return SaveStore.ForProfile(Application.persistentDataPath, BuiltInProfiles.FirstChild);
        }
    }
}
