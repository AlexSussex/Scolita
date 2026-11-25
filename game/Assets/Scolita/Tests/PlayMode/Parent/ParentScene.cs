using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Core.Timing;
using Scolita.Game.Parent;
using Scolita.Game.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Tests.PlayMode.Map;
using Scolita.UI.Parent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Scolita.Tests.PlayMode.Parent
{
    /// <summary>
    /// Helpers the parent PlayMode tests share: keeping and restoring the Editor's own files (the profile's and the
    /// tablet's settings), saving a daily limit and today's play, opening the map's gate and the Parent scene, and
    /// tapping like a finger. Used by <see cref="ParentGateTests"/>, <see cref="ParentAreaTests"/> and
    /// <see cref="SessionLimitTests"/>.
    /// </summary>
    public static class ParentScene
    {
        /// <summary>The numbers of the learning rules asset, for the settings the tests save.</summary>
        public static readonly ParentRules Rules = new ParentRules(3f, 11, 19, 3, 30f, new[] { 10, 15, 20 });

        /// <summary>The Editor's own files the parent tests touch, as they are now; used by the parent tests.</summary>
        public static KeptFiles Keep()
        {
            string tablet = File.Exists(TabletFile()) ? File.ReadAllText(TabletFile()) : null;
            return new KeptFiles(MapScene.KeepFiles(), tablet);
        }

        /// <summary>Puts back the files <see cref="Keep"/> kept; used by the parent tests.</summary>
        public static void Restore(KeptFiles kept)
        {
            MapScene.RestoreFiles(kept.Profile);
            if (kept.Tablet == null)
            {
                File.Delete(TabletFile());
                return;
            }

            File.WriteAllText(TabletFile(), kept.Tablet);
        }

        /// <summary>
        /// Leaves the open scene, so its composition root saves its files (her play time) now: before a test saves its own
        /// files (the scene a previous test left), and at a test's end, before the Editor's own files are put back. Used by
        /// the parent tests.
        /// </summary>
        public static IEnumerator LeaveTheOpenScene()
        {
            Scene left = SceneManager.GetActiveScene();
            Scene empty = SceneManager.CreateScene("ParentTestEmpty" + Time.frameCount);
            SceneManager.SetActiveScene(empty);
            yield return SceneManager.UnloadSceneAsync(left);
        }

        /// <summary>Saves both languages with a daily limit of <paramref name="minutes"/> (0: none); used by the parent tests.</summary>
        public static void SaveLimit(int minutes)
        {
            Assert.That(Store().Save(LearningSettingsData.FileName,
                new LearningSettingsData(LanguageMix.Both, SessionLimit.Of(minutes, Rules))), Is.True);
        }

        /// <summary>Saves <paramref name="played"/> as today's play; used by the parent tests.</summary>
        public static void SavePlayedToday(TimeSpan played)
        {
            Assert.That(Store().Save(PlayTimeData.FileName,
                new PlayTimeData(new DailyPlay(CalendarDay.FromDateTime(DateTime.Now), played))), Is.True);
        }

        /// <summary>Today's play as saved now; used by the parent tests.</summary>
        public static TimeSpan SavedPlayToday()
        {
            return Store().Load(PlayTimeData.FileName, PlayTimeData.CreateDefaults()).Data.Play
                .PlayedOn(CalendarDay.FromDateTime(DateTime.Now));
        }

        /// <summary>The profile's learning settings as saved now; used by the parent tests.</summary>
        public static LearningSettingsData SavedLearning()
        {
            return Store().Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data;
        }

        /// <summary>The tablet's settings as saved now; used by the parent tests.</summary>
        public static TabletSettingsData SavedTablet()
        {
            return SaveStore.ForTablet(Application.persistentDataPath)
                .Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults()).Data;
        }

        /// <summary>Saves the tablet's settings with the app's words in <paramref name="language"/>; used by the parent tests.</summary>
        public static void SaveAppLanguage(Language language)
        {
            Assert.That(SaveStore.ForTablet(Application.persistentDataPath).Save(TabletSettingsData.FileName,
                new TabletSettingsData(true, true, 1f, true, language)), Is.True);
        }

        /// <summary>Whether the profile file <paramref name="name"/> exists; used by the parent tests.</summary>
        public static bool ProfileFileExists(string name)
        {
            return File.Exists(Path.Combine(Store().Folder, name + ".json"));
        }

        /// <summary>The map's gate panel; used by the parent tests.</summary>
        public static ParentGatePanel GatePanel()
        {
            return SceneSearch.FindInActiveScene<ParentGatePanel>();
        }

        /// <summary>
        /// Makes the map's gate ask at once (the hold's frames given in a row, as <c>MapRunner</c> would over three
        /// seconds); used by the parent tests.
        /// </summary>
        public static void AskAtOnce()
        {
            ParentGateFlow gate = MapScene.Runner().Gate;
            gate.Press();
            for (int frame = 0; frame < 40 && !gate.IsAsking; frame++)
            {
                gate.Tick(0.1f);
            }

            Assert.That(gate.IsAsking, Is.True);
        }

        /// <summary>The place of the number said among the panel's words; used by the parent tests.</summary>
        public static int RightPlace()
        {
            string word = AppLocale.Text(ParentGateFlow.WordKeyOf(MapScene.Runner().Gate.AskedNumber));
            ParentGatePanel panel = GatePanel();
            return Enumerable.Range(0, panel.ChoiceButtons.Count).Single(index => panel.ChoiceWord(index) == word);
        }

        /// <summary>Opens the Parent scene and waits for its report; used by the parent tests.</summary>
        public static IEnumerator Open()
        {
            SceneManager.LoadScene(AppScene.Parent.ToString());
            yield return WaitForArea();
        }

        /// <summary>Waits for the Parent scene and its report (after the gate); used by the parent tests.</summary>
        public static IEnumerator WaitForArea()
        {
            yield return SceneSearch.WaitForScene(AppScene.Parent);
            yield return MapScene.WaitFor(() => Runner() != null && Runner().Report != null);
        }

        /// <summary>The Parent scene's composition root; used by the parent tests.</summary>
        public static ParentRunner Runner()
        {
            return SceneSearch.FindInActiveScene<ParentRunner>();
        }

        /// <summary>The parent area screen; used by the parent tests.</summary>
        public static ParentAreaScreen Screen()
        {
            return SceneSearch.FindInActiveScene<ParentAreaScreen>();
        }

        /// <summary>
        /// Presses a finger at <paramref name="position"/> (screen pixels) on whatever is there, holds it for
        /// <paramref name="frames"/> frames, and lifts it (a click when it lifts where it pressed), as the EventSystem would.
        /// Used by the monkey test.
        /// </summary>
        public static IEnumerator Tap(Vector2 position, int frames)
        {
            PointerEventData data = new PointerEventData(EventSystem.current) { position = position };
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, hits);
            if (hits.Count == 0)
            {
                yield break;
            }

            GameObject target = hits[0].gameObject;
            data.pointerCurrentRaycast = hits[0];
            data.pointerPressRaycast = hits[0];
            GameObject pressed = ExecuteEvents.ExecuteHierarchy(target, data, ExecuteEvents.pointerDownHandler);
            for (int frame = 0; frame < frames; frame++)
            {
                yield return null;
            }

            if (pressed != null)
            {
                ExecuteEvents.Execute(pressed, data, ExecuteEvents.pointerUpHandler);
            }

            if (target != null)
            {
                ExecuteEvents.ExecuteHierarchy(target, data, ExecuteEvents.pointerClickHandler);
            }
        }

        private static SaveStore Store()
        {
            return SaveStore.ForProfile(Application.persistentDataPath, BuiltInProfiles.FirstChild);
        }

        private static string TabletFile()
        {
            return Path.Combine(Application.persistentDataPath, TabletSettingsData.FileName + ".json");
        }
    }
}
