using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of one activity kind for one step (AGENTS.md, seam 3): it sets up the round, judges
    /// each answer, and says what its hints and its shown answer are and what the mascot says. It
    /// never knows how it is drawn, and it leaves the tries and the hint ladder to the lesson run.
    /// Made for each step by <see cref="ActivityRulesFactory"/> when a lesson is built; every voice
    /// line it can say is prepared then, so a step's answers allocate nothing.
    /// </summary>
    public interface IActivityRules
    {
        /// <summary>The kind.</summary>
        ActivityKind Kind { get; }

        /// <summary>
        /// The language it speaks: every line it says is in it, and so is every choice, except in a kind
        /// that bridges the two languages (<c>TwoSounds</c>), whose choices each have their own.
        /// </summary>
        Language Language { get; }

        /// <summary>What the view draws.</summary>
        ActivityRound Round { get; }

        /// <summary>What the mascot says when the step begins.</summary>
        VoiceSequence Instruction { get; }

        /// <summary>What is said again when she asks to hear it ("listen again"), for the part she is at.</summary>
        VoiceSequence Prompt { get; }

        /// <summary>
        /// What the last answer judged says before any praise or hint (the letter's sound, the word);
        /// null for nothing.
        /// </summary>
        VoiceSequence Response { get; }

        /// <summary>
        /// Every line it can say (its instruction, prompts, responses, hints and shown answers), so the
        /// Unity side prepares their bubble words when the lesson is built.
        /// </summary>
        IReadOnlyList<VoiceSequence> Lines { get; }

        /// <summary>
        /// Sets up the round for the step's beginning (which choices, in which order, from
        /// <paramref name="random"/>) and forgets any earlier answers. Called by the lesson run.
        /// </summary>
        void Begin(IRandom random);

        /// <summary>Judges <paramref name="answer"/>; called by the lesson run.</summary>
        AnswerVerdict Judge(in ActivityAnswer answer);

        /// <summary>
        /// The hint after miss number <paramref name="missNumber"/> (from 1) of the current part,
        /// <paramref name="answer"/> being the miss. Called by the lesson run.
        /// </summary>
        ActivityHint HintFor(int missNumber, in ActivityAnswer answer);

        /// <summary>The answer shown, for the current part (its glow and its line); called by the lesson run.</summary>
        ActivityHint AnswerShown();
    }
}
