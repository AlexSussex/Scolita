using System;
using UnityEngine;

namespace Scolita.UI.Map
{
    /// <summary>
    /// The galaxy map's measures (reference pixels at 1280 × 800, y from the map's middle), set on
    /// <c>MapScreen.prefab</c> and read by <see cref="MapLayout"/>.
    /// </summary>
    [Serializable]
    public sealed class MapLayoutSettings
    {
        [SerializeField] private float leftMargin = 250f;
        [SerializeField] private float rightMargin = 360f;
        [SerializeField] private float stopSpacing = 66f;
        [SerializeField] private float planetStretch = 320f;
        [SerializeField] private float planetGap = 60f;
        [SerializeField] private float planetLead = 150f;
        [SerializeField] private float stopsAfterPlanet = 130f;
        [SerializeField] private float planetY = 120f;
        [SerializeField] private float pathY = -120f;
        [SerializeField] private float pathWave = 20f;
        [SerializeField] private float pathWavelength = 760f;
        [SerializeField] private float pathStep = 16f;

        /// <summary>Room before the first planet's stretch (the mascot and its bubble sit there at the start).</summary>
        public float LeftMargin => leftMargin;

        /// <summary>Room after the last planet's stretch.</summary>
        public float RightMargin => rightMargin;

        /// <summary>From one stop's centre to the next (at least 64 dp).</summary>
        public float StopSpacing => stopSpacing;

        /// <summary>The least stretch a planet takes, whatever its stops (centred on the planet).</summary>
        public float PlanetStretch => planetStretch;

        /// <summary>The path between one planet's stretch and the next one's.</summary>
        public float PlanetGap => planetGap;

        /// <summary>From the start of a planet's stretch to its centre: the planet leads, its stops trail after it towards the next one.</summary>
        public float PlanetLead => planetLead;

        /// <summary>From a planet's centre to its first stop, past its letters.</summary>
        public float StopsAfterPlanet => stopsAfterPlanet;

        /// <summary>The planets' centres' height.</summary>
        public float PlanetY => planetY;

        /// <summary>The path's middle height.</summary>
        public float PathY => pathY;

        /// <summary>How far the path waves up and down.</summary>
        public float PathWave => pathWave;

        /// <summary>The length of one wave.</summary>
        public float PathWavelength => pathWavelength;

        /// <summary>The distance between two points of the path's mesh.</summary>
        public float PathStep => pathStep;
    }
}
