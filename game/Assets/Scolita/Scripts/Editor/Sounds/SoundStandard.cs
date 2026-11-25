namespace Scolita.Editor.Sounds
{
    /// <summary>
    /// How loud the app's own sounds are, beside the voice's −16 LUFS (AGENTS.md → "Audio"), so the voice always stands
    /// above them before any ducking: the effects about 6 dB under it (the soft ones 10 dB), the music 14 dB under it.
    /// Used by <see cref="SoundMaker"/> and checked by <c>SoundBankTests</c>.
    /// </summary>
    public static class SoundStandard
    {
        /// <summary>An effect's integrated loudness.</summary>
        public const double EffectLufs = -22.0;

        /// <summary>A soft effect's (a tap, "not yet", a page, the answer's glow).</summary>
        public const double SoftEffectLufs = -26.0;

        /// <summary>An effect made of noise that lasts longer (the take-off, the whoosh).</summary>
        public const double LongEffectLufs = -24.0;

        /// <summary>The music's integrated loudness.</summary>
        public const double MusicLufs = -30.0;

        /// <summary>An effect's highest sample peak.</summary>
        public const double EffectPeakDbfs = -3.0;

        /// <summary>The music's highest sample peak.</summary>
        public const double MusicPeakDbfs = -6.0;

        /// <summary>How far under its target a sound may end up, when its peak had to be turned down.</summary>
        public const double BelowTargetAllowed = 6.0;

        /// <summary>How far over its target a sound may be (rounding to 16-bit samples).</summary>
        public const double AboveTargetAllowed = 0.5;
    }
}
