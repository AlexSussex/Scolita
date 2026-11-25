using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using UnityEngine;

namespace Scolita.Game.Progress
{
    /// <summary>
    /// One item's progress in the <c>progress</c> save (<see cref="ProgressData"/>): its kind, id and language, its state,
    /// its right answers by activity kind, its practice, the days it was right, and its reviews. Read into Core's
    /// <see cref="ItemProgress"/>.
    /// </summary>
    [Serializable]
    public sealed class ProgressItemData
    {
        [SerializeField] private string kind;
        [SerializeField] private string id;
        [SerializeField] private string language;
        [SerializeField] private string state;
        [SerializeField] private ProgressCountData[] rights;
        [SerializeField] private int practised;
        [SerializeField] private string[] rightDays;
        [SerializeField] private string learnedOn;
        [SerializeField] private int reviewStep;
        [SerializeField] private string nextReview;

        /// <summary>The data of <paramref name="item"/>; called by <see cref="ProgressData"/>.</summary>
        public ProgressItemData(ItemProgress item)
        {
            kind = ProgressCodes.CodeOf(item.Item.Kind);
            id = item.Item.Id.Value;
            language = LanguageCodes.CodeOf(item.Item.Language);
            state = ProgressCodes.CodeOf(item.State);
            rights = item.Rights.Select(count => new ProgressCountData(count)).ToArray();
            practised = item.Practised;
            rightDays = item.RightDays.Select(day => day.ToString()).ToArray();
            learnedOn = item.LearnedOn.HasValue ? item.LearnedOn.Value.ToString() : string.Empty;
            reviewStep = item.ReviewStep;
            nextReview = item.NextReview.HasValue ? item.NextReview.Value.ToString() : string.Empty;
        }

        /// <summary>
        /// Core's progress of this item; false, dropping it, when its kind, id, language or state can't be read (only
        /// damage does that: a version that adds a kind raises the file's version). Its unknown activity kinds and its
        /// days that aren't days are dropped; an id the content doesn't know is kept. Called by <see cref="ProgressData"/>.
        /// </summary>
        public bool TryToItem(out ItemProgress item)
        {
            item = null;
            if (!ProgressCodes.TryParseKind(kind, out ItemKind itemKind) || !ContentId.TryCreate(id, out ContentId itemId)
                || !LanguageCodes.TryParseLanguage(language, out Language itemLanguage)
                || !ProgressCodes.TryParseState(state, out ItemState itemState))
            {
                return false;
            }

            List<KindCount> counts = new List<KindCount>();
            foreach (ProgressCountData count in rights ?? new ProgressCountData[0])
            {
                if (count != null && count.TryToCount(out KindCount read))
                {
                    counts.Add(read);
                }
            }

            item = new ItemProgress(new LearningItem(itemKind, itemId, itemLanguage), itemState, counts, practised,
                Days(rightDays), Day(learnedOn), reviewStep, Day(nextReview));
            return true;
        }

        private static IEnumerable<CalendarDay> Days(IEnumerable<string> texts)
        {
            foreach (string text in texts ?? Enumerable.Empty<string>())
            {
                if (CalendarDay.TryParse(text, out CalendarDay day))
                {
                    yield return day;
                }
            }
        }

        private static CalendarDay? Day(string text)
        {
            return CalendarDay.TryParse(text, out CalendarDay day) ? day : (CalendarDay?)null;
        }
    }
}
