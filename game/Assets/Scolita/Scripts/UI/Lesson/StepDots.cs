using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The row of dots at the top centre of a lesson (design/README.md): green for the steps done,
    /// yellow for the current one, translucent white for those to come. Its dots are made from a
    /// template the first time a lesson shows them, then reused.
    /// </summary>
    public sealed class StepDots : MonoBehaviour
    {
        [SerializeField] private Image template;
        [SerializeField] private Color done = new Color(0.298f, 0.851f, 0.482f);
        [SerializeField] private Color current = new Color(1f, 0.851f, 0.239f);
        [SerializeField] private Color ahead = new Color(1f, 1f, 1f, 0.35f);

        private readonly List<Image> dots = new List<Image>();

        /// <summary>How many dots are shown.</summary>
        public int Count { get; private set; }

        /// <summary>
        /// Shows <paramref name="count"/> dots, <paramref name="currentStep"/> (from 0) the current one;
        /// a current step equal to the count shows every dot done. Called by the lesson screen.
        /// </summary>
        public void Show(int currentStep, int count)
        {
            while (dots.Count < count)
            {
                Image dot = Instantiate(template, template.transform.parent);
                dots.Add(dot);
            }

            template.gameObject.SetActive(false);
            for (int index = 0; index < dots.Count; index++)
            {
                Image dot = dots[index];
                dot.gameObject.SetActive(index < count);
                dot.color = index < currentStep ? done : index == currentStep ? current : ahead;
            }

            Count = count;
        }

        /// <summary>The colour of dot <paramref name="index"/>; called by the PlayMode tests.</summary>
        public Color ColourOf(int index)
        {
            return dots[index].color;
        }
    }
}
