namespace Scolita.Core.Content
{
    /// <summary>
    /// An item in one of the content's catalogs (a <see cref="Letter"/>, a <see cref="Word"/>, a
    /// <see cref="Picture"/>, a <see cref="VoiceLine"/>): found by its stable id, kept when retired.
    /// </summary>
    public interface IContentItem
    {
        /// <summary>The item's stable id.</summary>
        ContentId Id { get; }

        /// <summary>Whether the item is retired: kept (so its id is never reused) but no longer used.</summary>
        bool Retired { get; }

        /// <summary>Where the item was read from (a CSV row, or an asset).</summary>
        ContentSource Source { get; }
    }
}
