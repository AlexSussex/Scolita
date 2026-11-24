using UnityEngine;
using UnityEngine.EventSystems;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// <c>Syllables</c>' big drum: a tap on it is a beat, handed to its <see cref="SyllablesView"/>, and it
    /// bounces, its head pressed down a little. Its sound comes with <c>sound-feel</c>. The bounce runs on plain
    /// arithmetic in <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class SyllableDrum : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private RectTransform body;
        [SerializeField] private float bounceSeconds = 0.22f;
        [SerializeField] private float squash = 0.1f;

        private SyllablesView owner;
        private float bounceLeft;

        /// <summary>Whether it is bouncing (a beat just tapped).</summary>
        public bool IsBouncing => bounceLeft > 0f;

        /// <summary>Remembers which view it belongs to; called by <see cref="SyllablesView"/> when it wakes.</summary>
        public void Bind(SyllablesView view)
        {
            owner = view;
        }

        /// <summary>Bounces (a beat); called by <see cref="SyllablesView"/> when a beat is counted.</summary>
        public void Bounce()
        {
            bounceLeft = bounceSeconds;
        }

        /// <summary>
        /// Hands the beat to the view at once, on the finger's touch rather than its lift, so quick beats keep
        /// their rhythm; called by the EventSystem.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            owner.DrumTapped();
        }

        private void OnDisable()
        {
            bounceLeft = 0f;
            body.localScale = Vector3.one;
        }

        private void Update()
        {
            if (bounceLeft <= 0f)
            {
                return;
            }

            bounceLeft = Mathf.Max(0f, bounceLeft - Time.unscaledDeltaTime);
            float press = squash * Mathf.Sin(bounceLeft / bounceSeconds * Mathf.PI);
            body.localScale = new Vector3(1f + press, 1f - press, 1f);
        }
    }
}
