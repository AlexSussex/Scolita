namespace Scolita.Core.Content
{
    /// <summary>
    /// The most items some activity screens can show, so the content is checked against what the screens
    /// are built for (<see cref="Rules.StepRules"/>) and the views hold enough places: the drum's row of
    /// beats, the rocket's windows, the twins' columns.
    /// </summary>
    public static class ActivityLimits
    {
        /// <summary>The most syllables of a <see cref="ActivityKind.Syllables"/> word (the drum's row of beats).</summary>
        public const int MostSyllables = 6;

        /// <summary>The most letters of a <see cref="ActivityKind.BuildWord"/> word (the rocket's windows).</summary>
        public const int MostWindows = 6;

        /// <summary>The most pairs of a <see cref="ActivityKind.WordTwins"/> step (four words a column).</summary>
        public const int MostPairs = 4;
    }
}
