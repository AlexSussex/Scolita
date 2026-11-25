using System;
using Scolita.Core.Audio;

namespace Scolita.Editor.Sounds
{
    /// <summary>
    /// How each effect is made (<see cref="SoundMaker"/>): bells and soft tones in the app's bright, calm world, the miss
    /// a soft low "boing", nothing harsh. A cue may have several takes, played in turn (the stars land a step higher each).
    /// </summary>
    public static class EffectRecipes
    {
        /// <summary>The takes of <paramref name="cue"/>; throws an <see cref="ArgumentOutOfRangeException"/> for a cue without a recipe. Called by <see cref="SoundMaker"/>.</summary>
        public static MadeSound[] For(SoundCue cue)
        {
            switch (cue)
            {
                case SoundCue.Tap:
                    return One("tap", Tap(), SoundStandard.SoftEffectLufs);
                case SoundCue.Sparkle:
                    return One("sparkle", Sparkle(), SoundStandard.EffectLufs);
                case SoundCue.StepDone:
                    return One("step-done", StepDone(), SoundStandard.EffectLufs);
                case SoundCue.Boing:
                    return One("boing", Boing(), SoundStandard.EffectLufs);
                case SoundCue.Glow:
                    return One("glow", Glow(), SoundStandard.SoftEffectLufs);
                case SoundCue.Drum:
                    return One("drum", Drum(), SoundStandard.EffectLufs);
                case SoundCue.NotYet:
                    return One("not-yet", NotYet(), SoundStandard.SoftEffectLufs);
                case SoundCue.TakeOff:
                    return One("take-off", TakeOff(), SoundStandard.LongEffectLufs);
                case SoundCue.Whoosh:
                    return One("whoosh", Whoosh(), SoundStandard.LongEffectLufs);
                case SoundCue.Landing:
                    return One("landing", Landing(), SoundStandard.EffectLufs);
                case SoundCue.StarLands:
                    return new[]
                    {
                        Effect("star-lands-1", StarLands(96), SoundStandard.EffectLufs),
                        Effect("star-lands-2", StarLands(98), SoundStandard.EffectLufs),
                        Effect("star-lands-3", StarLands(100), SoundStandard.EffectLufs)
                    };
                case SoundCue.GiftOpens:
                    return One("gift-opens", GiftOpens(), SoundStandard.EffectLufs);
                case SoundCue.StickerPlaced:
                    return One("sticker-placed", StickerPlaced(), SoundStandard.EffectLufs);
                case SoundCue.CardTapped:
                    return One("card-tapped", CardTapped(), SoundStandard.EffectLufs);
                case SoundCue.BookOpens:
                    return One("book-opens", BookOpens(), SoundStandard.SoftEffectLufs);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cue), cue, "A sound cue without a recipe.");
            }
        }

        private static MadeSound[] One(string name, Synth made, double lufs)
        {
            return new[] { Effect(name, made, lufs) };
        }

        private static MadeSound Effect(string name, Synth made, double lufs)
        {
            made.FadeOutEnd(0.01);
            return new MadeSound(name, made.Samples, lufs, SoundStandard.EffectPeakDbfs);
        }

        private static Synth Tap()
        {
            Synth synth = new Synth(0.09, 11UL);
            synth.Sine(0.0, 0.09, 1400.0, 1250.0, 0.03, 0.8, 0.001, 0.014);
            synth.Sine(0.0, 0.05, 2800.0, 2800.0, 0.0, 0.2, 0.001, 0.008);
            return synth;
        }

        private static Synth Sparkle()
        {
            Synth synth = new Synth(0.8, 12UL);
            synth.Bell(0.0, Synth.Hz(88), 0.5, 0.3);
            synth.Bell(0.06, Synth.Hz(92), 0.5, 0.3);
            synth.Bell(0.12, Synth.Hz(95), 0.55, 0.35);
            synth.Bell(0.19, Synth.Hz(100), 0.25, 0.3);
            synth.Reverb(0.25, 0.6);
            return synth;
        }

        private static Synth StepDone()
        {
            Synth synth = new Synth(1.3, 13UL);
            synth.Bell(0.0, Synth.Hz(84), 0.45, 0.45);
            synth.Bell(0.08, Synth.Hz(88), 0.45, 0.45);
            synth.Bell(0.16, Synth.Hz(91), 0.45, 0.5);
            synth.Bell(0.26, Synth.Hz(96), 0.5, 0.6);
            synth.Reverb(0.3, 0.65);
            return synth;
        }

        private static Synth Boing()
        {
            Synth synth = new Synth(0.5, 14UL);
            synth.Sine(0.0, 0.5, 300.0, 165.0, 0.3, 0.8, 0.012, 0.16, 0.05, 11.0);
            synth.Sine(0.0, 0.3, 600.0, 330.0, 0.3, 0.12, 0.012, 0.08, 0.05, 11.0);
            return synth;
        }

        private static Synth Glow()
        {
            Synth synth = new Synth(1.2, 15UL);
            synth.Sine(0.0, 1.2, Synth.Hz(81), Synth.Hz(81), 0.0, 0.45, 0.04, 0.35);
            synth.Sine(0.0, 1.2, Synth.Hz(88), Synth.Hz(88), 0.0, 0.3, 0.06, 0.3);
            synth.Bell(0.02, Synth.Hz(93), 0.12, 0.3);
            synth.Reverb(0.3, 0.65);
            return synth;
        }

        private static Synth Drum()
        {
            Synth synth = new Synth(0.4, 16UL);
            synth.Sine(0.0, 0.4, 170.0, 88.0, 0.1, 0.9, 0.002, 0.13);
            synth.Sine(0.0, 0.2, 340.0, 176.0, 0.1, 0.15, 0.002, 0.05);
            synth.Noise(0.0, 0.04, 0.35, 1400.0, 900.0, 600.0, 0.5, 0.002);
            return synth;
        }

        private static Synth NotYet()
        {
            Synth synth = new Synth(0.45, 17UL);
            synth.Sine(0.0, 0.18, Synth.Hz(67), Synth.Hz(67), 0.0, 0.6, 0.01, 0.1);
            synth.Sine(0.16, 0.29, Synth.Hz(64), Synth.Hz(64), 0.0, 0.6, 0.01, 0.12);
            return synth;
        }

        private static Synth TakeOff()
        {
            Synth synth = new Synth(1.4, 18UL);
            synth.Noise(0.0, 1.3, 0.9, 220.0, 2600.0, 1200.0, 0.7, 0.35);
            synth.Sine(0.0, 1.2, 70.0, 55.0, 1.0, 0.3, 0.2, 0.5);
            synth.Sine(0.1, 1.0, 240.0, 900.0, 0.8, 0.12, 0.3, 0.4);
            synth.Bell(0.75, Synth.Hz(96), 0.12, 0.3);
            synth.Reverb(0.2, 0.6);
            return synth;
        }

        private static Synth Whoosh()
        {
            Synth synth = new Synth(1.2, 19UL);
            synth.Noise(0.0, 1.1, 0.9, 400.0, 3200.0, 500.0, 0.45, 0.45);
            synth.Reverb(0.2, 0.6);
            return synth;
        }

        private static Synth Landing()
        {
            Synth synth = new Synth(1.0, 20UL);
            synth.Sine(0.0, 0.35, 130.0, 60.0, 0.15, 0.6, 0.004, 0.12);
            synth.Bell(0.08, Synth.Hz(91), 0.35, 0.35);
            synth.Bell(0.15, Synth.Hz(88), 0.35, 0.35);
            synth.Bell(0.22, Synth.Hz(84), 0.4, 0.45);
            synth.Reverb(0.25, 0.6);
            return synth;
        }

        private static Synth StarLands(int note)
        {
            Synth synth = new Synth(0.8, 21UL + (ulong)note);
            synth.Bell(0.0, Synth.Hz(note), 0.55, 0.4);
            synth.Bell(0.0, Synth.Hz(note - 12), 0.2, 0.3);
            synth.Reverb(0.3, 0.65);
            return synth;
        }

        private static Synth GiftOpens()
        {
            Synth synth = new Synth(1.2, 22UL);
            synth.Sine(0.0, 0.08, 520.0, 260.0, 0.05, 0.5, 0.002, 0.03);
            int[] notes = { 84, 86, 88, 91, 93, 96 };
            for (int index = 0; index < notes.Length; index++)
            {
                synth.Bell(0.05 + index * 0.05, Synth.Hz(notes[index]), 0.35, 0.35);
            }

            synth.Reverb(0.3, 0.65);
            return synth;
        }

        private static Synth StickerPlaced()
        {
            Synth synth = new Synth(0.22, 23UL);
            synth.Sine(0.0, 0.22, 720.0, 340.0, 0.06, 0.8, 0.002, 0.05);
            synth.Noise(0.0, 0.015, 0.15, 3000.0, 3000.0, 3000.0, 0.5, 0.001);
            return synth;
        }

        private static Synth CardTapped()
        {
            Synth synth = new Synth(1.0, 24UL);
            synth.Bell(0.0, Synth.Hz(79), 0.5, 0.4);
            synth.Bell(0.11, Synth.Hz(84), 0.55, 0.5);
            synth.Reverb(0.3, 0.65);
            return synth;
        }

        private static Synth BookOpens()
        {
            Synth synth = new Synth(0.5, 25UL);
            synth.Noise(0.0, 0.28, 0.6, 900.0, 2200.0, 700.0, 0.35, 0.07);
            synth.Bell(0.1, Synth.Hz(88), 0.15, 0.25);
            return synth;
        }
    }
}
