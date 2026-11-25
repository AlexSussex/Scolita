using Scolita.Core.Content;

namespace Scolita.Core.Map
{
    /// <summary>A letter a planet's stops introduce, as its tile shows it; made by <see cref="MapRoute"/>.</summary>
    public readonly struct PlanetLetter
    {
        /// <summary>Creates the tile's letter; called by <see cref="MapRoute"/>.</summary>
        public PlanetLetter(Letter letter, LetterState state)
        {
            Letter = letter;
            State = state;
        }

        /// <summary>The letter.</summary>
        public Letter Letter { get; }

        /// <summary>Met, current or still to come.</summary>
        public LetterState State { get; }
    }
}
