using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>A trail she can choose in the trail picker: its id and its picture.</summary>
    public readonly struct TrailChoice
    {
        /// <summary>Creates the choice; called by <see cref="RewardsSession"/>.</summary>
        public TrailChoice(ContentId id, Sprite picture)
        {
            Id = id;
            Picture = picture;
        }

        /// <summary>The trail's id.</summary>
        public ContentId Id { get; }

        /// <summary>Its picture.</summary>
        public Sprite Picture { get; }
    }
}
