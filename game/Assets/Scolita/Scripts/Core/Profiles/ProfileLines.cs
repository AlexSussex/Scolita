using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Voice;

namespace Scolita.Core.Profiles
{
    /// <summary>
    /// What the mascot says on "Who's playing?" (design screen 9), all from <c>voice-lines.csv</c>: „Cine se joacă? Atinge
    /// poza ta.” when it opens, and the greeting of the child whose card is tapped, with her name spliced in the
    /// nominative only („Bună, Ana! Hai să zburăm!”, the same for a girl or a boy, whatever the name), or without one for
    /// a profile with no name („Bună! Hai să zburăm!”).
    /// </summary>
    public static class ProfileLines
    {
        /// <summary>„Cine se joacă? Atinge poza ta.” in <paramref name="language"/>; called by <c>ProfilesRunner</c> and the tests.</summary>
        public static VoiceSequence WhoPlays(Language language)
        {
            return VoiceSequence.Of(Line(language, "who-plays"));
        }

        /// <summary>
        /// The greeting of <paramref name="profile"/> in <paramref name="language"/>: its name said by the tablet's voice
        /// (and shown in the bubble), or the greeting without a name. Called by <c>ProfilesRunner</c> and the tests.
        /// </summary>
        public static VoiceSequence Hello(Profile profile, Language language)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (profile.Name.IsNone)
            {
                return VoiceSequence.Of(Line(language, "hello"));
            }

            return VoiceSequence.Of(Line(language, "hello-name")).With(VoiceSlot.Name, VoiceFill.OfText(profile.Name.Text));
        }

        /// <summary>
        /// The language a child is greeted in: the only one she learns (<paramref name="mix"/>), else
        /// <paramref name="appLanguage"/>. Called by <c>ProfilesRunner</c> and the tests.
        /// </summary>
        public static Language LanguageFor(LanguageMix mix, Language appLanguage)
        {
            switch (mix)
            {
                case LanguageMix.RomanianOnly:
                    return Language.Romanian;
                case LanguageMix.EnglishOnly:
                    return Language.English;
                default:
                    return appLanguage;
            }
        }

        /// <summary>
        /// The ids of every line above in <paramref name="language"/>: the content must have each in both languages, and
        /// the clips to load. Called by <c>ProfilesRunner</c> and the tests.
        /// </summary>
        public static IReadOnlyList<VoiceLineId> LineIds(Language language)
        {
            return new[] { Line(language, "who-plays"), Line(language, "hello-name"), Line(language, "hello") };
        }

        private static VoiceLineId Line(Language language, string key)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, key);
        }
    }
}
