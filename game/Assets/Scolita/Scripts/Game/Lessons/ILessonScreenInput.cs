namespace Scolita.Game.Lessons
{
    /// <summary>Where the lesson screen sends her taps on the mascot and on "go on"; implemented by <see cref="LessonRunner"/>.</summary>
    public interface ILessonScreenInput
    {
        /// <summary>She tapped the mascot: it says its last line again. Called by the lesson screen.</summary>
        void RepeatLastLine();

        /// <summary>She tapped "Mai departe" at the end of the lesson: back to the map. Called by the lesson screen.</summary>
        void Continue();
    }
}
