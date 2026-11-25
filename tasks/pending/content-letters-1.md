# Task: feature/content-letters-1

> **Status: In progress (content-letters-1)**, since 2026-11-25.

Read `AGENTS.md` first (especially "Learning", "Audio", "The human's next
steps" → the child's name), `docs/curriculum.md` v1, and the completion
records in `tasks/done/`.

**Feature:** `feature/content-letters-1`
**Implements:** `docs/curriculum.md` §3 (planets 1–3), §4.1, §6, §8.
**Depends on:** `curriculum-research`, `content-model`; `tracing` and
`handwriting` for the strokes' format.

## Scope

1. **Planets 1–3** (a m t p, s i n, o d e) in both languages, in the
   content CSVs: letters (sounds, names, groups), words (letters,
   syllables, pictures, twins, listen-only), lessons (the shape in
   `curriculum.md` §4.1: per letter a Romanian and an English lesson, a
   word lesson per language per planet, a planet party), stickers and
   constellations for the three planets.
2. **Strokes** for the 10 letters, **print and handwritten** (lower case;
   upper case if D2 says so), the handwritten ones with their entry and
   exit strokes, as `curriculum-research` records the Romanian school's
   model; and the `TraceWord` words of each planet (mama, tata, pat …).
3. **Pictures** for every readable word, in the look's style (from
   `look`, or placeholders in its palette until then), each named by what
   it shows.
4. **Voice lines** for everything: the list complete in
   `voice-lines.csv`; clips as the human records them (the guide is
   `local/todo/04-record-voices.md`): the agent trims, normalises and
   names them with **Check voice clips**, and listens to every one.
5. **A human review:** every Romanian and English word, syllable split and
   line checked by the human (a table in Progress for them to tick).

## Out of scope — do not build

- Planets 4 and on (`content-letters-2`), code changes (a missing feature
  goes to a follow-up).

## Acceptance criteria

- The validator passes with no warnings except clips not yet recorded,
  which are listed in Progress.
- Every word on planets 1–3 is readable with the letters taught before it.
- The human has ticked the review table.
- She can play planets 1–3 from start to end in "both" mode on the Tab A7
  (after `parent-area`).

## Tests required

- EditMode: the validator and freshness test; each activity kind's rules
  run over every step of planets 1–3 (a test that walks the content).
- Listening: every line, both languages.

## Touches

`content/` (all CSVs, `strokes/`, `voice/`, pictures), the imported
assets, the catalogs.

## Screens

None new.

## Voice lines

All of planets 1–3 (several hundred lines: the list is in `voice-lines.csv`).

## Before closing out

- `local/for-human-next-steps.md`: the lines still to record, grouped by
  planet, so a recording session can do one planet at a time.
- Release (the next PATCH if only content; MINOR if it completes the MVP's
  content alongside `sound-feel`).
