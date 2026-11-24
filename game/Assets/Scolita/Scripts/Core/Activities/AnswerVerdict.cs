namespace Scolita.Core.Activities
{
    /// <summary>
    /// What an activity's rules say about one answer (<see cref="IActivityRules.Judge"/>); the lesson
    /// run turns it into the feedback she gets, with the tries and the hint ladder.
    /// </summary>
    public enum AnswerVerdict
    {
        /// <summary>A tap that is not an answer (hearing the letter again): never a miss.</summary>
        Neutral,

        /// <summary>A wrong answer: a miss.</summary>
        Wrong,

        /// <summary>A right answer that finishes one part of the step; more parts are to come.</summary>
        Right,

        /// <summary>A right answer that finishes the step.</summary>
        Done
    }
}
