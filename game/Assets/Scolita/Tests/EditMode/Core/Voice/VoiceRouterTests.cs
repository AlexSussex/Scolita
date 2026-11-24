using System;
using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Voice
{
    /// <summary>Tests <see cref="VoiceRouter"/>: its choice of clip, system voice or bubble, splicing, slow words and replacing a line.</summary>
    public sealed class VoiceRouterTests
    {
        /// <summary>Tests that <see cref="VoiceRouter.Speak"/> plays a line's clip when the clip voice has it.</summary>
        [Test]
        public void TestSpeak_PlaysTheClipWhenThereIsOne()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log, VoiceTestContent.SoundM);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian, Language.English);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);
            List<string> events = new List<string>();
            router.Started += line => events.Add("started " + line);
            router.Finished += line => events.Add("finished " + line);

            router.Speak(VoiceTestContent.Line("ro-sound-m"));
            bool speakingWhilePlaying = router.IsSpeaking;
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(log, Is.EqualTo(new[] { "clip ro-sound-m.ogg" }));
            Assert.That(speakingWhilePlaying, Is.True);
            Assert.That(events, Is.EqualTo(new[] { "started ro-sound-m", "finished ro-sound-m" }));
            Assert.That(router.IsSpeaking, Is.False);
        }

        /// <summary>Tests that <see cref="VoiceRouter.Speak"/> gives a line with no clip to the system voice, in its language.</summary>
        [Test]
        public void TestSpeak_UsesTheSystemVoiceWithoutAClip()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian, Language.English);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);

            router.Speak(VoiceTestContent.Line("ro-name-m"));
            VoiceRoute route = router.ActiveRoute;
            VoiceTestContent.Drain(router, clips, system);
            router.Speak(VoiceTestContent.Line("en-name-m"));
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(route, Is.EqualTo(VoiceRoute.System));
            Assert.That(log, Is.EqualTo(new[] { "say ro me", "say en em" }));
        }

        /// <summary>
        /// Tests that <see cref="VoiceRouter.Speak"/> shows the bubble when the system voice lacks the
        /// line's language, keeps it for the reading time, then hides it and goes on.
        /// </summary>
        [Test]
        public void TestSpeak_ShowsTheBubbleWhenTheLanguageIsMissing()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.English);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);
            List<string> finished = new List<string>();
            router.Finished += line => finished.Add(line.ToString());

            router.Speak(VoiceTestContent.Line("ro-name-m"));
            router.Tick(1f);
            bool speakingBeforeTheReadingTime = router.IsSpeaking;
            router.Tick(1f);

            Assert.That(speakingBeforeTheReadingTime, Is.True);
            Assert.That(log, Is.EqualTo(new[] { "show ro me", "hide" }));
            Assert.That(finished, Is.EqualTo(new[] { "ro-name-m" }));
            Assert.That(router.IsSpeaking, Is.False);
        }

        /// <summary>Tests that <see cref="VoiceRouter.Speak"/> falls back to the system voice when a clip refuses to play, then to the bubble.</summary>
        [Test]
        public void TestSpeak_FallsBackWhenTheClipOrTheSystemVoiceRefuses()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log, VoiceTestContent.SoundM) { RefusesToPlay = true };
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);
            List<VoiceRoute> parts = new List<VoiceRoute>();
            router.PartStarted += part => parts.Add(part.Route);

            router.Speak(VoiceTestContent.Line("ro-sound-m"));
            VoiceTestContent.Drain(router, clips, system);
            system.RefusesToSay = true;
            router.Speak(VoiceTestContent.Line("ro-name-m"));
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(log, Is.EqualTo(new[] { "refused clip ro-sound-m.ogg", "say ro m", "show ro me", "hide" }));
            Assert.That(parts, Is.EqualTo(new[] { VoiceRoute.System, VoiceRoute.Bubble }));
        }

        /// <summary>Tests that <see cref="VoiceRouter.SpeakSequence"/> says a line's text, then plays the letter's sound clip in its slot.</summary>
        [Test]
        public void TestSpeakSequence_SplicesTheSoundClipIntoTheLine()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log, VoiceTestContent.SoundM);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);
            List<string> started = new List<string>();
            router.Started += line => started.Add(line.ToString());

            router.SpeakSequence(VoiceSequence.Of(VoiceTestContent.Line("ro-line-pick-letter-x"))
                .With(VoiceSlot.X, VoiceFill.OfLine(VoiceTestContent.Line("ro-sound-m"), "m")));
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(log, Is.EqualTo(new[] { "say ro Atinge litera", "clip ro-sound-m.ogg" }));
            Assert.That(started, Is.EqualTo(new[] { "ro-line-pick-letter-x" }));
        }

        /// <summary>Tests that <see cref="VoiceRouter.SpeakSequence"/> says a line with a spliced name as one utterance, so it keeps its tune.</summary>
        [Test]
        public void TestSpeakSequence_JoinsANameIntoOneUtterance()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);

            router.SpeakSequence(VoiceSequence.Of(VoiceTestContent.Line("ro-line-hello-name"))
                .With(VoiceSlot.Name, VoiceFill.OfText("Ana")));
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(log, Is.EqualTo(new[] { "say ro Bun\u0103, Ana! Hai s\u0103 zbur\u0103m!" }));
        }

        /// <summary>
        /// Tests that <see cref="VoiceRouter.SpeakSequence"/> says an ellipsis as a pause between two
        /// sound clips, and adds the closing "!" to the word said by the system voice.
        /// </summary>
        [Test]
        public void TestSpeakSequence_PausesAtAnEllipsis()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log, VoiceTestContent.SoundM);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);

            router.SpeakSequence(VoiceSequence.Of(VoiceTestContent.Line("ro-line-meet-listen"))
                .With(VoiceSlot.X, VoiceFill.OfLine(VoiceTestContent.Line("ro-sound-m"), "m"))
                .With(VoiceSlot.Word, VoiceFill.OfLine(VoiceTestContent.Line("ro-word-mama"), "mama")));
            system.FinishSaying();
            router.Tick(0f);
            clips.FinishPlaying();
            router.Tick(0f);
            VoiceRoute afterTheFirstClip = router.ActiveRoute;
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(afterTheFirstClip, Is.EqualTo(VoiceRoute.Pause));
            Assert.That(log, Is.EqualTo(new[]
            {
                "say ro Ascult\u0103:", "clip ro-sound-m.ogg", "clip ro-sound-m.ogg", "say ro mama!"
            }));
        }

        /// <summary>
        /// Tests that <see cref="VoiceRouter.Speak"/> says a slow word with no clip of its own as its
        /// letters' sounds in order, with a pause between them (brief item 10).
        /// </summary>
        [Test]
        public void TestSpeak_SlowWordFromItsLettersSounds()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log, VoiceTestContent.SoundM, VoiceTestContent.SoundA);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);

            router.Speak(VoiceTestContent.Line("ro-slow-mama"));
            clips.FinishPlaying();
            router.Tick(0.1f);
            router.Tick(0.1f);
            int clipsBeforeThePauseEnds = log.Count;
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(clipsBeforeThePauseEnds, Is.EqualTo(1));
            Assert.That(log, Is.EqualTo(new[] { "clip ro-sound-m.ogg", "clip ro-sound-a.ogg", "clip ro-sound-m.ogg", "clip ro-sound-a.ogg" }));
        }

        /// <summary>
        /// Tests that <see cref="VoiceRouter.Speak"/> says a stretched word with no clip of its own as its first
        /// sound, a pause, then the word, never its text "mmmama" through the tablet's voice.
        /// </summary>
        [Test]
        public void TestSpeak_StretchedWordFromItsFirstSound()
        {
            List<string> log = new List<string>();
            Dictionary<string, string> csvs = VoiceTestContent.Csvs();
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-stretched-mama,ro,stretched,mmmama,,,,");
            FakeClipVoice clips = new FakeClipVoice(log, VoiceTestContent.SoundM, VoiceTestContent.SoundA);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(csvs), clips, system, new FakeSpeechBubble(log),
                VoiceTestContent.Timing);

            router.Speak(VoiceTestContent.Line("ro-stretched-mama"));
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(log, Is.EqualTo(new[] { "clip ro-sound-m.ogg", "say ro mama" }));
        }

        /// <summary>Tests that <see cref="VoiceRouter.Speak"/> plays a slow word's own clip when it has one.</summary>
        [Test]
        public void TestSpeak_SlowWordWithItsOwnClip()
        {
            List<string> log = new List<string>();
            Dictionary<string, string> csvs = VoiceTestContent.Csvs();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-slow-mama,", "ro-slow-mama,ro,slow,m-a-m-a,ro-slow-mama.ogg,recorded,,");
            FakeClipVoice clips = new FakeClipVoice(log, VoiceTestContent.SoundM, VoiceTestContent.SoundA, "ro-slow-mama.ogg");
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(csvs), clips, system, new FakeSpeechBubble(log),
                VoiceTestContent.Timing);

            router.Speak(VoiceTestContent.Line("ro-slow-mama"));
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(log, Is.EqualTo(new[] { "clip ro-slow-mama.ogg" }));
        }

        /// <summary>Tests that <see cref="VoiceRouter.Speak"/> stops the line being said before the new one, so two never overlap.</summary>
        [Test]
        public void TestSpeak_ANewLineStopsTheOneBeingSaid()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log, VoiceTestContent.SoundM);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);
            List<string> events = new List<string>();
            router.Started += line => events.Add("started " + line);
            router.Finished += line => events.Add("finished " + line);

            router.Speak(VoiceTestContent.Line("ro-sound-m"));
            router.Speak(VoiceTestContent.Line("ro-name-m"));
            VoiceTestContent.Drain(router, clips, system);

            Assert.That(log, Is.EqualTo(new[] { "clip ro-sound-m.ogg", "stop clip", "say ro me" }));
            Assert.That(events, Is.EqualTo(new[]
            {
                "started ro-sound-m", "finished ro-sound-m", "started ro-name-m", "finished ro-name-m"
            }));
        }

        /// <summary>Tests that <see cref="VoiceRouter.Stop"/> stops the system voice at once and raises <see cref="VoiceRouter.Finished"/>.</summary>
        [Test]
        public void TestStop_StopsAtOnce()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);
            List<string> finished = new List<string>();
            router.Finished += line => finished.Add(line.ToString());

            router.Speak(VoiceTestContent.Line("ro-name-m"));
            router.Stop();
            router.Stop();

            Assert.That(log, Is.EqualTo(new[] { "say ro me", "stop system" }));
            Assert.That(finished, Is.EqualTo(new[] { "ro-name-m" }));
            Assert.That(router.IsSpeaking, Is.False);
        }

        /// <summary>Tests that <see cref="VoiceRouter.Speak"/> reports a line that isn't in the content and says nothing, never crashing.</summary>
        [Test]
        public void TestSpeak_MissingLineIsReportedAndSkipped()
        {
            List<string> log = new List<string>();
            FakeClipVoice clips = new FakeClipVoice(log);
            FakeSystemVoice system = new FakeSystemVoice(log, Language.Romanian);
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), clips, system,
                new FakeSpeechBubble(log), VoiceTestContent.Timing);
            List<string> events = new List<string>();
            router.LineMissing += line => events.Add("missing " + line);
            router.Started += line => events.Add("started " + line);

            router.Speak(VoiceTestContent.Line("ro-line-not-there"));

            Assert.That(events, Is.EqualTo(new[] { "missing ro-line-not-there" }));
            Assert.That(log, Is.Empty);
            Assert.That(router.IsSpeaking, Is.False);
        }

        /// <summary>Tests that <see cref="VoiceRouter.Speak"/> and <see cref="VoiceRouter.SpeakSequence"/> refuse a line whose slot has no fill.</summary>
        [Test]
        public void TestSpeak_ASlotWithoutItsFillThrows()
        {
            List<string> log = new List<string>();
            VoiceRouter router = new VoiceRouter(VoiceTestContent.Read(VoiceTestContent.Csvs()), new FakeClipVoice(log),
                new FakeSystemVoice(log, Language.Romanian), new FakeSpeechBubble(log), VoiceTestContent.Timing);

            Assert.That(() => router.Speak(VoiceTestContent.Line("ro-line-pick-letter-x")),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contain("slot X"));
            Assert.That(() => router.SpeakSequence(VoiceSequence.Of(VoiceTestContent.Line("ro-line-pick-letter-x"))
                    .With(VoiceSlot.Word, VoiceFill.OfText("mama"))),
                Throws.TypeOf<InvalidOperationException>());
            Assert.That(router.IsSpeaking, Is.False);
        }
    }
}
