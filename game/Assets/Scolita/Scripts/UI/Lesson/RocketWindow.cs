using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// One round window of <c>BuildWord</c>'s rocket (design screen 5): empty and pale blue until a letter fills
    /// it, its rim glowing yellow while it is the next one to fill. The glow's pulse runs on plain arithmetic in
    /// <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class RocketWindow : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Graphic rim;
        [SerializeField] private Graphic glass;
        [SerializeField] private TMP_Text letter;
        [SerializeField] private Color rimColour = new Color(0.624f, 0.702f, 0.839f, 1f);
        [SerializeField] private Color nextRimColour = new Color(1f, 0.851f, 0.239f, 1f);
        [SerializeField] private Color emptyGlass = new Color(0.867f, 0.945f, 1f, 1f);
        [SerializeField] private Color filledGlass = new Color(1f, 0.973f, 0.925f, 1f);
        [SerializeField] private float pulsesPerSecond = 1.2f;
        [SerializeField] private float pulseScale = 0.06f;

        private bool next;

        /// <summary>The letter it shows; empty while it is empty.</summary>
        public string Shown => letter.text;

        /// <summary>Whether it is the next window to fill (its rim glows).</summary>
        public bool IsNext => next;

        /// <summary>
        /// Places it at <paramref name="x"/> on the rocket, <paramref name="size"/> wide, empty; called by
        /// <see cref="BuildWordView"/> when a step begins.
        /// </summary>
        public void Place(float x, float size)
        {
            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
            rect.sizeDelta = new Vector2(size, size);
            letter.fontSize = size * 0.62f;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Shows <paramref name="text"/> in it (empty for none), its rim glowing when it is
        /// <paramref name="isNext"/>; called by <see cref="BuildWordView"/> after every answer.
        /// </summary>
        public void Fill(string text, bool isNext)
        {
            letter.SetText(text);
            next = isNext;
            glass.color = text.Length > 0 ? filledGlass : emptyGlass;
            rim.color = isNext ? nextRimColour : rimColour;
            rim.rectTransform.localScale = Vector3.one;
        }

        private void Update()
        {
            if (!next)
            {
                return;
            }

            float grow = 1f + pulseScale * (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * pulsesPerSecond * 2f * Mathf.PI));
            rim.rectTransform.localScale = new Vector3(grow, grow, 1f);
        }
    }
}
