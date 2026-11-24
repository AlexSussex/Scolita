namespace Scolita.Game.Saving
{
    /// <summary>
    /// The data of one save file, read and written by <see cref="SaveStore"/>: it carries the
    /// version of the shape it was written in and brings an older shape up to date.
    /// </summary>
    /// <typeparam name="T">The data type itself.</typeparam>
    public interface ISaveData<out T> where T : class
    {
        /// <summary>The version of the shape this data was read or written in; 1 is the first.</summary>
        int Version { get; }

        /// <summary>The newest shape version this build of the app reads and writes.</summary>
        int CurrentVersion { get; }

        /// <summary>
        /// Returns this data in the current shape, migrated from an older one if needed, with every
        /// value in its range (out-of-range values clamped, unknown ones set to their default).
        /// Called by <see cref="SaveStore.Load{T}"/> after reading a file of a known version.
        /// </summary>
        T Upgraded();
    }
}
