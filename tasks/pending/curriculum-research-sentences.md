# Task: feature/curriculum-research-sentences

> **Status: After `curriculum-research-3`** (docs only; `feature/sentences`
> waits for it). The human asked (2026-11-24) that the curriculum's growth
> gets its own research, for both languages, before it is built.

Read `AGENTS.md` first (above all "Learning (a child is the learner)",
"Romanian, checked", "Correct, complete language"), the latest
`docs/curriculum.md`, PRD §3.10, and the completion records of the
curriculum research briefs in `tasks/done/`.

**Feature:** `feature/curriculum-research-sentences`
**Implements:** PRD §3.10 ("short sentences, little stories read aloud
with words lighting up"); a new `docs/curriculum.md` section.
**Depends on:** `curriculum-research-3` (the groups, digraphs and tricky
words taught).

## Scope

How she goes from words to sentences and short stories, **in both
languages equally**.

1. **What the schools do:** how Romanian clasa I manuals move from words
   to "propoziția" and short texts (capital letter, full stop, question
   mark, the dialogue dash, reading "pe silabe" then whole words), and how
   English programmes do (captions and sentences in Letters and Sounds
   Phases 2–4, decodable readers, tricky words in sentences); cite each.
2. **Decodable sentences:** what "only what has been taught" means for a
   sentence (every word decodable or a taught tricky word; Romanian
   function words: e, este, sunt, și, la, cu, în …, and the iotated e of
   "e, este, el, ea"), and how the validator could check it.
3. **Activities:** which existing kinds extend to sentences
   (`ReadAndMatch`, `BuildWord` as `BuildSentence`?), what new kind is
   needed only if none fits (AGENTS.md → "One activity contract"), reading
   aloud with words lighting up, and writing a short sentence by hand
   (joined, with a capital: needs `handwriting-research-2`).
4. **Little stories:** a first set of 3–5 short stories per language from
   her world (space, unicorns, animals, music), each only with taught
   words, each checked for correct, complete language; whether stories
   are paired across the languages (the same story in both) or apart.
5. **The voice lines** this needs in both languages (sentences read
   whole and word by word, instructions, praise), checked against
   "Romanian, checked".
6. **Write it up:** `design/research/curriculum-research-sentences.md`
   (every teaching claim cited) and the next version of
   `docs/curriculum.md`, agreed by the human.

## Out of scope — do not build

- Code and content (`feature/sentences` builds them).
- Free writing (`handwriting-research-2`).

## Acceptance criteria

- The next version of `docs/curriculum.md` has the sentence and story
  stage in both languages: when it starts, the activities, the rules for
  a decodable sentence, and the first stories; every teaching claim cited.
- Every sentence and story uses only taught letters, groups and tricky
  words (checked by hand, listed).
- Every Romanian sentence passes "Romanian, checked".
- The human has agreed it (recorded with its date).

## Tests required

None (docs only). The human reads every sentence and story.

## Touches

`docs/curriculum.md`, `design/research/curriculum-research-sentences.md`,
`local/for-human-next-steps.md`.

## Screens

None designed here (`feature/sentences` designs them).

## Voice lines

The draft list for the first sentences and stories, both languages.

## Before closing out

- Add "agree the sentences and stories stage" to
  `local/for-human-next-steps.md` when the draft is pushed.
- No release (docs only).
