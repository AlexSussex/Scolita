# Task: feature/tracing

> **Status: In progress (tracing).** Ready after `lesson-engine` (decisions D2: lower case first;
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
  **Waiting for the human to agree it** (the questions at its end) before
  any code is written.

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
