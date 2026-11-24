using Scolita.Core.Lessons;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// What an activity's view is given besides its round: where to send her taps, the pictures'
    /// sprites, and the learning rules (the numbers she meets, such as how fast a star may move). Made
    /// once per lesson by <see cref="LessonRunner"/>.
    /// </summary>
    public sealed class ActivityViewContext
    {
        /// <summary>Creates the context; called by <see cref="LessonRunner"/> and the PlayMode tests.</summary>
        public ActivityViewContext(IActivityInput input, PictureSprites pictures, LearningRules rules)
        {
            Input = input;
            Pictures = pictures;
            Rules = rules;
        }

        /// <summary>Where the view sends her taps.</summary>
        public IActivityInput Input { get; }

        /// <summary>The pictures' sprites, by picture id.</summary>
        public PictureSprites Pictures { get; }

        /// <summary>The learning rules the lesson runs by.</summary>
        public LearningRules Rules { get; }
    }
}
