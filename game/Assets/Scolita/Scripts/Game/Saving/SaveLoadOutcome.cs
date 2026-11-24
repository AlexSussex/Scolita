namespace Scolita.Game.Saving
{
    /// <summary>What <see cref="SaveStore.Load{T}"/> found on disk.</summary>
    public enum SaveLoadOutcome
    {
        /// <summary>The file was read; its data is returned.</summary>
        Loaded,

        /// <summary>There is no file yet; the defaults are returned.</summary>
        Missing,

        /// <summary>
        /// The file is damaged (empty, not JSON, or of version 0) and was renamed
        /// <c>&lt;name&gt;.broken.json</c>; the defaults are returned.
        /// </summary>
        SetAside,

        /// <summary>
        /// The file is kept as it is: it couldn't be opened (locked, storage not ready), it was
        /// written by a newer version of the app, it couldn't be set aside, or the save folder can't
        /// be used. The defaults are returned, and the store refuses to save over the file.
        /// </summary>
        Unreadable
    }
}
