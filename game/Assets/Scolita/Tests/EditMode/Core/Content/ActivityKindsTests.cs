using System;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>Tests <see cref="ActivityKinds"/> and <see cref="ContentCodes"/>: every kind has its requirements and its code.</summary>
    public sealed class ActivityKindsTests
    {
        /// <summary>Tests that <see cref="ActivityKinds.Of"/> has requirements for every kind, with at least one instruction line.</summary>
        [Test]
        public void TestOf_EveryKindHasRequirements()
        {
            ActivityKind[] kinds = (ActivityKind[])Enum.GetValues(typeof(ActivityKind));

            ActivityKindRequirements[] requirements = kinds.Select(ActivityKinds.Of).ToArray();

            Assert.That(requirements.Select(needs => needs.Kind), Is.EqualTo(kinds));
            Assert.That(requirements.All(needs => needs.InstructionLines.Count > 0), Is.True);
            Assert.That(requirements.Where(needs => needs.IsBridge).Select(needs => needs.Kind),
                Is.EqualTo(new[] { ActivityKind.TwoSounds, ActivityKind.WordTwins }));
        }

        /// <summary>Tests that <see cref="ContentCodes.CodeOf(ActivityKind)"/> and <see cref="ContentCodes.TryParseKind"/> round-trip every kind.</summary>
        [Test]
        public void TestCodeOf_KindsRoundTrip()
        {
            foreach (ActivityKind kind in (ActivityKind[])Enum.GetValues(typeof(ActivityKind)))
            {
                Assert.That(ContentCodes.TryParseKind(ContentCodes.CodeOf(kind), out ActivityKind parsed), Is.True);
                Assert.That(parsed, Is.EqualTo(kind));
            }
        }

        /// <summary>Tests that the group and voice line kind codes round-trip, and an unknown code is refused.</summary>
        [Test]
        public void TestCodeOf_GroupsAndLineKindsRoundTrip()
        {
            foreach (LetterGroup group in (LetterGroup[])Enum.GetValues(typeof(LetterGroup)))
            {
                Assert.That(ContentCodes.TryParseGroup(ContentCodes.CodeOf(group), out LetterGroup parsed), Is.True);
                Assert.That(parsed, Is.EqualTo(group));
            }

            foreach (VoiceLineKind kind in (VoiceLineKind[])Enum.GetValues(typeof(VoiceLineKind)))
            {
                Assert.That(ContentCodes.TryParseVoiceLineKind(ContentCodes.CodeOf(kind), out VoiceLineKind parsed), Is.True);
                Assert.That(parsed, Is.EqualTo(kind));
            }

            Assert.That(ContentCodes.TryParseGroup("SharedClose", out _), Is.False);
        }
    }
}
