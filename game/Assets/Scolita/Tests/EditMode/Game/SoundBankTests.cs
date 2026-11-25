using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Audio;
using Scolita.Editor.Sounds;
using Scolita.Editor.Voice;
using Scolita.Game.Audio;
using UnityEditor;
using UnityEngine;

namespace Scolita.Tests.EditMode.Game
{
    /// <summary>
    /// Tests the app's own sounds: <see cref="SoundBank"/> as <see cref="SoundMaker"/> filled it (every cue and piece has its
    /// clips), their import (effects decompressed on load, music streamed), their levels beside the voice
    /// (<see cref="SoundStandard"/>), and that making them again gives the same files.
    /// </summary>
    public sealed class SoundBankTests
    {
        private const float SampleTolerance = 1.5f / 32768f;

        /// <summary>Tests that <see cref="SoundBank.Take"/> and <see cref="SoundBank.LoopOf"/> have a clip for every cue and every piece.</summary>
        [Test]
        public void TestTake_EveryCueAndPieceHasItsClips()
        {
            SoundBank bank = AssetDatabase.LoadAssetAtPath<SoundBank>(SoundMaker.BankPath);

            Assert.That(bank, Is.Not.Null, SoundMaker.BankPath);
            foreach (SoundCue cue in (SoundCue[])Enum.GetValues(typeof(SoundCue)))
            {
                Assert.That(bank.Take(cue, 0), Is.Not.Null, cue.ToString());
                Assert.That(bank.Effects.Single(effect => effect.Cue == cue).Clips.Count, Is.EqualTo(EffectRecipes.For(cue).Length),
                    cue.ToString());
            }

            foreach (MusicPiece piece in ((MusicPiece[])Enum.GetValues(typeof(MusicPiece))).Where(piece => piece != MusicPiece.None))
            {
                Assert.That(bank.LoopOf(piece), Is.Not.Null, piece.ToString());
            }

            Assert.That(bank.LoopOf(MusicPiece.None), Is.Null);
        }

        /// <summary>Tests that the effects are decompressed on load and preloaded, and the music streamed (AGENTS.md → "Performance").</summary>
        [Test]
        public void TestMake_EffectsDecompressedMusicStreamed()
        {
            SoundBank bank = AssetDatabase.LoadAssetAtPath<SoundBank>(SoundMaker.BankPath);

            foreach (AudioClip clip in bank.Effects.SelectMany(effect => effect.Clips))
            {
                AudioImporterSampleSettings settings = ImporterOf(clip).defaultSampleSettings;
                Assert.That(settings.loadType, Is.EqualTo(AudioClipLoadType.DecompressOnLoad), clip.name);
                Assert.That(settings.preloadAudioData, Is.True, clip.name);
                Assert.That(settings.compressionFormat, Is.EqualTo(AudioCompressionFormat.Vorbis), clip.name);
            }

            foreach (SoundBankMusic music in bank.Music)
            {
                AudioImporter importer = ImporterOf(music.Clip);
                Assert.That(importer.defaultSampleSettings.loadType, Is.EqualTo(AudioClipLoadType.Streaming), music.Clip.name);
                Assert.That(importer.defaultSampleSettings.preloadAudioData, Is.False, music.Clip.name);
                Assert.That(importer.forceToMono, Is.True, music.Clip.name);
            }
        }

        /// <summary>
        /// Tests that every effect and loop is at its loudness (within <see cref="SoundStandard.BelowTargetAllowed"/> under it when
        /// its peak was turned down) with its peak under its highest, so the voice (−16 LUFS) stands above them.
        /// </summary>
        [Test]
        public void TestLevelled_EveryFileAtItsTarget()
        {
            foreach (SoundCue cue in (SoundCue[])Enum.GetValues(typeof(SoundCue)))
            {
                foreach (MadeSound sound in EffectRecipes.For(cue))
                {
                    AssertLevel(SoundMaker.EffectsFolder, sound.Name, sound.TargetLufs, sound.HighestPeakDbfs);
                }
            }

            AssertLevel(SoundMaker.MusicFolder, "space", SoundStandard.MusicLufs, SoundStandard.MusicPeakDbfs);
            AssertLevel(SoundMaker.MusicFolder, "lesson", SoundStandard.MusicLufs, SoundStandard.MusicPeakDbfs);
            Assert.That(SoundStandard.EffectLufs, Is.LessThan(-16.0 - 3.0));
            Assert.That(SoundStandard.MusicLufs, Is.LessThan(-16.0 - 10.0));
        }

        /// <summary>Tests that <see cref="SoundMaker.Levelled"/> of each recipe gives the committed files again (the sounds can be remade).</summary>
        [Test]
        public void TestMake_TheFilesAreFresh()
        {
            foreach (SoundCue cue in (SoundCue[])Enum.GetValues(typeof(SoundCue)))
            {
                foreach (MadeSound sound in EffectRecipes.For(cue))
                {
                    AssertFresh(SoundMaker.EffectsFolder, sound);
                }
            }

            foreach (MusicPiece piece in ((MusicPiece[])Enum.GetValues(typeof(MusicPiece))).Where(piece => piece != MusicPiece.None))
            {
                AssertFresh(SoundMaker.MusicFolder, MusicRecipes.For(piece));
            }
        }

        private static void AssertLevel(string folder, string name, double targetLufs, double highestPeakDbfs)
        {
            MonoAudio audio = WavFile.Read(Path.GetFullPath(folder + "/" + name + ".wav"));
            double lufs = Loudness.IntegratedLufs(audio);
            Assert.That(audio.SampleRate, Is.EqualTo(Synth.Rate), name);
            Assert.That(lufs, Is.LessThanOrEqualTo(targetLufs + SoundStandard.AboveTargetAllowed), name);
            Assert.That(lufs, Is.GreaterThanOrEqualTo(targetLufs - SoundStandard.BelowTargetAllowed), name);
            Assert.That(Loudness.PeakDbfs(audio), Is.LessThanOrEqualTo(highestPeakDbfs + 0.1), name);
        }

        private static void AssertFresh(string folder, MadeSound sound)
        {
            float[] made = SoundMaker.Levelled(sound.Samples, sound.TargetLufs, sound.HighestPeakDbfs);
            float[] saved = WavFile.Read(Path.GetFullPath(folder + "/" + sound.Name + ".wav")).Samples;
            Assert.That(saved.Length, Is.EqualTo(made.Length), sound.Name);
            for (int index = 0; index < made.Length; index++)
            {
                if (Math.Abs(saved[index] - made[index]) > SampleTolerance)
                {
                    Assert.Fail(sound.Name + " differs from its recipe at sample " + index + ": run Școlița → Make sounds.");
                }
            }
        }

        private static AudioImporter ImporterOf(AudioClip clip)
        {
            return (AudioImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip));
        }
    }
}
