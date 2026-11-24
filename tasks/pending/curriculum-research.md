# Task: feature/curriculum-research

> **Status: In progress (curriculum): draft v1 pushed; waiting for the human to agree v1** (docs only; can run beside `project-skeleton`).
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
- **Waiting for the human to agree v1** (since 2026-11-24; the step is in
  `local/for-human-next-steps.md` → "Now"). The worktree is removed;
  the next agent starts a fresh one from `origin/main`.
- **Next:** when the human answers, make the changes asked for (or
  none), record the agreement here with its date, move the brief to `tasks/done/`
  with its completion record (no release: docs only).
