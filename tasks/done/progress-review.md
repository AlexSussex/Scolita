# Task: feature/progress-review

> **Status: Done 2026-11-25, released as 0.11.0.** Decision D1
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
- the second only when there is no map progress (0.10.0's file was current, so it is taken
  alone); the letters the done lessons introduced become `Learning` (she met them; no right
  answers are invented);
- saved at once as `progress.json` version 1; `map-progress.json` is left as it is, never
  read again while `progress.json` reads.

A **damaged** `progress.json` is set aside dated (as every file) and rebuilt from both
(`map-progress.json` is stale by then), with a warning in the log: the map keeps the lessons done
and the rocket's stop instead of starting from the first lesson (a deviation from the acceptance
criterion's wording, following `galaxy-map`'s reviewed decision: her place is not thrown
away). Her item counts from the damaged file are in its dated copy, for rescue by hand.

**The damaged copies' cleanup** (the human's rule, `AGENTS.md` → "Progress survives
updates"). Each folder (the tablet's, each profile's) gets a small ledger,
`broken-copies.json` (through `SaveStore`, versioned): per file name with damaged copies, the
app version that set the last copy aside and the days it was read and saved fine since.

- **"A newer version"**: `SaveStore` writes `Application.version` (from `VERSION`) into the
  ledger when it sets a copy aside; a day counts only when the version reading and saving the
  file is newer (`System.Version` comparison). Copies found with no entry (0.10.0's, made
  before the ledger, or after a damaged ledger) are counted as set aside by the version running
  then, so only a later release removes them (review finding 1).
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

### Decisions

- **A damaged `progress.json` keeps her place** (the lessons done and the rocket's stop, from
  `map-progress.json` and the lessons before the one in `lesson-run.json`) rather than starting the
  map from the first lesson, as `galaxy-map`'s review decided for its own file; her item counts and
  learned letters are only in the dated copy (rescue by hand). A deviation from the acceptance
  criterion's wording.
- **An update from 0.10.0** takes `map-progress.json` alone (it was current); the lessons before
  the one in `lesson-run.json` are added only when there is no map progress (0.9.0 and earlier) or
  after damage.
- **"Hai să ne amintim!" is one bubble with the warm-up's instruction**
  (`LessonRun.CurrentInstruction`), so it is shown even where no voice speaks.
- **Counting steps, not taps**, and only warm-ups move the review schedule (Design note).
- **Words** are counted and get a state but no warm-ups (out of scope); `ItemMastered` is raised for
  words too.
- **The damaged copies' days** count only for a file the app reads and saves: the settings files are
  now saved back at start (`BootLoader`), `progress`, `lesson-run` and the map's files as they are
  played; `map-progress.json` is never written again, so copies of it made by 0.10.0 stay until
  removed by hand (a known limit; none are expected).
- **Copies found without a watch** (0.10.0's, or after a damaged ledger) count as set aside by the
  version running: only a later release removes them (review finding 1).

### Fresh-eyes review (2026-11-25, a second agent, before the push)

1. Medium: a copy with no ledger entry counted as set aside by an unknown older version, so the
   version that made it could remove it. **Fixed:** `DamagedCopies.Found` takes the running version
   (`TestRemoveSettledCopies_CopiesFromBeforeTheLedger` proves the same version never removes them).
2. Medium: a folder that can't be listed made the ledger forget everything. **Fixed:** nothing is
   removed or written then.
3. Medium: rebuilding a damaged `progress.json` drops her item counts, and the lessons before the
   one in progress follow the current mix. **Partly fixed:** an update from 0.10.0 takes
   `map-progress.json` alone; the Design note's wording is corrected. **Recorded:** after damage, a
   mix changed from one language to both may mark the other language's lessons before the lesson
   in progress done (as `galaxy-map`'s fallback did); keeping the last good file
   (`progress.previous.json`) to restore from is a follow-up.
4. Low: the settings files were never saved, so their copies never went. **Fixed:** `BootLoader`
   saves them back after a fine read.
5. Low: a warm-up that can't be played used a slot. **Fixed:** `WarmUps` takes the playable steps
   (`TestFor_OnlyStepsThatCanBePlayed`).
6. Low: a warm-up word could come from a later planet. **Fixed:** only words of lessons up to this
   one.
7. Low: rebase onto the rewards commits. **Done**, retested.
8. Low: test names and missing cases. **Fixed:** `TestRecordRight_…`, `TestLessonProgress_…`,
   `TestConstructor_…`, a direct `TestIsReached_NeedsRightsKindsAndDays`; added a newer-version
   ledger test (nothing removed, never written over).

### Pushed

- `c8a2499` `core:` learner progress, mastery, the review schedule, warm-ups, the damaged copies'
  rule (and `LessonRun.CurrentInstruction`, `LessonResume.WithWarmUps`, `CalendarDay.TryParse`);
- `459e46d` `game:` the `progress` save (migrated from `map-progress`), `SaveStore`'s ledger and
  `RemoveSettledCopies`, `BootLoader`'s cleanup, the runners, the rules asset's numbers;
- this Progress; then the close-out (0.11.0).

### Test results

- **Batch mode, rebased on the rewards commits (`a256f28`):** the core commit alone: EditMode
  676/676, PlayMode 26/26; the tip: EditMode 687/687, PlayMode 27/27, `BuildAndroidDebug` exit 0; no
  `warning CS` and no error in any log.
- **Emulator** (`emulator-5600`, AVD `Scolita_Tablet_tracing`, Android 16, 2000 × 1200 by `wm size`):
  - **An update from 0.10.0:** 0.10.0 installed clean, `ro-a`'s first step played, then its map
    progress set to `ro-a`, `en-a`, `ro-m` done with the rocket on `en-m` (pushed, then shown by 0.10.0);
    this build installed over it: the same three stops ticked, the rocket on `en-m`, no trip; the log
    "progress.json is made from the earlier saves (3 lessons done)"; `progress.json` holds the three
    lessons, `rocketStop` `en-m`, a (ro), a (en) and m (ro) met; `map-progress.json` left as it was.
  - **A warm-up on a later day:** a `progress.json` with m (ro) learned the day before (due today):
    **Hai!** opened `ro-p` with "Lesson: ro-p starts with 1 warm-up(s): FirstSound m (ro)" (the picture
    of mama among a, t, m; another start gave `ListenAndPick` with "Hai să ne amintim! Ascultă și atinge
    litera potrivită. Atinge litera m." in one bubble); answered right: the step dots went on to `p`'s
    `MeetLetter`, and m was saved with `reviewStep` 1, `nextReview` three days on, today in `rightDays`
    and `daysPlayed`.
  - **A damaged `progress.json`:** set aside as `progress.broken-20261125-184818.json` with the
    warning, `broken-copies.json` written, the map kept six stops done and the rocket hopped to `ro-p`.
  - **The copies' cleanup over days** (`adb root`, the emulator's date moved 26, 27, 28 November, the
    ledger's `setAsideBy` edited to an older version): one fine day added per day, and at the start
    after the third, "the damaged copies of progress.json were removed (1)"; the ledger empty.
  - **Romanian only:** only the Romanian stops (three ticked, the rocket on `ro-p`); back to both: the
    six stops ticked again (the English progress kept).
  - **Reinstalling the APK over itself** after a force-stop: `progress.json` and the map unchanged.
- **Not run:** the Device Simulator and the Profiler window (batch mode only; the slice adds no
  per-frame work: progress is recorded at a step's end, saved with the step); listening by ear (no
  installed voice on the emulator; the bubble showed the line); the warm-up by playing on two real
  days (a pushed save stood in; the date steps were used for the cleanup). Screens: none new.

## How to test locally

Prerequisites: the worktree's Unity project (`game/`), `UNITY` set to
`C:\Program Files\Unity\Hub\Editor\6000.3.29f1\Editor\Unity.exe`, `ADB` to the SDK's
`platform-tools/adb.exe`, your emulator as `EMU` (a tablet AVD, landscape, started with
`-feature -Vulkan`), a scratch folder as `S`, and
`F=/storage/emulated/0/Android/data/com.softshack.scolita/files/profiles/child-1`.
Start the app with
`"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`
and follow its log with `"$ADB" -s $EMU logcat -s Unity` (the lines below start with `Progress:`,
`Lesson:` or `Scolita:`).

### Core slice

1. `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: every test passes, among them `MasteryTests` (6), `ReviewScheduleTests` (4),
   `LearnerProgressTests` (7), `ItemProgressTests` (3), `PractisedItemsTests` (4),
   `LessonProgressTests` (4), `WarmUpsTests` (8), `ProgressRebuildTests` (2), `ProgressRulesTests` (2),
   `DamagedCopiesTests` (4), and the new cases in `LessonRunTests`, `LessonResumeTests`,
   `LessonVoiceLinesTests`, `CalendarDayTests` and `ContentRulesTests`.
2. What they prove, with a fake clock: 6 right answers across 2 kinds on 2 days make a letter
   learned (6 on one day, or of one kind, don't; English has its own number; practice never
   counts); a learned letter comes back 1 day later, then 3, then 7, and a miss brings it back
   sooner; at most 2 warm-ups, the most overdue first, only letters in the lesson's language and
   her mix, taught before the lesson, with only taught letters as choices; a step done is a right
   answer, a step done with the answer shown is practice; a damaged save's copies go only after a
   newer version read and saved the file fine on 3 different days.

### Game slice

1. The same EditMode run. Expected: `ProgressDataTests` (3: the shape, the round trip, unknown and
   malformed values), `ProgressFileTests` (5: the migration from 0.10.0 and from 0.9.0, a damaged
   file keeps her place, a newer file is never saved over, Romanian only keeps the English
   progress), `SaveStoreCopiesTests` (6), `LearningRulesAssetTests` (2) pass.
2. `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: every test passes, among them `LessonFlowTests.TestLessonRunner_StartsWithAWarmUp`
   (a lesson with m due starts with "Hai să ne amintim!" and a warm-up on m among a, m, t; once
   answered, the lesson's own first step, m one step on in `progress.json`).
3. Build and install: `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`,
   then `"$ADB" -s $EMU install -r game/Builds/Android/Scolita-<version>-debug.apk`.
4. **An update from 0.10.0 keeps her place.** `"$ADB" -s $EMU uninstall com.softshack.scolita`,
   install `C:\Projects\Scolita\local\apks\Scolita-0.10.0-debug.apk`, start it, play `ro-a`
   to its end (or push a `map-progress.json`:
   `{"version":1,"doneLessons":["ro-a","en-a"],"rocketStop":"ro-m"}` to `$F`), then install
   this build over it (`install -r`) and start it. Expected: the map shows the same stops done
   and the rocket where it was, with no trip; the log says
   `Progress: progress.json is made from the earlier saves (2 lessons done).`;
   `"$ADB" -s $EMU shell ls $F` shows `progress.json` beside the untouched `map-progress.json`;
   `"$ADB" -s $EMU shell cat $F/progress.json` lists the done lessons, `rocketStop` and the
   letter a met in both languages (`"state": "learning"`, no rights).
5. **Her answers are counted.** Play `ro-m` to its end. `cat $F/progress.json`: m in Romanian has
   `rights` by kind (`ListenAndPick`, `LetterHunt`, `TraceLetter`, `FirstSound`, one each when
   answered without the answer shown), today in `rightDays`, and today in `daysPlayed`; a step
   finished after the answer was shown counts in `practised` instead.
6. **A warm-up on a later day.** Push a `progress.json` where m in Romanian was learned yesterday
   (due today): `{"version":1,"doneLessons":["ro-a","en-a","ro-m","en-m","ro-t","en-t"],"rocketStop":"ro-p","daysPlayed":[],"items":[{"kind":"letter","id":"m","language":"ro","state":"learned","rights":[{"activity":"ListenAndPick","count":3},{"activity":"FirstSound","count":3}],"practised":0,"rightDays":["<yesterday>","<today - 2>"],"learnedOn":"<yesterday>","reviewStep":0,"nextReview":"<today>"}]}`
   (dates as `yyyy-MM-dd`), start the app and tap **Hai!**. Expected: `ro-p` opens with
   "Hai să ne amintim!" in the bubble, then a warm-up on m (the m among a and t, or the picture of
   mama with m, a, t); the step dots count it; the log says
   `Lesson: ro-p starts with 1 warm-up(s): …`. Answer it right: the lesson goes on with `MeetLetter`
   of p, and `progress.json` shows m with `"reviewStep": 1` and `nextReview` 3 days on.
   **With the clock instead** (the brief's way): make m learned by playing (6 right answers in 2
   kinds on 2 days: play `ro-m`, set the emulator's date a day on with `"$ADB" -s $EMU root` and
   `"$ADB" -s $EMU shell date <MMDDhhmmYYYY.ss>` after `settings put global auto_time 0`, play it
   again; the log says `Progress: letter m (ro) is learned`), move the date one more day on, and
   open the next Romanian lesson: it starts with the warm-up.
7. **A damaged progress file.** `"$ADB" -s $EMU shell "echo '{ not json' > $F/progress.json"`,
   start the app. Expected: the map keeps her place (from `map-progress.json` and
   `lesson-run.json`), the log has `progress.json could not be read … set aside as
   progress.broken-<date-time>.json` and `Progress: progress.json was damaged and set aside`;
   `ls $F` shows the dated copy and `broken-copies.json`, whose `files` list `progress` with the
   version that set it aside (the map keeps the lessons done and the rocket's stop; her letters'
   counts are only in the dated copy). Do it twice: two dated copies.
8. **The copies go after three fine days of a newer version.** Edit `$F/broken-copies.json` so the
   copy was set aside by an older version (`"setAsideBy": "0.10.0"`), start the app and play a
   step on three different days (move the emulator's date as in step 6): each day adds one day to
   `fineDays`; at the next start after the third, the log says `the damaged copies of
   progress.json were removed (n)` and the copies are gone. Copies found with no entry (0.10.0's)
   are watched as set aside by the version running, so only a later release removes them.
9. **Romanian only keeps the English progress.** Push `learning-settings.json` with
   `"languageMix": "ro"`, restart: the map shows only the Romanian stops (the planet parties' English
   steps are left out); push it back to `both`: the English stops done before are still done, and
   `progress.json` still holds the English letters.
10. **Kill and reinstall.** After step 5, `"$ADB" -s $EMU shell am force-stop com.softshack.scolita`,
    `install -r` the same APK again and start it: the map and `progress.json` are as before.

Cleanup: `"$ADB" -s $EMU shell pm clear com.softshack.scolita`; set the date back
(`settings put global auto_time 1`).

## Completion record

_Finished 2026-11-25 by `progress-review`; released as 0.11.0 (tag `v0.11.0`)._

### What was built

- **Pushed:** `c8a2499` `core:`, `459e46d` `game:`, `53c4489` `docs:` (Progress), then this close-out
  (tagged).
- **Core** (`Scripts/Core/Progress`): `LearnerProgress` (items, the lessons done and the rocket's stop
  as `MapProgress`, the days played; the map's `IDoneLessons`; raises `ItemMastered`), `ItemProgress`,
  `LearningItem` / `ItemKind` / `ItemState` / `KindCount`, `Mastery`, `ReviewSchedule`, `ProgressRules`,
  `PractisedItems`, `LessonProgress` (records a `LessonRun`), `WarmUps` (behind `Lessons.IWarmUps`),
  `ProgressRebuild`, `ProgressCodes`. `Scripts/Core/Saving`: `DamagedCopies`, `DamagedCopyWatch`.
  Also `LessonRun.CurrentInstruction`, `LessonResume.WithWarmUps`, `LessonVoiceLines.OfWarmUps`,
  `LessonLines.LetsRemember` (checked by the validator), `CalendarDay.TryParse`.
- **Game:** `Scripts/Game/Progress` (`ProgressData`, `ProgressItemData`, `ProgressCountData`,
  `ProgressFile`); `SaveStore`'s ledger (`DamagedCopiesData`, `DamagedCopyData`,
  `RemoveSettledCopies`, `ProfilesIn`, clock and version overloads for tests); `BootLoader` removes
  settled copies (its `learningRules` field set in `Boot.unity`); `LessonRunner` (progress, warm-ups,
  saving after each step) and `MapRunner` (the progress file); `LearningRulesAsset.ToProgressRules`
  and the asset's new numbers. `MapProgressFile` is gone; `MapProgressData` only reads 0.10.0's file.
- **Tests:** EditMode `MasteryTests`, `ReviewScheduleTests`, `LearnerProgressTests`,
  `ItemProgressTests`, `PractisedItemsTests`, `LessonProgressTests`, `WarmUpsTests`,
  `ProgressRebuildTests`, `ProgressRulesTests`, `DamagedCopiesTests`, `ProgressDataTests`,
  `ProgressFileTests`, `SaveStoreCopiesTests`, and new cases in the lesson, voice, content and asset
  tests; PlayMode `LessonFlowTests.TestLessonRunner_StartsWithAWarmUp`; the lesson PlayMode tests keep
  and clear the Editor's `progress.json` (no warm-up from the Editor's own play).

### Conventions for later briefs

- **Her progress is `LearnerProgress`, read and saved by `ProgressFile` only.** Ask "is this lesson
  done?" through `IDoneLessons`, an item's state through `StateOf` / `TryGet`; the map's rocket stop is
  `progress.Lessons.RocketStop`.
- **A new activity kind** adds its line in `PractisedItems` (which items it practises); a new item kind
  (a letter's name, a number) is an `ItemKind`, its code in `ProgressCodes`, and a raise of
  `ProgressData`'s version.
- **The numbers** of learned, reviews and warm-ups are in `LearningRules.asset`
  (`ToProgressRules()`); English has its own number of right answers.
- **Warm-ups** come from `WarmUps.For(lesson)`, through `LessonResume.WithWarmUps`; the first one's
  instruction starts with "Hai să ne amintim!" (`LessonRun.CurrentInstruction`: say that, not
  `Current.Instruction`, when a step begins).
- **A save file that should have its damaged copies cleaned** must be read and then saved by the app
  (a fine day counts only then); `SaveStore` does the rest.
- **PlayMode tests that open a lesson** keep and delete the Editor's `progress.json` first
  (`LessonScene.RestoreFile(ProgressData.FileName, null)`).

### Decisions and deviations

In Progress → "Decisions" and "Fresh-eyes review": a damaged `progress.json` keeps her place instead of
starting from the first lesson (the acceptance criterion's wording); steps, not taps, count; only
warm-ups move the schedule; words are counted but not reviewed; copies found without a ledger entry go
only after a later release; `map-progress.json` is never written again. The ledger of released ids for
0.11.0 also holds `rewards`' 38 content ids (its stickers, constellations, trails and lines, on `main`
before this close-out and so in this release's APK).

### Follow-ups

- **`parent-area`:** the grid reads `LearnerProgress.Items` / `StateOf` (new, learning, learned per
  letter and language) and `DaysPlayed` (the last two weeks); the language mix switch changes
  `learning-settings.json` (the map and warm-ups follow it, the other language's progress is kept);
  "reset progress" deletes `progress.json` (and `map-progress.json` and `lesson-run.json`, or the
  progress is made again from them).
- **`profiles`:** `progress.json` and `broken-copies.json` are per profile already; `BootLoader`
  cleans every folder under `profiles/`; `LessonRunner` and `MapRunner` still open
  `BuiltInProfiles.FirstChild`.
- **`rewards`:** `ItemMastered` is raised by `LearnerProgress` (a learned letter could earn a sticker
  later); its 38 ids were recorded in 0.11.0's ledger.
- **Later:** word reviews (TraceWord words never traced in a lesson: `content-letters-1`'s question),
  the letter names as their own items once `curriculum-research-names` is agreed, keeping the last good
  `progress.json` (`progress.previous.json`) to restore her counts after damage (review finding 3), a
  mix changed from one language to both before a damaged `progress.json` may mark the other language's
  earlier lessons done.
- **The human:** hear "Hai să ne amintim!" / "Let's remember!"; the 0.11.0 rows of
  `docs/manual-testing.md` (rows 3 to 5 need her to play on a few different days); whether 6 right
  answers on 2 days feels right for her, and whether English needs more.
