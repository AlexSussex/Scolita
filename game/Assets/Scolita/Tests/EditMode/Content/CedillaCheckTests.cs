using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Languages;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>
    /// The content validator's diacritics check: no cedilla ş ţ Ş Ţ anywhere in the string tables
    /// or the content CSVs (<see cref="RomanianSpelling.FindCedillas"/>), as Romanian writes ș ț
    /// with the comma below.
    /// </summary>
    public sealed class CedillaCheckTests
    {
        /// <summary>Tests that <see cref="RomanianSpelling.FindCedillas"/> finds no cedilla in any string table entry.</summary>
        [Test]
        public void TestFindCedillas_NoneInTheStringTables()
        {
            string[] problems = AssetDatabase.FindAssets("t:StringTable")
                .Select(AssetDatabase.GUIDToAssetPath)
                .SelectMany(ProblemsInStringTable)
                .ToArray();

            Assert.That(problems, Is.Empty);
        }

        /// <summary>Tests that <see cref="RomanianSpelling.FindCedillas"/> finds no cedilla in any CSV under the repository's <c>content/</c>.</summary>
        [Test]
        public void TestFindCedillas_NoneInTheContentCsvs()
        {
            string contentFolder = Path.Combine(Directory.GetParent(Application.dataPath).Parent.FullName, "content");
            string[] csvs = Directory.Exists(contentFolder)
                ? Directory.GetFiles(contentFolder, "*.csv", SearchOption.AllDirectories)
                : new string[0];

            string[] problems = csvs
                .SelectMany(ProblemsInCsv)
                .ToArray();

            Assert.That(problems, Is.Empty);
        }

        private static IEnumerable<string> ProblemsInStringTable(string path)
        {
            StringTable table = AssetDatabase.LoadAssetAtPath<StringTable>(path);
            return table.Values
                .SelectMany(entry => RomanianSpelling.FindCedillas(entry.Value)
                    .Select(finding => path + ", entry " + entry.Key + ", " + finding));
        }

        private static IEnumerable<string> ProblemsInCsv(string path)
        {
            return RomanianSpelling.FindCedillas(File.ReadAllText(path))
                .Select(finding => path + ", " + finding);
        }
    }
}
