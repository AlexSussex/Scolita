using NUnit.Framework;
using Scolita.Game.Tracing;

namespace Scolita.Tests.EditMode.Game
{
    /// <summary>Tests <see cref="FingerPicker"/>: one finger draws on the board, a second finger and a palm never do.</summary>
    public sealed class FingerPickerTests
    {
        /// <summary>Tests that <see cref="FingerPicker.Began"/> lets the first finger on the board draw, and no other until it lifts.</summary>
        [Test]
        public void TestBegan_OnlyTheFirstFingerDraws()
        {
            FingerPicker picker = new FingerPicker();

            Assert.That(picker.Began(3, true, false), Is.True);
            Assert.That(picker.Began(4, true, false), Is.False);
            Assert.That(picker.Draws(3), Is.True);
            Assert.That(picker.Draws(4), Is.False);
            Assert.That(picker.Lifted(4), Is.False);
            Assert.That(picker.Draws(3), Is.True);
            Assert.That(picker.Lifted(3), Is.True);
            Assert.That(picker.Drawing, Is.EqualTo(-1));
            Assert.That(picker.Began(4, true, false), Is.True);
        }

        /// <summary>Tests that <see cref="FingerPicker.Began"/> never lets a palm, or a finger down off the board, draw.</summary>
        [Test]
        public void TestBegan_NoPalmAndNothingOffTheBoard()
        {
            FingerPicker picker = new FingerPicker();

            Assert.That(picker.Began(1, true, true), Is.False);
            Assert.That(picker.Began(2, false, false), Is.False);
            Assert.That(picker.Began(3, true, false), Is.True);

            picker.Reset();

            Assert.That(picker.Draws(3), Is.False);
        }
    }
}
