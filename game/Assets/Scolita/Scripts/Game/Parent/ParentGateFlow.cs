using System;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Core.Voice;
using Scolita.Game.Settings;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// The parent gate on the map: Core's <see cref="ParentGate"/>, the parent button and the panel
    /// (<see cref="ParentGateView"/>), and the number said in the app's language through <see cref="IVoice"/>
    /// (<c>&lt;lang&gt;-line-gate-&lt;n&gt;</c>), never in the mascot's bubble. If no voice can say it, the panel shows
    /// the digits. A wrong number or the close button hides the panel quietly; the right one opens the parent area.
    /// Made by <c>MapRunner</c>, which ticks it every frame and forwards the voice's parts; allocates only when the
    /// gate asks.
    /// </summary>
    public sealed class ParentGateFlow : IParentGateInput
    {
        private const string LineKeyPrefix = "gate-";
        private const string WordKeyPrefix = "number-";
        private const float MostHoldSecondsPerFrame = 0.1f;

        private readonly ParentGate gate;
        private readonly IVoice voice;
        private readonly ParentGateView view;
        private readonly Language language;
        private readonly Action asking;
        private readonly Action opened;
        private readonly string[] words;
        private bool pressed;
        private VoiceLineId askedLine;

        /// <summary>
        /// The gate <paramref name="gate"/>, drawn by <paramref name="view"/>, saying its number with
        /// <paramref name="voice"/> in <paramref name="language"/> (the app's); <paramref name="asking"/> runs when the
        /// panel opens (the map hushes), <paramref name="opened"/> when the right number is chosen. Called by
        /// <c>MapRunner</c> and the PlayMode tests.
        /// </summary>
        public ParentGateFlow(ParentGate gate, IVoice voice, ParentGateView view, Language language, Action asking,
            Action opened)
        {
            this.gate = gate ?? throw new ArgumentNullException(nameof(gate));
            this.voice = voice ?? throw new ArgumentNullException(nameof(voice));
            this.view = view != null ? view : throw new ArgumentNullException(nameof(view));
            this.language = language;
            this.asking = asking ?? throw new ArgumentNullException(nameof(asking));
            this.opened = opened ?? throw new ArgumentNullException(nameof(opened));
            words = new string[gate.Choices.Count];
        }

        /// <summary>Whether the panel asks for the number.</summary>
        public bool IsAsking => gate.Stage == GateStage.Asking;

        /// <summary>Where the gate is (shut, held, asking, open).</summary>
        public GateStage Stage => gate.Stage;

        /// <summary>The number said, while the panel asks.</summary>
        public int AskedNumber => gate.Number;

        /// <summary>The voice line of a number said by the gate in <paramref name="language"/>; called by the flow and the tests.</summary>
        public static VoiceLineId LineOf(Language language, int number)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, LineKeyPrefix + number);
        }

        /// <summary>The string table key of a number written as a word; called by the flow and the tests.</summary>
        public static string WordKeyOf(int number)
        {
            return WordKeyPrefix + number;
        }

        /// <inheritdoc/>
        public void Press()
        {
            pressed = true;
        }

        /// <inheritdoc/>
        public void Release()
        {
            pressed = false;
            gate.Release();
            view.SetHold(gate.HoldProgress);
        }

        /// <inheritdoc/>
        public void Choose(int index)
        {
            if (!IsAsking || index < 0 || index >= gate.Choices.Count)
            {
                return;
            }

            bool right = gate.Choose(gate.Choices[index]);
            HidePanel();
            if (right)
            {
                opened();
            }
        }

        /// <inheritdoc/>
        public void Listen()
        {
            if (IsAsking)
            {
                voice.Speak(askedLine);
            }
        }

        /// <inheritdoc/>
        public void Close()
        {
            if (!IsAsking)
            {
                return;
            }

            gate.Shut();
            HidePanel();
        }

        /// <summary>
        /// Lets a rest after a wrong number pass, fills the ring while the button is held (a frame counts as a tenth of a
        /// second at most, so a hitch or a return from the background never fills it at once), and asks once it is full.
        /// Called every frame by <c>MapRunner</c> and the PlayMode tests.
        /// </summary>
        public void Tick(float seconds)
        {
            gate.Pass(seconds);
            if (!pressed || gate.Stage == GateStage.Open)
            {
                return;
            }

            if (gate.Hold(Math.Min(seconds, MostHoldSecondsPerFrame)))
            {
                pressed = false;
                Ask();
            }

            view.SetHold(gate.HoldProgress);
        }

        /// <summary>
        /// A part of a line has started: when it is the gate's number shown in the bubble (no clip, no tablet voice for
        /// the language), the panel shows the digits instead. Called by <c>MapRunner</c> for every part.
        /// </summary>
        public void PartStarted(VoiceUtterance part)
        {
            if (IsAsking && part.Route == VoiceRoute.Bubble && voice.Current == askedLine)
            {
                view.ShowNumber(gate.Number);
            }
        }

        private void Ask()
        {
            for (int index = 0; index < words.Length; index++)
            {
                words[index] = AppLocale.Text(WordKeyOf(gate.Choices[index]));
            }

            asking();
            view.Ask(words);
            askedLine = LineOf(language, gate.Number);
            voice.Speak(askedLine);
        }

        private void HidePanel()
        {
            voice.Stop();
            view.Hide();
            view.SetHold(gate.HoldProgress);
        }
    }
}
