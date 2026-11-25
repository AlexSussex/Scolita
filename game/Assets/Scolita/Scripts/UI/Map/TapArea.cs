using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// An invisible area that catches taps and drags (the map's scrolling, the trip's skip, a stop's
    /// 64 dp target) without drawing anything, so it costs no batch.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class TapArea : Graphic
    {
        /// <summary>Draws nothing; called by Unity's UI.</summary>
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
        }
    }
}
