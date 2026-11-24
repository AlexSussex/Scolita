# Task: feature/content-ledger

> **Status: Done 2026-11-24, released as 0.3.1.** Both follow-ups were
> answered by the human on 2026-11-24 (content-model's Q2 and Q4).

Read `AGENTS.md` first (especially "Versioning and releases" → "Progress
survives updates"), `content/README.md`, and the completion records
`tasks/done/content-model.md` and `tasks/done/voice.md`.

**Feature:** `feature/content-ledger`
**Implements:** PRD §4 (content as data, stable ids); AGENTS.md
"Progress survives updates"; the follow-ups "A ledger of released ids" and
"Placeholder pictures: a plain circle, no letter" from `content-model`.
**Depends on:** `content-model`, `voice` (both done).

## Scope

1. **A ledger of released ids,** `content/released-ids.txt`: every stable
   id ever released (letters, words, pictures, voice lines, units,
   lessons), each with the CSV that holds it.
   - **The importer refuses** an import where an id in the ledger is
     missing from its CSV (deleted instead of retired), naming the file
     and the id. So a row deleted together with its asset is still caught.
     A retired id is fine; a new id (not in the ledger yet) is fine.
   - **The ledger grows at each release's close-out,** by a step the
     close-out runs (an Editor menu item, with a batch-mode method), written
     into `tasks/README.md` → Lifecycle step 5, `AGENTS.md` → "Versioning
     and releases" and `content/README.md`.
   - **Seeded** with every id released so far (0.2.0 and 0.3.0).
2. **Placeholder pictures are a plain circle, no letter:** the generated
   placeholders no longer show the word's first letter (it gave
   `FirstSound`'s answer away). Re-imported, so the placeholder assets
   change.

**Out of scope:** anything in Core's learning rules; the content itself
(no letter, word or line added); a check in the release build.

**Acceptance criteria:**
- Deleting a released row (with or without its asset) makes the import
  fail with `<file>: the id '<id>' was released …`, and nothing is
  written; retiring it instead imports.
- A new row not in the ledger imports.
- A malformed or duplicated ledger line, or a missing ledger, is an error
  naming `released-ids.txt` and its line.
- Școlița → Record released ids adds the ids not yet in the ledger under
  the version in `VERSION`, and nothing when there are none.
- Every placeholder under `Assets/Scolita/Content/Pictures` is a plain
  coloured circle; the freshness test passes.

**Tests required:** EditMode (a deleted id refused, a retired id fine, a
new id fine, a bad ledger line, recording new ids, the repository's ledger
matches its content, the placeholder has no ink); the full batch run
(EditMode, PlayMode, BuildAndroidDebug). No emulator (no touch, voice or
screen change); the placeholder looked at in the Editor.

**Touches:** `Scripts/Editor/ContentImport/` (the importer),
`Tests/EditMode/Content/`, `content/released-ids.txt` (new),
`content/README.md`, the placeholder picture assets under
`Assets/Scolita/Content/Pictures/`; one line each in `tasks/README.md`
(Lifecycle step 5) and `AGENTS.md` ("Versioning and releases").

**Screens:** none (the placeholders change look, not layout).

**Voice lines:** none.

## Progress

### Design note (small change)

- **Where it lives:** the Editor's importer (`Scripts/Editor/ContentImport/`),
  not Core: the ledger is a rule about importing content, not a learning
  rule. `ReleasedIdLedger` reads `content/released-ids.txt` and checks it
  against the `Curriculum` the importer already builds. `ReleasedIdsRecorder`
  is the close-out's step (menu item and batch method). `ReleasedId` and
  `ReleasedIdsRecord` are its values.
- **The format:** one `<file> <id>` line per id (`words.csv ro-mama`), so the
  same text in two files (the lesson `ro-a`, a word `ro-a`) never clash.
  New ids go at the end under a `# <version>` line, so a rebase conflict
  only ever adds lines on both sides.
- **When it is checked:** `ContentImportPlan.Make` reads the ledger with the
  CSVs, so format errors and a missing ledger are listed with the CSVs'
  errors. It checks for deleted ids only once the CSVs have no error, so a
  row that failed to read never shows up as "deleted" as well.
- **Another domain** (numbers, music) needs nothing new: its units,
  lessons and items are rows of the same files.

### Pushed

- `f776133` `docs: claim content-ledger`.
- `21a40c0` `content:` the seeded ledger (331 ids: 4 letters, 18 words,
  17 pictures, 279 voice lines, 1 unit, 12 lessons, all under `# 0.2.0`;
  0.3.0 released no new ids, checked against the tags `v0.2.0` and
  `v0.3.0`), and `content/README.md` → "The ledger of released ids".
- `8b200b2` `build:` the check, the recorder and their tests.
- `7dd8f3d` `build:` plain placeholders, with the 17 re-imported PNGs.
- `06f1e77` `build:` the review's fixes: refusing to record before
  `VERSION` is raised, the ledger read as strict UTF-8, one error per
  deleted row, Unicode escapes instead of literal characters, and four
  more tests.
- `eb41a34` `content:` how to resolve a conflict in the ledger.
- The `docs:` commit with this Progress, the test steps, and the
  close-out line in `tasks/README.md` and `AGENTS.md`.

### Fresh-eyes review (2026-11-24)

A second agent, which didn't write the code, read the diff against
AGENTS.md before the push. It found no blockers. It checked the seeded
ledger by script against `content/*.csv` (331 ids, none missing either
way) and against the tags. Its findings, and what was done:

1. **The close-out lines in `tasks/README.md` and `AGENTS.md` were
   missing:** added. The AGENTS.md line also says that the close-out's
   `docs:` commit carries `content/released-ids.txt`.
2. **"Keep both sides' lines" on a conflict would make duplicates:** now
   "take `origin/main`'s ledger, raise `VERSION` to the next free number,
   and record again" (content/README.md and AGENTS.md).
3. **The recorder didn't check that `VERSION` was raised:** fixed. The
   ledger reads its `# <version>` lines (`LatestRelease`). Recording new
   ids under a version no later than the latest is an error ("raise
   VERSION first"), and the ledger is left as it was (test
   `TestRecord_RefusesAVersionNotRaised`).
4. **Nits:**
   - **Fixed:**
     - a literal byte order mark and menu label became `\u` escapes;
     - the ledger is read as strict UTF-8, like the CSVs (one shared
       helper);
     - two XML docs said the wrong caller;
     - a deleted released row whose asset is still there now gets one
       error, not two (the asset check runs only when the ledger check
       passed);
     - two tests added: `WithIds` on an unfinished last line, and the
       ledger check skipped when the CSVs have errors.
   - **Kept:**
     - The message has no quotes around the id (the acceptance
       criterion's `'<id>'`): `the id ro-am was released …` reads the
       same, and `content/README.md` shows it that way.
     - Retired units and lessons aren't tested on their own. The
       curriculum keeps them, which the reviewer confirmed, and a
       retired lesson in the small test set breaks "only taught letters".
     - A ledger line deleted together with its row and asset is caught
       only by review (a check in the release build is out of scope).

### Test results (2026-11-24, at the final state, before the rebase)

- **Batch mode:**
  - EditMode 296/296 (was 280);
  - PlayMode 6/6;
  - `BuildAndroidDebug` succeeded (`Scolita-0.3.0-debug.apk`; the first
    try failed because someone else stopped the shared Gradle daemon,
    and the rerun passed);
  - no `warning CS` and no `error CS` in any log.
- **The import** (`ImportFromCommandLine`): exit 0, "imported, with 17
  warnings" (the 17 pictures still on their placeholder). It rewrote the
  17 placeholder PNGs and nothing else.
- **Record released ids** (`RecordFromCommandLine`, `VERSION` 0.3.0):
  exit 0, "every id is in the ledger already; nothing was added"; the
  ledger is unchanged.
- **The placeholders looked at:** `mother.png` and the other re-imported
  PNGs are plain circles in one rainbow colour, with no letter. Opened as
  the image files the Editor imports, not in the Editor's Inspector (no
  Editor window in this session). `ContentFreshnessTests` checks that
  they import as single sprites.
- **Not run:**
  - **The emulator, the Device Simulator and the Profiler:** no touch,
    voice, screen or runtime change.
  - **The by-hand checks on the repository's own content** (delete a
    released row, retire it, record a new id): the session wasn't allowed
    to delete and restore files under `content/`. The same three cases run
    as EditMode tests on a copy in a temporary folder:
    - `TestMake_RefusesAReleasedIdDeletedWithItsAsset`;
    - `TestCheckNoneDeleted_AcceptsARetiredId`;
    - `TestRecord_AddsNewIdsUnderTheVersionOnce`.

### Next

None: closed out as 0.3.1 (see "Completion record").

## How to test locally

### Build slice (the ledger of released ids)

**Prerequisites:** your own worktree of `main`, with the Editor closed for
batch mode. Set `UNITY` to
`C:\Program Files\Unity\Hub\Editor\6000.3.29f1\Editor\Unity.exe`, and use a
scratch folder `<scratch>` outside the repo.

1. **The automated tests:**
   ```bash
   "$UNITY" -batchmode -nographics -projectPath game -runTests \
     -testPlatform EditMode -testResults <scratch>/editmode.xml -logFile <scratch>/editmode.log
   ```
   Expect 296 passed. The ledger's tests are in `ReleasedIdLedgerTests`
   (parsing, deleted, retired, new ids, the repository's ledger) and in
   `ContentImporterTests` (`TestMake_Refuses…`, `TestRecord_…`).
2. **The import still passes:**
   `-executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine`
   exits 0, and the log says "imported, with 17 warnings".
3. **A released row deleted with its asset is refused:**
   - Delete the row `ro-slow-mama` from `content/voice-lines.csv`, and
     `Assets/Scolita/Content/Voice/ro-slow-mama.asset` with its `.meta`.
   - Import: it exits 1 and logs `Content: voice-lines.csv: the id
     ro-slow-mama was released (released-ids.txt, row …), but its row is
     gone: … mark it retired.` Nothing is written.
   - Then `git checkout -- content game/Assets/Scolita/Content`.
4. **Retired instead, it imports:** write `yes` in `ro-slow-mama`'s
   `retired` cell and import: exit 0. Then restore as in step 3.
5. **A new id is recorded once, under the raised version:**
   - Add the row `star,,,` to `content/pictures.csv` and import (exit 0, 18
     warnings).
   - Run
     `-executeMethod Scolita.Editor.ContentImport.ReleasedIdsRecorder.RecordFromCommandLine`.
     The ledger ends with `# 0.3.0` (the current `VERSION`) and
     `pictures.csv star`, and the log says "1 new ids added".
   - Run it again: "nothing was added".
   - Restore with `git checkout -- content game/Assets/Scolita/Content`,
     and delete the new `Pictures/star.*` assets.
6. **`VERSION` not raised:** with a ledger whose last line is `# 0.3.0`
   and a new row as in step 5, recording with `VERSION` 0.3.0 again exits
   1 with "raise VERSION first"; the ledger is unchanged.
7. **A missing or broken ledger:** rename `content/released-ids.txt`, or add
   the line `steps.csv ro-a`. The import exits 1 and names
   `released-ids.txt` (and the row, for the bad line).

Steps 3 to 7 were not run by hand on the repository (see Test results);
the EditMode tests cover each one on a temporary copy.

**Cleanup:** `git status` must be clean under `content/` and
`Assets/Scolita/Content/` afterwards. Nothing is saved on a device.

### Build slice (plain placeholders)

1. After the import, open `Assets/Scolita/Content/Pictures/mother.png` (or
   any picture whose `art` is empty) in the Editor's Project window. It is
   a plain circle in one rainbow colour, with no letter.
2. `PlaceholderPictureTests.TestRender_SamePlainCircleEveryTime` checks
   that every opaque pixel has the circle's colour, and
   `ContentFreshnessTests` checks that the committed PNGs match what the
   importer draws.

## Completion record

_Finished 2026-11-24 by `ledger`; released as 0.3.1 (tag `v0.3.1`)._

### What was built

- **Pushed:**
  - `f776133` the claim;
  - `21a40c0` `content:` the seeded ledger;
  - `8b200b2` `build:` the check and the recorder;
  - `7dd8f3d` `build:` the plain placeholders, with the re-imported PNGs;
  - `06f1e77` `build:` the review's fixes;
  - `eb41a34` `content:` the conflict advice;
  - `88ebd4c` `docs:` Progress and the test steps;
  - the close-out.
- **The ledger**, `content/released-ids.txt`: 331 ids, one `<file> <id>`
  line each, under `# 0.2.0`.
- **In the importer** (`Scripts/Editor/ContentImport/`):
  - `ReleasedIdLedger` reads it and checks it against the CSVs.
    `ContentImportPlan.Make` refuses a released id missing from its CSV.
  - `ReleasedIdsRecorder` (Școlița → Record released ids, or
    `RecordFromCommandLine`) adds the new ids under `VERSION`, and refuses
    a version no later than the ledger's latest.
  - `ContentFolder.ReleasedIdsText` reads it as strict UTF-8.
  - `ReleasedId` and `ReleasedIdsRecord` are its values.
- **Placeholders:** `PlaceholderPicture.Render(pictureId)` draws a plain
  circle in a rainbow colour picked from the id. No letter is drawn and
  the font is no longer read.
- **Docs:**
  - `content/README.md` → "The ledger of released ids";
  - the close-out step in `tasks/README.md` → "Lifecycle" step 5 and in
    `AGENTS.md` → "Versioning and releases".
- **Tests:** EditMode 296 (was 280): `ReleasedIdLedgerTests` (10), four
  more in `ContentImporterTests`, and `PlaceholderPictureTests` rewritten.

### Conventions for later briefs

- **Every close-out**, after raising `VERSION`, runs **Școlița → Record
  released ids** and commits `content/released-ids.txt` in its `docs:`
  commit, even when nothing is added (then there is no change).
- **Never edit the ledger by hand.** On a rebase conflict in it, take
  `origin/main`'s ledger, raise `VERSION` to the next free number, and
  record again.
- **An id that has been released is retired, never deleted:** the import
  names the file and the id.
- **Test content for the importer** needs a ledger file: the tests'
  `RunInTemporaryFolders` writes an empty one unless the test gives one
  under `ReleasedIdLedger.FileName`.

### How it was tested

In Progress → "Test results":
- batch mode at the final state: EditMode, PlayMode and `BuildAndroidDebug`,
  with no warnings;
- the import and the record step in batch mode;
- the placeholders looked at;
- Record released ids run for real at this close-out, with `VERSION`
  0.3.1: "nothing was added".

Reviewed by a second agent before the push (Progress → "Fresh-eyes
review").

Not run: the emulator, the Device Simulator, the Profiler (no screen or
runtime change), and the by-hand delete, retire and record checks on the
repository's own content. The session wasn't allowed to delete and restore
files under `content/`; the EditMode tests cover the same cases on a copy.

### Decisions and deviations

- **The check lives in the Editor's importer,** not in Core: it is a rule
  for importing content, not a learning rule.
- **The ledger names each id's CSV,** so the same text in two files never
  clashes.
- **The placeholder commit carries the re-imported PNGs** with the code
  (`build:`). Otherwise the freshness test would fail between two
  commits.
- **The error message has no quotes around the id** (the acceptance
  criterion showed `'<id>'`).
- **The close-out's `docs:` commit carries `content/released-ids.txt`,**
  as it carries `VERSION`.

### Follow-ups

- **A release-build check** that every current id is in the ledger, so a
  forgotten Record step can't ship. It is out of scope here; small, if
  wanted.
- **A ledger line deleted by hand,** together with its row and asset, is
  caught only by review.
