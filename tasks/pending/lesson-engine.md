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
- **Built, reviewed by a second agent, tested and pushed** in six slices (below),
  then **closed out as release 0.4.0** (see "Completion record").

### What was built (within the agreed note)

- **Pushed** (rebased on `859f624`, release 0.3.1):
  - `content:` `ro-a` loses its `ListenAndPick` step; `ro-m` picks between m and a
    (the human's answer 1 A), with the two imported lesson assets;
  - `core:` the lesson run, the hint ladder, the plan, the order, the resume, the praise,
    the signature, the two kinds' rules, the validator's new rules and their tests;
  - `game:` the runner, its voice, the registry, the save, the learning rules asset type;
  - `ui:` the lesson screen, the two views, the cards, the dots, the mascot, the badge;
  - `game:` the Lesson scene, the prefabs, `ActivityRegistry.asset`,
    `LearningRules.asset`, the PlayMode tests; the voice sampler removed.

  Their hashes are in the completion record.
- **Core** (`Scripts/Core`, no engine reference):
  - `Activities/`: `IActivityRules`, `ActivityRound`, `ActivityChoice` (+ `Look`),
    `ActivityAnswer`, `AnswerVerdict`, `ActivityHint`, `HintCue`,
    `ActivityRulesFactory`, `MeetLetterRules`, `ListenAndPickRules`.
  - `Lessons/`: `LessonRun` (+ `LessonRunState`, `StepFeedback`, `FeedbackOutcome`,
    `AnswerResult`, the events `AnswerChecked`, `HintGiven`, `StepCompleted`,
    `LessonCompleted`), `LessonPlan` (+ `PlannedStep`), `LessonOrder`, `LessonResume`
    (+ `ResumeReason`), `LessonSignature`, `PraisePicker`, `LearningRules`.
  - `Content/`: `LessonLines` (praise and the end line); `ActivityKinds` gains each
    kind's hint lines and "its letters are choices"; `StepRules` checks taught choices,
    each choice once, hint lines and the lesson lines. `MeetLetter` now takes exactly
    one word (its line says it).
  - `Voice/`: `VoiceSequenceText` (the bubble's words of a sequence);
    `LessonVoiceLines` also loads the hint, praise and end lines.
- **Game** (`Scripts/Game/Lessons`): `LessonRunner` (the composition root),
  `LessonVoice`, `ActivityRegistry`, `ActivityView` / `IActivityView`,
  `IActivityInput`, `ActivityViewContext`, `PictureSprites`, `LessonScreenView`,
  `ILessonScreenInput`, `GuideMood`, `LearningRulesAsset`, `LessonRunData`.
  `VoiceHost` passes on `PartStarted`; `SpeechBubbleView.Hide` is safe once its
  screen is gone.
- **UI** (`Scripts/UI/Lesson`): `LessonScreen`, `MeetLetterView`,
  `ListenAndPickView`, `ChoiceTile`, `StepDots`, `GuideFigure`, `LanguageBadge`.
- **Assets:** `Prefabs/Lesson/LessonScreen.prefab`, `MeetLetterView.prefab`,
  `ListenAndPickView.prefab`, `ActivityRegistry.asset`; `Settings/LearningRules.asset`
  (2 tries, praise not repeated within 3); `Scenes/Lesson.unity` (the screen added,
  `LessonHud`'s placeholder card and letters hidden, the voice's bubble is the
  mascot's); `Prefabs/Voice/VoiceSampler.prefab` deleted (replaced, as `voice` asked).
  The prefabs were made by a throwaway Editor script (not committed), from `look`'s
  art: the cream card and tile, the bubble, the buttons, the icons, the badges, the
  unicorn's frames and the star particle.

### Decisions while building

- **One way for every line said:** the bubble is the voice's own bubble (the
  `VoiceHost`'s `SpeechBubbleView` is the mascot's), and `LessonVoice` shows each
  whole line's words in it while the line is said (`VoiceSequenceText`, made at
  load), also when a voice is installed. A letter's sound shows as the letter held
  ("a…").
- **No new voice line:** every line used was already in `voice-lines.csv` (the
  agreed draft). The first hint of `ListenAndPick` is "Ascultă încă o dată." + the
  sound, not "Ascultă încă o dată, mai încet." (`pick-hint-slow`): the sound clip
  plays at its own speed, so "mai încet" would not be true. Then "Aproape! Încearcă
  altă literă.", then "Uită-te bine la forma literei." (only with 3 or more tries).
  With pictures: "Ascultă încă o dată." + the word, and the answer shown with
  "Uite răspunsul. Acum fă-o și tu." + the word (no step uses pictures yet).
- **Praise only when a step is done:** the letter tap in `MeetLetter` says its
  sound and "Acum atinge poza.", not a praise, so praise isn't said twice per step.
- **The kinds not built yet** are logged once as information when a lesson opens
  (not as a warning: the Console stays clean during checks); the validator test
  prints them.
- **The step dots count the lesson's playable steps;** on resume the steps before
  are shown done (green).
- **The end of a lesson** (until `rewards`): the mascot cheers and says "Ai terminat
  lecția. Bravo!", the dots all green, and a big yellow "Mai departe" opens the map.
- **A missed answer after the answer is shown** is reported as
  `AnswerResult.WrongWithAnswerShown`, so `progress-review` doesn't count it as a
  real miss.

### Usability (the new screens)

- **Spoken:** every instruction, hint, praise and the end are said, the bubble shows
  the same words at the same time, tapping the mascot says the last line again, and
  "Ascultă" (listen again) says the question again.
- **Touch targets** at the 1280 × 800 reference: letter cards 200 × 200 (about 200 dp
  on the Tab A7), the `MeetLetter` cards 470 × 470 and 420 × 380, "Ascultă" 320 × 96,
  "Mai departe" 380 × 96, the mascot 160 × 160, the home button 80 × 80 (`LessonHud`).
- **Kind feedback:** a wobble and a fade on a miss, a yellow glow on the answer shown,
  a sparkle and praise when right; no red, no buzzer, no score.
- **The language in two ways:** every card and the bubble carry the star "RO" or the
  ringed planet "EN"; letters in Andika (at least 140 px), the app's words in Baloo 2.
- **A way out:** the home button top left on every step; leaving keeps the step.

### Fresh-eyes review (2026-11-24)

A second agent that didn't write the code read the whole diff against AGENTS.md. It
found the seams, the hint ladder, the state machine, the events, the plan, the order,
the signature, the save, the per-frame code (no allocation), the validator rules and
the touch targets sound. Its 14 findings, and what was done:

1. **The branch was behind `main`** (0.3.1 had landed): rebased on `859f624`, the
   import rerun (no change), every test and the build rerun.
2. **The resume decision was in the MonoBehaviour, untested:** moved to Core's
   `LessonResume` (+ `ResumeReason`), with `LessonResumeTests` (nothing saved, same
   signature, changed signature, past the end, unknown, nothing built, other language,
   nothing to play); the runner logs a changed lesson or a step past the end.
3. **A choice listed twice would judge wrong:** the validator refuses it
   (`TestCheck_ChoicesOnce`), and `ListenAndPick` judges by item.
4. **`LessonVoice` had no test:** it now takes Core's `ISpeechBubble`;
   `LessonVoiceTests` (first then praise, a newer line replaces, tap again keeps the
   praise, pause and resume).
5. **A mascot tap dropped the praise waiting:** fixed (the line starts again, the
   praise still follows).
6. **The voice allocates when a line starts** (`VoicePlanner`): on a tap, not per
   frame; measured (below) and a follow-up for the voice.
7. **A missing picture showed a blank card and warned every time:** a placeholder
   sprite (the purple planet) and one warning per picture.
8. **Literal diacritics in a test:** escaped.
9. **Doc comments still named the voice sampler:** updated.
10. **Misses after the answer is shown:** `WrongWithAnswerShown`.
11. **`?.` on a Unity object:** replaced.
12. **The PlayMode tests depended on the Editor's learning settings:** they keep and
    write both files.
13. **`ListenAndPick` requires both its letter and its picture lines:** kept (all
    exist in both languages); a follow-up for when picture steps come.
14. **`KindsWithViews` allocated on each read:** now a method.

### Test results (2026-11-24, rebased on `859f624`, at the commits pushed)

- **Batch mode:** EditMode **360/360**, PlayMode **7/7**, `BuildAndroidDebug` exit 0
  (`Scolita-0.3.1-debug.apk` before the close-out, then 0.4.0 at the close-out); no
  `warning CS` or `error CS` in any log; the import after the rebase changed nothing
  ("imported, with 17 warnings").
- **Emulator** (`emulator-5602`, `Scolita_Tablet_lesson`: Pixel Tablet, Android 16,
  x86_64; started with `-e unity -force-gles`; no installed TTS voice, so every line went
  to the bubble, as on a tablet without voices):
  - **2000 × 1200** (density 240): Go → `ro-a` (`MeetLetter`, star badges, one dot, the
    bubble with the whole instruction); the picture first ("avion. Atinge litera …", no
    miss), then the letter; the end ("Mai departe") → map → Go → `en-a` (ringed-planet
    badges, English bubble). `ro-t` (pushed in `lesson-run.json`): `MeetLetter`, then
    `ListenAndPick` with t, a, m shuffled; a miss faded the card and the mascot thought
    ("Ascultă încă o dată. t…"); the second miss made t glow; **Ascultă** said "Atinge
    litera t." again; t finished the lesson and `lesson-run.json` named `en-t`.
  - **Home and resume:** home on the `ListenAndPick` step, then Go: `ro-t` at step 2,
    reshuffled, its first dot green.
  - **The background mid-line:** Ascultă, then the Home key 0.5 s later: the line stopped
    (`onPause`); back in the app the same line ("Atinge litera t.") started again.
  - **1280 × 800** (density 160): `MeetLetter` (`ro-t`: t T, tobă) and `ListenAndPick`
    with a hint: the same layout, nothing cut off, the bubble's three lines fit.
  - **Logcat:** the information line about the kinds not built yet; otherwise only the
    development build's socket errors and the voice's "no installed voice" warnings.
  - Screenshots in the scratch folder (`emu-meet-2000.png`, `emu-meet-en.png`,
    `emu-pick-2000.png`, `emu-pick-miss1.png`, `emu-listen-again.png`, `emu-end-2000.png`,
    `emu-resumed2.png`, `emu-meet-1280.png`, `emu-pick-1280.png`).
- **Screen sizes:** the two the brief asks (2000 × 1200 and 1280 × 800). The large, 4:3
  and phone sizes and a cut-out were not checked: the layout scales from 1280 × 800 like
  every screen so far, and the activity area keeps clear of the corners; a follow-up for
  `look`'s final screens.
- **Allocations** (Editor Play mode, `ProfilerRecorder` "GC Allocated In Frame", a
  temporary probe not committed): on a `MeetLetter` step at rest **370 B per frame**,
  against **330 B per frame on the map** (the test harness's own). Turning off the
  mascot's animation or every raycaster leaves 370 B, so the 40 B are not in the lesson's
  `Update` code (which only does arithmetic); they are not attributed (most likely
  Editor-only). A tap costs 15 KB in its frame and 3–5 KB the next (the voice planning
  the line and the bubble's text): per tap, not per frame. **Not done:** the Profiler on
  a device (batch mode has no Profiler connection): a follow-up for the human's tablet
  or the next agent with an Editor window.
- **Not run:** the Editor's Device Simulator (batch mode only; the emulator's `wm size`
  stood in), listening by ear (no voice installed on the emulator; the human hears the
  lines with rows 0.4.0-1 to -9), the Frame Debugger.

### Voice lines (no new line; all from the agreed draft)

Said by the two kinds and the lesson, in both languages: `meet-new`, `meet-listen`
(spliced with the letter's sound and the word: a + avion / apple, m + muzică / moon,
t + tobă / tiger, p + planetă / planet), `meet-tap-letter`, `meet-tap-picture`,
`pick-letter`, `pick-letter-x` (with a, m, t, p), `listen-again` (+ the sound),
`pick-hint-other`, `pick-hint-shape`, `pick-hint-show` (with a, m, t, p), `praise-1` …
`praise-10`, `lesson-done`; for picture questions (not used yet) `pick-picture` and
`show-answer`. Checked against "Romanian, checked": whole sentences, no agreement with the
child, "s-o", "Atinge-o", "Uită-te" hyphenated, ș ț with the comma below. **For the
human:** "Uite răspunsul. Acum fă-o și tu." (its "o" stands for the task, not
"răspunsul"; not said by any step yet).

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

## How to test locally

Commands are for Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb"`, `EMU=emulator-5602` (your own
emulator), `S=<your scratch folder>` and
`export MSYS_NO_PATHCONV=1 ADB_LOCAL_TRANSPORT_MAX_PORT=5699`. Close your Editor on this
project before any batch-mode run. Start the app on the emulator with
`"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`.
Reset its saves with `"$ADB" -s $EMU shell pm clear com.softshack.scolita`.

### Core slice

1. **EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: every test passes, no `warning CS` or `error CS` in the log. Among them:
   - `LessonRunTests` (the real `ro-t`): the steps in order with praise after each and
     the end line; a miss gets "Ascultă încă o dată." and the sound, the second miss
     glows the answer ("Uite, aceasta este litera /t/. Atinge-o!"), a miss after that
     glows it again, and the right tap then counts as practice; one try in the rules
     shows the answer at the first miss; misses start again each step; no answer
     while no step waits; the resume point (1, then 2, then none; a review step
     resumes at 1);
   - `MeetLetterRulesTests` (`ro-a`: a and avion), `ListenAndPickRulesTests` (`ro-t`:
     t among a and m; the pictures of mama and tata), `ActivityRulesFactoryTests`
     (every step of a built kind in the content);
   - `LessonPlanTests`, `LessonOrderTests` (planet 1 in both languages: `ro-a`,
     `en-a`, … `en-p`, `party-1`; Romanian only; after the last, the first),
     `PraisePickerTests` (never one of the last three), `LessonSignatureTests`,
     `LearningRulesTests`, `VoiceSequenceTextTests`;
   - `ContentRulesTests`: a `ListenAndPick` with a letter not taught yet fails
     ("… some are not taught yet in the ro track by the lesson ro-a: m (taught: a);
     a wrong choice is never a letter still to come."), a missing hint line fails, a
     language without `lesson-done` or `praise-1` fails.
2. **A choice not taught yet fails the import:** add the row
   `ro-a,ListenAndPick,,a m t,,,` at the end of `content/steps.csv` and run
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`.
   Expected: exit 1 and the message above (with `m t`) in the log. Undo with
   `git checkout content/steps.csv`.

### Content slice

1. **Planet 1's picking steps** (the human, 2026-11-24): in `content/steps.csv`,
   `ro-a` has no `ListenAndPick`, `ro-m`'s is `m a`, `ro-t`'s `t a m`, `ro-p`'s
   `p a m t`. The import (Core slice, step 2, without the added row) exits 0,
   "imported, with 17 warnings", and `git status` shows nothing changed.

### Game and UI slices

1. **PlayMode tests:**
   `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: 7/7, among them `LessonFlowTests`: the real `ro-t` runs to its end with
   simulated taps (the letter, the picture, a miss on a that fades it, a miss on m that
   makes t glow, then t), the end shows "Mai departe", which opens the map, and the
   lesson file then names `en-t`; home on the `ListenAndPick` step, then Go, opens
   `ro-t` at that step with its first dot green. They keep the Editor's own `child-1`
   files as they were.
2. **The debug build:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`,
   then `"$ADB" -s $EMU install -r game/Builds/Android/Scolita-<version>-debug.apk`.
3. **The first lesson, on the emulator at the Tab A7's size:**
   `"$ADB" -s $EMU shell wm size 2000x1200; "$ADB" -s $EMU shell wm density 240`, reset,
   start the app (see above), tap **Hai!**. Expected:
   - `ro-a`: the letter card (a and A, a gold star "RO") and the picture card (a
     placeholder circle, "avion" with its a in pink, the star); one yellow dot at the
     top; the mascot top right; the bubble with its star says "Hai să cunoaștem o literă
     nouă! Ascultă: a… a… avion! Atinge litera ca s-o auzi din nou." (said by the
     tablet's Romanian voice if installed; on an emulator without it the bubble alone
     shows each part in turn).
   - Tap the picture first: "avion. Atinge litera ca s-o auzi din nou." (no miss).
     Tap the letter: it sparkles, "a…", then a praise ("Bravo, ai reușit!" or another),
     the dot turns green, the mascot cheers, "Ai terminat lecția. Bravo!" and the big
     yellow **Mai departe**. Tap it: the map.
   - **Hai!** again opens `en-a`: the same screen with the blue ringed planet "EN" on
     each card and in the bubble, "Let's meet a new letter! Listen: a… a… apple! …".
4. **`ListenAndPick` (on `ro-t`), with a hint and the answer shown:** stop the app, then
   `printf '{"version":1,"lessonId":"ro-t","stepNumber":1,"stepsSignature":""}' > $S/ro-t.json`
   and `"$ADB" -s $EMU push $S/ro-t.json /storage/emulated/0/Android/data/com.softshack.scolita/files/profiles/child-1/lesson-run.json`;
   start, **Hai!**, do the `MeetLetter` step (letter, picture). Expected: two dots
   (green, yellow); three cards m, a, t (shuffled) with their stars and **Ascultă**
   under them; "Ascultă și atinge litera potrivită. Atinge litera t."
   - Tap m: it wobbles and fades, the mascot thinks, "Ascultă încă o dată. t…".
   - Tap a: it wobbles, t glows yellow, "Uite, aceasta este litera t. Atinge-o!".
   - Tap **Ascultă**: "Atinge litera t." again. Tap the mascot: the last line again.
   - Tap t: a sparkle and a praise, then the end. `lesson-run.json` then names `en-t`.
5. **Home and resume:** on the `ListenAndPick` step, tap the home button: the map at
   once. **Hai!**: the same lesson at the same step, its first dot green.
6. **The background mid-line:** while the mascot speaks, press the emulator's Home
   (`"$ADB" -s $EMU shell input keyevent KEYCODE_HOME`), wait, then bring the app back
   (start it again without `-S`). Expected: the line stops at once, and on return the
   line being said starts again from its start (the praise still after it);
   `lesson-run.json` holds the current step.
7. **The other size:** `"$ADB" -s $EMU shell wm size 1280x800; "$ADB" -s $EMU shell wm density 160`,
   repeat 3 and 4: the same layout, nothing cut off, the bubble's text fits. Reset with
   `wm size reset; wm density reset`.
8. **The Console stays clean** (`"$ADB" -s $EMU logcat -d -s Unity`): one information
   line names the kinds not built yet; no error but the development build's socket lines
   and, without installed voices, the voice's "no installed voice" warning.

### Cleanup

`"$ADB" -s $EMU shell pm clear com.softshack.scolita` (the app's saves, among them
`lesson-run.json`); `wm size reset; wm density reset`.
