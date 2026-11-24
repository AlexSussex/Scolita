using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Tracing
{
    /// <summary>
    /// The stardust behind her finger on the writing board (design/README.md): a pool of small sparkles, made
    /// with the prefab, each left where her finger was, drifting a little and fading. Nothing is made,
    /// destroyed, shown or hidden while she draws (the sparkles stay active, see-through when unused), and
    /// fading changes only the sparkles' alpha and transform, so no mesh is rebuilt and nothing allocated.
    /// </summary>
    public sealed class Stardust : MonoBehaviour
    {
        [SerializeField] private Image[] sparkles = new Image[0];
        [SerializeField, Min(0.1f)] private float lifetime = 0.7f;
        [SerializeField] private float drift = 24f;
        [SerializeField, Min(1f)] private float spacing = 18f;

        private float[] ages;
        private Vector2[] drifts;
        private int next;
        private Vector2 lastLeft;
        private bool hasLast;

        /// <summary>
        /// Leaves a sparkle at <paramref name="local"/> (in its own local pixels) once her finger has moved far
        /// enough from the last one; called by the tracing view as she draws.
        /// </summary>
        public void Leave(Vector2 local)
        {
            if (sparkles.Length == 0 || (hasLast && (local - lastLeft).sqrMagnitude < spacing * spacing))
            {
                return;
            }

            EnsureBuffers();
            hasLast = true;
            lastLeft = local;
            Image sparkle = sparkles[next];
            ages[next] = 0f;
            float angle = next * 2.39996f;
            drifts[next] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * drift;
            sparkle.rectTransform.anchoredPosition = local;
            sparkle.rectTransform.localScale = Vector3.one;
            sparkle.canvasRenderer.SetAlpha(1f);
            next = (next + 1) % sparkles.Length;
        }

        /// <summary>Starts a new line of stardust (the finger lifted); called by the tracing view.</summary>
        public void Lift()
        {
            hasLast = false;
        }

        /// <summary>Hides every sparkle; called by the tracing view when its step is shown and hidden.</summary>
        public void Clear()
        {
            EnsureBuffers();
            for (int index = 0; index < sparkles.Length; index++)
            {
                ages[index] = lifetime;
                sparkles[index].canvasRenderer.SetAlpha(0f);
            }

            hasLast = false;
        }

        private void EnsureBuffers()
        {
            if (ages == null)
            {
                ages = new float[sparkles.Length];
                drifts = new Vector2[sparkles.Length];
                for (int index = 0; index < sparkles.Length; index++)
                {
                    ages[index] = lifetime;
                }
            }
        }

        private void Update()
        {
            if (ages == null)
            {
                return;
            }

            float step = Time.unscaledDeltaTime;
            for (int index = 0; index < sparkles.Length; index++)
            {
                if (ages[index] >= lifetime)
                {
                    continue;
                }

                Image sparkle = sparkles[index];
                ages[index] += step;
                float share = ages[index] / lifetime;
                if (share >= 1f)
                {
                    sparkle.canvasRenderer.SetAlpha(0f);
                    continue;
                }

                sparkle.rectTransform.anchoredPosition += drifts[index] * (step / lifetime);
                sparkle.rectTransform.localScale = Vector3.one * (1f - share * 0.6f);
                sparkle.canvasRenderer.SetAlpha(1f - share);
            }
        }
    }
}
