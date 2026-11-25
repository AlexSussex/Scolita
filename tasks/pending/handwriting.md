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

## Progress

_Kept by `handwriting` while the brief is in flight._

### Status

- **Claimed** in `51cabec`.
- **Design note** (below): a **medium change**. The stroke file format is unchanged (tracing's
  eight print files keep working as they are); the new meanings are read from what the files
  already hold (the entry and exit runs, where strokes begin and end). The content model gains
  one optional step option (`rounds`); the lesson engine is unchanged.
- **Next:** Core with its tests, then the handwritten shapes, then the board, `TraceWord`'s
  view and `MeetLetter`'s "de mână" button.

### Design note

#### The choices, in short

1. **No new field in the stroke files.** A handwritten shape is a file like the print ones
   (`a-lower-handwritten.json`, `slant` 0.2) using the run kinds tracing already reads
   (`entry`, `body`, `exit`). Three rules are read from the points, so nobody writes them twice:
   - **a stroke continues the one before** when it begins exactly where that one ended (within
     half a unit): she may then write both in one line, or lift between them;
   - **the join points** are the entry run's first point (or the first point when there is no
     entry: m, n) and the exit run's last point;
   - **the marks** are the strokes after the stroke holding the exit (i's dot, t's bar): in a
     word they are written after the whole word, as the manuals show ("tati").
2. **What she traces is a `TraceFigure`** (Core): its strokes in order, each knowing its letter,
   its number within the letter, whether it continues the one before and whether it ends its
   letter's line. A figure is made from one letter's shape (`TraceLetter`, any form) or from a
   word's handwritten letters joined (`TraceWord`). The board, the check and the rules see only
   the figure, so one board view draws a print letter, a handwritten letter and a joined word.
3. **A line through several strokes is judged as those strokes in order** (`LineCheck`): her
   line is followed along the chain of strokes that continue each other; every stroke she
   covered enough of is done at once; a line lifted part-way into the next stroke keeps what was
   done, and she goes on from the next start dot (no miss).
4. **A skipped entry is its own verdict,** `StrokeVerdict.SkippedEntry`: she began where the
   entry run ends (the letter's body) instead of on the line. Its hint is „Pornește de pe
   linie.” (never for m and n: they have no entry run).
5. **`TraceWord` is a kind like the others:** Core rules (`TraceWordRules`), a view prefab, a
   registry row, its lines, the validator's checks. A word plays when every letter has its
   handwritten lower-case shape (`PlayableSteps` via `ActivityRulesFactory.CanCreate`, as
   tracing set up).

#### The handwritten a: three strokes (the human, 2026-11-24)

The human: the handwritten a "looks a lot more like @ but with 2 small legs" and has three
strokes. Read with `curriculum-research` §8 (the entry rises "to the top right of the oval,
which is then drawn anticlockwise back over it"):

1. **the entry, the left leg:** from the baseline at the left, up the oval's left side and over
   its top to the top right (only its foot shows below the oval: the first small leg);
2. **the round body:** from the top right, anticlockwise, back over the entry's path, round the
   bottom and up the right side (the @'s inner a);
3. **the stem with the exit, the right leg:** down the right side, then the small hook to the
   right on the baseline (the second small leg), where the next letter joins.

Stroke 2 begins where stroke 1 ends and stroke 3 where stroke 2 ends, so **each has its numbered
start dot** (she may lift, as the human counts three), and **one line through all three is
accepted** (as a word needs: "fără să ridici degetul"). The other planet 1 letters follow the
draft (m one stroke with its hook and exit; t its line, then the bar; p its line; the capitals
as drawn). All are `draft`, for the human to check against her school's manual.

#### Entry and exit strokes, and joins in a word

- **Entry:** a run of kind `entry`, the first run of the first stroke, starting on the baseline
  (a d o i t p s e). m and n have none: they start at the middle line with their hook.
- **Exit:** a run of kind `exit`, the last run of the letter's line, ending between the middle
  line and the baseline (o leaves at the middle line, the others low, rising to the right).
- **Joining a word** (`JoinedWord.Of`, Core, made once when the lesson is built): the first
  letter is placed as it is; each next letter is moved right so its entry's first point lands
  on the previous exit's last point (that point becomes the join: the entry's first point is
  moved onto it, so the join is part of the next letter, as `joined-words-draft.svg` draws it).
  A letter without an entry (m, n), or a join that leaves high (after o), gets a join run from
  the exit's end to its first body point, a small gap to the right. Slant is applied after the
  letters are placed, so the word leans as one.
- **Order of a word's strokes:** each letter's line, letter by letter, then every mark in
  letter order (tata: the whole word, then the two bars).
- **A lifted finger at a join is forgiven:** a stroke that begins at a join may start within
  the **join radius** (a new number in the learning rules, wider than the start radius) of it;
  a stroke lifted just before the exit's end is already covered by tracing's end allowance.

#### How handwritten letters are drawn from the stroke data

- **`HandwritingView`** (UI): draws a `TraceFigure` fitted into its rectangle, from each
  stroke's sampled path (the same samples `StrokeCheck` follows), as an even pen line in ink
  `#2A2140`; it can also write the figure itself at a pen's pace (the `MeetLetter` card's
  "de mână" button). No cursive font anywhere (D3). A PlayMode test compares its line's points
  with the paths.
- **The board** (tracing's `TraceLetterView`, renamed `WritingBoardView`, its `.meta` kept):
  draws the figure's guide, dots, arrow and her trail as now; for a handwritten figure the
  "tip I" lines (dashed top, solid middle, solid baseline, dashed below) and faint slant
  guides; the letters done stay in ink; the start dot's number is the stroke's number within
  its letter. `TraceWord`'s prefab is the same board with the right panel of screen 8 (the word
  in print, "tipar / de mână").

#### `TraceLetter` by hand and `TraceWord`: rounds, lines, hints

- **Rounds:** three as print (dotted, faint, start dot only); a new optional step option
  **`rounds`** (1 to 3) plays the first N; the English handwritten steps get `rounds=1`
  (curriculum §4.1: "one round by hand"). `TracingTuning.Rounds` stays the most.
- **`TraceLetter` by hand:** „Scrie litera de mână.” (+ „Linia de jos: de aici pornești.” when
  the letter starts on the baseline) + „Începe de la punct și urmează săgeata.”; per stroke the
  print checkpoints; per letter „Bravo, ai scris litera {x} de mână!”.
- **`TraceWord`:** „Scrie cuvântul de mână, fără să ridici degetul.” and the word said slowly
  (its `slow` line). Each letter's line done: **„Bravo, ai făcut litera {x}! Acum leag-o de
  litera {y}, fără să ridici degetul.”** (`word-join`), said at once, the bubble with the same
  words; then the marks („Acum trage liniuța literei t.”, „Acum pune punctul pe i.”); the word
  done: „Bravo, ai scris cuvântul {word}!” (with the next round's line between rounds). Every
  line is built when the lesson is built.
- **The voice is not cut off by her writing on:** the checkpoint is the step's `Response`; a
  later stroke done within a letter says nothing new in `TraceWord`, so the letter's line plays
  to its end; tapping the mascot repeats it (the `Prompt`).
- **Hints:** a skipped entry „Pornește de pe linie.” (`trace-hint-from-line`,
  `word-hint-from-line`); begun elsewhere „Începe de la punctul verde.”; backwards or too short
  „Urmează săgeata.”; off the path „Rămâi pe linia punctată.” / „Încet și cu grijă: o să
  reușești!”; in a word, a line lifted inside a join „Nu ridica degetul la legătură.” and one
  lifted inside a letter „Nu-i nimic! Pune degetul înapoi pe punct și continuă.” (both said
  without a miss); the answer shown: the next letter writes itself once, slowly („Uite cum se
  leagă literele.”). All are in `voice-lines.csv` already, in both languages.
- **{x} and {y} are the letters' sounds,** as tracing's „Bravo, ai scris litera a!” and every
  line of planets 1–3 (the curriculum: names come later). The brief says "name clips"; that
  conflicts with the agreed curriculum, so the sound is used and the human is asked (one
  switch in `TraceWordRules`).

#### What changes in Core, and how tracing's files keep working

- `StrokePath` gains `EntryLength` (how far along the entry run ends; 0 without one) and a way
  to join paths into a chain; `StrokeCheck` gains `SkippedEntry`; `TraceState` holds a
  `TraceFigure` and the step's rounds, and moves on by several strokes at once. The print
  shapes have no entry and no strokes that continue each other, so **print tracing judges
  exactly as before** (its tests and recorded strokes run unchanged).
- `ShapeRules`: a missing handwritten shape for a step is now an **error** (tracing made it a
  warning); for a handwritten lower-case shape: an entry only as the first run and starting on
  the baseline, at most one exit and it ends the letter's line between the middle line and the
  baseline, the strokes up to the exit continue each other; a `TraceWord` word has at most 5
  letters (`ActivityLimits.MostTracedLetters`), each with a handwritten lower-case shape, all
  but the last with an exit.
- `MeetLetterRules` gets a third choice, the "de mână" button, when the letter has its
  handwritten lower-case shape: a tap is neutral and says „Uite cum se scrie litera de mână.”
  while the card writes the letter.

#### How it is tested

- **EditMode:** `TraceFigure`, `JoinedWord` (joins land on the exits, marks last, slant after
  placing), `LineCheck` (one line through a's three strokes, a lift part-way, a skipped entry,
  backwards), `TraceLetterRules` by hand (rounds option, lines, hints), `TraceWordRules` with
  the real planet 1 words (mama, tata, map, mat: every checkpoint line, the marks, rounds),
  `ShapeRules` with broken handwritten files, the factory and `PlayableSteps`; recorded strokes
  in `Tests/EditMode/Tracing/Strokes/handwritten/` (good, sloppy, backwards, no entry, a lifted
  join) for a m t p and for mama and tata.
- **PlayMode:** a scripted handwritten trace completes a letter and a word; `HandwritingView`'s
  points match the paths; drawing allocates nothing.
- **Emulator:** strokes drawn with `input motionevent` on a handwritten a (good, backwards,
  without its entry, off, two fingers) and on mama (letter by letter, a lifted join);
  screenshots of screen 8 at 2000 × 1200 and 1280 × 800 and the other sizes.

#### How it serves other domains

- **Digits** (numbers, later): `7-lower-print.json` and `7-lower-handwritten.json`, no entry
  or exit (digits aren't joined): `TraceLetter` traces them unchanged; a digit drawn with or
  without a lift (4) is the "continues" rule. A number is not joined like a word.
- **Music, drawing:** `HandwritingView` draws any figure (a note's stem and head, a stencil).

#### For the human (not blocking; asked in the next steps)

1. The handwritten a's three strokes as read above, and all eight handwritten shapes (a m t p,
   A M T P), are drafts: check them against her manual.
2. {x} and {y} in „Bravo, ai făcut litera {x}! …” said as the letters' sounds (as tracing), not
   their names (the brief's words), since names come later in the curriculum.
