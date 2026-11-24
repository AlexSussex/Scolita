# Task: feature/handwriting-research-2

> **Status: After `handwriting`** (docs only; `feature/handwriting-2`
> waits for it). The human asked (2026-11-24) that the curriculum's growth
> gets its own research, for both languages, before it is built.

Read `AGENTS.md` first (above all "Learning (a child is the learner)" →
letter shapes, "Romanian, checked"), `docs/curriculum.md` §7, the
`curriculum-research` research note (§8, the handwriting model) and the
completion records of `curriculum-research`, `tracing` and `handwriting`
in `tasks/done/`.

**Feature:** `feature/handwriting-research-2`
**Implements:** `docs/curriculum.md` §7 ("Later: handwritten capitals
joined into names and sentences, and free writing without a guide").
**Depends on:** `handwriting` (the MVP's handwritten letters and joined
lower-case words, with her reactions to them).

## Scope

How her handwriting grows after the MVP, **for both languages** (one hand
for both, decision D3).

1. **Joined capitals:** which handwritten capitals join to the next
   letter in the Romanian model and which do not (A M N join in v1's
   sources; check every capital in at least two manuals), how a capital
   starts a name or a sentence, and how English joined handwriting treats
   capitals (the national curriculum and at least one scheme); cite each.
2. **The rest of the alphabet by hand:** the handwritten forms of the
   letters after planets 1–6 as they are added (with
   `curriculum-research-2` and `-3`), and the letters with marks (ă â î ș
   ț: when the mark is written, after the word or at once).
3. **Free writing without a guide:** how schools move from tracing to
   copying ("copiere"), writing from memory ("transcriere") and dictation
   ("dictare") in clasa I, and the English equivalents; what a tablet can
   check fairly when there is no guide (shape recognition of one letter,
   of a word), and what it should not try to judge; how to stay kind when
   it cannot tell.
4. **Writing sentences by hand:** spacing between words, the capital and
   the full stop, in both languages (with `curriculum-research-sentences`).
5. **The model's drawings** for every new form (capitals joined, the
   remaining letters), as in v1, checked against the manuals and against
   her own school's manual or notebook if the human can share it.
6. **The voice lines** (checkpoints, hints) in both languages, checked
   against "Romanian, checked".
7. **Write it up:** `design/research/handwriting-research-2.md` (every
   teaching claim cited) and the next version of `docs/curriculum.md` §7,
   agreed by the human.

## Out of scope — do not build

- Code, stroke content files, the recognition engine
  (`feature/handwriting-2` builds them after this is agreed).

## Acceptance criteria

- The next version of `docs/curriculum.md` §7 has joined capitals, the
  rest of the handwritten alphabet's plan, free writing and handwritten
  sentences, for both languages, every teaching claim cited.
- The drawings cover every new form, with stroke order, entry, exit and
  joins.
- The human has agreed it (recorded with its date), after comparing the
  drawings with her school's handwriting.

## Tests required

None (docs only).

## Touches

`docs/curriculum.md` §7, `design/research/handwriting-research-2.md` and
its drawings, `local/for-human-next-steps.md`.

## Screens

None designed here.

## Voice lines

The draft list of the new checkpoints and hints, both languages.

## Before closing out

- Add "agree the handwriting plan after the MVP" to
  `local/for-human-next-steps.md` when the draft is pushed.
- No release (docs only).
