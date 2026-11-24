namespace Scolita.Game.Lessons
{
    /// <summary>
    /// What an activity's view is given besides its round: where to send her taps, and the pictures'
    /// sprites. Made once per lesson by <see cref="LessonRunner"/>.
    /// </summary>
    public sealed class ActivityViewContext
    {
        /// <summary>Creates the context; called by <see cref="LessonRunner"/> and the PlayMode tests.</summary>
        public ActivityViewContext(IActivityInput input, PictureSprites pictures)
        {
            Input = input;
            Pictures = pictures;
        }

        /// <summary>Where the view sends her taps.</summary>
        public IActivityInput Input { get; }

        /// <summary>The pictures' sprites, by picture id.</summary>
        public PictureSprites Pictures { get; }
    }
}
