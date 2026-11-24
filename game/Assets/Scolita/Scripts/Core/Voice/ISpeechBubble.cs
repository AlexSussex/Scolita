using Scolita.Core.Languages;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// The speech bubble that shows a line's text when no voice can say it (no clip, and the system
    /// voice missing its language): the app's is <c>SpeechBubbleView</c>; tests use a fake.
    /// </summary>
    public interface ISpeechBubble
    {
        /// <summary>Shows <paramref name="text"/>, marked as <paramref name="language"/>.</summary>
        void Show(string text, Language language);

        /// <summary>Hides the bubble.</summary>
        void Hide();
    }
}
