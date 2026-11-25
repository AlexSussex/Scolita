using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Core.Randomness;
using Scolita.Core.Rewards;
using Scolita.Core.Voice;
using Scolita.Game.Lessons;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// Her rewards in a scene: her <see cref="RewardBook"/> (read from the <c>rewards</c> save, saved after every
    /// change), Core's <see cref="RewardGiver"/> for a finished lesson, and the reward screens' taps
    /// (<see cref="IRewardsInput"/>): it opens the books, says their lines in the scene's language through the
    /// scene's <see cref="LessonVoice"/>, and places, moves and takes off stickers. Made once by each scene's
    /// composition root (<c>LessonRunner</c>, <c>MapRunner</c>); no learning rule lives here.
    /// </summary>
    public sealed class RewardsSession : IRewardsInput
    {
        private readonly RewardSources sources;
        private readonly SaveStore profileFiles;
        private readonly LessonOrder order;
        private readonly LessonVoice voice;
        private readonly RewardsView view;
        private readonly RewardGiver giver;
        private readonly RewardBook book;
        private readonly Curriculum curriculum;
        private readonly Language language;
        private readonly List<VoiceLineId> spoken = new List<VoiceLineId>();
        private readonly List<VoiceSequence> lines = new List<VoiceSequence>();
        private readonly Dictionary<ContentId, VoiceSequence> starsLines = new Dictionary<ContentId, VoiceSequence>();
        private readonly Dictionary<ContentId, VoiceSequence> nameLines = new Dictionary<ContentId, VoiceSequence>();
        private readonly VoiceSequence stickerLine;
        private readonly VoiceSequence trailLine;
        private readonly VoiceSequence stickerAndTrailLine;
        private readonly VoiceSequence skyBookLine;
        private readonly VoiceSequence stickerBookLine;
        private readonly VoiceSequence trailPickerLine;
        private SkyChart openSky;

        /// <summary>
        /// Reads her rewards from <paramref name="profileFiles"/> (a profile from before rewards gets the stars of what
        /// <paramref name="doneBefore"/> says she finished), makes the rewards' lines in <paramref name="language"/>
        /// for <paramref name="voice"/>, and binds <paramref name="view"/>. Called by <c>LessonRunner</c> and
        /// <c>MapRunner</c> when their scene starts (before a lesson is recorded as done), and by the tests.
        /// </summary>
        public RewardsSession(RewardSources sources, SaveStore profileFiles, LessonOrder order, IDoneLessons doneBefore,
            Curriculum curriculum, LessonVoice voice, Language language, RewardsView view, IRandom random)
        {
            this.sources = sources;
            this.profileFiles = profileFiles;
            this.order = order;
            this.voice = voice;
            this.view = view;
            this.curriculum = curriculum;
            this.language = language;
            giver = new RewardGiver(sources.Content, sources.Rules, random);
            book = RewardsFile.Load(profileFiles, giver, doneBefore, order);
            foreach (Constellation constellation in sources.Content.Constellations.All.Where(constellation => !constellation.Retired))
            {
                AddLine(starsLines, constellation.Id, RewardLines.Stars(constellation, language));
                AddLine(nameLines, constellation.Id, RewardLines.Name(constellation, language));
            }

            stickerLine = Line(RewardLines.Common(RewardLines.StickerKey, language));
            trailLine = Line(RewardLines.Common(RewardLines.TrailKey, language));
            stickerAndTrailLine = stickerLine != null && trailLine != null
                ? Prepared(VoiceSequence.Of(stickerLine.Lead).Then(trailLine.Lead))
                : stickerLine ?? trailLine;
            skyBookLine = Line(RewardLines.Common(RewardLines.SkyBookKey, language));
            stickerBookLine = Line(RewardLines.Common(RewardLines.StickerBookKey, language));
            trailPickerLine = Line(RewardLines.Common(RewardLines.TrailPickerKey, language));
            voice.Prepare(curriculum, lines);
            view.Bind(this);
        }

        /// <summary>Her rewards.</summary>
        public RewardBook Book => book;

        /// <summary>The rules that give them; <c>sound-feel</c> listens to its <see cref="RewardGiver.RewardEarned"/>.</summary>
        public RewardGiver Giver => giver;

        /// <summary>The lines the rewards can say here, whose clips the scene loads.</summary>
        public IReadOnlyList<VoiceLineId> SpokenLines => spoken;

        /// <summary>
        /// Shows the map's bottom bar and the rocket's trail. Called by <c>MapRunner</c> when the map opens.
        /// </summary>
        public void ShowOnMap()
        {
            view.ShowBar(book.StarsEarned, book.OwnedStickers.Count > 0, book.Trails.Count > 0);
            view.ShowRocketTrail(sources.Sprites.Trail(book.ChosenTrail));
        }

        /// <summary>
        /// Gives the rewards of <paramref name="lesson"/>, just finished (<see cref="RewardGiver.LessonFinished"/>), saves
        /// them, and returns what its end shows and says. Called by <c>LessonRunner</c> when the lesson ends.
        /// </summary>
        public LessonEnd FinishLesson(Lesson lesson)
        {
            LessonRewards rewards = giver.LessonFinished(book, lesson, order);
            RewardsFile.Save(profileFiles, book);
            SkyChart sky = SkyChart.Of(sources.Content, order, book, sources.Rules);
            SkyConstellation constellation = sky.TryGet(lesson.UnitId, out SkyConstellation found) ? found : null;
            int first = 0;
            int count = 0;
            constellation?.TryGetPlacesOf(lesson.Id, out first, out count);
            Sprite sticker = rewards.Sticker == null ? null : sources.Sprites.Sticker(rewards.Sticker.Id);
            Sprite trail = rewards.Trail == null ? null : sources.Sprites.Trail(rewards.Trail.Id);
            VoiceSequence line = constellation != null && starsLines.TryGetValue(constellation.Constellation.Id, out VoiceSequence stars)
                ? stars
                : null;
            return new LessonEnd(rewards, language, constellation, first, count, sticker, trail, line, ThenLine(rewards));
        }

        /// <inheritdoc/>
        public void OpenSkyBook()
        {
            openSky = SkyChart.Of(sources.Content, order, book, sources.Rules);
            view.ShowSkyBook(openSky);
            voice.Say(skyBookLine, null);
        }

        /// <inheritdoc/>
        public void OpenStickerBook()
        {
            view.ShowStickerBook(StickerShelf.Of(book, sources.Sprites, sources.Rules));
            voice.Say(stickerBookLine, null);
        }

        /// <inheritdoc/>
        public void OpenTrails()
        {
            TrailChoice[] choices = book.Trails
                .Select(trail => new TrailChoice(trail, sources.Sprites.Trail(trail)))
                .Where(choice => choice.Picture != null)
                .ToArray();
            view.ShowTrails(choices, book.ChosenTrail);
            voice.Say(trailPickerLine, null);
        }

        /// <inheritdoc/>
        public void CloseBook()
        {
            openSky = null;
            view.CloseBooks();
        }

        /// <inheritdoc/>
        public void TapConstellation(int index)
        {
            if (openSky == null || index < 0 || index >= openSky.Constellations.Count)
            {
                return;
            }

            if (nameLines.TryGetValue(openSky.Constellations[index].Constellation.Id, out VoiceSequence name))
            {
                voice.Say(name, null);
            }
        }

        /// <inheritdoc/>
        public bool PlaceSticker(ContentId sticker, Vector2 at, float size)
        {
            if (!book.TryPlace(sticker, at.x, at.y, size, sources.Rules))
            {
                return false;
            }

            RewardsFile.Save(profileFiles, book);
            return true;
        }

        /// <inheritdoc/>
        public void MoveSticker(int index, Vector2 at, float size)
        {
            if (!IsPlaced(index))
            {
                return;
            }

            book.Move(index, at.x, at.y, size, sources.Rules);
            RewardsFile.Save(profileFiles, book);
        }

        /// <inheritdoc/>
        public void TakeOffSticker(int index)
        {
            if (!IsPlaced(index))
            {
                return;
            }

            book.TakeOff(index);
            RewardsFile.Save(profileFiles, book);
        }

        /// <inheritdoc/>
        public void ChooseTrail(ContentId trail)
        {
            if (!book.ChooseTrail(trail))
            {
                Debug.LogError("Rewards: the trail " + trail + " was offered but isn't earned.");
                return;
            }

            RewardsFile.Save(profileFiles, book);
            view.ShowRocketTrail(sources.Sprites.Trail(trail));
            view.CloseBooks();
        }

        private bool IsPlaced(int index)
        {
            if (index >= 0 && index < book.Placed.Count)
            {
                return true;
            }

            Debug.LogError("Rewards: the sticker book named the sticker number " + index + ", which isn't on the scene.");
            return false;
        }

        private VoiceSequence ThenLine(LessonRewards rewards)
        {
            if (rewards.Sticker != null && rewards.Trail != null)
            {
                return stickerAndTrailLine;
            }

            if (rewards.Sticker != null)
            {
                return stickerLine;
            }

            return rewards.Trail != null ? trailLine : null;
        }

        private void AddLine(Dictionary<ContentId, VoiceSequence> byConstellation, ContentId constellation, VoiceLineId id)
        {
            VoiceSequence line = Line(id);
            if (line != null)
            {
                byConstellation.Add(constellation, line);
            }
        }

        private VoiceSequence Line(VoiceLineId id)
        {
            if (!curriculum.VoiceLines.TryGet(id.Id, out VoiceLine found) || found.Retired)
            {
                Debug.LogWarning("Rewards: the content has no voice line " + id + " in " + language + "; it is not said.");
                return null;
            }

            spoken.Add(id);
            return Prepared(VoiceSequence.Of(id));
        }

        private VoiceSequence Prepared(VoiceSequence line)
        {
            lines.Add(line);
            return line;
        }
    }
}
