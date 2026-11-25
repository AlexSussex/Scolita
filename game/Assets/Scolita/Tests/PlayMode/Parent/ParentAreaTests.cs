using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Game.Lessons;
using Scolita.Game.Parent;
using Scolita.Game.Profiles;
using Scolita.Game.Progress;
using Scolita.Game.Rewards;
using Scolita.Game.Saving;
using Scolita.Game.Settings;
using Scolita.Tests.PlayMode.Map;
using Scolita.UI.Parent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Parent
{
    /// <summary>
    /// Tests the Parent scene (<see cref="ParentRunner"/>, <see cref="ParentAreaScreen"/>): the grid, the days and the
    /// next lessons; each setting saved and taking effect (the mix on the map, the daily limit, the voice, the app's
    /// language); reset asking twice and keeping the settings; Back to the map. Each test keeps the Editor's own files.
    /// </summary>
    public sealed class ParentAreaTests
    {
        /// <summary>Tests that the area shows the track's letters in both languages, her states, today played and the next lessons.</summary>
        [UnityTest]
        public IEnumerator TestParentArea_ShowsHerProgress()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                SaveStore store = SaveStore.ForProfile(Application.persistentDataPath, BuiltInProfiles.FirstChild);
                MapScene.SaveProgress("ro-a", "ro-a");
                LearnerProgress progress = new LearnerProgress(new MapProgress(new[] { ContentId.Create("ro-a") }, default),
                    null, null);
                progress.Meet(LearningItem.Letter(ContentId.Create("a"), Language.Romanian));
                progress.RecordDayPlayed(CalendarDay.FromDateTime(DateTime.Now));
                Assert.That(ProgressFile.Save(store, progress), Is.True);

                yield return ParentScene.Open();
                ParentAreaScreen screen = ParentScene.Screen();

                Assert.That(screen.Cells.Count, Is.EqualTo(ParentScene.Runner().Report.Rows.Sum(row => row.Cells.Count)));
                Assert.That(screen.Cells[0].State, Is.EqualTo(ItemState.Learning));
                Assert.That(screen.Cells.Skip(1).Select(cell => cell.State), Is.All.EqualTo(ItemState.New));
                Assert.That(screen.IsPlayedDay(screen.Days.Count - 1), Is.True);
                Assert.That(screen.IsPlayedDay(0), Is.False);
                Assert.That(screen.NextLessonsText, Does.StartWith("Planeta 1: litera a, în engleză"));
                Assert.That(screen.VersionText, Does.StartWith("Versiunea "));
                Assert.That(screen.MixPills[0].IsChosen, Is.True);
                Assert.That(screen.LimitPills.Select(pill => pill.Words), Is.EqualTo(new[] { "Fără", "10 min", "15 min", "20 min" }));
                Assert.That(screen.LimitPills[0].IsChosen, Is.True);

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that choosing English only is saved and hides the Romanian lessons on the map at once.</summary>
        [UnityTest]
        public IEnumerator TestMix_EnglishOnlyHidesTheRomanianLessons()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                yield return ParentScene.Open();

                ParentScene.Screen().MixPills[2].Button.onClick.Invoke();
                Assert.That(ParentScene.SavedLearning().LanguageMix, Is.EqualTo(LanguageMix.EnglishOnly));
                Assert.That(ParentScene.Screen().MixPills[2].IsChosen, Is.True);
                ParentScene.Screen().BackButton.onClick.Invoke();
                yield return MapScene.WaitForMap();

                Assert.That(MapScene.Runner().Route.Stops.Select(stop => stop.Lesson.Languages),
                    Has.None.EqualTo(LanguageMix.RomanianOnly));
                Assert.That(MapScene.Runner().Route.GoStop.Lesson.Id.ToString(), Is.EqualTo("en-a"));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that a daily limit pill is saved and drawn as chosen.</summary>
        [UnityTest]
        public IEnumerator TestLimit_ChosenIsSaved()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                yield return ParentScene.Open();

                ParentScene.Screen().LimitPills[2].Button.onClick.Invoke();

                Assert.That(ParentScene.SavedLearning().SessionLimitBy(ParentScene.Rules).Minutes, Is.EqualTo(15));
                Assert.That(ParentScene.Screen().LimitPills[2].IsChosen, Is.True);
                Assert.That(ParentScene.Screen().LimitPills[0].IsChosen, Is.False);

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that the voice slider's value is saved when the finger leaves it.</summary>
        [UnityTest]
        public IEnumerator TestVoice_SavedOnRelease()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                ParentScene.SaveAppLanguage(Language.Romanian);
                yield return ParentScene.Open();
                ParentAreaScreen screen = ParentScene.Screen();

                screen.VoiceSlider.value = 0.3f;
                screen.VoiceSlider.GetComponent<SliderRelease>().OnPointerUp(new PointerEventData(EventSystem.current));

                Assert.That(ParentScene.SavedTablet().VoiceVolume, Is.EqualTo(0.3f).Within(0.001f));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that "Meniurile în engleză" saves English, switches the app's words at once, and the map says "Go!".</summary>
        [UnityTest]
        public IEnumerator TestMenusInEnglish_SwitchesTheAppsWords()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                ParentScene.SaveAppLanguage(Language.Romanian);
                yield return ParentScene.Open();

                ParentScene.Screen().MenusInEnglish.isOn = true;
                yield return null;

                Assert.That(ParentScene.SavedTablet().AppLanguage, Is.EqualTo(Language.English));
                Assert.That(LocalizationSettings.SelectedLocale.Identifier.Code, Is.EqualTo("en-GB"));
                Assert.That(ParentScene.Screen().VersionText, Does.StartWith("Version "));
                ParentScene.Screen().BackButton.onClick.Invoke();
                yield return MapScene.WaitForMap();
                yield return MapScene.WaitFor(() => MapScene.Screen().GoWord == "Go!", 3f);

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
                AppLocale.Select(Language.Romanian);
            }
        }

        /// <summary>Tests that reset asks twice, then deletes her progress files together and keeps the settings.</summary>
        [UnityTest]
        public IEnumerator TestReset_AsksTwiceThenDeletesTheProgress()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress("ro-m", "ro-a", "en-a");
                ParentScene.SaveLimit(20);
                ProfileStore().Save(LessonRunData.FileName, LessonRunData.CreateDefaults());
                yield return ParentScene.Open();
                ParentAreaScreen screen = ParentScene.Screen();

                screen.ResetButton.onClick.Invoke();
                bool firstAsked = screen.IsAskingReset;
                string firstWords = screen.QuestionText;
                screen.YesButton.onClick.Invoke();
                bool secondAsked = screen.IsAskingReset;
                bool stillThere = MapScene.SavedProgress().DoneLessons.Count == 2;
                screen.YesButton.onClick.Invoke();
                yield return null;

                Assert.That(firstAsked, Is.True);
                Assert.That(firstWords, Does.Contain("Setările rămân"));
                Assert.That(secondAsked, Is.True);
                Assert.That(stillThere, Is.True, "nothing is deleted before the second yes");
                Assert.That(screen.IsAskingReset, Is.False);
                Assert.That(screen.IsResetDoneShown, Is.True);
                Assert.That(MapScene.SavedProgress().DoneLessons, Is.Empty, "progress.json is made again, empty");
                Assert.That(ParentScene.ProfileFileExists(LessonRunData.FileName), Is.False);
                Assert.That(ParentScene.ProfileFileExists(RewardsData.FileName), Is.False);
                Assert.That(ParentScene.SavedLearning().SessionLimitBy(ParentScene.Rules).Minutes, Is.EqualTo(20));
                Assert.That(ParentScene.Runner().Report.NextLessons[0].Lesson.Id.ToString(), Is.EqualTo("ro-a"));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that "no" (and Back) at a reset question keeps everything.</summary>
        [UnityTest]
        public IEnumerator TestReset_NoKeepsEverything()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress("ro-m", "ro-a");
                yield return ParentScene.Open();
                ParentAreaScreen screen = ParentScene.Screen();

                screen.ResetButton.onClick.Invoke();
                screen.NoButton.onClick.Invoke();
                bool closedByNo = !screen.IsAskingReset;
                screen.ResetButton.onClick.Invoke();
                screen.YesButton.onClick.Invoke();
                ParentScene.Runner().Back();
                yield return null;

                Assert.That(closedByNo, Is.True);
                Assert.That(screen.IsAskingReset, Is.False);
                Assert.That(ParentScene.Runner(), Is.Not.Null, "Back at a question stays in the area");
                Assert.That(ParentScene.ProfileFileExists(ProgressData.FileName), Is.True);

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        private static SaveStore ProfileStore()
        {
            return SaveStore.ForProfile(Application.persistentDataPath, BuiltInProfiles.FirstChild);
        }
    }
}
