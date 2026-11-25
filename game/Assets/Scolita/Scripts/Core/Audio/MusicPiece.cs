namespace Scolita.Core.Audio
{
    /// <summary>The music a scene plays under everything, streamed and ducked under the voice (<see cref="MusicDucking"/>).</summary>
    public enum MusicPiece
    {
        /// <summary>No music.</summary>
        None,

        /// <summary>The calm space theme: "Who's playing?", the map and the parent area.</summary>
        Space,

        /// <summary>The lighter loop under a lesson.</summary>
        Lesson
    }
}
