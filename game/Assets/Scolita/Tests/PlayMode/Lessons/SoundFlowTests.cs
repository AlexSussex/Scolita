using System.Collections;
using System.IO;
using NUnit.Framework;
using Scolita.Core.Audio;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Game.Audio;
using Scolita.Game.Lessons;
using Scolita.Game.Progress;
using Scolita.Game.Saving;
using Scolita.Game.Settings;
using Scolita.Game.Voice;
using Scolita.UI.Lesson;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Lessons
{
    /// <summary>
    /// Tests the Lesson scene's sound (<see cref="SoundPlayer"/> wired by <see cref="LessonRunner"/>) on the real <c>ro-t</c>
    /// lesson: with the music and the sounds at 0 the lesson runs to its end and nothing plays; with them on the lesson's
    /// music plays, ducks while the voice speaks and comes back, and a right answer sparkles and ticks.
    /// </summary>
    public sealed class SoundFlowTests
    {
        private const float Tolerance = 0.02f;

        /// <summary>Tests that with sound off a lesson runs to its end, and no music, no effect and no vibration is played.</summary>
        [UnityTest]
        public IEnumerator TestLessonRunner_WithSoundOffRunsToItsEndSilently()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptProgress = LessonScene.KeepFile(ProgressData.FileName);
            try
            {
                SaveTablet(new TabletSettingsData(0f, 0f, 1f, false, Language.Romanian));
                LessonScene.RestoreFile(ProgressData.FileName, null);
                LessonScene.SaveStartingFiles("ro-t");
                yield return LessonScene.Open();
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                SoundPlayer sounds = SceneSearch.FindInActiveScene<SoundPlayer>();
                LessonScreen screen = SceneSearch.FindInActiveScene<LessonScreen>();

                yield return LessonScene.FinishLesson(runner);
                yield return new WaitForSecondsRealtime(3f);

                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Finished));
                Assert.That(screen.IsEndShown, Is.True);
                Assert.That(sounds.IsBegun, Is.True);
                Assert.That(sounds.EffectsPlayed, Is.EqualTo(0));
                Assert.That(sounds.TicksGiven, Is.EqualTo(0));
                Assert.That(sounds.MusicSource.clip, Is.Null);
                Assert.That(sounds.MusicSource.isPlaying, Is.False);
                foreach (AudioSource effect in sounds.EffectSources)
                {
                    Assert.That(effect.clip, Is.Null);
                }
            }
            finally
            {
                DeleteTablet();
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(ProgressData.FileName, keptProgress);
            }
        }

        /// <summary>
        /// Tests that with sound on the lesson's music plays at its volume, goes down to the ducked level while the voice speaks
        /// and comes back after, and that a right answer plays its sound and gives a tick.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLessonRunner_WithSoundOnTheMusicDucksUnderTheVoice()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptProgress = LessonScene.KeepFile(ProgressData.FileName);
            try
            {
                SaveTablet(new TabletSettingsData(1f, 1f, 1f, true, Language.Romanian));
                LessonScene.RestoreFile(ProgressData.FileName, null);
                LessonScene.SaveStartingFiles("ro-t");
                yield return LessonScene.Open();
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                SoundPlayer sounds = SceneSearch.FindInActiveScene<SoundPlayer>();
                VoiceHost voice = SceneSearch.FindInActiveScene<VoiceHost>();
                SoundRules rules = Resources.FindObjectsOfTypeAll<LearningRulesAsset>()[0].ToSoundRules();
                Assert.That(sounds.MusicSource.clip, Is.Not.Null);
                Assert.That(sounds.MusicSource.clip.name, Is.EqualTo("lesson"));

                yield return new WaitForSecondsRealtime(rules.MusicFadeInSeconds + 0.2f);
                voice.Voice.SpeakSequence(runner.Run.CurrentInstruction);
                yield return new WaitForSecondsRealtime(rules.DuckSeconds + 0.3f);
                float ducked = sounds.MusicSource.volume;
                voice.Voice.Stop();
                yield return new WaitForSecondsRealtime(rules.HoldSeconds + rules.RecoverSeconds + 0.3f);
                float back = sounds.MusicSource.volume;
                LessonScene.AnswerRight(runner);

                Assert.That(ducked, Is.EqualTo(rules.DuckedLevel).Within(Tolerance));
                Assert.That(back, Is.EqualTo(1f).Within(Tolerance));
                Assert.That(sounds.EffectsPlayed, Is.GreaterThanOrEqualTo(1));
                Assert.That(sounds.LastPlayed, Is.EqualTo(SoundCue.Sparkle).Or.EqualTo(SoundCue.StepDone));
                Assert.That(sounds.TicksGiven, Is.GreaterThanOrEqualTo(1));
            }
            finally
            {
                DeleteTablet();
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(ProgressData.FileName, keptProgress);
            }
        }

        /// <summary>Tests that the PlayMode tests save into a temporary root of their own (<see cref="SaveRoot.Path"/>), never the Editor's own.</summary>
        [Test]
        public void TestSaveRoot_TheTestsHaveTheirOwn()
        {
            string root = SaveRoot.Path;

            Assert.That(root, Is.Not.EqualTo(Application.persistentDataPath));
            Assert.That(Path.GetFullPath(root), Does.StartWith(Path.GetFullPath(Path.GetTempPath())));
            Assert.That(Path.GetFileName(root), Does.StartWith("scolita-saves-"));
        }

        private static void SaveTablet(TabletSettingsData settings)
        {
            Assert.That(SaveStore.ForTablet(SaveRoot.Path).Save(TabletSettingsData.FileName, settings), Is.True);
        }

        private static void DeleteTablet()
        {
            Assert.That(SaveStore.ForTablet(SaveRoot.Path).Delete(TabletSettingsData.FileName), Is.True);
        }
    }
}
