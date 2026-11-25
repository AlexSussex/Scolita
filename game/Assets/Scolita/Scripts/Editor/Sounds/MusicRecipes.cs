using System;
using Scolita.Core.Audio;
using Scolita.Core.Randomness;

namespace Scolita.Editor.Sounds
{
    /// <summary>
    /// How the two music loops are made (<see cref="SoundMaker"/>): the calm space theme (slow pads in C major, a bass note
    /// per chord, a bell melody wandering on the pentatonic scale, a few far-away twinkles) and the lighter lesson loop
    /// (a soft marimba's arpeggios in F major over a quiet pad). Each loops without a seam (<see cref="Synth.Looped"/>).
    /// </summary>
    public static class MusicRecipes
    {
        private const double SpaceBeat = 60.0 / 64.0;
        private const double LessonBeat = 60.0 / 96.0;
        private const int Bars = 16;
        private const int BarsPerChord = 2;
        private const double TailSeconds = 8.0;

        private static readonly int[][] SpaceChords =
        {
            new[] { 48, 55, 59, 62, 64 },
            new[] { 45, 52, 55, 59, 60 },
            new[] { 41, 48, 52, 55, 57 },
            new[] { 43, 50, 55, 57, 60 }
        };

        private static readonly int[] SpaceScale = { 72, 74, 76, 79, 81, 84, 86 };
        private static readonly double[][] SpaceRhythms =
        {
            new[] { 0.0, 1.5, 3.0 },
            new[] { 0.5, 2.0 },
            new[] { 0.0, 2.0, 2.5 },
            new[] { 1.0, 3.0 }
        };

        private static readonly int[] Twinkles = { 96, 100, 103, 98 };

        private static readonly int[][] LessonChords =
        {
            new[] { 53, 57, 60 },
            new[] { 50, 53, 57 },
            new[] { 46, 50, 53 },
            new[] { 48, 52, 55 }
        };

        private static readonly int[] LessonPattern = { 0, 1, 2, 3, 2, 1, 3, 1 };
        private static readonly int[] LessonScale = { 77, 79, 81, 84, 86 };

        /// <summary>The loop of <paramref name="piece"/>; throws an <see cref="ArgumentOutOfRangeException"/> for <see cref="MusicPiece.None"/> or a piece without a recipe. Called by <see cref="SoundMaker"/>.</summary>
        public static MadeSound For(MusicPiece piece)
        {
            switch (piece)
            {
                case MusicPiece.Space:
                    return new MadeSound("space", Space(), SoundStandard.MusicLufs, SoundStandard.MusicPeakDbfs);
                case MusicPiece.Lesson:
                    return new MadeSound("lesson", Lesson(), SoundStandard.MusicLufs, SoundStandard.MusicPeakDbfs);
                default:
                    throw new ArgumentOutOfRangeException(nameof(piece), piece, "A music piece without a recipe.");
            }
        }

        private static float[] Space()
        {
            double bar = SpaceBeat * 4.0;
            double loop = bar * Bars;
            Synth synth = new Synth(loop + TailSeconds, 31UL);
            IRandom random = new SeededRandom(32UL);
            int step = 2;
            for (int barIndex = 0; barIndex < Bars; barIndex++)
            {
                double barStart = barIndex * bar;
                int[] chord = SpaceChords[barIndex / BarsPerChord % SpaceChords.Length];
                if (barIndex % BarsPerChord == 0)
                {
                    foreach (int note in chord)
                    {
                        synth.Pad(barStart, bar * BarsPerChord - 0.5, Synth.Hz(note), 0.11, 2.0, 3.0);
                    }

                    synth.Sine(barStart, bar * BarsPerChord, Synth.Hz(chord[0] - 12), Synth.Hz(chord[0] - 12), 0.0, 0.22, 0.08, 2.5);
                }

                foreach (double beat in SpaceRhythms[barIndex % SpaceRhythms.Length])
                {
                    step = Math.Max(0, Math.Min(SpaceScale.Length - 1, step + random.NextInt(-2, 3)));
                    synth.Bell(barStart + beat * SpaceBeat, Synth.Hz(SpaceScale[step]), 0.17, 1.1);
                }

                if (barIndex % 2 == 1)
                {
                    synth.Bell(barStart + 3.5 * SpaceBeat, Synth.Hz(Twinkles[barIndex / 2 % Twinkles.Length]), 0.045, 0.7);
                }
            }

            synth.Reverb(0.45, 0.78);
            return synth.Looped(loop);
        }

        private static float[] Lesson()
        {
            double bar = LessonBeat * 4.0;
            double eighth = LessonBeat / 2.0;
            double loop = bar * Bars;
            Synth synth = new Synth(loop + TailSeconds, 41UL);
            IRandom random = new SeededRandom(42UL);
            for (int barIndex = 0; barIndex < Bars; barIndex++)
            {
                double barStart = barIndex * bar;
                int[] chord = LessonChords[barIndex / BarsPerChord % LessonChords.Length];
                if (barIndex % BarsPerChord == 0)
                {
                    foreach (int note in chord)
                    {
                        synth.Pad(barStart, bar * BarsPerChord - 0.3, Synth.Hz(note), 0.06, 0.6, 1.2);
                    }
                }

                synth.Pluck(barStart, Synth.Hz(chord[0] - 12), 0.2, 0.35);
                synth.Pluck(barStart + 2.0 * LessonBeat, Synth.Hz(chord[0] - 12), 0.15, 0.35);
                for (int index = 0; index < LessonPattern.Length; index++)
                {
                    int tone = LessonPattern[index];
                    int note = tone < chord.Length ? chord[tone] + 12 : chord[0] + 24;
                    double accent = index == 0 ? 1.15 : index % 2 == 1 ? 0.8 : 1.0;
                    synth.Pluck(barStart + index * eighth, Synth.Hz(note), 0.13 * accent, 0.22);
                }

                if (barIndex % 4 == 3)
                {
                    int first = random.NextInt(0, LessonScale.Length - 2);
                    for (int note = 0; note < 3; note++)
                    {
                        synth.Bell(barStart + (1.0 + note) * LessonBeat, Synth.Hz(LessonScale[first + note]), 0.1, 0.6);
                    }
                }
            }

            synth.Reverb(0.3, 0.7);
            return synth.Looped(loop);
        }
    }
}
