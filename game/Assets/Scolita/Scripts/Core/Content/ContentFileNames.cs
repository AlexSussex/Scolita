using System.Collections.Generic;

namespace Scolita.Core.Content
{
    /// <summary>The names of the content's CSV files in <c>content/</c>, each documented in <c>content/README.md</c>.</summary>
    public static class ContentFileNames
    {
        /// <summary>The letters.</summary>
        public const string Letters = "letters.csv";

        /// <summary>The words, in both languages.</summary>
        public const string Words = "words.csv";

        /// <summary>The pictures.</summary>
        public const string Pictures = "pictures.csv";

        /// <summary>Everything the app says.</summary>
        public const string VoiceLines = "voice-lines.csv";

        /// <summary>The units (planets).</summary>
        public const string Units = "units.csv";

        /// <summary>The lessons, one row each.</summary>
        public const string Lessons = "lessons.csv";

        /// <summary>The lessons' steps, one row each.</summary>
        public const string Steps = "steps.csv";

        /// <summary>The stickers for her sticker book (<c>Scolita.Core.Rewards</c>).</summary>
        public const string Stickers = "stickers.csv";

        /// <summary>The planets' constellations in her sky book (<c>Scolita.Core.Rewards</c>).</summary>
        public const string Constellations = "constellations.csv";

        /// <summary>The rocket's trails, in the order they are earned (<c>Scolita.Core.Rewards</c>).</summary>
        public const string Trails = "trails.csv";

        /// <summary>The profile pictures a child chooses from, in the order offered (<c>Scolita.Core.Profiles</c>).</summary>
        public const string Avatars = "avatars.csv";

        /// <summary>Every file, in the order they are read; called by the importer, <c>ContentTables</c> and <c>RewardTables</c>.</summary>
        public static IReadOnlyList<string> All { get; } = new[]
        {
            Letters, Words, Pictures, VoiceLines, Units, Lessons, Steps, Stickers, Constellations, Trails, Avatars
        };
    }
}
