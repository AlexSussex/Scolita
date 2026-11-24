using UnityEngine;

namespace Scolita.Game.Tracing
{
    /// <summary>
    /// Where <see cref="TraceFinger"/> sends the stroke of the finger drawing on the board, in the board's
    /// local pixels: implemented by the tracing view, which turns them into the letter's units.
    /// </summary>
    public interface ITraceInput
    {
        /// <summary>The finger came down at <paramref name="local"/>; called by <see cref="TraceFinger"/>.</summary>
        void StrokeBegan(Vector2 local);

        /// <summary>The finger moved to <paramref name="local"/>; called by <see cref="TraceFinger"/>.</summary>
        void StrokeMoved(Vector2 local);

        /// <summary>The finger lifted: the stroke is done; called by <see cref="TraceFinger"/>.</summary>
        void StrokeEnded();

        /// <summary>The touch was cancelled by the system (the app paused): the stroke is dropped; called by <see cref="TraceFinger"/>.</summary>
        void StrokeCancelled();
    }
}
