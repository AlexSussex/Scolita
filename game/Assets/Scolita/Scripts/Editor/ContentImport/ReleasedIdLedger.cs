using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Scolita.Core.Content;
using Scolita.Core.Ids;
using Scolita.Core.Rewards;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// The ledger of released ids, <c>content/released-ids.txt</c>: every stable id ever released, one
    /// <c>&lt;file&gt; &lt;id&gt;</c> line each (<c>words.csv ro-mama</c>), under a <c># &lt;version&gt;</c>
    /// line for the release that first shipped it; <c>#</c> lines are comments. An id in the ledger
    /// must stay in its CSV (retired, never deleted), so a row deleted together with its asset is
    /// still caught (<see cref="CheckNoneDeleted"/>). The close-out adds each release's new ids
    /// (<see cref="ReleasedIdsRecorder"/>).
    /// </summary>
    public sealed class ReleasedIdLedger
    {
        /// <summary>The ledger's file name in <c>content/</c>.</summary>
        public const string FileName = "released-ids.txt";

        private const char Comment = '#';
        private const string ByteOrderMark = "\uFEFF";

        private static readonly string[] IdFiles =
        {
            ContentFileNames.Letters, ContentFileNames.Words, ContentFileNames.Pictures,
            ContentFileNames.VoiceLines, ContentFileNames.Units, ContentFileNames.Lessons,
            ContentFileNames.Stickers, ContentFileNames.Constellations, ContentFileNames.Trails, ContentFileNames.Avatars
        };

        private static readonly char[] Spaces = { ' ', '\t' };

        private static readonly Regex ReleaseLine = new Regex(
            @"^#\s*((0|[1-9][0-9]?)\.(0|[1-9][0-9]?)\.(0|[1-9][0-9]?))\z", RegexOptions.CultureInvariant);

        private readonly string text;
        private readonly IReadOnlyList<ReleasedId> ids;
        private readonly AppVersion? latestRelease;

        private ReleasedIdLedger(string text, IReadOnlyList<ReleasedId> ids, AppVersion? latestRelease)
        {
            this.text = text;
            this.ids = ids;
            this.latestRelease = latestRelease;
        }

        /// <summary>Every id in the ledger, in its order.</summary>
        public IReadOnlyList<ReleasedId> Ids => ids;

        /// <summary>The highest version among its <c># &lt;version&gt;</c> lines; none for a ledger without one.</summary>
        public AppVersion? LatestRelease => latestRelease;

        /// <summary>
        /// Reads the ledger's <paramref name="text"/> (<c>null</c> when the file is missing, which is an
        /// error) and the latest of its <c># &lt;version&gt;</c> lines, reporting a missing file, a
        /// malformed line, an unknown file or a repeated id into <paramref name="problems"/> at its row. Called by <see cref="ContentImportPlan"/>,
        /// <see cref="ReleasedIdsRecorder"/> and <c>ReleasedIdLedgerTests</c>.
        /// </summary>
        public static ReleasedIdLedger Parse(string text, ContentProblems problems)
        {
            if (text == null)
            {
                problems.Error(new ContentSource(FileName, 0), "the ledger of released ids is missing: restore "
                    + "content/" + FileName + " from git (content/README.md, \"The ledger of released ids\").");
                return new ReleasedIdLedger(string.Empty, new ReleasedId[0], null);
            }

            List<ReleasedId> read = new List<ReleasedId>();
            HashSet<string> seen = new HashSet<string>();
            AppVersion? latest = null;
            string[] lines = text.Split('\n');
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index].Replace(ByteOrderMark, string.Empty).Trim();
                Match release = ReleaseLine.Match(line);
                if (release.Success)
                {
                    AppVersion version = AppVersion.Parse(release.Groups[1].Value);
                    latest = latest == null || version.VersionCode > latest.Value.VersionCode ? version : latest;
                    continue;
                }

                if (line.Length == 0 || line[0] == Comment)
                {
                    continue;
                }

                ContentSource source = new ContentSource(FileName, index + 1);
                string[] parts = line.Split(Spaces, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2 || !IdFiles.Contains(parts[0]) || !StableIdText.IsValid(parts[1]))
                {
                    problems.Error(source, "\"" + line + "\" is not a ledger line: write \"<file> <id>\", such as "
                        + "\"words.csv ro-mama\", with the file one of " + string.Join(", ", IdFiles) + ".");
                    continue;
                }

                ReleasedId id = new ReleasedId(parts[0], parts[1], index + 1);
                if (!seen.Add(id.ToString()))
                {
                    problems.Error(source, "\"" + line + "\" is in the ledger twice: each released id is written once.");
                    continue;
                }

                read.Add(id);
            }

            return new ReleasedIdLedger(text, read, latest);
        }

        /// <summary>
        /// Reports an error into <paramref name="problems"/>, at the CSV it belongs to, for every id in
        /// the ledger that <paramref name="curriculum"/> and <paramref name="rewards"/> no longer have (its row was
        /// deleted, not retired). Called by <see cref="ContentImportPlan.Make"/> (for the importer and
        /// <see cref="ReleasedIdsRecorder"/>) and tests.
        /// </summary>
        public void CheckNoneDeleted(Curriculum curriculum, RewardContent rewards, ContentProblems problems)
        {
            HashSet<string> present = new HashSet<string>(ContentIds(curriculum, rewards).Select(id => id.ToString()));
            foreach (ReleasedId released in ids.Where(released => !present.Contains(released.ToString())))
            {
                problems.Error(new ContentSource(released.File, 0), "the id " + released.Id + " was released (" + FileName
                    + ", row " + released.Row + "), but its row is gone: ids are never deleted (her progress names "
                    + "them), so put its row back and mark it retired.");
            }
        }

        /// <summary>
        /// The ids of <paramref name="curriculum"/> and <paramref name="rewards"/> not in the ledger yet, by file then in
        /// their CSV's order. Called by <see cref="ReleasedIdsRecorder"/> and tests.
        /// </summary>
        public IReadOnlyList<ReleasedId> NewIds(Curriculum curriculum, RewardContent rewards)
        {
            HashSet<string> released = new HashSet<string>(ids.Select(id => id.ToString()));
            return ContentIds(curriculum, rewards)
                .Where(id => !released.Contains(id.ToString()))
                .ToArray();
        }

        /// <summary>
        /// The ledger's text with <paramref name="newIds"/> added at its end under a
        /// <c># <paramref name="version"/></c> line; the text unchanged when there are none. Called by
        /// <see cref="ReleasedIdsRecorder"/> and tests.
        /// </summary>
        public string WithIds(string version, IReadOnlyList<ReleasedId> newIds)
        {
            if (newIds.Count == 0)
            {
                return text;
            }

            StringBuilder added = new StringBuilder(text);
            if (added.Length > 0 && added[added.Length - 1] != '\n')
            {
                added.Append('\n');
            }

            if (added.Length > 0)
            {
                added.Append('\n');
            }

            added.Append(Comment).Append(' ').Append(version).Append('\n');
            foreach (ReleasedId id in newIds)
            {
                added.Append(id).Append('\n');
            }

            return added.ToString();
        }

        private static IEnumerable<ReleasedId> ContentIds(Curriculum curriculum, RewardContent rewards)
        {
            return curriculum.Letters.All.Select(letter => Unwritten(ContentFileNames.Letters, letter.Id))
                .Concat(curriculum.Words.All.Select(word => Unwritten(ContentFileNames.Words, word.Id)))
                .Concat(curriculum.Pictures.All.Select(picture => Unwritten(ContentFileNames.Pictures, picture.Id)))
                .Concat(curriculum.VoiceLines.All.Select(line => Unwritten(ContentFileNames.VoiceLines, line.Id)))
                .Concat(curriculum.Domains.SelectMany(domain => domain.Units).Select(unit => Unwritten(ContentFileNames.Units, unit.Id)))
                .Concat(curriculum.AllLessons.Select(lesson => Unwritten(ContentFileNames.Lessons, lesson.Id)))
                .Concat(rewards.Stickers.All.Select(sticker => Unwritten(ContentFileNames.Stickers, sticker.Id)))
                .Concat(rewards.Constellations.All.Select(constellation => Unwritten(ContentFileNames.Constellations, constellation.Id)))
                .Concat(rewards.Trails.All.Select(trail => Unwritten(ContentFileNames.Trails, trail.Id)))
                .Concat(rewards.Avatars.All.Select(avatar => Unwritten(ContentFileNames.Avatars, avatar.Id)));
        }

        private static ReleasedId Unwritten(string file, ContentId id)
        {
            return new ReleasedId(file, id.Value, 0);
        }
    }
}
