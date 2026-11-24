# Task: feature/content-model

> **Status: Done (content, 2026-11-24), released as 0.2.0.** The Design note was agreed by the human
> on 2026-11-24.

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
- **Design note** (below) pushed in `b830308`. **The human agreed it on
  2026-11-24, all of it, with no changes** (relayed by the lead): the
  model and its layers; a planet holds both languages, its lessons marked
  `ro`, `en` or `both`; seven CSVs (`lessons.csv` one row per lesson,
  `steps.csv` one row per step); the adjusted picture rule; the row order
  as the teaching order; a deleted row refused (retire it instead).
- **Built and pushed in four slices,** tested together at the last
  commit, after the fresh-eyes review:
  - `core:` the model, the CSV reader and the rules;
  - `content:` the asset types and the catalogs;
  - `build:` the importer and the placeholders;
  - `content:` planet 1's sample and its imported assets.

  Their hashes are in the completion record.
- **Closed out** as release 0.2.0 (see "Completion record").

### Decisions while building (within the agreed note)

- **A step's language in a `both` lesson** is written `ro` or `en` (a
  planet party has steps of each). Empty means the lesson's language, and
  the bridge kinds (`TwoSounds`, `WordTwins`) are always in both. A step
  in a `ro` or `en` lesson can't name the other language.
- **Voice line ids are ASCII,** like every id. The draft's
  `ro-word-albină` becomes `ro-word-albina`. Its text keeps the
  diacritics.
- **The placeholder pictures are drawn from Andika's distance-field
  atlas** (`Andika-Regular SDF.asset`, read on the CPU). TextMeshPro
  can't add glyphs to a new atlas in batch mode with `-nographics`. The
  circle's colour comes from the picture's id (the rainbow path's six
  colours). The letter is the first letter of the first word using the
  picture.
- **Pictures are imported as single sprites** without mipmaps. The 2D
  template imports PNGs as "Multiple" sprites, so the importer sets the
  mode, and the freshness test checks it.
- **What a step needs is in one table,** Core's `ActivityKinds`. For each
  kind: its letter and word counts, how it shows its words (heard,
  pictured, read), the extra word lines it needs (`Syllables` → the
  syllables line), whether it bridges both languages, its options
  (`TraceLetter`: `form`, `case`, both required) and its instruction
  lines (the first line of each kind in the draft). The kinds' own briefs
  add their hints and checkpoints there.
- **Small Core additions:** `StableIdText` (the one `[a-z0-9-]+` check,
  now shared by `ProfileId` and `ContentId`) and `LanguageMixExtensions`
  (`Includes`, `Languages`, `Alone`).
- **Planet 1's sample content** follows curriculum §4.1:
  - **Lessons:** one per letter in each language
    (`ro-a`, `en-a`, … `en-p`), `ro-words-1`, `en-words-1`, `capitals-1`
    and `party-1`: 12 lessons and 71 steps.
  - **Words:** the TraceWord words (mama, tata, pat, apa; map, mat, tap,
    pat), with the false friends pat / pat. Ten listen-only words for the
    letters' pictures and `TwoSounds`: avion, albină, muzică, tobă,
    planetă; apple, astronaut, moon, tiger, planet (planetă / planet are
    twins).
  - **Pictures:** 17, all placeholders.
  - **Voice lines:** 279, text only, no clips. They are the sounds and
    names of a m t p in both languages, the words' word, slow and
    syllables lines, and all 224 general lines of the agreed draft.
  - `ListenAndPick` in `ro-a` uses a, m and t, as `lesson-engine`'s
    acceptance expects.

### Fresh-eyes review (2026-11-24)

A second agent, which didn't write the code, read the diff against
AGENTS.md before the push. It found no blockers: the seams hold, Core has
no engine reference, and there is no `var` and no `using static`. It also
found the CSV reader's row numbers, the GUID handling, the importer's
folders, the batch exit codes and the freshness comparison sound. Its 15
findings, and what was done:

1. **An id deleted together with its asset could be reused later.** The
   message no longer invites deleting an asset by hand. A ledger of
   released ids, checked at import, is a follow-up for the human to
   agree.
2. **A step whose kind can't be played with its items:**
   - **Fixed:** a `MeetLetter` or `FirstSound` word must start with one of
     the step's letters (`ActivityItemRules.WordStartsWithALetter`; test
     `TestCheck_WordStartsWithTheStepsLetter`).
   - **Left to the kinds' own briefs, which list them:** a `WordTwins`
     step's words must pair up (`activities-words`); `TwoSounds` must be
     on a letter whose two sounds differ (`activities-letters`). Both are
     in Follow-ups.
3. **`ListenAndPick` took letters and words together.** Now letters or
   words, never both (`ActivityItemRules.LettersOrWords`; test
   `TestCheck_ListenAndPickTakesLettersOrWords`).
4. **The syllables line wasn't checked against the word.** Now it must
   say the word's syllables (test `TestCheck_SyllablesLineSaysTheSyllables`).
5. **`capitals-1` is a Romanian lesson,** so "English only" mode skips
   the capitals. Sample data: a follow-up for `content-letters-1`, which
   writes planets 1–3's lessons.
6. **A missing sprite stopped the curriculum from loading.** At run time
   it now logs a warning (the activities show a placeholder). The
   validator still fails on it.
7. **Romanian letters, a cedilla and the byte order mark were written
   literally in C#.** Every non-ASCII character outside XML comments is
   now a `\u` escape (the project-skeleton convention). The tool used to
   write the files had decoded the escapes.
8. **Steps use letters not yet taught** (in `ro-a`, `ListenAndPick` and
   `LetterHunt` use m and t as wrong choices). This is kept, because
   `lesson-engine`'s acceptance asks for `ListenAndPick` with a, m and t
   in the first lesson. The rule covers words. Whether a step's *target*
   letter must be taught is for the kinds' briefs (they say which item is
   the target): a follow-up, and a question for the human in
   `local/for-human-next-steps.md`.
9. **`Apply` could stop halfway on an asset of the wrong type,** and leak
   the in-memory assets. The types are now checked in `Make` (an error,
   nothing written), and the planned assets are destroyed in a `finally`.
10. **An art cell could name a file outside `content/pictures/`**
    (`../x.png`). Now refused (test `TestCheck_ArtStaysInThePicturesFolder`).
11. **A public method called another public one** (`Curriculum.Track`,
    `RowReader.Required` and `Error`, `ContentFolder.ArtPath`,
    `AssetFields.Id`). Private helpers now.
12. **XML docs without "called from"** on `ContentAssetPaths`. Added.
13. **The validator test filtered warnings by their wording.** It now
    filters by file, and uses `ContentImporter.AssetFolder`.
14. **The placeholder shows the word's first letter,** which gives away
    `FirstSound`'s answer until real art comes (the brief asked for it).
    Noted for the human.
15. **Small leftovers:**
    - The stray note on `en-word-mat` is gone.
    - Steps have no ids (a lesson in progress resumes by step number): a
      note for `lesson-engine`.
    - A misspelt domain would make a new domain: a follow-up.

### Follow-ups

- **A ledger of released ids** (finding 1): for example, the importer
  checks `content/released-ids.txt` (appended at each close-out). A row
  deleted together with its asset is then still caught. The human to
  agree; `content-letters-1` or a chore brief.
- **`activities-letters`:**
  - `TwoSounds` only on a letter whose sounds differ (`vowel-differs`,
    `sound-differs`), with words of both languages;
  - which of a step's letters is the answer, and whether it must be
    taught. Add these to `ActivityKinds` and `StepRules`.
- **`activities-words`:** `WordTwins` words pair up as twins or false
  friends within the step; `BuildWord` words fit the rocket's windows.
- **`lesson-engine`:** a lesson in progress resumes by step number, and
  steps have no ids, so a step inserted later shifts it. Restart a
  lesson whose steps changed. Also: each kind's hints and checkpoints
  are lines to add to `ActivityKinds.InstructionLines` (only the first
  instruction lines are required now).
- **`content-letters-1`:**
  - capitals for "English only" (`capitals-1` is `ro`);
  - the other two TraceWord words per language (pat and apa; tap and pat);
  - planets 2–3;
  - the lines' `notes` (copied from the draft) can be trimmed.
- **`voice`:**
  - copy the clips named in `voice-lines.csv` into
    `Assets/Scolita/Content/Voice/` and link them to the voice line assets
    (now a file name);
  - its status column is a new column in `ContentTables`;
  - a line's planet for the recording sessions can be computed from the
    lessons.
- **Domains:** `units.csv`'s `domain` isn't checked against a list (a
  typo makes a new domain). Fine while there is one domain; check it when
  a second comes.

### Test results (2026-11-24, rebased on `6c15c7c`, at the commits pushed)

- **EditMode** 236/236 in batch mode, with `look`'s new tests. The 72 new
  tests:
  - Core: the CSV reader, ids, kinds, the curriculum and tracks, the
    reading problems, and every validator rule;
  - the importer, the placeholders, the freshness test, the validator on
    `content/`, and the curriculum from the catalogs.
- **PlayMode** 3/3.
- **`BuildAndroidDebug`:** exit 0, `Scolita-0.1.0-debug.apk`. An earlier
  run failed only because another process stopped the shared Gradle
  daemon ("stop command received"); the rerun passed.
- No `warning CS` or `error CS` in any log. `git status` was clean after
  the runs.
- **Batch-mode import:**
  - exit 0, "imported, with 17 warnings" (the 17 placeholder pictures);
  - with a broken row added to `words.csv`: exit 1, naming
    `words.csv, row 20` and both reasons, and writing nothing.
- **Placeholders** looked at (`bed.png`, `planet.png`): a coloured
  circle with an Andika "p" in the ink.
- **GUID stability:** a re-import that changed the pictures' sprite mode
  kept every asset's GUID (`bed.png` `2c7733c7…`).
- **Not run, and why:**
  - **The emulator, the Device Simulator, screen sizes and the
    Profiler:** this brief has no screen, says nothing yet and runs
    nothing per frame. No scene loads the content until `lesson-engine`,
    so the APK behaves exactly as 0.1.0.
  - **Listening:** no line is spoken yet. The 279 lines' text is the
    human's agreed draft, checked again by the validator (no cedilla,
    each word's line says its word).
  - **The Editor's menu item:** run by the human; agents use batch mode.

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

## How to test locally

Run the commands in Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`
and `S=<your scratch folder>`. Close your Editor on this project before
any batch-mode run. This brief has no screen and nothing the app says
yet, so it needs no emulator and no Device Simulator. The app doesn't
load the content until `lesson-engine`.

### Core slice

1. **The EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`
   Expected: exit 0, `result="Passed"`, with the new classes among the
   results:
   - `CsvReaderTests`, `ContentIdTests`, `ActivityKindsTests`;
   - `CurriculumTests`, the taught-letters query;
   - `ContentTablesTests`: every reading problem names its file, row and
     reason;
   - `ContentRulesTests`: one test per validator rule. These include the
     brief's "mama before m fails, and passes once listen-only".

   `CoreAssemblyTests` still passes (Core references no Unity
   assembly). The log has no `warning CS` and no `error CS`.

### Content slice (the asset types)

1. **`CurriculumAssetsTests`** (in the EditMode run above) builds the
   `Curriculum` from the imported catalogs. Expected:
   - planet 1's 12 lessons, in order;
   - `ro-words-1` comes after a m t p in the Romanian track;
   - every letter and word has its voice lines.
2. **`ContentValidatorTests`:**
   - every asset under `Assets/Scolita/Content` is `IValidatableContent`
     and valid;
   - `TestCheck_TheContentCsvsHaveNoError`: the repository's CSVs have no
     error, and their only warnings are pictures still on their
     placeholder.

### Build slice (the importer)

1. **`ContentImporterTests`** (EditMode) imports the test content into a
   temporary folder:
   - a changed word keeps its asset's GUID and gets the new value;
   - a deleted row is refused and its asset stays;
   - an error writes nothing;
   - a file saved as ANSI is reported, and a byte order mark is read.

   `PlaceholderPictureTests` checks the placeholder is the same PNG every
   time. Expected: all pass, and nothing is left under `game/Assets` but
   `Plugins`, `Scolita`, `TextMesh Pro` and `ThirdParty`.
2. **Batch-mode import:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`
   Expected:
   - exit 0;
   - the log ends with `Scolita content import: imported, with 17
     warnings`, the 17 pictures that have no art yet;
   - `git status` shows no change: the assets are fresh.
3. **A broken CSV stops the import:**
   - add `ro-mami,ro,mami,m a m i,ma-mi,mother,,,,,` at the end of
     `content/words.csv` and run step 2. Expected:
     - exit 1;
     - `Content: words.csv, row 20: the letter i is not in letters.csv.`;
     - `Content: words.csv, row 20: the voice line ro-word-mami is not in
       voice-lines.csv.`;
     - `… 2 errors, 17 warnings: nothing was imported`;
     - `git status` shows only `words.csv` changed.
   - Then `git checkout content/words.csv`.
4. **In the Editor** (the human; agents run batch mode): **Școlița →
   Import content** shows "imported, with 17 warnings" in a dialog. The
   Console lists the warnings.

### Content slice (planet 1's sample)

1. **Open `content/README.md`,** then the CSVs, in a spreadsheet (Excel:
   *Data → From Text/CSV*, UTF-8). Expected:
   - planet 1 (a m t p): 4 letters, 18 words, 17 pictures, 279 voice
     lines, 12 lessons, 71 steps;
   - ă, ș and ț are drawn properly in "albină", "muzică", "planetă" and
     the lines.
2. **An edit round-trips:**
   - In `content/words.csv`, change `ro-mama`'s picture from `mother` to
     `plane` and run the import (Build slice, step 2). Expected:
     - `Words/ro-mama.asset` changes, with the same GUID in its `.meta`;
     - two placeholders change (`mother.png` loses its letter,
       `plane.png` shows m);
     - `ContentFreshnessTests` passes.
   - Then `git checkout content game/Assets/Scolita/Content`.
3. **The freshness test bites:** change any cell of a CSV without
   importing, then run the EditMode tests. Expected:
   `ContentFreshnessTests.TestDifferences_TheAssetsMatchTheCsvs` fails,
   naming the asset that differs. Then put the cell back.
4. **The cedilla check:** in `content/voice-lines.csv`, write `ş`
   (U+015F) for `ș` in any line and import. Expected: exit 1, with
   `voice-lines.csv, row N: the cell text has U+015F ş with a cedilla;
   Romanian writes U+0219 ș with the comma below.` Then put it back.

### PlayMode and build

1. `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: exit 0, 3 passed (unchanged).
2. `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`.
   Expected:
   - exit 0;
   - no `warning CS`;
   - the APK builds as before. The content isn't loaded by any scene yet,
     so the app on the tablet is unchanged.

### Cleanup

- `git checkout content game/Assets/Scolita/Content` undoes any edit
  made while testing.
- `game/Builds/` is ignored; delete it to free space.
- No save files are touched.

## Completion record

_Finished 2026-11-24 by `content`; released as 0.2.0 (tag `v0.2.0`)._

### What was built

- **Pushed:**
  - `3e05bf5` core: the content model, the CSV reader and the rules;
  - `db75cd0` content: the asset types and the catalogs;
  - `08f626a` build: the importer and the placeholders;
  - `f6bce65` content: planet 1's sample and its imported assets;
  - the close-out.
- **Core** (`Scripts/Core/Content`, no engine references):
  - **The model:**
    - `ContentId`, `VoiceLineId`, `VoiceLineIds` (the naming rule);
    - `Letter`, `Word`, `Picture`, `VoiceLine`;
    - `Domain`, `Unit`, `Lesson`, `Step`, `StepOption`, `Track`;
    - `Curriculum` (catalogs, tracks, `LettersTaughtBefore`) and
      `CurriculumBuilder` (with `UnitEntry`, `LessonEntry`, `StepEntry`).
  - **The kinds:** `ActivityKind`, `ActivityKinds` (per kind:
    `ActivityKindRequirements`, `ItemRange`, `ActivityItemRules`,
    `ActivityWordUse`, `ActivityOption`).
  - **Codes:** `LetterGroup`, `VoiceLineKind`, `ContentCodes`,
    `ContentFileNames`.
  - **Reading:** `Csv/` (`CsvReader`, `CsvTable`, `CsvRow`),
    `ContentTables`, `RowReader`.
  - **Checking:** `Rules/` (`ContentRules`, `WordRules`, `PictureRules`,
    `VoiceLineRules`, `LessonRules`, `StepRules`, `References`),
    `ContentProblem(s)`, `IContentFiles`.
  - **Shared:** `Ids/StableIdText` (also used by `ProfileId`) and
    `Languages/LanguageMixExtensions`.
- **Content** (`Scripts/Content`):
  - the assets `LetterAsset`, `WordAsset`, `PictureAsset` (its
    `Sprite`), `VoiceLineAsset`, `LessonAsset` (with `StepData`),
    `UnitAsset`, `DomainAsset`;
  - `ContentCatalog<T>` and `LetterCatalog`, `WordCatalog`,
    `PictureCatalog`, `VoiceLineCatalog`, `DomainCatalog`;
  - `AssetFields`, and `CurriculumAssets.Build` (the run-time loader).
- **Editor** (`Scripts/Editor/ContentImport`):
  - `ContentImporter` (Școlița → Import content; `ImportFromCommandLine`);
  - `ContentImportPlan`, `PlannedAsset`, `PlannedImage`;
  - `ContentFolder`, `ContentAssetPaths`, `PlaceholderPicture`.
- **Content:**
  - `content/README.md` and the seven CSVs;
  - the imported assets in `Assets/Scolita/Content/` (`Letters/`,
    `Words/`, `Pictures/` with the PNGs, `Voice/`, `Domains/` with
    `Units/` and `Lessons/`, and the five catalogs).
- **Tests:** 72 new EditMode tests:
  - `Tests/EditMode/Core/Content/*`;
  - `Tests/EditMode/Content/ContentImporterTests`,
    `PlaceholderPictureTests`, `ContentFreshnessTests`,
    `CurriculumAssetsTests`, and the validator's CSV test;
  - fakes in `Tests/EditMode/Fakes` (`ContentCsvs`, `FakeContentFiles`).

### Conventions for later briefs

- **Content is edited in `content/*.csv` only,** then imported. A commit
  that changes a CSV carries the assets the importer wrote
  (`ContentFreshnessTests`). The format and its rules are in
  `content/README.md`.
- **Ids are ASCII `[a-z0-9-]+`, stable, never deleted:** retire a row.
  A word's id starts with its language (`ro-mama`). Voice line ids are
  `<lang>-<kind>-<key>`. A letter's and a word's lines are found by that
  rule (`Letter.SoundLine`, `Word.WordLine`, `SyllablesLine`, `SlowLine`),
  never by a column.
- **A new activity kind or a kind's new need** goes in Core's
  `ActivityKinds`: counts, item rules, word use, word lines, options and
  instruction lines. A kind's semantic checks go in `StepRules`, each
  with a broken-CSV test in `ContentRulesTests`.
- **The app loads the content with `CurriculumAssets.Build`** from the
  five catalogs (serialized references on the composition root), and
  works with Core's `Curriculum`, never with the assets.
- **A step's languages:**
  - `Step.Languages` is one language, or both for `TwoSounds` and
    `WordTwins`;
  - a profile's language mix plays the steps whose languages it holds
    (`LanguageMixExtensions.Includes`);
  - a lesson in both languages has steps of each.
- **Test content:** `ContentCsvs.Valid()` is a small valid set for rule
  tests; break one thing with `AddRow` or `ReplaceRow`, then assert with
  `AssertError(file, row, reason parts)`.
- **Romanian letters in C# are `\u` escapes,** strings and char literals
  alike (the tool used to write files may decode them: check with
  `grep -P '[^\x00-\x7F]'`).
- **`-nographics` batch mode can't add glyphs to a TextMeshPro atlas;**
  read the existing SDF atlas instead (`PlaceholderPicture`).

### How it was tested

- **Batch mode, at the pushed commits:** EditMode 236/236, PlayMode 3/3,
  and `BuildAndroidDebug`, with no compiler warnings.
- **The batch import:** exit 0 on the content, exit 1 on a broken row,
  writing nothing.
- **Placeholders** looked at.
- **Reviewed** by a second agent before the push (Progress → "Fresh-eyes
  review").
- **Not used:** the emulator, the Device Simulator and the Profiler (no
  screen, no runtime use yet), and listening (nothing is spoken yet).

### Decisions and deviations

- **The Design note** (agreed by the human on 2026-11-24): seven CSVs
  instead of six, tracks computed from the lessons' language, voice lines
  by naming rule, and the adjusted picture rule.
- **Decisions while building:** in Progress.
- **Clips** stay a file name; linking them is `voice`'s job.

### Follow-ups

All in Progress → "Follow-ups":

- **For the human:** a ledger of released ids.
- **`activities-letters`:** `TwoSounds` letters and words; a step's
  answer letter.
- **`activities-words`:** `WordTwins` pairs; `BuildWord` length.
- **`lesson-engine`:** resuming by step number; each kind's hint lines.
- **`content-letters-1`:** capitals for "English only", the other
  TraceWord words, planets 2–3.
- **`voice`:** linking the clips; the status column.
- **The domain list.**
