# Task briefs

One brief per feature, built in the order of the
[MVP Build Plan](../docs/mvp-build-plan.md#build-order). All work goes
straight to `main`, and several agents may work at once. The rules are in
`AGENTS.md` under "Workflow", "Commits", "Several agents at once" and
"Testing before a push".

## Folders

| Folder | Holds |
| --- | --- |
| `pending/` | Briefs not yet finished, including the ones in progress |
| `done/` | Finished briefs, each with a **Completion record** appended |

## Lifecycle

1. **Write the brief** in `pending/<feature>.md` using the template in
   `AGENTS.md`.
2. **Claim it** in "In flight" below with a `docs: claim <feature>` commit
   pushed to `main` (`AGENTS.md`, "Claiming work").
3. **Build it** in your own worktree, Core commits first, then the Unity
   side, then content, each tested before it's pushed to `main`
   (`AGENTS.md`, "Testing before a push"). Keep the brief's `## Progress`
   section current with every push.
4. **Write the test steps.** Append a `## How to test locally` section to
   the brief (one `###` subsection per slice). The full rule is in
   `AGENTS.md` under "Local test instructions".
5. **When the last commit is pushed,** move the brief with
   `git mv tasks/pending/<feature>.md tasks/done/` and append a
   `## Completion record`: what was built (with paths), conventions later
   work must follow, how it was tested, decisions or deviations from the
   brief, and follow-ups. Add a status line under the title, update the
   index below, and remove the "In flight" row. The same close-out commit
   is the brief's release: it raises `VERSION`, adds the entry at the top
   of `CHANGELOG.md` and the release's section in
   `docs/manual-testing.md`, and adds the release's new content ids to
   `content/released-ids.txt` (Școlița → Record released ids, after
   raising `VERSION`); after it is pushed, push the tag
   `v<version>` on it (`AGENTS.md`, "Versioning and releases").

Each brief file therefore reads, top to bottom:

1. the original brief (unchanged), showing what was asked;
2. `## Progress`, kept by the agent holding it while it's in flight;
3. `## How to test locally`, a runnable script for checking the feature;
4. `## Completion record`, covering what was delivered, decisions and
   follow-ups.

## Index

### The MVP

| # | Feature | Status | Brief |
| --- | --- | --- | --- |
| 1 | `feature/project-skeleton` | Done 2026-11-24, released as 0.1.0 | [done/project-skeleton.md](done/project-skeleton.md) |
| 2 | `feature/curriculum-research` | Done 2026-11-24 (curriculum v1 agreed by the human) | [done/curriculum-research.md](done/curriculum-research.md) |
| 3 | `feature/content-model` | Done 2026-11-24, released as 0.2.0 | [done/content-model.md](done/content-model.md) |
| 4 | `feature/voice` | Done 2026-11-24, released as 0.3.0 | [done/voice.md](done/voice.md) |
| 5 | `feature/lesson-engine` | Done 2026-11-24, released as 0.4.0 | [done/lesson-engine.md](done/lesson-engine.md) |
| 6 | `feature/tracing` | In progress (tracing) | [pending/tracing.md](pending/tracing.md) |
| 7 | `feature/handwriting` | Ready after 6 (decision D3 answered: handwritten letters in the MVP) | [pending/handwriting.md](pending/handwriting.md) |
| 8 | `feature/activities-letters` | In progress (letters) | [pending/activities-letters.md](pending/activities-letters.md) |
| 9 | `feature/activities-words` | Ready after 8 | [pending/activities-words.md](pending/activities-words.md) |
| 10 | `feature/galaxy-map` | Ready after 5, beside 6–9 | [pending/galaxy-map.md](pending/galaxy-map.md) |
| 11 | `feature/progress-review` | Ready after 7, 9 and 10 | [pending/progress-review.md](pending/progress-review.md) |
| 12 | `feature/rewards` | Ready after 10 | [pending/rewards.md](pending/rewards.md) |
| 13 | `feature/parent-area` | Ready after 11 (decision D8) | [pending/parent-area.md](pending/parent-area.md) |
| 14 | `feature/profiles` | Ready after 13 (decision D9; moved into the MVP 2026-11-24) | [pending/profiles.md](pending/profiles.md) |
| 15 | `feature/content-letters-1` | After 2 and 3, in parallel (decision D4) | [pending/content-letters-1.md](pending/content-letters-1.md) |
| 16 | `feature/look` | In progress (look) | [pending/look.md](pending/look.md) |
| 17 | `feature/sound-feel` | After 12, 14, 15 and 16 | [pending/sound-feel.md](pending/sound-feel.md) |
| 18 | `feature/content-ledger` | Done 2026-11-24, released as 0.3.1 (a chore from `content-model`'s follow-ups) | [done/content-ledger.md](done/content-ledger.md) |

### After the MVP

| Feature | Status | Brief |
| --- | --- | --- |
| `feature/curriculum-research-2` | Ready (docs only): planets 4–6 in both languages, ă earlier or not; the human agrees the curriculum version | [pending/curriculum-research-2.md](pending/curriculum-research-2.md) |
| `feature/curriculum-research-names` | Ready (docs only): when and how letter names come in, both languages | [pending/curriculum-research-names.md](pending/curriculum-research-names.md) |
| `feature/content-letters-2` | Idea: planets 4–6 (u r c l, b f g h, v z j k), content only; after `curriculum-research-2` (and `-names` for the names after planet 6) | — |
| `feature/curriculum-research-3` | After `curriculum-research-2` (docs only): planets 7–9, the Romanian groups, the English digraphs, vowel teams and tricky words | [pending/curriculum-research-3.md](pending/curriculum-research-3.md) |
| `feature/content-letters-3` | Idea: planets 7–9 (ă â î ș ț and ck sh ch th ng; w x y q; the Romanian groups and the English vowel teams); after `curriculum-research-3` | — |
| `feature/drawing-pad` | Idea (PRD §3.9): "Atelierul", rainbow brushes, sticker stamps, letter stencils; first after the MVP | — |
| `feature/handwriting-research-2` | After `handwriting` (docs only): joined capitals, the rest of the alphabet by hand, free writing, both languages | [pending/handwriting-research-2.md](pending/handwriting-research-2.md) |
| `feature/handwriting-2` | Idea: joined capitals (names, the start of a sentence), then free writing without a guide; after `handwriting-research-2` | — |
| `feature/curriculum-research-sentences` | After `curriculum-research-3` (docs only): sentences and short stories, both languages | [pending/curriculum-research-sentences.md](pending/curriculum-research-sentences.md) |
| `feature/sentences` | Idea (PRD §3.10): short sentences, then little stories read aloud with words lighting up; after `curriculum-research-sentences` | — |
| `feature/domain-music` | Idea (PRD §3.10): instruments, high and low, rhythm, a rainbow xylophone; a large change, the human agrees the Design note first | — |
| `feature/domain-numbers` | Idea (PRD §3.10): counting stars, numbers 0–20 in both languages | — |
| `feature/portrait` | Idea: portrait as well as landscape | — |
| `feature/guides` | Idea (D5, 2026-11-24): more guides for her to choose from beside the unicorn and the cat co-pilot, chosen per profile; each new guide is art only (its poses and moods), since `look` builds the guide as data | — |
| `feature/ios` | Idea: needs a Mac and Apple's developer programme | — |

## In flight

One row per agent at work. Edit only your own row (`AGENTS.md`, "Keeping
progress visible"). Empty when nobody is at work.

| Agent | Brief | Touches | Emulator | Since | Status |
| --- | --- | --- | --- | --- | --- |
| look | [look](pending/look.md) | New: `Assets/Scolita/Art/` (sprites, atlases), `Scripts/Editor/Art/`, `design/art/`; changes: `Prefabs/UI/MapScreen.prefab`, `Prefabs/UI/LessonHud.prefab`, `ProjectSettings/ProjectSettings.asset` (the app icon only), `design/README.md`, `design/canvas/` | emulator-5604 | 2026-11-24 | Slice 1 pushed (the art, atlases, icon, placeholder prefabs); **waiting for the human and her** to choose on `local/look/choose.html` (step 5 in `local/for-human-next-steps.md`); the guide catalog's C# type waits for `content`'s Design note |
| letters | [activities-letters](pending/activities-letters.md) | `Scripts/Core/Activities`, `Scripts/Core/Content` (`ActivityKinds`, `StepRules`, a `stretched` voice line kind), `Scripts/Core/Lessons/LearningRules.cs`, `Scripts/Game/Lessons`, `Scripts/UI/Lesson`, `Scripts/Editor/Voice` (the new kind), new `Prefabs/Lesson/{LetterHunt,FirstSound,TwoSounds}View.prefab`; changes: `Prefabs/Lesson/ActivityRegistry.asset` (three rows), `Settings/LearningRules.asset`, `content/steps.csv` (`ro-a`, `en-a`, `ro-m`, `en-m`), `content/voice-lines.csv` (new rows) and their imported assets | emulator-5602 | 2026-11-24 | Claimed; Design note next |
| tracing | [tracing](pending/tracing.md) | New: `Scripts/Core/Tracing`, `Scripts/Game/Tracing`, `Scripts/UI/Tracing`, `content/strokes/`, `Content/Strokes/` (imported), `Prefabs/Lesson/TraceLetterView.prefab`; changes: the importer (a stroke reader), `CurriculumAssets`, `LearningRules` (+ asset), `ActivityRegistry.asset` (one row), `Scenes/Lesson.unity` (the runner's stroke catalog field), `Core/Lessons` (which steps can be played), `steps.csv` / `voice-lines.csv` only if the note needs it | emulator-5600 | 2026-11-24 | Claimed; writing the Design note |

## Where we are

_Last updated 2026-11-24 by agent `lesson` (lesson-engine done, release 0.4.0)._
_Update this section whenever a slice is pushed or a brief is finished._

### Resume here (2026-11-23)

- **Done:** the plan: `AGENTS.md`, the PRD, the curriculum draft v0, the
  build plan, these briefs, and the look: the purple space world agreed by
  the human, with eleven screens designed (`design/README.md`,
  `design/screens.md`, `design/canvas/`).
- **Done 2026-11-24:** `curriculum-research`. The curriculum v1 was
  agreed by the human (`docs/curriculum.md`). The growth research briefs
  are indexed under "After the MVP".
- **Done 2026-11-24:** `project-skeleton`, release 0.1.0: the Unity
  project in `game/`, Core's foundation, saving per tablet and per
  profile, the fonts, the build with release signing kept out of Gradle,
  and the Boot → Map → Lesson placeholders.
- **Done 2026-11-24:** `content-model`, release 0.2.0: the lessons as
  seven CSVs in `content/` (`content/README.md`), Core's content model and
  validator, the importer (Școlița → Import content) and planet 1 as
  sample content (text only, placeholder pictures).
- **Done 2026-11-24:** `voice`, release 0.3.0: everything said goes
  through `IVoice` (a clip, else the tablet's installed voice in ro-RO or
  en-GB, else a bubble); tap-to-hear `Hearable`; the voice sampler on the
  Lesson placeholder; `voice-lines.csv`'s `status` column; the recording kit
  (Școlița → Make recording scripts, Import / Accept recording session,
  Check voice clips; ffmpeg agreed as an Editor-only tool).
- **Done 2026-11-24:** `content-ledger`, release 0.3.1:
  `content/released-ids.txt` lists every released id, and the import
  refuses one deleted instead of retired. Each close-out runs Școlița →
  Record released ids. The placeholder pictures are plain circles.
- **Done 2026-11-24:** `lesson-engine`, release 0.4.0: Go opens the
  lesson in progress or the next one; Core's `LessonRun` with one hint
  ladder for every kind, `IActivityRules` / `IActivityView` /
  `ActivityRegistry`, `MeetLetter` and `ListenAndPick`, the mascot's
  bubble for every line, `lesson-run.json` per profile. Kinds not built
  yet are skipped (the registry's list).
- **Next:** `tracing` (then `handwriting`), `activities-letters` and
  `galaxy-map` in parallel, each touching its own folders (a new kind:
  `lesson-engine`'s completion record, "Conventions"); `content-letters-1`
  (content only); `look` in flight. The human: try 0.4.0 on the tablet
  (`local/for-human-next-steps.md`).

### Open follow-ups across briefs

- **From `lesson-engine`** (full list in its completion record):
  - Each activity brief takes its kind off `ActivityRegistry.asset`'s
    "not built yet" list and adds its rules, view, registry row and hint
    lines.
  - The device Profiler check of 0 B per frame in a lesson; a tap costs
    about 15 KB in the voice's planning (a follow-up for the voice).
  - `look`: the lesson screen is `Prefabs/Lesson/LessonScreen.prefab`;
    `LessonHud.prefab`'s placeholder card is hidden in the scene.
  - `rewards`: design screen 6 replaces the end panel.

- **From `voice`** (full list in its completion record):
  - ~~`lesson-engine`: load a lesson's clips, replace the voice sampler~~:
    done in 0.4.0.
  - `sound-feel`: duck the music on `IVoice.Started` / `Finished`.
  - The guide's mouth and bubble words can follow `VoiceRouter.PartStarted`.

- **From `content-model`** (full list in its completion record):
  - ~~A ledger of released ids~~ and ~~plain placeholder pictures~~: done
    in `content-ledger` (0.3.1).
  - **Wrong choices are only taught letters** (the human, 2026-11-24):
    done for `ListenAndPick` in 0.4.0 (`ActivityKinds.LettersAreChoices`,
    checked by the validator); `activities-letters` sets it for
    `LetterHunt` and `FirstSound` and fixes their `ro-a` / `en-a` rows.
  - `activities-letters`: `TwoSounds` only on letters whose sounds differ;
    which letter of a step is the answer, and whether it must be taught.
    `activities-words`: `WordTwins` words pair up.
  - ~~`lesson-engine`: resume by step number, hint lines in `ActivityKinds`~~:
    done in 0.4.0 (`LessonSignature`, `ActivityKinds.HintLines`).
  - `content-letters-1`: capitals for "English only" (`capitals-1` is
    `ro`); the other TraceWord words; planets 2–3.
  - `voice`: copy and link the clips named in `voice-lines.csv`.

- **From `curriculum-research`** (full list in its completion record):
  - `MeetLetter` gives a letter's sound only in planets 1–3; the name
    button on design screen 2 waits.
  - The writing board gets four lines.
  - Handwritten m and n start at the middle line.
  - The human checks the handwriting drawings against her manual
    before `tracing` and `handwriting` finalise their strokes.

- **From `project-skeleton`** (full list in its completion record):
  - Release signing keeps the key's passwords out of Unity's settings,
    `build.gradle` and Gradle's caches (a temporary file read by the
    launcher's Gradle template); SeliSnake's lead has the finding.
  - **Vulkan on the Tab A7:** Graphics APIs stay automatic, as SeliSnake;
    manual test 0.1.0-2 confirms Vulkan on the tablet, or a fix drops it.
  - `BootLoader` could take its save root from the test, for an
    end-to-end "corrupt settings at boot" PlayMode test.
  - **Each damaged save is kept, dated** (the human, 2026-11-24, Q1):
    `<name>.broken-<date-time>.json`, never replaced by the next one, and removed by themselves once a
    newer version has read and saved that file fine on several days (the
    human: no point keeping them once the version is stable); for
    `progress-review` (it touches `SaveStore`; its Design note sets the
    exact rule).
  - `AGENTS.md`'s repo layout: add `Assets/Scolita/Settings`,
    `Assets/Plugins/Android` and the `ThirdParty` packs' `SOURCE.md`.
