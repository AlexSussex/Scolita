using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// Tells when the finger leaves a slider (on its handle or its bar), so its value is saved once, not on every step
    /// of the drag. On the voice slider of the parent area.
    /// </summary>
    public sealed class SliderRelease : MonoBehaviour, IPointerUpHandler
    {
        /// <summary>Raised when the finger is lifted; <see cref="ParentAreaScreen"/> listens.</summary>
        public event Action Released;

        /// <summary>The finger is lifted from the slider. Called by the EventSystem.</summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            Released?.Invoke();
        }
    }
}
