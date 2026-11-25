using Scolita.Core.Parent;
using Scolita.Core.Profiles;
using UnityEngine;

namespace Scolita.Game.Parent
{
    /// <summary>A profile's row in the parent area's list (design screen 10): its avatar, name, kind and summary.</summary>
    public sealed class ParentProfileRow
    {
        /// <summary>Creates a row; called by <see cref="ParentRunner"/> and the tests.</summary>
        public ParentProfileRow(Profile profile, Sprite avatar, ProfileSummary summary, int daysSinceLastPlayed, bool isChosen)
        {
            DaysSinceLastPlayed = daysSinceLastPlayed;
            Profile = profile;
            Avatar = avatar;
            Summary = summary;
            IsChosen = isChosen;
        }

        /// <summary>The profile.</summary>
        public Profile Profile { get; }

        /// <summary>Its avatar's picture.</summary>
        public Sprite Avatar { get; }

        /// <summary>The letters learned and the last day played.</summary>
        public ProfileSummary Summary { get; }

        /// <summary>How many days ago it last played (0 today), or -1 when it never played.</summary>
        public int DaysSinceLastPlayed { get; }

        /// <summary>Whether it is the profile chosen at the top of the area (outlined).</summary>
        public bool IsChosen { get; }
    }
}
