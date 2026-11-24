using Scolita.Core.Content;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// What a voice clip must be (AGENTS.md → "Audio"), in one place: mono at 44.1 kHz, −16 LUFS
    /// integrated, peaks under −1 dBFS, 100 ms of silence at each end, and a length that suits its
    /// kind; with the tolerances the clip check and the session import flag against.
    /// </summary>
    public static class VoiceClipStandard
    {
        /// <summary>The sample rate of every clip.</summary>
        public const int SampleRate = 44100;

        /// <summary>The integrated loudness every clip is levelled to.</summary>
        public const double TargetLufs = -16.0;

        /// <summary>How far from <see cref="TargetLufs"/> a clip may be before it is flagged too quiet or too loud.</summary>
        public const double LufsTolerance = 1.5;

        /// <summary>The highest sample peak a clip may have.</summary>
        public const double MaximumPeakDbfs = -1.0;

        /// <summary>The silence kept at each end of a clip.</summary>
        public const double TargetSilenceSeconds = 0.1;

        /// <summary>Less silence than this at an end is flagged (the start or end may be cut).</summary>
        public const double MinimumSilenceSeconds = 0.05;

        /// <summary>More silence than this at an end is flagged.</summary>
        public const double MaximumSilenceSeconds = 0.25;

        /// <summary>The level under which a 10 ms frame of a levelled clip counts as silence.</summary>
        public const double SilenceDbfs = -45.0;

        /// <summary>A sample at or above this (of full scale) counts as clipped.</summary>
        public const float ClippedLevel = 0.999f;

        /// <summary>This many clipped samples in a row flag the clip as clipped.</summary>
        public const int ClippedRun = 3;

        /// <summary>The fade at each end of a shaped clip, so it never clicks.</summary>
        public const double FadeSeconds = 0.005;

        /// <summary>The shortest a clip's sound may be (silence aside).</summary>
        public const double MinimumSoundSeconds = 0.08;

        /// <summary>
        /// The longest a clip of <paramref name="kind"/> may be, silence included; called by
        /// <see cref="ClipChecks"/>.
        /// </summary>
        public static double MaximumSeconds(VoiceLineKind kind)
        {
            switch (kind)
            {
                case VoiceLineKind.Sound:
                case VoiceLineKind.Name:
                    return 2.5;
                case VoiceLineKind.Word:
                    return 3.0;
                case VoiceLineKind.Syllables:
                case VoiceLineKind.Slow:
                    return 6.0;
                default:
                    return 12.0;
            }
        }
    }
}
