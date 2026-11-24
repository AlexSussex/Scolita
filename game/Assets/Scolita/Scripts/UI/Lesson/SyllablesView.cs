using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>Syllables</c>: the word's picture card (the round's subject, its word written under it when
    /// she can read it; a tap says the word again), the big drum (<see cref="SyllableDrum"/>), the row of beats
    /// under the card (one lit dot per beat she taps, <see cref="ActivityRound.PlacesFilled"/>), and the green tick
    /// she taps when she has finished. Once the answer is shown, the row also shows one glowing empty circle per
    /// syllable (<see cref="ActivityRound.Places"/>) for her to fill, as the Romanian manuals draw them. It draws
    /// the round's beats after every answer, so it keeps no count of its own. The prefab is
    /// <c>Prefabs/Lesson/SyllablesView.prefab</c>.
    /// </summary>
    public sealed class SyllablesView : ActivityView
    {
        [SerializeField] private ChoiceTile pictureCard;
        [SerializeField] private SyllableDrum drum;
        [SerializeField] private Button tick;
        [SerializeField] private Graphic[] beats = new Graphic[0];
        [SerializeField] private Color beatColour = new Color(1f, 0.851f, 0.239f, 1f);
        [SerializeField] private Color placeColour = new Color(1f, 1f, 1f, 0.55f);
        [SerializeField] private float placePulsesPerSecond = 1.2f;

        private IActivityInput input;
        private ActivityRound round;
        private bool placesShown;

        /// <summary>The picture card.</summary>
        public ChoiceTile PictureCard => pictureCard;

        /// <summary>The drum.</summary>
        public SyllableDrum Drum => drum;

        /// <summary>The tick she taps when she has finished.</summary>
        public Button Tick => tick;

        /// <summary>How many beats are lit.</summary>
        public int BeatsLit
        {
            get
            {
                int lit = 0;
                for (int index = 0; index < beats.Length; index++)
                {
                    if (beats[index].enabled && beats[index].color == beatColour)
                    {
                        lit++;
                    }
                }

                return lit;
            }
        }

        /// <summary>How many empty circles are shown (the answer shown); 0 before.</summary>
        public int PlacesShown
        {
            get
            {
                int shown = 0;
                for (int index = 0; index < beats.Length; index++)
                {
                    if (beats[index].enabled && beats[index].color != beatColour)
                    {
                        shown++;
                    }
                }

                return shown;
            }
        }

        /// <inheritdoc/>
        public override void Show(ActivityRound shownRound, ActivityViewContext context)
        {
            round = shownRound;
            input = context.Input;
            placesShown = false;
            ActivityChoice word = round.Subject;
            string text = word.Look == ActivityChoiceLook.Word ? word.Text : string.Empty;
            pictureCard.ShowSubject(word, text, context.Pictures.Of(word.Picture), input);
            if (round.Places > beats.Length)
            {
                Debug.LogError("Lesson: a Syllables word has " + round.Places + " syllables; the view shows " + beats.Length + ".");
            }

            gameObject.SetActive(true);
            Refresh();
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            switch (feedback.Outcome)
            {
                case FeedbackOutcome.Neutral:
                    if (feedback.Choice == SyllablesRules.Drum)
                    {
                        drum.Bounce();
                    }

                    break;
                case FeedbackOutcome.TryAgain:
                    pictureCard.Wobble();
                    break;
                case FeedbackOutcome.ShowAnswer:
                    pictureCard.Wobble();
                    placesShown = placesShown || feedback.Hint.Cue == HintCue.ShowPlaces;
                    break;
                default:
                    pictureCard.Sparkle();
                    break;
            }

            Refresh();
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>The drum was tapped: a beat. Called by <see cref="SyllableDrum"/>.</summary>
        public void DrumTapped()
        {
            input?.Answer(ActivityAnswer.Tap(SyllablesRules.Drum));
        }

        private void Awake()
        {
            drum.Bind(this);
            tick.onClick.AddListener(OnTick);
        }

        private void OnDestroy()
        {
            tick.onClick.RemoveListener(OnTick);
        }

        private void OnTick()
        {
            input?.Answer(ActivityAnswer.Tap(SyllablesRules.Tick));
        }

        private void Refresh()
        {
            for (int index = 0; index < beats.Length; index++)
            {
                bool beat = index < round.PlacesFilled;
                bool place = placesShown && index < round.Places;
                beats[index].enabled = beat || place;
                beats[index].color = beat ? beatColour : placeColour;
                beats[index].canvasRenderer.SetAlpha(1f);
            }
        }

        private void Update()
        {
            if (!placesShown)
            {
                return;
            }

            float pulse = 0.6f + 0.4f * Mathf.Sin(Time.unscaledTime * placePulsesPerSecond * 2f * Mathf.PI);
            for (int index = round.PlacesFilled; index < beats.Length && index < round.Places; index++)
            {
                beats[index].canvasRenderer.SetAlpha(pulse);
            }
        }
    }
}
