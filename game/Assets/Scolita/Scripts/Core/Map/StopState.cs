namespace Scolita.Core.Map
{
    /// <summary>A stop (a lesson) on the galaxy map's path.</summary>
    public enum StopState
    {
        /// <summary>Finished at least once: a white dot with a green tick; it opens again.</summary>
        Done,

        /// <summary>The first stop not done: the big yellow dot under the rocket; Go opens it.</summary>
        Next,

        /// <summary>After the next stop: a faint dot; tapping it says "not yet".</summary>
        Ahead
    }
}
