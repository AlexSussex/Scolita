using System;
using System.Collections;
using System.Globalization;
using NUnit.Framework;
using Scolita.Core.Parent;
using Scolita.Core.Randomness;
using Scolita.Game.Map;
using Scolita.Game.Scenes;
using Scolita.Tests.PlayMode.Map;
using Scolita.UI.Parent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Parent
{
    /// <summary>
    /// Tests the parent gate on the map (<see cref="Game.Parent.ParentGateFlow"/>, <see cref="ParentGatePanel"/>,
    /// <see cref="ParentHoldButton"/>): the hold of three seconds, letting go early, the right number, a wrong one, the
    /// close button, the digits when no voice can say the number, and a minute of random taps that never open it.
    /// Each test keeps the Editor's own files as they were.
    /// </summary>
    public sealed class ParentGateTests
    {
        /// <summary>Tests that holding the parent button asks only after three seconds of real time (the rule is about real time).</summary>
        [UnityTest]
        public IEnumerator TestParentButton_AsksAfterThreeSeconds()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                yield return MapScene.Open();
                ParentGatePanel panel = ParentScene.GatePanel();
                float start = Time.realtimeSinceStartup;

                panel.HoldButton.OnPointerDown(new PointerEventData(EventSystem.current));
                yield return MapScene.WaitFor(() => panel.IsAsking, 6f);
                float held = Time.realtimeSinceStartup - start;
                panel.HoldButton.OnPointerUp(new PointerEventData(EventSystem.current));

                Assert.That(held, Is.GreaterThanOrEqualTo(2.9f));
                Assert.That(panel.HoldButton.RingFill, Is.EqualTo(1f));
                Assert.That(MapScene.Runner().Gate.AskedNumber, Is.InRange(11, 19));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that letting go of the parent button before three seconds empties the ring and asks nothing.</summary>
        [UnityTest]
        public IEnumerator TestParentButton_LetGoEarlyEmptiesTheRing()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                yield return MapScene.Open();
                ParentGatePanel panel = ParentScene.GatePanel();
                float until = Time.realtimeSinceStartup + 1.5f;

                panel.HoldButton.OnPointerDown(new PointerEventData(EventSystem.current));
                yield return MapScene.WaitFor(() => Time.realtimeSinceStartup >= until, 3f);
                float halfway = panel.HoldButton.RingFill;
                panel.HoldButton.OnPointerUp(new PointerEventData(EventSystem.current));
                yield return null;

                Assert.That(halfway, Is.InRange(0.3f, 0.95f));
                Assert.That(panel.HoldButton.RingFill, Is.EqualTo(0f));
                Assert.That(panel.IsAsking, Is.False);
                Assert.That(MapScene.Runner().Gate.Stage, Is.EqualTo(GateStage.Shut));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that the number said opens the parent area, which wakes the rocket (today's play starts again).</summary>
        [UnityTest]
        public IEnumerator TestGate_RightNumberOpensTheParentArea()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                yield return ParentScene.LeaveTheOpenScene();
                MapScene.SaveProgress(null);
                ParentScene.SavePlayedToday(TimeSpan.FromMinutes(7));
                yield return MapScene.Open();
                ParentScene.AskAtOnce();

                ParentScene.GatePanel().ChoiceButtons[ParentScene.RightPlace()].onClick.Invoke();
                yield return ParentScene.WaitForArea();

                Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Parent.ToString()));
                Assert.That(ParentScene.SavedPlayToday(), Is.EqualTo(TimeSpan.Zero));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>
        /// Tests that one long frame (a hitch, or the return from the background with the finger still counted as down)
        /// fills a tenth of a second of the hold at most, never the whole of it.
        /// </summary>
        [UnityTest]
        public IEnumerator TestGate_OneLongFrameNeverFillsTheHold()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                yield return MapScene.Open();
                Game.Parent.ParentGateFlow gate = MapScene.Runner().Gate;

                gate.Press();
                gate.Tick(10f);

                Assert.That(gate.IsAsking, Is.False);
                Assert.That(ParentScene.GatePanel().HoldButton.RingFill, Is.LessThan(0.1f));
                gate.Release();

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that a wrong number closes the gate quietly: the map stays and the gate must be held again.</summary>
        [UnityTest]
        public IEnumerator TestGate_WrongNumberClosesQuietly()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                yield return MapScene.Open();
                ParentScene.AskAtOnce();
                int wrong = (ParentScene.RightPlace() + 1) % ParentScene.GatePanel().ChoiceButtons.Count;

                ParentScene.GatePanel().ChoiceButtons[wrong].onClick.Invoke();
                for (int frame = 0; frame < 10; frame++)
                {
                    yield return null;
                }

                MapRunner runner = MapScene.Runner();
                Assert.That(runner, Is.Not.Null, "still on the map");
                Assert.That(ParentScene.GatePanel().IsAsking, Is.False);
                Assert.That(runner.Gate.Stage, Is.EqualTo(GateStage.Shut));
                Assert.That(ParentScene.GatePanel().HoldButton.RingFill, Is.EqualTo(0f));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that the close button shuts the gate without an answer.</summary>
        [UnityTest]
        public IEnumerator TestGate_CloseButtonShutsIt()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                yield return MapScene.Open();
                ParentScene.AskAtOnce();

                ParentScene.GatePanel().CloseButton.onClick.Invoke();
                yield return null;

                Assert.That(ParentScene.GatePanel().IsAsking, Is.False);
                Assert.That(MapScene.Runner().Gate.Stage, Is.EqualTo(GateStage.Shut));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>
        /// Tests that when no voice can say the number (the Editor has no clip and no tablet voice, so the line goes to
        /// the bubble), the panel shows it in digits, so a parent is never locked out.
        /// </summary>
        [UnityTest]
        public IEnumerator TestGate_NoVoiceShowsTheDigits()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                MapScene.SaveProgress(null);
                yield return MapScene.Open();
                ParentScene.AskAtOnce();
                string digits = MapScene.Runner().Gate.AskedNumber.ToString(CultureInfo.InvariantCulture);

                yield return MapScene.WaitFor(() => ParentScene.GatePanel().Question.Contains(digits), 3f);

                Assert.That(ParentScene.GatePanel().Question, Does.Contain(digits));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>
        /// Tests the acceptance criterion: a minute of random taps anywhere on the map (each held 1 to 40 frames, the
        /// parent button among the places hit) never opens the gate. The rocket rests (a daily limit reached) so no tap
        /// opens a lesson and the map stays under the taps.
        /// </summary>
        [UnityTest]
        public IEnumerator TestGate_AMinuteOfRandomTapsNeverOpensIt()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                yield return ParentScene.LeaveTheOpenScene();
                MapScene.SaveProgress(null);
                ParentScene.SaveLimit(10);
                ParentScene.SavePlayedToday(TimeSpan.FromMinutes(10));
                yield return MapScene.Open();
                SeededRandom random = new SeededRandom(2026);
                ParentGatePanel panel = ParentScene.GatePanel();
                RectTransform button = (RectTransform)panel.HoldButton.transform;
                float until = Time.realtimeSinceStartup + 60f;
                int taps = 0;
                bool everAsked = false;

                while (Time.realtimeSinceStartup < until)
                {
                    Vector2 position = taps % 4 == 0
                        ? RectTransformUtility.WorldToScreenPoint(null, button.position)
                        : new Vector2(random.NextInt(0, Screen.width), random.NextInt(0, Screen.height));
                    yield return ParentScene.Tap(position, random.NextInt(1, 41));
                    taps++;
                    everAsked |= panel.IsAsking;
                    Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Map.ToString()));
                }

                Assert.That(taps, Is.GreaterThan(100));
                Assert.That(everAsked, Is.False);
                Assert.That(MapScene.Runner().Gate.Stage, Is.Not.EqualTo(GateStage.Open));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }
    }
}
