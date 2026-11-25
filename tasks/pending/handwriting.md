# Task: feature/handwriting

> **Status: In progress (handwriting).** (The human, 2026-11-24: she must also
> learn how letters are made by hand, not only typed letters; decision D3
> answered: handwritten letters are in the MVP.) The handwriting model is
> the Romanian school's "litere de mână" for both languages (assumed in
> D3; `curriculum-research` confirms the shapes).

Read `AGENTS.md` first (especially "Learning" → letter shapes, "Testing
before a push" → tracing, "Performance"), then `design/README.md` and
`design/screens.md` (build the screens as designed there: screen 8), and
the completion records in `tasks/done/`, above all `tracing`'s.

**Feature:** `feature/handwriting`
**Implements:** PRD §3.3 (handwritten letters, joined words);
`docs/curriculum.md` §4.1, §4.2 (`TraceLetter` in its handwritten form,
`TraceWord`), §7.
**Depends on:** `tracing`; the handwriting model and stroke order from
`curriculum-research`.

A **medium change** to the stroke format: Design note first (the entry and
exit strokes, how letters join, how handwritten forms are drawn from the
stroke data, how digits for the numbers domain would use it).

## Scope

1. **Handwritten forms in the stroke data:** each letter gets a
   `handwritten` form for each case, next to its `print` form, in
   `content/strokes/<letter-id>-<case>-<form>.json`: its strokes in the
   order a Romanian teacher shows them, with the **entry stroke** (where
   the pen comes in from the baseline) and the **exit stroke** (where it
   leaves to join the next letter), and the join points on the writing
   lines. Importer and validator support: every letter in a lesson with a
   handwritten step has the form for the case asked; a form's entry and
   exit sit on the lines the model says.
2. **Drawing handwritten letters from their strokes:** a `HandwritingView`
   that draws any letter or word in its handwritten form from the stroke
   data (a smooth, even pen line, ink `#2A2140`), so no cursive font is
   needed and what she sees is exactly what she traces. Used on the
   tracing board and wherever a handwritten form is shown (the
   `MeetLetter` card's "de mână" button animates the letter being written).
3. **`TraceLetter` with the handwritten form:** the same three rounds as
   print (dotted guide, faint guide, start dot only), the entry stroke
   first, then the letter, then the exit; `StrokeCheck` (from `tracing`)
   checks each stroke, and a letter written in one continuous line is
   accepted as its strokes in order when the model writes it without
   lifting the pen.
4. **`TraceWord`** (a new activity kind: Core rules, view prefab,
   registry row, validator checks, voice lines): a short word she has
   learned to read (mama, tata, pat …), shown handwritten and joined, said
   slowly; she writes it letter by letter, each join flowing into the next
   letter's entry; the letters she has done stay in ink, the next one shows
   its start dot. Rounds as `TraceLetter`; a word up to 5 letters in the
   MVP.
   - **Checkpoint voice** (the human, 2026-11-24): when she finishes a
     letter, the mascot **says** "Bravo, ai făcut litera m! Acum leag-o de
     litera a, fără să ridici degetul." at once, and the bubble shows the
     same words (`design/screens.md` → 8); at the word's end, "Bravo, ai
     scris cuvântul mama!". The line plays to its end while she goes on
     writing; tapping the mascot repeats it. `TraceLetter` by hand gets the
     same checkpoints per stroke and per letter.
5. **Lessons:** the lesson shape in `docs/curriculum.md` §4.1 gains the
   handwritten step (in each letter's Romanian lesson, after the print
   one) and `TraceWord` (in each planet's word lessons); the sample data
   for planet 1 gets them.

## Out of scope — do not build

- Free handwriting without a guide (recognising any letter she writes),
  writing sentences, capital letters joined into words (the MVP joins
  lower case only; a capital starts a word only when the curriculum asks).

## Acceptance criteria

- On the emulator with drawn strokes: a good handwritten "a" with its
  entry and exit passes all three rounds; starting at the wrong end is
  `WrongDirection` with a kind hint; skipping the entry stroke is caught
  with the hint "Pornește de pe linie" / "Start from the line".
- At each letter of "mama", the checkpoint line is heard at once, in the
  words above, and the bubble shows the same words; she can write on
  while it plays, and it is not cut off.
- "mama" written letter by letter joins at each letter's exit and entry;
  a join that lifts off and lands near the next entry is accepted
  (tolerance in the learning rules).
- The handwritten forms drawn by `HandwritingView` match the stroke data
  exactly (a test renders the strokes and compares the points).
- 0 B allocated per frame while she writes; 60 fps in the Profiler.

## Tests required

- EditMode: the handwritten stroke files (validator), entry/exit/join
  rules, `TraceWord` rules with real planet 1–3 words, recorded strokes in
  `Tests/EditMode/Tracing/Strokes/handwritten/` (good, sloppy-but-fine,
  backwards, no entry, lifted join) for each planet 1 letter and two
  words.
- PlayMode: a scripted handwritten trace completes a letter and a word.
- Emulator: `adb shell input` swipe chains for each case above;
  screenshots of screen 8 at 2000 × 1200 and 1280 × 800.

## Touches

`Scripts/Core/Tracing` (handwritten forms, joins), `Scripts/Core/Activities/TraceWord`,
`Scripts/Game/Tracing` (`HandwritingView`), `content/strokes/`, the importer
(the form field), the `TraceWord` prefab (new), `ActivityRegistry` (one
row), the `MeetLetter` view (the "de mână" button), `voice-lines.csv`.

## Screens

The handwriting board (screen 8 in `design/screens.md`), the `MeetLetter`
card's handwritten form; all sizes in `AGENTS.md`'s table.

## Voice lines

Complete, correct sentences only (`AGENTS.md` → "Correct, complete
language"), both languages, each heard before the push:

- "Scrie litera de mână." / "Write the letter by hand."
- "Linia de jos: de aici pornești." / "The bottom line: you start from
  here."
- "Pornește de pe linie." / "Start from the line."
- The letter checkpoint: "Bravo, ai făcut litera {x}! Acum leag-o de
  litera {y}, fără să ridici degetul." / "Well done, you made the letter
  {x}! Now join it to the letter {y} without lifting your finger." — the
  letters spliced in as their name clips; list every {x}–{y} pair of
  planets 1–3's words in Progress and hear each one.
- The word checkpoint: "Bravo, ai scris cuvântul {word}!" / "Well done,
  you wrote the word {word}!"
- The hints, at least 4 per language.

## Before closing out

- Release (the next MINOR). Ask the human to let her try writing "mama"
  by hand on the Tab A7 and note whether the joins feel natural.
