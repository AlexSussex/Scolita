using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Voice;
using Scolita.Game.Lessons;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Game
{
    /// <summary>Tests <see cref="LessonVoice"/> with a fake voice and a fake bubble, on the real content's lines.</summary>
    public sealed class LessonVoiceTests
    {
        /// <summary>
        /// Tests that <see cref="LessonVoice.Say"/> says the first line, then the one after it once the
        /// first is said, showing each in the bubble, and hides the bubble at the end.
        /// </summary>
        [Test]
        public void TestSay_FirstThenThen()
        {
            List<string> log = new List<string>();
            FakeVoice voice = new FakeVoice();
            LessonVoice lessonVoice = Prepared(voice, log, out VoiceSequence sound, out VoiceSequence praise);

            lessonVoice.Say(sound, praise);
            lessonVoice.Tick();
            voice.FinishSpeaking();
            lessonVoice.Tick();
            bool busyDuringPraise = lessonVoice.IsBusy;
            voice.FinishSpeaking();
            lessonVoice.Tick();

            Assert.That(voice.Spoken, Is.EqualTo(new[] { "ro-sound-a", "ro-line-praise-1" }));
            Assert.That(log, Is.EqualTo(new[] { "show ro a\u2026", "show ro Bravo, ai reu\u0219it!", "hide" }));
            Assert.That(busyDuringPraise, Is.True);
            Assert.That(lessonVoice.IsBusy, Is.False);
        }

        /// <summary>Tests that a newer <see cref="LessonVoice.Say"/> replaces the line being said and the one waiting after it.</summary>
        [Test]
        public void TestSay_ANewerLineReplaces()
        {
            List<string> log = new List<string>();
            FakeVoice voice = new FakeVoice();
            LessonVoice lessonVoice = Prepared(voice, log, out VoiceSequence sound, out VoiceSequence praise);
            lessonVoice.Say(sound, praise);

            lessonVoice.Say(praise, null);
            voice.FinishSpeaking();
            lessonVoice.Tick();

            Assert.That(voice.Spoken, Is.EqualTo(new[] { "ro-sound-a", "ro-line-praise-1" }));
            Assert.That(lessonVoice.IsBusy, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="LessonVoice.SayAgain"/> during the first line starts it again and keeps
        /// the praise after it; after both, it says the praise again alone.
        /// </summary>
        [Test]
        public void TestSayAgain_KeepsThePraise()
        {
            FakeVoice voice = new FakeVoice();
            LessonVoice lessonVoice = Prepared(voice, new List<string>(), out VoiceSequence sound, out VoiceSequence praise);
            lessonVoice.Say(sound, praise);

            lessonVoice.SayAgain();
            voice.FinishSpeaking();
            lessonVoice.Tick();
            voice.FinishSpeaking();
            lessonVoice.Tick();
            lessonVoice.SayAgain();

            Assert.That(voice.Spoken, Is.EqualTo(new[] { "ro-sound-a", "ro-sound-a", "ro-line-praise-1", "ro-line-praise-1" }));
        }

        /// <summary>
        /// Tests that <see cref="LessonVoice.Pause"/> stops the voice without moving on, and
        /// <see cref="LessonVoice.Resume"/> says the line again from its start, the praise still after it.
        /// </summary>
        [Test]
        public void TestPause_ResumeSaysTheLineAgain()
        {
            FakeVoice voice = new FakeVoice();
            LessonVoice lessonVoice = Prepared(voice, new List<string>(), out VoiceSequence sound, out VoiceSequence praise);
            lessonVoice.Say(sound, praise);

            lessonVoice.Pause();
            lessonVoice.Tick();
            bool speakingWhilePaused = voice.IsSpeaking;
            lessonVoice.Resume();
            voice.FinishSpeaking();
            lessonVoice.Tick();

            Assert.That(speakingWhilePaused, Is.False);
            Assert.That(voice.Spoken, Is.EqualTo(new[] { "ro-sound-a", "ro-sound-a", "ro-line-praise-1" }));
        }

        private static LessonVoice Prepared(FakeVoice voice, List<string> log, out VoiceSequence sound, out VoiceSequence praise)
        {
            Curriculum curriculum = RealContent.Read();
            sound = VoiceSequence.Of(new VoiceLineId(ContentId.Create("ro-sound-a")));
            praise = VoiceSequence.Of(new VoiceLineId(ContentId.Create("ro-line-praise-1")));
            LessonVoice lessonVoice = new LessonVoice(voice, new FakeSpeechBubble(log));
            lessonVoice.Prepare(curriculum, new[] { sound, praise });
            return lessonVoice;
        }
    }
}
