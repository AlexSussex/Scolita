using Scolita.Core.Profiles;
using UnityEngine;

namespace Scolita.Game.Profiles
{
    /// <summary>A child's card on "Who's playing?" (design screen 9): her avatar, her name (empty for none) and her stars.</summary>
    public readonly struct ProfileCard
    {
        /// <summary>Creates a card; called by <see cref="ProfilesRunner"/> and the tests.</summary>
        public ProfileCard(ProfileId id, Sprite avatar, string name, int stars, bool playedLast)
        {
            Id = id;
            Avatar = avatar;
            Name = name ?? string.Empty;
            Stars = stars;
            PlayedLast = playedLast;
        }

        /// <summary>The child's profile.</summary>
        public ProfileId Id { get; }

        /// <summary>Her avatar's picture.</summary>
        public Sprite Avatar { get; }

        /// <summary>Her name, or an empty text for a profile without one.</summary>
        public string Name { get; }

        /// <summary>The stars she has earned.</summary>
        public int Stars { get; }

        /// <summary>Whether she played last (her card has a yellow ring).</summary>
        public bool PlayedLast { get; }
    }
}
