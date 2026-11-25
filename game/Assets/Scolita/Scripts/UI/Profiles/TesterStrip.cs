using Scolita.Game.Profiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Profiles
{
    /// <summary>
    /// The tester's strip (<c>Prefabs/Profiles/TesterStrip.prefab</c>, its own canvas over the screen): a band at the
    /// bottom with a "TEST" badge, the test information and, in a lesson, "Sari peste pas" (64 px). Hidden for a child.
    /// </summary>
    public sealed class TesterStrip : TesterStripView
    {
        [SerializeField] private GameObject strip;
        [SerializeField] private TMP_Text info;
        [SerializeField] private Button skipButton;

        private ITesterInput input;

        /// <summary>Whether the strip is shown.</summary>
        public bool IsShown => strip.activeSelf;

        /// <summary>The test information shown.</summary>
        public string Info => info.text;

        /// <summary>"Sari peste pas".</summary>
        public Button SkipButton => skipButton;

        /// <inheritdoc/>
        public override void Bind(ITesterInput testerInput)
        {
            input = testerInput;
        }

        /// <inheritdoc/>
        public override void Show(bool shown, bool canSkip)
        {
            strip.SetActive(shown);
            skipButton.gameObject.SetActive(shown && canSkip);
        }

        /// <inheritdoc/>
        public override void ShowInfo(string text)
        {
            info.SetText(text);
        }

        private void Awake()
        {
            strip.SetActive(false);
            skipButton.onClick.AddListener(OnSkip);
        }

        private void OnDestroy()
        {
            skipButton.onClick.RemoveListener(OnSkip);
        }

        private void OnSkip()
        {
            input?.SkipStep();
        }
    }
}
