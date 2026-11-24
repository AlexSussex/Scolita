# Task: feature/tracing

> **Status: Ready after `lesson-engine`** (decisions D2: lower case first;
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
