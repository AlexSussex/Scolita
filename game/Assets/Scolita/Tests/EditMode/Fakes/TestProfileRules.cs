using Scolita.Core.Profiles;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>The profiles' numbers as the learning rules asset has them, for the profile tests.</summary>
    public static class TestProfileRules
    {
        /// <summary>At most 6 children, names of at most 20 characters; called by the tests.</summary>
        public static ProfileRules Default()
        {
            return new ProfileRules(6, 20);
        }
    }
}
