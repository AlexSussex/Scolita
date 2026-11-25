namespace Scolita.Game.Lessons
{
    /// <summary>The mascot's moods in a lesson and on the map (design/README.md → "The mascot").</summary>
    public enum GuideMood
    {
        /// <summary>Happy: an instruction, a right answer.</summary>
        Happy,

        /// <summary>Thinking: a hint, the answer shown.</summary>
        Thinking,

        /// <summary>Cheering: the end of the lesson.</summary>
        Cheering,
        /// <summary>Sleepy, yawning: the map at the daily limit ("Racheta se odihnește").</summary>
        Sleepy
    }
}
