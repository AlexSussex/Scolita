using System;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// The voice's timings, set on the Voice prefab (<c>VoiceHost</c>), never in code: the pause
    /// between a slow word's sounds (and at an ellipsis), and how long the bubble stays when no voice
    /// can say a line.
    /// </summary>
    public readonly struct VoiceTiming
    {
        /// <summary>
        /// Creates the timings; throws an <see cref="ArgumentOutOfRangeException"/> for a negative
        /// one. Called by <c>VoiceHost</c> and the voice tests.
        /// </summary>
        public VoiceTiming(float pauseSeconds, float bubbleSecondsPerCharacter, float bubbleMinimumSeconds)
        {
            if (pauseSeconds < 0f || bubbleSecondsPerCharacter < 0f || bubbleMinimumSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(pauseSeconds), "The voice's timings can't be negative.");
            }

            PauseSeconds = pauseSeconds;
            BubbleSecondsPerCharacter = bubbleSecondsPerCharacter;
            BubbleMinimumSeconds = bubbleMinimumSeconds;
        }

        /// <summary>The pause between a slow word's sounds, and at an ellipsis.</summary>
        public float PauseSeconds { get; }

        /// <summary>How long the bubble stays per character of its text.</summary>
        public float BubbleSecondsPerCharacter { get; }

        /// <summary>The shortest time the bubble stays.</summary>
        public float BubbleMinimumSeconds { get; }

        /// <summary>How long the bubble shows <paramref name="text"/>; called by <see cref="VoicePlanner"/>.</summary>
        public float BubbleSeconds(string text)
        {
            return Math.Max(BubbleMinimumSeconds, text.Length * BubbleSecondsPerCharacter);
        }
    }
}
