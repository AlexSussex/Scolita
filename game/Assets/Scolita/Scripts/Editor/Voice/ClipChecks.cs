using System;
using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Checks a clip against <see cref="VoiceClipStandard"/>: its loudness, peak, clipping, the
    /// silence at its ends and its length for its kind. Used by Check voice clips and the session
    /// import.
    /// </summary>
    public static class ClipChecks
    {
        /// <summary>Measures <paramref name="audio"/>, a clip of a <paramref name="kind"/> line; called by the kit and <c>ClipChecksTests</c>.</summary>
        public static ClipMeasurement Measure(MonoAudio audio, VoiceLineKind kind)
        {
            double lufs = Loudness.IntegratedLufs(audio);
            double peak = Loudness.PeakDbfs(audio);
            double leading = SilenceFinder.LeadingSilence(audio, VoiceClipStandard.SilenceDbfs);
            double trailing = SilenceFinder.TrailingSilence(audio, VoiceClipStandard.SilenceDbfs);
            List<ClipFlag> flags = new List<ClipFlag>();
            if (lufs < VoiceClipStandard.TargetLufs - VoiceClipStandard.LufsTolerance)
            {
                flags.Add(ClipFlag.TooQuiet);
            }

            if (lufs > VoiceClipStandard.TargetLufs + VoiceClipStandard.LufsTolerance
                || peak > VoiceClipStandard.MaximumPeakDbfs)
            {
                flags.Add(ClipFlag.TooLoud);
            }

            if (Loudness.LongestClippedRun(audio) >= VoiceClipStandard.ClippedRun)
            {
                flags.Add(ClipFlag.Clipped);
            }

            AddSilenceFlags(leading, ClipFlag.ShortSilenceAtStart, ClipFlag.LongSilenceAtStart, flags);
            AddSilenceFlags(trailing, ClipFlag.ShortSilenceAtEnd, ClipFlag.LongSilenceAtEnd, flags);
            double sound = Math.Max(0.0, audio.Seconds - leading - trailing);
            if (sound < VoiceClipStandard.MinimumSoundSeconds)
            {
                flags.Add(ClipFlag.TooShort);
            }

            if (audio.Seconds > VoiceClipStandard.MaximumSeconds(kind))
            {
                flags.Add(ClipFlag.TooLong);
            }

            return new ClipMeasurement(audio.Seconds, lufs, peak, leading, trailing, flags);
        }

        private static void AddSilenceFlags(double silence, ClipFlag tooShort, ClipFlag tooLong, List<ClipFlag> flags)
        {
            if (silence < VoiceClipStandard.MinimumSilenceSeconds)
            {
                flags.Add(tooShort);
            }
            else if (silence > VoiceClipStandard.MaximumSilenceSeconds)
            {
                flags.Add(tooLong);
            }
        }
    }
}
