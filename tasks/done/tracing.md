# Task: feature/tracing

> **Status: Done 2026-11-25, released as 0.7.0.** (Decisions D2: lower case first;
> D3 answered 2026-11-24: handwritten letters are in the MVP, built by
> `handwriting` right after this brief — the stroke format made here must
> carry a letter's print and handwritten forms, with entry and exit
> strokes and join points).

Read `AGENTS.md` first, then `design/README.md` and `design/screens.md` (build the screens as designed there) (especially "Testing before a push" → tracing,
"Performance", "Learning" → letter shapes), and the completion records in
`tasks/done/`.

**Feature:** `feature/tracing`
**Implements:** PRD §3.3; `docs/curriculum.md` §4.2 (`TraceLetter`), §7.
**Depends on:** `lesson-engine`; the stroke order from
`curriculum-research`.

A **medium change** with a new data format: Design note first (the stroke
format, the check, how the handwritten forms (`handwriting`, next) and digits for the numbers domain would
use it).

## Scope

1. **Stroke data** in `content/strokes/<letter-id>-<case>-<form>.json`
   (`form`: `print` now, `handwritten` from `handwriting`): the letter
   in a unit box with three writing lines (top, middle, baseline), its
   strokes in order, each a polyline (or a few cubic segments) with its
   start and its direction; importer and validator support (every letter
   in a lesson with `TraceLetter` has strokes for the case asked).
2. **Core's `StrokeCheck`:** given a stroke and the child's points
   (normalised to the box), it says `Good`, `StartedElsewhere`,
   `WrongDirection`, `LeftThePath`, `TooShort`; with a corridor width, a
   start radius and a coverage share from the learning rules (more
   forgiving in round 1, tighter in round 3). Pure, allocation-free with a
   reused point buffer.
3. **`TraceLetter` rules:** three rounds (dotted guide, faint guide, start
   dot only), strokes in order; a stroke that fails fades and the dot
   pulses again; the hint after two failed tries animates the stroke once.
4. **The writing board view:** the slate with the three lines, the start
   dot and arrow, **the rainbow trail with stardust** under her finger
   (one finger only: a second finger is ignored, a palm resting is
   ignored), a pooled trail mesh; a finished letter bounces and says its
   sound.
5. **Stroke data for planet 1** (a m t p, lower case; upper case if D2
   says both), from `curriculum-research`.

## Out of scope — do not build

- Handwritten letters and words (`handwriting`, next: only the format and
  `StrokeCheck` must serve them), free drawing (`drawing-pad` will reuse
  the brush).

## Acceptance criteria

- On the emulator with drawn strokes: a good "a" passes all three rounds;
  starting the "a" at the wrong end is `WrongDirection` with a kind hint;
  a stroke that wanders off is `LeftThePath`; a dot-tap is `TooShort`.
- With two fingers down, only the first draws.
- The trail keeps up with a fast finger at 60 fps in the Editor's
  Profiler, with 0 B allocated per frame while drawing.

## Tests required

- EditMode: `StrokeCheck` on recorded strokes in
  `Tests/EditMode/Tracing/Strokes/` (good, sloppy-but-fine, backwards, off,
  too short, for each planet 1 letter), the rounds' tolerances, the stroke
  file validator.
- PlayMode: a scripted trace completes a letter.
- Emulator: `adb shell input` swipe chains for each case above; a
  screenshot of each round at 2000 × 1200.

## Touches

`Scripts/Core/Tracing`, `Scripts/Game/Tracing`, `content/strokes/`, the
importer (a reader), the `TraceLetter` prefab (new), `ActivityRegistry`
(one row).

## Screens

The writing board (all sizes in `AGENTS.md`'s table: it is the most
space-hungry screen).

## Voice lines

"Urmează săgeata" / "Follow the arrow", "Începe de la punct" / "Start at
the dot", the hint lines, both languages.

## Before closing out

- Release (the next MINOR). Ask the human to let her try tracing on the
  Tab A7 and note whether the tolerances feel right.

## Progress

_Kept by `tracing` while the brief is in flight._

### Status

- **Claimed** in `35833d0`.
- **Design note** (below): treated as a **large change**, because it adds a
  content type to the content model (the letters' shapes, a new catalog in
  the `Curriculum`) and changes which steps the lesson engine can play.
  **The human agreed it on 2026-11-24** (relayed by the lead): questions 1,
  3, 4, 5, 6 and 7 as recommended; question 2 answered **two strokes** (see
  "The human's answers" below).
- **Built and pushed** (tested together at the last of them, after the
  fresh-eyes review):
  - `70a74a3` `core:` shapes, the JSON reader, `StrokeCheck`, `TraceLetterRules`,
    `PlayableSteps`, `ShapeRules`, the tolerances;
  - `5acd8e7` `content:` the shape asset and its catalog;
  - `3477c73` `build:` the importer reads `content/strokes/`;
  - `769a3c9` `content:` planet 1's eight print shapes and the recorded strokes;
  - `1dc3026` `game:` `TraceFinger` and `FingerPicker`;
  - `ac72ba3` `ui:` the writing board view;
  - `070da45` `game:` the prefab, the registry row, the scene's catalog, the
    PlayMode tests;
  - `60f8a82`, `596682d`, `caf7864`, `5765d2a`, `ee04ea0`: the review's fixes
    (below).
- **The test APK** for the human: `local/apks/Scolita-0.6.0-tracing-try-debug.apk`
  (step 2 of `local/for-human-next-steps.md`).
- **Closed out** as release 0.7.0 (see "Completion record").

### The human's answers (2026-11-24)

1. The stroke format, shapes as content with their own catalog, not in the
   ledger of released ids: **yes**.
2. **The print „a” is two strokes:** the round, then the stem as a separate
   stroke with the pencil lifted, as design screen 3 draws it (so `look`
   needn't redraw it). The human's words: "a is 2 strokes if I try to use a
   pencil and actually write the a that is being printed by the majority of
   fonts. An 'a' on a font that would look like it was written by hand would
   have 3 strokes." The draft's one-stroke `a` is split into two strokes in
   `a-lower-print.json`. **For `handwriting`** (a follow-up): the "3 strokes"
   is read by the lead as the handwritten „a” (litera de mână, with its
   entry stroke); still to be checked against her manual (Follow-ups →
   "`handwriting`: the handwritten a").
3. Only print tracing until `handwriting` (no tracing in the English lessons
   yet): **yes**.
4. Planet 1's capitals A M T P in print too: **yes**.
5. A faint fourth line below the baseline: **yes**.
6. The round labels as pictures, without words, for now: **yes**.
7. A lifted finger ends the stroke, judged as it is: **yes**.

### Decisions while building (within the agreed note)

- **The print a is two strokes** (the human's answer 2): `a-lower-print.json`
  has the round (from the top right, anticlockwise, closing near its start)
  and the stem (from the middle line down), so design screen 3 stands. The
  draft's other print letters are used as drawn: m and p one stroke each, t
  two (the stem with its flick, then the bar), A two, M one, T two (the
  stem, then the top bar), P one. All eight files are `draft`.
- **A shape file removed has its asset removed** at the next import (the
  review's finding 7; the note had said "refused like a deleted row").
- **Leaving the path is judged near where she is,** not against the whole
  letter: a point is off when it is farther than the corridor from the part
  of the stroke she has reached (a line cutting across m to its last stem,
  or into a's round, has left it). Her line may stray for at most the
  corridor's width in all before it is `LeftThePath`.
- **`WrongDirection` needs her to begin at the stroke's end** (for a round
  stroke the end is near the start): a line begun near the start that
  wanders is `LeftThePath`, never "follow the arrow" (found by the recorded
  wobbly bar of t, which the window ahead let the backwards follow reach).
- **A stroke drawn during a round's pause starts the next round at once**
  (the ink of the round just finished goes): she is never made to wait.
- **The answer shown uses round 1's tolerance** for that stroke (its dotted
  guide is shown then, in every round).
- **The writing board holds at most 4 strokes** (`ActivityLimits.MostStrokes`,
  checked by the import), like the other screens' limits.
- **The steps that can be played** are a `PlayableSteps` (the kinds with a
  view, and `ActivityRulesFactory.CanCreate`: a tracing step's shape is in
  the content); `LessonPlan.Of` and `LessonOrder` take it instead of the
  list of kinds. 14 handwritten tracing steps of planet 1 are skipped, with
  a warning at each import ("skipped until it has one").
- **`ReadCsvs` is now `ContentFolder.ReadTexts`:** it reads the CSVs and the
  shape files (`strokes/a-lower-print.json` in the texts' names).
- **No new voice line:** the instruction is `trace-print` + `trace-start`;
  the hints are `trace-hint-dot`, `-arrow`, `-stay`, `-slow`, `-watch`; the
  checkpoints `trace-next-2`, `trace-next-3`, `trace-done` (with the letter's
  sound spliced in) and `trace-round-2`, `-3`. All were in
  `voice-lines.csv`; `ActivityKinds` now requires them for `TraceLetter`.
- **The recorded strokes** in `Tests/EditMode/Tracing/Strokes/` are made from
  the shapes with seeded jitter (a good line, sloppy but fine, wobbly,
  backwards, off the path, too short, begun elsewhere, for every stroke of
  the eight letters, each with its verdict in each round), not drawn by a
  finger: no real recording exists yet. The emulator runs below drew real
  touch strokes through the app.
- **Drawing allocates nothing:** the stardust sparkles stay active and are
  hidden by their alpha (enabling a UI element can allocate), and the finger
  glow likewise.

### Fresh-eyes review (2026-11-25)

A second agent, which didn't write the code, read the seven commits and the
uncommitted fixes against AGENTS.md. It found the seams intact (Core and its
JSON reader have no engine reference; verdicts and rounds live in
`StrokeCheck` and `TraceLetterRules`), the main cases of the check right, every
voice line used present in both languages, and no allocation on the drawing
path. Its findings, and what was done:

1. **Blocker: round 2's faint shadow was cut off** (the prefab gave it 64
   points). Fixed: 4096 like the dotted guide; seen whole on the emulator.
2. **A short stroke could pass half drawn, or backwards:** coverage counted
   from the start, and the first point's window let `reached` jump ahead.
   Fixed: coverage is now the part she drew herself (from where her first
   point reached the stroke); a stroke shorter than the start radius is told
   by direction. Test `TestCheck_AShortBar` (half a bar `TooShort`, a bar and
   a 20-unit tick drawn backwards `WrongDirection`).
3. **The end was very strict on short strokes** (round 3 asked for t's bar to
   within 5 units of its end). Fixed: "enough" is the coverage share or all of
   the stroke but half the start radius at its end, whichever is less (never
   under half). The tolerances stay for the human to try.
4. **After the answer is shown she could get stuck** (every miss replays it).
   Kept as the lesson run's one ladder (the shown stroke uses round 1's
   tolerance); **a question for the human** in Follow-ups: accept any stroke
   from the dot after the answer, or move on after N tries.
5. **`TraceFinger`'s history:** (a) a second finger's or a palm's `Began`
   moved the drawing finger's high-water mark. Fixed: set only when the touch
   is the one drawing. (b) The history's order: confirmed in the Input
   System's source (`TouchHistory`: index 0 is the newest), so it is read from
   the last index to the first.
6. **A palm that lands lightly and grows:** fixed: a drawing touch whose
   radius grows past the palm threshold is cancelled; the radius is the
   larger of its two axes.
7. **A shape could never be removed** (the importer refused it as a deleted
   row). Fixed: shapes are replaced, not retired, so the importer removes the
   asset of a shape whose file is gone; test
   `TestApply_ShapesKeepTheirGuidAndGoWithTheirFile` (also the GUID kept on a
   re-import); `content/README.md` says so.
8. **A raw byte order mark (and Romanian letters) in C# literals:** now `\u`
   escapes.
9. **Round 3 said "follow the arrow" with no arrow shown:** the prompt there
   is now „Începe de la punctul verde.” (`trace-hint-dot`).
10. **Accidental touches count as misses** (two taps show the answer): a
    question for the human (Follow-ups).
11. **„Bravo, ai scris litera a!” says the letter's sound,** as every line of
    planets 1–3 does (names come later): for the human's listening check.
12. **Tests:** the unused line in the PlayMode test removed; the test names
    follow `Test<Method>_<Case>`; the recorded strokes are said to be generated
    (below, and in "How to test locally"); the Profiler check is still to do
    on the tablet. Not added: a test of `TraceFinger.Update` (it needs the
    Input System's test fixture, which isn't in the project's testables).
13. **`TracingTuning`** now checks the start radius tightens too, and names
    the right round.
14. **Numbers in the shape reader's errors** are written with the invariant
    culture.
15. **A dot is `Good` for any line begun on it:** kept for now (no dot in
    planet 1); a follow-up for `handwriting` (i's dot).
16. **Docs and robustness:** `TracePoints`' callers, `PlayableSteps`' null
    check, fixed.

### Test results (2026-11-25, at `ee04ea0`, before the docs)

- **Batch mode:** EditMode **486/486** (was 413), PlayMode **17/17** (was 14),
  `BuildAndroidDebug` exit 0 (`Scolita-0.6.0-debug.apk`); no `warning CS` or
  `error CS` in any log.
- **The import:** exit 0, "imported, with 37 warnings" (17 placeholder pictures, 8
  draft shapes, 12 handwritten tracing steps skipped); `git status` clean after.
- **The emulator** (`emulator-5600`, Pixel Tablet, Android 16 image, `wm size
  2000x1200`, `-force-gles`), strokes drawn with `input motionevent` chains and raw
  `sendevent` multi-touch (see "How to test locally"):
  - round 1: a good round and stem: rainbow ink kept, „Foarte bine! Acum mergi la
    punctul cu numărul doi.”, the bounce and „… Acum scrie pe umbră.”;
  - round 2: the whole faint shadow; backwards: „Urmează săgeata.”; off the
    letter (second miss): „Uită-te cum se scrie, apoi încearcă tu.” and the round
    drawing itself; half a stroke: a miss;
  - round 3: the start dot only; a tap: „Urmează săgeata.”; begun at the
    bottom: the answer shown;
  - **two fingers:** the first finger's stem counted, the second drew nothing,
    the letter done and the lesson moved on;
  - **a palm:** not verifiable on the emulator: its touch device reports no
    contact size to the app, so a large `ABS_MT_TOUCH_MAJOR` came through as a
    normal finger (it became the drawing touch). Verified in PlayMode with a
    scripted radius (`TestPress_OneFingerNoPalm`); **to check on the Tab A7**.
  - The development console showed "Unhandled pen event action: 5/6" during the
    raw multi-touch runs: the emulator's virtio touch device also declares a
    stylus; not seen with `input motionevent`.
- **Screen sizes** (screenshots in the scratch folder): 2000 × 1200, 1280 × 800,
  2560 × 1600, 2048 × 1536 and 2400 × 1080: the board on the left below the
  mascot's bubble (the bubble covered the board's top at first: the board now ends
  at 69% of the height), the letter centred and whole, the stars and round
  pictures on the right. The cut-out profile was not run (the board sits inside
  the lesson screen's safe area, like the other views).
- **Allocation:** `TestMove_DrawingAllocatesNothing` (the finger moving, the trail
  and stardust growing, the lines' meshes rebuilt: 0 B), and
  `TestCheck_AllocatesNothing`, `TestAdd_AllocatesNothing` in EditMode. **Not
  measured:** the whole frame in the device Profiler (no Editor window in this
  session); to measure on the Tab A7.
- **Not run:** the Device Simulator (batch mode only; `wm size` stood in),
  listening by ear (no new line; the lines said were checked by their lengths in
  the log), frame time on a real tablet.

### How the writing board meets "Usability" and "Learning"

- **Spoken:** every bubble is a voice line (the instruction, each checkpoint,
  each hint, the answer shown); tapping the mascot repeats the prompt (the
  arrow's line while the arrow shows, the green dot's in round 3).
- **Big targets:** the board is about 65% of the screen's width and 65% of its
  height (the tracing area as large as the lesson screen allows beside the
  mascot and the rounds); the start dot is 60 px at the reference size (about
  90 dp on the Tab A7), the corridor about 32 units either side in round 1.
- **Kind feedback:** a miss fades her line and pulses the dot, never red, never
  lost stars; the second miss shows the stroke.
- **One finger, no pressure:** no timer; a second finger and a palm are
  ignored; home is always there.
- **The language:** the board carries the RO star; the lines are Romanian.
- **Learning:** one stroke at a time, in the order and direction of the stroke
  data; three rounds from most to least help; print only until `handwriting`.

### Follow-ups

- **`handwriting`:**
  - the handwritten shapes (`<id>-lower-handwritten.json`, entry, body and
    exit runs, `slant` 0.2): the 14 handwritten `TraceLetter` steps of planet
    1 play as soon as their files exist; make a missing one an error then
    (`ShapeRules`);
  - the board's handwritten look: the "tip I" lines (dashed, solid, solid,
    dashed) and faint slant guides; the entry-stroke hint („Pornește de pe
    linie.”, never for m and n) from `StrokeResult.Covered` over the entry
    run; `TraceWord` joining shapes at their exit and entry points, and a
    lifted finger at a join forgiven;
  - the English lessons' `TraceLetter` is "one round by hand"
    (curriculum §4.1): a `rounds` option on the step, which
    `TraceLetterRules` would take (it always plays three rounds now).
- **For the human:** the tolerances (corridor, start radius, coverage per
  round, in `Settings/LearningRules.asset`) are first guesses: watch her
  trace on the Tab A7 and say whether a stroke she drew well was refused,
  or a sloppy one passed. The eight print shapes are drafts: check them
  against `print-stroke-order-draft.svg` and her manual.
- **Real recorded strokes:** the recorded lines are made from the shapes;
  strokes drawn by her finger on the tablet (logged in a development build)
  would make better test data.
- **`look`:** the round pictures beside the stars are drawn from the shape
  (a dotted, a faint, and a start dot); the board's slate and dots are
  `look`'s; the larger, 4:3, phone and cut-out sizes of the board are in
  "Test results".
- **The voice:** a stroke's checkpoint line starting allocates (the voice's
  planning, as `lesson-engine` found): per stroke, not per frame.
- **For the human (from the review):** after the answer is shown, should any
  stroke from the dot count (or the step move on after a number of tries in the
  learning rules)? Should a touch of only a few units be ignored instead of
  counting as a miss? And „Bravo, ai scris litera a!” says the letter's sound.
- **`handwriting`:** a dot stroke (i's dot) is `Good` for any line begun on it;
  check its length too.
- **`handwriting`: the handwritten a** (the human, 2026-11-24, still to be
  checked against her manual). The human's words: "An 'a' on a font that
  would look like it was written by hand would have 3 strokes", and "the
  hand written a looks a lot more like @ but with 2 small legs". The lead's
  reading: an oval opened like the inside of an @; the stem coming down its
  right side; the two "legs" are the small entry and exit hooks at the
  bottom. **Compared with the draft** (`handwriting-model-draft.svg`, the
  points in `strokes-draft.json` → `handwritten.a`): the shape agrees in
  part and the strokes don't.
  - It agrees on the oval drawn anticlockwise from its top right, the
    stem down its right side, and a small exit hook to the right at the
    bottom.
  - It differs in two ways. The draft is **one continuous stroke** (entry,
    oval, stem, exit, without lifting), not three. Its entry is **a long
    rising line** from the baseline at the left, (0, 200), up to the oval's
    top right, (50, 112): not a small leg at the bottom.
  - So `handwriting` should settle, from her manual, whether the a is one
    stroke or three, and what the entry looks like. The draft is unchanged
    (that's `handwriting`'s job, after the human checks the manual).
    Nothing changes for print tracing.

### Design note (a new content type, a new activity kind, a small lesson-engine change)

#### The choices, in short

1. **A letter's shape is content:** one JSON file per letter, case and form,
   `content/strokes/<strokes-id>-<case>-<form>.json` (`a-lower-print.json`),
   read by Core, imported into one asset per file and a new catalog. Adding
   a letter's shape, changing a stroke or adding the handwritten forms is
   data, never code.
2. **One check for every stroke, `StrokeCheck`,** in Core, pure and
   allocation-free: it follows her finger along the stroke in order (so a
   stroke that goes down and back up the same line, like m's, is checked
   right), and says `Good`, `StartedElsewhere`, `WrongDirection`,
   `LeftThePath` or `TooShort`.
3. **`TraceLetter` uses the lesson run's one hint ladder:** every stroke of
   every round is one "part". A failed stroke is a miss: the first gets a
   hint chosen by what went wrong, the second shows the answer (the stroke
   draws itself once), as for every other kind.
4. **The tolerances are numbers in the learning rules** (corridor width,
   start radius, coverage share, for each of the three rounds).
5. **Steps the app can't play yet are skipped by step, not only by kind:**
   a `TraceLetter` step is played when its letter has the shape it asks for.
   So the handwritten steps (and, for now, the English lessons' tracing,
   which is by hand) wait for `handwriting` without any code naming them.

#### The stroke format (`content/strokes/`, UTF-8 JSON)

```json
{
  "status": "draft",
  "source": "design/research/strokes-draft.json (curriculum-research), not yet checked against her manual",
  "slant": 0,
  "strokes": [
    { "runs": [
      { "kind": "body", "points": [[65,112],[45,100],[17.5,108],[7.5,150],[17.5,192],[42.5,200],[62.5,190],[70,160],[70,100]] },
      { "kind": "body", "points": [[70,100],[70,200]] }
    ] }
  ]
}
```

- **The file name is the id:** `<strokes-id>-<case>-<form>`, where
  `strokes-id` is the letter's `strokes` cell in `letters.csv` (it is there
  already), `case` is `lower` or `upper` and `form` is `print` or
  `handwritten`, the values of the `TraceLetter` step's options. A digit
  (numbers, later) has one case: `7-lower-print`.
- **Units: the writing lines are fixed for every shape** (the
  `curriculum-research` convention): the top line y=0, the middle line
  y=100, the baseline y=200, the line below y=300; x from 0 at the letter's
  left. So a small letter fills 100–200, t and the capitals reach 0, p goes
  to 300. Points are upright; **`slant`** (0 for print, 0.2 for the
  handwritten forms) leans them when the shape is loaded
  (x += (200 − y) × slant), so the check and the drawing see the same
  slanted line.
- **Strokes in order, each one finger-down to finger-up;** a stroke's
  **runs** are drawn without lifting, each a smooth curve through its
  points (Catmull–Rom), with a corner between two runs (m's turn at the
  baseline). A run's `kind` is `body`, `entry` (from the line into the
  letter) or `exit` (to the next letter): print uses `body` only;
  `handwriting` uses the three, and a word's joins are the entry run's
  first point and the exit run's last point. A stroke of one point is a dot
  (i's dot). **The start is the first point and the direction is the order
  of the points;** the arrow and the start dot are computed from them.
- **`status`** is `draft` or `checked`. A draft shape is a warning at every
  import (like a picture still on its placeholder), so the human sees which
  shapes are still to check against her manual. `source` is for people.
- **Read by a small JSON reader in Core** (objects, arrays, numbers,
  strings; every error with its file and line): Unity's `JsonUtility` can't
  read arrays of arrays and stays silent about a missing field, and Core
  can't use Unity.

#### Where the logic lives

- **Core, `Scripts/Core/Tracing`** (no Unity):
  - `LetterShape` (id, letter, case, form, status, slant, strokes, its
    bounds) with `Stroke`, `StrokeRun`, `StrokeRunKind`, `LetterCase`,
    `LetterForm`; `StrokePath`: a stroke resampled every few units into
    arrays (x, y, distance along it), made once when the content loads.
  - `StrokeShapeReader`: the JSON to a `LetterShape`, with its problems.
  - `TracePoints`: the reused buffer her finger fills (pre-sized; points
    closer than a unit to the last are dropped, so it never grows).
  - `StrokeCheck.Check(path, points, tolerance)` gives a `StrokeResult`
    (the verdict and how much of the stroke she covered):
    1. a dot stroke: `Good` when she touched near it, else
       `StartedElsewhere`;
    2. her line is shorter than the start radius (a tap): `TooShort`;
    3. **following her in order:** each of her points moves a "reached"
       mark forward along the stroke to the nearest sample within the
       corridor, never back and never further ahead than she moved (so an
       overlapping return, m's or A's, is reached only after the way
       down); she is `Good` when she started within the start radius,
       never left the corridor, and reached the coverage share;
    4. the same followed backwards reaching the share: `WrongDirection`
       (a closed o drawn the wrong way round too);
    5. her first point outside the start radius: `StartedElsewhere`;
    6. a point outside the corridor: `LeftThePath`;
    7. otherwise (started right, stayed on, stopped early): `TooShort`.
  - `StrokeTolerance` (corridor half-width, start radius, coverage share, in
    the shape's units) and `TracingRules` (one per round), in Core's
    `LearningRules`, filled from `LearningRulesAsset`. Defaults to tune on
    the tablet: round 1 corridor 32, start radius 34, coverage 0.8; round 2
    28, 30, 0.85; round 3 24, 28, 0.9 (100 units is the height of a small
    letter).
- **Core, `Scripts/Core/Activities`:** `TraceLetterRules` (one line in
  `ActivityRulesFactory`):
  - **Three rounds,** each all the strokes in order: round 1 the dotted
    guide, the start dot with its number and the arrow; round 2 a faint
    shadow, the start dot and the arrow; round 3 the start dot only.
  - **Judging:** a stroke judged `Good` is `Right` (the stroke stays in
    rainbow ink, the next dot appears), the last stroke of round 3 is
    `Done`; any other verdict is `Wrong`.
  - **The hints** (first miss of a stroke), by verdict, from the lines
    already in `voice-lines.csv`: `StartedElsewhere` gives „Începe de la
    punctul verde.”; `WrongDirection` and `TooShort` give „Urmează
    săgeata.”; `LeftThePath` gives „Rămâi pe linia punctată.” in round 1,
    „Încet și cu grijă: o să reușești!” in rounds 2–3 (no dotted line is
    shown there). The failed stroke fades and its dot pulses (a new
    `HintCue.PulseStart`).
  - **The answer shown** (the second miss): „Uită-te cum se scrie, apoi
    încearcă tu.”, the stroke draws itself once with its guide shown (a new
    `HintCue.AnimateStroke`); she then traces it, as practice.
  - **The checkpoints** (every bubble is spoken): stroke 1 of 2 done gives
    „Foarte bine! Acum mergi la punctul cu numărul doi.” (`trace-next-2`,
    `-3`); a round done: the letter bounces, „Bravo, ai scris litera a!”
    (`trace-done`, with the letter's sound spliced) and the next round's
    line (`trace-round-2`, `trace-round-3`); the last round done: „Bravo,
    ai scris litera a!”, then the run's praise. The instruction is
    `trace-print` + `trace-start` („Scrie litera de tipar. Începe de la
    punct și urmează săgeata.”). **No new voice line.**
  - **Her stroke reaches Core as an answer:** `ActivityAnswer.Traced(points)`
    carries a reference to the reused `TracePoints` (the convention from
    `lesson-engine`: "a kind that needs more than a choice extends
    `ActivityAnswer`"); still a small struct, no allocation.
  - **What the view draws** comes from the round: `ActivityRound` gets an
    optional `Trace` (the shape, the round, the stroke she is at, what guide
    shows), null for the other kinds.
- **Core, `Scripts/Core/Lessons`:** `LessonPlan` and `LessonOrder` take a
  `PlayableSteps` (the kinds with rules and views, as now, **and** for
  `TraceLetter` the letter's shape for the step's case and form being in
  the content) instead of the list of kinds. Nothing else in the run
  changes.
- **Core, `Scripts/Core/Content`:** the `Curriculum` gets `Shapes` (an
  `ItemCatalog<LetterShape>`), built by the same `CurriculumBuilder` from
  the files or from the assets. **Validator rules** (in a new
  `Rules/ShapeRules`): a stroke file that doesn't parse, an unknown key, a
  name not `<id>-<case>-<form>`, a `strokes` id no letter has, a stroke
  with no point, runs that don't meet, points far outside the box; **a
  print `TraceLetter` step whose letter has no shape for its case is an
  error; a handwritten one is a warning** ("skipped until `handwriting`";
  that brief makes it an error); a draft shape is a warning.
- **Content (`Scripts/Content`):** `LetterShapeAsset` and
  `LetterShapeCatalog`; `CurriculumAssets.Build` takes the catalog.
- **Editor (the importer):** reads `content/strokes/*.json` with the CSVs
  (strict UTF-8), one asset per file under `Assets/Scolita/Content/Strokes/`,
  kept by GUID like the others, a removed file with its asset still there
  refused like a deleted row; the freshness test covers them. **Not in the
  ledger of released ids:** her progress names letters, never a shape, so a
  shape can be replaced; the step rule above still catches a shape a lesson
  needs.
- **Game, `Scripts/Game/Tracing`:** `TraceFinger` reads the Enhanced Touch
  API (and the mouse in the Editor): **the first finger down on the board
  draws until it lifts; any other finger is ignored; a touch whose contact
  is larger than a palm threshold (a serialized number on the prefab) is
  ignored.** Its rule for picking the finger is a plain class
  (`FingerPicker`) tested in EditMode. Positions go from the screen to the
  board's units with `RectTransformUtility` (no allocation) into
  `TracePoints`; on lift it sends `ActivityAnswer.Traced`.
- **UI, `Scripts/UI/Tracing`:** `TraceLetterView` (the prefab
  `Prefabs/Lesson/TraceLetterView.prefab`, one row in `ActivityRegistry`,
  `TraceLetter` off its "not built yet" list), built from `look`'s board
  sprites (`Art/Board`): the slate and its lines (top line, dashed middle
  line, baseline, and a faint dashed line below for p), the dotted guide,
  the faint shadow, the start dots with their numbers, the yellow arrow,
  **the rainbow trail** (a uGUI graphic whose mesh is rebuilt into
  pre-sized buffers: purple, blue, green, yellow, orange, pink along her
  line, 26 px at the reference size), **stardust** (a pool of sparkles
  behind her finger), the finished strokes kept in ink, the stroke drawing
  itself for the answer shown, the letter's bounce, the language badge and
  the three round stars on the right.

#### Data needed

- **Planet 1's print shapes,** lower and upper case (D2: lower case first,
  the capitals later in the same planet, in `capitals-1`): `a m t p` and
  `A M T P`, from `strokes-draft.json`'s print drawings, all `draft`.
- No change to `letters.csv` (its `strokes` cells are there), `steps.csv` or
  `voice-lines.csv`.

#### How it is tested

- **EditMode:** `StrokeCheck` on recorded strokes in
  `Tests/EditMode/Tracing/Strokes/` (for each of a m t p: good,
  sloppy-but-fine, backwards, off the path, too short; plus a dot and an
  overlapping return); the three rounds' tolerances (a sloppy stroke good
  in round 1 and not in round 3); `TraceLetterRules` with the real shapes
  (three rounds, the hint for each verdict, the answer shown, the
  checkpoints); the JSON reader; every shape rule with a broken file; the
  importer's GUIDs and freshness; `PlayableSteps` (a handwritten step
  skipped); `FingerPicker`.
- **PlayMode:** a scripted trace (points fed through the view's finger)
  completes the letter's three rounds in the Lesson scene; allocation per
  frame while drawing measured with `ProfilerRecorder` ("GC.Alloc").
- **Emulator** (`emulator-5600`): `adb shell input swipe` chains and
  `motionevent` for a good a, a backwards a, one that wanders off, a tap,
  two fingers; a screenshot of each round at 2000 × 1200; the screen sizes
  of AGENTS.md (the board is the most space-hungry screen).

#### How it serves other domains and briefs

- **`handwriting`:** adds `<id>-lower-handwritten.json` files (entry, body
  and exit runs, `slant` 0.2) and the board's handwritten look; the check is
  the same, and its result says how far along the stroke she reached, so a
  skipped entry stroke („Pornește de pe linie.”) can be told. `TraceWord`
  joins the letters' shapes at their exit and entry points.
- **Numbers:** a digit is a letter (content-model's note) with
  `7-lower-print.json`; `TraceLetter` traces it unchanged.
- **`drawing-pad`:** reuses `TraceFinger` and the rainbow trail; letter
  stencils are the same shapes.

#### Options compared

| Question | Picked | Rejected, and why |
| --- | --- | --- |
| The file format | JSON per shape (the brief), read by Core | One `strokes.csv` row per run: a point list in a cell is hard to read and to diff, and it is not edited in a spreadsheet anyway |
| Reading JSON | A small reader in Core | `JsonUtility` (Unity only, no arrays of arrays, silent on missing fields); a JSON package (not in the stack) |
| Curves | Points joined by a smooth curve, runs meeting at corners | Cubic Béziers: harder to write by hand and from the drafts, and the check needs points anyway |
| The check | Follow her line in order along the stroke | Nearest point anywhere on the letter: wrong for strokes that go back over themselves (m, p, A, M) and can't tell the direction of an o |
| Misses and hints | The run's one ladder, a stroke being a part | The rules counting their own tries: two ladders to keep in step |
| Steps not playable yet | By step: the shape must exist | A "not built yet" list of forms: code to change in `handwriting`, and a step with a shape missing would crash |
| Where the tolerances live | `LearningRules` (one place for the numbers she meets) | On the view's prefab: the rules couldn't be tested without Unity |

#### For the human to agree

1. **The stroke format** above (JSON per letter, case and form; the
   writing lines fixed; points through a smooth curve; runs; `slant`;
   `status`), and that **shapes are content in the content model** with
   their own catalog, but **not in the ledger of released ids**.
2. **The print `a` is one stroke** (round, then up and down the stem
   without lifting), as `curriculum-research` §7 and Little Wandle teach,
   not two as design screen 3 draws it (a "2" dot at the top of the stem).
   `look` would redraw screen 3. Or two strokes, as the screen shows?
3. **Until `handwriting`, only print tracing plays:** the handwritten
   `TraceLetter` steps are skipped, and so **the English lessons have no
   tracing yet** (their only `TraceLetter` is by hand).
4. **Planet 1's capitals A M T P are traced in print too** (`capitals-1`),
   as D2 says, all from the drafts and marked draft.
5. **The board gets a faint fourth line below the baseline now** (p goes
   down to it), the rest as screen 3.
6. **The round labels** („pe puncte”, „pe umbră”, „fără ajutor!”) are
   shown as small pictures with the stars, without words, until the app's
   string tables exist (no screen has them yet); every round is spoken
   anyway.
7. **A lifted finger ends the stroke** and it is judged as it is (a stroke
   left half-way gets „Urmează săgeata.”); finishing a stroke after lifting
   is left for `handwriting`'s joins.

## How to test locally

Run the commands in Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb.exe"` and `S=<your scratch
folder>`. Close your Editor on this project before any batch-mode run. Reset the
app's saves with `"$ADB" -s <emulator> shell pm clear com.softshack.scolita`.

### Core slice (shapes, the stroke check, `TraceLetter`'s rules)

1. **EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: exit 0, all passed, among them:
   - `JsonReaderTests` (every kind of value; nine broken files, each with its line);
   - `LetterShapeReaderTests` (the valid shape; bad names; twelve broken files, each
     with its line and reason; a cedilla);
   - `StrokePathTests`, `TracePointsTests` (and `TestAdd_AllocatesNothing`);
   - `StrokeCheckTests`: each verdict, m's down-and-up, a round drawn the wrong way,
     a dot, the rounds tightening, a short bar, `TestCheck_AllocatesNothing`;
   - `TraceLetterRulesTests`: three rounds of two strokes, the hint for each
     mistake, the answer shown, the lines, the lesson run's ladder;
   - `ShapeRulesTests`, `PlayableStepsTests`, `LearningRulesTests.TestForRound_…`.
   No `warning CS` or `error CS` in the log.

### Content and Build slices (the shape files and their import)

1. **The import:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`.
   Expected: exit 0, `imported, with 37 warnings`: the 17 pictures on their
   placeholder, the 8 shapes still drafts (`strokes/a-lower-print.json: the shape is
   a draft …`), and 12 handwritten tracing steps skipped (`steps.csv, row 4: the
   letter a has no handwritten shape yet (strokes/a-lower-handwritten.json): the step
   is skipped until it has one.`). `git status` shows no change.
2. **`RecordedStrokeTests`** (in the EditMode run): every recorded line of the
   eight letters gets its verdict in each round.
3. **A broken shape stops the import:** in `content/strokes/t-lower-print.json`
   change `"kind": "body"` to `"kind": "stem"` and import. Expected: exit 1,
   `strokes/t-lower-print.json, row 7: a run's "kind" is "body", "entry" or "exit".`,
   and nothing written. Then `git checkout content`.
4. **A print step without its shape is an error:** rename
   `content/strokes/m-upper-print.json` to `m-upper-print.json.off` and import.
   Expected: exit 1 with `steps.csv, row 61: the letter m has no print shape for
   this step: it needs strokes/m-upper-print.json.`, nothing written. Then rename
   it back. (With the step removed too, the import would remove the shape's asset:
   `TestApply_ShapesKeepTheirGuidAndGoWithTheirFile`.)

### Game and UI slices (the writing board)

**Prerequisites:** the debug APK
(`"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`,
APK in `game/Builds/Android/`), your emulator started with
`emulator -avd <yours> -port <port>` (with `-feature -Vulkan`; the app started with `-e unity -force-gles`), and the app installed fresh:
`"$ADB" -s <emulator> install -r game/Builds/Android/Scolita-<version>-debug.apk`,
`"$ADB" -s <emulator> shell pm clear com.softshack.scolita`,
`"$ADB" -s <emulator> shell wm size 2000x1200`. Start it with
`"$ADB" -s <emulator> shell am start -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`.

The strokes below were drawn with `input motionevent` chains (one finger) and raw
`sendevent` multi-touch on `/dev/input/event2` after `adb root` (two fingers, a palm with a
large contact); the points come from the letter's units mapped with the board's layout
(canvas 1280 × 800, match 0.5; board anchors 0.03–0.68 × 0.04–0.69; the four lines over
84% of the board's height). At 2000 × 1200 the print a's start dot is at (776, 606), the
stem's at (788, 576).

1. **Get to the board:** tap through the system's "Viewing full screen" notice, **Hai!**,
   then the letter card and the picture of `ro-a`'s first step. Expected: the board as in
   manual row 0.7.0-1, two step dots, „Scrie litera de tipar. Începe de la punct și urmează
   săgeata.” in the bubble.
2. **A good round 1:** the round from (776, 606) anticlockwise back up the right side, then
   the stem (788, 576) → (788, 828). Expected: the rainbow trail kept; after the round
   „Foarte bine! Acum mergi la punctul cu numărul doi.” (dot 2 and a down arrow); after the
   stem the bounce and „Bravo, ai scris litera a! Acum scrie pe umbră.” (46 characters in
   the log's `Voice: Bubble` line), the faint a, the second star gold.
3. **Round 2, backwards then off:** the round from its end; then the first 40% and a jump
   300 px to the right. Expected: „Urmează săgeata.” (16), the trail fading, dot 1 pulsing;
   then „Uită-te cum se scrie, apoi încearcă tu.” (39) and the round drawing itself in cream
   with its dots.
4. **Round 3:** after a good round and stem: only the start dot. A tap on it: „Urmează
   săgeata.”; a stroke begun at the round's bottom: the answer shown again.
5. **Two fingers:** finger A traces the stem while finger B comes down 300 px to the right
   and moves. Expected: only A's line, the letter done, a praise („Foarte bine, așa
   este!”), the lesson moves on. (The development console shows "Unhandled pen event
   action: 5/6": the emulator's raw touch device also reports a stylus; not the app's.)
6. **A palm:** a touch with a large contact (`ABS_MT_TOUCH_MAJOR` 4000) put down first, then
   a finger tracing a stroke. Expected: the palm draws nothing; the finger's stroke counts.
7. **Screen sizes:** `wm size 1280x800`, `2560x1600`, `2048x1536`, `2400x1080`, then
   `wm size reset`; restart the app between sizes (`am force-stop`). Expected: the board
   left, below the mascot's bubble, the stars and the round pictures on the right, the
   letter centred on the board and fully inside it.

**Automated (PlayMode):** `TracingTests` draws through the finger: three rounds to the
step's end, one finger with a palm and a second finger ignored and a stroke drawn backwards
hinted, and `TestMove_DrawingAllocatesNothing` (the finger moving and the lines' meshes
rebuilt allocate 0 B).

**The recorded strokes** in `Tests/EditMode/Tracing/Strokes/` are generated from the shapes
(each stroke's path sampled every 5–6 units, seeded jitter of 1–3 units, an offset beside
it for the sloppy and wobbly lines, reversed for backwards, a detour of 60 units for off,
35% for too short, begun half-way for begun elsewhere). To regenerate them after a shape
changes, sample the new paths the same way and check each verdict by hand.

**Cleanup:** `"$ADB" -s <emulator> shell pm clear com.softshack.scolita` and
`"$ADB" -s <emulator> shell wm size reset`.

## Completion record

_Finished 2026-11-25 by `tracing`; released as 0.7.0 (tag `v0.7.0`)._

### What was built

- **Pushed:** `35833d0` the claim; `1787760` the Design note; `5f16f1d` the human's
  answers; `70a74a3` `core:`, `5acd8e7` `content:`, `3477c73` `build:`, `769a3c9`
  `content:`, `1dc3026` `game:`, `ac72ba3` `ui:`, `070da45` `game:`; the review's fixes
  `60f8a82` `core:`, `596682d` `build:`, `caf7864` `game:`, `5765d2a` `ui:`, `ee04ea0`
  `game:`; `f3a744c` `docs:`; the close-out.
- **Letter shapes as content:** `content/strokes/<strokes id>-<case>-<form>.json`
  (`content/README.md` → "strokes/"), read by Core's `JsonReader` and
  `LetterShapeReader`, imported to `Content/Strokes/` and `LetterShapeCatalog`;
  `Curriculum.Shapes` and `TryGetShape`; planet 1's eight print shapes (a m t p,
  A M T P), all `draft`.
- **Core** (`Scripts/Core/Tracing`): `LetterShape`, `Stroke`, `StrokeRun`, `StrokePath`
  (Catmull-Rom, resampled every 2 units, slant), `StrokeCheck` (follows her line in
  order; `Good`, `StartedElsewhere`, `WrongDirection`, `LeftThePath`, `TooShort`),
  `TracePoints`, `StrokeTolerance`, `TracingTuning`, `TraceState`, `WritingLines`,
  `LetterShapeCodes`; `Activities/TraceLetterRules` (three rounds, the run's one ladder);
  `Lessons/PlayableSteps`; `Content/Rules/ShapeRules`; `ActivityLimits.MostStrokes`;
  `ActivityAnswer.Traced`, `AnswerGesture.Trace`, `HintCue.PulseStart`,
  `HintCue.AnimateStroke`, `ActivityRound.Trace`; the tracing lines in `ActivityKinds`.
- **Unity side:** `Game/Tracing/TraceFinger` (Enhanced Touch, every sample once, one
  finger, palms dropped) and `FingerPicker`; `UI/Tracing/TraceLetterView`,
  `StrokeLineGraphic`, `Stardust`, `StartDot`; `Prefabs/Lesson/TraceLetterView.prefab`;
  the `TraceLetter` row in `ActivityRegistry.asset` (off "not built yet"); the shape
  catalog on the Lesson scene's runner; the tolerances in `Settings/LearningRules.asset`.
- **Tests:** EditMode 486 (was 413): `JsonReaderTests`, `LetterShapeReaderTests`,
  `StrokePathTests`, `TracePointsTests`, `StrokeCheckTests`, `RecordedStrokeTests` (with
  `Tests/EditMode/Tracing/Strokes/`), `TraceLetterRulesTests`, `ShapeRulesTests`,
  `PlayableStepsTests`, `FingerPickerTests`, importer tests. PlayMode 17 (was 14):
  `TracingTests`.

### Conventions for later briefs

- **A letter's shape is a file, not code:** add `<id>-<case>-<form>.json` and import; the
  board, the check and the rounds follow. A shape is replaced, never retired (not in the
  ledger); its asset goes with its file.
- **A step plays when its content is there** (`PlayableSteps`,
  `ActivityRulesFactory.CanCreate`): a later kind that needs content which may be missing
  adds its test there, not a list of "not built yet" forms.
- **Strokes are judged in Core** (`StrokeCheck`): views send `ActivityAnswer.Traced` with a
  reused `TracePoints` on finger-up, and draw `ActivityRound.Trace`.
- **The tolerances** (corridor, start radius, coverage per round) are in the learning
  rules asset, in a shape's units (a small letter is 100 high); a later round is never
  more forgiving.
- **Lines on a board** are `StrokeLineGraphic`s (one mesh, pre-sized buffers: size their
  `capacity` for the most samples they can hold).

### How it was tested

Batch mode at the pushed commits (EditMode 486/486, PlayMode 17/17, `BuildAndroidDebug`, no
compiler warning), the import, the emulator at 2000 × 1200 with drawn strokes (good,
backwards, off, half, a tap, begun elsewhere, two fingers), five screen sizes, the 0 B
allocation test, and a fresh-eyes review (Progress, 16 findings resolved or recorded).
Școlița → Record released ids at this close-out (`VERSION` 0.7.0): nothing was added (no new
id; shapes are not in the ledger). Not
run: the Device Simulator, the device Profiler, listening by ear, a palm on a real
screen (the emulator reports no contact size).

### Decisions and deviations

- **The human's answers** (2026-11-24): the note as recommended, and the print a is two
  strokes.
- **While building and from the review** (Progress): coverage is what she drew herself, with
  an end allowance for short strokes; leaving the path is judged near where she is;
  `WrongDirection` needs her to begin at the stroke's end; a shape's asset goes with its
  file (the note said "refused"); the board ends below the mascot's bubble; round 3's
  prompt is the green dot's line; the recorded strokes are generated from the shapes.

### Follow-ups

In Progress → "Follow-ups": `handwriting` (handwritten shapes, the four-line board, the
entry hint, `TraceWord`, a `rounds` option for the English steps, a dot's length, the
handwritten a); the human (tolerances on the Tab A7, the eight drafts, the answer-shown
and tap questions, a palm on the tablet, the Profiler); real recorded strokes; `look`.
