# Task: feature/content-letters-1

> **Status: Done 2026-11-25, released as 0.8.1** (tag `v0.8.1`). The human's ticks of the review table are a
> step in `local/for-human-next-steps.md`.

Read `AGENTS.md` first (especially "Learning", "Audio", "The human's next
steps" → the child's name), `docs/curriculum.md` v1, and the completion
records in `tasks/done/`.

**Feature:** `feature/content-letters-1`
**Implements:** `docs/curriculum.md` §3 (planets 1–3), §4.1, §6, §8.
**Depends on:** `curriculum-research`, `content-model`; `tracing` and
`handwriting` for the strokes' format.

## Scope

1. **Planets 1–3** (a m t p, s i n, o d e) in both languages, in the
   content CSVs: letters (sounds, names, groups), words (letters,
   syllables, pictures, twins, listen-only), lessons (the shape in
   `curriculum.md` §4.1: per letter a Romanian and an English lesson, a
   word lesson per language per planet, a planet party), stickers and
   constellations for the three planets.
2. **Strokes** for the 10 letters, **print and handwritten** (lower case;
   upper case if D2 says so), the handwritten ones with their entry and
   exit strokes, as `curriculum-research` records the Romanian school's
   model; and the `TraceWord` words of each planet (mama, tata, pat …).
3. **Pictures** for every readable word, in the look's style (from
   `look`, or placeholders in its palette until then), each named by what
   it shows.
4. **Voice lines** for everything: the list complete in
   `voice-lines.csv`; clips as the human records them (the guide is
   `local/todo/04-record-voices.md`): the agent trims, normalises and
   names them with **Check voice clips**, and listens to every one.
5. **A human review:** every Romanian and English word, syllable split and
   line checked by the human (a table in Progress for them to tick).

## Out of scope — do not build

- Planets 4 and on (`content-letters-2`), code changes (a missing feature
  goes to a follow-up).

## Acceptance criteria

- The validator passes with no warnings except clips not yet recorded,
  which are listed in Progress.
- Every word on planets 1–3 is readable with the letters taught before it.
- The human has ticked the review table.
- She can play planets 1–3 from start to end in "both" mode on the Tab A7
  (after `parent-area`).

## Tests required

- EditMode: the validator and freshness test; each activity kind's rules
  run over every step of planets 1–3 (a test that walks the content).
- Listening: every line, both languages.

## Touches

`content/` (all CSVs, `strokes/`, `voice/`, pictures), the imported
assets, the catalogs.

## Screens

None new.

## Voice lines

All of planets 1–3 (several hundred lines: the list is in `voice-lines.csv`).

## Before closing out

- `local/for-human-next-steps.md`: the lines still to record, grouped by
  planet, so a recording session can do one planet at a time.
- Release (the next PATCH if only content; MINOR if it completes the MVP's
  content alongside `sound-feel`).

## Progress

_Kept by `content-letters-1` while the brief is in flight._

### Status

- **Claimed** in `78bb88d`.
- **Built** (content only, no app code): planets 2 and 3 (s i n, o d e) in both languages, their 24 letter
  shapes (all `draft`), the recorded handwritten test lines, and a test that plays every lesson to its end.
- **Reviewed** by a fresh-eyes agent (below); its findings fixed or put to the human.
- **Pushed** together, tested at the last commit: the hashes are in the completion record.

### Design note

A **large content change, no code change**: the content model, the importer and the lesson engine are unchanged.

- **Where it lives:** only `content/` (the seven CSVs and `strokes/`) and what the importer makes of it under
  `Assets/Scolita/Content/`. Each planet copies planet 1's shape (curriculum §4.1): per letter a Romanian lesson
  (`MeetLetter`, `ListenAndPick`, `LetterHunt`, `TraceLetter` print, `TraceLetter` by hand, `FirstSound`) and an
  English one (`MeetLetter`, `TwoSounds` for i, o and a light one for e, `LetterHunt`, one round by hand,
  `FirstSound`); a word lesson per language (`Syllables`, `BuildWord`, `ReadAndMatch`, `TraceWord`; plain words
  first, Romanian diphthongs and English two-consonant words last); a capitals lesson (three hunts, each capital
  in print and by hand); a planet party in both languages (pick the letter, read and match, one word by hand per
  language, `WordTwins`).
- **The data it needs:** every readable word of curriculum §3 with a picture (Romanian 13 + 21, English 10 + 17),
  the listen-only words for meeting a letter, its first sound and the twins (Romanian 10, English 16); 71 new
  placeholder pictures, named by what they show (twins and words that show the same thing share one: `sun`,
  `star`, `nose`, `mother`, `father`, `elephant`, `dolphin`, `piano`, `rocket` …); every voice line those use.
- **How it is tested:** the import (the validator: every step's letters taught, words readable, shapes and lines
  there, pairs one to one); `RealLessonsTests` plays **every step of every lesson** of planets 1–3 to its end with
  right answers through `LessonRun` (no step skipped, none unfinishable, no right answer judged wrong, tracing
  along each figure); `HandwrittenStrokeTests` with recorded lines for s i n o d e and the joined words pom and
  dinte, dot and stea; the emulator.
- **Another domain** (numbers, music) would add its own CSV rows the same way; nothing here is specific to letters
  beyond the data.

### Decisions while building

- **No app code changed.** Four EditMode tests that pinned planet 1 as the whole content now look at planet 1 only
  (three in `LessonOrderTests`, one in `CurriculumAssetsTests`); new tests: `RealLessonsTests` (one case per lesson),
  `CurriculumAssetsTests.TestBuild_PlanetsTwoAndThreeFromTheCatalogs`, planet 1's party leading to `ro-s`, and the
  recorded lines.
- **Shapes** (`content/strokes/`, all `draft`): the curriculum draft's points (`design/research/strokes-draft.json`)
  as drawn, except where planet 1's agreed patterns or an eye check said otherwise:
  - **print d in two strokes** like the print a (the human, 2026-11-24): the round, then the stem from the top line;
  - **handwritten d in three strokes** like the handwritten a: the entry up the round's left side and over its
    top, the round back over it and on up to the top line, the stem down with its exit;
  - **handwritten o's entry** goes up the round's left side as the a's does (the draft's entry cut across the
    round, so it read as „ø”); two strokes, the exit leaving at the middle line;
  - **handwritten e's loop widened** (the draft's read as an l), the entry rising into it as its bar;
  - **handwritten S redrawn** (the draft read as a J): the print S's line, leaning, with a curl at its end; the
    **handwritten E's** middle loop made larger;
  - **capitals by hand don't join** (as P and T): the hooks of S and I and N's top flourish are `body` runs; E keeps
    its exit (as A and M).
- **What was left out, and why:**
  - the words without a picture (Romanian am; English at, am, in, it, is, on, not, tip, sat) and the names (Ana,
    Sam, Pam): every word activity shows a picture, and names need capitals in words (`handwriting-2`); so the
    false friends sat / sat are not paired (English sat has no picture);
  - planet 1's other words (pata, papa, pa, tam-tam), its other two TraceWord words per language, and capitals for
    "English only" (`capitals-1/2/3` are Romanian lessons, so "English only" meets no capitals at all): planet 1 is
    unchanged; each is a question in the table below and a follow-up in the completion record;
  - stickers and constellations: there is no content for them yet (`rewards` makes their format).
- **A TraceWord word per planet and language is not traced:** the word lesson traces two, the party one, to keep each
  lesson near 5 minutes; ro sat, en tin, ro pod and en mop (and planet 1's ro pat, apa, en tap, pat) are in
  `words.csv` but no step traces them (nothing schedules reviews yet: `progress-review`). A question below.
- **Two backwards lines are taken for "no entry"** (recorded, `HandwrittenStrokeTests`): an o drawn backwards from its
  high exit ends near the round's start (round 1 in the recorded line; rounds 1 and 2 on the emulator), and an e
  joined in a word drawn backwards does too, in every round (the join's wider start radius); she hears „Pornește de pe
  linie.” instead of „Urmează săgeata.”. Fine for now; a question below.
- **The adb server was restarted by something else** several times during the emulator checks (another project's
  build, not this brief's): a drawn line cut short was redrawn; no app problem.

### Reaching planets 2–3

Go opens the next playable lesson in teaching order (`LessonOrder`), so planets 2–3 follow planet 1's party with no
code change; there is no map to jump to them yet (`galaxy-map`). On the emulator they were reached by writing the
lesson in progress (`lesson-run.json`, "How to test locally").

### Fresh-eyes review (2026-11-25)

A second agent, which didn't write the change, read it against AGENTS.md, the curriculum and planet 1's patterns. No
blocker: every readable word uses only taught letters, one new letter per lesson, wrong choices only taught letters,
the TwoSounds and FirstSound words start with the right sound (British orange /ɒ/), twins and false friends name each
other, the Romanian syllable splits are right, no cedilla; the shapes start and turn as planet 1's do. Its findings
and what was done:

| Finding | Done |
| --- | --- |
| Progress named sections not written yet and said "pushed" too early; three tests, not four | Reworded (Status, Decisions) |
| Scope left out without the human: planet 1's other words and TraceWord words, stickers and constellations, capitals in "English only" | Each is a question in the table below and a follow-up |
| False friends mops and sat in no game, not asked | A question below |
| "Left for reviews" is wrong: nothing schedules reviews | Reworded; a question below |
| `RealLessonsTests` claims more than it proves, walks all lessons in one case | One case per lesson (`TestCaseSource`), the claim narrowed to "playable and finishable", plus a test that every lesson is walked |
| Planet 1 → planet 2 not pinned | `LessonOrderTests` asserts `After("party-1")` is `ro-s`; `CurriculumAssetsTests` checks planets 2–3 |
| No recorded line for o's high exit into an entry (dot) or e → a (stea) | Recorded `en-dot` and `ro-stea` (all verdicts as meant) |
| Nits: the backwards e, ant's two consonants, pictures that could be confused (nani's moon, pas's footprint, ten / dot / spot), held short vowels, numbers in the test | The first two are questions below; the pictures go to `look` (named in the follow-ups); the held vowels are already asked; the test's numbers are named |

### For the human to check (tick each)

Every new word, syllable split, picture and line. The general lines (instructions, hints, praise) are planet 1's,
already in `voice-lines.csv`: no new sentence was written.

**The ones the agent is not sure of (answer these first):**

| ✓ | What | The question |
| --- | --- | --- |
| ☐ | The 24 shapes | Do s i n o d e and S I N O D E, in print and by hand, start and go as her manual shows? Above all: the print d in two strokes; the handwritten d (as the a) and o (entry up the left side); the e's loop; the capital S redrawn; the capital I (it looks like a J); the capital E's middle loop |
| ☐ | ochi, ochelari | The o's words to meet and hear (ochelari, ochi): both start with a clean /o/; „ou” and „oaie” were left out (diphthongs) |
| ☐ | Held vowels | The stretched lines say the vowel held: „iiinel”, „ooochi”, „eeelicopter”, "iiigloo", "ooorange", "eeelephant" |
| ☐ | pants | British "pants" are underpants: a funny picture, or swap the word? |
| ☐ | e's TwoSounds | e sorts elefant, elicopter, egg, elephant: the "light" one (the sounds are close); elefant / elephant are twins |
| ☐ | Party length | The parties have 7 and 8 steps (planet 1's had 5): two word twins games on planet 3 |
| ☐ | Backwards o and e | An o drawn backwards, and an e in a word drawn backwards, hear „Pornește de pe linie.” instead of „Urmează săgeata.” (see above): acceptable? |
| ☐ | Untraced words | ro sat, pod and en tin, mop (planets 2–3) and planet 1's pat, apa, tap, pat are never traced: add them to the parties (longer), or wait for reviews (`progress-review`)? |
| ☐ | Planet 1's other words | pata, papa, pa, tam-tam are not in the content yet (planet 1 was left as it is): add them in a later chore? |
| ☐ | Capitals in "English only" | The capitals lessons are Romanian, so "English only" never meets S I N, O D E (or A M T P): an English capitals lesson (but then "both" mode does them twice), or a lesson that plays in "English only" only (a code change)? |
| ☐ | False friends sat and mops | sat / sat is not paired (English sat has no picture); mops / mops are paired in `words.csv` but in no game (each party's twins game holds 4 pairs at most): swap one in? |
| ☐ | ant | ant has two consonants together (nt) but sits with the plain words (curriculum §3 lists it there): fine? |
| ☐ | Stickers and constellations | Not in this content: their format comes with `rewards` |


**Every new word** (its syllables as the drum beats them, its picture, and whether she reads it or only hears it):

| ✓ | Word | Lang | Syllables | Picture | Read or listen | Twin / false friend | Note |
| --- | --- | --- | --- | --- | --- | --- | --- |
| ☐ | nas | ro | nas | nose | read |  | TraceWord |
| ☐ | masa | ro | ma-sa | table | read |  | 'the table' (masă needs ă); TraceWord |
| ☐ | pian | ro | pi-an | piano | read | twin en-piano | hiatus: two syllables; word twin; TraceWord |
| ☐ | nai | ro | nai | pan-flute | read |  | diphthong ai: one syllable |
| ☐ | pin | ro | pin | pine-tree | read | false friend en-pin | a pine tree; false friend |
| ☐ | sat | ro | sat | village | read |  | a village (en sat has no picture, so no false friend here) |
| ☐ | ananas | ro | a-na-nas | pineapple | read |  |  |
| ☐ | mami | ro | ma-mi | mother | read |  | familiar (kept by the human, 2026-11-24) |
| ☐ | tati | ro | ta-ti | father | read |  | familiar (kept by the human, 2026-11-24) |
| ☐ | nani | ro | na-ni | sleeping-moon | read |  | familiar: 'sleep' (hai la nani) |
| ☐ | pas | ro | pas | footprint | read |  | a step |
| ☐ | spin | ro | spin | thorn | read |  | a thorn |
| ☐ | sita | ro | si-ta | sieve | read |  | 'the sieve' (sită needs ă) |
| ☐ | pom | ro | pom | fruit-tree | read |  | a fruit tree; TraceWord |
| ☐ | pod | ro | pod | bridge | read | false friend en-pod | a bridge; false friend; TraceWord |
| ☐ | dinte | ro | din-te | tooth | read |  | TraceWord |
| ☐ | stea | ro | stea | star | read |  | diphthong ea: one syllable; TraceWord |
| ☐ | mop | ro | mop | mop | read | twin en-mop | word twin |
| ☐ | dop | ro | dop | cork | read |  | a cork |
| ☐ | pepene | ro | pe-pe-ne | watermelon | read |  |  |
| ☐ | domino | ro | do-mi-no | domino | read | twin en-domino | word twin |
| ☐ | desen | ro | de-sen | drawing | read |  | a drawing |
| ☐ | dans | ro | dans | dance | read |  | a dance |
| ☐ | nota | ro | no-ta | music-note | read |  | 'the note' (notă needs ă) |
| ☐ | ponei | ro | po-nei | pony | read | twin en-pony | diphthong ei; word twin |
| ☐ | pisoi | ro | pi-soi | kitten | read |  | diphthong oi |
| ☐ | panda | ro | pan-da | panda | read | twin en-panda | word twin |
| ☐ | piton | ro | pi-ton | python | read |  |  |
| ☐ | tenis | ro | te-nis | tennis | read | twin en-tennis | word twin |
| ☐ | sanie | ro | sa-ni-e | sled | read |  | hiatus: three syllables |
| ☐ | patine | ro | pa-ti-ne | skates | read |  |  |
| ☐ | mops | ro | mops | pug | read | false friend en-mops | a pug; false friend |
| ☐ | nod | ro | nod | knot | read | false friend en-nod | a knot; false friend |
| ☐ | stop | ro | stop | stop-sign | read | twin en-stop | word twin |
| ☐ | soare | ro | soa-re | sun | listen only |  | s |
| ☐ | insulă | ro | in-su-lă | island | listen only |  | i |
| ☐ | inel | ro | i-nel | ring | listen only |  | i |
| ☐ | nor | ro | nor | cloud | listen only |  | n |
| ☐ | ochelari | ro | o-che-lari | glasses | listen only |  | o |
| ☐ | ochi | ro | ochi | eye | listen only |  | o |
| ☐ | delfin | ro | del-fin | dolphin | listen only | twin en-dolphin | d; word twin |
| ☐ | elefant | ro | e-le-fant | elephant | listen only | twin en-elephant | e; word twin |
| ☐ | elicopter | ro | e-li-cop-ter | helicopter | listen only |  | e |
| ☐ | rachetă | ro | ra-che-tă | rocket | listen only | twin en-rocket | word twin |
| ☐ | pin | en | pin | pin | read | false friend ro-pin | a pin; false friend; TraceWord |
| ☐ | sit | en | sit | sitting | read |  | TraceWord |
| ☐ | man | en | man | man | read |  | TraceWord |
| ☐ | tin | en | tin | tin-can | read |  | a tin can; TraceWord |
| ☐ | pan | en | pan | pan | read |  | a frying pan |
| ☐ | ant | en | ant | ant | read |  |  |
| ☐ | nap | en | nap | nap | read |  | a child napping |
| ☐ | sip | en | sip | sip | read |  | sipping a drink |
| ☐ | mint | en | mint | mint-leaf | read |  | two consonants together: last |
| ☐ | pants | en | pants | pants | read |  | two consonants together: last; underpants (British English) |
| ☐ | dot | en | dot | dot | read |  | TraceWord |
| ☐ | pen | en | pen | pen | read |  | TraceWord |
| ☐ | mop | en | mop | mop | read | twin ro-mop | word twin; TraceWord |
| ☐ | ten | en | ten | ten | read |  | the number ten; TraceWord |
| ☐ | pot | en | pot | pot | read |  | a cooking pot |
| ☐ | top | en | top | spinning-top | read |  | a spinning top |
| ☐ | net | en | net | net | read |  |  |
| ☐ | dad | en | dad | father | read |  |  |
| ☐ | pet | en | pet | pet | read |  | a pet (a puppy in its basket) |
| ☐ | pod | en | pod | pea-pod | read | false friend ro-pod | a pea pod; false friend |
| ☐ | nod | en | nod | nodding | read | false friend ro-nod | nodding; false friend |
| ☐ | tent | en | tent | tent | read |  | two consonants together: last |
| ☐ | nest | en | nest | nest | read |  | two consonants together: last |
| ☐ | pond | en | pond | pond | read |  | two consonants together: last |
| ☐ | stop | en | stop | stop-sign | read | twin ro-stop | two consonants together: last; word twin |
| ☐ | spot | en | spot | spot | read |  | two consonants together: last; a paint spot |
| ☐ | mops | en | mops | mops | read | false friend ro-mops | two consonants together: last; more than one mop; false friend |
| ☐ | star | en | star | star | listen only |  | s |
| ☐ | sun | en | sun | sun | listen only |  | s |
| ☐ | insect | en | in-sect | insect | listen only |  | i |
| ☐ | igloo | en | ig-loo | igloo | listen only |  | i |
| ☐ | nose | en | nose | nose | listen only |  | n |
| ☐ | octopus | en | oc-to-pus | octopus | listen only |  | o |
| ☐ | orange | en | or-ange | orange | listen only |  | o: British /ɒ/ |
| ☐ | dolphin | en | dol-phin | dolphin | listen only | twin ro-delfin | d; word twin |
| ☐ | egg | en | egg | egg | listen only |  | e |
| ☐ | elephant | en | el-e-phant | elephant | listen only | twin ro-elefant | e; word twin |
| ☐ | piano | en | pi-an-o | piano | listen only | twin ro-pian | word twin |
| ☐ | rocket | en | rock-et | rocket | listen only | twin ro-racheta | word twin |
| ☐ | panda | en | pan-da | panda | listen only | twin ro-panda | word twin |
| ☐ | pony | en | po-ny | pony | listen only | twin ro-ponei | word twin |
| ☐ | tennis | en | ten-nis | tennis | listen only | twin ro-tenis | word twin |
| ☐ | domino | en | dom-i-no | domino | listen only | twin ro-domino | word twin |

**Every new sound, name, stretched and syllables line** (the word lines say the word's text, the slow lines its letters' sounds):

| ✓ | Line | Says |
| --- | --- | --- |
| ☐ | `ro-sound-s` | /s/ |
| ☐ | `ro-name-s` | se |
| ☐ | `ro-sound-i` | /i/ |
| ☐ | `ro-name-i` | i |
| ☐ | `ro-sound-n` | /n/ |
| ☐ | `ro-name-n` | ne |
| ☐ | `ro-sound-o` | /o/ |
| ☐ | `ro-name-o` | o |
| ☐ | `ro-sound-d` | /d/ |
| ☐ | `ro-name-d` | de |
| ☐ | `ro-sound-e` | /e/ |
| ☐ | `ro-name-e` | e |
| ☐ | `en-sound-s` | /s/ |
| ☐ | `en-name-s` | ess |
| ☐ | `en-sound-i` | /ɪ/ |
| ☐ | `en-name-i` | eye |
| ☐ | `en-sound-n` | /n/ |
| ☐ | `en-name-n` | en |
| ☐ | `en-sound-o` | /ɒ/ |
| ☐ | `en-name-o` | oh |
| ☐ | `en-sound-d` | /d/ |
| ☐ | `en-name-d` | dee |
| ☐ | `en-sound-e` | /e/ |
| ☐ | `en-name-e` | ee |
| ☐ | `ro-stretched-nas` | nnnas |
| ☐ | `ro-stretched-sat` | sssat |
| ☐ | `ro-syllables-ananas` | a-na-nas |
| ☐ | `ro-syllables-sita` | si-ta |
| ☐ | `ro-syllables-pepene` | pe-pe-ne |
| ☐ | `ro-stretched-dans` | d-d-dans |
| ☐ | `ro-syllables-patine` | pa-ti-ne |
| ☐ | `ro-stretched-inel` | iiinel |
| ☐ | `ro-stretched-ochi` | ooochi |
| ☐ | `ro-stretched-elicopter` | eeelicopter |
| ☐ | `en-stretched-nap` | nnnap |
| ☐ | `en-stretched-dad` | d-d-dad |
| ☐ | `en-stretched-sun` | sssun |
| ☐ | `en-stretched-igloo` | iiigloo |
| ☐ | `en-syllables-octopus` | oc-to-pus |
| ☐ | `en-stretched-orange` | ooorange |
| ☐ | `en-stretched-elephant` | eeelephant |
| ☐ | `en-syllables-piano` | pi-an-o |

## How to test locally

Run the commands in Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb.exe"`, `S=<your scratch folder>` and, so Git Bash leaves the
device paths alone, `export MSYS_NO_PATHCONV=1`. Close your Editor on this project before any batch-mode run. Reset
the app's saves with `"$ADB" -s <emulator> shell pm clear com.softshack.scolita`.

### Content slice (planets 2–3)

1. **The import:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`.
   Expected: exit 0, `imported, with 128 warnings` (88 pictures still to draw, 40 draft shapes: planet 1's 16 and the
   24 new ones; no step skipped). `git status` shows no change.
2. **A word with a letter not taught yet:** in `content/steps.csv`, change the `ro-words-2` row
   `ReadAndMatch,,,ro-masa ro-pin ro-sat` to `ro-masa ro-pin ro-dop` and import. Expected: exit 1, `steps.csv, row 103:
   the word ro-dop uses d o, not taught yet in the ro track by the lesson ro-words-2 (taught: a m t p s i n); mark the
   word listen_only if it is only heard here.` and `1 errors, 128 warnings: nothing was imported`. Then
   `git checkout content`.
3. **The automated tests** (EditMode, see AGENTS.md → "Testing before a push" for the PlayMode and build lines):
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: all pass, among them `RealLessonsTests.TestAnswer_TheLessonToItsEndWithRightAnswers` once per lesson
   (32 lessons: every step of planets 1–3 played to its end), `HandwrittenStrokeTests` for s i n o d e, pom, dinte,
   dot and stea, `CurriculumAssetsTests.TestBuild_PlanetsTwoAndThreeFromTheCatalogs` and
   `LessonOrderTests.TestAfter_TheNextPlayableLesson` (planet 1's party leads to `ro-s`). No `warning CS` or
   `error CS` in the log.

**On the emulator** (the debug APK installed, `wm size 2000x1200`, landscape). Planets 2–3 come after planet 1's
party with **Hai!** (there is no map yet). To jump to a step: start the app once, `"$ADB" -s <emulator> root`, then
write the lesson in progress and restart the app:

```
"$ADB" -s <emulator> shell "echo '{\"version\": 1, \"lessonId\": \"ro-s\", \"stepNumber\": 4, \"stepsSignature\": \"c1f5882409f36125\"}' > /data/media/0/Android/data/com.softshack.scolita/files/profiles/child-1/lesson-run.json"
```

(`chown` it to the folder's owner, `ls -ln` shows it). The signatures (`LessonSignature.Of`): ro-s `c1f5882409f36125`,
en-s `b8f9f10741b05206`, ro-i `7be93f900ae47436`, en-i `9bf0abb0c54f6643`, ro-n `1415bca231b01d0f`, en-n
`793d9c84aa427674`, ro-words-2 `fc7136f1d13c36ac`, en-words-2 `f83a1b4870f0c9d6`, capitals-2 `943d44d05ae3ba81`,
party-2 `8843d6fbb53dbd5c`, ro-o `fbd383404bda7a5e`, en-o `9c257132516d13ec`, ro-d `b8713a6d19f43cf2`, en-d
`3fe7d596ce829a62`, ro-e `cd95112c8ed3982c`, en-e `9d35091825e55340`, ro-words-3 `760769ea6be7726d`, en-words-3
`3cde25ae71815c65`, capitals-3 `fbcbe0470a41816f`, party-3 `68c02fdbbf1ba4dd`. Lines were drawn with
`input motionevent` chains along the shapes (at 2000 × 1200 the writing lines are at y 434, 651, 870, 1088: 2.18 px a
unit; x from the start dot); the bubble's length is in the log (`Voice: Bubble ro, <n> characters`). Results, all run
on `emulator-5602` (Pixel Tablet, Android 16, the tablet's voice not installed, so the lines show in the bubble):

1. **ro-s, the print s (step 4):** the dotted s, dot 1 at its top right, the arrow left. A good s: „Bravo, ai scris
   litera s! Acum scrie pe umbră.” (46); drawn **backwards**: „Urmează săgeata.” (16); then rounds 2 and 3 good: „…
   Acum scrie fără ajutor, de la punct.” (62), „Bravo, ai scris litera s!” (25), a praise.
2. **ro-s, the s by hand (step 5):** the "tip I" lines, dot 1 on the baseline, the leaning s. Three good lines in one:
   „Bravo, ai scris litera s de mână! Acum scrie pe umbră.” (54), the round-3 line (70), „Bravo, ai scris litera s de
   mână!” (33), „Ești o stea!”.
3. **ro-s, FirstSound (step 6, sat):** s, m, p; m first: the hint „Ascultă primul sunet: sssat.”, m fades; then s:
   a praise and „Ai terminat lecția. Bravo!”.
4. **en-s from the start:** "Let's meet a new letter! Listen: s… s… star! Tap the letter to hear it again." with
   the s, S and the s by hand, and the star's card; the hunt's stars carry s, S, a, A, m, M (moving: catching them
   with `input tap` is hit and miss); the s by hand, one round: "Well done, you wrote the letter s by hand!"; FirstSound
   "… sun. Tap the letter the word starts with." with t, a, s: s, "You finished the lesson. Well done!".
5. **ro-o, the print o (step 4):** dot 1 at the top, anticlockwise; three good rounds as for the s.
6. **ro-o, the o by hand (step 5):** dot 1 on the baseline, the entry up the left side; one line through both strokes
   (54); **backwards**: „Pornește de pe linie.” (21, see Decisions); lifted between the entry and the round: „Foarte
   bine! Acum mergi la punctul cu numărul doi.” (50), then the round-3 line (70); round 3 good (33), a praise.
   FirstSound (ochi) with i, o, a: o.
7. **en-o, TwoSounds (step 2):** the o between the gold RO planet and the blue EN planet; both tapped (each says its
   sound), four pictures sorted to their planets (a wrong drop is hinted), then the hunt (o, O among a, s). The o by
   hand (step 4), one round: "Well done, you wrote the letter o by hand!"; FirstSound (orange): o.
8. **ro-words-3, pom (step 8):** „Scrie cuvântul de mână, fără să ridici degetul. p-o-m”, joined with o's high exit
   into the m. The p alone: „Bravo, ai făcut litera p! Acum leag-o de litera o, fără să ridici degetul.” (74); the o:
   „… litera o! Acum leag-o de litera m …” (74); the m: „Bravo, ai scris cuvântul pom! Acum scrie pe umbră.” (50);
   rounds 2 and 3 in one line (66, 29), a praise.
9. **ro-words-3, dinte (step 9):** the whole word in one line: „Acum pune punctul pe i.” (23); the dot: „Acum trage
   liniuța literei t.” (29); the bar: „Bravo, ai scris cuvântul dinte! Acum scrie pe umbră.” (52).
10. **1280 × 800:** dinte fits the board whole, the word in print and the stars on the right.

**Screen sizes:** 2000 × 1200 for everything, 1280 × 800 for the longest word (dinte). No screen was added or moved
(content only), so the other sizes were not rechecked.

**Not run:** the Device Simulator; the Profiler (no view or effect changed); listening by ear (the emulator has no
Romanian or British voice installed and no clip is recorded yet: every new line was read in the bubble); catching
moving hunt stars by script.

**Cleanup:** `"$ADB" -s <emulator> shell pm clear com.softshack.scolita` and `"$ADB" -s <emulator> shell wm size reset`.

## Completion record

_Finished 2026-11-25 by `content-letters-1`; released as 0.8.1 (tag `v0.8.1`)._

### What was built

- **Pushed:** `78bb88d` the claim; then, tested together at the last of them: `core:` the planet 1 tests read past
  planet 1, `content:` planets 2–3 with their imported assets, `core:` the content walk and the recorded lines,
  `docs:` this close-out (the hashes follow `78bb88d` in `git log`; the tag is on the close-out).
- **Content** (`content/`, imported under `Assets/Scolita/Content/`): units `planet-2`, `planet-3`; letters s i n o d e;
  20 lessons (per letter `ro-x`, `en-x`; `ro-words-2/3`, `en-words-2/3`, `capitals-2/3`, `party-2/3`), 128 steps; 87
  words (61 to read, 26 listen-only), 71 placeholder pictures, 190 voice lines; 24 shapes in `content/strokes/`
  (`{s,i,n,o,d,e}-{lower,upper}-{print,handwritten}.json`, all `draft`). 376 ids in the ledger under `# 0.8.1`.
- **Tests:** `Tests/EditMode/Core/Lessons/RealLessonsTests.cs` (every lesson played to its end, one case per lesson),
  `CurriculumAssetsTests.TestBuild_PlanetsTwoAndThreeFromTheCatalogs`, `LessonOrderTests` (planet 1's party leads to
  `ro-s`), recorded handwritten lines in `Tests/EditMode/Tracing/Strokes/handwritten/` for s i n o d e, pom, dinte,
  dot, stea.

### Conventions for later briefs

- **A planet copies planet 1's shape** (curriculum §4.1): per letter a Romanian and an English lesson, a word lesson
  per language, a capitals lesson, a party; MeetLetter and FirstSound words from her world, listen-only where needed.
- **Round letters by hand enter as the a does** (up the round's left side and over its top), so the entry never
  cuts across the round: a, d, o. A print letter with a round and a stem is two strokes (a, d).
- **Capitals by hand don't join yet:** their hooks to the left are `body` runs; an exit to the right stays (A, M, E).
- **Recorded lines** for a new handwritten shape or joined word are made from the figures (a scratch Editor script
  sampling `TraceFigure.ChainFrom` every 5 units with seeded jitter, as `handwriting`'s), each verdict checked; add
  the figure to `HandwrittenStrokeTests.Figures`.
- **The recording scripts group by planet** (the first planet whose steps say a line), so the human records one
  planet at a time.

### How it was tested

Batch mode on the final tree (EditMode 556/556, was 512; PlayMode 20/20; `BuildAndroidDebug`; no compiler warning), the import and an import
refused for a word with untaught letters, the emulator at 2000 × 1200 (and 1280 × 800 for dinte): ro-s and en-s (print
and handwritten s, backwards, FirstSound with a miss), ro-o and en-o (print and handwritten o in one line, backwards,
lifted; TwoSounds; FirstSound), ro-words-3 (pom letter by letter and in one line, dinte with its dot and bar); a
fresh-eyes review (Progress). Not run: the Device Simulator, the Profiler, listening by ear (no clip and no installed
voice on the emulator; the lines were read in the bubble).

### Decisions and deviations

- **Content only, as the brief says;** the brief's "a test that walks the content" and the planet-agnostic test fixes
  are the only C# (tests).
- **Left out** (each a question for the human, Progress): words without pictures and names, planet 1's other words and
  TraceWord words, capitals in "English only", the false friends sat and mops in a game, stickers and constellations
  (no format before `rewards`).
- **Shapes redrawn from the curriculum draft** where an eye check or planet 1's agreed pattern said so (print d, d and
  o by hand, the e's loop, capitals S and E); the rest as drawn.
- **Closed without the human's ticks** (the lead's instruction): the content is playable and checked by the validator,
  the tests and a review; the ticks and her play on the tablet are a step in `local/for-human-next-steps.md`, and any
  change they bring is a content chore.

### Follow-ups

- **The human:** tick the review table (Progress → "For the human to check"): the 24 shapes against her manual, every
  word, split, picture and line, and the questions (untraced words, planet 1's other words, capitals in "English only",
  sat / mops, ant, the backwards o and e); record planets 2–3 after planet 1 (the recording scripts group by planet);
  play planets 1–3 on the Tab A7 in "both" mode after `parent-area` (the brief's last acceptance criterion).
- **A content chore** after the answers: the shapes marked `checked`, any word or picture swapped, planet 1's other
  words, more TraceWord steps.
- **`rewards`:** stickers and constellations for planets 1–3 (the constellation lines exist: unicorn, rocket, whale).
- **`progress-review`:** reviews that trace the untraced TraceWord words.
- **A question for the human first:** capitals in "English only" (a lesson that plays only in "English only" needs code).
- **`look`:** pictures for the 71 new ids (`content/pictures.csv`; keep `ten`, `dot` and `spot` apart, nani's moon and
  pas's footprint readable as such); the word under the FirstSound picture; "Ascultă" on the English listen button.
- **Tracing:** an o drawn backwards from its high exit and an e in a word drawn backwards are taken for "no entry"
  (the start radius reaches the round's start); fine unless the human finds it confusing.
