using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Rules;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>
    /// Tests the voice line rules added by <c>voice</c> (<see cref="VoiceLineRules"/>, <see cref="ContentTables"/>):
    /// clips per spoken segment and their names, slots, and the <c>status</c> column.
    /// </summary>
    public sealed class VoiceLineClipRulesTests
    {
        /// <summary>Tests that <see cref="ContentRules.Check"/> takes one clip per spoken segment, named by place.</summary>
        [Test]
        public void TestCheck_OneClipPerSpokenSegment()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines,
                "ro-line-hunt,ro,line,Atinge toate stelele care au litera {x}.,ro-line-hunt.ogg,recorded,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines,
                "ro-line-more,ro,line,Mai sunt stele cu litera {x}. Caut\u0103-le!,ro-line-more.1.ogg ro-line-more.2.ogg,recorded,,");

            IReadOnlyList<ContentProblem> problems = ContentCsvs.Check(csvs, "ro-line-hunt.ogg", "ro-line-more.1.ogg", "ro-line-more.2.ogg");

            Assert.That(problems.Where(problem => problem.Source.File == ContentFileNames.VoiceLines).Select(problem => problem.ToString()),
                Is.Empty);
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails on the wrong number of clips, a clip with the wrong name, and an unknown slot.</summary>
        [Test]
        public void TestCheck_WrongClipsAndSlots()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int countRow = ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines,
                "ro-line-more,ro,line,Mai sunt stele cu litera {x}. Caut\u0103-le!,ro-line-more.ogg,,,");
            int nameRow = ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-sound-m,", "ro-sound-m,ro,sound,/m/,m.ogg,,,");
            int slotRow = ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-line-odd,ro,line,Atinge {letter}.,,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.VoiceLines, countRow, "names 1 clips", "2 spoken parts");
            ContentCsvs.AssertError(errors, ContentFileNames.VoiceLines, nameRow, "m.ogg must be named ro-sound-m.ogg");
            ContentCsvs.AssertError(errors, ContentFileNames.VoiceLines, slotRow, "{letter}", "{x}");
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> refuses a status that isn't one, and only warns when the column is missing.</summary>
        [Test]
        public void TestRead_StatusColumn()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-name-m,", "ro-name-m,ro,name,me,,done,,");
            Dictionary<string, string> withoutStatus = ContentCsvs.Valid();
            withoutStatus[ContentFileNames.VoiceLines] = withoutStatus[ContentFileNames.VoiceLines]
                .Replace("clip,status,retired", "clip,retired");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);
            IReadOnlyList<ContentProblem> withoutProblems = ContentCsvs.Check(withoutStatus);

            ContentCsvs.AssertError(errors, ContentFileNames.VoiceLines, row, "status 'done'");
            Assert.That(withoutProblems.Any(problem => problem.IsError), Is.False);
            Assert.That(withoutProblems.Single(problem => problem.Source.File == ContentFileNames.VoiceLines).ToString(),
                Does.Contain("no column status"));
        }
    }
}
