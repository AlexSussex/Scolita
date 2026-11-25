# The content

Școlița's lessons are data: these CSV files are the source of truth for
every letter, word, picture, voice line, planet, lesson and step, and of the rewards (stickers,
constellations, trails). The
importer (Unity: **Școlița → Import content**) turns them into the assets
under `game/Assets/Scolita/Content/`, which are never edited by hand. The
model and its rules are in `tasks/done/content-model.md` (Design note).

## How to edit

1. **Open a CSV** in a spreadsheet or a text editor, and save it as UTF-8
   (Excel: **CSV UTF-8**; `,` or `;` between cells both work). A file
   saved as ANSI is refused, never misread.
2. **Romanian ș and ț** are the comma-below letters (U+0219, U+021B); the
   cedilla letters ş ţ fail the import, which names the file, the row and
   the cell.
3. **Run Școlița → Import content** (or, in batch mode,
   `-executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine`,
   which exits 1 on any error). Each problem is listed as
   `words.csv, row 7: …`, with the row number the spreadsheet shows. With
   any error nothing is imported; warnings (a picture without art yet, a
   clip not recorded yet) don't stop it.
4. **Commit the CSV together with what the importer wrote** under
   `Assets/Scolita/Content/` (the freshness test fails otherwise).

## Rules for every file

- **Row 1 is the header,** with exactly the columns below (an unknown or
  missing column is an error). Blank rows are ignored.
- **The order of the rows is the teaching order:** the units, then each
  unit's lessons, then each lesson's steps. A sort changes what she
  learns: check `git diff` after editing in a spreadsheet.
- **Ids** are lower-case letters a–z, digits and `-` (`ro-mama`,
  `planet-1`; no diacritics: `ro-albina` for "albină"). An id is **stable**:
  it is never renamed, never reused, and its row is never deleted (her
  progress names it). To stop using an item, write `yes` in its `retired`
  cell; the importer refuses a deleted row, and an id of a release that
  is missing from its CSV (see "The ledger of released ids" below).
- **Yes/no cells** (`retired`, `listen_only`) are `yes` or empty.
- **Languages** are `ro` or `en` (`both` for a lesson or step that mixes
  them).
- **`notes`** is for people; the importer ignores it.

## The files

### `letters.csv`: one row per letter

| Column | What it holds |
| --- | --- |
| `id` | The letter's id (`a`; `a-breve` for ă; later `ch` for a group of letters, a digit `7`) |
| `lower`, `upper` | The letter in both cases (`a`, `A`) |
| `group` | `shared-close`, `vowel-differs`, `sound-differs` (in both alphabets), `romanian-only` or `english-only`; it gives the letter's languages |
| `strokes` | The id of its stroke data (`tracing` defines the files); empty for none yet |
| `retired`, `notes` | See above |

```
id,lower,upper,group,strokes,retired,notes
m,m,M,shared-close,m,,
```

Each letter needs its **sound** and **name** voice lines in each of its
languages: `ro-sound-m`, `ro-name-m`, `en-sound-m`, `en-name-m`.

### `words.csv`: one row per word, in one language

| Column | What it holds |
| --- | --- |
| `id` | `<language>-<word>` (`ro-pat`, `en-pat`: the same text can be a word in both) |
| `language` | `ro` or `en` |
| `text` | The word as written (`mama`, `tam-tam`, `Ana`) |
| `letters` | Its letters as letter ids, separated by spaces (`m a m a`); they must spell the text (case and hyphens aside). May be empty for a listen-only word |
| `syllables` | Split by `-` (`ma-ma`, `pi-an-o`); they must join back into the text |
| `picture` | A picture id; empty for a word with none (a reading-only word such as `am`) |
| `listen_only` | `yes` for a word only heard and pictured, never read (its letters needn't be taught; it needs a picture) |
| `twin` | The other language's word that is alike (`ro-planeta` ↔ `en-planet`); both rows name each other |
| `false_friend` | The other language's word spelled the same with another meaning (`ro-pat` ↔ `en-pat`); both rows name each other |
| `retired`, `notes` | See above |

```
id,language,text,letters,syllables,picture,listen_only,twin,false_friend,retired,notes
ro-mama,ro,mama,m a m a,ma-ma,mother,,,,,TraceWord
```

Each word needs its **word** line (`ro-word-mama`, saying exactly the
word's text); a word in a `Syllables` step also needs its **syllables**
line (`ro-syllables-mama`), and a word in a `FirstSound` step its
**stretched** line (`ro-stretched-mama`: its first sound held, "mmmama", or
bounced when it can't be held, "t-t-tata"). Its **slow** line
(`ro-slow-mama`) is optional.

### `pictures.csv`: one row per picture

| Column | What it holds |
| --- | --- |
| `id` | Named by what it shows, in no language (`bed`, `moon`); twins share one |
| `art` | Its PNG file in `content/pictures/` (`bed.png`); empty until it is drawn, and the importer draws a placeholder (a plain coloured circle, with no letter, so it never gives an answer away) |
| `retired`, `notes` | See above |

```
id,art,retired,notes
bed,,,ro pat
```

### `voice-lines.csv`: one row per thing the app says

| Column | What it holds |
| --- | --- |
| `id` | `<language>-<kind>-<key>` (`ro-sound-m`, `en-word-pat`, `ro-line-try-again`); it must match the next two columns |
| `language` | `ro` or `en` |
| `kind` | `sound`, `name`, `word`, `slow`, `syllables`, `stretched` or `line` |
| `text` | What is said (and shown in the bubble), a whole, correct sentence for a line (AGENTS.md → "Romanian, checked"); `{x}`, `{y}`, `{word}`, `{slow}`, `{stretched}`, `{name}` are spliced parts |
| `clip` | Its recordings' file names in `content/voice/`, one per spoken part of the text, separated by spaces; empty while the tablet's voice says it. A line without `{…}` has one clip, `<id>.ogg` (`ro-sound-m.ogg`); a line with spliced parts has one per part of its own text, `<id>.1.ogg <id>.2.ogg …` ("Atinge litera {x}." has one part, "Atinge litera"; a "." or "!" alone isn't a part). **Accept recording session** fills it. A named clip that isn't there yet is a warning |
| `status` | **Filled by the importer, never typed:** `recorded` (every clip named is in `content/voice/`), `missing` (a named clip isn't there), `sounds` (a slow word with no clip: the app says its letters' sounds), or empty (still to record: the tablet's voice says it) |
| `retired`, `notes` | See above |

```
id,language,kind,text,clip,status,retired,notes
ro-line-try-again,ro,line,Mai încearcă o dată!,,,,
ro-sound-m,ro,sound,/m/,ro-sound-m.ogg,recorded,,
```

The spliced parts are `{x}` and `{y}` (a letter, usually its sound), `{word}`,
`{slow}`, `{stretched}` (a word) and `{name}` (the profile's name, said by
the tablet's voice); an `…` between them is a short pause.

### `units.csv`: one row per unit (a planet)

| Column | What it holds |
| --- | --- |
| `id` | `planet-1` |
| `domain` | The domain it belongs to: `reading` (later `numbers`, `music`) |
| `retired`, `notes` | See above |

```
id,domain,retired,notes
planet-1,reading,,a m t p
```

### `lessons.csv`: one row per lesson

| Column | What it holds |
| --- | --- |
| `id` | `ro-m`, `en-words-1`, `party-1` |
| `unit` | Its unit's id |
| `language` | `ro`, `en`, or `both` for a lesson that mixes them (a planet party) |
| `introduces` | The one new letter it teaches (in its language's track), or empty; a `both` lesson introduces none |
| `retired`, `notes` | See above |

```
id,unit,language,introduces,retired,notes
ro-m,planet-1,ro,m,,m in Romanian
```

A **track** is a language's lessons (and the `both` lessons) in teaching
order; a word in a step may only use letters introduced earlier in its
language's track, or by the step's own lesson, unless it is listen-only.

### `steps.csv`: one row per step

| Column | What it holds |
| --- | --- |
| `lesson` | Its lesson's id; a lesson's steps run in the order of their rows |
| `kind` | `MeetLetter`, `ListenAndPick`, `LetterHunt`, `FirstSound`, `TraceLetter`, `TraceWord`, `Syllables`, `BuildWord`, `ReadAndMatch`, `TwoSounds` or `WordTwins` |
| `language` | Empty: the lesson's. In a `both` lesson, `ro` or `en`. `TwoSounds` and `WordTwins` are always in both (leave it empty) |
| `letters` | Letter ids, separated by spaces, in the order the kind reads them |
| `words` | Word ids, separated by spaces, in the order the kind reads them |
| `options` | `key=value` pairs separated by `;` (`TraceLetter`: `form=print` or `handwritten`, and `case=lower` or `upper`, both required; `TraceLetter` and `TraceWord`: `rounds=1`, `2` or `3`, the first rounds only, all three without it) |
| `notes` | See above |

```
lesson,kind,language,letters,words,options,notes
ro-m,TraceLetter,,m,,form=handwritten;case=lower,
```

What each kind needs (how many letters and words, pictures, its
instruction lines such as `ro-line-pick-letter`) is in
`Scripts/Core/Content/ActivityKinds.cs`; the import says which is missing.

- **The letters a step chooses among are taught letters** (`ListenAndPick`,
  `LetterHunt`, `FirstSound`): each is taught by the step's lesson in its
  language, each once, so a wrong choice is never a letter still to come
  (the human, 2026-11-24). The first letter is the one asked for
  (`ListenAndPick`, `LetterHunt`); `FirstSound`'s answer is the letter its
  word starts with.
- **`TwoSounds`** takes one letter whose sound differs between the
  languages (`vowel-differs` or `sound-differs`) and 4 to 6 words of both
  languages, each starting with that letter and with that letter's sound in
  its language (the import can't hear that: check it).

### `stickers.csv`: one row per sticker

| Column | What it holds |
| --- | --- |
| `id` | Named by what it shows (`guitar`, `whale-rainbow`) |
| `retired`, `notes` | See above (a retired sticker is no longer given; she keeps hers) |

```
id,retired,notes
guitar,,an instrument
```

Its picture is `game/Assets/Scolita/Art/Stickers/sticker-<id>.png` (made by `look`'s tools,
`design/README.md`); a sticker without one fails the import. A picture redrawn under the same name
needs no change here. A sticker comes every second finished lesson and at every planet's end,
never one she already has until she has them all (`Scripts/Core/Rewards`).

### `constellations.csv`: one row per planet's constellation

| Column | What it holds |
| --- | --- |
| `id` | Named by what it shows (`unicorn`, `whale`) |
| `unit` | The unit (planet) whose lessons light it: one constellation per unit that has lessons |
| `stars` | Its figure's stars as `x:y` in a 640 × 480 frame (x to the right, y down from the top left), separated by spaces: `470:56 418:150 356:146`; at least 3 |
| `lines` | Its figure's lines, as two star numbers (from 1) joined by `-`, separated by spaces: `1-2 2-3 3-1` |
| `retired`, `notes` | See above |

```
id,unit,stars,lines,retired,notes
rocket,planet-2,320:50 262:168 378:168 262:318 378:318,1-2 1-3 2-4 3-5,,
```

It is drawn from these numbers (no picture). Each lesson of its planet lights 3 places of it:
its stars first, then places along its lines, so a planet with more lessons fills the same figure
more densely. It needs two lines in each language of its planet's lessons:
`<lang>-line-stars-<id>` (said at a lesson's end: "Bravo! Ai aprins 3 stele în constelația
Unicornului.") and `<lang>-line-constellation-<id>` (its name, said when she taps it in the sky
book).

### `trails.csv`: one row per trail for the rocket

| Column | What it holds |
| --- | --- |
| `id` | Named by what it shows (`rainbow`, `stars`) |
| `retired`, `notes` | See above |

```
id,retired,notes
rainbow,,
```

The order of the rows is the order they are earned: each planet's end gives the next one. Its
picture is `game/Assets/Scolita/Art/Map/trail-<id>.png`, a ribbon repeated behind the rocket; a
trail without one fails the import.

The rewards also need these lines in each language of the lessons: `sticker`, `trail`,
`sky-book`, `sticker-book` and `trail-picker` (`ro-line-sticker`, …).

### `avatars.csv`: one row per profile picture

| Column | What it holds |
| --- | --- |
| `id` | Named by what it shows (`unicorn`, `dog`); a profile names its avatar by this id in `profiles.json` |
| `retired`, `notes` | See above (a retired avatar is no longer offered; a profile that has it keeps it) |

```
id,retired,notes
unicorn,,a white unicorn
```

The pictures a child chooses for her profile (design screen 11), in the order the parent area offers
them; the first is the one the app gives its first profile. Its picture is
`game/Assets/Scolita/Art/Avatars/avatar-<id>.png`; an avatar without one fails the import. Read with
the rewards' files (`Scripts/Core/Rewards`, `RewardTables`), used by `Scripts/Core/Profiles`.

### `strokes/<strokes-id>-<case>-<form>.json`: how a letter is written

One file per letter, case and form: `a-lower-print.json`, `a-upper-print.json`,
`a-lower-handwritten.json`. The first part is the letter's `strokes` cell
in `letters.csv`; `case` is `lower` or `upper` and `form` is `print` or
`handwritten`, as a `TraceLetter` step's options say. The file's name is the
shape's id. A digit (numbers, later) has one case: `7-lower-print.json`.

```json
{
  "status": "draft",
  "source": "where the drawing comes from, for people",
  "slant": 0,
  "strokes": [
    { "runs": [
      { "kind": "body", "points": [[65, 112], [45, 100], [17.5, 108], [7.5, 150], [17.5, 192], [42.5, 200], [62.5, 190], [70, 160], [68, 125]] }
    ] },
    { "runs": [
      { "kind": "body", "points": [[70, 100], [70, 200]] }
    ] }
  ]
}
```

- **The writing lines are the same for every letter:** the top line is
  y 0, the middle line y 100, the baseline y 200 and the line below y 300;
  x starts at 0 on the letter's left. A small letter fills 100 to 200, t
  and the capitals reach 0, p goes down to 300.
- **`strokes`** are in the order she draws them; each is one finger down
  to finger up. A stroke's **`runs`** are drawn without lifting: each run
  is a smooth curve through its `points` ([x, y], in drawing order), and
  two runs meet at a corner (m's turn at the baseline), so a run starts
  where the one before it ends. A stroke of one point is a dot (i's dot).
  The start dot is a stroke's first point; its direction is the order of
  its points.
- **`kind`** is `body` (the letter), `entry` (a handwritten letter's stroke
  from the line into the letter) or `exit` (its stroke to the next letter).
- **`slant`** leans the letter: x += (200 - y) × slant; 0 for print, 0.2
  for the handwritten letters. Write the points upright.
- **`status`** is `draft` until the human has checked the letter against
  her school's manual, then `checked`. Each draft is a warning at every
  import. `source` is for people.
- **At most 4 strokes** (the writing board's start dots), and each stroke
  after the first needs its line (`trace-next-2`, `trace-next-3`, …).
- **Every `TraceLetter` step needs its letter's shape** for its case and form,
  and every letter of a `TraceWord` word its handwritten lower-case shape (an
  import error). A `TraceWord` word has at most 5 letters.
- **Handwritten lower-case letters join** (the Romanian school's "litere de
  mână"), and the import checks what that needs:
  - an `entry` run, when there is one, is the first run of the first stroke
    and starts on the baseline (y 200); m and n have none (they start at the
    middle line with their hook);
  - one `exit` run at most, the last run of the letter's **line**, ending
    between the middle line and the baseline (o leaves at the middle line);
  - the strokes of the line, up to the exit, each begin where the one before
    ends (half a unit at most): she may lift between them (each has its
    numbered start dot) or write them in one line, as a word needs. The
    handwritten a is three such strokes: the entry, the round, the stem with
    its exit (the human, 2026-11-24);
  - the strokes after the exit are the letter's **marks** (i's dot, t's bar):
    in a word they are written after the whole word, and each needs its line
    (`trace-dot-<strokes id>`, `trace-bar-<strokes id>`).
- **Joining a word** needs no file: each letter after the first starts at the
  previous exit's end (its entry's first point moved there, or a join run to
  its body for m and n, and after o's high exit). Print shapes have only
  `body` runs.
- Shapes are not in the ledger of released ids: her progress names letters,
  never a shape, so a shape can be redrawn or replaced. A shape file removed
  (or renamed) has its imported asset removed at the next import.

## The ledger of released ids

`released-ids.txt` lists every id ever released, one per line as
`<file> <id>` (`words.csv ro-mama`), under a `# <version>` line for the
release that first shipped it (`#` lines are comments). It makes sure an id
of a release is never deleted, even together with its imported asset:

- **The importer refuses** an import where an id in the ledger is missing
  from its CSV (`words.csv: the id ro-tata was released (released-ids.txt,
  row 14), but its row is gone …`): put the row back and mark it retired.
  A retired id is fine; a new id, not released yet, is fine.
- **Each release's close-out adds its new ids,** after raising `VERSION`:
  **Școlița → Record released ids** (or, in batch mode,
  `-executeMethod Scolita.Editor.ContentImport.ReleasedIdsRecorder.RecordFromCommandLine`)
  adds every id not in the ledger yet under the new version, and nothing
  when there is none. Commit the ledger with the close-out.
- **Record released ids refuses** to add ids under a version that isn't
  after the ledger's latest `# <version>` line: raise `VERSION` first.
- **Never edit, reorder or delete a line** by hand. If the ledger conflicts
  after a rebase, someone released first: take `origin/main`'s ledger,
  raise `VERSION` to the next free number, and run Record released ids
  again.

## Folders

- `pictures/`: the pictures' art (PNG, in Git LFS), named in `pictures.csv`.
- `released-ids.txt`: the ledger of released ids (above).
- `strokes/`: the letters' shapes, one JSON file per letter, case and form
  (above).
- `voice/`: the recorded clips (mono OGG Vorbis, 44.1 kHz, AGENTS.md →
  "Audio"), named in `voice-lines.csv`. They come from the recording kit
  (Școlița → Make recording scripts, Import recording session, Accept
  recording session; `local/todo/04-record-voices.md`), or from any other
  source named the same way. Check them with Școlița → Check voice clips.
