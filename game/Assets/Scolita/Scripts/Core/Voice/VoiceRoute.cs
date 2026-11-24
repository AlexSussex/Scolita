namespace Scolita.Core.Voice
{
    /// <summary>How one part of what the voice says is said (<see cref="VoicePlanner"/>).</summary>
    public enum VoiceRoute
    {
        /// <summary>A recorded clip.</summary>
        Clip,

        /// <summary>The tablet's own voice.</summary>
        System,

        /// <summary>No voice can say it: the bubble shows the text for a reading time.</summary>
        Bubble,

        /// <summary>A short silence (between a slow word's sounds, or at an ellipsis).</summary>
        Pause
    }
}
