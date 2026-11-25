using System;
using Scolita.Core.Languages;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Settings
{
    /// <summary>
    /// The tablet's own settings, shared by every profile and saved as <c>settings.json</c> in the
    /// save root through <see cref="SaveStore.ForTablet"/>: the music's, the sounds' and the voice's volumes, vibration
    /// on or off, and the language of the app's own words (buttons, the parent area). Version 2 has the music's and the
    /// sounds' volumes (0–1); version 1 had them on or off, read as 1 or 0. <c>musicOn</c> and <c>soundsOn</c> stay
    /// declared so a version 1 file can be read, and since <c>JsonUtility</c> writes every declared field they are written
    /// too, as "its volume is above 0".
    /// </summary>
    [Serializable]
    public sealed class TabletSettingsData : ISaveData<TabletSettingsData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "settings";

        private const int LatestVersion = 2;
        private const int FirstVersionWithVolumes = 2;
        private const float LowestVolume = 0f;
        private const float HighestVolume = 1f;
        private const Language DefaultAppLanguage = Language.Romanian;

        [SerializeField] private int version;
        [SerializeField] private bool musicOn;
        [SerializeField] private bool soundsOn;
        [SerializeField] private float musicVolume;
        [SerializeField] private float soundsVolume;
        [SerializeField] private float voiceVolume;
        [SerializeField] private bool vibrationOn;
        [SerializeField] private string appLanguage;

        /// <summary>
        /// Creates settings in the current shape, each volume clamped to 0–1 (1 when it is not a number). Called by
        /// <see cref="CreateDefaults"/>, <see cref="Upgraded"/>, the parent area and the tests.
        /// </summary>
        public TabletSettingsData(float musicVolume, float soundsVolume, float voiceVolume, bool vibrationOn, Language appLanguage)
            : this(musicVolume, soundsVolume, voiceVolume, vibrationOn, LanguageCodes.CodeOf(appLanguage))
        {
        }

        private TabletSettingsData(float musicVolume, float soundsVolume, float voiceVolume, bool vibrationOn, string appLanguageCode)
        {
            version = LatestVersion;
            this.musicVolume = ClampedVolume(musicVolume);
            this.soundsVolume = ClampedVolume(soundsVolume);
            this.voiceVolume = ClampedVolume(voiceVolume);
            musicOn = this.musicVolume > LowestVolume;
            soundsOn = this.soundsVolume > LowestVolume;
            this.vibrationOn = vibrationOn;
            appLanguage = appLanguageCode;
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>The music's volume, from 0 (no music) to 1 (full).</summary>
        public float MusicVolume => musicVolume;

        /// <summary>The sound effects' volume, from 0 (none) to 1 (full).</summary>
        public float SoundsVolume => soundsVolume;

        /// <summary>The voice's volume, from 0 (silent) to 1 (full).</summary>
        public float VoiceVolume => voiceVolume;

        /// <summary>Whether the tablet vibrates on a right answer or a sticker placed.</summary>
        public bool VibrationOn => vibrationOn;

        /// <summary>The language of the app's own words; Romanian if the saved code is unknown.</summary>
        public Language AppLanguage => LanguageOf(appLanguage);

        /// <summary>
        /// The settings of a first start: every volume full, vibration on, the app's words in
        /// Romanian (the anchor language, decision D1). Called by <c>BootLoader</c> and the tests.
        /// </summary>
        public static TabletSettingsData CreateDefaults()
        {
            return new TabletSettingsData(HighestVolume, HighestVolume, HighestVolume, true, DefaultAppLanguage);
        }

        /// <summary>
        /// Returns these settings in the current shape: a version 1 file's music and sounds on or off become volumes of 1
        /// or 0; the volumes clamped; an unknown language
        /// code (from a later version of the app) is kept, so that version can still use it, and
        /// read as Romanian (<see cref="AppLanguage"/>). <c>JsonUtility</c> leaves a field missing from the file at its type's
        /// default (false, 0, null), not at this class's default, so a migration from a version
        /// without a field must set that field by <see cref="Version"/> here.
        /// Called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public TabletSettingsData Upgraded()
        {
            bool hasVolumes = version >= FirstVersionWithVolumes;
            return new TabletSettingsData(hasVolumes ? musicVolume : VolumeOf(musicOn), hasVolumes ? soundsVolume : VolumeOf(soundsOn),
                voiceVolume, vibrationOn, appLanguage);
        }

        private static float ClampedVolume(float volume)
        {
            return float.IsNaN(volume) ? HighestVolume : Mathf.Clamp(volume, LowestVolume, HighestVolume);
        }

        private static float VolumeOf(bool on)
        {
            return on ? HighestVolume : LowestVolume;
        }

        private static Language LanguageOf(string code)
        {
            return LanguageCodes.TryParseLanguage(code, out Language language) ? language : DefaultAppLanguage;
        }
    }
}
