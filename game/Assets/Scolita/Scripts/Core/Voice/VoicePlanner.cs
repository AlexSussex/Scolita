using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// Turns what is to be said into the <see cref="VoiceUtterance"/>s <see cref="VoiceRouter"/> says
    /// in turn: each spoken segment by its clip when the clip voice has it, else by the system voice
    /// in the line's language when it speaks it, else in the bubble; slots by their fills; a slow line
    /// without its own clip by its word's letters' sounds with pauses, and a stretched line without its
    /// own clip by its word's first sound, a pause, then the word (the tablet's voice can't hold a sound,
    /// and would say "t-t-tata" as letter names). Neighbouring texts for the
    /// system voice (or the bubble) in one language are joined into one, so a spliced name keeps the
    /// sentence's tune. The lines' templates are split once, when the planner is made.
    /// </summary>
    public sealed class VoicePlanner
    {
        private readonly ItemCatalog<VoiceLine> lines;
        private readonly Dictionary<ContentId, VoiceTemplate> templates;
        private readonly Dictionary<ContentId, IReadOnlyList<VoiceLineId>> saidInParts;
        private readonly IClipVoice clips;
        private readonly ISystemVoice system;
        private readonly VoiceTiming timing;

        /// <summary>Creates the planner for <paramref name="curriculum"/>'s lines; called by <see cref="VoiceRouter"/>.</summary>
        public VoicePlanner(Curriculum curriculum, IClipVoice clips, ISystemVoice system, VoiceTiming timing)
        {
            lines = curriculum.VoiceLines;
            templates = curriculum.VoiceLines.All.ToDictionary(line => line.Id, line => VoiceTemplate.Parse(SpokenText(line)));
            saidInParts = curriculum.Words.All
                .Where(word => !word.Retired)
                .ToDictionary(word => word.SlowLine.Id, word => SoundsOf(curriculum, word));
            foreach (Word word in curriculum.Words.All.Where(word => !word.Retired))
            {
                saidInParts[word.StretchedLine.Id] = FirstSoundThenWord(curriculum, word);
            }

            this.clips = clips;
            this.system = system;
            this.timing = timing;
        }

        /// <summary>
        /// Plans <paramref name="line"/> into <paramref name="plan"/>, adding to <paramref name="missing"/>
        /// any line that isn't in the content. Throws an <see cref="InvalidOperationException"/> for a
        /// line with slots (it needs <see cref="PlanSequence"/>). Called by <see cref="VoiceRouter.Speak"/>.
        /// </summary>
        public void PlanLine(VoiceLineId line, List<VoiceUtterance> plan, List<VoiceLineId> missing)
        {
            PlanItem(line, null, plan, missing);
        }

        /// <summary>
        /// Plans <paramref name="sequence"/> into <paramref name="plan"/>, adding to
        /// <paramref name="missing"/> any line that isn't in the content. Throws an
        /// <see cref="InvalidOperationException"/> for a slot left without its fill. Called by
        /// <see cref="VoiceRouter.SpeakSequence"/>.
        /// </summary>
        public void PlanSequence(VoiceSequence sequence, List<VoiceUtterance> plan, List<VoiceLineId> missing)
        {
            foreach (VoiceSequenceItem item in sequence.Items)
            {
                PlanItem(item.Line, item, plan, missing);
            }
        }

        private static string SpokenText(VoiceLine line)
        {
            return line.Kind == VoiceLineKind.Sound ? line.Text.Replace("/", string.Empty) : line.Text;
        }

        private static IReadOnlyList<VoiceLineId> SoundsOf(Curriculum curriculum, Word word)
        {
            return word.Letters
                .Where(letter => curriculum.Letters.Contains(letter))
                .Select(letter => curriculum.Letters.Get(letter).SoundLine(word.Language))
                .ToArray();
        }

        private static IReadOnlyList<VoiceLineId> FirstSoundThenWord(Curriculum curriculum, Word word)
        {
            IEnumerable<Letter> letters = curriculum.Letters.All.Where(letter => letter.HasLanguage(word.Language));
            return WordStarts.TryFind(word, letters, out Letter first)
                ? new[] { first.SoundLine(word.Language), word.WordLine }
                : new VoiceLineId[0];
        }

        private void PlanItem(VoiceLineId id, VoiceSequenceItem item, List<VoiceUtterance> plan, List<VoiceLineId> missing)
        {
            if (!lines.TryGet(id.Id, out VoiceLine line))
            {
                missing.Add(id);
                return;
            }

            if (IsSaidAsSounds(line, out IReadOnlyList<VoiceLineId> sounds))
            {
                PlanSounds(sounds, plan, missing);
                return;
            }

            VoiceTemplate template = templates[line.Id];
            bool clipPerSegment = line.Clips.Count == template.SpokenSegments;
            foreach (VoiceTemplatePiece piece in template.Pieces)
            {
                if (piece.IsSlot)
                {
                    PlanSlot(line, piece.Slot, item, plan, missing);
                }
                else if (piece.IsSpoken)
                {
                    AddSpoken(clipPerSegment ? line.Clips[piece.SpokenIndex] : null, piece.Text, line.Language, plan);
                }
                else if (piece.IsPause)
                {
                    plan.Add(new VoiceUtterance(VoiceRoute.Pause, null, null, line.Language, timing.PauseSeconds));
                }
                else
                {
                    AppendToLastText(piece.Text, line.Language, plan);
                }
            }
        }

        private bool IsSaidAsSounds(VoiceLine line, out IReadOnlyList<VoiceLineId> sounds)
        {
            sounds = null;
            return (line.Kind == VoiceLineKind.Slow || line.Kind == VoiceLineKind.Stretched)
                && saidInParts.TryGetValue(line.Id, out sounds)
                && sounds.Count > 0
                && !(line.HasClip && line.Clips.All(clips.Has));
        }

        private void PlanSounds(IReadOnlyList<VoiceLineId> sounds, List<VoiceUtterance> plan, List<VoiceLineId> missing)
        {
            for (int index = 0; index < sounds.Count; index++)
            {
                if (index > 0)
                {
                    plan.Add(new VoiceUtterance(VoiceRoute.Pause, null, null, default, timing.PauseSeconds));
                }

                PlanItem(sounds[index], null, plan, missing);
            }
        }

        private void PlanSlot(VoiceLine line, VoiceSlot slot, VoiceSequenceItem item, List<VoiceUtterance> plan,
            List<VoiceLineId> missing)
        {
            if (item == null || !item.TryGetFill(slot, out VoiceFill fill))
            {
                throw new InvalidOperationException("The voice line " + line.Id + " has the slot " + slot
                    + ", but nothing was given for it: speak it as a VoiceSequence with that slot filled.");
            }

            if (fill.HasLine)
            {
                PlanItem(fill.Line, null, plan, missing);
            }
            else
            {
                AddSay(fill.Text, line.Language, plan);
            }
        }

        private void AddSpoken(string clip, string text, Language language, List<VoiceUtterance> plan)
        {
            if (clip != null && clips.Has(clip))
            {
                plan.Add(new VoiceUtterance(VoiceRoute.Clip, clip, text, language, 0f));
                return;
            }

            AddSay(text, language, plan);
        }

        private void AddSay(string text, Language language, List<VoiceUtterance> plan)
        {
            VoiceRoute route = system.Speaks(language) ? VoiceRoute.System : VoiceRoute.Bubble;
            int last = plan.Count - 1;
            if (last >= 0 && plan[last].Route == route && plan[last].Language == language)
            {
                plan[last] = plan[last].WithMoreText(NeedsSpace(plan[last].Text, text) ? " " + text : text);
                return;
            }

            plan.Add(new VoiceUtterance(route, null, text, language, 0f));
        }

        private static bool NeedsSpace(string before, string after)
        {
            return before.Length > 0 && after.Length > 0 && !char.IsWhiteSpace(before[before.Length - 1])
                && char.IsLetterOrDigit(after[0]);
        }

        private static void AppendToLastText(string text, Language language, List<VoiceUtterance> plan)
        {
            int last = plan.Count - 1;
            if (last >= 0
                && (plan[last].Route == VoiceRoute.System || plan[last].Route == VoiceRoute.Bubble)
                && plan[last].Language == language)
            {
                plan[last] = plan[last].WithMoreText(text);
            }
        }
    }
}
