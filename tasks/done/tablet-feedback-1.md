# Task: feature/tablet-feedback-1

> **Status: Done 2026-11-26, released as 0.14.1.** A fix (PATCH release) from the human's first try of the
> handwriting lessons with her on the Tab A7.

Read `AGENTS.md` first (especially "Learning", "Usability", "Performance" and the voice seam), and the
completion records of `tracing`, `handwriting` and `voice` in `tasks/done/`.

**Feature:** `feature/tablet-feedback-1`
**Implements:** PRD §3.4 (tracing), §3.7 (the voice); `docs/curriculum.md` §4.2 (tracing, the letters by
hand); `AGENTS.md` → "Usability" (every bubble is spoken, kind feedback) and "Learning" (the letter shapes
she will write at school).
**Depends on:** `tracing`, `handwriting`, `voice`, `content-letters-1` (all done).

## The human's report (2026-11-26, verbatim)

1. "either the lines that she's expected to follow with her finger on the screen should be thicker and
   therefore allow a like a 5-10% mistake, or the fact that the user might do a mistake and the letter is
   almost identical 90-95% to what it should be, then it should still be accepted as correct; otherwise the
   kid(s) can get frustrated as they think they did pretty well - when it's actually not perfect. They need
   to practice it to learn it, but a 90% job is fine - for some letters it might be the first time when a kid
   sees how it's written and it can't be perfect from the first try. Even I struggle a bit to do a perfect m
   (but perhaps my fingers are too thick, compared to the area that the stroke expects)."
2. "I've noticed that the strokes of the t letters are wrong/confusing: after creating the first line from
   top to bottom, the pencil should be moved in a diagonal so that it would do the other line that a small t
   has, and not having to actually draw from the bottom right of the t to where the upper line actually
   starts to be drew."
3. "When I come back to the app and press on Hai!, the unicorn shows up as if it would talk, and I can see
   its text, but there's no sound; if i press on the unicorn - it plays the sound correctly of what it should
   say when entering the writting screen for the first time; I see that on the normal flow (where the kid
   finishes everything on the screen and moves on to the next screen), then the initial sound of the unicorn
   of the second screen worked as it should have."

## Scope

1. **Tracing forgives a 90% good letter.** Find every check a trace can fail and which one rejected a good
   m; widen the accepted band to suit a child's (and an adult's) finger, forgiving a small wobble (about
   5–10% of the stroke); draw the guide line as wide as what is accepted, so what looks inside is inside.
   Print and handwritten alike. All numbers stay in `Settings/LearningRules.asset`.
2. **The pen lifts between strokes.** The move from one stroke's end to the next stroke's start dot (the t's
   bar after its stem) is shown as a hop through the air, never as a line to trace, and the next stroke is
   accepted from its own start dot. Check every letter with more than one stroke in planets 1–3, print and
   by hand.
3. **The first line is said after coming back.** Find the real cause of the silent first line on the
   writing screen after a cold start or a return from the background, and fix it at its cause inside the
   voice seam (`IVoice` and its implementations): a line asked for before the voice is ready is said once
   it is ready; only the newest line counts. Nothing allocates in an activity's frame loop.

## Out of scope — do not build

- New shapes or a new stroke order (the human's call; doubtful ones are listed for the human).
- New voice lines, new activity kinds, new packages.
- `sound-feel`'s music, effects and prefabs.

## Acceptance criteria

- A handwritten m (and the other planet 1–3 letters) traced about 90% well, a little wobbly, or with an
  adult's thick finger, is accepted in every round; one drawn backwards, one that clearly leaves the
  letter and one far too short are still not.
- The guide she follows (dots and the faint shadow) is as wide as the accepted band.
- Between two strokes the board shows a hop through the air to the next start dot, never a line to trace.
- After a cold start, and after a return from the background, the first line of the first writing screen
  (and of any screen) is heard, not only shown.

## Tests required

- EditMode: recorded strokes "about 90% good, a little wobbly" and an adult's thick-finger m pass, the
  bad ones still fail; the voice with a system voice that is ready late (only the newest line said).
- PlayMode: the board's hop between strokes; the lesson's first line waiting for a late voice where it fits.
- Emulator: drawn strokes (good, wobbly, backwards, off the letter, too short, two fingers), the t's two
  strokes, a cold start and a return from the background, then **Hai!**.

## Touches

`Scripts/Core/Tracing`, `Scripts/Core/Voice`, `Scripts/Game/Voice`, `Scripts/UI/Tracing`,
`Prefabs/Lesson/TraceLetterView.prefab`, `Prefabs/Lesson/TraceWordView.prefab`,
`Settings/LearningRules.asset` (the tracing rows only), `Tests/EditMode/Tracing`,
`Tests/EditMode/Core/Voice`, `Tests/EditMode/Fakes`, `Tests/PlayMode` (tracing and voice tests).

## Screens

The writing board (design screens 3 and 8): its guide's width and the hop between strokes.

## Voice lines

None new.

## Progress

_Kept by `tablet-feedback-1`._

### Status

- Claimed 2026-11-26 (`a5892a4`). Built and tested in the worktree (`C:\Projects\scolita-feedback`), all pushed
  together with the close-out (the slices need each other to compile, as tracing's did): `e341dec` `core:` (the
  stroke check, the guide's width), `a5bbd75` `core:` (the waiting voice), `b43f68c` `game:` (the numbers),
  `2688872` `game:` (Android's engine started again), `45290f1` `ui:` (strokes apart, the guide's width, the hop),
  `7de2f71` `ui:` (no folds, the hop's end), `204bf8f` `game:`, `1a06d0f` `game:` (the hop in both prefabs, the
  PlayMode test).

### Findings (the real causes)

**Units.** A shape's unit is a hundredth of the x-height (a small letter is 100 high; the writing lines span 0 to
300). On the Tab A7 (2000 × 1200, 224 dpi, 8.83 px/mm) the board's lines span 0.84 of its height: 2.18 screen pixels a
unit (measured on the emulator at 2000 × 1200: the middle line at y 652, the baseline at y 870), so **1 unit = 0.25 mm**
and a small letter is 25 mm high.

| | Corridor (either side) | Start radius | Coverage |
| --- | --- | --- | --- |
| Round 1, dots (was) | 32 = 7.9 mm | 34 = 8.4 mm | 0.8 |
| Round 2, shadow (was) | 28 = 6.9 mm | 30 = 7.4 mm | 0.85 |
| Round 3, start dot (was) | 24 = 5.9 mm | 28 = 6.9 mm | 0.9 |
| What she saw | dots 14 px = ±1.2 mm; the shadow 26 px = ±2.2 mm | | |

**1. Tracing too strict.** The corridor was already far wider than the line she saw, so a wobble was rarely the
reason. A trace can fail by: starting away from the dot (`StartedElsewhere`), being too short (`TooShort`, coverage or
the length of her line), going backwards (`WrongDirection`), skipping a handwritten entry (`SkippedEntry`), or leaving
the path (`LeftThePath`: more than one corridor's length of her line outside the corridor of the part reached). The m
failed on the last: `StrokeCheck` moves a "reached" mark along the stroke only within (moved + corridor) of path ahead.
Where she turns back up an m's stem a little before the baseline (16 units, 4 mm, is enough in round 3; an adult's
finger that lands 3–4 mm above where it aims does it at every stem), the stroke's turn lies more than that ahead, so the
mark waited at the foot of the stem while she went up it, her line counted as off the path, and the m was refused
(`LeftThePath`), in rounds 2 and 3 especially. A probe over every planet 1–3 shape (scratch harness) refused, with the
old numbers, an m drawn 16 units high, a thick finger 14 units high, and "about 90%" m's.

**2. The t's bar.** `StrokeLineGraphic.BeginLine` never started a second line (its test `lineEnds[last] < count` is
never true once a point is added), so every stroke of a guide was joined to the next by a drawn line: the t's dots and
shadow ran from the end of its hook (bottom right) up to the bar's start dot, which looks like a line to trace. The same
joined the slant guides into a zigzag, her kept strokes in ink, the small pictures beside the board and the "de mână"
tile. The check itself never asked for that line (the bar is judged from its own dot).

**3. The silent first line.** Android's `TextToSpeech` starts a moment after it is made (`onInit` is asynchronous),
and each scene makes its own (`VoiceHost`). The planner chose the bubble for every part while the engine wasn't ready,
so the first line of the Lesson scene (and of the map) showed only its bubble; tapping the unicorn later found the
engine ready. Reproduced on the emulator: with the TTS service cold (`am force-stop com.google.android.tts`, as a
low-end tablet does to a service it isn't using), Hai! gave `Voice: Bubble ro` for the whole first line. A second
cause on a return from the background: once Android has let the engine go, `getVoices()` returns nothing and `speak`
fails, so every line of that scene stayed in the bubble ("no installed ro-RO voice" in the log) until the scene
changed. Audio focus and the music's ducking play no part (the line is never given to the voice).

### Design note (a small Core change, a UI fix, a voice-seam change)

- **Tracing (Core, `StrokeCheck`):** the reached mark may also move over the samples ahead that lie within the
  corridor of her point (so it passes the turn at the foot of a stem), never from her first point (where she began
  stays the start of what she drew); of samples about as near, the earliest wins (half a sample), so the down and up
  sides of a stem are not confused. A short stroke's end allowance is three quarters of the start radius (was half;
  a finger that lands a little along a stem from where she aims still reaches its end), never under three fifths of
  it (half a bar is still too short).
- **The numbers** (`Settings/LearningRules.asset`, one place): corridors 36 / 32 / 30 (8.9 / 7.9 / 7.4 mm), start
  radii 38 / 34 / 32 (9.4 / 8.4 / 7.9 mm), coverage 0.8 / 0.85 / 0.85, the join radius 44 (unchanged), and a new
  `traceFingerAllowance` 20 (4.9 mm, half a child's fingertip).
- **The guide as wide as what is accepted:** `TraceState.GuideHalfWidth` = the round's corridor less the finger
  allowance: what is judged is the middle of her fingertip, so the guide is drawn as wide as the band her fingertip
  may touch and still be on the stroke. Round 1 shows its dots on a pale road 32 units wide (7.9 mm, was dots of
  2.4 mm), round 2 a shadow 24 units wide (5.9 mm, was 4.5 mm); round 3 shows the start dot only, as before. A road
  as wide as the whole corridor (18 mm in round 1) would fill the m's counters (its stems are 9 mm apart).
- **The pen lift (UI):** `BeginLine` starts a new line whenever the last one holds points, and a graphic that is
  full drops the next lines' points instead of joining them. `WritingBoardView` draws a dotted yellow hop (a curve
  bowing upwards, with a small arrowhead) from where the stroke just done ends to the next start dot, whenever the
  next stroke begins after a lift, in the rounds that show the arrow. New `Hop` and `HopHead` objects in both board
  prefabs (added with an Editor script, as earlier briefs did).
- **The voice (seam 4):** `ISystemVoice.IsStarting`; the planner gives the system voice the parts a starting voice
  may say; `VoiceRouter` waits at such a part (silent, still "speaking", up to `VoiceTiming.SystemVoiceWaitSeconds`,
  5 s on the Voice prefab), then says it, or shows the bubble if the voice never gets ready. A newer line replaces the
  waiting one (only the newest counts); `Stop` forgets it. `AndroidTextToSpeechVoice` reports `IsStarting` while its
  engine starts, and starts the engine again (at most twice in a row) when Android has let it go. Waiting allocates
  nothing.
- **Another domain** (numbers, music) traces digits and notes through the same check and board, and speaks through
  the same voice: nothing here is about letters.
- **Tests:** EditMode: recorded "about 90% good, a little wobbly", "a thick finger, a little above" and "turned back
  before the baseline" lines for a m t p (print and by hand) pass every round (the old check refused the m's and the
  p's in rounds 2–3); the bad recorded lines still fail; the late voice (waits, only the newest, never ready, starts
  again, stopped, no allocation); the tuning's finger allowance. PlayMode: the t's guide is two lines, the hop shows
  after the stem, the faint round's guide is narrower.

### Letters with more than one stroke (planets 1–3)

Every one of them had its strokes joined in the guide (the same cause), and every one now shows the hop: print a, d,
t (lower), A, E, T (upper) and i's dot; by hand t, i's dot, A, P, T. The handwritten a, d and o continue their strokes
(no lift, no hop). **Doubtful for the human (not changed, drafts):** the print A goes down its left side and back up
before the right side, and the print E down its stem and back up before the top bar (a child may expect two separate
strokes from the top); the handwritten T writes its top wave first, then the stem.

### Test results (2026-11-26, at `1a06d0f`, after the last rebase)

- **Batch mode:** EditMode 848/848 (was 842), PlayMode 64/64 (`TestShow_TheTsBarIsAHopNotALine` new),
  `BuildAndroidDebug` succeeded; no compiler warning, no error.
- **The check, every planet 1–3 shape** (a scratch harness compiling Core with Unity's Roslyn): with the old check
  and numbers, the recorded "a thick finger, a little above" m (print and by hand) and p, and "turned back before the
  baseline" m, were refused in rounds 2–3 (the handwritten m in every round); now every new recorded line passes
  every round, and every bad one (backwards, off the path, too short, begun elsewhere, half a bar) still fails.
- **Emulator** (`emulator-5602`, `Scolita_Tablet_tablet-feedback-1`, 2000 × 1200 at 240 dpi, Android 16, Google's
  ro-RO and en-GB voices installed on this AVD only), with drawn strokes (`adb shell input motionevent` chains from
  the stroke data):
  - **The handwritten m** drawn 16 units (4 mm) high with a wobble, then 14 units high and 4 to the right, then 12
    high with a bigger wobble: accepted in rounds 1, 2 and 3; „Super, ai răspuns corect!”.
  - **The handwritten t:** the dots on their pale road, the shadow, no line from the hook to the bar any more; after
    the stem, the dotted yellow hop to dot 2 and „Foarte bine! Acum mergi la punctul cu numărul doi.”; the bar from
    its own dot accepted; round 2 with a thick finger (14 high) and a wobble accepted. Round 3: the stem backwards
    („Urmează săgeata.”), the bar leaving the letter (the answer shown, „Uită-te cum se scrie, apoi încearcă tu.”),
    45 % of the bar (refused), then a bar a little short and low (accepted).
  - **The first line:** a cold start with the TTS service stopped (`am force-stop com.google.android.tts`): the
    map's greeting and, after **Hai!** (the service stopped again), the lesson's first line were `System ro` (the
    lesson's 0.5 s after the scene was ready); before the fix both were `Bubble ro`. **A return from the
    background** with the service stopped meanwhile: "the system voice has let go of its engine; starting it again",
    then the line said again `System ro` 0.6 s later; tapping the unicorn after it: `System ro` (before: every line of
    the scene `Bubble`, "no installed ro-RO voice").
- **Screen sizes:** 2000 × 1200 (the Tab A7) and 1280 × 800 (`wm size`; the print t, its road and its hop): the board
  is unchanged in layout, only its guide is wider and the hop is added, so the two sizes "every change on screen" asks
  for are enough. Screenshots in the scratch folder.
- **Not run:** the Device Simulator (the Editor wasn't opened; the emulator stood in), two fingers on the emulator
  (`input` has one pointer; `TraceFinger` is unchanged and its PlayMode test passes), the device Profiler (the board
  draws a few more points when a stroke is done, not per frame; `TestMove_DrawingAllocatesNothing` and
  `TestTick_WaitingAllocatesNothing` pass), listening by ear (no new line).

### Still to test

- Nothing: everything above ran after the last rebase.

### How the board meets "Usability" and "Learning"

- The guide is easier to follow (a road under the finger, not a 2 mm line), and a 90 % letter is praised, not refused:
  kind feedback for a first try. Backwards, off the letter and far too short still get their hint, so she still
  learns the direction and the shape.
- The pen lift is shown, not said only: the hop points to the next numbered dot while „Acum mergi la punctul cu
  numărul doi” is heard.
- Every bubble is spoken: the first line now waits a moment for the tablet's voice instead of showing silently.

### Follow-ups

- **The human:** try 0.14.1 with her (step in `local/for-human-next-steps.md`): is a well-drawn letter still refused,
  a messy one accepted, the road too wide or too narrow (`traceFingerAllowance`), the hop clear? Check three drafts
  against her manual: the print A (left side down and back up before the right side), the print E (stem down and
  back up before the top bar), the handwritten T (top wave first).
- **`sound-feel`:** its music and effects come from the same scenes; nothing here changes them (the voice's
  `Started` comes when the line starts waiting, so the music ducks a moment early, which is harmless).
- **Later:** the lesson's own bubble still shows as the line is asked for, a moment before the voice when the
  tablet's voice is waking; showing it on `PartStarted` only would need `LessonVoice` to know a missing line from a
  waiting one.

## How to test locally

### Core slice (the stroke check, the guide's width, the waiting voice)

1. **Automated** (close your Editor first), from the worktree root, `UNITY` =
   `C:\Program Files\Unity\Hub\Editor\6000.3.29f1\Editor\Unity.exe`:
   ```bash
   "$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode \
     -testResults <scratch>/editmode.xml -logFile <scratch>/editmode.log
   ```
   All pass (848). The ones for this brief: `RecordedStrokeTests` and `HandwrittenStrokeTests` (the new lines
   "about 90% good, a little wobbly", "a thick finger, a little above" and, for m, "turned back before the baseline"
   in `Tests/EditMode/Tracing/Strokes/` are `Good` in every round; "backwards", "off-the-path", "too-short",
   "begun-elsewhere" still fail), `StrokeCheckTests` (the rounds tighten at 33 units beside the stroke; half a bar is
   too short), `LearningRulesTests.TestForRound_EachRoundAndItsOrder` (the finger allowance),
   `VoiceRouterTests` (`TestSpeak_WaitsForTheSystemVoiceToStart`, `TestSpeak_OnlyTheNewestLineWaits`,
   `TestSpeak_ShowsTheBubbleWhenTheSystemVoiceNeverStarts`, `TestSpeak_WaitsWhenTheSystemVoiceStartsAgain`,
   `TestStop_ForgetsAWaitingLine`, `TestTick_WaitingAllocatesNothing`).
2. **The numbers** are in `Assets/Scolita/Settings/LearningRules.asset`: `traceDottedCorridor` 36,
   `traceDottedStartRadius` 38, `traceFaintCorridor` 32, `traceFaintStartRadius` 34, `traceStartDotCorridor` 30,
   `traceStartDotStartRadius` 32, `traceStartDotCoverage` 0.85, `traceFingerAllowance` 20 (a shape's unit is 0.25 mm on
   the Tab A7).

### Game and UI slices (the board, the hop, the tablet's voice)

**Prerequisites:** the debug APK (`Scolita.Editor.BuildScript.BuildAndroidDebug`, then
`adb -s emulator-5602 install -r <scratch>/Scolita-<version>-debug.apk`); an emulator at 2000 × 1200 (density 240)
with Google's TTS voices **Romanian (Romania)** and **English (United Kingdom)** installed on it (Settings, or
`adb shell am start -a android.speech.tts.engine.INSTALL_TTS_DATA -p com.google.android.tts`, then download both);
the app started with `adb -s emulator-5602 shell am start -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`.
To open a lesson's step directly (a rooted emulator, `adb root`): stop the app, write
`profiles/child-1/progress.json` with the lessons before it in `doneLessons` and `rocketStop` set to it, and
`profiles/child-1/lesson-run.json` with `lessonId`, `stepNumber` and its `stepsSignature` (ro-t: `c377369933d0db45`,
ro-m: `f52e968460b22b89`), under `/data/media/0/Android/data/com.softshack.scolita/files/`, then start the app and tap
**Hai!**. In the Editor (Play mode, Boot scene), the tester profile's **Sari peste pas** does the same.

**PlayMode** (batch, as above with `-testPlatform PlayMode`): `TracingTests.TestShow_TheTsBarIsAHopNotALine` and the
rest pass.

**Checks** (2000 × 1200 unless said; all verified on `emulator-5602`):

1. **The handwritten t** (`ro-t`, step 5). Round 1: dots on a pale road about as wide as a fingertip; no line from
   the end of the hook (bottom right) to the bar. Trace the stem from dot 1 (up the entry, down, out to the right):
   „Foarte bine! Acum mergi la punctul cu numărul doi.” and a dotted yellow hop curving from the hook's end to dot 2,
   ending at it with a small arrowhead. Trace the bar from dot 2: round 2 („… Acum scrie pe umbră.”), a narrower
   shadow, still no joining line. Round 3 shows only dot 1; after a miss the arrow and the hop come back.
2. **A thick finger on the m** (`ro-m`, step 5): trace the m about 4 mm above it and a little to the right,
   wobbling, turning back up each leg a little above the baseline: accepted in all three rounds.
3. **Still refused:** the stem drawn backwards („Urmează săgeata.”); a stroke that leaves the letter half-way; half
   a bar or less; a stroke begun far from the dot („Începe de la punctul verde.”). Two misses on one stroke show the
   answer („Uită-te cum se scrie, apoi încearcă tu.”).
4. **Print** (`ro-t` step 4, `ro-m` step 4; also at 1280 × 800: `adb -s emulator-5602 shell wm size 1280x800`,
   `wm density 160`, restart the app; `wm size reset` and `wm density reset` after): the same road, hop and
   tolerance; the print t's bar is reached by a hop from the stem's foot.
5. **The first line after a cold start:** `adb -s emulator-5602 shell am force-stop com.softshack.scolita`, then
   `am force-stop com.google.android.tts` (the tablet's voice cold, as on a slow tablet), start the app: the map's
   greeting is heard (`Voice: System ro` in `adb logcat -s Unity`), not only shown. Stop the TTS service again, tap
   **Hai!**: the lesson's first line is heard, a moment after its bubble.
6. **A return from the background:** in a lesson, press Home, `am force-stop com.google.android.tts`, open the app
   again: the log says "the system voice has let go of its engine; starting it again" and the line is said again
   (`System ro`); tapping the unicorn says it again.
7. **No voice on the tablet** (unhappy path; Editor Play mode, or a tablet with the voices uninstalled): every line
   shows in the bubble at once (the Editor) or after the 5 s wait (a voice that never starts), never stuck.

**Cleanup / reset:** `adb -s emulator-5602 shell pm clear com.softshack.scolita`; in the Editor, a fresh
`SCOLITA_SAVE_ROOT` folder.

## Completion record

_Finished 2026-11-26 by `tablet-feedback-1`; released as 0.14.1 (tag `v0.14.1`)._

### What was built

- **Pushed:** `a5892a4` the claim; `e341dec` `core:`, `a5bbd75` `core:`, `b43f68c` `game:`, `2688872` `game:`,
  `45290f1` `ui:`, `7de2f71` `ui:`, `204bf8f` `game:`, `1a06d0f` `game:`; the close-out.
- **Tracing** (`Scripts/Core/Tracing`): `StrokeCheck` passes a turn within the corridor of her finger (never from her
  first point), prefers the earliest of near-equal samples, allows three quarters of the start radius at a short
  stroke's end, never under three fifths; `TracingTuning.FingerAllowance`; `TraceState.Tolerance` (used by
  `TraceJudging`) and `TraceState.GuideHalfWidth`; the numbers in `Settings/LearningRules.asset` (Findings and
  Design note above).
- **The board** (`Scripts/UI/Tracing`): `StrokeLineGraphic.BeginLine` starts a new line again, a full graphic drops
  further lines, a wide strip's inside edge waits instead of folding, `SetWidth`, `LineCount`; `WritingBoardView`
  draws the road and the shadow at `GuideHalfWidth` and the hop (`Hop`, `HopHead` in `TraceLetterView.prefab` and
  `TraceWordView.prefab`).
- **The voice** (`Scripts/Core/Voice`, `Scripts/Game/Voice`): `ISystemVoice.IsStarting`, the planner and
  `VoiceRouter`'s wait, `VoiceTiming.SystemVoiceWaitSeconds` (`VoiceHost`'s `systemVoiceWaitSeconds`, 5 s),
  `AndroidTextToSpeechVoice`'s engine started again, `SilentVoice.IsStarting`.
- **Tests:** EditMode 848 (was 842): the recorded lines "about 90% good, a little wobbly", "a thick finger, a little
  above", "turned back before the baseline" (a m t p, print and by hand), six `VoiceRouterTests`, the finger
  allowance; expectations moved for the "wobbly" print lines (now good in round 2) and a few verdicts. PlayMode 64
  (was 63): `TestShow_TheTsBarIsAHopNotALine`.

### Conventions for later briefs

- **A guide is drawn as wide as what is accepted for a fingertip** (`TraceState.GuideHalfWidth`): change the corridor
  or `traceFingerAllowance` in the learning rules, never the prefab's width.
- **Each stroke is its own line** on every `StrokeLineGraphic`: call `BeginLine` per stroke; a hop, never a line,
  joins two strokes she writes with a lift.
- **A system voice may be starting** (`ISystemVoice.IsStarting`): a new implementation says so while it gets ready,
  and the router waits for it; tests use `FakeSystemVoice.IsStarting` and `LostOnNextSay`.
- **Probing the stroke check** across every shape is quick outside Unity: compile Core and the EditMode fakes with
  Unity's Roslyn (as `handwriting`'s and this brief's scratch harnesses did) and print the verdicts per round.

### How it was tested

Batch mode after the last rebase (EditMode 848/848, PlayMode 64/64, `BuildAndroidDebug`, no compiler warning), the
emulator at 2000 × 1200 and 1280 × 800 with drawn strokes (the m thick-fingered in three rounds; the t with its hop;
backwards, leaving the letter, half a bar), a cold start and a return with the TTS service stopped (Progress → "Test
results"). Școlița → Record released ids at this close-out (`VERSION` 0.14.1): nothing was added (no new id). Not
run: the Device Simulator, two fingers on the emulator, the device Profiler, listening by ear (no new line).

### Decisions and deviations

- **The guide is as wide as the corridor less a fingertip's allowance,** not the whole corridor: what is judged is
  the middle of her fingertip, and a road as wide as the whole corridor (18 mm in round 1) would fill the m's
  counters. A fingertip touching the road's edge is still on the stroke.
- **The shapes and their order are unchanged;** three doubtful drafts are listed for the human.
- **No fresh-eyes review:** a fix within existing seams (no change to saving, the content model, the importer or the
  build); the brief's Design note records the reasoning.

### Follow-ups

In Progress → "Follow-ups": the human (try it with her, the road's width, the three drafts), `sound-feel` (nothing to
do), the lesson's bubble shown on `PartStarted` later.
