using Scolita.Game.Rewards;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// A book over the map or the lesson's end (the sky book, the sticker book, the trail picker): a full-screen page on
    /// the purple sky that keeps her taps from what is under it, with the back button top left (AGENTS.md → "Always a
    /// way out"), which goes to <see cref="IRewardsInput.CloseBook"/>.
    /// </summary>
    public abstract class BookPage : MonoBehaviour
    {
        [SerializeField] private Button backButton;

        /// <summary>Where its taps go; set by <see cref="Bind"/>.</summary>
        protected IRewardsInput Input { get; private set; }

        /// <summary>Whether it is open.</summary>
        public bool IsOpen => gameObject.activeSelf;

        /// <summary>The back button.</summary>
        public Button BackButton => backButton;

        /// <summary>Sends its taps to <paramref name="rewardsInput"/>; called by <see cref="RewardScreens"/>.</summary>
        public void Bind(IRewardsInput rewardsInput)
        {
            Input = rewardsInput;
        }

        /// <summary>Closes it; called by <see cref="RewardScreens"/>.</summary>
        public void Close()
        {
            gameObject.SetActive(false);
        }

        /// <summary>Opens it on top of its siblings; called by the page's own show method.</summary>
        protected void Open()
        {
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            backButton.onClick.AddListener(OnBack);
        }

        private void OnDisable()
        {
            backButton.onClick.RemoveListener(OnBack);
        }

        private void OnBack()
        {
            Input?.CloseBook();
        }
    }
}
