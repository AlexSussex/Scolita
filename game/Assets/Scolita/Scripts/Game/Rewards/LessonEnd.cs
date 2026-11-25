using Scolita.Core.Languages;
using Scolita.Core.Rewards;
using Scolita.Core.Voice;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// What the end of a lesson shows and says (design screen 6): the planet's constellation with the places this
    /// lesson's stars fly into, the sticker that pops out of its box and the new trail (when earned), the stars line,
    /// and the line said after it (the sticker's, then the trail's). Made by <see cref="RewardsSession.FinishLesson"/>.
    /// </summary>
    public sealed class LessonEnd
    {
        /// <summary>Creates the end; called by <see cref="RewardsSession"/> and the tests.</summary>
        public LessonEnd(LessonRewards rewards, Language language, SkyConstellation constellation, int firstPlace, int placeCount,
            Sprite sticker, Sprite trail, VoiceSequence line, VoiceSequence then)
        {
            Rewards = rewards;
            Language = language;
            Constellation = constellation;
            FirstPlace = firstPlace;
            PlaceCount = placeCount;
            Sticker = sticker;
            Trail = trail;
            Line = line;
            Then = then;
        }

        /// <summary>What the lesson gave.</summary>
        public LessonRewards Rewards { get; }

        /// <summary>The language the end speaks in (the lesson's last step's).</summary>
        public Language Language { get; }

        /// <summary>The planet's constellation; null when its planet has none.</summary>
        public SkyConstellation Constellation { get; }

        /// <summary>The first place this lesson's stars fly into.</summary>
        public int FirstPlace { get; }

        /// <summary>How many places they fly into (0 when the constellation has none for them).</summary>
        public int PlaceCount { get; }

        /// <summary>The new sticker's picture; null for none.</summary>
        public Sprite Sticker { get; }

        /// <summary>The new trail's picture; null for none.</summary>
        public Sprite Trail { get; }

        /// <summary>"Bravo! Ai aprins 3 stele în constelația …"; null when the content has no such line (the lesson's own end line is said).</summary>
        public VoiceSequence Line { get; }

        /// <summary>The sticker's and the trail's lines, said after <see cref="Line"/>; null for none.</summary>
        public VoiceSequence Then { get; }
    }
}
