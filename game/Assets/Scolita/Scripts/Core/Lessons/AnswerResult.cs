namespace Scolita.Core.Lessons
{
    /// <summary>How an answer counts for her progress (<see cref="AnswerChecked.Result"/>).</summary>
    public enum AnswerResult
    {
        /// <summary>Right, by herself.</summary>
        Right,

        /// <summary>A miss.</summary>
        Wrong,

        /// <summary>A miss while the answer is shown (it glows): not a new miss of what she knows.</summary>
        WrongWithAnswerShown,

        /// <summary>Right with the answer shown: practice, not right (<c>docs/curriculum.md</c> §4.3).</summary>
        Practised
    }
}
