using System;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Settings
{
    /// <summary>
    /// One profile's learning settings, saved as <c>learning-settings.json</c> in the profile's
    /// folder through <see cref="SaveStore.ForProfile"/>: the language mix (both languages, or only one)
    /// and the daily limit (version 2; off by default, decision D8). Changed in the parent area.
    /// </summary>
    [Serializable]
    public sealed class LearningSettingsData : ISaveData<LearningSettingsData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "learning-settings";

        private const int LatestVersion = 2;
        private const int LimitOff = 0;
        private const LanguageMix DefaultLanguageMix = LanguageMix.Both;

        [SerializeField] private int version;
        [SerializeField] private string languageMix;
        [SerializeField] private int sessionLimitMinutes;

        /// <summary>
        /// Creates settings in the current shape with no daily limit. Called by <see cref="CreateDefaults"/> and the
        /// tests.
        /// </summary>
        public LearningSettingsData(LanguageMix languageMix)
            : this(LanguageCodes.CodeOf(languageMix), LimitOff)
        {
        }

        /// <summary>
        /// Creates settings in the current shape. Called by the parent area (through <see cref="WithLanguageMix"/> and
        /// <see cref="WithSessionLimit"/>) and the tests.
        /// </summary>
        public LearningSettingsData(LanguageMix languageMix, SessionLimit sessionLimit)
            : this(LanguageCodes.CodeOf(languageMix), sessionLimit.Minutes)
        {
        }

        private LearningSettingsData(string languageMixCode, int sessionLimitMinutes)
        {
            version = LatestVersion;
            languageMix = languageMixCode;
            this.sessionLimitMinutes = Math.Max(LimitOff, sessionLimitMinutes);
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>The languages this profile learns in; both if the saved code is unknown.</summary>
        public LanguageMix LanguageMix => MixOf(languageMix);

        /// <summary>
        /// The settings of a new profile: both languages, no daily limit. Called by <c>BootLoader</c> and the tests.
        /// </summary>
        public static LearningSettingsData CreateDefaults()
        {
            return new LearningSettingsData(DefaultLanguageMix);
        }

        /// <summary>
        /// The daily limit: one of <paramref name="rules"/>' choices, or off (a number from another version of the
        /// app reads as off). Called by <c>MapRunner</c>, <c>ParentRunner</c> and the tests.
        /// </summary>
        public SessionLimit SessionLimitBy(ParentRules rules)
        {
            return SessionLimit.Of(sessionLimitMinutes, rules);
        }

        /// <summary>These settings with <paramref name="mix"/>; called by the parent area and the tests.</summary>
        public LearningSettingsData WithLanguageMix(LanguageMix mix)
        {
            return new LearningSettingsData(LanguageCodes.CodeOf(mix), sessionLimitMinutes);
        }

        /// <summary>These settings with <paramref name="limit"/>; called by the parent area and the tests.</summary>
        public LearningSettingsData WithSessionLimit(SessionLimit limit)
        {
            return new LearningSettingsData(languageMix, limit.Minutes);
        }

        /// <summary>
        /// Returns these settings in the current shape. Version 1 had no daily limit, so it reads as off, as decision
        /// D8 wants. An unknown language mix code or limit (from a later version of the app) is kept, so that version
        /// can still use it, and read as both languages and off; a negative limit becomes off. Called by
        /// <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public LearningSettingsData Upgraded()
        {
            return new LearningSettingsData(languageMix, version < LatestVersion ? LimitOff : sessionLimitMinutes);
        }

        private static LanguageMix MixOf(string code)
        {
            return LanguageCodes.TryParseMix(code, out LanguageMix mix) ? mix : DefaultLanguageMix;
        }
    }
}
