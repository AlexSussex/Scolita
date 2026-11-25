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

## Progress

_Agent `progress-review`, worktree `C:\Projects\scolita-progress`, emulator `emulator-5600`._

### Design note (a medium change: the save format)

Not a large change: the lesson engine already plays review steps before a lesson's own
(`LessonPlan.Of`'s `reviewSteps`, `PlannedStep.IsReview`, never resumed), and the content
model is untouched (a warm-up is an ordinary `Step` made from content items; one voice line
pair is new content).

**Where the logic lives.** All of it in Core (`Scripts/Core/Progress`, no UnityEngine); the
Unity side loads and saves the file and wires the events.

- **An item** (`LearningItem`): an `ItemKind` (`Letter`, `Word` now), a content id and one
  language. `m` in Romanian and `m` in English are two items. Room for the proposed letter
  names (`curriculum-research-names`, not agreed): a name becomes its own kind
  (`LetterName`), practised by steps with `cue=name`; nothing else changes. Not built.
- **`LearnerProgress`** (per profile): for each item, its state (`New` when absent,
  `Learning`, `Learned`), its right answers by activity kind, its answers practised with the
  answer shown, the days it was answered right (the latest 30 kept), its review step and next
  review day; the lessons done and the rocket's stop (`galaxy-map`'s `MapProgress`, kept
  inside it); the days she played (the latest 100, for `parent-area`'s two weeks). It is the
  map's `IDoneLessons` now. It raises `ItemMastered` when an item becomes learned.
- **What a step practises** (`PractisedItems`, one line per kind, like the rules factory):
  `ListenAndPick` its answer (the first letter, or the first word), `LetterHunt` and
  `TraceLetter` their letter, `FirstSound` the letter its word starts with, `TwoSounds` its
  letter in both languages, `Syllables` / `BuildWord` / `ReadAndMatch` / `TraceWord` their
  word, `WordTwins` all its words; `MeetLetter` only meets its letter (first contact, no
  answer).
- **What counts** (`LessonProgress`, listening to `LessonRun`'s events): a step done without
  its answer shown is one right answer for each of its items, of its kind, today; a step done
  with the answer shown is practice (counted apart, never "right"). Counting steps, not taps,
  keeps a hunt's four stars or a trace's three rounds from making a letter learned in one
  step. Recorded at the end of each step (no allocation during an activity), then saved.
- **Mastery** (`Mastery`, the one owner): learned when the right answers reach the number of
  its language (6 in Romanian, 6 in English: per language so English can be given more,
  `curriculum.md` §5), across at least 2 activity kinds, on at least 2 different days. Learned
  stays learned: a rule tuned later never takes a letter away (no lost rewards).
- **The review schedule** (`ReviewSchedule`, the one owner): learned on day D, its first
  review is D + 1; a warm-up done without a miss moves it one step on (1, 3, 7, 14, 30 days,
  counted from the day of the review; the last step repeats); a miss moves it back one step
  and it comes again after that step's days. Only warm-ups move the schedule.
- **Warm-ups** (`WarmUps`, behind a small `IWarmUps` that `LessonResume` asks, as the map
  asks `IDoneLessons`): before any lesson (also one resumed), up to 2 of her learned letters
  due today (most overdue first, then in teaching order), each one step:
  - only letters in the lesson's language (either, for a lesson in both), in her mix, taught
    before this lesson in its track, and not the letter the lesson introduces;
  - the step is a `ListenAndPick` (the letter and 2 other letters taught before the lesson:
    only taught letters as wrong choices) or a `FirstSound` (a word from the content's own
    `FirstSound` steps for that letter, so its picture and stretched line exist), chosen with
    the run's `IRandom`; an item with no possible step waits for a later lesson;
  - before the first warm-up the mascot says "Hai să ne amintim!" / "Let's remember!" in the
    warm-up's language (`LessonRun.StepIntroduction`, said before the step's instruction).

  The numbers (2 warm-ups, 3 choices, the schedule, the steps back) are in the rules asset.
  Words are counted and get a state, but get no warm-ups yet (out of scope: words' own
  mastery); a numbers or music domain adds its item kind and its warm-up kinds the same way.
- **The language mix** is already the map's and the lesson's filter (`LessonOrder`,
  `LessonPlan`); this brief adds that a change of mix keeps every item and lesson of the other
  language (they are only hidden), and warm-ups follow the mix.

**The save** (`profiles/<id>/progress.json`, through `SaveStore` only), replacing
`map-progress.json`:

```json
{ "version": 1,
  "doneLessons": ["ro-a", "en-a"], "rocketStop": "ro-m", "daysPlayed": ["2026-11-25"],
  "items": [ { "kind": "letter", "id": "m", "language": "ro", "state": "learned",
               "rights": [ { "activity": "ListenAndPick", "count": 3 }, { "activity": "FirstSound", "count": 3 } ],
               "practised": 1, "rightDays": ["2026-11-24", "2026-11-25"],
               "learnedOn": "2026-11-25", "reviewStep": 0, "nextReview": "2026-11-26" } ] }
```

Read safely (`ProgressData.Upgraded`): an id the content doesn't know is kept and ignored
(a later version may know it); a text that can't be an id, an unknown item kind, language,
state or activity kind, or a date that isn't one is dropped (a later version that adds an
item kind or an activity kind raises the file's version, so these come only from damage);
counts below 0 become 0; a learned item without a next review is due today.

**The migration**, when `progress.json` is missing (each earlier shape):

- **0.10.0** (`map-progress.json` version 1): its lessons done and `rocketStop` (which still
  decides the trip);
- **0.9.0 and earlier** (no `map-progress.json`): every lesson before the one in
  `lesson-run.json` is done (`MapProgress.Before`, as `galaxy-map` did);
- both are joined (neither ever claims more than she did); the letters the done lessons
  introduced become `Learning` (she met them; no right answers are invented);
- saved at once as `progress.json` version 1; `map-progress.json` is left as it is, never
  read again while `progress.json` reads.

A **damaged** `progress.json` is set aside dated (as every file) and rebuilt the same way,
with a warning in the log: the map keeps her place from `map-progress.json` and
`lesson-run.json` instead of starting from the first lesson (a deviation from the acceptance
criterion's wording, following `galaxy-map`'s reviewed decision: her place is not thrown
away). Her item counts from the damaged file are in its dated copy, for rescue by hand.

**The damaged copies' cleanup** (the human's rule, `AGENTS.md` → "Progress survives
updates"). Each folder (the tablet's, each profile's) gets a small ledger,
`broken-copies.json` (through `SaveStore`, versioned): per file name with damaged copies, the
app version that set the last copy aside and the days it was read and saved fine since.

- **"A newer version"**: `SaveStore` writes `Application.version` (from `VERSION`) into the
  ledger when it sets a copy aside; a day counts only when the version reading and saving the
  file is newer (`System.Version` comparison). Copies from 0.10.0, made before the ledger,
  are found at start and counted as set aside by an unknown, older version.
- **"Read and saved fine on a day"**: the store notes the calendar day (the tablet's local
  date) when a file it loaded without trouble in this session is saved without trouble, once
  per file per day. A new damaged copy of the same file starts the count again.
- **The removal**: at start, `BootLoader` asks each folder's store to remove every damaged
  copy of a file whose count reached the rules asset's number (3 different days), and to
  forget it in the ledger. The rule itself (`DamagedCopies`) is Core, tested with a fake
  clock; the file work is `SaveStore`'s.

**Tests.** EditMode: `MasteryTests` (6 right across 2 kinds on 2 days: learned; 6 on one
day, or of one kind: learning; English's own number), `ReviewScheduleTests` (1, then 3, then
7 days; a miss back one step), `LessonProgressTests` (real lessons played through
`LessonRun`: a step done, one with the answer shown, a warm-up missed), `WarmUpsTests` (due
items, at most 2, language and mix, taught-only choices, none when nothing is due),
`LessonResumeTests` (warm-ups before a resumed lesson), `ProgressDataTests` (shape, round
trip, unknown and malformed values), `ProgressFileTests` (the migration from 0.10.0, from
0.9.0, from nothing; a damaged file rebuilt; a mix change keeps the other language),
`DamagedCopiesTests` and `SaveStoreTests` (the ledger, the newer-version rule, different days,
the removal). PlayMode: a lesson with a due letter starts with its warm-up and the "let's
remember" line. Emulator: an upgrade from 0.10.0 keeps her place, a damaged
`progress.json` is set aside dated, a warm-up on a later day, kill and reinstall.

**Another domain.** Items, mastery, the schedule and the save know nothing of letters except
`PractisedItems` and `WarmUps`' choice of kinds: a numbers domain adds a `Number` item kind,
its line in `PractisedItems` and its warm-up kinds.
