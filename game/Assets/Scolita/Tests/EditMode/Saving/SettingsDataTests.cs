using NUnit.Framework;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Tests.EditMode.Fakes;
using Scolita.Game.Settings;
using UnityEngine;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="TabletSettingsData"/> and <see cref="LearningSettingsData"/>: defaults and ranges.</summary>
    public sealed class SettingsDataTests
    {
        /// <summary>Tests that <see cref="TabletSettingsData.CreateDefaults"/> turns everything on, in Romanian.</summary>
        [Test]
        public void TestCreateDefaults_TabletEverythingOnInRomanian()
        {
            TabletSettingsData defaults = TabletSettingsData.CreateDefaults();

            Assert.That(defaults.Version, Is.EqualTo(1));
            Assert.That(defaults.MusicOn, Is.True);
            Assert.That(defaults.SoundsOn, Is.True);
            Assert.That(defaults.VoiceVolume, Is.EqualTo(1f));
            Assert.That(defaults.VibrationOn, Is.True);
            Assert.That(defaults.AppLanguage, Is.EqualTo(Language.Romanian));
        }

        /// <summary>Tests that the <see cref="TabletSettingsData"/> constructor clamps the voice volume to 0–1, and a non-number to 1.</summary>
        [TestCase(-0.5f, 0f)]
        [TestCase(0.4f, 0.4f)]
        [TestCase(3f, 1f)]
        [TestCase(float.NaN, 1f)]
        [TestCase(float.PositiveInfinity, 1f)]
        public void TestTabletSettingsData_ClampsVoiceVolume(float volume, float expected)
        {
            TabletSettingsData settings = new TabletSettingsData(true, true, volume, true, Language.English);

            Assert.That(settings.VoiceVolume, Is.EqualTo(expected).Within(0.0001f));
        }

        /// <summary>Tests that <see cref="TabletSettingsData.Upgraded"/> reads an unknown language code as Romanian.</summary>
        [Test]
        public void TestUpgraded_TabletUnknownLanguageIsRomanian()
        {
            TabletSettingsData read = JsonUtility.FromJson<TabletSettingsData>(
                "{\"version\":1,\"voiceVolume\":0.5,\"appLanguage\":\"de\"}");

            TabletSettingsData upgraded = read.Upgraded();

            Assert.That(upgraded.AppLanguage, Is.EqualTo(Language.Romanian));
            Assert.That(upgraded.VoiceVolume, Is.EqualTo(0.5f).Within(0.0001f));
        }

        /// <summary>
        /// Tests that <see cref="TabletSettingsData.Upgraded"/> and <see cref="LearningSettingsData.Upgraded"/>
        /// keep an unknown code (from a later version of the app) when saved again.
        /// </summary>
        [Test]
        public void TestUpgraded_KeepsUnknownCodes()
        {
            TabletSettingsData tablet = JsonUtility.FromJson<TabletSettingsData>("{\"version\":1,\"appLanguage\":\"de\"}");
            LearningSettingsData learning = JsonUtility.FromJson<LearningSettingsData>("{\"version\":1,\"languageMix\":\"fr\"}");

            string tabletJson = JsonUtility.ToJson(tablet.Upgraded());
            string learningJson = JsonUtility.ToJson(learning.Upgraded());

            Assert.That(tabletJson, Does.Contain("\"appLanguage\":\"de\""));
            Assert.That(learningJson, Does.Contain("\"languageMix\":\"fr\""));
        }

        /// <summary>Tests that <see cref="LearningSettingsData.CreateDefaults"/> learns in both languages.</summary>
        [Test]
        public void TestCreateDefaults_LearningInBothLanguages()
        {
            Assert.That(LearningSettingsData.CreateDefaults().LanguageMix, Is.EqualTo(LanguageMix.Both));
        }

        /// <summary>Tests that <see cref="LearningSettingsData.Upgraded"/> reads an unknown language mix code as both.</summary>
        [Test]
        public void TestUpgraded_LearningUnknownMixIsBoth()
        {
            LearningSettingsData read = JsonUtility.FromJson<LearningSettingsData>("{\"version\":1,\"languageMix\":\"fr\"}");

            Assert.That(read.Upgraded().LanguageMix, Is.EqualTo(LanguageMix.Both));
        }

        /// <summary>Tests that <see cref="LearningSettingsData"/> is written with the mix's stable code.</summary>
        [Test]
        public void TestLearningSettingsData_WritesStableCode()
        {
            string json = JsonUtility.ToJson(new LearningSettingsData(LanguageMix.EnglishOnly));

            Assert.That(json, Is.EqualTo("{\"version\":2,\"languageMix\":\"en\",\"sessionLimitMinutes\":0}"));
        }

        /// <summary>Tests that <see cref="LearningSettingsData.CreateDefaults"/> has no daily limit (decision D8).</summary>
        [Test]
        public void TestCreateDefaults_NoDailyLimit()
        {
            Assert.That(LearningSettingsData.CreateDefaults().SessionLimitBy(TestParentRules.Default()), Is.EqualTo(SessionLimit.Off));
        }

        /// <summary>Tests that <see cref="LearningSettingsData.Upgraded"/> brings a version 1 file to version 2, its mix kept and no daily limit.</summary>
        [Test]
        public void TestUpgraded_VersionOneHasNoLimit()
        {
            LearningSettingsData read = JsonUtility.FromJson<LearningSettingsData>(
                "{\"version\":1,\"languageMix\":\"ro\",\"sessionLimitMinutes\":15}");

            LearningSettingsData upgraded = read.Upgraded();

            Assert.That(upgraded.Version, Is.EqualTo(2));
            Assert.That(upgraded.LanguageMix, Is.EqualTo(LanguageMix.RomanianOnly));
            Assert.That(upgraded.SessionLimitBy(TestParentRules.Default()), Is.EqualTo(SessionLimit.Off));
        }

        /// <summary>Tests that <see cref="LearningSettingsData.Upgraded"/> keeps a limit, and an unknown one (read as off) for a later version.</summary>
        [Test]
        public void TestUpgraded_KeepsTheLimitAndAnUnknownOne()
        {
            ParentRules rules = TestParentRules.Default();
            LearningSettingsData known = JsonUtility.FromJson<LearningSettingsData>(
                "{\"version\":2,\"languageMix\":\"both\",\"sessionLimitMinutes\":20}");
            LearningSettingsData unknown = JsonUtility.FromJson<LearningSettingsData>(
                "{\"version\":2,\"languageMix\":\"both\",\"sessionLimitMinutes\":45}");
            LearningSettingsData negative = JsonUtility.FromJson<LearningSettingsData>(
                "{\"version\":2,\"languageMix\":\"both\",\"sessionLimitMinutes\":-3}");

            Assert.That(known.Upgraded().SessionLimitBy(rules).Minutes, Is.EqualTo(20));
            Assert.That(unknown.Upgraded().SessionLimitBy(rules), Is.EqualTo(SessionLimit.Off));
            Assert.That(JsonUtility.ToJson(unknown.Upgraded()), Does.Contain("\"sessionLimitMinutes\":45"));
            Assert.That(JsonUtility.ToJson(negative.Upgraded()), Does.Contain("\"sessionLimitMinutes\":0"));
        }

        /// <summary>Tests that <see cref="LearningSettingsData.WithLanguageMix"/> and <see cref="LearningSettingsData.WithSessionLimit"/> change only their setting.</summary>
        [Test]
        public void TestWith_ChangesOnlyItsSetting()
        {
            ParentRules rules = TestParentRules.Default();
            LearningSettingsData settings = new LearningSettingsData(LanguageMix.Both, SessionLimit.Of(10, rules));

            LearningSettingsData english = settings.WithLanguageMix(LanguageMix.EnglishOnly);
            LearningSettingsData longer = settings.WithSessionLimit(SessionLimit.Of(20, rules));

            Assert.That(english.LanguageMix, Is.EqualTo(LanguageMix.EnglishOnly));
            Assert.That(english.SessionLimitBy(rules).Minutes, Is.EqualTo(10));
            Assert.That(longer.LanguageMix, Is.EqualTo(LanguageMix.Both));
            Assert.That(longer.SessionLimitBy(rules).Minutes, Is.EqualTo(20));
        }
    }
}
