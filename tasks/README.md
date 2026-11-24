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
   `docs/manual-testing.md`; after it is pushed, push the tag
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
| 1 | `feature/project-skeleton` | In progress (skeleton) | [pending/project-skeleton.md](pending/project-skeleton.md) |
| 2 | `feature/curriculum-research` | Ready, beside 1 (docs only; the human agrees v1) | [pending/curriculum-research.md](pending/curriculum-research.md) |
| 3 | `feature/content-model` | Ready after 1 | [pending/content-model.md](pending/content-model.md) |
| 4 | `feature/voice` | Ready after 3 | [pending/voice.md](pending/voice.md) |
| 5 | `feature/lesson-engine` | Ready after 4 (decision D6) | [pending/lesson-engine.md](pending/lesson-engine.md) |
| 6 | `feature/tracing` | Ready after 5 (decisions D2, D3) | [pending/tracing.md](pending/tracing.md) |
| 7 | `feature/handwriting` | Ready after 6 (decision D3 answered: handwritten letters in the MVP) | [pending/handwriting.md](pending/handwriting.md) |
| 8 | `feature/activities-letters` | Ready after 5, beside 6 and 7 | [pending/activities-letters.md](pending/activities-letters.md) |
| 9 | `feature/activities-words` | Ready after 8 | [pending/activities-words.md](pending/activities-words.md) |
| 10 | `feature/galaxy-map` | Ready after 5, beside 6–9 | [pending/galaxy-map.md](pending/galaxy-map.md) |
| 11 | `feature/progress-review` | Ready after 7, 9 and 10 | [pending/progress-review.md](pending/progress-review.md) |
| 12 | `feature/rewards` | Ready after 10 | [pending/rewards.md](pending/rewards.md) |
| 13 | `feature/parent-area` | Ready after 11 (decision D8) | [pending/parent-area.md](pending/parent-area.md) |
| 14 | `feature/profiles` | Ready after 13 (decision D9; moved into the MVP 2026-11-24) | [pending/profiles.md](pending/profiles.md) |
| 15 | `feature/content-letters-1` | After 2 and 3, in parallel (decision D4) | [pending/content-letters-1.md](pending/content-letters-1.md) |
| 16 | `feature/look` | After 1, in parallel (decisions D5, D6, D7, D9) | [pending/look.md](pending/look.md) |
| 17 | `feature/sound-feel` | After 12, 14, 15 and 16 | [pending/sound-feel.md](pending/sound-feel.md) |

### After the MVP

| Feature | Status | Brief |
| --- | --- | --- |
| `feature/content-letters-2` | Idea: planets 4–6 (u r c l, b f g h, v z j k), content only | — |
| `feature/content-letters-3` | Idea: planets 7–9 (ă â î ș ț and ck sh ch th ng; w x y q; the Romanian groups and the English vowel teams) | — |
| `feature/drawing-pad` | Idea (PRD §3.9): "Atelierul", rainbow brushes, sticker stamps, letter stencils; first after the MVP | — |
| `feature/handwriting-2` | Idea: joined capitals (names, the start of a sentence), then free writing without a guide | — |
| `feature/sentences` | Idea (PRD §3.10): short sentences, then little stories read aloud with words lighting up | — |
| `feature/domain-music` | Idea (PRD §3.10): instruments, high and low, rhythm, a rainbow xylophone; a large change, the human agrees the Design note first | — |
| `feature/domain-numbers` | Idea (PRD §3.10): counting stars, numbers 0–20 in both languages | — |
| `feature/portrait` | Idea: portrait as well as landscape | — |
| `feature/ios` | Idea: needs a Mac and Apple's developer programme | — |

## In flight

One row per agent at work. Edit only your own row (`AGENTS.md`, "Keeping
progress visible"). Empty when nobody is at work.

| Agent | Brief | Touches | Emulator | Since | Status |
| --- | --- | --- | --- | --- | --- |
| skeleton | [project-skeleton](pending/project-skeleton.md) | Everything: creates `game/` | emulator-5600 | 2026-11-24 | Building |

## Where we are

_Last updated 2026-11-23 by the planner (no code yet)._
_Update this section whenever a slice is pushed or a brief is finished._

### Resume here (2026-11-23)

- **Done:** the plan: `AGENTS.md`, the PRD, the curriculum draft v0, the
  build plan, these briefs, and the look: the purple space world agreed by
  the human, with eleven screens designed (`design/README.md`,
  `design/screens.md`, `design/canvas/`).
- **Next:** `project-skeleton` and `curriculum-research`, side by side
  (one is code, the other docs only; they touch nothing in common). The
  human: create the repository and answer D1–D8 (`local/for-human-next-steps.md`).
- **Parallel work possible** once `project-skeleton` is done: `look`
  (art only); once `content-model` is done: `content-letters-1`; once
  `lesson-engine` is done: `tracing` (then `handwriting`),
  `activities-letters` and `galaxy-map`, each touching its own folders.

### Open follow-ups across briefs

- **From SeliSnake, built in from the start here:** the release signing
  that keeps the key's passwords out of `build.gradle` and Gradle's caches
  (SeliSnake's `release-signing` follow-up) is part of `project-skeleton`.
- **Vulkan on the Tab A7:** confirmed or not by SeliSnake's 0.1.0 test on
  the tablet; `project-skeleton` takes the same Graphics APIs setting
  SeliSnake ends up with.
