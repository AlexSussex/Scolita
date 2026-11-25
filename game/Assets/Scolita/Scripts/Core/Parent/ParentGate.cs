using System;
using System.Collections.Generic;
using Scolita.Core.Randomness;

namespace Scolita.Core.Parent
{
    /// <summary>
    /// The parent gate (AGENTS.md → "Security and privacy"): the parent button held for the whole
    /// <see cref="ParentRules.GateHoldSeconds"/>, then the number said chosen among the numbers shown. A tap
    /// never opens it: letting go empties the hold, a wrong number shuts the gate and rests it (the hold doesn't fill
    /// for <see cref="ParentRules.GateRestSeconds"/>), and a shut gate must be held again. Made by the map's gate flow (<c>ParentGateFlow</c>) and the tests; allocates nothing
    /// after it is made.
    /// </summary>
    public sealed class ParentGate
    {
        private readonly ParentRules rules;
        private readonly IRandom random;
        private readonly int[] choices;
        private readonly int[] others;
        private float heldSeconds;
        private float restSeconds;

        /// <summary>A shut gate with <paramref name="rules"/>' numbers, picking with <paramref name="random"/>. Called by the gate flow and the tests.</summary>
        public ParentGate(ParentRules rules, IRandom random)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            choices = new int[rules.GateChoices];
            others = new int[rules.GateHighestNumber - rules.GateLowestNumber];
        }

        /// <summary>Where the gate is.</summary>
        public GateStage Stage { get; private set; }

        /// <summary>How full the hold's ring is, from 0 to 1 (1 once the gate asks or is open).</summary>
        public float HoldProgress => Stage == GateStage.Asking || Stage == GateStage.Open
            ? 1f
            : Math.Min(1f, heldSeconds / rules.GateHoldSeconds);

        /// <summary>Whether the gate rests after a wrong number: the hold doesn't fill.</summary>
        public bool IsResting => restSeconds > 0f;

        /// <summary>The number said, while <see cref="Stage"/> is <see cref="GateStage.Asking"/> (or open).</summary>
        public int Number { get; private set; }

        /// <summary>The numbers shown, the said one among them in a random place, while the gate asks.</summary>
        public IReadOnlyList<int> Choices => choices;

        /// <summary>
        /// The parent button has been held <paramref name="seconds"/> more (this frame's time); true when the
        /// hold is full now and the gate asks (a new number is picked). Ignored while the gate asks or is open.
        /// Throws an <see cref="ArgumentOutOfRangeException"/> for negative seconds. Called every frame by the
        /// gate flow while the button is held, and the tests.
        /// </summary>
        public bool Hold(float seconds)
        {
            if (seconds < 0f || float.IsNaN(seconds))
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "A hold lasts 0 seconds or more.");
            }

            if ((Stage != GateStage.Shut && Stage != GateStage.Holding) || IsResting)
            {
                return false;
            }

            heldSeconds += seconds;
            if (heldSeconds < rules.GateHoldSeconds)
            {
                Stage = GateStage.Holding;
                return false;
            }

            Ask();
            return true;
        }

        /// <summary>The parent button was let go: a hold not yet full empties. Called by the gate flow and the tests.</summary>
        public void Release()
        {
            if (Stage == GateStage.Holding)
            {
                ShutGate();
            }
        }

        /// <summary>
        /// <paramref name="number"/> was chosen: the said one opens the gate (true), any other shuts it (false).
        /// Throws an <see cref="InvalidOperationException"/> when the gate isn't asking. Called by the gate flow
        /// and the tests.
        /// </summary>
        public bool Choose(int number)
        {
            if (Stage != GateStage.Asking)
            {
                throw new InvalidOperationException("A number is chosen only while the gate asks (it is " + Stage + ").");
            }

            if (number != Number)
            {
                ShutGate();
                restSeconds = rules.GateRestSeconds;
                return false;
            }

            Stage = GateStage.Open;
            return true;
        }

        /// <summary>
        /// <paramref name="seconds"/> of time passed (this frame's), held or not: a rest after a wrong number runs out.
        /// Called every frame by the gate flow, and the tests.
        /// </summary>
        public void Pass(float seconds)
        {
            if (seconds > 0f)
            {
                restSeconds = Math.Max(0f, restSeconds - seconds);
            }
        }

        /// <summary>The question was closed without an answer (its close button, Android Back): the gate shuts. Called by the gate flow and the tests.</summary>
        public void Shut()
        {
            ShutGate();
        }

        private void ShutGate()
        {
            Stage = GateStage.Shut;
            heldSeconds = 0f;
        }

        private void Ask()
        {
            Number = random.NextInt(rules.GateLowestNumber, rules.GateHighestNumber + 1);
            int count = 0;
            for (int candidate = rules.GateLowestNumber; candidate <= rules.GateHighestNumber; candidate++)
            {
                if (candidate != Number)
                {
                    others[count] = candidate;
                    count++;
                }
            }

            int place = random.NextInt(0, choices.Length);
            int taken = 0;
            for (int index = 0; index < choices.Length; index++)
            {
                if (index == place)
                {
                    choices[index] = Number;
                    continue;
                }

                int pick = random.NextInt(taken, count);
                (others[taken], others[pick]) = (others[pick], others[taken]);
                choices[index] = others[taken];
                taken++;
            }

            Stage = GateStage.Asking;
        }
    }
}
