using System;
using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Csv;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>Tests <see cref="CsvCellWriter"/>: one column rewritten, every other character kept.</summary>
    public sealed class CsvCellWriterTests
    {
        /// <summary>Tests that <see cref="CsvCellWriter.Write"/> changes only the cells whose value changes, keeping quotes, CRLF and the byte order mark.</summary>
        [Test]
        public void TestWrite_ChangesOnlyTheCellsThatChange()
        {
            string text = "\uFEFFid,text,status,notes\r\n"
                + "a,\"Hello, you\",,\"a, note\"\r\n"
                + "b,Bye,recorded,\r\n";
            Dictionary<string, string> values = new Dictionary<string, string> { { "a", "recorded" }, { "b", "recorded" } };

            string written = CsvCellWriter.Write(ContentFileNames.VoiceLines, text, "id", "status", "text", values, new ContentProblems());

            Assert.That(written, Is.EqualTo("\uFEFFid,text,status,notes\r\n"
                + "a,\"Hello, you\",recorded,\"a, note\"\r\n"
                + "b,Bye,recorded,\r\n"));
        }

        /// <summary>Tests that <see cref="CsvCellWriter.Write"/> empties a cell, and keeps a row whose id isn't given.</summary>
        [Test]
        public void TestWrite_EmptiesACellAndKeepsOtherRows()
        {
            string text = "id;text;status\na;Hi;missing\nb;Yo;recorded\n";
            Dictionary<string, string> values = new Dictionary<string, string> { { "a", string.Empty } };

            string written = CsvCellWriter.Write(ContentFileNames.VoiceLines, text, "id", "status", "text", values, new ContentProblems());

            Assert.That(written, Is.EqualTo("id;text;status\na;Hi;\nb;Yo;recorded\n"));
        }

        /// <summary>Tests that <see cref="CsvCellWriter.Write"/> adds a missing column right after the one named, in the header and every row.</summary>
        [Test]
        public void TestWrite_AddsTheColumn()
        {
            string text = "id,clip,notes\na,a.ogg,x\nb,,\n";
            Dictionary<string, string> values = new Dictionary<string, string> { { "a", "recorded" }, { "b", string.Empty } };

            string written = CsvCellWriter.Write(ContentFileNames.VoiceLines, text, "id", "status", "clip", values, new ContentProblems());

            Assert.That(written, Is.EqualTo("id,clip,status,notes\na,a.ogg,recorded,x\nb,,,\n"));
        }

        /// <summary>Tests that <see cref="CsvCellWriter.Write"/> fills a row that stops before the column, adding the cells it needs.</summary>
        [Test]
        public void TestWrite_FillsAShortRow()
        {
            string text = "id,text,clip,status\na,Hi\n";
            Dictionary<string, string> values = new Dictionary<string, string> { { "a", "sounds" } };

            string written = CsvCellWriter.Write(ContentFileNames.VoiceLines, text, "id", "status", "clip", values, new ContentProblems());

            Assert.That(written, Is.EqualTo("id,text,clip,status\na,Hi,,sounds\n"));
        }

        /// <summary>Tests that <see cref="CsvCellWriter.Write"/> refuses a value that would need quotes.</summary>
        [Test]
        public void TestWrite_RefusesAValueNeedingQuotes()
        {
            Dictionary<string, string> values = new Dictionary<string, string> { { "a", "x,y" } };

            Assert.That(() => CsvCellWriter.Write(ContentFileNames.VoiceLines, "id,status\na,\n", "id", "status", "id", values, new ContentProblems()),
                Throws.TypeOf<ArgumentException>());
        }
    }
}
