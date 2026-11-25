using System.Collections.Generic;
using UnityEngine;

namespace Scolita.UI.Map
{
    /// <summary>
    /// Where the galaxy map's things go, in the map's scrolling content (reference pixels, x from its left
    /// edge, y from its middle): the rainbow path is one wave across the whole map; each planet stands near the
    /// start of its stretch of path, its letters below it, and its stops (at least 64 dp apart, AGENTS.md →
    /// "Big touch targets") trail after it on the path, clear of its letters, so the rocket on a stop never
    /// hides them. Worked out once when the
    /// map opens, by <see cref="MapScreen"/>.
    /// </summary>
    public sealed class MapLayout
    {
        private readonly MapLayoutSettings settings;
        private readonly List<Vector2> planetCentres = new List<Vector2>();
        private readonly List<Vector2> stopPositions = new List<Vector2>();

        /// <summary>
        /// Lays out planets with <paramref name="stopsPerPlanet"/> stops each, by <paramref name="layoutSettings"/>.
        /// Called by <see cref="MapScreen.Show"/>.
        /// </summary>
        public MapLayout(IReadOnlyList<int> stopsPerPlanet, MapLayoutSettings layoutSettings)
        {
            settings = layoutSettings;
            float left = settings.LeftMargin;
            foreach (int stops in stopsPerPlanet)
            {
                float planetX = left + settings.PlanetLead;
                float firstStop = planetX + settings.StopsAfterPlanet;
                for (int stop = 0; stop < stops; stop++)
                {
                    float x = firstStop + stop * settings.StopSpacing;
                    stopPositions.Add(new Vector2(x, PathY(x)));
                }

                planetCentres.Add(new Vector2(planetX, settings.PlanetY));
                float lastStop = firstStop + (stops - 1) * settings.StopSpacing;
                float end = Mathf.Max(lastStop + settings.StopSpacing * 0.5f, planetX + settings.PlanetStretch * 0.5f);
                left = end + settings.PlanetGap;
            }

            Width = left - settings.PlanetGap + settings.RightMargin;
        }

        /// <summary>The map's whole width.</summary>
        public float Width { get; }

        /// <summary>Each planet's centre, in map order.</summary>
        public IReadOnlyList<Vector2> PlanetCentres => planetCentres;

        /// <summary>Each stop's centre on the path, in the route's order.</summary>
        public IReadOnlyList<Vector2> StopPositions => stopPositions;

        /// <summary>
        /// The path's centre line from the map's left edge to its right, a point every
        /// <see cref="MapLayoutSettings.PathStep"/>; called by <see cref="MapScreen.Show"/> for the path's mesh.
        /// </summary>
        public Vector2[] PathPoints()
        {
            int count = Mathf.CeilToInt(Width / settings.PathStep) + 1;
            Vector2[] points = new Vector2[count];
            for (int index = 0; index < count; index++)
            {
                float x = Mathf.Min(index * settings.PathStep, Width);
                points[index] = new Vector2(x, PathY(x));
            }

            return points;
        }

        private float PathY(float x)
        {
            return settings.PathY + settings.PathWave * Mathf.Sin(x * 2f * Mathf.PI / settings.PathWavelength);
        }
    }
}
