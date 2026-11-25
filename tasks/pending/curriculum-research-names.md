# Task: feature/curriculum-research-names

> **Status: In progress (curriculum-research-names)** (docs only; best done before planet 6's content, since
> v1 brings names in after planet 6; one curriculum research brief at a
> time edits `docs/curriculum.md`). The human agreed on 2026-11-24 that
> letters are met by their sound first and their names come later.

Read `AGENTS.md` first (above all "Learning (a child is the learner)",
"Romanian, checked"), `docs/curriculum.md` v1 (§1 "Sounds first, names
later"), and `tasks/done/curriculum-research.md` with its research note
(§6.7: why names wait).

**Feature:** `feature/curriculum-research-names`
**Implements:** `docs/curriculum.md` §1, §4.2 (`MeetLetter`), §8.
**Depends on:** `curriculum-research` (done).

## Scope

When and how the letter names come in, **in both languages equally**, so
that the names never blur the sounds or the two languages.

1. **What the schools do:** when Romanian manuals and teachers bring in
   the names ("a, be, ce, de …", the alphabet lesson of clasa I; which
   forms: "me" or "em", "ne" or "en", the names of q w x y) and when
   English phonics does ("Teaching letter names" in Letters and Sounds
   Phase 3, the alphabet song; at least one more programme); cite each.
2. **The clashes between the two languages' names and sounds** (the
   English name of e is the Romanian sound of i; the English name of i is
   no Romanian sound; a, o, u; the Romanian names of k, q, w, y) and what
   the research on bilingual children says about them; how to show a name
   is a name (not a sound) and which language it belongs to.
3. **The design:** when names start (v1: after planet 6), what activity
   teaches them (an alphabet song, an activity kind of its own, or a
   `MeetLetter` round), whether the alphabet's order is taught, how the
   name button on design screen 2 comes back, and how spliced lines ("the
   letter {x}") switch from the sound to the name; reusing an existing
   activity kind where one fits (AGENTS.md → "One activity contract").
4. **The voice lines** this needs in both languages (names, any new
   instructions, the song if any), checked against "Romanian, checked".
5. **Write it up:** `design/research/curriculum-research-names.md` (every
   teaching claim cited) and a section in the next version of
   `docs/curriculum.md`, agreed by the human.

## Out of scope — do not build

- Code, content, the song's music or recording.
- Changing what planets 1–3 do (sounds only, agreed in v1).

## Acceptance criteria

- The next version of `docs/curriculum.md` says when and how names are
  taught in both languages, with the activity, the lines and the switch
  from sounds to names; every teaching claim cited.
- Each name clash between the two languages is listed with how it is
  handled.
- The human has agreed it (recorded with its date).

## Tests required

None (docs only).

## Touches

`docs/curriculum.md`, `design/research/curriculum-research-names.md`,
`local/for-human-next-steps.md`.

## Screens

None designed here; a new screen or activity becomes a follow-up brief.

## Voice lines

The letter names of both alphabets (31 and 26) and any new lines, as a
draft list.

## Before closing out

- Add "agree when letter names come in" to
  `local/for-human-next-steps.md` when the draft is pushed.
- No release (docs only).
