using System.Collections;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Game.Lessons;
using Scolita.Game.Scenes;
using Scolita.Game.Voice;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Scolita.Tests.PlayMode.Voice
{
    /// <summary>Tests <see cref="Hearable"/>: a tap says its line, it pulses while the line is said, and the Lesson placeholder's sampler speaks.</summary>
    public sealed class HearableTests
    {
        /// <summary>Tests that tapping a <see cref="Hearable"/> asks the voice for its line, and that it pulses until the line ends.</summary>
        [UnityTest]
        public IEnumerator TestOnPointerClick_SpeaksItsLineAndPulses()
        {
            GameObject canvas = new GameObject("Canvas", typeof(Canvas));
            GameObject events = new GameObject("EventSystem", typeof(EventSystem));
            GameObject tile = new GameObject("Tile", typeof(RectTransform), typeof(Image));
            tile.transform.SetParent(canvas.transform, false);
            Hearable hearable = tile.AddComponent<Hearable>();
            FakeVoice voice = new FakeVoice();
            VoiceLineId line = new VoiceLineId(ContentId.Create("ro-sound-m"));
            try
            {
                hearable.Bind(voice, line);

                ExecuteEvents.Execute(tile, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                yield return null;
                yield return null;
                bool pulsedWhileSpeaking = hearable.IsSpeaking && tile.transform.localScale != Vector3.one;
                voice.FinishSpeaking();
                yield return null;

                Assert.That(voice.Spoken, Is.EqualTo(new[] { "ro-sound-m" }));
                Assert.That(pulsedWhileSpeaking, Is.True);
                Assert.That(hearable.IsSpeaking, Is.False);
                Assert.That(tile.transform.localScale, Is.EqualTo(Vector3.one));
            }
            finally
            {
                Object.Destroy(canvas);
                Object.Destroy(events);
            }
        }

        /// <summary>Tests that a <see cref="Hearable"/> stops pulsing when another line starts, so only the line being said pulses.</summary>
        [UnityTest]
        public IEnumerator TestOnPointerClick_AnotherLineStopsThePulse()
        {
            GameObject canvas = new GameObject("Canvas", typeof(Canvas));
            GameObject first = new GameObject("First", typeof(RectTransform), typeof(Image));
            GameObject second = new GameObject("Second", typeof(RectTransform), typeof(Image));
            first.transform.SetParent(canvas.transform, false);
            second.transform.SetParent(canvas.transform, false);
            Hearable firstHearable = first.AddComponent<Hearable>();
            Hearable secondHearable = second.AddComponent<Hearable>();
            FakeVoice voice = new FakeVoice();
            try
            {
                firstHearable.Bind(voice, new VoiceLineId(ContentId.Create("ro-sound-m")));
                secondHearable.Bind(voice, new VoiceLineId(ContentId.Create("ro-sound-a")));

                firstHearable.OnPointerClick(null);
                yield return null;
                secondHearable.OnPointerClick(null);
                yield return null;

                Assert.That(voice.Spoken, Is.EqualTo(new[] { "ro-sound-m", "ro-sound-a" }));
                Assert.That(firstHearable.IsSpeaking, Is.False);
                Assert.That(secondHearable.IsSpeaking, Is.True);
                Assert.That(first.transform.localScale, Is.EqualTo(Vector3.one));
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        /// <summary>
        /// Tests that the Lesson placeholder's voice sampler is bound: tapping its first letter makes
        /// the scene's voice speak, and in the Editor (no clip, no system voice) the bubble shows it.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLessonSampler_SpeaksOnATap()
        {
            SceneManager.LoadScene(AppScene.Lesson.ToString());
            yield return SceneSearch.WaitForScene(AppScene.Lesson);
            yield return null;

            Hearable letter = SceneSearch.FindInActiveScene<Hearable>();
            SpeechBubbleView bubble = SceneSearch.FindInActiveScene<SpeechBubbleView>();
            bool bubbleHiddenBefore = !bubble.transform.GetChild(0).gameObject.activeSelf;
            letter.OnPointerClick(null);
            yield return null;

            Assert.That(SceneSearch.FindInActiveScene<LessonRunner>(), Is.Not.Null);
            Assert.That(bubbleHiddenBefore, Is.True);
            Assert.That(letter.IsSpeaking, Is.True);
            Assert.That(bubble.transform.GetChild(0).gameObject.activeSelf, Is.True);
        }
    }
}
