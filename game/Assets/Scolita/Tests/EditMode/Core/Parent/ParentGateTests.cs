using System;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Parent;
using Scolita.Core.Randomness;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Parent
{
    /// <summary>Tests <see cref="ParentGate"/>.</summary>
    public sealed class ParentGateTests
    {
        /// <summary>Tests that <see cref="ParentGate.Hold"/> fills the ring and asks only once the whole hold is done.</summary>
        [Test]
        public void TestHold_AsksOnlyAfterTheWholeHold()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());

            bool asksEarly = gate.Hold(1.5f);
            float halfway = gate.HoldProgress;
            bool asksLate = gate.Hold(1.5f);

            Assert.That(asksEarly, Is.False);
            Assert.That(halfway, Is.EqualTo(0.5f).Within(0.0001f));
            Assert.That(asksLate, Is.True);
            Assert.That(gate.Stage, Is.EqualTo(GateStage.Asking));
            Assert.That(gate.HoldProgress, Is.EqualTo(1f));
        }

        /// <summary>Tests that <see cref="ParentGate.Hold"/> counts many short frames like one long hold.</summary>
        [Test]
        public void TestHold_ManyFramesAddUp()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());

            int frames = 1;
            while (!gate.Hold(1f / 60f))
            {
                frames++;
            }

            Assert.That(frames, Is.InRange(179, 181));
            Assert.That(gate.Stage, Is.EqualTo(GateStage.Asking));
        }

        /// <summary>Tests that <see cref="ParentGate.Release"/> before the hold is full empties the ring.</summary>
        [Test]
        public void TestRelease_EarlyEmptiesTheRing()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());

            gate.Hold(2.9f);
            gate.Release();
            GateStage released = gate.Stage;
            bool asks = gate.Hold(0.2f);

            Assert.That(released, Is.EqualTo(GateStage.Shut));
            Assert.That(asks, Is.False);
            Assert.That(gate.Stage, Is.EqualTo(GateStage.Holding));
            Assert.That(gate.HoldProgress, Is.EqualTo(0.2f / 3f).Within(0.0001f));
        }

        /// <summary>Tests that <see cref="ParentGate.Release"/> while the gate asks keeps the question.</summary>
        [Test]
        public void TestRelease_WhileAskingKeepsTheQuestion()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());

            gate.Hold(3f);
            gate.Release();

            Assert.That(gate.Stage, Is.EqualTo(GateStage.Asking));
        }

        /// <summary>Tests that <see cref="ParentGate.Hold"/> with <see cref="FixedRandom"/> says 19 among 18, 11 and 19.</summary>
        [Test]
        public void TestHold_PicksTheNumberAndItsChoices()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());

            gate.Hold(3f);

            Assert.That(gate.Number, Is.EqualTo(19));
            Assert.That(gate.Choices, Is.EqualTo(new[] { 18, 11, 19 }));
        }

        /// <summary>Tests that <see cref="ParentGate.Hold"/> always offers different numbers in range, the said one among them, over many seeds.</summary>
        [Test]
        public void TestHold_ChoicesDifferentInRangeWithTheNumber()
        {
            ParentRules rules = TestParentRules.Default();
            int[] placesOfTheNumber = new int[rules.GateChoices];
            int[] numbersSaid = new int[rules.GateHighestNumber + 1];
            for (ulong seed = 0; seed < 300; seed++)
            {
                ParentGate gate = new ParentGate(rules, new SeededRandom(seed));

                gate.Hold(3f);

                Assert.That(gate.Choices.Distinct().Count(), Is.EqualTo(rules.GateChoices));
                Assert.That(gate.Choices, Is.All.InRange(rules.GateLowestNumber, rules.GateHighestNumber));
                Assert.That(gate.Choices, Does.Contain(gate.Number));
                placesOfTheNumber[gate.Choices.ToList().IndexOf(gate.Number)]++;
                numbersSaid[gate.Number]++;
            }

            Assert.That(placesOfTheNumber, Is.All.GreaterThan(50));
            Assert.That(numbersSaid.Skip(rules.GateLowestNumber), Is.All.GreaterThan(10));
        }

        /// <summary>Tests that <see cref="ParentGate.Choose"/> with the said number opens the gate, which no hold changes then.</summary>
        [Test]
        public void TestChoose_TheNumberOpens()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());
            gate.Hold(3f);

            bool opened = gate.Choose(gate.Number);

            Assert.That(opened, Is.True);
            Assert.That(gate.Stage, Is.EqualTo(GateStage.Open));
            Assert.That(gate.Hold(3f), Is.False);
        }

        /// <summary>Tests that <see cref="ParentGate.Choose"/> with another number shuts the gate, and it must be held again.</summary>
        [Test]
        public void TestChoose_AnotherNumberShuts()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());
            gate.Hold(3f);

            bool opened = gate.Choose(gate.Choices[0]);

            Assert.That(opened, Is.False);
            Assert.That(gate.Stage, Is.EqualTo(GateStage.Shut));
            Assert.That(gate.HoldProgress, Is.EqualTo(0f));
            Assert.Throws<InvalidOperationException>(() => gate.Choose(gate.Number));
        }

        /// <summary>Tests that <see cref="ParentGate.Choose"/> throws while the gate isn't asking.</summary>
        [Test]
        public void TestChoose_NotAskingThrows()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());

            gate.Hold(1f);

            Assert.Throws<InvalidOperationException>(() => gate.Choose(11));
        }

        /// <summary>Tests that <see cref="ParentGate.Shut"/> closes the question; the next hold starts from an empty ring.</summary>
        [Test]
        public void TestShut_ClosesTheQuestion()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());
            gate.Hold(3f);

            gate.Shut();
            bool asks = gate.Hold(2f);

            Assert.That(asks, Is.False);
            Assert.That(gate.Stage, Is.EqualTo(GateStage.Holding));
        }

        /// <summary>Tests that <see cref="ParentGate.Hold"/> refuses a negative time.</summary>
        [Test]
        public void TestHold_NegativeThrows()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());

            Assert.Throws<ArgumentOutOfRangeException>(() => gate.Hold(-0.1f));
        }

        /// <summary>Tests that random short taps (each held under a second, let go between) never make <see cref="ParentGate"/> ask.</summary>
        [Test]
        public void TestHold_RandomTapsNeverAsk()
        {
            ParentGate gate = new ParentGate(TestParentRules.Default(), new FixedRandom());
            SeededRandom taps = new SeededRandom(7);
            bool everAsked = false;

            for (int tap = 0; tap < 2000; tap++)
            {
                int frames = taps.NextInt(1, 50);
                for (int frame = 0; frame < frames; frame++)
                {
                    everAsked |= gate.Hold(1f / 60f);
                }

                gate.Release();
            }

            Assert.That(everAsked, Is.False);
            Assert.That(gate.Stage, Is.EqualTo(GateStage.Shut));
        }
    }
}
