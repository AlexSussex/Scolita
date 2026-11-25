using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Parent
{
    /// <summary>
    /// The parent area's "Vocile înregistrate": per language, the lines recorded out of the lines to record
    /// (retired lines and lines said by their word's sounds left out); the rest is said by the tablet's voice.
    /// </summary>
    public static class VoiceStatus
    {
        /// <summary>
        /// The status of <paramref name="lines"/> in Romanian, then English. Called by <see cref="ParentReport"/>
        /// and the tests.
        /// </summary>
        public static IReadOnlyList<LanguageVoiceStatus> Of(ItemCatalog<VoiceLine> lines)
        {
            return new[] { StatusOf(lines, Language.Romanian), StatusOf(lines, Language.English) };
        }

        private static LanguageVoiceStatus StatusOf(ItemCatalog<VoiceLine> lines, Language language)
        {
            int recorded = 0;
            int toRecord = 0;
            foreach (VoiceLine line in lines.All)
            {
                if (line.Retired || line.Language != language || line.Status == VoiceLineStatus.FromSounds)
                {
                    continue;
                }

                toRecord++;
                if (line.Status == VoiceLineStatus.Recorded)
                {
                    recorded++;
                }
            }

            return new LanguageVoiceStatus(language, recorded, toRecord);
        }
    }
}
