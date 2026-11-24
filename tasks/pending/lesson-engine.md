# Task: feature/lesson-engine

> **Status: Ready after `voice`.** D6 answered: the gold star "RO" and the
> blue ringed planet "EN" (design/README.md). The human, 2026-11-24: wrong
> choices are only letters already taught, never one still to come.

Read `AGENTS.md` first, then `design/README.md` and `design/screens.md` (build the screens as designed there) (especially "The five seams" → 1 and 3, "How the
app is put together", "Usability" and "Learning"), and the completion
records in `tasks/done/`.

**Feature:** `feature/lesson-engine`
**Implements:** PRD §3.2 (lessons, feedback, leaving); `docs/curriculum.md`
§4.1, §4.3.
**Depends on:** `voice`.

This is a **large change** (it defines the lesson engine every domain will
use): write the Design note with the options compared, and have the human
agree it before code. A fresh-eyes review before the push.

## Scope

1. **Core:**
   - `IActivityRules`: given a step's items, the learning rules asset and an
     `IRandom`, it sets up the round (which choices, in which order), takes
     the child's answers, and returns `Right`, `TryAgain(hint)`,
     `ShowAnswer`, or `Done`; it never knows how it is drawn.
   - `LessonRun`: steps through a lesson's steps (plus up to two review
     steps at the start, given by `progress-review` later; none for now),
     counts tries per step, applies the "second try with a hint, then show"
     rule, raises `AnswerChecked`, `HintGiven`, `StepCompleted`,
     `LessonCompleted`, and can be saved and resumed at its step.
   - `LearningRules`: the numbers (tries before showing, praise variety) as
     a plain class filled from a ScriptableObject on the Unity side.
   - The rules for **`MeetLetter`** and **`ListenAndPick`**.
2. **Unity side:**
   - The **Lesson scene** with `LessonRunner` (its composition root), the
     home button (top left, large), the mascot placeholder (speaks every
     instruction through `IVoice`, tap to repeat), and the step's view.
   - `IActivityView` and `ActivityRegistry` (`ActivityKind` → view prefab),
     a ScriptableObject listing the kinds and their prefabs; a missing view
     for a kind fails the content validator.
   - The views for `MeetLetter` and `ListenAndPick` (choices as planets or
     bubbles, at least 96 dp), with kind feedback: a sparkle and a praise
     line on right, a wobble and a hint on a miss, the answer glowing on
     the second miss.
   - The **language badge** component (star = Romanian, circle = English)
     on every letter and word card.
   - Leaving keeps the run (resumes at its step next time); the app going
     to the background pauses the voice and resumes it.
3. **The Map placeholder's "Go"** opens planet 1's first lesson from the
   sample content.

## Out of scope — do not build

- The other activity kinds (their briefs), tracing, the galaxy map, stars
  and stickers, progress and review (their briefs; leave events for them).

## Acceptance criteria

- On the emulator: Go → planet 1's first lesson runs `MeetLetter` (a) then
  `ListenAndPick` with a, m, t; every instruction is heard; a right tap
  sparkles and praises; a wrong tap wobbles and hints; a second wrong tap
  shows the answer; the lesson ends and returns to the Map.
- Home mid-lesson, then Go: the lesson resumes at the same step.
- Every card shows its language badge, star or circle.
- No allocation per frame while a step runs (Profiler, recorded).

## Tests required

- EditMode: `LessonRun` (step order, tries, the hint ladder, resume),
  `MeetLetter` and `ListenAndPick` rules with planet 1's real items, the
  registry's validator check.
- PlayMode: a lesson runs to its end with simulated taps; home and resume.
- Emulator: the lesson at 2000 × 1200 and 1280 × 800, listened to in both
  languages; the app sent to the background mid-line.

## Touches

`Scripts/Core/Lessons`, `Scripts/Core/Activities`, `Scripts/Game/Lessons`,
`Scripts/UI/Lesson`, the `Lesson` scene, `ActivityRegistry` (new), new
prefabs, the Map placeholder's button.

## Screens

The Lesson screen with `MeetLetter` and `ListenAndPick`.

## Voice lines

The instructions, praise and hints for the two kinds, both languages
(text in `voice-lines.csv`; clips if recorded).

## Before closing out

- Release 0.4.0: the first lesson she could try (tell the human in
  `local/for-human-next-steps.md`, with the APK).
