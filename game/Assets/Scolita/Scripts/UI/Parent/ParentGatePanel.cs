using System.Collections.Generic;
using System.Globalization;
using Scolita.Game.Parent;
using Scolita.Game.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// The parent gate's panel over the map: "Pentru părinți", "Atinge numărul pe care îl auzi.", a listen button, the
    /// numbers written as words (64 px tall or more) and a close button; opaque, so the mascot's bubble under it never
    /// shows the number. Its words come from the string tables, in the app's language. With the parent button
    /// (<see cref="ParentHoldButton"/>) it is the <see cref="ParentGateView"/> of <c>MapScreen.prefab</c>.
    /// </summary>
    public sealed class ParentGatePanel : ParentGateView
    {
        private const string AskKey = "gate-ask";
        private const string AskDigitsKey = "gate-ask-digits";

        [SerializeField] private ParentHoldButton holdButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text question;
        [SerializeField] private Button listenButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button[] choiceButtons = new Button[0];
        [SerializeField] private TMP_Text[] choiceLabels = new TMP_Text[0];

        private IParentGateInput input;

        /// <inheritdoc/>
        public override bool IsAsking => panel.activeSelf;

        /// <summary>The parent button.</summary>
        public ParentHoldButton HoldButton => holdButton;

        /// <summary>The numbers' buttons, in their places.</summary>
        public IReadOnlyList<Button> ChoiceButtons => choiceButtons;

        /// <summary>The number's word in place <paramref name="index"/>; read by the tests.</summary>
        public string ChoiceWord(int index)
        {
            return choiceLabels[index].text;
        }

        /// <summary>The question shown (with the digits when no voice can say the number).</summary>
        public string Question => question.text;

        /// <summary>The close button.</summary>
        public Button CloseButton => closeButton;

        /// <inheritdoc/>
        public override void Bind(IParentGateInput gateInput)
        {
            input = gateInput;
            holdButton.Bind(gateInput);
        }

        /// <inheritdoc/>
        public override void SetHold(float progress)
        {
            holdButton.SetFill(progress);
        }

        /// <inheritdoc/>
        public override void Ask(IReadOnlyList<string> words)
        {
            question.SetText(AppLocale.Text(AskKey));
            for (int index = 0; index < choiceButtons.Length; index++)
            {
                bool shown = index < words.Count;
                choiceButtons[index].gameObject.SetActive(shown);
                if (shown)
                {
                    choiceLabels[index].SetText(words[index]);
                }
            }

            panel.SetActive(true);
        }

        /// <inheritdoc/>
        public override void ShowNumber(int number)
        {
            question.SetText(AppLocale.Text(AskDigitsKey, number.ToString(CultureInfo.InvariantCulture)));
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            panel.SetActive(false);
        }

        private void Awake()
        {
            panel.SetActive(false);
            listenButton.onClick.AddListener(OnListen);
            closeButton.onClick.AddListener(OnClose);
            for (int index = 0; index < choiceButtons.Length; index++)
            {
                int place = index;
                choiceButtons[index].onClick.AddListener(() => input?.Choose(place));
            }
        }

        private void OnDestroy()
        {
            listenButton.onClick.RemoveListener(OnListen);
            closeButton.onClick.RemoveListener(OnClose);
            foreach (Button button in choiceButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        private void OnListen()
        {
            input?.Listen();
        }

        private void OnClose()
        {
            input?.Close();
        }
    }
}
