using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Tracing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>
    /// Tests <see cref="LessonRun"/> over every lesson of the real content (planets 1–3), one case per lesson: each step
    /// is played by its kind's rules from start to end, so a content change that leaves a step unplayable (no rules, a
    /// missing shape) or unfinishable fails here. The answers are the ones the rules show as right; whether the content
    /// itself is right (the words, the shapes) is the validator's and the human's check.
    /// </summary>
    public sealed class RealLessonsTests
    {
        /// <summary>
        /// Tests that <see cref="LessonRun.Answer"/> takes the lesson <paramref name="lessonId"/> to its end in both
        /// languages with every kind playable: none of its steps is skipped, each ends, and each answer the rules show as
        /// right (for tracing, a line along each stroke) is judged right.
        /// </summary>
        [TestCaseSource(nameof(LessonIds))]
        public void TestAnswer_TheLessonToItsEndWithRightAnswers(string lessonId)
        {
            Curriculum curriculum = RealContent.Read();
            ActivityKind[] kinds = (ActivityKind[])Enum.GetValues(typeof(ActivityKind));
            Lesson lesson = RealContent.Lesson(curriculum, lessonId);
            LessonPlan plan = LessonPlan.Of(lesson, LanguageMix.Both, new PlayableSteps(kinds, curriculum), Array.Empty<Step>(), 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            List<AnswerResult> results = new List<AnswerResult>();
            run.AnswerChecked += answer => results.Add(answer.Result);

            Assert.That(plan.Steps.Count, Is.EqualTo(lesson.Steps.Count), "a step is skipped");
            run.Start();
            for (int step = 0; step < run.StepCount; step++)
            {
                string where = "step " + (step + 1) + " (" + run.Current.Kind + ")";
                AnswerRight(run, where);
                Assert.That(run.State, Is.EqualTo(LessonRunState.StepDone), where + " didn't end");
                run.Next();
            }

            Assert.That(run.State, Is.EqualTo(LessonRunState.Finished));
            Assert.That(results, Is.All.EqualTo(AnswerResult.Right));
        }

        /// <summary>
        /// Tests that the real content holds planets 1–3's lessons, so <see cref="TestAnswer_TheLessonToItsEndWithRightAnswers"/>
        /// walks all of them.
        /// </summary>
        [Test]
        public void TestLessonIds_PlanetsOneToThree()
        {
            Curriculum curriculum = RealContent.Read();

            Assert.That(curriculum.AllLessons.Select(lesson => lesson.UnitId.Value).Distinct(),
                Is.SupersetOf(new[] { "planet-1", "planet-2", "planet-3" }));
            Assert.That(LessonIds().Count(), Is.EqualTo(curriculum.AllLessons.Count(lesson => !lesson.Retired)));
        }

        private static IEnumerable<string> LessonIds()
        {
            return RealContent.Read().AllLessons
                .Where(lesson => !lesson.Retired)
                .Select(lesson => lesson.Id.Value);
        }

        private static void AnswerRight(LessonRun run, string where)
        {
            IActivityRules rules = run.Current;
            ActivityRound round = rules.Round;
            switch (rules.Kind)
            {
                case ActivityKind.MeetLetter:
                    run.Answer(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));
                    run.Answer(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));
                    break;
                case ActivityKind.LetterHunt:
                    for (int star = 0; star < round.Choices.Count; star++)
                    {
                        if (round.Choices[star].Item == round.AnswerItem && round.StateOf(star) != ChoiceState.Done)
                        {
                            run.Answer(ActivityAnswer.Tap(star));
                        }
                    }

                    break;
                case ActivityKind.TwoSounds:
                    run.Answer(ActivityAnswer.Tap(TwoSoundsRules.RomanianPlanet));
                    run.Answer(ActivityAnswer.Tap(TwoSoundsRules.EnglishPlanet));
                    for (int picture = 0; picture < round.Choices.Count && round.Current >= 0; picture++)
                    {
                        run.Answer(ActivityAnswer.Drop(round.Answer));
                    }

                    break;
                case ActivityKind.ListenAndPick:
                case ActivityKind.FirstSound:
                case ActivityKind.ReadAndMatch:
                    run.Answer(ActivityAnswer.Tap(round.Answer));
                    break;
                case ActivityKind.Syllables:
                    for (int beat = 0; beat < round.Places; beat++)
                    {
                        run.Answer(ActivityAnswer.Tap(SyllablesRules.Drum));
                    }

                    run.Answer(ActivityAnswer.Tap(SyllablesRules.Tick));
                    break;
                case ActivityKind.BuildWord:
                    for (int window = 0; window < round.Places && round.Answer >= 0; window++)
                    {
                        run.Answer(ActivityAnswer.Place(round.Answer));
                    }

                    break;
                case ActivityKind.WordTwins:
                    for (int word = 0; word < round.Choices.Count / 2; word++)
                    {
                        run.Answer(ActivityAnswer.Tap(word));
                        run.Answer(ActivityAnswer.Join(word, round.Answer));
                    }

                    break;
                case ActivityKind.TraceLetter:
                case ActivityKind.TraceWord:
                    TraceEveryStroke(run, where);
                    break;
                default:
                    Assert.Fail(where + ": this test can't answer a " + rules.Kind + " step yet.");
                    break;
            }
        }

        private static void TraceEveryStroke(LessonRun run, string where)
        {
            int mostLines = TracingTuning.Rounds * ActivityLimits.MostTracedLetters * ActivityLimits.MostStrokes;
            for (int lines = 0; run.State == LessonRunState.Playing; lines++)
            {
                Assert.That(lines, Is.LessThan(mostLines), where + " never ends");
                TraceState trace = run.Current.Round.Trace;
                StrokePath path = trace.Figure.Strokes[trace.Stroke].Path;
                float pointEvery = 4f;
                run.Answer(ActivityAnswer.Traced(TestStrokes.Along(path, 0f, 1f, pointEvery, 0f)));
            }
        }
    }
}
