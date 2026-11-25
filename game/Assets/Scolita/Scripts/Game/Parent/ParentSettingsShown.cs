using System.Collections.Generic;
using Scolita.Core.Languages;
using Scolita.Core.Parent;

namespace Scolita.Game.Parent
{
    /// <summary>The settings the parent area shows: the profile's (the mix, the daily limit) and the tablet's (the voice, the app's language).</summary>
    public readonly struct ParentSettingsShown
    {
        /// <summary>The settings as shown; called by <see cref="ParentRunner"/> and the PlayMode tests.</summary>
        public ParentSettingsShown(LanguageMix mix, SessionLimit limit, IReadOnlyList<int> limitChoices, float voiceVolume,
            Language appLanguage)
        {
            Mix = mix;
            Limit = limit;
            LimitChoices = limitChoices;
            VoiceVolume = voiceVolume;
            AppLanguage = appLanguage;
        }

        /// <summary>The languages of her lessons.</summary>
        public LanguageMix Mix { get; }

        /// <summary>The daily limit, or off.</summary>
        public SessionLimit Limit { get; }

        /// <summary>The daily limit's choices beside off, in minutes.</summary>
        public IReadOnlyList<int> LimitChoices { get; }

        /// <summary>The voice's volume, 0 to 1.</summary>
        public float VoiceVolume { get; }

        /// <summary>The language of the app's own words.</summary>
        public Language AppLanguage { get; }
    }
}
