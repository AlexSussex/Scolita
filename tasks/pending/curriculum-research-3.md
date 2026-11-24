# Task: feature/curriculum-research-3

> **Status: After `curriculum-research-2`** (docs only). The human asked
> (2026-11-24) that the curriculum's growth gets its own research, for
> both languages, before its content is written.

Read `AGENTS.md` first (above all "Learning (a child is the learner)",
"Romanian, checked", "Audio"), the latest `docs/curriculum.md`, and the
completion records of `curriculum-research` and `curriculum-research-2`
in `tasks/done/`, with their research notes in `design/research/`.

**Feature:** `feature/curriculum-research-3`
**Implements:** `docs/curriculum.md` §2 (the groups of letters), §3
(planets 7–9), §6, §7, §8; PRD §3.10 ("Reading further").
**Depends on:** `curriculum-research-2` (planets 4–6 agreed, so the letters
taught before planet 7 are known, ă included).

## Scope

The research that turns planets 7–9 into a curriculum version the human
agrees before `content-letters-3` starts. The two languages part here
(each has letters and groups the other has not), so **each gets its own
track of equal depth**, with the bridges between them kept.

1. **Romanian:** ă â î ș ț (whatever `curriculum-research-2` left for
   planet 7), and the groups **ce ci ge gi che chi ghe ghi**, with the
   diphthongs and triphthongs the manuals teach alongside (ea, oa, ia, ie,
   iu …), and the asyllabic final i (pomi, ani). The order and the way the
   manuals teach them ([R1]–[R5] in the v1 research and at least one more
   manual), and when â and î are each used (DOOM 3 rules).
2. **English:** the Phase 2–3 digraphs **ck sh ch th ng** (and nk), the
   letters **w x y q** (qu), and the Phase 3 vowel teams (ai ee igh oa oo
   ar or ur ow oi ear air er), in the order of Letters and Sounds and at
   least one more SSP programme ([E1], [E2] in the v1 research); the
   **tricky words** of Phases 2–4 (the, to, I, no, go, into, he, she, we,
   me, be, was, you, they, all, are, my, her, said, have, like, so, do,
   some, come, were, there, little, one, when, out, what): which are
   taught, how they are taught without breaking "only what has been
   taught", and how the content marks them (a tricky word is not a
   "listen only" word).
3. **The planets:** how many planets (v1 drafts 7–9 with a Romanian and an
   English planet side by side), what a child in "both" mode does on a
   planet that has only one language, and how `TwoSounds` and `WordTwins`
   bridge them (rachetă/rocket, planetă/planet, astronaut, cometă/comet
   become readable here). Q, w, x, y in Romanian (borrowed words only):
   taught, or English only.
4. **The words** of each planet and language: at least 8 readable words
   per planet per language, only letters and groups taught before (checked
   by hand and listed), Romanian syllables as a school splits them
   (dexonline / DOOM 3), picturable, her world (the space words become
   readable here), listen-only words, twins and false friends.
5. **Print and handwritten strokes** for every new letter, **lower and
   upper case** (ă â î ș ț with their marks: where the breve, the
   circumflex and the comma below go and when they are written; w x y q),
   drawn as in v1; how a group (ch, sh) is traced: as its letters, joined.
6. **The voice lines** of planets 7–9, both languages (sounds of the
   groups and digraphs, words, slow words, syllables, tricky-word lines,
   any new instructions), as a draft CSV, every Romanian line checked
   against "Romanian, checked" and every spliced combination listed.
7. **Write it up:** `design/research/curriculum-research-3.md` (every
   teaching claim cited) and the next version of `docs/curriculum.md`
   ("Changes from" the previous version at the end).

## Out of scope — do not build

- Content and code (`content-letters-3` builds the content).
- Sentences and stories (`curriculum-research-sentences`), letter names
  (`curriculum-research-names`).

## Acceptance criteria

- The next version of `docs/curriculum.md` has planets 7–9 in both
  languages: order, groups, tricky words, words, strokes (lower and upper
  case) and voice lines, every teaching claim cited.
- Every word uses only letters and groups taught before it in its track,
  tricky words aside, which are listed as such (checked by hand).
- No cedilla anywhere; ș ț with the comma below in every line and drawing;
  every Romanian line passes "Romanian, checked".
- The human has agreed the new version (recorded with its date), or the
  changes asked for are made.

## Tests required

None (docs only). The human reviews the words, the syllables, the
diacritic drawings and the lines.

## Touches

`docs/curriculum.md`, `design/research/curriculum-research-3.md` and its
drafts, `local/for-human-next-steps.md`.

## Screens

None (a planet with one language may need a screen; if so, note it as a
follow-up for `galaxy-map`).

## Voice lines

The draft list for planets 7–9, both languages.

## Before closing out

- Add "agree the curriculum for planets 7–9" to
  `local/for-human-next-steps.md` when the draft is pushed; move it to
  Done when agreed.
- No release (docs only).
