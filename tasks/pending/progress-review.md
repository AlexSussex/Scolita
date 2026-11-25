# Task: feature/progress-review

> **Status: In progress (progress-review).** Decision D1
> (the language mix) is read from the settings. The human, 2026-11-24 (Q1):
> each damaged save is kept, dated (`<name>.broken-<date-time>.json`), never
> replaced by the next, and removed by themselves once a newer version has
> read and saved that file fine on several days (the exact rule in the
> Design note).

Read `AGENTS.md` first (especially "Learning" → learned and reviewed,
"Versioning and releases" → "Progress survives updates", "Fresh-eyes
review"), `docs/curriculum.md` §5, and the completion records in
`tasks/done/`.

**Feature:** `feature/progress-review`
**Implements:** PRD §3.5; `docs/curriculum.md` §4.1 (reviews, the mix), §5.
**Depends on:** `activities-words`, `handwriting`, `galaxy-map`.

A **medium change** to the save format: Design note first; a fresh-eyes
review before the push (it is her progress).

## Scope

1. **Core's `LearnerProgress`:** per item (letter, later word) and
   language: right answers by activity kind, the days they happened on,
   its state (new, learning, learned), its review step and next review
   date. Raises `ItemMastered`.
2. **Mastery** from the learning rules asset (`docs/curriculum.md` §5
   defaults).
3. **The review schedule:** learned items due for review become up to two
   warm-up steps at a lesson's start (a quick `ListenAndPick` or
   `FirstSound` on the item); a miss moves it back a step.
4. **The language mix:** `Both` (each letter's Romanian lesson then its
   English one, as `curriculum.md` §4.1), `RomanianOnly`, `EnglishOnly`
   (the other track's lessons and the mixed kinds left out), read from the
   settings; the map shows only the lessons of the chosen mix.
5. **The `progress` save file** per profile, versioned, replacing
   `map-progress` (migrated from it), with unknown ids kept and ignored.

## Out of scope — do not build

- The parent area's grid (it reads this; `parent-area`), words' own mastery
  beyond the data being kept.

## Acceptance criteria

- With a `FakeClock`, an item answered right 6 times across 2 kinds on 2
  days becomes learned; one answered 6 times on one day stays learning.
- A learned item comes back as a warm-up 1 day later, then 3, then 7.
- Switching the mix to Romanian only hides the English lessons on the map
  and keeps the English progress.
- An old `map-progress` file is migrated; a corrupt `progress` file is set
  aside and the map starts from the first lesson, with a log line.

## Tests required

- EditMode: mastery, the schedule over simulated days, the warm-up
  choice, the mix filter, the migration and the broken-file rule.
- PlayMode: a lesson with a due review starts with its warm-up.
- Emulator: progress survives killing the app and reinstalling the APK
  over itself.

## Touches

`Scripts/Core/Progress`, `Scripts/Game/Saving`, `LessonRun` (warm-ups),
the map's progress source, the `progress` save file (new).

## Screens

None new (warm-up steps use existing views).

## Voice lines

"Hai să ne amintim!" / "Let's remember!", both languages.

## Before closing out

- Release (the next MINOR). Technical notes: the save-format change.
