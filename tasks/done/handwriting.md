# Task: feature/handwriting

> **Status: Done 2026-11-25, released as 0.8.0.** (The human, 2026-11-24: she must also
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
  one optional step option (`rounds`); the lesson engine is unchanged. Pushed in `42dce67`.
- **Built** (Core, the eight handwritten shapes, the board, `TraceWord`, the "de mână" tile), reviewed by a
  second agent (below) and tested together; pushed as `core:`, `content:`, `ui:` and `game:` commits (hashes in
  the completion record).
- **The test APK** for the human: `local/apks/Scolita-0.7.0-handwriting-try-debug.apk` (step 0 of
  `local/for-human-next-steps.md`).
- **Closed out** as release 0.8.0 (see "Completion record").

### Decisions while building (within the note)

- **The handwritten a** (`a-lower-handwritten.json`): stroke 1 the entry, from the baseline up the
  round's left side and over its top to the top right (only its foot shows: the left leg); stroke 2
  the round, back over it, anticlockwise; stroke 3 the stem down the right side with the exit hook
  (the right leg). The other seven handwritten shapes (m t p, A M T P) are the curriculum draft's
  points as drawn; all eight are `draft`.
- **A line begun near a join counts from the join** (the emulator found it): she lifted at the join
  and landed a little ahead along the next letter's entry; coverage is then measured from the join,
  not from where she landed (`LineCheck`; test `TestJudge_ALiftedJoin`).
- **A line lifted part-way into the next stroke** keeps the strokes it did and only its points along
  them stay in ink (`TraceState.LastPointsKept`); the next start dot shows where to go on. In a word
  it says „Nu-i nimic! Pune degetul înapoi pe punct și continuă.” (no miss), unless it finished a letter:
  then the letter's checkpoint is said. A line that stops part-way along a join (nothing done) is a miss
  hinted „Nu ridica degetul la legătură.”.
- **The board's next dot** is shown only for a stroke that doesn't continue the current one (a
  continued stroke starts where the current one ends, under her finger).
- **`MeetLetter`'s "de mână" tile** shows the letter by hand on a small card between the letter and
  the picture (no word on it: the app has no string tables yet, as tracing's round pictures); a tap
  writes it again before her eyes and says „Uite cum se scrie litera de mână.”. It carries the
  language badge like the other cards.
- **`TraceWord`'s view** is the writing board's prefab (`TraceWordView.prefab`, a copy of
  `TraceLetterView.prefab`) with the word in print (Andika) above the round stars; the "tipar / de
  mână" pills of screen 8 wait for the string tables (a follow-up for `look`).
- **{x} and {y} are the letters' sounds** (the note's point; for the human).
- **Planet 1's join pairs said by `word-join`** (`TraceWordRulesTests.TestLines_PlanetOnesWords`):
  ro mama m–a, a–m, m–a; ro tata t–a, a–t, t–a (then „Acum trage liniuța literei t.” twice); en
  map m–a, a–p; en mat m–a, a–t (then "Now draw the line across the t."). Planets 2–3's pairs are
  listed in `design/research/curriculum-research.md` §6 ("Join pairs"); `content-letters-1` adds
  their words.
- **The board and `HandwritingView` share one fit** (`UI/Tracing/FigureFit`: the writing lines over a share of
  the rectangle's height, the figure centred). The board draws its guide, dots and ink with its own line graphics,
  as tracing built it; `HandwritingView` is the plain pen line for anything else (the "de mână" tile now).
- **Capitals by hand don't join in the MVP:** P's and T's left hooks are `body` runs (they join nothing), so
  their second parts aren't taken for marks; A's and M's exits stay (they will join, `handwriting-2`).
- **Her point buffer holds 8192 points** (`TracePoints.Capacity`; was 2048), and the board's trail and ink 8192, so
  a five-letter word written slowly in one line is never cut short.

### Fresh-eyes review (2026-11-25)

A second agent, which didn't write the code, read the diff against AGENTS.md. It found no blocker: print
tracing judges exactly as before (no print stroke continues another, so `LineCheck` goes straight to
`StrokeCheck`), the joins, marks and slant are right, every line said is in both languages, no `var`, no
engine reference in Core, no mutable static. Its findings, and what was done:

1. **`TraceWordView`'s round pictures and the drawing held 1024 points** (a word is more): now 4096 on both
   boards (and the trail and ink 8192).
2. **A line that left the a after its first stroke was Good, with no word said:** the stroke done is kept, and
   `TraceLetterRules` now says „Rămâi pe linia punctată.” (round 1) or „Încet și cu grijă: o să reușești!” as its
   response, without a miss (`TestJudge_ALineThatStraysAfterAStroke`); the recorded lines now have "off during its
   first stroke" (`LeftThePath`) and, for the a, "off after its first stroke" (the entry kept).
3. **The fitting was written twice** (`HandwritingView` and the board): now `FigureFit`, used by both; the
   Design note's "the board uses `HandwritingView`" is corrected above.
4. **Handwritten capitals had no run checks:** the entry-first and one-exit-last rules now apply to every
   handwritten shape (the baseline, middle-line and continuity rules to small letters); P's and T's hooks are
   `body` runs.
5. **A whole word in one line could fill her point buffer:** 8192 points now.
6. **Nits:** all handwritten small letters must lean alike (a validator rule, and `JoinedWord` refuses mixed
   slants); the continuity error now reads "stroke 3 doesn't begin where stroke 2 ends"; a double blank line;
   tata's two „Acum trage liniuța literei t.” share one prepared line; the narrowest width is
   `FigureFit.NarrowestWidth`. **Kept:** a mark's line is `trace-dot-<id>` or `trace-bar-<id>` (later marks, ă's
   breve or ș's comma, will need their own key: a follow-up); t's bar in `TraceLetter` by hand says „Foarte bine!
   Acum mergi la punctul cu numărul doi.” as print t does.
### Test results (2026-11-25, before the push)

- **Batch mode:** EditMode **511/511** (was 486), PlayMode **20/20** (was 17), `BuildAndroidDebug` exit 0; no
  `warning CS` or `error CS` in any log. The import: exit 0, "imported, with 33 warnings" (17 placeholder
  pictures, 16 draft shapes), no step skipped; `git status` clean after.
- **The emulator** (`emulator-5600`, Pixel Tablet, Android 16 image, `wm size 2000x1200`, `-force-gles`), lines
  drawn with `input motionevent` chains and raw `sendevent` two-finger touches, the bubble's words checked by
  their lengths in the log (all in "How to test locally"):
  - `MeetLetter`: the "de mână" tile with the a by hand, drawn from its strokes;
  - the a by hand: one line through its three strokes (round 1 done), without the entry („Pornește de pe
    linie.”), backwards (the answer shown), off during a stroke (a miss) and after the entry (the entry kept,
    „Rămâi pe linia punctată.”), two fingers (only the first counted);
  - mama: each letter's checkpoint at once (m–a, a–m), a lifted join landing near the join (taken, nothing new
    said), a line lifted inside a join (the checkpoint) and inside a letter („Nu-i nimic! …”), "ma" in one line,
    three rounds to the end;
  - tata: without the entry, the whole word in one line, then both bars („Acum trage liniuța literei t.”), the
    round done.
- **Screen sizes** (screenshots in the scratch folder): mama's board at 2000 × 1200, 1280 × 800, 2560 × 1600,
  2048 × 1536 and 2400 × 1080: the word whole on the board under the bubble, the word in print and the stars
  on the right. At 2400 × 1080 (a phone) the bubble covers the board's RO badge while it shows (the lesson
  screen's bubble; a follow-up for `look`). The cut-out profile was not run (the board sits in the lesson
  screen's safe area, as tracing's).
- **Allocation:** `TestMove_DrawingAllocatesNothing` (PlayMode, the board's drawing path, 0 B) still passes with
  the larger buffers; `LineCheck` and `StrokeCheck` allocate nothing (arrays only, no `new` on the judging
  path). **Not measured:** the whole frame in the device Profiler (no Editor window in this session).
- **Not run:** the Device Simulator (batch mode only; `wm size` stood in); listening by ear (every line said
  was already in `voice-lines.csv`; the lines were checked by their lengths in the log; the human hears them,
  manual rows); the Profiler on a real tablet.

### How the screens meet "Usability" and "Learning"

- **Spoken:** every bubble is a voice line: the instruction, each letter's checkpoint at once (it plays to its
  end while she writes on, as strokes within a letter say nothing new), each hint, the answer shown; tapping
  the mascot repeats the last line said.
- **Big targets:** the board as tracing's (65% of the screen); the word is fitted to the board, so a five-letter
  word's small letters are about 1.6 times smaller than a single letter's; the "de mână" tile is 170 × 240 at
  the reference size.
- **Kind feedback:** a lifted join is forgiven; a line that strays after a stroke keeps the stroke and is
  reminded to stay on the line; a skipped entry is told „Pornește de pe linie.”, never for m (no entry).
- **The language:** the RO or EN star on the board and on the "de mână" tile; all lines in the step's language.
- **Learning:** the handwritten form after the print one, in the order of the stroke data; the word's dots and
  bars after the word; three rounds in Romanian, one in English (curriculum §4.1).

### Follow-ups

- **For the human:**
  - the eight handwritten shapes are drafts, the a's three strokes as read in the Design note: check them
    against her manual, above all the a, the t (its tall entry) and the capitals;
  - {x} and {y} in „Bravo, ai făcut litera {x}! …” are the letters' sounds, not their names (the brief said
    names; the curriculum says names come later);
  - the tolerances: the join radius (44 units) beside tracing's; try writing "mama" by hand on the Tab A7.
- **`content-letters-1`** (planets 2–3): write s i n o d e by hand as data (`content/README.md` → "strokes/");
  i's dot needs no new line (`trace-dot-i` exists), o's high exit is joined by `JoinedWord`; check each new
  shape passes the import's join rules; the planets' TraceWord words up to 5 letters; regenerate the recorded
  lines for the new letters the way `Strokes/handwritten/` was made.
- **Later marks:** ă's breve, ș's and ț's commas, â's and î's hats would be called "bars": a mark's line key
  then needs the kind of mark (a follow-up for planet 7).
- **`look`:** the "tipar / de mână" pills of screen 8 and words on the "de mână" tile wait for the string
  tables; the lesson bubble breaks „leag-o” at its hyphen; at phone size the bubble covers the board's badge.
- **A dot's length** (tracing's follow-up): still `Good` for any line begun on it; with i (planet 2) check
  that a long line from the dot is refused.
- **Capitals by hand** join in `handwriting-2` (A, M, N with their curls).

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

## How to test locally

Run the commands in Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb.exe"` and `S=<your scratch folder>`. Close your Editor on
this project before any batch-mode run. Reset the app's saves with
`"$ADB" -s <emulator> shell pm clear com.softshack.scolita`.

### Core slice (figures, joins, the line check, `TraceWord`'s rules)

1. **EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: exit 0, all passed, among them:
   - `TraceFigureTests`: the handwritten a's three strokes that continue each other; t's bar a mark; mama
     joined (8 strokes, one line); tata's bars last; `LineCheck` through the a in one line, lifted part-way,
     without its entry, backwards; print judged stroke by stroke;
   - `HandwrittenStrokeTests`: the recorded lines in `Tests/EditMode/Tracing/Strokes/handwritten/` (a m t p,
     mama, tata) get their verdicts and strokes done in each round;
   - `TraceWordRulesTests`: mama letter by letter (each „Bravo, ai făcut litera {x}! Acum leag-o de litera
     {y}…” with the sounds m–a, a–m), mama in one line, tata's bars, a lifted join, the hints, every line of the
     four planet 1 words, `TraceLetter` by hand (its instruction, one line, no entry, `rounds=1`), a stray after
     a stroke, `PlayableSteps`;
   - `ShapeRulesTests` (entries, exits, lines, a word's letters), `MeetLetterRulesTests.TestJudge_TheByHandButton`,
     `ActivityRegistryTests`, `LearningRulesTests` (the join radius).
   No `warning CS` or `error CS` in the log.

### Content slice (the handwritten shapes)

1. **The import:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`.
   Expected: exit 0, `imported, with 33 warnings` (17 placeholder pictures, 16 draft shapes: the eight print and
   the eight handwritten ones; no step is skipped any more). `git status` shows no change.
2. **A handwritten step without its shape is an error now:** rename
   `content/strokes/m-lower-handwritten.json` to `.off` and import. Expected: exit 1, `steps.csv, row 12: the
   letter m has no handwritten shape for this step: it needs strokes/m-lower-handwritten.json.` and the
   `TraceWord` rows naming m; nothing written. Rename it back.
3. **An entry off the line:** in `content/strokes/a-lower-handwritten.json` change the first point `[0, 200]` to
   `[0, 150]` and import. Expected: exit 1, `the entry starts on the baseline (y 200), not at y 150.` Then
   `git checkout content`.

### Game and UI slices (the handwriting board, `TraceWord`, the "de mână" tile)

**Prerequisites:** the debug APK
(`"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`,
then `git checkout game/ProjectSettings/ProjectSettings.asset`, the build writes the version there), your
emulator (`emulator -avd <yours> -port <port> -feature -Vulkan`), the APK installed
(`"$ADB" -s <emulator> install -r game/Builds/Android/Scolita-<version>-debug.apk`), `wm size 2000x1200`.

**Getting to a step without playing the lessons before it:** start the app once (so it makes its folders),
`"$ADB" -s <emulator> root`, then write the lesson in progress as the app's user and start it:
`echo '{"version": 1, "lessonId": "ro-words-1", "stepNumber": 7, "stepsSignature": "329476ad8bfb4922"}' > /data/media/0/Android/data/com.softshack.scolita/files/profiles/child-1/lesson-run.json`
(in `adb shell`, then `chown` it to the folder's owner, `ls -ln` shows it). The signatures are
`LessonSignature.Of` of the lesson: `ro-a` `eb9fff3c835bde4a` (step 3 is the a by hand), `en-a`
`2e2a3f7c4e612716` (step 3, one round), `ro-words-1` `329476ad8bfb4922` (steps 7 mama, 8 tata), `en-words-1`
`54681349ebccdf8c` (6 map, 7 mat); they change when the lesson's steps change. Tap through the system's
"Viewing full screen" notice and **Hai!**.

The lines below were drawn with `input motionevent` chains (one finger) and raw `sendevent` multi-touch after
`adb root` (two fingers); the points are the figures' sampled paths mapped with the board's layout (canvas
1280 × 800, match 0.5; board anchors 0.03–0.68 × 0.04–0.69; the four lines over 84% of the board's height).
The bubble's words are in the log (`Voice: Bubble ro, <n> characters`).

1. **`MeetLetter` (ro-a, step 1):** between the letter card and the picture, a small card with the a by hand
   (and the RO badge). Tap it: the a writes itself, „Uite cum se scrie litera de mână.”.
2. **The a by hand (ro-a, step 3):** the board with the "tip I" lines (dashed top, solid middle, solid baseline,
   dashed below) and faint slant lines, the dotted a, dot 1 on the baseline at the left, the yellow arrow up.
   - **One line through all three strokes:** „Bravo, ai scris litera a de mână! Acum scrie pe umbră.” (54).
   - **Round 2, begun at the round's top (no entry):** „Pornește de pe linie.” (21), dot 1 pulses.
   - **Then backwards from the exit:** the second miss: „Uită-te cum se scrie, apoi încearcă tu.” (39), the
     entry draws itself.
   - **A good line:** „… Acum scrie fără ajutor, de la punct.” (70).
   - **Round 3, off after the entry:** the entry kept (dot 2 at the round's top) and „Încet și cu grijă: o să
     reușești!”; **off during the round:** a miss.
   - **Two fingers:** finger A writes the round and the stem from dot 2 while finger B scribbles to the right:
     only A counts, „Bravo, ai scris litera a de mână!” (33), a praise, the next step.
3. **mama (ro-words-1, step 7):** „Scrie cuvântul de mână, fără să ridici degetul.” and „m-a-m-a”; the word
   in print on the right.
   - **The m alone:** „Bravo, ai făcut litera m! Acum leag-o de litera a, fără să ridici degetul.” (74) at
     once, and in the bubble.
   - **A lifted join:** the a's first stroke begun 38 units from the join: taken, nothing new said (the m's
     line goes on).
   - **The rest of the a:** „Bravo, ai făcut litera a! Acum leag-o de litera m, …” (74).
   - **„ma” in one line:** „Bravo, ai scris cuvântul mama! Acum scrie pe umbră.” (51).
   - **Round 2, the m and 25 units into the join, lifted:** the m's line (the checkpoint wins); **the rest in one
     line:** „… Acum scrie fără ajutor, de la punct.” (67).
   - **Round 3, the m, then lifted inside the a's entry:** „Nu-i nimic! Pune degetul înapoi pe punct și
     continuă.” (53), no miss; **the rest:** the word's line, a praise, the next step.
4. **tata (step 8):** begun at the t's stem top: „Pornește de pe linie.” (21); **the whole word in one line:**
   „Acum trage liniuța literei t.” (29), dot 2 on the first t's bar; **each bar:** the same line again, then
   „Bravo, ai scris cuvântul tata! Acum scrie pe umbră.” (51).
5. **en-a (step 3):** one round only (one star), "Well done, you wrote the letter a by hand!".
6. **Screen sizes:** `wm size 1280x800`, `2560x1600`, `2048x1536`, `2400x1080`, then `wm size reset`; restart
   the app between sizes. Expected: the board left, below the bubble, the word whole on it; the word in print
   and the stars on the right.

**Automated (PlayMode):** `TracingTests.TestShow_AHandwrittenLetterInOneLine`, `TestShow_MamaLetterByLetter`,
`TestShow_HandwritingMatchesTheStrokes` (every point of the pen line is a sample of the stroke data), and
tracing's `TestMove_DrawingAllocatesNothing`.

**The recorded lines** in `Tests/EditMode/Tracing/Strokes/handwritten/` are generated from the figures (each
path sampled every 5 units, seeded jitter of 1.5–2.5 units, 7 beside for sloppy, reversed for backwards, begun
at the entry's end for no entry, a 60-unit detour for off, begun 38 units from the join for a lift), not drawn by
a finger; regenerate them when a shape changes and check each verdict by hand.

**Cleanup:** `"$ADB" -s <emulator> shell pm clear com.softshack.scolita` and
`"$ADB" -s <emulator> shell wm size reset`.

## Completion record

_Finished 2026-11-25 by `handwriting`; released as 0.8.0 (tag `v0.8.0`)._

### What was built

- **Pushed:** `51cabec` the claim; `42dce67` the Design note; `31da274` `core:`, `cfc3e56` `content:`, `fc5b8b1` `ui:`,
  `547d280` `game:` (pushed together, tested at the last of them, as tracing's slices); `29c8d48` `docs:`; the
  close-out.
- **Handwritten shapes as data:** planet 1's a m t p and A M T P by hand
  (`content/strokes/*-handwritten.json`, `slant` 0.2, all `draft`), the a in three strokes (the human); no new
  field in the stroke format: continued strokes, marks and joins are read from the points and run kinds
  (`content/README.md` → "strokes/"). The step option `rounds`; the English handwritten steps play one round.
- **Core** (`Scripts/Core/Tracing`): `TraceFigure` (a letter or a joined word, each stroke's letter, number,
  continuation, line end, mark), `JoinedWord`, `LineCheck` (a line through strokes that continue each other),
  `StrokeReach`, `LineResult`, `StrokeVerdict.SkippedEntry`, `StrokePath.EntryLength` and `Chain`,
  `TracingTuning.JoinRadius`, `TraceState` with a figure and its rounds; `Activities/TraceWordRules`,
  `TraceJudging`, `TraceLetterRules` by hand; `MeetLetterRules`' "de mână" button (`ActivityRound.Handwriting`);
  `ShapeRules`' handwriting rules; `ActivityLimits.MostTracedLetters`; the factory's `TraceWord`.
- **Unity side:** `UI/Tracing/WritingBoardView` (tracing's `TraceLetterView`, renamed, its `.meta` kept; the "tip
  I" lines and slant guides, dots numbered within their letter, the word in print), `HandwritingView`,
  `FigureFit`; `Prefabs/Lesson/TraceWordView.prefab` and its registry row (`TraceWord` off "not built yet"); the
  "de mână" tile in `MeetLetterView.prefab`; the join radius in `Settings/LearningRules.asset`.
- **Tests:** EditMode 512 (was 486): `TraceFigureTests`, `HandwrittenStrokeTests` (with
  `Tests/EditMode/Tracing/Strokes/handwritten/`), `TraceWordRulesTests`, new `ShapeRulesTests`,
  `MeetLetterRulesTests`, `ActivityRulesFactoryTests` cases. PlayMode 20 (was 17): a handwritten a in one line,
  mama letter by letter, `HandwritingView` matching the stroke data.

### Conventions for later briefs

- **A handwritten letter is a file:** its line's strokes each begin where the one before ends; its entry (if
  any) is the first run, on the baseline; its exit the last run of its line; strokes after the exit are marks
  (written after a word); every handwritten small letter leans alike. The import checks all of it.
- **What is traced is a `TraceFigure`;** judge lines with `LineCheck` (never `StrokeCheck` alone) so strokes that
  continue each other can be written in one line; views read `ActivityRound.Trace.Figure`.
- **A word by hand** is `JoinedWord.Of` of its letters' handwritten small shapes, at most 5 letters.
- **A mark's line** is `trace-dot-<strokes id>` or `trace-bar-<strokes id>`, in both languages.
- **Fitting a figure in a rectangle** is `FigureFit` (board and pen line alike).

### How it was tested

Batch mode at the pushed commits (EditMode 512/512, PlayMode 20/20, `BuildAndroidDebug`, no compiler warning),
the import, the emulator at 2000 × 1200 with drawn lines (the a by hand in one line, without its entry,
backwards, off during and after a stroke, two fingers; mama letter by letter, a lifted join, lifted inside a
join and a letter, in one line; tata with its bars), five screen sizes, the 0 B drawing test, and a fresh-eyes
review (Progress, findings resolved or recorded). Școlița → Record released ids at this close-out (`VERSION`
0.8.0): nothing was added (no new id; shapes are not in the ledger). Not run: the Device Simulator, the device Profiler, listening by ear (no new line).

### Decisions and deviations

- **The handwritten a is three strokes** that continue each other (the human), read as entry, round, stem with
  exit; a line through all three is accepted.
- **{x} and {y} are the letters' sounds,** not their names as the brief said (the agreed curriculum: names
  after planet 6); one switch in `TraceWordRules` if the human prefers names.
- **No words on the "de mână" tile or the screen-8 pills** (no string tables yet); the board's view class is
  renamed `WritingBoardView`; capitals by hand don't join yet (P's and T's hooks are body runs).
- **The four code slices were pushed together,** tested at the last (each needs the others to compile: the
  figure replaces the shape on the board), as tracing's were.

### Follow-ups

In Progress → "Follow-ups": the human (the drafts, sounds or names, the joins under her finger, the join
radius); `content-letters-1` (s i n o d e by hand as data, i's dot, o's high exit, planets 2–3's words);
later marks' line keys (planet 7); `look` (the screen-8 pills, the bubble's hyphen break, the phone-size
bubble); a dot's length; joined capitals (`handwriting-2`).
