using NUnit.Framework;
using Scolita.Core.Audio;
using Scolita.Core.Languages;
using Scolita.Game.Audio;
using Scolita.Game.Settings;
using Scolita.Tests.EditMode.Fakes;
using UnityEditor;
using UnityEngine;

namespace Scolita.Tests.EditMode.Game
{
    /// <summary>Tests <see cref="SoundPlayer"/>: the volume routing to its sources, the ducking, the sliders at 0, the takes, the vibration switch.</summary>
    public sealed class SoundPlayerTests
    {
        private const float Tolerance = 0.0001f;

        /// <summary>Tests that <see cref="SoundPlayer.Play"/> sends an effect to an effect source at the sounds volume, never to the music's.</summary>
        [Test]
        public void TestPlay_AnEffectGoesToAnEffectSourceAtTheSoundsVolume()
        {
            SoundPlayer player = MakePlayer(out SoundBank bank);
            try
            {
                player.Begin(TestSoundRules.Default(), Settings(0.5f, 0.8f, true), MusicPiece.Lesson);

                player.Play(SoundCue.Sparkle);

                AudioSource effect = player.EffectSources[0];
                Assert.That(effect.clip, Is.SameAs(bank.Take(SoundCue.Sparkle, 0)));
                Assert.That(effect.volume, Is.EqualTo(0.8f).Within(Tolerance));
                Assert.That(player.MusicSource.clip, Is.SameAs(bank.LoopOf(MusicPiece.Lesson)));
                Assert.That(player.EffectsPlayed, Is.EqualTo(1));
                Assert.That(player.LastPlayed, Is.EqualTo(SoundCue.Sparkle));
            }
            finally
            {
                Destroy(player, bank);
            }
        }

        /// <summary>Tests that <see cref="SoundPlayer.Advance"/> fades the music in to the music volume and ducks it while the voice speaks; effects are lower then.</summary>
        [Test]
        public void TestAdvance_TheMusicFadesInAndDucksUnderTheVoice()
        {
            SoundPlayer player = MakePlayer(out SoundBank bank);
            FakeVoice voice = new FakeVoice();
            try
            {
                player.Begin(TestSoundRules.Default(), Settings(0.5f, 0.8f, true), MusicPiece.Space);
                player.FollowVoice(voice);
                float silentAtStart = player.MusicSource.volume;
                player.Advance(2f);
                float full = player.MusicSource.volume;

                voice.Speak(VoiceTestContent.Line("ro-line-hello"));
                player.Advance(1f);
                float ducked = player.MusicSource.volume;
                player.Play(SoundCue.Tap);
                float effectUnderVoice = player.EffectSources[0].volume;
                voice.FinishSpeaking();
                player.Advance(0.5f);
                player.Advance(1f);

                Assert.That(silentAtStart, Is.EqualTo(0f));
                Assert.That(full, Is.EqualTo(0.5f).Within(Tolerance));
                Assert.That(ducked, Is.EqualTo(0.5f * 0.25f).Within(Tolerance));
                Assert.That(effectUnderVoice, Is.EqualTo(0.8f * 0.5f).Within(Tolerance));
                Assert.That(player.MusicSource.volume, Is.EqualTo(0.5f).Within(Tolerance));
            }
            finally
            {
                Destroy(player, bank);
            }
        }

        /// <summary>Tests that a slider at 0 plays nothing (<see cref="SoundPlayer.Begin"/>, <see cref="SoundPlayer.Play"/>), and <see cref="SoundPlayer.UseSettings"/> starts the music when its slider leaves 0.</summary>
        [Test]
        public void TestPlay_SlidersAtZeroPlayNothing()
        {
            SoundPlayer player = MakePlayer(out SoundBank bank);
            try
            {
                player.Begin(TestSoundRules.Default(), Settings(0f, 0f, true), MusicPiece.Space);

                player.Play(SoundCue.Sparkle);
                player.PlayAcrossScenes(SoundCue.TakeOff);
                AudioClip musicBefore = player.MusicSource.clip;
                player.UseSettings(0.6f, 0f, true);

                Assert.That(player.EffectsPlayed, Is.EqualTo(0));
                Assert.That(player.EffectSources[0].clip, Is.Null);
                Assert.That(musicBefore, Is.Null);
                Assert.That(player.MusicSource.clip, Is.SameAs(bank.LoopOf(MusicPiece.Space)));
                Assert.That(player.Levels.MusicVolume, Is.EqualTo(0.6f).Within(Tolerance));
            }
            finally
            {
                Destroy(player, bank);
            }
        }

        /// <summary>Tests that <see cref="SoundPlayer.Play"/> plays a cue's takes in turn, on the effect sources in turn.</summary>
        [Test]
        public void TestPlay_TakesAndSourcesInTurn()
        {
            SoundPlayer player = MakePlayer(out SoundBank bank);
            try
            {
                player.Begin(TestSoundRules.Default(), Settings(1f, 1f, true), MusicPiece.None);

                player.Play(SoundCue.StarLands);
                player.Play(SoundCue.StarLands);
                player.Play(SoundCue.StarLands);

                Assert.That(player.EffectSources[0].clip, Is.SameAs(bank.Take(SoundCue.StarLands, 0)));
                Assert.That(player.EffectSources[1].clip, Is.SameAs(bank.Take(SoundCue.StarLands, 1)));
                Assert.That(player.EffectSources[0].clip, Is.Not.SameAs(player.EffectSources[1].clip));
                Assert.That(player.EffectsPlayed, Is.EqualTo(3));
                Assert.That(player.MusicSource.clip, Is.Null);
            }
            finally
            {
                Destroy(player, bank);
            }
        }

        /// <summary>Tests that <see cref="SoundPlayer.Vibrate"/> follows the vibration switch.</summary>
        [Test]
        public void TestVibrate_FollowsTheSwitch()
        {
            SoundPlayer player = MakePlayer(out SoundBank bank);
            try
            {
                player.Begin(TestSoundRules.Default(), Settings(1f, 1f, false), MusicPiece.None);
                player.Vibrate(Haptic.Tick);
                int whileOff = player.TicksGiven;

                player.UseSettings(1f, 1f, true);
                player.Vibrate(Haptic.Tick);
                player.Vibrate(Haptic.None);

                Assert.That(whileOff, Is.EqualTo(0));
                Assert.That(player.TicksGiven, Is.EqualTo(1));
            }
            finally
            {
                Destroy(player, bank);
            }
        }

        private static TabletSettingsData Settings(float music, float sounds, bool vibration)
        {
            return new TabletSettingsData(music, sounds, 1f, vibration, Language.Romanian);
        }

        private static SoundPlayer MakePlayer(out SoundBank bank)
        {
            bank = ScriptableObject.CreateInstance<SoundBank>();
            bank.Fill(new[]
            {
                new SoundBankEffect(SoundCue.Sparkle, new[] { Clip("sparkle") }),
                new SoundBankEffect(SoundCue.Tap, new[] { Clip("tap") }),
                new SoundBankEffect(SoundCue.TakeOff, new[] { Clip("take-off") }),
                new SoundBankEffect(SoundCue.StarLands, new[] { Clip("star-1"), Clip("star-2") })
            }, new[]
            {
                new SoundBankMusic(MusicPiece.Space, Clip("space")),
                new SoundBankMusic(MusicPiece.Lesson, Clip("lesson"))
            });
            GameObject root = new GameObject("SoundPlayerTest");
            AudioSource music = new GameObject("Music").AddComponent<AudioSource>();
            music.transform.SetParent(root.transform);
            AudioSource first = new GameObject("Effect1").AddComponent<AudioSource>();
            first.transform.SetParent(root.transform);
            AudioSource second = new GameObject("Effect2").AddComponent<AudioSource>();
            second.transform.SetParent(root.transform);
            SoundPlayer player = root.AddComponent<SoundPlayer>();
            SerializedObject serialized = new SerializedObject(player);
            serialized.FindProperty("bank").objectReferenceValue = bank;
            serialized.FindProperty("music").objectReferenceValue = music;
            SerializedProperty effects = serialized.FindProperty("effects");
            effects.arraySize = 2;
            effects.GetArrayElementAtIndex(0).objectReferenceValue = first;
            effects.GetArrayElementAtIndex(1).objectReferenceValue = second;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return player;
        }

        private static AudioClip Clip(string name)
        {
            return AudioClip.Create(name, 4410, 1, 44100, false);
        }

        private static void Destroy(SoundPlayer player, SoundBank bank)
        {
            foreach (SoundBankEffect effect in bank.Effects)
            {
                foreach (AudioClip clip in effect.Clips)
                {
                    Object.DestroyImmediate(clip);
                }
            }

            foreach (SoundBankMusic music in bank.Music)
            {
                Object.DestroyImmediate(music.Clip);
            }

            Object.DestroyImmediate(player.gameObject);
            Object.DestroyImmediate(bank);
        }
    }
}
