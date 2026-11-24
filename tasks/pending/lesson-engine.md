# Task: feature/lesson-engine

> **Status: In progress (lesson).** D6 answered: the gold star "RO" and the
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

## Progress

_Kept by `lesson` while the brief is in flight._

### Status

- **Claimed** in `78aa810` (worktree `C:\Projects\scolita-lesson`, emulator
  `emulator-5602`).
- **Design note** (below) pushed in `ee8437d`. **The human agreed it on
  2026-11-24, with every recommendation** (relayed by the lead):
  1. **A:** the validator enforces "wrong choices are only taught letters"
     for `ListenAndPick`; it starts in each track's second lesson (`ro-m`:
     a m; `ro-t`: a m t; `ro-p`: a m t p). `ro-a`'s `ListenAndPick` row is
     removed (a content change the human agreed), and the acceptance test
     runs on `ro-t`. The same problem for `LetterHunt` and `FirstSound` in
     `ro-a` and `en-a` is a follow-up for `activities-letters`.
  2. **A:** Go opens the next lesson in her languages' order, skipping
     lessons with nothing playable yet, kept in `lesson-run.json` until
     `galaxy-map` and `progress-review` replace it.
  3. **A:** the registry lists the kinds not built yet; the validator warns
     instead of failing, and their steps are skipped. Each activity brief
     takes its kind off the list.
  4. **Yes:** one question per `ListenAndPick` step; the step's first
     letter (or word) is the answer.

  The smaller points stand.

### Design note (large change: the lesson engine)

#### In plain words

1. **Core runs the lesson; Unity only draws it and says it.** A lesson is a
   list of steps. Core's `LessonRun` steps through them, counts her tries
   and decides what happens after each tap: praise, a hint, the answer
   shown, the next step. Each kind of activity has its own small rules
   class in Core, and its own view on the Unity side.
2. **The "second try with a hint, then show" rule is written once,** in
   `LessonRun`, for every kind. A kind's rules only say whether a tap is
   right, and what its hint and its shown answer are.
3. **Core decides every line the mascot says** (the instruction, the hint,
   the praise, the letter's sound), so a view never chooses words. Every
   line is spoken and shown in the mascot's bubble at the same time.
4. **Leaving keeps her place:** after each step, and when the tablet
   sleeps, the lesson in progress is saved in her profile. Go opens it again
   at the same step. If an update changed the lesson's steps, it starts
   again from its first step, never at the wrong one.
5. **Everything a lesson needs is made when it opens** (well inside the 2
   seconds), so nothing is made while she plays: no stutter on the tablet.

Four points need the human's answer ("Questions for the human", at the
end).

#### The choices compared

**1. The shape of `IActivityRules`.**

| Option | For | Against |
| --- | --- | --- |
| A. One stateless check per kind (`Check(step, answer)`), all state in `LessonRun` | Smallest | Kinds with several parts (`BuildWord`'s letters, `LetterHunt`'s stars, `TraceLetter`'s rounds and strokes) push their own state into `LessonRun`, which grows with every kind |
| B. A stateful rules object per step that also runs the tries and the hints | Each kind is self-contained | The "hint, then show" rule is written eleven times and drifts |
| **C. (picked)** A stateful rules object per step for the kind's own state (its round, its parts, its choices); the shared ladder in `LessonRun` | Each rule has one owner (AGENTS.md: "one owner per rule"); a new kind writes only what is new about it | Two classes take part in each answer (the rules judge, the run decides) |

The interface, in Core (`Scolita.Core.Activities`):

```csharp
public interface IActivityRules
{
    ActivityKind Kind { get; }
    ActivityRound Round { get; }                    // what the view draws: the choices in order, the current part
    void Begin(IRandom random);                     // sets up the round (which choices, in which order)
    AnswerVerdict Judge(in ActivityAnswer answer);  // Right (a part done), Done (the step done), Wrong or Neutral
    ActivityHint HintFor(int missNumber);           // what the n-th miss of the current part shows and says
    ActivityHint AnswerShown();                     // what showing the answer means for this kind (the glow, its line)
}
```

- A kind's rules are made from the step's items and the learning rules by
  `ActivityRulesFactory.Create(step, curriculum, rules)` (one line per
  kind; a kind with no rules yet can't be played).
- `ActivityAnswer` is a small value (`readonly struct`): which choice or
  which part (the letter, the picture) was tapped; `tracing` adds a
  stroke's result.
- `AnswerVerdict.Neutral` is a tap that is not an answer (hearing the
  letter again): never a miss.

**2. `LessonRun`: the state machine and its events** (`Scolita.Core.Lessons`).

- **States:** `Playing` (a step's rules wait for an answer) ⇄
  `AnswerShown` (the part's answer glows; her next right tap counts as
  practice, not as right) → `StepDone` (the praise is being said; Unity
  calls `Next()` once it has been heard) → `Playing` the next step … →
  `Finished`.
- **What a tap gives back** (`StepFeedback`, a value): `Right`, `TryAgain`
  with its hint, `ShowAnswer`, `Done` or `Neutral`, with the voice line to
  say and which choice to sparkle, wobble, fade or glow. The ladder: the
  first miss of a part gets the kind's hint; the miss that reaches
  `LearningRules.TriesBeforeAnswerShown` (2) shows the answer; a miss after
  that glows it again, nothing more; a part done with the answer shown is
  practice.
- **Events** (each argument a `readonly struct`, so raising one allocates
  nothing): `AnswerChecked` (step, part, right / wrong / practised, tries),
  `HintGiven`, `StepCompleted`, `LessonCompleted`. `progress-review` and
  `rewards` listen to these later for `ItemMastered` and `RewardEarned`
  (not built here).
- **The steps it plays** come from `LessonPlan.Of(curriculum, lesson, mix,
  playableKinds, reviewSteps, resumeAt)`: the lesson's steps in order,
  without those in a language she doesn't learn (her
  `learning-settings.json`) or of a kind not built yet (question 3), after
  up to two review steps (none until `progress-review`).
- **Praise is varied:** one of the lesson language's `praise-1` …
  `praise-n` lines (found in the content, so a `praise-11` needs no code),
  never one of the last few said (`LearningRules.PraiseNotRepeatedWithin`,
  3).
- **`LearningRules`** is a plain, immutable Core class (the tries before
  the answer is shown, the praise variety), filled from
  `Assets/Scolita/Settings/LearningRules.asset` on the Unity side;
  `progress-review` adds §5's numbers to the same asset.

**3. Resuming.**

| Option | For | Against |
| --- | --- | --- |
| A. Save the run's whole state (every answer) | Resumes mid-tap | A large, fragile save shape for nothing she would notice |
| **B. (picked)** Save the lesson id and the next step's number, with a signature of the lesson's steps | Small and versioned; a changed lesson is caught | A half-done step starts again from its start (steps are short) |

- `lesson-run.json` in `profiles/<id>/` (seam 5), through `SaveStore`, with
  its `version` and `Upgraded()`: `lessonId`, `stepNumber` (the lesson's
  own step number, from 1; review steps are never resumed) and
  `stepsSignature` (made by Core from each step's kind, language, items and
  options).
- Saved when a step is completed and when the app goes to the background;
  cleared when the lesson is finished.
- On Go: an unknown or retired lesson, a different signature (the content
  changed: the content-model follow-up) or a step number past the end →
  the lesson starts at its first step, with a warning in the log. A file
  that can't be read is set aside, as `SaveStore` does.

**4. `ActivityRegistry`.**

| Option | For | Against |
| --- | --- | --- |
| **A. (picked)** A ScriptableObject listing each kind and its view prefab, referenced by `LessonRunner` | Data, checked by a test; a new kind is a row | Needs a validator test (wanted anyway) |
| B. Find views by reflection or attributes | No list to keep | A service locator by another name (AGENTS.md forbids them); unchecked |
| C. A `switch` in `LessonRunner` | Simple | Every new kind edits the composition root |

- `Prefabs/Lesson/ActivityRegistry.asset`: rows of `ActivityKind` → a
  prefab whose root has an `ActivityView` (an abstract MonoBehaviour
  implementing `IActivityView`, because Unity can't serialize an
  interface).
- **The validator** (an EditMode test): every kind used by a lesson in use
  has a view in the registry and rules in `ActivityRulesFactory`, or is on
  the registry's "not built yet" list (question 3), which gives a warning.

**5. How a view gets its items and the voice.**

- `IActivityView.Show(ActivityRound round, ActivityViewContext context)`:
  the round holds the choices as Core values (a letter's text, or a
  picture's id, with their language); the context gives the sprites (from
  the picture catalog) and the place to send taps
  (`IActivityInput.Answer(ActivityAnswer)`), which `LessonRunner` passes to
  `LessonRun`.
- `IActivityView.ShowFeedback(in StepFeedback)`: a sparkle on right; on a
  miss, a gentle wobble and the wrong choice fading; the yellow glow on the
  answer shown. The view never speaks: `LessonRunner` says the feedback's
  line through `IVoice` (seam 4). The next step's instruction waits for the
  praise to end, and only a newer line of the same step replaces a line
  being said.
- **The mascot** (the unicorn guide's frames from `look`, standing in for
  the final guide): it talks while a line is said, thinks on a hint and
  cheers at the end. Its bubble shows each line's words as they are said
  (following `VoiceRouter.PartStarted`, which `VoiceHost` passes on), with
  the language badge. Tapping it says the last line again.
- **The language badge** (`LanguageBadge`, UI): the gold star "RO" or the
  blue ringed planet "EN" (`look`'s badges, D6), top right on every letter,
  word and picture card, and beside the bubble.
- **The lesson screen** (`Prefabs/Lesson/LessonScreen.prefab`, new, so
  `look`'s `LessonHud.prefab` and `MapScreen.prefab` stay untouched): the
  home button (the existing `LessonHud`), the step dots at the top centre
  (green done, yellow current, white to come), the mascot, and the step's
  view. The end of the lesson, until `rewards` builds screen 6: the mascot
  cheering, "Ai terminat lecția. Bravo!" / "You finished the lesson. Well
  done!" said, and a big yellow **Mai departe** (with an arrow) back to the
  map.

**6. The two kinds.**

- **`MeetLetter`** (design screen 2, without the name button in planets
  1–3 and without "de mână" until `handwriting`): the letter card (small
  and capital, in Andika, with its badge) and the picture card with its
  word (its first letter in pink). Said: "Hai să cunoaștem o literă nouă!",
  "Ascultă: a… a… avion!", "Atinge litera ca s-o auzi din nou." Her tap on
  the letter says its sound, then "Acum atinge poza."; her tap on the
  picture says the word, then praise, and the step is done. A tap on the
  picture first just says the word (never a miss); the step waits for
  both. No wrong answers, so no hints.
- **`ListenAndPick`:** the step's first letter (or word) is the one she
  hears; all its letters (or pictures) are the choices, shuffled by
  `IRandom`, as round cream bubbles (at least 96 dp; 180 px at the
  1280 × 800 reference) with their badges, and a "listen again" button.
  Said: "Ascultă și atinge litera potrivită. Atinge litera /a/." First
  miss: the tapped bubble wobbles and fades, "Ascultă încă o dată, mai
  încet." and the sound again. Second miss: the answer glows, "Uite,
  aceasta este litera /a/. Atinge-o!" Right: a sparkle and a praise line.
  With pictures it is the same, with the word ("Ascultă și atinge poza
  potrivită.").
- **Each kind's hint lines go in Core's `ActivityKinds`,** beside its
  instruction lines, so the validator checks that they exist in both
  languages (the content-model follow-up). Every line above is already in
  `voice-lines.csv`; any new one is added there in both languages and
  listed for the human.

**7. Another domain.** `LessonRun`, the ladder, the events, the save and
the registry know nothing about letters: they run steps through
`IActivityRules`. `ListenAndPick`'s rules work on choices (an id, the line
it is heard by, and how it is drawn: a letter or a picture), made from the
step's items. So "tap the drum" (music) or "tap the 3" (numbers) is the
same kind with other choices, once the content model gives steps those
items. A new domain adds a kind only when none fits: one rules class, one
view prefab, one registry row, one factory line.

**8. Allocations.** At lesson load (inside the 2-second budget): the plan,
every step's rules, every voice sequence (instructions, hints, shown
answers, praise), and one view per kind used, reused from step to step,
with its choice bubbles pooled (at most 4). While a step runs: a tap calls
Core with a value; Core answers with a value and raises events with value
arguments; the views animate with plain arithmetic in `Update`. No LINQ, no
`new`, no strings, no capturing lambdas. Measured with the Profiler (0 B
per frame while a step runs). Whether `VoiceRouter` allocates when a line
starts (on a tap, not every frame) is measured too; if it does, the brief
says so and it becomes a follow-up for the voice.

**9. Where it lives.**

- `Scripts/Core/Activities/`: `IActivityRules`, `ActivityRound`,
  `ActivityAnswer`, `AnswerVerdict`, `ActivityHint`, `ActivityChoice`,
  `ActivityRulesFactory`, `MeetLetterRules`, `ListenAndPickRules`.
- `Scripts/Core/Lessons/`: `LessonRun`, `LessonPlan`, `StepFeedback`, the
  event arguments, `LearningRules`, `PraisePicker`, `StepsSignature`,
  `LessonOrder` (question 2).
- `Scripts/Core/Content/`: `ActivityKinds` gains the hint lines (and the
  "choices are taught letters" mark, question 1); `StepRules` checks them.
- `Scripts/Game/Lessons/`: `LessonRunner` (the composition root; the voice
  sampler goes), `ActivityRegistry`, `ActivityView`, `LearningRulesAsset`,
  `LessonRunData` (the save), `LessonVoice` (says the feedback's lines in
  order).
- `Scripts/UI/Lesson/`: the two views, `StepDots`, `Mascot`,
  `LanguageBadge`, the end panel.
- `VoiceHost` gains `PartStarted` (for the bubble). `MapScreen` is
  unchanged: Go opens the Lesson scene, which picks the lesson.

**10. Tests.**

- **EditMode:** `LessonRun` (the step order; skipping a language and a kind
  not built; the tries; the hint ladder; practice after the answer is
  shown; `Next`; the events; resuming; a changed signature);
  `MeetLetterRules` and `ListenAndPickRules` with planet 1's real items from
  the catalogs; `PraisePicker`; `LessonPlan`; `LessonOrder`; the registry's
  validator test; `LessonRunData`'s upgrade and clamping.
- **PlayMode:** a lesson runs to its end with simulated taps (right, one
  miss, two misses); home mid-lesson, then Go, resumes at the same step.
- **Emulator** (`emulator-5602`): the lesson at 2000 × 1200 and 1280 × 800
  in both languages; the app sent to the background mid-line.
- **Profiler:** 0 B per frame while a step runs.

#### Questions for the human

1. **The first lesson and "wrong choices are only taught letters".** In
   `ro-a` only `a` is taught, so its `ListenAndPick` (a, m, t) breaks your
   rule, and so does the brief's acceptance test ("`ListenAndPick` with a,
   m, t" in the first lesson).
   - **A (recommended):** the validator enforces your rule for letter
     choices (every letter of a `ListenAndPick` step is taught by its
     lesson, in its language), so `ListenAndPick` starts in each track's
     second lesson: `ro-m` (a, m), `ro-t` (a, m, t), `ro-p` (a, m, t, p).
     `ro-a`'s `ListenAndPick` row goes, and the acceptance test runs on
     `ro-t`. (`LetterHunt` and `FirstSound` in `ro-a` and `en-a` have the
     same problem; `activities-letters` applies the same check to them.)
   - **B:** `ro-a` keeps a `ListenAndPick` with a single choice (`a`): she
     can't get it wrong; it practises "hear it, tap it".
   - **C:** `ro-a`'s `ListenAndPick` uses pictures instead (she hears
     "avion" and taps the aeroplane among two or three pictures):
     listening, no letters.
2. **What Go opens after a lesson is finished.**
   - **A (recommended):** the next lesson in her languages' order (`ro-a`,
     `en-a`, `ro-m`, …), skipping lessons with nothing to play yet, and
     after the last one the first again; kept in the same
     `lesson-run.json`. `galaxy-map` and `progress-review` replace it with
     the map's stops and her real progress. This lets you (and her) try
     `ListenAndPick` on the tablet.
   - **B:** always planet 1's first lesson (the brief as written). Then
     `ListenAndPick` with real choices can only be tried in agents' tests
     until the map comes.
3. **Steps whose kind isn't built yet.** Planet 1 uses nine kinds whose
   views come in later briefs (`LetterHunt`, `TraceLetter`, `FirstSound`,
   …). The brief says a kind without a view fails the validator, which
   would fail today's content.
   - **A (recommended):** the registry lists those kinds as "not built
     yet": the validator warns instead of failing, and their steps are
     skipped. So for now `ro-a` is `MeetLetter` alone and `ro-t` is
     `MeetLetter` then `ListenAndPick`. Each activity brief takes its kind
     off the list; once the list is empty, a kind without a view is an
     error again.
   - **B:** remove the unbuilt steps from `steps.csv` until their briefs
     come (planet 1's sample loses most of its steps, and each brief puts
     its own back).
4. **One question per `ListenAndPick` step.** A step asks for one letter
   (its first), and a lesson that wants more questions has more steps.
   Agreed, or should one step ask several times (each of its letters in
   turn)?

Smaller points, decided unless the human says otherwise: the end panel's
"Mai departe" is written in its prefab, as "Hai!" and "Acasă" are, until
the app's words move to the string tables (a `project-skeleton`
follow-up). `ListenAndPick` isn't designed on the canvas yet: it is built
from `look`'s parts by the rules of the look, and `look` may refine it.
