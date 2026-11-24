# Școlița — Agent Context

Read this file first, every session. It is the source of truth for how this
project is built. If something here conflicts with an instruction in a task,
stop and flag it rather than guessing.

## What Școlița is

A learning game for Android tablets that teaches a young child to **read and
write in Romanian and English at the same time**, in a playful space world:
a rocket flies from planet to planet, each planet holds a few letters, and
every lesson is a short game (hear it, find it, trace it, build a word, read
it). It is made by the human for their daughter, so a child is the main
player: everything must be easy to understand, safe, kind and fun.

- **Reading and writing first:** letters, their sounds and names, tracing
  them with a finger in print **and by hand** (the handwritten "litere de
  mână" taught at school), then syllables, words written by hand and short
  sentences, in both languages.
- **Both languages at once,** each always clearly marked, with Romanian as
  the anchor (decision D1) and English beside it; letters and words that are
  alike in both ("rachetă / rocket", "pian / piano") are the bridges.
- **Play first:** space, rainbows, unicorns, the moon and the Earth, stars,
  rockets, animals, music and drawing are the world the lessons live in.
- **Other domains later** (numbers, music and instruments, space and
  animals, drawing): the lesson engine and the content format are
  domain-agnostic from day one.
- **Fully offline.** No account, no internet, no ads, no purchases, no
  analytics.

- Full spec (PRD): docs/prd.md
- How the child learns (letter order, words, activities): docs/curriculum.md
- Build order + local-dev setup: docs/mvp-build-plan.md
- The look (the purple space world, the mascot, the language badges):
  design/README.md; each screen: design/screens.md; the design canvas:
  https://claude.ai/artifact/RVfBkheGcrfB824aq4cEEf

The name is **Școlița** in text (with `Ș` comma-below, U+0218, and `ț`,
U+021B), `scolita` in package ids, folders and file names, and `Scolita` in
C# namespaces.

## Stack (locked — do not substitute)

| Layer | Choice |
| --- | --- |
| Engine | Unity 6.3 LTS (`6000.3.29f1`, the version SeliSnake uses on this PC), Unity Personal |
| Language | C# (Unity's own .NET, the language version Unity 6.3 supports) |
| Rendering | Universal Render Pipeline (URP) with the 2D Renderer; sprites, sprite atlases, particles (the Built-in Particle System), 2D lights only if a brief asks |
| Look | Bright 2D with soft shading in a purple space world (design/README.md, design/screens.md) |
| Input | Input System package (touch), Enhanced Touch API; tracing reads raw touch positions |
| UI | Unity UI (uGUI) + TextMeshPro, with a font atlas that covers ă â î ș ț and their capitals |
| Text | Unity Localization package: the app's own words (buttons, the parent area) in Romanian and English |
| Voice | Recorded clips (the family's voices) first, Android's system TextToSpeech as the fallback, behind `IVoice` |
| Content | Lesson content authored as CSV in `content/` (the source of truth), imported by an Editor importer into ScriptableObjects listed in catalogs |
| Saving | One JSON file per concern in `Application.persistentDataPath`, through `SaveStore` only, per child profile |
| Tests | Unity Test Framework (NUnit): EditMode for Core and the content, PlayMode for scenes |
| Build | Unity batch-mode build script (`Scolita.Editor.BuildScript`) → APK, installed with `adb` |
| Audio tool | ffmpeg (an LGPL Windows build, pinned, found through `SCOLITA_FFMPEG`), for the recording kit in the Editor only (reading M4A takes, writing OGG clips); never in the repo or the APK (agreed by the human 2026-11-24) |
| VCS | Git + Git LFS, GitHub, trunk-based on `main` |

- Android package id: `com.softshack.scolita`; product name `Școlița`
  (the launcher label), `Scolita` where a file name needs ASCII.
- C# root namespace: `Scolita` (`Scolita.Core`, `Scolita.Game`, …).
- Target: Android tablets, landscape (left and right); the human's
  Samsung Galaxy Tab A7 10.4 (SM-T503, Android 12, a low-end tablet) is the
  reference device; minimum API 26 (Android 8.0).
- **No new engine package** beyond those `project-skeleton` installs
  without the human's agreement (see "Do not, without asking").

## The five seams (must stay intact)

These keep the later steps (more letters, joined capitals, sentences, other
domains, a second child, more voices) from becoming rewrites. Do not
collapse them.

1. **The learning Core apart from presentation.** Everything about
   learning lives in `Scolita.Core`: plain C# with **no reference to
   UnityEngine** (its assembly definition sets `noEngineReferences: true`):
   the curriculum graph, a lesson's steps, every activity's rules (is this
   answer right, how close is this trace, what hint comes next), progress,
   mastery, the review schedule and the rewards. Core takes randomness from
   a seeded `IRandom` and time from an `IClock`, never from
   `UnityEngine.Random` or `Time`. Unity code (`Scolita.Game`, `Scolita.UI`)
   draws Core's state and forwards the child's taps and strokes. This makes
   every learning rule testable without a scene.
2. **Content as data, domain-agnostic.** A domain (reading and writing now;
   numbers or music later) is a tree of data: `Domain → Track → Unit →
   Lesson → Step`, where a step names an activity kind and the items it
   uses (letters, words, pictures, sounds, notes later). Items live in
   catalogs (`LetterCatalog`, `WordCatalog`, `PictureCatalog`, …). Adding a
   letter, a word, a lesson, a unit or a whole domain is data and art,
   never code. The content validator (an EditMode test) checks all of it.
3. **One activity contract.** Each activity kind (`ListenAndPick`,
   `TraceLetter`, `BuildWord`, …) is one Core rules class implementing
   `IActivityRules` (pure: it takes the step's items and the child's
   answers, and says right, try again with a hint, or done) and one Unity
   view prefab implementing `IActivityView`, found through
   `ActivityRegistry` by its `ActivityKind`. Lessons never know how an
   activity is drawn; a new domain reuses the kinds it can and adds a kind
   only when no existing one fits.
4. **One voice interface.** Everything the app says goes through `IVoice`
   (`Speak(VoiceLineId)`), with `RecordedVoice` (clips from the content),
   `AndroidTextToSpeechVoice` (the system voice, through `AndroidJavaObject`,
   for lines with no clip yet) and `SilentVoice` (tests). Lessons name what
   to say, never how it is said, so recording a clip later never changes a
   lesson.
5. **Saves are per profile.** Every save file about a learner lives under
   its profile's folder (`profiles/<profileId>/`): progress, rewards, the
   lesson in progress, the profile's learning settings. Only the profile
   list and the tablet's own settings (volumes, vibration, the app's
   language) sit outside. Several children and a tester profile are in the
   MVP (`profiles`); Core never assumes there is only one, and nothing a
   profile does may change another profile's files.

## Repo layout

```
/                 AGENTS.md, README.md, VERSION, CHANGELOG.md,
                  .gitignore, .gitattributes
/content          the lesson content's source of truth, as CSV
                  (letters.csv, words.csv, lessons.csv, voice-lines.csv, …)
                  and its art and audio sources (in Git LFS)
/game             the Unity project (open this folder in Unity Hub)
  /Assets/Scolita
    /Scripts/Core     Scolita.Core: learning rules, no UnityEngine
    /Scripts/Game     Scolita.Game: the composition root, activity views,
                      voice, saving, scene flow
    /Scripts/UI       Scolita.UI: the galaxy map, menus, HUD, parent area
    /Scripts/Content  Scolita.Content: ScriptableObject types and catalogs
    /Scripts/Editor   Scolita.Editor: build script, content importer
    /Tests/EditMode   Scolita.Tests.EditMode (Core, content validator)
    /Tests/PlayMode   Scolita.Tests.PlayMode
    /Content          imported assets: Domains/, Letters/, Words/,
                      Pictures/, Voice/, Stickers/ (generated by the
                      importer from /content; never edited by hand)
    /Settings         render pipeline and input settings
    /Art /Audio /Fonts /Prefabs /Scenes /Localization
  /Assets/Plugins/Android  the launcher manifest (no INTERNET)
  /Assets/ThirdParty  bought or free packs, each with its LICENSE.txt
  /Packages /ProjectSettings
/docs             prd.md, curriculum.md, mvp-build-plan.md, manual-testing.md
/design           README.md: the look, palettes, the mascot, the badges;
                  screens.md: each designed screen; canvas/: the design
                  canvas's source (one .dc.html per screen, canvas.json)
  /research       research notes (art prompts, themes)
/tasks            one brief per feature (see tasks/README.md)
  /pending        briefs not yet finished, including the ones in progress
  /done           finished briefs, each with a completion record
/local            the human's own notes (never committed, see below)
```

## Workflow

**Trunk-based from the first commit.** All work goes straight to `main`.
There are no feature branches on GitHub.

- **Each brief is one feature** (`tasks/pending/<feature>.md`). The
  `feature/<name>` in a brief's title names the feature, not a branch.
- **Core first, then the Unity side.** Push the `core:` commits (with their
  tests) before the `game:` / `ui:` commits that use them, so `main` always
  builds and works. Content follows the code that reads it.
- **Every push to `main` is tested first by the agent pushing it**: the
  automated tests, the Android build, and the brief's "How to test locally"
  steps in the Editor and on the emulator (see "Testing before a push").
  Nothing untested reaches `main`.
- **`main` is always playable.** Each commit compiles, passes the tests and
  starts the app. Don't push a planet, a button or a lesson that leads
  nowhere; if a feature takes several commits, push its entry point last.
- **Never force-push, and never rewrite `main`'s history.**
- Never commit secrets, keystores, keystore passwords or `.env` files.

## Commits

- **One area per commit**, named by a prefix:
  - `core:` for `Scripts/Core` and its EditMode tests;
  - `game:` for `Scripts/Game`, scenes, prefabs and PlayMode tests;
  - `ui:` for `Scripts/UI`, menus, the galaxy map, the parent area and
    string tables;
  - `content:` for `content/` (CSV, art and audio sources), the imported
    assets under `Assets/Scolita/Content`, art, audio, fonts and
    `ThirdParty/`;
  - `build:` for `ProjectSettings/`, `Packages/`, `Scripts/Editor`,
    `Plugins/Android`, `.gitignore`, `.gitattributes`;
  - `docs:` for `docs/`, `design/`, `tasks/`, `README.md`, `AGENTS.md`,
    and `CHANGELOG.md` and `VERSION` in a close-out.
- **Short, concise, to the point:** lowercase, at most about 72 characters,
  no full stop. Say what changed, not how. Examples:
  - `core: add review schedule for learned letters`
  - `content: add planet m words in romanian and english`
  - `docs: claim tracing`
- **The brief's progress goes in the same commit** as the work it
  describes.
- **A Unity asset and its `.meta` file always travel together.** Never
  commit one without the other, never hand-edit a GUID, and never delete a
  `.meta` to "fix" something.
- **A content CSV and its imported assets travel together:** a commit that
  changes `content/*.csv` also carries what the importer made from it, so
  `main` never has stale content (the importer's freshness test fails
  otherwise).

## Several agents at once

Several agents may work in parallel, each on its own brief. They coordinate
only through the repo, so the docs must always say who is doing what.

### Claiming work

1. **Fetch `main` and read `tasks/README.md` → "In flight".** Pick a brief
   nobody holds whose "Touches" don't overlap anything in flight. If every
   free brief overlaps, wait or ask the human.
2. **Claim it in one small commit:** add a row to "In flight" (agent name,
   brief, what it touches, emulator, start date) and set the brief's status
   line to "In progress (<agent>)". Commit `docs: claim <feature>` and push
   at once. If the push is rejected, rebase, re-read "In flight", and pick
   again if someone took the same brief.
3. **Release it when done:** the last commit moves the brief to
   `tasks/done/` with its completion record and removes the row.

### Keeping progress visible

- **Each brief has a `## Progress` section**, below the brief text and above
  "How to test locally". Only the agent holding the brief edits it: what's
  pushed (with commit hashes), what's in progress, what's next, anything
  blocked. Update it with every push.
- **`tasks/README.md` changes stay small:** your own "In flight" row, the
  index row for your brief, and follow-ups you add. Don't reflow the file.
- **A question for the human** goes in the brief's Progress section as
  "Blocked: …", in your "In flight" row and in
  `local/for-human-next-steps.md`, so it isn't lost when your session ends.
- **A fresh-eyes reviewer** is a second agent for the length of one review.
  It doesn't claim a row; the author's row says "in review by <agent>"
  until the findings are resolved.

### Avoiding conflicts

- **Work in your own git worktree,** never in the human's checkout or
  another agent's: `git worktree add ../scolita-<agent> -b agent/<agent>
  origin/main`. The `agent/<agent>` branch is local only and is never
  pushed. Each worktree is its own Unity project: its first import builds
  its own `Library/` (several minutes; never copy another's `Library/`).
- **Never use `git stash`** (the stash is shared by every worktree).
- **Before every push:** `git fetch`, `git rebase origin/main`, then rerun
  the tests. Push with `git push origin HEAD:main`. A rejected push means
  repeating this, never forcing.
- **Unity files that can't be merged by hand.** Scenes (`.unity`), prefabs
  (`.prefab`), ScriptableObject assets (`.asset`) and `ProjectSettings/`
  are YAML, but merging two agents' edits to the same one usually breaks
  it. So:
  - **every scene, prefab and catalog you will change is listed in your
    claim's "Touches"**, and two briefs that touch the same one go one
    after the other;
  - prefer a new prefab over a change to a shared scene; scenes hold
    little more than their root prefabs;
  - a conflict in one of these files is resolved by taking the other
    side's version and redoing your change in the Editor, never by editing
    the YAML;
  - **a conflict in an imported content asset** is resolved by taking
    either side and rerunning the importer on the merged CSV, never by
    merging the asset.
- **Files everyone edits:** `Packages/manifest.json` and
  `packages-lock.json`, `ProjectSettings/*`, `VERSION`, `CHANGELOG.md`, the
  content CSVs under `content/` and their catalogs, the string tables under
  `Localization/`, `ActivityRegistry`, and the `Boot` scene. Change them in
  small, separate hunks (new rows at the end of their group, never
  reordering or reformatting existing rows) and push soon after, so others
  rebase onto your change early.
- **Resolving a conflict:** keep both sides' intent; if that's unclear, stop
  and ask the agent holding the other brief (through the docs) or the human.
  Rerun the tests after resolving.

### Shared machine resources

The PC also holds SeliSnake and Skillr, with their own agents. Everything
below applies across all three projects.

- **One Unity Editor per worktree, and only your own.** Never open the
  human's checkout (`C:\Projects\Scolita`) or any other project's
  checkout in an Editor and never run batch mode on it. Batch mode can't
  run on a project that is open in an Editor (`Temp/UnityLockfile`): close
  your own Editor first, or test with the Editor's Test Runner instead.
- **The Unity licence is the human's** (Unity Personal, activated in Unity
  Hub). Never sign in, sign out, return or change the licence.
- **One Android emulator per agent**, on a port no other agent of any
  project uses (`emulator-5600`, `emulator-5602`, …; SeliSnake's agents
  start at 5580, Skillr's at 5554), written in your "In flight" row. Create
  your own AVD once, `Scolita_Tablet_<agent>`: a tablet profile (for
  example "Pixel Tablet"), an `x86_64` Google APIs image of Android 12 or
  newer, landscape, launched with `-e unity -force-gles` (the emulator
  crashes under Vulkan). Always target it with `adb -s`. **Never touch the
  human's tablet or phone**, even if it shows in `adb devices`.
- **Scratch files in your own folder** (`<scratchpad>/<agent>/`): helper
  scripts, screenshots, test results, built APKs, voice test clips. Never
  at a shared root, never in the repo.
- **Never kill Unity, Gradle, Java or adb processes you didn't start**, and
  never run `adb kill-server` or `./gradlew --stop`: other agents'
  Editors, builds and emulators share them.
- **No cleanup beyond your own work** without asking: remove only your own
  worktree, AVD and scratch files, by name.

## Session usage (from Skillr, the human, 2026-11-16)

Check your session's usage of its quota (Claude Code's `/usage`, or the
status line) when you start a task and before each large step, then act on
the range it is in:

| Usage of the quota | What to do |
| --- | --- |
| Under 70% | Work as usual. |
| 70% to 80% | **Don't start any large workflow:** no new brief or slice, no large change, no fresh-eyes review, no full emulator test pass. Small steps that finish quickly are fine. |
| 80% to 90% | **Finish what you're working on, then write a self-contained handoff** (below). Start nothing new. |
| 90% to 100% | **Pause.** Write the handoff if it isn't written yet, then schedule a wake-up for when the quota resets, to continue from the handoff. |

- **A self-contained handoff** is enough for a fresh agent with no other
  context to carry on. It goes in the brief's `## Progress` section, with a
  pointer in your "In flight" row: what is done and pushed (commit hashes),
  what is done but not pushed (worktree, uncommitted files), the exact next
  step, the commands to open, test and build, and any open question.
- **Never leave `main` half-done:** an untested slice stays unpushed, and
  the handoff says so.

## Testing before a push

The agent pushing is the tester. Before each push of anything but docs:

1. **Automated, in batch mode** (close your Editor first), from the
   worktree root, with `UNITY` set to
   `C:\Program Files\Unity\Hub\Editor\6000.3.29f1\Editor\Unity.exe`:
   ```bash
   "$UNITY" -batchmode -nographics -projectPath game -runTests \
     -testPlatform EditMode -testResults <scratch>/editmode.xml -logFile <scratch>/editmode.log
   "$UNITY" -batchmode -projectPath game -runTests \
     -testPlatform PlayMode -testResults <scratch>/playmode.xml -logFile <scratch>/playmode.log
   "$UNITY" -batchmode -nographics -quit -projectPath game \
     -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug \
     -logFile <scratch>/build.log
   ```
   All tests pass, the build succeeds, and the logs show **no new compiler
   warnings and no errors** (search them for `warning CS` and `error`).
   The content validator and the importer's freshness test run with the
   EditMode tests.
2. **The brief's "How to test locally" steps, run for real:**
   - **in the Editor's Play mode with the Device Simulator**, on the screen
     sizes below;
   - **on your emulator**, with the debug APK installed
     (`adb -s <emulator> install -r <scratch>/Scolita-debug.apk`), for
     anything touching touch input or tracing, the voice, screen size,
     saving, the app's lifecycle (pause, resume, rotation) or
     performance-sensitive code. The emulator is slower than a tablet:
     judge function there, not smoothness.
   - **Tracing with a mouse is not enough:** a change to tracing is also
     checked on the emulator with drawn strokes (`adb shell input swipe`
     chains or `motionevent`), including a stroke drawn backwards, one that
     leaves the letter, and two fingers at once.
   Record the results in the brief.
3. **Screen sizes** (for a change that adds or moves anything on screen):

   | Size | Device Simulator / AVD | When |
   | --- | --- | --- |
   | The Tab A7, 10.4", 5:3 | 2000 × 1200 | Every change on screen |
   | 10" tablet, 16:10 | 1280 × 800 | Every change on screen |
   | Large tablet | 2560 × 1600 | New screens and activity layouts |
   | 4:3 tablet | 2048 × 1536 | New screens and activity layouts |
   | Phone, landscape | 2400 × 1080 | New screens (it must work, it needn't shine) |
   | A display cut-out | a simulator profile with a notch | HUD and menu edges |

   Take a screenshot of each size checked (in your scratch folder, never
   the repo) and say in the brief which sizes were checked and why that was
   enough.
4. **Listening** (for a change that adds or changes what the app says):
   play every new voice line in the Editor in both languages, check it is
   the right line, in the right language, not cut off, and as loud as its
   neighbours (see "Audio"). Say in the brief which lines were heard.
5. **Performance** (for changes to activity views, effects, the galaxy map
   or content counts): run the brief's worst case on the emulator and in
   the Editor's Profiler and record in the brief: frame time, **GC
   allocations per frame while an activity runs (must be 0 B)**, and the
   time from tapping a planet to the first activity on screen.
6. **After rebasing onto newer `main`,** repeat the steps the rebase could
   have affected.

The human's own tablet is the final check: the release's rows in
`docs/manual-testing.md` (see "Versioning and releases"), and the child's
own reactions.

## Local test instructions (required for every slice)

After implementing a slice and before pushing it:

1. **Append a `## How to test locally` section to the brief** in
   `tasks/pending/<feature>.md`, below the original brief text, with one
   `### <Core|Game|UI|Content|Build> slice` subsection per slice. Don't edit
   the brief text above it.
2. **Write it for someone who wasn't in the session.** Include:
   - **Prerequisites and start-up:** which scene to open, which profile and
     settings to choose, the exact batch-mode and `adb` commands.
   - **Step-by-step checks:** each with what to do (in Play mode or on the
     emulator, at which screen size, in which language) and what should
     happen, and what should be heard.
   - **The unhappy paths** the acceptance criteria name (a wrong answer
     twice, a trace that leaves the letter, the app paused mid-lesson, a
     missing voice clip, an empty unit, a corrupt save file).
   - **How to run the automated tests,** and where the results are.
   - **Cleanup / reset:** how to clear the save files
     (`adb -s <emulator> shell pm clear com.softshack.scolita`).
3. **Run the steps yourself first**, so the expected results are real, not
   guessed. Say plainly which steps you could not run.
4. **Summarise the steps in the hand-off message,** pointing to the section
   in the brief for the full version.

When the brief is finished the section stays in place, above the completion
record, so `tasks/done/` keeps a runnable test script for every feature.

## Versioning and releases

- **One version for the app,** in `VERSION` at the repo root:
  `MAJOR.MINOR.PATCH`, each part 0–99 without leading zeros. It is the
  only place the number is written. `BuildScript` reads it into
  `PlayerSettings.bundleVersion` and the Android `bundleVersionCode`
  (`major * 10000 + minor * 100 + patch`, so 0.4.2 is 402; at least 1),
  and the parent area shows it. A malformed `VERSION` stops the build.
- **Numbering before 1.0.0:** a feature raises MINOR (0.4.0 → 0.5.0); a fix,
  a chore or new content alone raises PATCH (0.5.0 → 0.5.1). 1.0.0 is the
  human's call.
- **A brief that changes no app files** (research, docs only) doesn't
  release.
- **A version per finished brief.** The close-out commit (the `docs:` commit
  that moves the brief to `tasks/done/`) raises `VERSION` and adds the
  brief's entry at the top of `CHANGELOG.md`. The commits before it touch
  neither.
- **The changelog entry:** `## <version> (<date>)`; a short summary in plain
  words; what changed for the child and for the parent, in bullets; then
  `### Technical notes`: new packages, save-format changes, content added
  (letters, words, voice lines), anything the human must do, and the link
  to the brief.
- **The manual test steps:** the close-out commit also adds the release's
  section at the top of `docs/manual-testing.md` (under "The basics"): a row
  per thing the child or the parent can see or hear, naming the screen, what
  to tap, and what should happen, with an empty "Tested on" cell. No tools
  needed (no adb).
- **The tag, after the push:** `git tag -a v<version> -m "Școlița
  <version>"` on exactly the close-out commit, then `git push origin
  v<version>` (never `--tags`). A pushed tag is never moved, deleted or
  reused.
- **Two agents, one number.** Take the next number from `origin/main` while
  writing the close-out; after `git rebase origin/main`, a conflict in
  `CHANGELOG.md` or a `VERSION` that no longer shows your bump means
  someone released first: take the next number and rerun the checks.
- **Release APKs** are built from a tag with
  `Scolita.Editor.BuildScript.BuildAndroidRelease`, signed with the human's
  Școlița release key (README → "Build a release APK"). Agents build debug
  APKs only; the release key and its passwords are never on an agent's
  command line.
- **Progress survives updates:** her progress is the most precious data in
  the app. A change to a save file's shape keeps reading the previous shape
  (a `version` field in every save file, and a migration step in
  `SaveStore`), and a change to the content never loses progress: letters,
  words and lessons have **stable ids** that are never reused or renamed
  (a retired item keeps its id, marked retired). A save that can't be read
  is set aside as `<name>.broken-<date-time>.json` (each damaged copy kept,
  so progress can be rescued by hand; the human, 2026-11-24) and the
  defaults are used, never a crash.

## Task briefs

The current state of the project is in `tasks/README.md` → "Where we are".
Keep your own parts of it up to date with every push.

Each feature's brief lives in `tasks/pending/<feature>.md` while it is being
built. When its last commit is pushed it moves to `tasks/done/<feature>.md`
with a completion record appended (see `tasks/README.md`). Before starting a
feature, read the completion records in `tasks/done/`: they hold the
conventions and follow-ups earlier briefs left for later ones. Template:

```
Feature: feature/<name>
Implements: <PRD section(s)>, <curriculum.md section(s)>
Depends on: <briefs that must be done first>
Scope: <what to build>
Out of scope: <what NOT to touch>
Acceptance criteria:
  - <observable behaviour>
Tests required: <EditMode / PlayMode / emulator / Profiler / listening>
Touches: <folders, scenes, prefabs, catalogs, CSVs it changes>
Screens: <screens it adds or changes: each gets the screen-size check>
Voice lines: <lines it adds, in both languages, and whether clips exist>
```

## The human's next steps (local file)

`local/for-human-next-steps.md` in the human's checkout
(`C:\Projects\Scolita\local\`) is the **single, ordered list of what the
human has to do next**, and which brief each step belongs to. `local/` is
ignored through `.git/info/exclude`, so the file is never committed; agents
working in their own worktrees edit it by its absolute path.

- **Keep it current.** Update it in the same step as anything that adds,
  changes, unblocks or finishes something only the human can do: an install,
  a setting, a test on the tablet, an APK to install, voice lines to record,
  art to choose with the child, a decision.
- **Its shape:** "Now: what needs you, in order" at the top holds only
  active steps; a finished step is **crossed out and moved to "Done" at the
  bottom** (newest first, with its date), never deleted. Open questions go
  in "Open decisions" until answered, then to "Done". Work that will need
  the human soon goes under "Coming soon".
- **Each step says** what to do, the exact file or page to open, why now and
  what it unblocks, and links to the detailed guide (`local/todo/`, the
  README, a brief).
- **Update the "Last updated" line** with the date and the latest release.
- **Never write a secret in it** (keystore passwords, tokens); name the file
  or password-manager entry where it goes instead.
- **Never write the child's name, photo or voice in the repo.** Her
  drawings, recordings of her and anything that names her stay in
  `local/`; only assets made from them, named by what they show, go in the
  repo.

## Definition of done (per slice)

- The five seams are intact; Core still has no UnityEngine reference.
- Tests written and passing: EditMode for every Core rule, PlayMode for
  every new scene flow; the content validator and the importer's freshness
  test pass.
- No deferred feature pulled forward (see the PRD's roadmap).
- The Android debug build succeeds with no new warnings; the slice was run
  in the Editor and on the emulator, on the screen sizes in "Testing before
  a push".
- 0 B of GC allocation per frame while an activity runs, measured when the
  slice touches activity views, effects or the map.
- Every new line the app says was heard in both languages.
- Rebased onto the latest `main` and retested before the push.
- The brief's Progress section and your "In flight" row are up to date.
- Changed screens follow "Usability (a child is the player)"; changed
  lessons and activities follow "Learning (a child is the learner)".
- `local/for-human-next-steps.md` reflects anything the human now has to do.
- The diff was self-reviewed against "Code quality, architecture,
  performance and security"; a medium or large change has its Design note.
- A change listed under "Fresh-eyes review" was reviewed by a second agent.
- A `## How to test locally` section for the slice is in the brief, with
  verified expected results.
- When the feature is finished: brief moved to `tasks/done/` with its
  completion record, the index in `tasks/README.md` updated, the "In flight"
  row removed; in the same close-out commit, `VERSION` raised, the
  `CHANGELOG.md` entry and the `docs/manual-testing.md` section added; after
  its push, the tag `v<version>` pushed.

## Do not, without asking

- Add a reference to UnityEngine in `Scolita.Core`, or learning rules to a
  MonoBehaviour.
- Add a deferred feature "while you're in there".
- Add a package, an Asset Store pack or a library that isn't in the stack.
- Add anything that goes online: analytics, ads, purchases, crash
  reporting, Unity Gaming Services, remote config, downloadable content, a
  web request of any kind. The Android `INTERNET` permission stays off.
- Use the microphone or the camera (recording her own voice is a later
  idea that needs the human's agreement and a brief of its own).
- Collect, store or send anything personal. A profile's name and picture,
  if any, stay on the tablet.
- Change a stable content id, or reuse one.
- Change the curriculum's order or the teaching approach in
  `docs/curriculum.md` (the human agrees those).
- Commit any secret, keystore or password.
- Force-push, rewrite `main`'s history, or push without the full test run.
- Push to a branch other than `main` on GitHub (the exception is a version
  tag on a close-out commit).
- Upgrade Unity or a package version (that is a brief of its own, with the
  human's agreement).

## Code quality, architecture, performance and security

Every change is judged on all four, not only on whether it works. Choose
the solution that stays right long term over the quickest one. Ask if the
trade-off isn't clear.

### Architecture

Look at every feature from an architectural view, as deep as its size calls
for, so Școlița can grow (all the letters, free handwriting, sentences and stories,
numbers, music, a second child) without a major refactor.

- **Small change** (a value, a label, a word, a fix): follow the existing
  patterns and put logic where it already lives.
- **Medium change** (a new activity kind, screen, system or content type):
  before coding, write a short **Design** note in the brief's Progress
  section: where the logic lives (Core or Unity side), the data it needs,
  how it is tested, and how it would serve another domain (numbers, music).
- **Large change** (a new domain, a change to the content model or the
  lesson engine, anything across several features): the Design note also
  compares the options and why one was picked, and the human agrees it
  before code is written.
- Always: keep the five seams intact; data-driven content; one owner per
  rule (one Core system owns mastery, one owns the review schedule, one owns
  rewards); no layers or abstractions before something needs them.

**How the app is put together:**

- **Core** holds the `Curriculum` (built from the content), the
  `LearnerProgress` (per profile), and a `LessonRun` that steps through a
  lesson's steps, each run by its `IActivityRules`. Core raises events
  (`AnswerChecked`, `HintGiven`, `StepCompleted`, `LessonCompleted`,
  `ItemMastered`, `RewardEarned`) that the Unity side uses for sound,
  effects and the voice.
- **`LessonRunner`** (the composition root of the Lesson scene) builds a
  `LessonRun` from the chosen lesson and the profile, picks each step's
  view through `ActivityRegistry`, and passes answers and strokes to Core.
  It is the only place that wires a lesson together: no
  `FindObjectOfType`, `GameObject.Find`, `SendMessage` or service locators
  anywhere; references go through serialized fields or the constructor.
- **MonoBehaviours are thin**, like controllers: they read touch, draw
  state, play the voice and forward events. No learning rule lives in a
  MonoBehaviour.
- **No mutable static state** (it survives between plays in the Editor and
  breaks the next run). No singletons beyond what Unity itself provides.

### Quality

- Small, single-purpose classes and methods; names that make comments
  unnecessary.
- Immutable by default: `readonly` fields, `readonly struct` for values,
  get-only properties.
- Handle every error on purpose: no empty `catch`, no swallowed
  exceptions; impossible states fail loudly (an exception in Core, an error
  log on the Unity side). A missing voice clip or picture is never an
  impossible state on a tablet: it falls back (the system voice, a
  placeholder picture) and logs a warning; the content validator makes it
  impossible in a release.
- No dead code, commented-out code or `TODO`s. A follow-up goes in the
  brief instead.
- Zero new compiler warnings, and no warnings or errors in the Console
  during the brief's checks.
- **Numbers the child or the parent meets** (how many tries before the
  answer is shown, how many correct answers make a letter learned, the
  session limit choices) come from one place (the learning rules asset)
  and the UI reads them from there.

### Performance (the Tab A7, 60 frames a second)

- **No allocations while an activity runs.** Nothing in `Update`,
  `LateUpdate`, a tracing stroke or an effect allocates: no LINQ, no `new`
  of classes, no boxing, no string building, no lambdas capturing state.
  Collections are pre-sized and reused; a stroke's points go into a reused
  buffer. LINQ is fine in menus, loading, the importer and the Editor.
- **Pools, not Instantiate/Destroy,** for stars, stickers, letter tiles,
  particles and stroke segments during an activity.
- **Draw calls:** each planet's and each activity's sprites in one sprite
  atlas; particles capped per effect; check the Frame Debugger when a brief
  adds a new kind of thing on screen.
- **Audio:** voice clips load per lesson (Addressables are not in the
  stack: clips are loaded from the lesson's catalog entries and unloaded
  after), music streams, effects are decompressed on load.
- **Budgets** (checked on the human's tablet before each release, by the
  human; on the emulator and in the Profiler by agents): 60 fps on the
  galaxy map and in every activity; a tap answered (sound or movement)
  within 100 ms; a lesson ready within 2 seconds of tapping its planet; the
  app ready within 5 seconds of tapping its icon; the APK under 150 MB.

### Security and privacy (a child's app)

- **Offline by design:** no network code; no internet permission; no
  third-party SDKs; Unity's own analytics and services off in
  `ProjectSettings`. `BuildScript` checks every APK for `INTERNET` and
  fails if it is there.
- **No personal data:** no account, no email, no location, no camera, no
  microphone, no contacts. Android permissions are only those a brief
  names, with the reason.
- **The system voice** (`TextToSpeech`) is used through Android's own
  service, which needs no permission; agents never ask the tablet to
  download a voice.
- **Saves are local JSON** and are validated on load (a value out of its
  range is clamped, an unknown letter or lesson id is kept but ignored, so
  a later version can still use it).
- **The parent area is behind a parent gate** (`ParentGate`), the only way
  to change settings, manage profiles, reset progress, open the tester
  profile or see the version; a child can't open it by tapping around.
- **Profiles hold almost nothing personal:** a name typed by the parent
  and a built-in avatar, kept on the tablet only. No photos, no birthday,
  no age, no surname field.
- **The tester profile** is for the human only: opened from the parent
  area, never listed for the children. Its unlocks, skip button and debug
  overlay exist only inside it; nothing in a child profile can turn them
  on.
- **Dependencies:** a new package only with a reason in the brief and the
  human's agreement, at an exact version.

### Usability (a child is the player)

The player can't read yet, or reads little, so the app must explain itself.
From SeliSnake, with what a learning app adds:

- **Every instruction is spoken,** and can be heard again by tapping the
  mascot or a "listen" button. Nothing important is said only in text;
  icons and pictures first, with one or two short words under them.
- **Every bubble is spoken, at the moment it appears** (the human,
  2026-11-24: she can't read yet). Each checkpoint (a stroke done, a
  letter done, a word done, a step done, a hint) has its voice line, and
  the bubble shows the same words. No bubble without its voice line; the
  content validator fails on one. The line is never cut off because she
  keeps going; only a newer line of the same activity replaces it.
- **Big touch targets:** at least 64 dp for anything tapped, with space
  between them; letter tiles and answer pictures at least 96 dp; the
  tracing area as large as the screen allows.
- **One tap to continue:** the galaxy map opens on the next lesson, with
  the rocket on it and a big "Go" (Hai! / Go!). No choosing needed to play.
- **Always a way out:** a home button on every lesson screen (top corner,
  large) and Back on every menu. Leaving a lesson keeps what was done and
  never asks to confirm (nothing is lost).
- **Kind feedback, never a scolding:** a wrong answer gets a soft sound, a
  gentle wobble and a spoken hint ("Ascultă încă o dată" / "Listen
  again"); no buzzers, no red crosses, no lost stars, no lives, no score
  that goes down.
- **The language is always shown in two ways:** every letter, word and
  voice line belongs to Romanian or English, and the screen shows which by
  a colour **and** a badge with its own shape (design/README.md), never by
  colour alone.
- **Her own card:** "Who's playing?" shows one big avatar card per child,
  the name under it and spoken when tapped, so a child who can't read finds
  her own.
- **No pressure and no tricks:** no timers in lessons, no ads, no
  purchases, no "come back" notifications, no streaks, no lost rewards.
- **Readable everywhere:** text at least 24 sp on a tablet, letters to
  learn far larger (at least 120 sp), high contrast (WCAG AA, 4.5:1), and
  the activity area clear of the HUD and the display's cut-outs (safe
  area).
- **Sound and vibration have switches** in the parent area; with the sound
  off, the app says so and every activity still works from pictures (a
  listening activity shows its picture instead).

Note in the brief's Progress how a new screen meets these.

### Learning (a child is the learner)

New for Școlița. The approach is in `docs/curriculum.md`; these rules keep
every lesson true to it:

- **One new thing at a time.** A lesson introduces at most one new letter
  (or one new sound of a letter already met), then practises it with what
  is already known.
- **Hear before see, say before read.** A letter first comes as a sound
  and a picture, then as a shape, then to trace, then in words.
- **Sounds and names are different things,** and both are recorded: the
  sound is pure (`/mmm/`, never "muh"), the name is the alphabet's name
  ("me" in Romanian, "em" in English). Activities say which they mean.
- **Only what has been taught.** A word or sentence in a lesson uses only
  letters (and sounds) taught before it in its track, unless it is marked
  as a "listen only" word. The content validator checks this from the
  curriculum's order.
- **Two languages, never blurred.** A card, a word or a voice line has one
  language. The two meet only in activities made for it (`TwoSounds`,
  `WordTwins`), where both badges are shown.
- **Romanian spelled right:** ș and ț always with the comma below (U+0219,
  U+0218, U+021B, U+021A), never the cedilla (ş ţ, U+015F, U+0163);
  â and î where Romanian spelling puts them. The validator fails on a
  cedilla anywhere in the content or the string tables.
- **The letter shapes she will write at school, made by hand.** Letters
  to read use a school print typeface (single-storey ɑ and ɡ, as children
  write them). Letters to write follow the stroke data in the content, in
  the order and direction a teacher shows, in two forms: print, then the
  **handwritten** form ("litera de mână", with its entry and exit strokes,
  joined into words). Handwritten letters are always drawn from their
  stroke data, never from a cursive font, so what she sees is what she
  traces (the human, 2026-11-24; decision D3).
- **Second try with a hint, then show and move on.** The first miss gets a
  hint, the second shows the answer kindly and lets her do it with the
  answer shown; the number of tries is in the learning rules asset.
- **Learned means seen often, on different days.** An item is learned
  after enough correct answers across different activity kinds on at least
  two days (numbers in the learning rules asset); learned items come back
  for short reviews on a growing schedule.
- **Short and sweet.** A lesson takes 3 to 5 minutes; the parent can set a
  session limit, which ends gently at the end of a lesson ("Racheta se
  odihnește" / "The rocket needs a rest"), never in the middle.
- **Correct for any name and any child:** a line with a child's name
  spliced in is worded so it stays correct whatever the name and whether
  the child is a girl or a boy: the name only in the nominative ("Bună,
  {name}!"), never in the genitive or dative, and nothing that agrees with
  the child's gender (see "Romanian, checked").
- **Correct, complete language** (the human, 2026-11-24: "it is important
  that she learns correctly"). She learns her language from what the app
  says, so every line is a complete, grammatical sentence with its verb,
  never a shortcut ("Bravo, ai făcut litera m!", never "Bravo, m!"), with
  the right agreement ("litera" is feminine: "leag-o", "scrie-o"), the
  right diacritics and the right punctuation, in both languages. When a
  line is built from parts ("Bravo, ai făcut litera" + "m"), every
  combination must be correct, and the brief lists them.
- **Native words, checked by the human.** Every word, sentence and voice
  line is checked by the human before its release (the brief's
  "Voice lines" and the release's rows in `docs/manual-testing.md`).

### Romanian, checked

New for Școlița (the human, 2026-11-24: "she should learn as correct as
possible"). Agents get Romanian wrong in predictable ways; every Romanian
line (voice lines, bubbles, buttons, the parent area) is checked against
this list before the push, and the brief lists any line the agent is not
sure of, for the human:

- **Names in the genitive or dative change with the name:** "profilul
  Anei", "lui Ioana" is wrong too ("Ioanei"), but "profilul lui Matei",
  "lui Carmen". A spliced name never goes in these cases; word the line
  around it ("Schimbă profilul", the name shown apart).
- **Agreement with the child:** no adjective or participle about the child
  ("singură", "harnică", "aleasă", "gata?" is fine, it does not change);
  use forms that don't change: "fără ajutor", "ai reușit", "ai făcut".
- **Agreement with the word:** "litera" is feminine ("leag-o", "scrie-o",
  "litera nouă"), "cuvântul" is neuter ("scrie-l", "cuvântul nou"),
  "sunetul" is neuter ("ascultă-l").
- **Numbers:** from 20 on, a noun takes "de" ("20 de stele", "24 de
  stele", "douăzeci de minute"); under 20, none ("3 stele", "15 minute").
  "Un" / "o" and "doi" / "două" agree ("o stea", "două stele", "doi pași").
- **Hyphens where Romanian joins words:** "leag-o", "du-o", "s-a",
  "într-o", "n-ai", "dă-mi".
- **Diacritics:** ș ț with the comma below (the validator checks it); â
  inside a word and î at its start or end ("când", "în", "a coborî"; also
  "România", "român"), and "sunt", "suntem", "sunteți" with u (never
  "sînt").
- **Quotation marks** in Romanian text are „…”; the punctuation is
  Romanian (no space before "!" or "?").
- **Imperatives addressed to the child** use the 2nd person singular
  ("atinge", "ascultă", "scrie", "du", "leagă"), the same form for a girl or
  a boy.
- **The human is the last check** for Romanian (and English): a release's
  new lines are in its rows of `docs/manual-testing.md`, to be heard and
  read before she sees them.

### Audio

New for Școlița:

- **Voice clips** are mono OGG Vorbis at 44.1 kHz, trimmed to 100 ms of
  silence at each end, loudness-normalised to the same level (−16 LUFS
  integrated, peaks under −1 dBFS), named `<lang>-<kind>-<id>.ogg`
  (`ro-sound-m.ogg`, `en-word-rocket.ogg`, `ro-line-try-again.ogg`), in
  `content/voice/` (Git LFS).
- **Every voice line is listed** in `content/voice-lines.csv` with its
  language, its text and its clip (or none, for the system voice), so the
  human can see what is still to record (`local/todo/04-record-voices.md`).
- **Music and effects** never cover the voice: the music ducks while the
  voice speaks.

### Fresh-eyes review

- Every large change (see Architecture), any change to saving or the
  progress format, to the content model or the importer, to the build or
  signing, and the parent gate gets a review by a second agent that didn't
  write it, before the push. The reviewer checks the diff against this
  section; findings and how they were resolved go in the brief's Progress
  section.

## Code (C#)

- Do not add comments except XML doc comments (`///`) on classes and public
  members. An exception is when a comment is absolutely necessary to explain
  something that can't be said in code and can't be an XML doc comment.
- Each XML doc comment on a method says where it is called from
  (`<see cref="..."/>`), especially for tests. The code should be mostly
  self-explanatory, with clear names for variables and methods.
- Shared logic is defined in one place and reused, not duplicated.
- **Do not use `var`.** Always write the specific type.
- **No `using static`.** Qualify every static call with its type, for
  example `Mathf.Clamp(...)`, `MathF.Sqrt(...)`.
- Use enums, not strings or magic numbers, for kinds and states
  (`ActivityKind.TraceLetter`, `Language.Romanian`, `StepState.Hinting`);
  tuning numbers live in the rules assets, never in code. Content ids are
  the one place strings identify things, and they are validated.
- Fields serialized by Unity are `[SerializeField] private`, never public
  fields. Public state is exposed through get-only properties.
- One class per file, the file named after it; namespaces follow folders
  (`Scolita.Core.Tracing`).
- Do not call a public method of the same class from another public method.
  Extract the shared logic into a private method and call it from both.
- No learning logic in MonoBehaviours (see "How the app is put together"):
  they are the controllers; Core holds the rules, so each can be tested on
  its own.
- LINQ is preferred outside an activity's frame loop, one operation per
  line for readability; inside it, plain loops (see "Performance").
- Use the latest C# features Unity 6.3 compiles, as long as they don't
  allocate while an activity runs.

## C# tests (Unity Test Framework, NUnit)

- **EditMode tests for every Core rule** (they run in milliseconds, with no
  scene); PlayMode tests for scene flows (the map opens on the next lesson,
  a lesson runs to its end, leaving a lesson keeps its progress, the parent
  gate holds).
- **Every activity kind's rules are tested with real content items** from
  the catalogs, not only made-up ones, so a content change that breaks an
  activity fails a test.
- **Tracing is tested with recorded strokes:** good, sloppy-but-fine,
  backwards, off the letter, too short, as point lists in
  `Tests/EditMode/Tracing/Strokes/`.
- Name tests `Test<MethodName>_<MinimalCaseDescription>`.
- Add a class XML doc comment linking to the tested class, and a method XML
  doc comment linking to the tested method.
- **No class or instance fields in test classes** and no `[SetUp]` state.
  Create the tested instance and its fakes as local variables in each test.
- **No mocking library.** Use small hand-written fakes for interfaces
  (`FakeVoice`, `FixedRandom`, `FakeClock`) in `Tests/EditMode/Fakes/`.
- Assert with NUnit's constraint model, qualified:
  `Assert.That(actual, Is.EqualTo(expected))`; floats with
  `.Within(tolerance)`.
- Core is deterministic: a test sets the seed and the clock, never waits
  for real time.
- PlayMode tests use `[UnityTest]` and wait for frames, never for seconds
  of real time unless the rule is about real time.
- Apply these rules when editing older tests too.
