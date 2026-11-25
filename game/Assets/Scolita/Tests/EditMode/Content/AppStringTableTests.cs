using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Game.Lessons;
using Scolita.Game.Parent;
using Scolita.Game.Settings;
using Scolita.Tests.EditMode.Fakes;
using UnityEditor;
using UnityEngine.Localization.Tables;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>
    /// Tests the app's string tables (<see cref="AppLocale.TableName"/>, under <c>Assets/Scolita/Localization/</c>):
    /// every key has its words in Romanian and in English, and the parent gate's written numbers match what the gate
    /// says (<see cref="ParentGateFlow"/>).
    /// </summary>
    public sealed class AppStringTableTests
    {
        /// <summary>Tests that every key of the app's table has words in both <c>ro</c> and <c>en-GB</c>.</summary>
        [Test]
        public void TestTables_EveryKeyInBothLanguages()
        {
            StringTable romanian = Table(Language.Romanian);
            StringTable english = Table(Language.English);

            string[] keys = romanian.SharedData.Entries.Select(entry => entry.Key).ToArray();
            string[] missing = keys
                .Where(key => string.IsNullOrWhiteSpace(romanian.GetEntry(key)?.Value)
                    || string.IsNullOrWhiteSpace(english.GetEntry(key)?.Value))
                .ToArray();

            Assert.That(keys, Does.Contain("go"));
            Assert.That(missing, Is.Empty);
        }

        /// <summary>
        /// Tests that each number the gate can say (the learning rules asset's range) is written in the table in both
        /// languages, and that its voice line in the content says that word (<see cref="ParentGateFlow.LineOf"/>).
        /// </summary>
        [Test]
        public void TestTables_GateWordsMatchTheirLines()
        {
            ParentRules rules = AssetDatabase.LoadAssetAtPath<LearningRulesAsset>("Assets/Scolita/Settings/LearningRules.asset")
                .ToParentRules();
            Curriculum curriculum = RealContent.Read();
            List<string> problems = new List<string>();

            foreach (Language language in new[] { Language.Romanian, Language.English })
            {
                StringTable table = Table(language);
                for (int number = rules.GateLowestNumber; number <= rules.GateHighestNumber; number++)
                {
                    string word = table.GetEntry(ParentGateFlow.WordKeyOf(number))?.Value;
                    VoiceLineId line = ParentGateFlow.LineOf(language, number);
                    bool said = curriculum.VoiceLines.TryGet(line.Id, out VoiceLine voiceLine);
                    if (string.IsNullOrEmpty(word) || !said || !voiceLine.Text.Contains(" " + word + "."))
                    {
                        problems.Add(language + " " + number + ": '" + word + "' / " + line);
                    }
                }
            }

            Assert.That(problems, Is.Empty);
        }

        private static StringTable Table(Language language)
        {
            string name = AppLocale.TableName + "_" + AppLocale.CodeOf(language);
            StringTable table = AssetDatabase.FindAssets("t:StringTable " + name)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<StringTable>)
                .SingleOrDefault(candidate => candidate.name == name);
            Assert.That(table, Is.Not.Null, name);
            return table;
        }
    }
}
