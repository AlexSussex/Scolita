using System;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// The voice's timings, set on the Voice prefab (<c>VoiceHost</c>), never in code: the pause
    /// between a slow word's sounds (and at an ellipsis), how long the bubble stays when no voice
    /// can say a line, and how long a line waits for the tablet's voice to get ready.
    /// </summary>
    public readonly struct VoiceTiming
    {
        /// <summary>
        /// Creates the timings; throws an <see cref="ArgumentOutOfRangeException"/> for a negative
        /// one. Called by <c>VoiceHost</c> and the voice tests.
        /// </summary>
        public VoiceTiming(float pauseSeconds, float bubbleSecondsPerCharacter, float bubbleMinimumSeconds,
            float systemVoiceWaitSeconds)
        {
            if (pauseSeconds < 0f || bubbleSecondsPerCharacter < 0f || bubbleMinimumSeconds < 0f || systemVoiceWaitSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(pauseSeconds), "The voice's timings can't be negative.");
            }

            PauseSeconds = pauseSeconds;
            BubbleSecondsPerCharacter = bubbleSecondsPerCharacter;
            BubbleMinimumSeconds = bubbleMinimumSeconds;
            SystemVoiceWaitSeconds = systemVoiceWaitSeconds;
        }

        /// <summary>The pause between a slow word's sounds, and at an ellipsis.</summary>
        public float PauseSeconds { get; }

        /// <summary>How long the bubble stays per character of its text.</summary>
        public float BubbleSecondsPerCharacter { get; }

        /// <summary>The shortest time the bubble stays.</summary>
        public float BubbleMinimumSeconds { get; }

        /// <summary>
        /// How long a line waits for the tablet's voice while it gets ready (<see cref="ISystemVoice.IsStarting"/>)
        /// before it shows in the bubble instead.
        /// </summary>
        public float SystemVoiceWaitSeconds { get; }

        /// <summary>How long the bubble shows <paramref name="text"/>; called by <see cref="VoicePlanner"/>.</summary>
        public float BubbleSeconds(string text)
        {
            return Math.Max(BubbleMinimumSeconds, text.Length * BubbleSecondsPerCharacter);
        }
    }
}
