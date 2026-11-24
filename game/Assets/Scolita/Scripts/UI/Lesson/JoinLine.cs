using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// A line between two points of its parent (<c>WordTwins</c>' join: the one following her finger, and one per
    /// pair joined): a stretched, turned image, so drawing it allocates nothing.
    /// </summary>
    public sealed class JoinLine : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Image image;

        /// <summary>The line's colour.</summary>
        public Color Colour => image.color;

        /// <summary>
        /// Draws it from <paramref name="from"/> to <paramref name="to"/> (points of its parent) in
        /// <paramref name="colour"/>, and shows it; called by <see cref="WordTwinsView"/>.
        /// </summary>
        public void Connect(Vector2 from, Vector2 to, Color colour)
        {
            Vector2 along = to - from;
            rect.anchoredPosition = (from + to) / 2f;
            rect.sizeDelta = new Vector2(along.magnitude, rect.sizeDelta.y);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(along.y, along.x) * Mathf.Rad2Deg);
            image.color = colour;
            gameObject.SetActive(true);
        }
    }
}
