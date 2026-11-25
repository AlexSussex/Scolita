using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scolita.Core.Audio;
using Scolita.Editor.Voice;
using Scolita.Game.Audio;
using UnityEditor;
using UnityEngine;

namespace Scolita.Editor.Sounds
{
    /// <summary>
    /// Școlița → Make sounds: makes every effect (<see cref="EffectRecipes"/>) and both music loops (<see cref="MusicRecipes"/>)
    /// for the app, brings each to its loudness (<see cref="SoundStandard"/>, measured as the recording kit measures the
    /// voice, <see cref="Loudness"/>), writes them as mono 44.1 kHz WAVs to <see cref="EffectsFolder"/> and
    /// <see cref="MusicFolder"/>, imports them (effects decompressed on load, music streamed; AGENTS.md → "Performance")
    /// and fills <see cref="BankPath"/>. The sounds are made from fixed seeds, so making them again gives the same files.
    /// </summary>
    public static class SoundMaker
    {
        /// <summary>The folder of the effects' WAVs.</summary>
        public const string EffectsFolder = "Assets/Scolita/Audio/Effects";

        /// <summary>The folder of the music's WAVs.</summary>
        public const string MusicFolder = "Assets/Scolita/Audio/Music";

        /// <summary>The sound bank the scenes' sound players read.</summary>
        public const string BankPath = "Assets/Scolita/Audio/SoundBank.asset";

        /// <summary>The Vorbis quality of an effect.</summary>
        public const float EffectQuality = 0.6f;

        /// <summary>The Vorbis quality of the music.</summary>
        public const float MusicQuality = 0.5f;

        /// <summary>Makes every sound and the bank; the menu item, and the command line through <see cref="MakeFromCommandLine"/>.</summary>
        [MenuItem("Școlița/Make sounds")]
        public static void Make()
        {
            Directory.CreateDirectory(EffectsFolder);
            Directory.CreateDirectory(MusicFolder);
            List<SoundBankEffect> effects = new List<SoundBankEffect>();
            foreach (SoundCue cue in (SoundCue[])Enum.GetValues(typeof(SoundCue)))
            {
                AudioClip[] clips = EffectRecipes.For(cue)
                    .Select(sound => Write(sound, EffectsFolder, false))
                    .ToArray();
                effects.Add(new SoundBankEffect(cue, clips));
            }

            List<SoundBankMusic> music = new List<SoundBankMusic>();
            foreach (MusicPiece piece in ((MusicPiece[])Enum.GetValues(typeof(MusicPiece))).Where(piece => piece != MusicPiece.None))
            {
                music.Add(new SoundBankMusic(piece, Write(MusicRecipes.For(piece), MusicFolder, true)));
            }

            SoundBank bank = AssetDatabase.LoadAssetAtPath<SoundBank>(BankPath);
            if (bank == null)
            {
                bank = ScriptableObject.CreateInstance<SoundBank>();
                AssetDatabase.CreateAsset(bank, BankPath);
            }

            bank.Fill(effects.ToArray(), music.ToArray());
            EditorUtility.SetDirty(bank);
            AssetDatabase.SaveAssets();
            Debug.Log("Sounds: made " + effects.Sum(effect => effect.Clips.Count) + " effects and " + music.Count
                + " music loops into " + BankPath + ".");
        }

        /// <summary>
        /// <see cref="Make"/> in batch mode (<c>-executeMethod Scolita.Editor.Sounds.SoundMaker.MakeFromCommandLine</c>), quitting
        /// with 0 when done and 1 when it failed.
        /// </summary>
        public static void MakeFromCommandLine()
        {
            try
            {
                Make();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        /// <summary>
        /// <paramref name="samples"/> brought to <paramref name="targetLufs"/>, then turned down if their peak is above
        /// <paramref name="highestPeakDbfs"/>. Called by <see cref="Write"/> and <c>SoundBankTests</c>.
        /// </summary>
        public static float[] Levelled(float[] samples, double targetLufs, double highestPeakDbfs)
        {
            MonoAudio audio = new MonoAudio(samples, Synth.Rate);
            double gain = Math.Pow(10.0, (targetLufs - Loudness.IntegratedLufs(audio)) / 20.0);
            double peak = Math.Pow(10.0, Loudness.PeakDbfs(audio) / 20.0) * gain;
            double highest = Math.Pow(10.0, highestPeakDbfs / 20.0);
            if (peak > highest)
            {
                gain *= highest / peak;
            }

            return samples.Select(sample => (float)(sample * gain)).ToArray();
        }

        private static AudioClip Write(MadeSound sound, string folder, bool isMusic)
        {
            string path = folder + "/" + sound.Name + ".wav";
            float[] levelled = Levelled(sound.Samples, sound.TargetLufs, sound.HighestPeakDbfs);
            WavFile.Write(Path.GetFullPath(path), new MonoAudio(levelled, Synth.Rate));
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AudioImporter importer = (AudioImporter)AssetImporter.GetAtPath(path);
            importer.forceToMono = true;
            importer.loadInBackground = isMusic;
            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            settings.loadType = isMusic ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat = AudioCompressionFormat.Vorbis;
            settings.quality = isMusic ? MusicQuality : EffectQuality;
            settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
            settings.preloadAudioData = !isMusic;
            importer.defaultSampleSettings = settings;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }
    }
}
