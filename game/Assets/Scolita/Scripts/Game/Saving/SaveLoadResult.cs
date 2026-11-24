namespace Scolita.Game.Saving
{
    /// <summary>
    /// The data <see cref="SaveStore.Load{T}"/> returns, with what it found on disk.
    /// </summary>
    /// <typeparam name="T">The save file's data type.</typeparam>
    public readonly struct SaveLoadResult<T> where T : class
    {
        /// <summary>
        /// Creates a result; called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public SaveLoadResult(T data, SaveLoadOutcome outcome)
        {
            Data = data;
            Outcome = outcome;
        }

        /// <summary>The data read from the file, or the defaults when it was missing or set aside.</summary>
        public T Data { get; }

        /// <summary>Whether the file was read, missing, or unreadable and set aside.</summary>
        public SaveLoadOutcome Outcome { get; }
    }
}
