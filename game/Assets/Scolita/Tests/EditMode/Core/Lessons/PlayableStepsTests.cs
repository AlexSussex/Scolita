using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="PlayableSteps"/>: a step plays when its kind has a view and rules, and its content is there.</summary>
    public sealed class PlayableStepsTests
    {
        /// <summary>
        /// Tests that <see cref="PlayableSteps.Includes"/> plays a print tracing step whose letter has its shape,
        /// and skips a handwritten one with no shape yet, and the steps of a kind with no view.
        /// </summary>
        [Test]
        public void TestIncludes_TracingNeedsItsShape()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,TraceLetter,,a,,form=handwritten;case=lower,");
            Curriculum curriculum = ContentCsvs.Read(csvs);
            Step[] tracing = RealContent.Lesson(curriculum, "ro-a").Steps
                .Where(step => step.Kind == ActivityKind.TraceLetter)
                .ToArray();
            Step meet = RealContent.Lesson(curriculum, "ro-a").Steps.First(step => step.Kind == ActivityKind.MeetLetter);

            PlayableSteps playable = new PlayableSteps(new[] { ActivityKind.TraceLetter }, curriculum);

            Assert.That(tracing.Length, Is.EqualTo(2));
            Assert.That(playable.Includes(tracing[0]), Is.True);
            Assert.That(playable.Includes(tracing[1]), Is.False);
            Assert.That(playable.Includes(meet), Is.False);
        }
    }
}
