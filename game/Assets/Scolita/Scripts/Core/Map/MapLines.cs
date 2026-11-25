using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Voice;

namespace Scolita.Core.Map
{
    /// <summary>
    /// What the mascot says on the galaxy map, in one language, all from <c>voice-lines.csv</c>: the
    /// greeting (the next stop's new letter, else a hello, or the line for every planet done), the
    /// landing after a trip, the two kind "not yet"s (a sleepy planet, a stop ahead), and the rocket's rest at the daily
    /// limit. Made by
    /// <c>MapRunner</c> each time the map opens, and by the tests.
    /// </summary>
    public sealed class MapLines
    {
        private readonly List<VoiceSequence> all = new List<VoiceSequence>();

        /// <summary>The lines of <paramref name="route"/> in <paramref name="language"/>. Called by <c>MapRunner</c> and the tests.</summary>
        public MapLines(MapRoute route, Language language)
        {
            Language = language;
            Greeting = Add(GreetingOf(route, language));
            Landed = Add(LandingOf(route, language));
            PlanetAsleep = Add(VoiceSequence.Of(Line(language, "planet-asleep")));
            StopAhead = Add(VoiceSequence.Of(Line(language, "stop-ahead")));
            Resting = Add(VoiceSequence.Of(Line(language, "rocket-rests")));
        }

        /// <summary>The language every line is in.</summary>
        public Language Language { get; }

        /// <summary>Said when the map opens and the rocket is there: "Bună! Azi învățăm litera m."</summary>
        public VoiceSequence Greeting { get; }

        /// <summary>Said when a trip ends: "Am aterizat pe o planetă nouă!", then the greeting.</summary>
        public VoiceSequence Landed { get; }

        /// <summary>Said when she taps a sleepy planet (or a stop on it): it never opens.</summary>
        public VoiceSequence PlanetAsleep { get; }

        /// <summary>Said when she taps a stop ahead of the rocket on its planet: it never opens.</summary>
        public VoiceSequence StopAhead { get; }

        /// <summary>
        /// Said when the daily limit is reached and the map sleeps, and again on any tap that would open a lesson:
        /// „Racheta se odihnește. Ne vedem mâine!”
        /// </summary>
        public VoiceSequence Resting { get; }

        /// <summary>Every line above, for the bubble's words.</summary>
        public IReadOnlyList<VoiceSequence> All => all;

        /// <summary>
        /// The ids of every line in <see cref="All"/> and of what fills their slots (the letter's sound),
        /// each once: the clips to load when the map opens.
        /// </summary>
        public IReadOnlyList<VoiceLineId> SpokenLines
        {
            get
            {
                List<VoiceLineId> spoken = new List<VoiceLineId>();
                foreach (VoiceSequence sequence in all)
                {
                    foreach (VoiceSequenceItem item in sequence.Items)
                    {
                        AddOnce(spoken, item.Line);
                        if (item.TryGetFill(VoiceSlot.X, out VoiceFill fill) && fill.HasLine)
                        {
                            AddOnce(spoken, fill.Line);
                        }
                    }
                }

                return spoken;
            }
        }

        /// <summary>
        /// The language the map speaks: the go stop's lesson's language, so the lesson she is about to play
        /// is greeted in its own language; for a lesson in both languages, the only one she learns, else
        /// <paramref name="appLanguage"/>. Called by <c>MapRunner</c> and the tests.
        /// </summary>
        public static Language LanguageFor(MapRoute route, LanguageMix mix, Language appLanguage)
        {
            LanguageMix spoken = route.GoStop == null ? LanguageMix.Both : route.GoStop.Lesson.Languages;
            if (spoken == LanguageMix.Both)
            {
                spoken = mix;
            }

            switch (spoken)
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
        /// The ids of the lines the map can say in <paramref name="language"/>, whatever the route: the
        /// content must have each in both languages (<c>MapLinesTests</c>). Called by the tests.
        /// </summary>
        public static IReadOnlyList<VoiceLineId> LineIds(Language language)
        {
            return new[]
            {
                Line(language, "today-letter"),
                Line(language, "hello"),
                Line(language, "all-planets"),
                Line(language, "planet-landed"),
                Line(language, "planet-asleep"),
                Line(language, "stop-ahead"),
                Line(language, "rocket-rests")
            };
        }

        private static VoiceSequence GreetingOf(MapRoute route, Language language)
        {
            Letter letter = SpokenLetter(route, language);
            return Filled(VoiceSequence.Of(GreetingLine(route, language, letter)), letter, language);
        }

        private static VoiceSequence LandingOf(MapRoute route, Language language)
        {
            Letter letter = SpokenLetter(route, language);
            VoiceSequence landed = VoiceSequence.Of(Line(language, "planet-landed"));
            return Filled(landed.Then(GreetingLine(route, language, letter)), letter, language);
        }

        private static Letter SpokenLetter(MapRoute route, Language language)
        {
            Letter letter = route.NextLetter;
            return letter != null && letter.HasLanguage(language) ? letter : null;
        }

        private static VoiceLineId GreetingLine(MapRoute route, Language language, Letter letter)
        {
            if (route.AllDone)
            {
                return Line(language, "all-planets");
            }

            return Line(language, letter != null ? "today-letter" : "hello");
        }

        private static VoiceSequence Filled(VoiceSequence sequence, Letter letter, Language language)
        {
            return letter == null
                ? sequence
                : sequence.With(VoiceSlot.X, VoiceFill.OfLine(letter.SoundLine(language), letter.Lower));
        }

        private static VoiceLineId Line(Language language, string key)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, key);
        }

        private static void AddOnce(List<VoiceLineId> lines, VoiceLineId line)
        {
            if (!lines.Contains(line))
            {
                lines.Add(line);
            }
        }

        private VoiceSequence Add(VoiceSequence line)
        {
            all.Add(line);
            return line;
        }
    }
}
