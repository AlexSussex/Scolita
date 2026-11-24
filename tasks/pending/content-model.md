# Task: feature/content-model

> **Status: In progress (content).**

Read `AGENTS.md` first (especially "The five seams", "Learning" and
"Versioning and releases" → "Progress survives updates"), and the
completion records in `tasks/done/`.

**Feature:** `feature/content-model`
**Implements:** PRD §4 (content as data); `docs/curriculum.md` §2, §3, §6.
**Depends on:** `project-skeleton`.

## Scope

1. **The content model in Core** (plain C#, no Unity): `Domain`, `Track`
   (a domain's track in one language), `Unit` (a planet), `Lesson`,
   `Step` (an `ActivityKind` and its item ids and options), and the items:
   `Letter` (its id, the grapheme in both cases, per language its sound
   and name voice lines, its shared/differs group, its stroke data id),
   `Word` (id, language, text, letters, syllables, picture id, voice
   lines, twin id, listen-only flag), `Picture` (id, art reference),
   `VoiceLine` (id, language, text, clip reference or none). The
   `Curriculum` is built from these and answers "which letters are taught
   before this lesson in this track".
2. **The CSV format** in `content/`: `letters.csv`, `words.csv`,
   `pictures.csv`, `lessons.csv` (one row per step), `voice-lines.csv`,
   `units.csv`; UTF-8, a header row, stable ids (`[a-z0-9-]+`), a
   `retired` column. Documented in `content/README.md` with an example row
   per file.
3. **The importer** (`Scolita → Import content`, and a batch-mode method
   for agents): reads the CSVs, builds ScriptableObjects under
   `Assets/Scolita/Content/` and their catalogs (`LetterCatalog`,
   `WordCatalog`, `PictureCatalog`, `VoiceLineCatalog`, `DomainCatalog`),
   keeps each asset's GUID across imports (an asset per stable id, updated
   in place), and reports every problem with its file and row.
4. **The validator** (EditMode, runs with the tests), failing on:
   - a cedilla ş ţ Ş Ţ anywhere;
   - a duplicate or reused id, or a retired id still used by a lesson;
   - a word using a letter not taught before its lesson in its track
     (unless listen-only);
   - a word's syllables that don't join back into the word;
   - a lesson step whose kind needs items it doesn't have (for example
     `TwoSounds` on a letter with one language);
   - a word, letter or step whose voice line is missing in the
     `voice-lines.csv` (a missing **clip** is a warning, not a failure:
     the system voice covers it);
   - a missing picture for a word that isn't listen-only.
5. **The freshness test:** the imported assets match the CSVs (a commit
   that changes a CSV without importing fails).
6. **Sample data:** planet 1 (a m t p) with four words per language, its
   lessons and voice lines, placeholder pictures (coloured circles with
   the word's first letter), no clips.

## Out of scope — do not build

- The lessons' screens, the voice, tracing (their briefs use this model).
- Stroke data's format beyond an id reference (`tracing` defines it).
- A custom editor window for content.

## Acceptance criteria

- Editing `content/words.csv` and running the importer updates the word
  asset in place (same GUID), and the validator passes.
- Each validator rule above has a test that feeds it a broken CSV and
  checks the error names the file, the row and the reason.
- A word "mama" in a lesson before "m" is taught fails validation; marked
  listen-only it passes.
- The importer runs in batch mode and exits non-zero on any error.

## Tests required

- EditMode: the model (`Curriculum`'s taught-letters query), the CSV
  reader (quoted fields, UTF-8 with and without a byte order mark, Excel's
  "CSV UTF-8"), each validator rule, the importer's GUID stability, the
  freshness test.

## Touches

`Scripts/Core/Content`, `Scripts/Content`, `Scripts/Editor/ContentImport`,
`content/`, `Assets/Scolita/Content/`, the catalogs (new).

## Screens

None.

## Voice lines

Planet 1's sample lines (text only, no clips).

## Before closing out

- README → "Edit the content" checked against what was built.
- Release 0.2.0.
