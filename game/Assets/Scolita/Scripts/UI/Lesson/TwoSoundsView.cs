using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>TwoSounds</c> (design screen 4): the letter card between the two little planets,
    /// the Romanian one on the left and the English one on the right (<see cref="SoundPlanet"/>), each
    /// with its badge, its tick once heard and its tray; and, once both sounds are heard, the picture in
    /// her hand (<see cref="HeldPicture"/>), to drag to a planet. It draws the round's state after every
    /// answer (the ticks, the picture in play, the trays), so it keeps none of its own. The prefab is
    /// <c>Prefabs/Lesson/TwoSoundsView.prefab</c>.
    /// </summary>
    public sealed class TwoSoundsView : ActivityView
    {
        [SerializeField] private ChoiceTile letterCard;
        [SerializeField] private SoundPlanet[] planets = new SoundPlanet[0];
        [SerializeField] private HeldPicture held;

        private IActivityInput input;
        private ActivityRound round;
        private PictureSprites pictures;

        /// <summary>The planets: the Romanian one, then the English one.</summary>
        public IReadOnlyList<SoundPlanet> Planets => planets;

        /// <summary>The picture in her hand.</summary>
        public HeldPicture Held => held;

        /// <summary>The letter card.</summary>
        public ChoiceTile LetterCard => letterCard;

        /// <inheritdoc/>
        public override void Show(ActivityRound shownRound, ActivityViewContext context)
        {
            round = shownRound;
            input = context.Input;
            pictures = context.Pictures;
            letterCard.ShowSubject(round.Subject, round.Subject.Text, null, input);
            planets[TwoSoundsRules.RomanianPlanet].Show(TwoSoundsRules.RomanianPlanet, round.Choices[TwoSoundsRules.RomanianPlanet], input);
            planets[TwoSoundsRules.EnglishPlanet].Show(TwoSoundsRules.EnglishPlanet, round.Choices[TwoSoundsRules.EnglishPlanet], input);
            gameObject.SetActive(true);
            Refresh();
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            int dropped = feedback.Choice;
            switch (feedback.Outcome)
            {
                case FeedbackOutcome.TryAgain:
                case FeedbackOutcome.ShowAnswer:
                    if (dropped >= 0 && dropped < planets.Length)
                    {
                        planets[dropped].Wobble();
                    }

                    held.Wobble();
                    if (feedback.Hint.Cue == HintCue.GlowAnswer && feedback.Hint.Choice >= 0 && feedback.Hint.Choice < planets.Length)
                    {
                        planets[feedback.Hint.Choice].SetGlowing(true);
                    }

                    break;
                case FeedbackOutcome.Right:
                case FeedbackOutcome.Done:
                    if (dropped >= 0 && dropped < planets.Length)
                    {
                        planets[dropped].SetGlowing(false);
                        planets[dropped].Sparkle();
                    }

                    Refresh();
                    break;
                default:
                    Refresh();
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// The picture in her hand was let go at <paramref name="screenPoint"/>, seen by
        /// <paramref name="eventCamera"/>: dropped on the planet under it, if any. Called by
        /// <see cref="HeldPicture"/>.
        /// </summary>
        public void PictureLetGo(Vector2 screenPoint, Camera eventCamera)
        {
            for (int planet = 0; planet < planets.Length; planet++)
            {
                if (planets[planet].Holds(screenPoint, eventCamera))
                {
                    input?.Answer(ActivityAnswer.Drop(planets[planet].Index));
                    return;
                }
            }
        }

        /// <summary>The picture in her hand was tapped: its word is said again. Called by <see cref="HeldPicture"/>.</summary>
        public void PictureTapped()
        {
            if (round != null && round.Current >= 0)
            {
                input?.Answer(ActivityAnswer.Tap(round.Current));
            }
        }

        private void Refresh()
        {
            for (int planet = 0; planet < planets.Length; planet++)
            {
                planets[planet].SetHeard(round.StateOf(planets[planet].Index) != ChoiceState.Open);
                planets[planet].EmptyTray();
            }

            for (int index = TwoSoundsRules.FirstPicture; index < round.Choices.Count; index++)
            {
                if (round.StateOf(index) == ChoiceState.Done)
                {
                    ActivityChoice picture = round.Choices[index];
                    planets[PlanetOf(picture)].AddToTray(picture, pictures.Of(picture.Picture));
                }
            }

            held.Hold(round.Current >= 0 ? pictures.Of(round.Choices[round.Current].Picture) : null);
        }

        private int PlanetOf(ActivityChoice picture)
        {
            for (int planet = 0; planet < planets.Length; planet++)
            {
                if (round.Choices[planets[planet].Index].Language == picture.Language)
                {
                    return planet;
                }
            }

            return 0;
        }

        private void Awake()
        {
            held.Bind(this);
        }
    }
}
