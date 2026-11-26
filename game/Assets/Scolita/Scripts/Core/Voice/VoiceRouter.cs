using System;
using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// The app's voice (<see cref="IVoice"/>): says each part of a line by its clip, else by the
    /// system voice, else in the bubble (<see cref="VoicePlanner"/> chooses), one after another, and
    /// never two lines at once: a new line stops the one being said. A part for the system voice waits
    /// while that voice gets ready (<see cref="ISystemVoice.IsStarting"/>, up to the timing's wait), so
    /// the first line after the app starts or comes back is heard; a newer line replaces the waiting one.
    /// Driven by <see cref="Tick"/> every frame; nothing allocates while it speaks or waits. A line that isn't in the content is reported
    /// through <see cref="LineMissing"/> and skipped.
    /// </summary>
    public sealed class VoiceRouter : IVoice
    {
        private const int PlanCapacity = 32;

        private readonly VoicePlanner planner;
        private readonly IClipVoice clips;
        private readonly ISystemVoice system;
        private readonly ISpeechBubble bubble;
        private readonly VoiceTiming timing;
        private readonly List<VoiceUtterance> plan = new List<VoiceUtterance>(PlanCapacity);
        private readonly List<VoiceLineId> missing = new List<VoiceLineId>(4);
        private int index;
        private VoiceRoute activeRoute;
        private float remainingSeconds;
        private bool bubbleShown;
        private bool waitingForSystem;
        private float waitLeft;
        private int generation;

        /// <summary>
        /// Creates the voice for <paramref name="curriculum"/>'s lines, saying them through
        /// <paramref name="clips"/>, <paramref name="system"/> and <paramref name="bubble"/>. Called by
        /// <c>VoiceHost</c> and <c>VoiceRouterTests</c>.
        /// </summary>
        public VoiceRouter(Curriculum curriculum, IClipVoice clips, ISystemVoice system, ISpeechBubble bubble,
            VoiceTiming timing)
        {
            planner = new VoicePlanner(curriculum, clips, system, timing);
            this.clips = clips;
            this.system = system;
            this.bubble = bubble;
            this.timing = timing;
        }

        /// <inheritdoc/>
        public event Action<VoiceLineId> Started;

        /// <inheritdoc/>
        public event Action<VoiceLineId> Finished;

        /// <summary>
        /// Raised as each part starts, with how it is really said (after any fallback): for the
        /// development log now, and for a guide that moves its mouth or shows the words later.
        /// </summary>
        public event Action<VoiceUtterance> PartStarted;

        /// <summary>Raised for each line asked for that isn't in the content (the Unity side logs it as an error).</summary>
        public event Action<VoiceLineId> LineMissing;

        /// <inheritdoc/>
        public bool IsSpeaking { get; private set; }

        /// <inheritdoc/>
        public VoiceLineId Current { get; private set; }

        /// <summary>How the part being said is said, when <see cref="IsSpeaking"/>.</summary>
        public VoiceRoute ActiveRoute => activeRoute;

        /// <summary>
        /// Says <paramref name="line"/>, stopping whatever was being said. Throws an
        /// <see cref="InvalidOperationException"/> for a line with slots. Called by the activities,
        /// <c>Hearable</c> and the voice tests.
        /// </summary>
        public void Speak(VoiceLineId line)
        {
            if (!StopSpeakingFor())
            {
                return;
            }

            plan.Clear();
            missing.Clear();
            planner.PlanLine(line, plan, missing);
            Begin(line);
        }

        /// <summary>
        /// Says <paramref name="sequence"/>, stopping whatever was being said. Throws an
        /// <see cref="InvalidOperationException"/> for a slot left without its fill. Called by the
        /// activities, the lesson's voice and the voice tests.
        /// </summary>
        public void SpeakSequence(VoiceSequence sequence)
        {
            if (!StopSpeakingFor())
            {
                return;
            }

            plan.Clear();
            missing.Clear();
            planner.PlanSequence(sequence, plan, missing);
            Begin(sequence.Lead);
        }

        /// <summary>Stops at once, raising <see cref="Finished"/>; called by the scenes when they close and the voice tests.</summary>
        public void Stop()
        {
            StopSpeaking();
        }

        /// <summary>
        /// Moves on by <paramref name="seconds"/>: starts the next part when the one being said is
        /// done. Called every frame by <c>VoiceHost</c>, and by the voice tests.
        /// </summary>
        public void Tick(float seconds)
        {
            if (!IsSpeaking)
            {
                return;
            }

            if (waitingForSystem)
            {
                waitLeft -= seconds;
                if (system.IsStarting && waitLeft > 0f)
                {
                    return;
                }

                waitingForSystem = false;
                SayOrShow(plan[index], false);
                return;
            }

            switch (activeRoute)
            {
                case VoiceRoute.Clip:
                    if (!clips.IsPlaying)
                    {
                        Advance();
                    }

                    break;
                case VoiceRoute.System:
                    if (!system.IsSaying)
                    {
                        Advance();
                    }

                    break;
                default:
                    remainingSeconds -= seconds;
                    if (remainingSeconds <= 0f)
                    {
                        HideBubble();
                        Advance();
                    }

                    break;
            }
        }

        private void Begin(VoiceLineId lead)
        {
            for (int missingIndex = 0; missingIndex < missing.Count; missingIndex++)
            {
                LineMissing?.Invoke(missing[missingIndex]);
            }

            if (plan.Count == 0)
            {
                return;
            }

            generation++;
            int started = generation;
            Current = lead;
            IsSpeaking = true;
            index = -1;
            Started?.Invoke(lead);
            if (started == generation && IsSpeaking)
            {
                Advance();
            }
        }

        private void Advance()
        {
            index++;
            if (index >= plan.Count)
            {
                Finish();
                return;
            }

            VoiceUtterance utterance = plan[index];
            switch (utterance.Route)
            {
                case VoiceRoute.Clip:
                    if (clips.Play(utterance.Clip))
                    {
                        activeRoute = VoiceRoute.Clip;
                        PartStarted?.Invoke(utterance);
                    }
                    else
                    {
                        SayOrShow(utterance, true);
                    }

                    break;
                case VoiceRoute.System:
                    SayOrShow(utterance, true);
                    break;
                case VoiceRoute.Bubble:
                    ShowBubble(utterance);
                    break;
                default:
                    activeRoute = VoiceRoute.Pause;
                    remainingSeconds = utterance.Seconds;
                    PartStarted?.Invoke(utterance);
                    break;
            }
        }

        private void SayOrShow(VoiceUtterance utterance, bool mayWait)
        {
            if (mayWait && system.IsStarting)
            {
                WaitForSystem();
                return;
            }

            if (system.Speaks(utterance.Language) && system.Say(utterance.Text, utterance.Language))
            {
                activeRoute = VoiceRoute.System;
                PartStarted?.Invoke(new VoiceUtterance(VoiceRoute.System, utterance.Clip, utterance.Text, utterance.Language, 0f));
                return;
            }

            if (mayWait && system.IsStarting)
            {
                WaitForSystem();
                return;
            }

            ShowBubble(utterance);
        }

        private void WaitForSystem()
        {
            activeRoute = VoiceRoute.Pause;
            waitingForSystem = true;
            waitLeft = timing.SystemVoiceWaitSeconds;
        }

        private void ShowBubble(VoiceUtterance utterance)
        {
            activeRoute = VoiceRoute.Bubble;
            remainingSeconds = timing.BubbleSeconds(utterance.Text);
            bubble.Show(utterance.Text, utterance.Language);
            bubbleShown = true;
            PartStarted?.Invoke(new VoiceUtterance(VoiceRoute.Bubble, utterance.Clip, utterance.Text, utterance.Language, 0f));
        }

        private void HideBubble()
        {
            if (bubbleShown)
            {
                bubbleShown = false;
                bubble.Hide();
            }
        }

        private void Finish()
        {
            waitingForSystem = false;
            HideBubble();
            IsSpeaking = false;
            Finished?.Invoke(Current);
        }

        private bool StopSpeakingFor()
        {
            int before = generation;
            StopSpeaking();
            return generation == before;
        }

        private void StopSpeaking()
        {
            if (!IsSpeaking)
            {
                return;
            }

            if (activeRoute == VoiceRoute.Clip)
            {
                clips.Stop();
            }
            else if (activeRoute == VoiceRoute.System)
            {
                system.Stop();
            }

            Finish();
        }
    }
}
