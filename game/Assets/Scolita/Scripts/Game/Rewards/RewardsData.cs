using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// Her rewards, saved as <c>rewards.json</c> in the profile's folder through <see cref="SaveStore.ForProfile"/>
    /// (AGENTS.md, seam 5): the stars earned, the lessons lit (in order), the lessons finished, the stickers given (in
    /// order), the planets rewarded, the trails earned and the one chosen, and the stickers on the scene. Read into
    /// Core's <see cref="RewardBook"/>, which brings it into its rules.
    /// </summary>
    [Serializable]
    public sealed class RewardsData : ISaveData<RewardsData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "rewards";

        private const int LatestVersion = 1;

        [SerializeField] private int version;
        [SerializeField] private int starsEarned;
        [SerializeField] private string[] litLessons;
        [SerializeField] private int lessonsFinished;
        [SerializeField] private string[] stickers;
        [SerializeField] private string[] rewardedUnits;
        [SerializeField] private string[] trails;
        [SerializeField] private string trail;
        [SerializeField] private PlacedStickerData[] placed;

        /// <summary>The data of <paramref name="book"/>; called by <see cref="RewardsFile"/> and the tests.</summary>
        public RewardsData(RewardBook book)
        {
            version = LatestVersion;
            starsEarned = book.StarsEarned;
            litLessons = Texts(book.LitLessons);
            lessonsFinished = book.LessonsFinished;
            stickers = Texts(book.GivenStickers);
            rewardedUnits = Texts(book.RewardedUnits);
            trails = Texts(book.Trails);
            trail = book.ChosenTrail.IsSet ? book.ChosenTrail.Value : string.Empty;
            placed = book.Placed.Select(sticker => new PlacedStickerData(sticker)).ToArray();
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>A new profile's rewards: nothing yet. Called by <see cref="RewardsFile"/> and the tests.</summary>
        public static RewardsData CreateDefaults()
        {
            return new RewardsData(RewardBook.Empty());
        }

        /// <summary>
        /// Core's book from this data: a text that can't be an id is dropped, an id the content doesn't know is kept
        /// (a later version may know it), and the book brings the rest into its rules (<see cref="RewardBook"/>).
        /// Called by <see cref="RewardsFile"/> and the tests.
        /// </summary>
        public RewardBook ToBook()
        {
            IEnumerable<PlacedSticker> stickersPlaced = (placed ?? new PlacedStickerData[0])
                .Where(sticker => sticker != null)
                .Select(sticker => sticker.ToPlaced())
                .Where(sticker => sticker.StickerId.IsSet);
            return new RewardBook(starsEarned, Ids(litLessons), lessonsFinished, Ids(stickers), Ids(rewardedUnits), Ids(trails),
                ContentId.TryCreate(trail, out ContentId chosen) ? chosen : default, stickersPlaced);
        }

        /// <summary>
        /// Returns this data in the current shape, through Core's rules (<see cref="ToBook"/>): missing lists become
        /// empty, counts below 0 become 0, a placement is kept inside the scene, the chosen trail must be earned.
        /// A later version adds its migration step here, by <see cref="Version"/>. Called by
        /// <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public RewardsData Upgraded()
        {
            return new RewardsData(ToBook());
        }

        private static string[] Texts(IEnumerable<ContentId> ids)
        {
            return ids.Select(id => id.Value).ToArray();
        }

        private static IEnumerable<ContentId> Ids(IEnumerable<string> texts)
        {
            List<ContentId> ids = new List<ContentId>();
            foreach (string text in texts ?? Enumerable.Empty<string>())
            {
                if (ContentId.TryCreate(text, out ContentId id))
                {
                    ids.Add(id);
                }
            }

            return ids;
        }
    }
}
