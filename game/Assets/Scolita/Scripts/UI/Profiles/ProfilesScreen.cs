using System.Collections.Generic;
using Scolita.Core.Profiles;
using Scolita.Game.Parent;
using Scolita.Game.Profiles;
using Scolita.Game.Voice;
using Scolita.UI.Lesson;
using Scolita.UI.Parent;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Profiles
{
    /// <summary>
    /// "Who's playing?" (design screen 9, <c>Prefabs/Profiles/ProfilesScreen.prefab</c>): the purple sky, the mascot at the
    /// top with its bubble, one big card per child (one to three in a row, four to six in two rows; each at least 96 px),
    /// and the parent button top right with its gate. Its taps go to <see cref="IProfilesInput"/> (the Profiles scene's
    /// <c>ProfilesRunner</c>).
    /// </summary>
    public sealed class ProfilesScreen : ProfilesScreenView
    {
        [SerializeField] private SpeechBubbleView bubble;
        [SerializeField] private GuideFigure guide;
        [SerializeField] private ParentGatePanel parentGate;
        [SerializeField] private GridLayoutGroup cardsGrid;
        [SerializeField] private ProfileCardView cardTemplate;
        [SerializeField] private Vector2 largeCard = new Vector2(300f, 400f);
        [SerializeField] private Vector2 smallCard = new Vector2(210f, 270f);
        [SerializeField, Min(1)] private int cardsInALargeRow = 3;

        private readonly List<ProfileCardView> cards = new List<ProfileCardView>();
        private IProfilesInput input;

        /// <inheritdoc/>
        public override SpeechBubbleView Bubble => bubble;

        /// <inheritdoc/>
        public override ParentGateView ParentGate => parentGate;

        /// <summary>The cards drawn, in order.</summary>
        public IReadOnlyList<ProfileCardView> Cards => cards;

        /// <summary>The gate's panel and the parent button.</summary>
        public ParentGatePanel GatePanel => parentGate;

        /// <summary>The mascot.</summary>
        public GuideFigure Guide => guide;

        /// <inheritdoc/>
        public override void Bind(IProfilesInput profilesInput)
        {
            input = profilesInput;
            guide.Bind(profilesInput);
        }

        /// <inheritdoc/>
        public override void Show(IReadOnlyList<ProfileCard> shown)
        {
            bool large = shown.Count <= cardsInALargeRow;
            cardsGrid.cellSize = large ? largeCard : smallCard;
            cardsGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            cardsGrid.constraintCount = large ? Mathf.Max(1, shown.Count) : (shown.Count + 1) / 2;
            foreach (ProfileCard card in shown)
            {
                ProfileCardView view = Instantiate(cardTemplate, cardsGrid.transform);
                view.gameObject.SetActive(true);
                view.Show(card);
                ProfileId id = card.Id;
                view.Button.onClick.AddListener(() => input?.Choose(id));
                cards.Add(view);
            }
        }

        /// <inheritdoc/>
        public override void ShowChosen(ProfileId child)
        {
            foreach (ProfileCardView card in cards)
            {
                card.SetChosen(card.Id == child);
            }
        }

        /// <inheritdoc/>
        public override void SetTalking(bool talking)
        {
            guide.SetTalking(talking);
        }

        private void Awake()
        {
            cardTemplate.gameObject.SetActive(false);
        }
    }
}
