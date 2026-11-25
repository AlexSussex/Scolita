using System;
using Scolita.Core.Map;
using UnityEngine;

namespace Scolita.UI.Map
{
    /// <summary>
    /// One planet colour's three whole pictures from <c>look</c> (<c>Art/Planets/planet-&lt;colour&gt;-&lt;state&gt;</c>):
    /// sleepy (with its "z z"), awake (with its ring) and beaming (with its sparkle), each with its shadow drawn in.
    /// The map's colours repeat in map order (gold, pink, green, purple, blue), set on <c>MapScreen.prefab</c>.
    /// </summary>
    [Serializable]
    public sealed class PlanetLook
    {
        [SerializeField] private Sprite sleepy;
        [SerializeField] private Sprite awake;
        [SerializeField] private Sprite beaming;

        /// <summary>The picture for <paramref name="state"/>; called by <see cref="PlanetView.Show"/> and the trip.</summary>
        public Sprite PictureFor(PlanetState state)
        {
            switch (state)
            {
                case PlanetState.Awake:
                    return awake;
                case PlanetState.Beaming:
                    return beaming;
                default:
                    return sleepy;
            }
        }
    }
}
