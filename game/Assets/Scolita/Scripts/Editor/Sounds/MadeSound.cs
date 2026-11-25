namespace Scolita.Editor.Sounds
{
    /// <summary>
    /// One sound made for the app (<see cref="EffectRecipes"/>, <see cref="MusicRecipes"/>): its file name, its samples at
    /// <see cref="Synth.Rate"/>, and the loudness and highest peak <see cref="SoundMaker"/> brings it to.
    /// </summary>
    public sealed class MadeSound
    {
        /// <summary>Names a sound and its level; called by the recipes.</summary>
        public MadeSound(string name, float[] samples, double targetLufs, double highestPeakDbfs)
        {
            Name = name;
            Samples = samples;
            TargetLufs = targetLufs;
            HighestPeakDbfs = highestPeakDbfs;
        }

        /// <summary>The file name, without its extension (<c>sparkle</c>, <c>star-lands-2</c>, <c>space</c>).</summary>
        public string Name { get; }

        /// <summary>The samples, from -1 to 1 before levelling.</summary>
        public float[] Samples { get; }

        /// <summary>The integrated loudness it is brought to, in LUFS.</summary>
        public double TargetLufs { get; }

        /// <summary>The highest sample peak it may reach, in dBFS (it is turned down further if needed).</summary>
        public double HighestPeakDbfs { get; }
    }
}
