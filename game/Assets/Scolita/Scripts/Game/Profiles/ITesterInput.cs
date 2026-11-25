namespace Scolita.Game.Profiles
{
    /// <summary>Where the tester's strip sends "Sari peste pas"; implemented by the Lesson scene's composition root.</summary>
    public interface ITesterInput
    {
        /// <summary>The tester skips the step on screen. Called by the tester's strip.</summary>
        void SkipStep();
    }
}
