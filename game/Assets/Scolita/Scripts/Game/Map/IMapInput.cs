using Scolita.Core.Map;
using Scolita.Game.Lessons;

namespace Scolita.Game.Map
{
    /// <summary>Where the map screen sends her taps; implemented by <see cref="MapRunner"/>.</summary>
    public interface IMapInput : IGuideInput
    {
        /// <summary>She tapped "Hai!" / "Go!": the go stop's lesson opens. Called by the map screen.</summary>
        void Go();

        /// <summary>She tapped <paramref name="stop"/>: its lesson opens, or the mascot says "not yet". Called by the map screen.</summary>
        void TapStop(MapStop stop);

        /// <summary>She tapped <paramref name="planet"/>: a sleepy one says "not yet", another says the greeting again. Called by the map screen.</summary>
        void TapPlanet(MapPlanet planet);

        /// <summary>The trip ended or she skipped it: the rocket has landed. Called by the map screen.</summary>
        void TripEnded();
    }
}
