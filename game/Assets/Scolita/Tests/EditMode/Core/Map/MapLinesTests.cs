using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Core.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Map
{
    /// <summary>Tests <see cref="MapLines"/> on the real content.</summary>
    public sealed class MapLinesTests
    {
        /// <summary>Tests that <see cref="MapLines.LanguageFor"/> speaks the go stop's language: Romanian before a Romanian lesson, English before an English one.</summary>
        [Test]
        public void TestLanguageFor_TheNextLessonsLanguage()
        {
            Curriculum curriculum = RealContent.Read();

            Language beforeRomanian = MapLines.LanguageFor(MapRoutes.Before(curriculum, "ro-m"), LanguageMix.Both, Language.English);
            Language beforeEnglish = MapLines.LanguageFor(MapRoutes.Before(curriculum, "en-m"), LanguageMix.Both, Language.Romanian);

            Assert.That(beforeRomanian, Is.EqualTo(Language.Romanian));
            Assert.That(beforeEnglish, Is.EqualTo(Language.English));
        }

        /// <summary>Tests that <see cref="MapLines.LanguageFor"/> speaks the app's language before a lesson in both, unless she learns only one.</summary>
        [Test]
        public void TestLanguageFor_ALessonInBoth()
        {
            Curriculum curriculum = RealContent.Read();
            MapRoute beforeParty = MapRoutes.Before(curriculum, "party-1");
            MapRoute romanianOnly = MapRoutes.Of(curriculum, LanguageMix.RomanianOnly, "ro-a", "ro-m", "ro-t", "ro-p",
                "ro-words-1", "capitals-1");

            Assert.That(beforeParty.GoStop.Lesson.Languages, Is.EqualTo(LanguageMix.Both));
            Assert.That(MapLines.LanguageFor(beforeParty, LanguageMix.Both, Language.English), Is.EqualTo(Language.English));
            Assert.That(MapLines.LanguageFor(beforeParty, LanguageMix.Both, Language.Romanian), Is.EqualTo(Language.Romanian));
            Assert.That(romanianOnly.GoStop.Lesson.Id.ToString(), Is.EqualTo("party-1"));
            Assert.That(MapLines.LanguageFor(romanianOnly, LanguageMix.RomanianOnly, Language.English), Is.EqualTo(Language.Romanian));
        }

        /// <summary>Tests that the <see cref="MapLines.Greeting"/> names the next stop's new letter by its sound, in the map's language.</summary>
        [Test]
        public void TestGreeting_TheNewLetter()
        {
            MapRoute route = MapRoutes.Before(RealContent.Read(), "en-m");

            MapLines lines = new MapLines(route, Language.English);

            VoiceSequenceItem greeting = lines.Greeting.Items.Single();
            Assert.That(greeting.Line.ToString(), Is.EqualTo("en-line-today-letter"));
            Assert.That(greeting.TryGetFill(VoiceSlot.X, out VoiceFill letter), Is.True);
            Assert.That(letter.Line.ToString(), Is.EqualTo("en-sound-m"));
            Assert.That(letter.Text, Is.EqualTo("m"));
        }

        /// <summary>Tests that the <see cref="MapLines.Greeting"/> is a hello before a stop with no new letter.</summary>
        [Test]
        public void TestGreeting_HelloWithoutANewLetter()
        {
            MapRoute route = MapRoutes.Before(RealContent.Read(), "ro-words-1");

            MapLines lines = new MapLines(route, Language.Romanian);

            Assert.That(lines.Greeting.Items.Single().Line.ToString(), Is.EqualTo("ro-line-hello"));
        }

        /// <summary>Tests that the <see cref="MapLines.Greeting"/> says every planet is done when every stop is.</summary>
        [Test]
        public void TestGreeting_EveryPlanetDone()
        {
            MapRoute route = MapRoutes.AllDone(RealContent.Read());

            MapLines lines = new MapLines(route, Language.Romanian);

            Assert.That(lines.Greeting.Items.Single().Line.ToString(), Is.EqualTo("ro-line-all-planets"));
        }

        /// <summary>Tests that <see cref="MapLines.Landed"/> says the landing, then the greeting with its letter.</summary>
        [Test]
        public void TestLanded_ThenTheGreeting()
        {
            MapRoute route = MapRoutes.Before(RealContent.Read(), "ro-s");

            MapLines lines = new MapLines(route, Language.Romanian);

            Assert.That(lines.Landed.Items.Select(item => item.Line.ToString()),
                Is.EqualTo(new[] { "ro-line-planet-landed", "ro-line-today-letter" }));
            Assert.That(lines.Landed.Items[1].TryGetFill(VoiceSlot.X, out VoiceFill letter), Is.True);
            Assert.That(letter.Line.ToString(), Is.EqualTo("ro-sound-s"));
        }

        /// <summary>Tests that the two "not yet"s are their own lines, and <see cref="MapLines.All"/> lists every line once.</summary>
        [Test]
        public void TestNotYet_TheirOwnLines()
        {
            MapLines lines = new MapLines(MapRoutes.Before(RealContent.Read(), "ro-m"), Language.English);

            Assert.That(lines.PlanetAsleep.Lead.ToString(), Is.EqualTo("en-line-planet-asleep"));
            Assert.That(lines.StopAhead.Lead.ToString(), Is.EqualTo("en-line-stop-ahead"));
            Assert.That(lines.All, Is.EquivalentTo(new[] { lines.Greeting, lines.Landed, lines.PlanetAsleep, lines.StopAhead }));
            Assert.That(lines.Language, Is.EqualTo(Language.English));
        }

        /// <summary>Tests that <see cref="MapLines.SpokenLines"/> lists each line and the letter's sound once, for loading their clips.</summary>
        [Test]
        public void TestSpokenLines_TheLinesAndTheirFills()
        {
            MapLines lines = new MapLines(MapRoutes.Before(RealContent.Read(), "ro-m"), Language.Romanian);

            Assert.That(lines.SpokenLines.Select(line => line.ToString()), Is.EquivalentTo(new[]
            {
                "ro-line-today-letter", "ro-sound-m", "ro-line-planet-landed", "ro-line-planet-asleep", "ro-line-stop-ahead"
            }));
        }

        /// <summary>Tests that every line the map can say (<see cref="MapLines.LineIds"/>) is in the content, in both languages.</summary>
        [Test]
        public void TestLineIds_InTheContentInBothLanguages()
        {
            Curriculum curriculum = RealContent.Read();

            VoiceLineId[] missing = MapLines.LineIds(Language.Romanian)
                .Concat(MapLines.LineIds(Language.English))
                .Where(line => !curriculum.VoiceLines.Contains(line.Id))
                .ToArray();

            Assert.That(missing.Select(line => line.ToString()), Is.Empty);
        }
    }
}
