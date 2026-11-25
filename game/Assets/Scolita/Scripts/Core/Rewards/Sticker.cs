using Scolita.Core.Content;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// A sticker for her sticker book (an animal, an instrument, a planet, a space thing): one row of
    /// <c>stickers.csv</c>. Its picture is <c>Art/Stickers/sticker-&lt;id&gt;</c>, by name, so a redrawn
    /// picture needs no change here. A retired sticker is no longer given, but she keeps hers.
    /// </summary>
    public sealed class Sticker : IContentItem
    {
        /// <summary>Creates a sticker; called by <see cref="RewardTables"/>, the sticker asset and tests.</summary>
        public Sticker(ContentId id, bool retired, ContentSource source)
        {
            Id = id;
            Retired = retired;
            Source = source;
        }

        /// <inheritdoc/>
        public ContentId Id { get; }

        /// <inheritdoc/>
        public bool Retired { get; }

        /// <inheritdoc/>
        public ContentSource Source { get; }
    }
}
