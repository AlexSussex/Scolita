# Task: feature/content-ledger

> **Status: In progress (ledger).** Both follow-ups were answered by the
> human on 2026-11-24 (content-model's Q2 and Q4).

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
