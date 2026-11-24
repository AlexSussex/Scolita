# Task: feature/curriculum-research

> **Status: Done (2026-11-24).** `docs/curriculum.md` v1 agreed by the human on 2026-11-24; docs only, no release.
> Decision D1 assumed: Romanian as the anchor, both languages on.

Read `AGENTS.md` first (especially "Learning (a child is the learner)")
and `docs/curriculum.md` (draft v0).

**Feature:** `feature/curriculum-research`
**Implements:** `docs/curriculum.md` §1–§8; PRD §2.
**Depends on:** nothing.

## Scope

A research pass that turns the curriculum draft v0 into v1, which the
human agrees before `content-letters-1` starts.

1. **The Romanian order:** read at least two current Romanian clasa I
   manuals ("Comunicare în limba română", the abecedar part) and one
   clasa pregătitoare guide; record the order in which their letters are
   taught, how they introduce upper and lower case and handwritten letters,
   and how they split words into syllables. Cite each source.
2. **The English order:** the UK "Letters and Sounds" Phase 2–5 order and
   one other systematic synthetic phonics programme; record the order, the
   pure-sound rule, the "tricky words".
3. **The bilingual order:** check draft v0's planets against both; keep
   shared letters first unless the research gives a reason not to; move
   letters if a planet's words don't work in one language. Note every
   change and why.
4. **Bilingual teaching:** a short summary (one page, with sources) of what
   is known about teaching a child to read two languages at once (transfer
   of phonological awareness, keeping the languages apart, cognates), and
   anything it changes in §1 or §4.
5. **The words for planets 1–3,** in both languages: for each, its letters
   (only taught ones), its syllables (Romanian split as a school would),
   whether it can be pictured clearly, and whether it is from her world
   (space, animals, music, drawing, cycling). At least 8 readable words per
   planet per language; the "listen only" words; the word twins and false
   friends among them.
6. **The stroke order** for the 10 letters of planets 1–3, lower and upper
   case, print, as Romanian and English schools teach them (they may
   differ: note where, and propose one that serves both, for decision D2).
7. **The handwriting model** (decision D3, answered 2026-11-24:
   handwritten letters are in the MVP): the handwritten forms ("litere de
   mână") of the same 10 letters, lower and upper case, as current
   Romanian clasa I manuals and their exercise books ("caietul elevului")
   show them: the shapes on the writing lines, the stroke order, the entry
   and exit strokes, how letters join, and how English schools' joined
   handwriting differs. Propose one model for both languages (assumed: the
   Romanian one), with a drawing of each letter for the human to check.
8. **The voice lines** for planets 1–3: the full list (sounds, names,
   words, slow words, syllables, instructions, praise, hints, greetings) in
   both languages, as a draft of `content/voice-lines.csv`.
9. **Update `docs/curriculum.md`** to v1 (keep the structure; mark what
   changed from v0 in a short "Changes from v0" list at the end), and
   write the research notes in `design/research/curriculum-research.md`.

## Out of scope — do not build

- Any code or content asset (that is `content-model` and
  `content-letters-1`).
- Planets after 3 beyond a sanity check of the draft's order.

## Acceptance criteria

- `docs/curriculum.md` v1 has the planets 1–3 words, print and
  handwritten strokes and voice lines, every claim about Romanian or English teaching backed by a cited
  source in the research note.
- Every planet 1–3 word uses only letters taught before it in its track
  (checked by hand, listed in the note).
- The human has agreed v1 (recorded in Progress with the date), or the
  changes asked for are made.

## Tests required

None (docs only). The human reviews the Romanian words and syllables.

## Touches

`docs/curriculum.md`, `design/research/curriculum-research.md`,
`local/for-human-next-steps.md`.

## Screens

None.

## Voice lines

The draft list for planets 1–3 (in the note and §8 of the curriculum).

## Before closing out

- Add "agree the curriculum v1" to `local/for-human-next-steps.md` as soon
  as the draft is pushed; move it to Done when agreed.
- No release (docs only).

## Progress

_Agent `curriculum`, worktree `C:\Projects\scolita-curriculum` (branch
`agent/curriculum`, local only). Docs only: no code, no Unity, no
emulator._

- **Assumed** (recorded in `docs/curriculum.md` v1 and the research
  note): D1 Romanian as the anchor, both on; D3 the Romanian "litere de
  mână" for both languages; D4 British English for the vowels.
- **Pushed:**
  - `746392f` docs: claim curriculum-research.
  - `dd86093` docs: add curriculum v1 draft and its research:
    `docs/curriculum.md` **draft v1** and
    `design/research/`: the research note
    (`curriculum-research.md`: sources, the Romanian and English orders,
    the bilingual check, the one-page bilingual summary, the words of
    planets 1–3 with syllables and the taught-letters check, print stroke
    order, the handwriting model, the voice lines), the drawings
    (`handwriting-model-draft.svg`, `joined-words-draft.svg`,
    `print-stroke-order-draft.svg`), the draft points
    (`strokes-draft.json`) and the voice-line draft
    (`voice-lines-draft.csv`, 584 lines).
  - `542f9d5` docs: curriculum v1 waits for the human.
  - `addc110` docs: mark curriculum v1 agreed by the human.
  - `a0a0a75` docs: add research briefs for how the curriculum grows.
  - The close-out commit: this brief moved to `tasks/done/`.
- **How it was done:** five Romanian manuals or their plannings, the
  Romanian programme and a clasa pregătitoare letter read as PDFs
  (handwriting models looked at page by page); Letters and Sounds, Little
  Wandle and the English national curriculum read as PDFs; the bilingual
  papers read by a helper agent (abstracts, one full text); planet words
  checked on dexonline by a helper agent. The taught-letters check and
  the cedilla check were run by script on the same word list the CSV is
  made from (scratch scripts, not in the repo).
- **Acceptance criteria:** v1 has the planets 1–3 words, print and
  handwritten strokes and voice lines, every teaching claim cited in the
  research note (done); every word uses only taught letters (done,
  research §6, table "The taught-letters check"); ≥8 readable words per
  planet per language: **met except English planet 1 (7)**, for the human
  to agree (research §4.1); the human agrees v1: **waiting**.
- **Open questions for the human** (research §10): English planet 1's 7
  words; names later (sounds only in planets 1–3); the familiar words
  (mami, tati, tanti, nani, papa); ă earlier (for `content-letters-2`);
  the handwriting drawings; the Romanian lines in research §9.2; British
  English (D4).
- **Follow-ups for other briefs** (once v1 is agreed): `look` and
  `handwriting`: the writing board gets a fourth line (descender) and
  maybe slant guides; design screen 8's text: m starts at the middle line
  with a hook. `activities-letters`: `MeetLetter` without the name in
  planets 1–3 (screen 2's name button). `progress-review`: more practice
  for English items. `content-letters-2`: ă earlier.
- **Agreed: the human agreed v1 on 2026-11-24**, with the recommendations of
  research §10 (how each question resolved is at the top of
  `docs/curriculum.md`); D1–D3 recorded by the lead in `ab7abf8`.
- **Growth research briefs** written at the human's request ("its own
  research/briefs, for both languages"): `curriculum-research-2`,
  `curriculum-research-3`, `curriculum-research-names`,
  `curriculum-research-sentences`, `handwriting-research-2`.
- **Finished.** The worktree is removed.

## How to test locally

### Docs slice

Docs only: nothing to build, install or run, and no emulator.

1. **Get the latest `main`:** `git pull` in `C:\Projects\Scolita`.
2. **Read** `docs/curriculum.md` v1. Check that it has:
   - how the seven open questions were answered, at the top;
   - sections §1–§8 in the same structure as v0;
   - "Changes from v0" at the end.
3. **Check the sources:** in `design/research/curriculum-research.md`,
   every claim about Romanian or English teaching carries a source label
   ([R1]–[R9], [E1]–[E4], [B1]–[B16]). Each label is listed in §1, which
   says whether that source was read online.
4. **Check the words:** in research §6, the table "The taught-letters
   check" says "yes" for every word. Pick any five words and check by eye
   that their letters were all taught by their planet (planet 1: a m t
   p; planet 2: + s i n; planet 3: + o d e).
5. **Check for cedillas:** search `design/research/voice-lines-draft.csv`
   for ş or ţ (U+015F, U+0163). There should be none; ș and ț with the
   comma below are used throughout.
6. **Look at the drawings:** open the three `.svg` files in
   `design/research/` in a browser. Each letter should have:
   - four writing lines (in the handwriting sheet);
   - a green numbered dot where each stroke starts;
   - pink arrows showing the direction;
   - orange entry and exit strokes (in the handwriting sheet).
7. **Cleanup:** none.

## Completion record

**What was built** (docs only):

- `docs/curriculum.md` v1, agreed by the human on 2026-11-24.
- `design/research/curriculum-research.md`, the research note:
  - sources;
  - the Romanian and English letter orders;
  - the bilingual check of planets 1–3;
  - the one-page bilingual summary;
  - the words of planets 1–3, with the taught-letters check and the join
    pairs;
  - print stroke order;
  - the handwriting model;
  - the voice lines;
  - the open questions.
- `design/research/handwriting-model-draft.svg`,
  `joined-words-draft.svg` and `print-stroke-order-draft.svg`: drawings
  of the 10 letters, lower and upper case, and of four joined words.
- `design/research/strokes-draft.json`: the points behind the drawings.
- `design/research/voice-lines-draft.csv`: 584 lines, 316 Romanian and
  268 English.
- Five growth research briefs in `tasks/pending/`:
  - `curriculum-research-2`
  - `curriculum-research-3`
  - `curriculum-research-names`
  - `curriculum-research-sentences`
  - `handwriting-research-2`

**Conventions for later briefs:**

- **Research comes before content, for both languages equally** (the
  human, 2026-11-24). Every stage of growth has a research brief. Each
  ends in a version of `docs/curriculum.md` that the human agrees before
  its content or feature brief starts.
- **Cite every claim about how a school teaches.** Use labelled sources
  ([R…] Romanian, [E…] English, [B…] research), and say plainly when a
  source could not be read online.
- **The taught-before check:**
  - Every readable word uses only letters taught before it in its track.
  - Check it by hand, and also by script from the same list the voice
    lines are generated from.
  - List the result in the research note.
- **Romanian words:**
  - Check them on dexonline / DOOM 3.
  - Mark familiar or child-talk words, and drop words that aren't
    standard.
  - Split syllables as a school does: one consonant goes with the next
    vowel; two consonants split; hiatus splits; a diphthong stays whole.
  - Before ă is taught, use the article forms (apa, masa) for feminine
    nouns.
- **Voice lines:**
  - Ids are `<lang>-<kind>-<key>`, with kind one of sound, name, word,
    slow, syllables or line; the clip is the id plus `.ogg`.
  - Placeholders: `{x}` and `{y}` are a letter's clip (its sound in
    planets 1–3), `{word}`, `{slow}`, `{stretched}`, and `{name}` (the
    nominative only).
  - List every spliced combination, and list the lines the agent is
    unsure of for the human.
- **Letter drawings:**
  - Units: top line y=0, middle line 100, baseline 200, descender line
    300.
  - The handwritten slant is x += (200 − y) × 0.2.
  - Each run of a stroke is marked entry, body or exit.
  - Print letters use the English top-start order (D2); handwritten
    letters use the Romanian model (D3).
- **Letters are referred to by their sound in planets 1–3.** Names come
  later (`curriculum-research-names`).

**How it was tested:** docs only.

- The sources were read as PDFs, with the handwriting pages looked at one
  by one.
- Two checks were run by script on the generated data: the taught-letters
  check and the cedilla check.
- The drawings were rendered and checked by eye.
- The human reviewed and agreed v1.

**Decisions** (the human, 2026-11-24):

- D1–D3 are answered: D1 Romanian as the anchor, both languages on; D2
  lower case first, print in the top-start order; D3 the Romanian model
  for both languages (`ab7abf8`).
- The seven open questions were answered as follows:
  1. English planet 1 keeps its 7 words.
  2. Names come later.
  3. The familiar words stay, with their flags.
  4. Moving ă earlier is for `curriculum-research-2`.
  5. The drawings are still to be checked against her manual.
  6. The §9.2 lines are agreed as drafted.
  7. British English stays assumed until D4 is answered.
- **Deviation from the brief:** English planet 1 has 7 readable words,
  not 8, which the human accepted.

**Follow-ups:**

- **`activities-letters` and design screen 2:**
  - `MeetLetter` in planets 1–3 gives the sound and a picture, not the
    name.
  - The name button waits until names are taught
    (`curriculum-research-names`).
  - Spliced letters use the sound clip.
- **`tracing` and `handwriting`:** before their strokes are final, the
  human compares the drawings with her school's manual or notebook. This
  is in `local/for-human-next-steps.md`. The capitals T, S and E are the
  least certain.
- **`handwriting` and `look`:**
  - The writing board gets a fourth line, for descenders, in the "tip I"
    pattern: dashed, solid, solid, dashed. Faint slant guides are worth
    trying.
  - m and n start at the middle line with a hook, not from the baseline.
    Design screen 8's description of the first m should follow this.
  - "Pornește de pe linie." is never said for m or n.
- **`content-letters-1`:** take the words, TraceWord words, syllables and
  voice lines from research §6 and §9 and the draft CSV.
- **`content-model`:** the draft CSV's columns are a proposal
  (id, language, kind, text, clip, planet, used_by, notes).
- **`progress-review`:** English items may need more practice than
  Romanian ones (research §5).
- **`curriculum-research-2`:** decide whether ă moves earlier.
- **D4:** British English for the system voice and the vowels is still
  open.
