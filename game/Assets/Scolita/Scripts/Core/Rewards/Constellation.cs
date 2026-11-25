using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// A planet's constellation in her sky book (a unicorn, a rocket, a whale): one row of
    /// <c>constellations.csv</c>, with its figure's stars and lines in a
    /// <see cref="FrameWidth"/> × <see cref="FrameHeight"/> frame. It is drawn from this data, so a new one
    /// needs no art; the stars a lesson earns light places spread over the figure
    /// (<see cref="ConstellationPlaces"/>).
    /// </summary>
    public sealed class Constellation : IContentItem
    {
        /// <summary>The width of the frame the stars are placed in.</summary>
        public const float FrameWidth = 640f;

        /// <summary>The height of the frame the stars are placed in.</summary>
        public const float FrameHeight = 480f;

        /// <summary>Creates a constellation; called by <see cref="RewardTables"/>, the constellation asset and tests.</summary>
        public Constellation(ContentId id, ContentId unitId, IReadOnlyList<StarPoint> stars, IReadOnlyList<StarLine> lines,
            bool retired, ContentSource source)
        {
            Id = id;
            UnitId = unitId;
            Stars = stars;
            Lines = lines;
            Retired = retired;
            Source = source;
        }

        /// <inheritdoc/>
        public ContentId Id { get; }

        /// <summary>The unit (planet) whose lessons light it.</summary>
        public ContentId UnitId { get; }

        /// <summary>The figure's stars, in the order written.</summary>
        public IReadOnlyList<StarPoint> Stars { get; }

        /// <summary>The figure's lines between its stars.</summary>
        public IReadOnlyList<StarLine> Lines { get; }

        /// <inheritdoc/>
        public bool Retired { get; }

        /// <inheritdoc/>
        public ContentSource Source { get; }
    }
}
