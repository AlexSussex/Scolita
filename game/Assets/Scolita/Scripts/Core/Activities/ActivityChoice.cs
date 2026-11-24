using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// One thing she can tap in an activity: a letter, a picture, a word to read or a part of the
    /// activity (a drum), with its language, the text it shows and the line it is heard by. Activities work on choices, not on letters or words, so a
    /// later domain's items (an instrument, a number) can be choices too. Made by the activities'
    /// rules when a lesson is built.
    /// </summary>
    public sealed class ActivityChoice
    {
        /// <summary>Creates a choice; called by <see cref="OfLetter"/>, <see cref="OfWord"/> and the tests.</summary>
        public ActivityChoice(ContentId item, ActivityChoiceLook look, Language language, string text, string upperText,
            ContentId picture, VoiceLineId heardBy)
            : this(item, look, language, text, upperText, picture, heardBy, false)
        {
        }

        private ActivityChoice(ContentId item, ActivityChoiceLook look, Language language, string text, string upperText,
            ContentId picture, VoiceLineId heardBy, bool isCapital)
        {
            IsCapital = isCapital;
            Item = item;
            Look = look;
            Language = language;
            Text = text ?? string.Empty;
            UpperText = upperText ?? string.Empty;
            Picture = picture;
            HeardBy = heardBy;
        }

        /// <summary>The id of the letter or word it is; not set for a control.</summary>
        public ContentId Item { get; }

        /// <summary>How it is drawn.</summary>
        public ActivityChoiceLook Look { get; }

        /// <summary>Its language, shown by its badge.</summary>
        public Language Language { get; }

        /// <summary>
        /// The text it shows: the letter in lower case (or its capital, <see cref="OfCapital"/>), or the word;
        /// empty for a control.
        /// </summary>
        public string Text { get; }

        /// <summary>A letter's capital (shown beside it by <c>MeetLetter</c>); empty for a word.</summary>
        public string UpperText { get; }

        /// <summary>Whether it is a letter shown as its capital alone (<see cref="OfCapital"/>).</summary>
        public bool IsCapital { get; }

        /// <summary>The picture it shows (a word to read shows it only when revealed); not set for a letter or a control.</summary>
        public ContentId Picture { get; }

        /// <summary>The line it says when tapped or asked for: a letter's sound, a word's word line; not set for a control.</summary>
        public VoiceLineId HeardBy { get; }

        /// <summary>
        /// A letter as a choice in <paramref name="language"/>: its lower case, its capital and its
        /// sound. Called by the activities' rules.
        /// </summary>
        public static ActivityChoice OfLetter(Letter letter, Language language)
        {
            return new ActivityChoice(letter.Id, ActivityChoiceLook.Letter, language, letter.Lower, letter.Upper, default,
                letter.SoundLine(language));
        }

        /// <summary>
        /// A letter as a choice in <paramref name="language"/> that shows its capital alone
        /// (<c>LetterHunt</c>'s stars), heard by its sound. Called by the activities' rules.
        /// </summary>
        public static ActivityChoice OfCapital(Letter letter, Language language)
        {
            return new ActivityChoice(letter.Id, ActivityChoiceLook.Letter, language, letter.Upper, string.Empty, default,
                letter.SoundLine(language), true);
        }

        /// <summary>A word as a choice: its picture, its text and its word line. Called by the activities' rules.</summary>
        public static ActivityChoice OfWord(Word word)
        {
            return new ActivityChoice(word.Id, ActivityChoiceLook.Picture, word.Language, word.Text, string.Empty,
                word.Picture, word.WordLine);
        }

        /// <summary>
        /// A word as a choice written to read: its text, its picture (shown only when the round reveals it)
        /// and its word line. Called by the activities' rules.
        /// </summary>
        public static ActivityChoice OfWordToRead(Word word)
        {
            return new ActivityChoice(word.Id, ActivityChoiceLook.Word, word.Language, word.Text, string.Empty,
                word.Picture, word.WordLine);
        }

        /// <summary>
        /// A part of the activity in <paramref name="language"/> that the view draws by its index
        /// (<c>Syllables</c>' drum and tick): no item, no text, heard by nothing. Called by the activities' rules.
        /// </summary>
        public static ActivityChoice OfControl(Language language)
        {
            return new ActivityChoice(default, ActivityChoiceLook.Control, language, string.Empty, string.Empty, default,
                default);
        }
    }
}
