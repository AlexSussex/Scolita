using Scolita.Game.Parent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// The parent button on the map (a lock in <c>ParentPlace</c>, top right): pressing it starts the hold, letting go
    /// (or the finger leaving it) stops it; its ring shows how full the hold is. A tap does nothing a child can see
    /// beyond a flicker of the ring. Bound by <see cref="ParentGatePanel"/>.
    /// </summary>
    public sealed class ParentHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private Image ring;

        private IParentGateInput input;
        private bool held;

        /// <summary>How full the ring is drawn, 0 to 1.</summary>
        public float RingFill => ring.fillAmount;

        /// <summary>Sends the presses to <paramref name="gateInput"/>; called by <see cref="ParentGatePanel.Bind"/>.</summary>
        public void Bind(IParentGateInput gateInput)
        {
            input = gateInput;
        }

        /// <summary>Draws the ring <paramref name="progress"/> full; called by <see cref="ParentGatePanel.SetHold"/> every frame.</summary>
        public void SetFill(float progress)
        {
            ring.fillAmount = progress;
        }

        /// <summary>The finger is down on the button: the hold starts. Called by the EventSystem.</summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            held = true;
            input?.Press();
        }

        /// <summary>The finger is lifted: the hold stops. Called by the EventSystem.</summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            Let();
        }

        /// <summary>The finger slid off the button: the hold stops. Called by the EventSystem.</summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            Let();
        }

        private void OnDisable()
        {
            Let();
        }

        private void Let()
        {
            if (!held)
            {
                return;
            }

            held = false;
            input?.Release();
        }
    }
}
