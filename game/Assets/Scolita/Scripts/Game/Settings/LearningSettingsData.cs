using System;
using Scolita.Core.Languages;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Settings
{
    /// <summary>
    /// One profile's learning settings, saved as <c>learning-settings.json</c> in the profile's
    /// folder through <see cref="SaveStore.ForProfile"/>: for now the language mix (both languages,
    /// or only one). Later briefs add the letter case first and the session limit.
    /// </summary>
    [Serializable]
    public sealed class LearningSettingsData : ISaveData<LearningSettingsData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "learning-settings";

        private const int LatestVersion = 1;
        private const LanguageMix DefaultLanguageMix = LanguageMix.Both;

        [SerializeField] private int version;
        [SerializeField] private string languageMix;

        /// <summary>
        /// Creates settings in the current shape. Called by <see cref="CreateDefaults"/>,
        /// <see cref="Upgraded"/> and the tests.
        /// </summary>
        public LearningSettingsData(LanguageMix languageMix)
            : this(LanguageCodes.CodeOf(languageMix))
        {
        }

        private LearningSettingsData(string languageMixCode)
        {
            version = LatestVersion;
            languageMix = languageMixCode;
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>The languages this profile learns in; both if the saved code is unknown.</summary>
        public LanguageMix LanguageMix => MixOf(languageMix);

        /// <summary>
        /// The settings of a new profile: both languages. Called by <c>BootLoader</c> and the tests.
        /// </summary>
        public static LearningSettingsData CreateDefaults()
        {
            return new LearningSettingsData(DefaultLanguageMix);
        }

        /// <summary>
        /// Returns these settings in the current shape; an unknown language mix code (from a later
        /// version of the app) is kept, and read as both (<see cref="LanguageMix"/>).
        /// A migration from a version without a field sets that field by <see cref="Version"/> here
        /// (<c>JsonUtility</c> leaves it at its type's default). Called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public LearningSettingsData Upgraded()
        {
            return new LearningSettingsData(languageMix);
        }

        private static LanguageMix MixOf(string code)
        {
            return LanguageCodes.TryParseMix(code, out LanguageMix mix) ? mix : DefaultLanguageMix;
        }
    }
}
