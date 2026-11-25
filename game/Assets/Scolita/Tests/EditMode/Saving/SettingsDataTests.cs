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

            Assert.That(defaults.Version, Is.EqualTo(2));
            Assert.That(defaults.MusicVolume, Is.EqualTo(1f));
            Assert.That(defaults.SoundsVolume, Is.EqualTo(1f));
            Assert.That(defaults.VoiceVolume, Is.EqualTo(1f));
            Assert.That(defaults.VibrationOn, Is.True);
            Assert.That(defaults.AppLanguage, Is.EqualTo(Language.Romanian));
        }

        /// <summary>Tests that the <see cref="TabletSettingsData"/> constructor clamps every volume to 0–1, and a non-number to 1.</summary>
        [TestCase(-0.5f, 0f)]
        [TestCase(0.4f, 0.4f)]
        [TestCase(3f, 1f)]
        [TestCase(float.NaN, 1f)]
        [TestCase(float.PositiveInfinity, 1f)]
        public void TestTabletSettingsData_ClampsVolumes(float volume, float expected)
        {
            TabletSettingsData settings = new TabletSettingsData(volume, volume, volume, true, Language.English);

            Assert.That(settings.VoiceVolume, Is.EqualTo(expected).Within(0.0001f));
            Assert.That(settings.MusicVolume, Is.EqualTo(expected).Within(0.0001f));
            Assert.That(settings.SoundsVolume, Is.EqualTo(expected).Within(0.0001f));
        }

        /// <summary>Tests that <see cref="TabletSettingsData.Upgraded"/> turns a version 1 file's music and sounds on or off into volumes of 1 or 0.</summary>
        [Test]
        public void TestUpgraded_TabletVersion1OnAndOffBecomeVolumes()
        {
            TabletSettingsData musicOff = JsonUtility.FromJson<TabletSettingsData>(
                "{\"version\":1,\"musicOn\":false,\"soundsOn\":true,\"voiceVolume\":0.6,\"vibrationOn\":false,\"appLanguage\":\"en\"}");
            TabletSettingsData soundsOff = JsonUtility.FromJson<TabletSettingsData>(
                "{\"version\":1,\"musicOn\":true,\"soundsOn\":false,\"voiceVolume\":1.0,\"vibrationOn\":true,\"appLanguage\":\"ro\"}");

            TabletSettingsData first = musicOff.Upgraded();
            TabletSettingsData second = soundsOff.Upgraded();

            Assert.That(first.Version, Is.EqualTo(2));
            Assert.That(first.MusicVolume, Is.EqualTo(0f));
            Assert.That(first.SoundsVolume, Is.EqualTo(1f));
            Assert.That(first.VoiceVolume, Is.EqualTo(0.6f).Within(0.0001f));
            Assert.That(first.VibrationOn, Is.False);
            Assert.That(first.AppLanguage, Is.EqualTo(Language.English));
            Assert.That(second.MusicVolume, Is.EqualTo(1f));
            Assert.That(second.SoundsVolume, Is.EqualTo(0f));
            Assert.That(second.VibrationOn, Is.True);
        }

        /// <summary>Tests that <see cref="TabletSettingsData.Upgraded"/> keeps a version 2 file's volumes (clamped), whatever its on and off say.</summary>
        [Test]
        public void TestUpgraded_TabletVersion2KeepsItsVolumes()
        {
            TabletSettingsData read = JsonUtility.FromJson<TabletSettingsData>(
                "{\"version\":2,\"musicOn\":false,\"soundsOn\":false,\"musicVolume\":0.3,\"soundsVolume\":4.0,\"voiceVolume\":0.5}");

            TabletSettingsData upgraded = read.Upgraded();

            Assert.That(upgraded.MusicVolume, Is.EqualTo(0.3f).Within(0.0001f));
            Assert.That(upgraded.SoundsVolume, Is.EqualTo(1f));
        }

        /// <summary>Tests that <see cref="TabletSettingsData"/> writes version 2 with its volumes, and its on and off as "volume above 0".</summary>
        [Test]
        public void TestTabletSettingsData_WritesVolumesAndOnOff()
        {
            string json = JsonUtility.ToJson(new TabletSettingsData(0f, 0.5f, 1f, true, Language.Romanian));

            Assert.That(json, Does.Contain("\"version\":2"));
            Assert.That(json, Does.Contain("\"musicOn\":false"));
            Assert.That(json, Does.Contain("\"soundsOn\":true"));
            Assert.That(json, Does.Contain("\"musicVolume\":0.0"));
            Assert.That(json, Does.Contain("\"soundsVolume\":0.5"));
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
