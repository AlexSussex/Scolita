using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Voice;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Plans the recording sessions (brief item 8): the lines still to record (no clip yet, or a
    /// named clip missing), one spoken segment per item, grouped by language, then by the first
    /// planet whose lessons say the line (the lines no lesson names go last, as <c>everywhere</c>),
    /// then by kind (sounds, names, words, syllables, slow words, lines), at most
    /// <see cref="MostItems"/> items a session, each line's segments kept together. A line with a
    /// spliced name is left to the system voice (decision D9), and a slow word is said from its
    /// letters' sounds, so neither is listed.
    /// </summary>
    public static class RecordingSessionPlanner
    {
        /// <summary>The most items in one session (about 15 minutes of reading).</summary>
        public const int MostItems = 50;

        /// <summary>The place of the lines no lesson names.</summary>
        public const string Everywhere = "everywhere";

        private const string RomanianLevelTest = "Unu, doi, trei, patru, cinci.";
        private const string EnglishLevelTest = "One, two, three, four, five.";
        private const float SoundSeconds = 0.7f;
        private const float NameSeconds = 0.6f;
        private const float WordStartSeconds = 0.35f;
        private const float SecondsPerCharacter = 0.07f;
        private const float SlowSecondsPerCharacter = 0.15f;
        private const float LineStartSeconds = 0.4f;
        private const float LevelTestSeconds = 2.5f;

        private static readonly VoiceLineKind[] KindOrder =
        {
            VoiceLineKind.Sound, VoiceLineKind.Name, VoiceLineKind.Word, VoiceLineKind.Syllables, VoiceLineKind.Slow,
            VoiceLineKind.Line
        };

        /// <summary>
        /// The sessions for <paramref name="curriculum"/>, whose clips are looked for in
        /// <paramref name="files"/>. Called by Make recording scripts and <c>RecordingSessionPlannerTests</c>.
        /// </summary>
        public static IReadOnlyList<RecordingSession> Plan(Curriculum curriculum, IContentFiles files)
        {
            IReadOnlyDictionary<ContentId, VoiceLineStatus> statuses = VoiceLineStatuses.Of(curriculum, files);
            Dictionary<ContentId, string> places = PlacesOf(curriculum);
            List<string> placeOrder = curriculum.Domains
                .SelectMany(domain => domain.Units)
                .Select(unit => unit.Id.Value)
                .Append(Everywhere)
                .ToList();
            VoiceLine[] toRecord = curriculum.VoiceLines.All
                .Where(line => !line.Retired)
                .Where(line => statuses[line.Id] == VoiceLineStatus.ToRecord || statuses[line.Id] == VoiceLineStatus.Missing)
                .Where(line => !HasNameSlot(line))
                .ToArray();
            List<RecordingSession> sessions = new List<RecordingSession>();
            foreach (Language language in new[] { Language.Romanian, Language.English })
            {
                foreach (string place in placeOrder)
                {
                    VoiceLine[] group = toRecord
                        .Where(line => line.Language == language)
                        .Where(line => PlaceOf(places, line) == place)
                        .OrderBy(line => Array.IndexOf(KindOrder, line.Kind))
                        .ToArray();
                    sessions.AddRange(Sessions(language, place, group));
                }
            }

            return sessions;
        }

        /// <summary>How long <paramref name="text"/>, a <paramref name="kind"/> line, is expected to last; called by the planner and tests.</summary>
        public static float ExpectedSeconds(VoiceLineKind kind, string text)
        {
            switch (kind)
            {
                case VoiceLineKind.Sound:
                    return SoundSeconds;
                case VoiceLineKind.Name:
                    return NameSeconds;
                case VoiceLineKind.Slow:
                case VoiceLineKind.Syllables:
                    return WordStartSeconds + text.Length * SlowSecondsPerCharacter;
                case VoiceLineKind.Word:
                    return WordStartSeconds + text.Length * SecondsPerCharacter;
                default:
                    return LineStartSeconds + text.Length * SecondsPerCharacter;
            }
        }

        private static bool HasNameSlot(VoiceLine line)
        {
            return VoiceTemplate.Parse(line.Text).Pieces.Any(piece => piece.IsSlot && piece.Slot == VoiceSlot.Name);
        }

        private static Dictionary<ContentId, string> PlacesOf(Curriculum curriculum)
        {
            Dictionary<ContentId, string> places = new Dictionary<ContentId, string>();
            foreach (Unit unit in curriculum.Domains.SelectMany(domain => domain.Units))
            {
                foreach (Lesson lesson in unit.Lessons)
                {
                    foreach (VoiceLineId line in LessonVoiceLines.Of(curriculum, lesson))
                    {
                        if (!places.ContainsKey(line.Id))
                        {
                            places.Add(line.Id, unit.Id.Value);
                        }
                    }
                }
            }

            return places;
        }

        private static string PlaceOf(Dictionary<ContentId, string> places, VoiceLine line)
        {
            return places.TryGetValue(line.Id, out string place) ? place : Everywhere;
        }

        private static IEnumerable<RecordingSession> Sessions(Language language, string place, IReadOnlyList<VoiceLine> lines)
        {
            List<RecordingSession> sessions = new List<RecordingSession>();
            List<RecordingItem> items = new List<RecordingItem>();
            foreach (VoiceLine line in lines)
            {
                IReadOnlyList<RecordingItem> lineItems = ItemsOf(line, items.Count + 1);
                if (items.Count > 0 && items.Count + lineItems.Count > MostItems)
                {
                    sessions.Add(Session(language, place, sessions.Count + 1, items));
                    items = new List<RecordingItem>();
                    lineItems = ItemsOf(line, 1);
                }

                items.AddRange(lineItems);
            }

            if (items.Count > 0)
            {
                sessions.Add(Session(language, place, sessions.Count + 1, items));
            }

            return sessions;
        }

        private static RecordingSession Session(Language language, string place, int number, IEnumerable<RecordingItem> items)
        {
            string code = LanguageCodes.CodeOf(language);
            string levelText = language == Language.Romanian ? RomanianLevelTest : EnglishLevelTest;
            RecordingItem levelTest = new RecordingItem(0, string.Empty, 0, 1, string.Empty,
                ContentCodes.CodeOf(VoiceLineKind.Line), levelText, string.Empty, LevelTestSeconds);
            return new RecordingSession(code + "-" + place + "-" + number, code, place, items.Prepend(levelTest));
        }

        private static IReadOnlyList<RecordingItem> ItemsOf(VoiceLine line, int firstNumber)
        {
            VoiceTemplate template = VoiceTemplate.Parse(line.Text);
            List<RecordingItem> items = new List<RecordingItem>();
            IReadOnlyList<VoiceTemplatePiece> pieces = template.Pieces;
            for (int index = 0; index < pieces.Count; index++)
            {
                VoiceTemplatePiece piece = pieces[index];
                if (!piece.IsSpoken)
                {
                    continue;
                }

                string text = piece.Text.Trim();
                string next = index + 1 < pieces.Count && pieces[index + 1].IsSlot
                    ? pieces[index + 1].Slot.ToString()
                    : string.Empty;
                items.Add(new RecordingItem(firstNumber + items.Count, line.Id.Value, piece.SpokenIndex,
                    template.SpokenSegments, VoiceClipNames.For(line.LineId, piece.SpokenIndex, template.SpokenSegments),
                    ContentCodes.CodeOf(line.Kind), text, next, ExpectedSeconds(line.Kind, text)));
            }

            return items;
        }
    }
}
