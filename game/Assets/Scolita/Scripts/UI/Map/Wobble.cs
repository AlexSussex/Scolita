using UnityEngine;

namespace Scolita.UI.Map
{
    /// <summary>
    /// A gentle side-to-side wobble (a "not yet", never a scolding: AGENTS.md → "Kind feedback"), as a
    /// rotation that fades out, as the lesson's cards wobble on a miss. Made once by the planet and stop
    /// views from their own measures and moved in their <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class Wobble
    {
        private readonly float seconds;
        private readonly float degrees;
        private readonly float wobblesPerSecond;
        private float startedAt;
        private bool running;

        /// <summary>
        /// A wobble of <paramref name="wobbleDegrees"/> for <paramref name="wobbleSeconds"/>,
        /// <paramref name="perSecond"/> times a second. Called by <see cref="PlanetView"/> and <see cref="StopView"/>.
        /// </summary>
        public Wobble(float wobbleSeconds, float wobbleDegrees, float perSecond)
        {
            seconds = wobbleSeconds;
            degrees = wobbleDegrees;
            wobblesPerSecond = perSecond;
        }

        /// <summary>Starts it again at <paramref name="now"/> (unscaled seconds); called by the views on a "not yet".</summary>
        public void Start(float now)
        {
            startedAt = now;
            running = true;
        }

        /// <summary>
        /// The rotation at <paramref name="now"/>; false when there is nothing to change (it has settled and
        /// its last frame, at 0°, was already given). Called every frame by the views.
        /// </summary>
        public bool TryGetAngle(float now, out float angle)
        {
            float elapsed = now - startedAt;
            if (!running || elapsed >= seconds)
            {
                bool lastFrame = running;
                running = false;
                angle = 0f;
                return lastFrame;
            }

            angle = degrees * (1f - elapsed / seconds) * Mathf.Sin(elapsed * wobblesPerSecond * 2f * Mathf.PI);
            return true;
        }
    }
}
