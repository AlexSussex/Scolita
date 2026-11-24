using Scolita.Core.Activities;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// Where an activity's view sends what she does (AGENTS.md, seam 3): her answers and her asking to
    /// hear the question again. Implemented by <see cref="LessonRunner"/>, which passes them to Core.
    /// </summary>
    public interface IActivityInput
    {
        /// <summary>She answered; called by the activity views on a tap.</summary>
        void Answer(ActivityAnswer answer);

        /// <summary>She asked to hear the question again ("listen again"); called by the activity views.</summary>
        void RepeatPrompt();
    }
}
