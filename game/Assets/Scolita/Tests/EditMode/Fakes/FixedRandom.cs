using Scolita.Core.Randomness;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// An <see cref="IRandom"/> that always gives the highest number it may, so a shuffle keeps the
    /// order it was given and a test knows where every choice is. Used by the activity and lesson tests.
    /// </summary>
    public sealed class FixedRandom : IRandom
    {
        /// <summary>Returns one below <paramref name="maxExclusive"/>; called by the code under test.</summary>
        public int NextInt(int minInclusive, int maxExclusive)
        {
            return maxExclusive - 1;
        }
    }
}
