namespace Scolita.Core.Randomness
{
    /// <summary>
    /// The only source of randomness in Core (the order of answer cards, which word comes next),
    /// so a lesson seeded the same way runs the same way. Implemented by <see cref="SeededRandom"/>
    /// and, in tests, by hand-written fakes.
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// Returns a whole number from <paramref name="minInclusive"/> up to, but not including,
        /// <paramref name="maxExclusive"/>. Called by Core's rules and by <c>SeededRandomTests</c>.
        /// </summary>
        int NextInt(int minInclusive, int maxExclusive);
    }
}
