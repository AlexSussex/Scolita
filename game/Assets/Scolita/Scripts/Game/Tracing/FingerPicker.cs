namespace Scolita.Game.Tracing
{
    /// <summary>
    /// Which finger draws on the writing board (<c>feature/tracing</c>): the first finger that comes down
    /// on the board draws until it lifts; any other finger is ignored meanwhile, and a touch as big as a
    /// palm is never one that draws. A plain class, so the rule is tested without touches.
    /// </summary>
    public sealed class FingerPicker
    {
        private const int None = -1;

        /// <summary>The id of the finger drawing; -1 when none is.</summary>
        public int Drawing { get; private set; } = None;

        /// <summary>
        /// A touch <paramref name="touchId"/> came down; it draws when no finger does, it is on the board and
        /// it is not a palm. Returns whether it draws. Called by <see cref="TraceFinger"/> and the tests.
        /// </summary>
        public bool Began(int touchId, bool onBoard, bool isPalm)
        {
            if (Drawing != None || !onBoard || isPalm)
            {
                return false;
            }

            Drawing = touchId;
            return true;
        }

        /// <summary>Whether the touch <paramref name="touchId"/> is the one drawing; called by <see cref="TraceFinger"/> and the tests.</summary>
        public bool Draws(int touchId)
        {
            return Drawing != None && touchId == Drawing;
        }

        /// <summary>
        /// The touch <paramref name="touchId"/> lifted (or was cancelled); returns whether it was the one
        /// drawing, which then stops. Called by <see cref="TraceFinger"/> and the tests.
        /// </summary>
        public bool Lifted(int touchId)
        {
            if (Drawing == None || touchId != Drawing)
            {
                return false;
            }

            Drawing = None;
            return true;
        }

        /// <summary>Forgets the finger drawing (the board is hidden); called by <see cref="TraceFinger"/>.</summary>
        public void Reset()
        {
            Drawing = None;
        }
    }
}
