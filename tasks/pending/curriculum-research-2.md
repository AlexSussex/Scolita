# Task: feature/curriculum-research-2

> **Status: Ready** (docs only; can run beside any code brief; one
> curriculum research brief at a time edits `docs/curriculum.md`).
> The human asked (2026-11-24) that the curriculum's growth gets its own
> research, for both languages, before its content is written.

Read `AGENTS.md` first (above all "Learning (a child is the learner)",
"Romanian, checked", "Audio"), `docs/curriculum.md` v1, and
`tasks/done/curriculum-research.md`: its research note
(`design/research/curriculum-research.md`) and its completion record hold
the sources, the method and the conventions this brief continues.

**Feature:** `feature/curriculum-research-2`
**Implements:** `docs/curriculum.md` §3 (planets 4–6), §6, §7, §8; PRD
§3.10 ("Reading further").
**Depends on:** `curriculum-research` (done).

## Scope

The research that turns planets 4–6 (draft: u r c l · b f g h · v z j k)
into a curriculum version the human agrees before `content-letters-2`
starts. **Both languages get the same depth** at every step.

1. **The order of planets 4–6, in both languages.** Check the draft
   against the Romanian manuals already read ([R1]–[R5] in the v1
   research) and the English programmes ([E1], [E2]), and at least one
   more source per language where it helps (another clasa I manual or its
   "caietul elevului"; Jolly Phonics or another validated English SSP
   programme). Record where each letter sits in each, and keep, move or
   swap letters with the reason written down.
2. **ă: earlier or not.** Every Romanian manual teaches ă about 9th–13th,
   v1 puts it on planet 7 (v1 research §4.2). Decide, with the human,
   whether ă joins planet 4 or 5 (and what that does to the English side
   of that planet: an English lesson-free letter, or a Romanian-only
   step), and how planets 4–6's Romanian words change with it.
3. **The sounds that differ:** c (Romanian /k/ before a o u, /tʃ/ in ce
   ci; English /k/, /s/ before e i y), g (the same pattern, /dʒ/), r
   (rolled in Romanian), h, j (/ʒ/ and /dʒ/), u (/u/ and /ʌ/): which get
   `TwoSounds`, which soft sounds are kept out of planets 4–6 (so "cer",
   "ger", "cinema" are listen only until the groups are taught), and the
   English letters with two common sounds (c, g) at this stage.
4. **The words of planets 4–6, in both languages:** for each planet and
   language at least 8 readable words (only letters taught before it in
   its track, checked by hand and listed), with Romanian syllables as a
   school splits them (checked on dexonline / DOOM 3), whether each can be
   pictured, whether it is from her world (space, unicorns, rainbows,
   animals, music, drawing, cycling), the listen-only words, the word
   twins (unicorn, telescop, robot …) and false friends; the TraceWord
   words; English words with two consonants together marked, as in v1.
5. **Print strokes** (English top-start order, D2) and **handwritten
   strokes** (the Romanian model, D3) for the 12 letters, **lower and
   upper case**, with the same kind of drawings as v1
   (`design/research/handwriting-model-draft.svg`): stroke order, entry and
   exit strokes, joins, and any letter that does not join; checked against
   at least two Romanian manuals' models; the letters with a descender or
   an ascender on the four writing lines.
6. **The voice lines** of planets 4–6 in both languages (sounds, names
   recorded for later, words, slow words, syllables, new instructions or
   hints if the planet needs any, planet arrivals, constellations), as a
   draft CSV in `design/research/`, every Romanian line checked against
   "Romanian, checked" and every spliced combination listed; the lines the
   agent is unsure of listed for the human.
7. **Write it up:** the research note
   `design/research/curriculum-research-2.md` (every claim about teaching
   tied to a cited source; sources that could not be read online said so)
   and the next version of `docs/curriculum.md` (keep its structure; a
   "Changes from v1" list at the end).

## Out of scope — do not build

- Content (`content/`, imported assets): that is `content-letters-2`.
- Code, scenes, art.
- Planets 7–9 beyond a sanity check (`curriculum-research-3`) and letter
  names (`curriculum-research-names`).

## Acceptance criteria

- The next version of `docs/curriculum.md` has planets 4–6's order, words,
  print and handwritten strokes (lower and upper case) and voice lines in
  **both languages**, every teaching claim backed by a cited source in the
  research note.
- Every planet 4–6 word uses only letters taught before it in its track
  (checked by hand, listed in the note).
- No cedilla anywhere; every Romanian line passes "Romanian, checked".
- The ă decision is recorded with its reason.
- The human has agreed the new version (recorded in Progress with the
  date), or the changes asked for are made.

## Tests required

None (docs only). The human reviews the words, the syllables, the
drawings and the lines.

## Touches

`docs/curriculum.md`, `design/research/curriculum-research-2.md` and its
drafts (`design/research/`), `local/for-human-next-steps.md`.

## Screens

None.

## Voice lines

The draft list for planets 4–6, both languages (in the note's CSV).

## Before closing out

- Add "agree the curriculum for planets 4–6" to
  `local/for-human-next-steps.md` as soon as the draft is pushed; move it
  to Done when agreed.
- No release (docs only).
