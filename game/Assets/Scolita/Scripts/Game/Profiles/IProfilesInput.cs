using Scolita.Core.Profiles;
using Scolita.Game.Lessons;

namespace Scolita.Game.Profiles
{
    /// <summary>Where the taps on "Who's playing?" go; implemented by <see cref="ProfilesRunner"/>.</summary>
    public interface IProfilesInput : IGuideInput
    {
        /// <summary>She tapped the card of <paramref name="child"/>: her name is said, then her map opens. Called by the screen.</summary>
        void Choose(ProfileId child);
    }
}
