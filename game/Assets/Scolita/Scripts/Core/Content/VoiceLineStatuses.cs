using System.Collections.Generic;
using System.Linq;

namespace Scolita.Core.Content
{
    /// <summary>
    /// Works out each voice line's <see cref="VoiceLineStatus"/> from the clips found in
    /// <c>content/voice/</c>: the one owner of the rule. The importer writes it into the CSV's
    /// <c>status</c> column; the voice line rule fails on a cell that says otherwise.
    /// </summary>
    public static class VoiceLineStatuses
    {
        /// <summary>
        /// The status of every voice line of <paramref name="curriculum"/>, by id. Called by the
        /// importer, the voice line rule and <c>VoiceLineStatusesTests</c>.
        /// </summary>
        public static IReadOnlyDictionary<ContentId, VoiceLineStatus> Of(Curriculum curriculum, IContentFiles files)
        {
            HashSet<ContentId> wordsSlowLines = new HashSet<ContentId>(curriculum.Words.All.Select(word => word.SlowLine.Id));
            return curriculum.VoiceLines.All.ToDictionary(
                line => line.Id,
                line => StatusOf(line, wordsSlowLines.Contains(line.Id), files));
        }

        private static VoiceLineStatus StatusOf(VoiceLine line, bool isAWordsSlowLine, IContentFiles files)
        {
            if (line.HasClip)
            {
                return line.Clips.All(files.ClipExists) ? VoiceLineStatus.Recorded : VoiceLineStatus.Missing;
            }

            return line.Kind == VoiceLineKind.Slow && isAWordsSlowLine
                ? VoiceLineStatus.FromSounds
                : VoiceLineStatus.ToRecord;
        }
    }
}
