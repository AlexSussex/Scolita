# Task: feature/curriculum-research

> **Status: Ready** (docs only; can run beside `project-skeleton`).
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
