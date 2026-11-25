namespace Scolita.Game.Lessons
{
    /// <summary>
    /// Where the mascot sends a tap on it: it says its last line again. Implemented by the screens' composition
    /// roots (<see cref="LessonRunner"/> through <see cref="ILessonScreenInput"/>, and the map's).
    /// </summary>
    public interface IGuideInput
    {
        /// <summary>She tapped the mascot: it says its last line again. Called by <c>GuideFigure</c>.</summary>
        void RepeatLastLine();
    }
}
