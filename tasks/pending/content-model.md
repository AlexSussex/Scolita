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

## Progress

_Kept by `content` while the brief is in flight._

### Status

- **Claimed** in `b775670`.
- **Design note** (below) written. This is a large change, so **I am
  waiting for the human's agreement** before writing any code.

### Design note (large change: the content model and the importer)

#### The choices, in short

1. **Core owns the model, the CSV reading and every rule.** Unity only
   stores what Core read (as ScriptableObjects) and moves files.
2. **A planet holds both languages' lessons.** Each lesson says its
   language (`ro`, `en` or `both`). A language's **track** is computed
   from that, not kept as a second tree.
3. **One asset per stable id**, updated in place so its GUID is kept,
   plus the five catalogs.
4. **Seven CSVs, not six:** `lessons.csv` (one row per lesson) and
   `steps.csv` (one row per step). The brief's one file would repeat each
   lesson's columns on every step row. *For the human to agree.*
5. **Voice lines are named by rule** (`ro-sound-m`, `en-word-pat`), so a
   letter or a word needs no voice columns. The validator checks that
   each line exists and that a word's line says the word.
6. **The importer makes placeholder pictures** for every picture with no
   art yet: a coloured circle with the word's first letter, in Andika.
   Filling in a picture's `art` cell swaps its placeholder for the real
   art.

#### Where the logic lives

- **Core (`Scripts/Core/Content`, no Unity):**
  - **Ids and sources:**
    - `ContentId`: a validated `[a-z0-9-]+` id. It shares its check with
      `ProfileId`.
    - `VoiceLineId`: the id `IVoice.Speak` will take.
    - `ContentSource`: the file and row, for messages.
  - **Items:** `Letter`, `Word`, `Picture`, `VoiceLine`.
  - **The tree:** `Domain`, `Unit`, `Lesson`, `Step`.
  - **Kinds and codes:** `ActivityKind` (the 11 kinds), `LetterGroup`,
    `VoiceLineKind`.
  - **`ActivityKinds`:** for each kind, what a step needs: how many
    letters and words, pictures or not, both languages or not, whether
    its words are shown to read, its option keys and its instruction
    lines. When a later brief's kind needs more, this is the one place it
    changes.
  - **The `Curriculum`:**
    - the catalogs (letters, words, pictures and voice lines, by id);
    - the domains;
    - `Track(domain, language)`;
    - `LettersTaughtBefore(lesson, language)`.
  - **Reading:**
    - `CsvReader`: quoted fields, `""` escapes, CRLF or LF line endings,
      a byte order mark, and `,` or `;` as the separator (Excel in a
      Romanian Windows writes `;`).
    - `ContentTables`: turns the seven tables into the model.
  - **Checking:**
    - `ContentRules`: every validator rule.
    - `ContentProblem`: the file, the row, the reason, and whether it is
      an error or a warning. The row is the spreadsheet's row number,
      with the header as row 1.
  - **No file access.** Core takes each CSV's name and text, and an
    `IContentFiles` that says whether an art or clip file exists.
- **Content (`Scripts/Content`, Unity at run time):**
  - **The assets:** `LetterAsset`, `WordAsset`, `PictureAsset` (with its
    `Sprite`), `VoiceLineAsset`, `UnitAsset`, `LessonAsset` (with its
    steps) and `DomainAsset`.
  - **The catalogs:** `LetterCatalog`, `WordCatalog`, `PictureCatalog`,
    `VoiceLineCatalog` and `DomainCatalog`.
  - Each asset and catalog is `IValidatableContent`. It holds ids as text,
    and Unity references only for Unity objects: the sprite now, the clip
    with `voice`.
  - **`CurriculumAssets`** builds the Core `Curriculum` from the five
    catalogs at run time, through the same Core builder the CSVs use. A
    problem found there is an impossible state, so it throws.
- **Editor (`Scripts/Editor/ContentImport`):**
  - **How to run it:** the menu item **Școlița → Import content**, or
    `ImportFromCommandLine` in batch mode, which exits 1 on any error.
  - **Reading:** it reads `content/*.csv` as strict UTF-8, so a file
    saved as ANSI is reported, not misread. It runs Core's reader and
    rules and logs every problem as `words.csv, row 7: …`. If there is
    any error, it writes nothing.
  - **Writing:** it first makes an import *plan* (each asset as it should
    be), then applies it.
    - An asset that exists is updated in place, so its GUID stays.
    - A new id gets a new asset.
    - An asset whose id has gone from its CSV is an error ("retire it,
      don't delete it"). That way an id can't be deleted and later reused
      by mistake.
  - **Freshness:** the same plan drives the freshness test.
  - **`PlaceholderPicture`** draws the placeholders:
    - the letter in Andika, rastered by TextMeshPro's font engine (as the
      font builder already does in batch mode);
    - a circle in one of the rainbow path's colours, picked from the
      picture's id;
    - ink `#2A2140`.

#### The data (`content/`, UTF-8, header row, documented in `content/README.md`)

| File | One row per | Columns |
| --- | --- | --- |
| `letters.csv` | letter (later also a group like `ch`, or a digit) | `id, lower, upper, group, strokes, retired, notes` |
| `words.csv` | word, in one language | `id, language, text, letters, syllables, picture, listen_only, twin, false_friend, retired, notes` |
| `pictures.csv` | picture (in no language, named by what it shows) | `id, art, retired, notes` |
| `voice-lines.csv` | thing the app says | `id, language, kind, text, clip, retired, notes` |
| `units.csv` | unit (a planet) | `id, domain, retired, notes` |
| `lessons.csv` | lesson | `id, unit, language, introduces, retired, notes` |
| `steps.csv` | step | `lesson, kind, language, letters, words, options, notes` |

- **Ids** are `[a-z0-9-]+`, stable and never reused. A word's id starts
  with its language (`ro-pat`, `en-pat`), because the same text can be a
  word in both languages. `retired` and `listen_only` are `yes` or empty.
- **Order:** the order of the rows is the teaching order (the units, a
  unit's lessons, a lesson's steps). Inserting a lesson in a spreadsheet
  needs no renumbering, and an accidental sort shows in `git diff`.
- **Letters:**
  - `group` is `shared-close`, `vowel-differs`, `sound-differs`,
    `romanian-only` or `english-only`. It also gives the letter's
    languages.
  - `strokes` is the id of the letter's stroke data. The stroke format
    itself is `tracing`'s.
- **Words:**
  - `letters` are letter ids (`m a m a`, `t a m t a m`), since a group
    like `ch` counts as one letter. They must join back into the text,
    and a listen-only word may leave them out.
  - `syllables` are split by `-` (`ma-ma`, `pi-an-o`).
  - `twin` and `false_friend` name the other language's word, and the
    link must go both ways.
- **Voice lines:**
  - **Ids follow a rule:** `<lang>-<kind>-<key>` (AGENTS.md → "Audio").
    - The letter `m` speaks `ro-sound-m`, `ro-name-m`, `en-sound-m` and
      `en-name-m`.
    - The word `ro-mama` speaks `ro-word-mama`, `ro-syllables-mama` and
      `ro-slow-mama`. The slow line may be left to its letters' sounds,
      as `voice` plans.
    - A step speaks its kind's lines (`ro-line-pick-letter`).
  - **The `clip` column** holds the clip's file name in `content/voice/`,
    or nothing for the system voice. A clip that is named but not there
    is a warning. Copying the clips into the project and linking them as
    `AudioClip`s is `voice`'s job: its `RecordedVoice` loads them per
    lesson.
  - **Two draft columns dropped:** `planet` and `used_by`. Both can be
    computed from the lessons, and copies kept by hand would drift.
    `voice` can compute each line's first planet for its recording
    sessions.
- **Lessons:**
  - `language` is `ro`, `en` or `both` (the stable `LanguageCodes`, read
    as a `LanguageMix`).
  - `introduces` is the lesson's one new letter (AGENTS.md: one new thing
    at a time). Only a `ro` or `en` lesson can have one.
- **Steps:**
  - `language` is empty (the lesson's language) or `both`. `both` is only
    allowed for the bridge kinds, `TwoSounds` and `WordTwins`. So in
    "English only" mode, an English lesson drops its `TwoSounds`, as
    curriculum §4.1 says.
  - `options` are `key=value` pairs separated by `;`
    (`form=handwritten;case=lower`). Each kind lists the keys it takes.
  - The items are kept in their order. Each kind's brief says what the
    order means.

**Tracks without a second tree:**

- **What a track is:** a domain's track in one language is its lessons
  in that language, plus its `both` lessons, in teaching order. The
  letters taught in a track are those lessons' `introduces`.
- **Why not the brief's tree:** the tree the brief suggests
  (`Domain → Track → Unit`) makes each planet two units, one per language.
  The map would then have to interleave two trees to get "a-ro, a-en,
  m-ro, …". And a planet party, which mixes both languages, would have no
  track to live in.
- **So** the tree stays `Domain → Unit → Lesson → Step`, and `Track` is a
  view of it.
- **"Taught before this lesson"** counts the letters introduced by the
  track's earlier lessons. The validator also allows the letter the lesson
  itself introduces, since its `MeetLetter` step comes first.

#### The validator

The validator is Core's `ContentRules`. The importer runs it, and so does
an EditMode test on `content/`. Every problem names the file, the row and
the reason.

**Errors:**

- **Spelling and format:**
  - a cedilla ş ţ Ş Ţ in any cell (the existing check on whole files
    stays);
  - a malformed or duplicate id;
  - a missing or unknown column;
  - an unknown code (a language, group, kind or option).
- **References:**
  - an id that doesn't exist;
  - an id that is retired, used by anything still in use.
- **Words:**
  - a letter not yet taught by the word's lesson in the track of the
    word's language, unless the word is listen-only;
  - a listen-only word in a step that shows words to read
    (`ReadAndMatch`, `BuildWord`, `TraceWord`);
  - letters that don't join into the word, or syllables that don't join
    back into it (case and hyphens aside, as in `tam-tam` and `Ana`);
  - a word whose language isn't one of its step's languages.
- **Steps:**
  - too few or too many letters or words for the step's kind;
  - a kind that needs both languages used on a letter that has one
    (`TwoSounds` on a Romanian-only letter).
- **Voice lines:**
  - a missing line:
    - a letter's sound or name, in each of its languages;
    - a word's own line;
    - the syllables line of a word in a `Syllables` step;
    - a step's instruction lines, in each of its languages;
  - a word line whose text isn't the word;
  - a line id that doesn't match its `language` and `kind`.
- **Lessons:**
  - a lesson that introduces a letter not in its language;
  - a `both` lesson that introduces any letter.
- **Pictures:** a step whose kind shows pictures using a word with no
  picture.

**Warnings:**

- a clip that is named but not in `content/voice/` (the system voice
  covers it);
- a picture still on its placeholder.

**The brief's picture rule, adjusted (for the human to agree).** The brief
asks for "a missing picture for a word that isn't listen-only" to fail.
But in the agreed curriculum, the reading-only words (am, at, in, it, is,
not, on) have **no** picture by design. And the listen-only words **must**
have one, because they are shown as pictures. So I propose two rules
instead:

- a word with no picture can't be used in a step whose kind shows
  pictures (`ListenAndPick` with words, `FirstSound`, `ReadAndMatch`,
  `TwoSounds`, `Syllables`, `MeetLetter`, `WordTwins`);
- a listen-only word must have a picture.

#### How it is tested (EditMode)

- **The taught-letters query of the `Curriculum`:** with planet 1's real
  content, and with small made-up tracks (the two languages kept apart,
  `both` lessons, retired lessons).
- **`CsvReader`:**
  - quoted fields with commas, quotes and line breaks;
  - with and without a byte order mark;
  - Excel's "CSV UTF-8" (a byte order mark, CRLF, `;` in a Romanian
    locale);
  - blank lines;
  - a row that is too short or too long;
  - invalid UTF-8.
- **Every validator rule:** a small valid set of CSVs, built in the test,
  with one thing broken. The test checks the file, the row and the
  reason. This includes the brief's case: "mama" before m fails, and
  passes once it is listen-only.
- **The importer,** into a temporary folder:
  - a word changed and imported again keeps its asset's GUID, with the
    new text;
  - a deleted row is refused;
  - an error writes nothing.
- **The freshness test:** the plan made from `content/` equals the
  assets under `Assets/Scolita/Content`.
- **The content validator:** the rules pass on `content/`, with warnings
  only for clips and placeholders.
- **`CurriculumAssets`:** it builds the same `Curriculum` from the
  imported catalogs.
- **The placeholder:** it comes out the same, byte for byte, every time.
- **Batch mode, run by hand:** a broken CSV gives exit code 1.

#### How it serves another domain

- **Numbers:** data only.
  - A `numbers` row in `units.csv`, and its lessons.
  - The digits are `Letter`s: a symbol with a sound line, a name line and
    strokes, so `TraceLetter` can trace a 7.
  - The number words are `Word`s with pictures ("șapte" with seven
    stars).
- **Music:**
  - The instruments are `Word`s with pictures and voice lines
    (pian / piano).
  - What music lacks is an instrument's own sound (which isn't a voice
    line) and notes. Those would be a new item type, `Sound`, with its own
    catalog. That is a code change, which `domain-music`'s Design note
    brings to the human, as the PRD plans.
- **Nothing else is tied to reading:** the reader, the tree, the tracks
  and the rules never name the alphabet's letters, planets or Romanian.
  The one exception is the cedilla rule, which applies to everything.

#### Options compared

| Question | Picked | Rejected, and why |
| --- | --- | --- |
| Where the rules live | Core, on the model read from the CSVs | The Editor, over ScriptableObjects: not testable without Unity, and against seam 1 |
| What the app reads | ScriptableObjects in catalogs (the stack) | CSV `TextAsset`s parsed at start: slower, with no place for sprite references |
| Assets | One per stable id, plus catalogs | Records inside each catalog: fewer files, but every new word conflicts in one file, and the brief asks for a GUID per id |
| Planets and languages | A unit holds both languages' lessons; the track is computed | A unit per language (a strict tree): the map must interleave two trees, and the party has no home |
| Lessons and steps | Two files | One file with the lesson's columns repeated on each step: the copies drift, and one spreadsheet edit must change many rows |
| Voice line references | By rule from the id, checked | A column per line: seven more columns to type, and more typos |
| Placeholder pictures | Made by the importer | Hand-made PNGs: every new word needs one made by hand |
| Clips | The file name now; the files and `AudioClip`s with `voice` | Linking clips now: there is no clip yet to test it with |

#### For the human to agree

1. The model and the layers above.
2. `lessons.csv` plus `steps.csv` (the brief asked for one `lessons.csv`).
3. The adjusted picture rule: reading-only words have no picture, and
   listen-only words must have one.
4. The order of the rows is the teaching order.
5. The importer refuses a deleted row (retire it instead).
