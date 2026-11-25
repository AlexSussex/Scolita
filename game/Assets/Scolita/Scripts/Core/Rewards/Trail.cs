using Scolita.Core.Content;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// A trail the rocket can wear on the galaxy map, earned by finishing a planet: one row of
    /// <c>trails.csv</c>, whose order is the order they are earned in. Its picture is
    /// <c>Art/Map/trail-&lt;id&gt;</c>, by name.
    /// </summary>
    public sealed class Trail : IContentItem
    {
        /// <summary>Creates a trail; called by <see cref="RewardTables"/>, the trail asset and tests.</summary>
        public Trail(ContentId id, bool retired, ContentSource source)
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
