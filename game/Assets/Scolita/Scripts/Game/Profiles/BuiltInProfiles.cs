using Scolita.Core.Profiles;

namespace Scolita.Game.Profiles
{
    /// <summary>
    /// The profiles the app makes by itself. Until <c>profiles</c> adds the profile list, the app
    /// has one profile, <see cref="FirstChild"/>, made at the first start.
    /// </summary>
    public static class BuiltInProfiles
    {
        /// <summary>The first child's profile, <c>child-1</c>; the profile list keeps it first later.</summary>
        public static readonly ProfileId FirstChild = ProfileId.Create("child-1");
    }
}
