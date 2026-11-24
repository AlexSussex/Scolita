using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="LessonSignature"/>.</summary>
    public sealed class LessonSignatureTests
    {
        /// <summary>
        /// Tests that <see cref="LessonSignature.Of"/> is the same for the same steps read twice, and
        /// differs between lessons.
        /// </summary>
        [Test]
        public void TestOf_SameStepsSameSignature()
        {
            Curriculum first = ContentCsvs.Read(ContentCsvs.Valid());
            Curriculum again = ContentCsvs.Read(ContentCsvs.Valid());

            string signature = LessonSignature.Of(RealContent.Lesson(first, "en-t"));

            Assert.That(signature, Has.Length.EqualTo(16));
            Assert.That(LessonSignature.Of(RealContent.Lesson(again, "en-t")), Is.EqualTo(signature));
            Assert.That(LessonSignature.Of(RealContent.Lesson(first, "en-m")), Is.Not.EqualTo(signature));
        }

        /// <summary>Tests that <see cref="LessonSignature.Of"/> changes when a step's letters change or a step is added.</summary>
        [Test]
        public void TestOf_ChangedStepsChangeIt()
        {
            string before = LessonSignature.Of(RealContent.Lesson(ContentCsvs.Read(ContentCsvs.Valid()), "en-t"));
            Dictionary<string, string> reordered = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(reordered, ContentFileNames.Steps, "en-t,", "en-t,ListenAndPick,,t a m,,,");
            Dictionary<string, string> longer = ContentCsvs.Valid();
            ContentCsvs.AddRow(longer, ContentFileNames.Steps, "en-t,ListenAndPick,,a t,,,");

            Assert.That(LessonSignature.Of(RealContent.Lesson(ContentCsvs.Read(reordered), "en-t")), Is.Not.EqualTo(before));
            Assert.That(LessonSignature.Of(RealContent.Lesson(ContentCsvs.Read(longer), "en-t")), Is.Not.EqualTo(before));
        }
    }
}
