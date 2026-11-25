using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Voice;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// What the mascot says during a lesson, through the scene's <see cref="IVoice"/> (seam 4): a line
    /// and the line after it (the praise once the letter's sound is heard), each shown in the mascot's
    /// bubble in the same words while it is said. A newer line replaces the one being said; a line is
    /// never cut off otherwise. It remembers the last line (the mascot says it again when tapped) and
    /// the line being said when the app goes to the background, to say it again on its return. The
    /// bubble's words are made when the lesson is built (<see cref="Prepare"/>), so saying a line
    /// allocates nothing here. Moved on every frame by <see cref="LessonRunner"/>; the galaxy map's
    /// <c>MapRunner</c> speaks through one too.
    /// </summary>
    public sealed class LessonVoice
    {
        private readonly IVoice voice;
        private readonly ISpeechBubble bubble;
        private readonly Dictionary<VoiceSequence, string> texts = new Dictionary<VoiceSequence, string>();
        private readonly Dictionary<VoiceSequence, Language> languages = new Dictionary<VoiceSequence, Language>();
        private VoiceSequence current;
        private VoiceSequence next;
        private VoiceSequence last;
        private bool paused;

        /// <summary>Creates the lesson's voice; called by <see cref="LessonRunner"/> and the PlayMode tests.</summary>
        public LessonVoice(IVoice voice, ISpeechBubble bubble)
        {
            this.voice = voice;
            this.bubble = bubble;
        }

        /// <summary>Whether a line is being said, or waits to be.</summary>
        public bool IsBusy => current != null || voice.IsSpeaking;

        /// <summary>Whether the voice is saying something now (the mascot's mouth moves).</summary>
        public bool IsSpeaking => voice.IsSpeaking;

        /// <summary>
        /// Makes the bubble's words of every line in <paramref name="lines"/>. Called by
        /// <see cref="LessonRunner"/> when a lesson is built.
        /// </summary>
        public void Prepare(Curriculum curriculum, IEnumerable<VoiceSequence> lines)
        {
            foreach (VoiceSequence line in lines)
            {
                if (line == null || texts.ContainsKey(line))
                {
                    continue;
                }

                texts.Add(line, VoiceSequenceText.Of(curriculum, line));
                languages.Add(line, VoiceSequenceText.TryGetLanguage(curriculum, line, out Language language)
                    ? language
                    : Language.Romanian);
            }
        }

        /// <summary>
        /// Says <paramref name="first"/>, then <paramref name="then"/>, replacing whatever was being said;
        /// either may be null, and nothing changes when both are. Called by <see cref="LessonRunner"/>.
        /// </summary>
        public void Say(VoiceSequence first, VoiceSequence then)
        {
            if (first == null && then == null)
            {
                return;
            }

            next = first == null ? null : then;
            Begin(first ?? then);
        }

        /// <summary>
        /// Says the last line again; when it is still being said, it starts again and the line after it
        /// (the praise) still follows. Called by <see cref="LessonRunner"/> when the mascot is tapped.
        /// </summary>
        public void SayAgain()
        {
            if (last == null)
            {
                return;
            }

            if (last != current)
            {
                next = null;
            }

            Begin(last);
        }

        /// <summary>
        /// Moves on: once a line is said, says the one after it, or hides the bubble. Called every
        /// frame by <see cref="LessonRunner"/>.
        /// </summary>
        public void Tick()
        {
            if (paused || current == null || voice.IsSpeaking)
            {
                return;
            }

            if (next != null)
            {
                VoiceSequence following = next;
                next = null;
                Begin(following);
                return;
            }

            current = null;
            bubble.Hide();
        }

        /// <summary>
        /// Shows the words of the line being said again: the voice's own bubble may have shown only a
        /// part of it. Called by <see cref="LessonRunner"/> when the voice starts a part.
        /// </summary>
        public void PartStarted()
        {
            if (current != null)
            {
                ShowBubble(current);
            }
        }

        /// <summary>Stops the voice when the app goes to the background; called by <see cref="LessonRunner"/>.</summary>
        public void Pause()
        {
            paused = true;
            voice.Stop();
        }

        /// <summary>Says again the line that was being said when the app went to the background; called by <see cref="LessonRunner"/>.</summary>
        public void Resume()
        {
            paused = false;
            if (current != null)
            {
                Begin(current);
            }
        }

        private void Begin(VoiceSequence line)
        {
            current = line;
            last = line;
            voice.SpeakSequence(line);
            ShowBubble(line);
        }

        private void ShowBubble(VoiceSequence line)
        {
            if (texts.TryGetValue(line, out string text))
            {
                bubble.Show(text, languages[line]);
            }
        }
    }
}
