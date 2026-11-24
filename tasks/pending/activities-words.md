# Task: feature/activities-words

> **Status: In progress (words).**

Read `AGENTS.md` first, then `design/README.md` and `design/screens.md` (build the screens as designed there) (especially "The five seams" → 3, "Usability",
"Learning"), and the completion records in `tasks/done/`.

**Feature:** `feature/activities-words`
**Implements:** PRD §3.2; `docs/curriculum.md` §4.2 (`Syllables`,
`BuildWord`, `ReadAndMatch`, `WordTwins`), §6.
**Depends on:** `activities-letters`.

## Scope

For each kind: its Core rules, its view prefab, its registry row, its
validator checks, its voice lines.

1. **`Syllables`:** a word heard and pictured; a big drum; she taps it once
   per syllable (Romanian, from the word's syllables) or per beat
   (English); each tap lights a dot under the word's syllable; done on the
   right count. Hint: the word said slowly with its beats, drum sounds on
   each. The drum's sound is the first seed of the music domain.
2. **`BuildWord`:** a word heard; the rocket has one window per letter;
   letter tiles below (the word's letters plus 1–2 others taught); she
   drags them in order; a wrong tile slides back; done: the rocket takes
   off, the word said slowly, then blended. Hint: the next window's letter
   glows.
3. **`ReadAndMatch`:** a word on a card (no sound at first); 2–3 pictures;
   she taps the right one; after the first try she may tap the word to
   hear it. Hint: the word's letters light one by one with their sounds.
4. **`WordTwins`:** Romanian words on the left (star badges), English on
   the right (circle badges); she draws a line or drags to join the twins,
   hearing each word on touch; a false friend pair shows both pictures and
   a giggle line. Hint: the pair's pictures appear.

## Out of scope — do not build

- Sentences, stories, upper-case-only words.

## Acceptance criteria

- Each kind runs in a lesson on the emulator with planet 1–3 words.
- `BuildWord` refuses nothing silently: a wrong tile slides back with a
  soft sound and the hint ladder applies.
- `ReadAndMatch` never says the word before her first try.
- `Syllables` counts "ma-ma" as 2, "pe-pe-ne" as 3 from the content.

## Tests required

- EditMode: each kind's rules with real words; the validator checks (a
  `WordTwins` step whose words aren't twins or false friends fails; a
  `BuildWord` word longer than the rocket's windows fails).
- PlayMode: each kind to its end with simulated input.
- Emulator: each at 2000 × 1200 and 1280 × 800, both languages heard.

## Touches

`Scripts/Core/Activities/{Syllables,BuildWord,ReadAndMatch,WordTwins}`,
`Scripts/Game/Activities/...`, four new prefabs, `ActivityRegistry` (four
rows), `voice-lines.csv` (new rows).

## Screens

The four activities.

## Voice lines

Instructions and hints for the four kinds, both languages; slow and
syllable lines for planet 1–3 words.

## Before closing out

- Release (the next MINOR).

## Progress

_Kept by `words` while the brief is in flight._

### Status

- **Claimed** in `82cca71` (worktree `C:\Projects\scolita-words`, emulator `emulator-5602` on the
  AVD `Scolita_Tablet_lesson`, reused).
- **Design note** below (a medium change: four activity kinds). No content-model change; the decisions
  it takes inside the brief, and the lines to check, are listed under "For the human".
- **Core slice pushed:** `d8f39d7` `content:` the tick, twins hint and shown lines (and the English
  "clap" lines reworded to the drum); `f862054` `core:` the four kinds' rules, the round's `Places` and
  `PlacesFilled`, the `Place` and `Join` gestures, the `Word` and `Control` looks, the `SpellOut`,
  `RevealPictures` and `ShowPlaces` cues, `ActivityLimits`, the validator's word rules, with their
  EditMode tests; `fa272c0` `content:` and `11df670` `core:` the fresh-eyes review's fixes (below).
  Batch mode at `11df670`: EditMode 413/413, PlayMode 10/10, `BuildAndroidDebug` exit 0, no `warning CS`
  or `error CS`. Nothing changes on screen yet: the four kinds have no views, so their steps are still
  skipped (the registry's "not built yet" list).
- **Next:** the views slice (the four views, their prefabs, the registry's rows, PlayMode tests).

### Fresh-eyes review of the Core slice (2026-11-24)

A second agent that didn't write it read `d8f39d7..f862054` against AGENTS.md, this note and the earlier
briefs' conventions. No blockers; it found the rules true to the note, every gesture refused where it
isn't taken, `Begin` resetting each kind, nothing allocated per answer (`ContentId` is `IEquatable`, the
lines made in the constructors), the validator rules and the tests sound, and the Romanian lines
grammatical. Its 11 findings, and what was done:

1. **A `WordTwins` word listed twice passed the validator** and would throw when the lesson is built:
   `PairsUp` now checks each word once (test).
2. **A word's twin and false friend both in one step** would pair up wrongly: refused (test).
3. **7 beats for a word of 6 syllables were capped to 6 and judged right:** one beat past the row is
   counted now (test with a made-up six-syllable word).
4. **Tapping a Romanian word, then an English one to hear it, was a miss:** a tap joins only the
   partner; a tap on another word only says it. Only a line drawn to a wrong word is a miss (for the
   human).
5. **A line within a column, or to a joined word, is no answer** (`Neutral`): the note says so now.
6. **`Syllables`' circles under `GlowAnswer` with no choice:** a cue of their own, `ShowPlaces`.
7. **`ActivityChoice.OfControl`:** the note's name fixed.
8. **„Unește-le!” had no plural before it:** „Unește cele două cuvinte!” / "Join the two words!"; „bifa
   verde” is for the human to confirm.
9. **A listen-only false friend** (shown as a picture) would make "spelled the same" untrue: refused.
10. **`ReadAndMatch`'s pictures must not show their words:** written in the note's Unity side.
11. **Progress pushed with the slice:** this section, in a `docs:` commit.

### Design note (medium change: four activity kinds)

#### In plain words

1. **Each kind is one rules class in Core and one view prefab,** found through `ActivityRegistry`, as
   `lesson-engine` laid out; the "hint, then show" ladder stays in `LessonRun`. Each kind only says what
   is right, what its hint is and what its shown answer is.
2. **`Syllables`:** the word's picture (its word written under it when she can read it), a big drum and
   a green tick. She taps the drum once per syllable (Romanian) or beat (English); each tap lights a dot
   in a row under the picture; then she taps the tick. The right count (from the word's `syllables`
   cell: ma-ma 2, pe-pe-ne 3) finishes the step; another count is a miss: the dots clear, and she hears
   the word in syllables ("Ascultă cum spun cuvântul pe silabe. ma-ma."); the answer shown puts one
   glowing empty circle per syllable under the picture (as the Romanian manuals draw them) for her to
   fill.
3. **`BuildWord`** (design screen 5): the picture card (tap: the word again), the rocket lying on its
   side with one window per letter (the next one's rim glows), and the letter tiles under it: the word's
   letters and the step's 1–2 other letters (taught ones), shuffled. She drags a tile to the rocket: the
   right one flies into the next window and its sound is said; a wrong one slides back with a wobble
   (a miss). The first miss says the word slowly ("Ascultă cuvântul încet: m-a-m-a."); the answer
   shown makes the next letter's tile glow ("Litera care urmează strălucește."). Done: the rocket takes
   off, "Racheta pornește!", the word said slowly, then blended.
4. **`ReadAndMatch`:** the word on a cream card, **never said before her first try**, and the step's 2–3
   pictures. The first miss fades that picture and lights the word's letters one by one while their
   sounds are said ("Citește cuvântul sunet cu sunet. m… a… m… a…"); from then on, a tap on the word
   says it. The answer shown glows the right picture and blends the word ("Unește sunetele: m-a-m-a…
   mama!").
5. **`WordTwins`:** the Romanian words on the left (star badges), the English ones on the right (ringed
   planets); a word she can read is shown written, a listen-only word by its picture. Touching a word
   says it; she joins two by dragging a line from one to the other, or by tapping one then its partner
   (a tap on another word only says it: she may be listening, so it is never a miss).
   A twin pair stays joined ("Sunt gemeni!"); a false friend pair stays joined too, both its pictures
   appear and the mascot giggles ("Se scriu la fel, dar înseamnă altceva!"). A line to a wrong word is a
   miss: the hint shows the pictures of the word she started from and of its partner; the answer shown glows
   the partner.
6. **The human's rules hold:** a `BuildWord`'s extra tiles are taught letters, never in the word; every
   word shown to read uses only taught letters (listen-only words appear only as pictures); a
   `WordTwins` step's words pair up (each with its twin or false friend in the step).

#### Core (`Scripts/Core`, no engine reference)

- **The round** (`ActivityRound`) grows two small things, set by the rules, read by the views:
  `Places` (how many places the round has: `BuildWord`'s windows, `Syllables`' syllables) and
  `PlacesFilled` (how many are filled: windows filled, drum beats tapped). `PlacedOn` (existing) says
  which window a tile went to, and which word a joined word was joined to. `ShuffleRange(first, count)`
  shuffles one column of `WordTwins` at a time.
- **The answer** (`ActivityAnswer`) grows two gestures, refused by every kind that doesn't take them:
  `Place(choice)` (`BuildWord`: the tile she dragged, let go on the rocket; the place is always the next
  window) and `Join(choice, other)` (`WordTwins`: a line from one word to another; `Other` is the
  second). `TwoSounds` now refuses them too (it reads any gesture but `Drop` as a tap today).
- **The choices** (`ActivityChoiceLook`) gain `Word` (a word written to read, its picture shown only
  when the round reveals it: `ReadAndMatch`'s card, a readable `WordTwins` word) and `Control` (a part of
  the activity drawn by the view by its index: `Syllables`' drum and tick). `ActivityChoice.OfWordToRead`
  and `ActivityChoice.OfControl` make them.
- **The cues** (`HintCue`) gain `SpellOut` (the subject word's letters light one by one; the choice
  missed fades: `ReadAndMatch`) and `RevealPictures` (the pictures of `Round.Current` and
  `Round.Answer` appear: `WordTwins`) and `ShowPlaces` (one empty place per `Round.Places` still to fill:
  `Syllables`' circles). `GlowAnswer` serves the rest (the next tile, the right picture, the partner).
- **`SyllablesRules`** (`ro-words-1`: mama, apa; `en-words-1`: planet): choices `Drum` (0) and `Tick`
  (1), the word as the subject; `Places` = the word's syllables. A drum tap is `Neutral` and adds a
  beat (the row shows at most `SyllablesRules.MostBeats`, 6; one more is counted, so 7 beats for a word
  of 6 syllables is still a miss; more are ignored); the tick is `Done` when the beats
  equal the syllables, else `Wrong` and the beats go back to 0. A tick with no beat is `Neutral` (she
  hasn't tried). Instruction: `syll` + the word + `syll-tick` (new); prompt: the word. Hints:
  `syll-hint-listen` + the syllables line, then `syll-hint-count` + the syllables line; shown: the
  syllables line + `syll-hint-once`, cue `ShowPlaces` (the view draws `Places` empty circles).
- **`BuildWordRules`** (`ro-words-1`: tata with m, pat with m; `en-words-1`: map with t, mat with p):
  the tiles are the choices (the word's letters, one tile each, then the step's other letters), shuffled;
  the word's picture is the subject (`OfWord`); `Places` = the word's letters (at most
  `BuildWordRules.MostWindows`, 6: the rocket's windows). `Place(tile)`: a tile with the next window's
  letter (any of them, when a letter comes twice) is `Right` (the last `Done`), marked `Done` and placed
  on the window; another is `Wrong`. `Round.Answer` is an open tile with the next letter. `Tap(tile)`
  says its sound (`Neutral`). Instruction: `build` + the word; prompt: the word; response to a right
  tile: its sound; to the last: `build-liftoff` + the slow line + the word. Hints: `build-hint-slow` with
  the slow line, then `build-hint-after` with the last letter placed (or `build-hint-say` at the first
  window); shown: `build-hint-glow`, cue `GlowAnswer` on `Round.Answer`.
- **`ReadAndMatchRules`** (`ro-words-1`: mama among tata and pat; `party-1`, both languages): the step's
  first word is the one to read (the subject, `OfWordToRead`); the pictures of all its words are the
  choices, shuffled. The prompt is `read` until her first miss, then the word: so the word is never said
  before her first try, even when she taps the card or "listen again". Hints: `read-hint-sounds` + the
  slow line (cue `SpellOut`), then `read-hint-tap` (cue `FadeChoice`); shown: `read-hint-blend` with the
  slow line and the word, cue `GlowAnswer`.
- **`WordTwinsRules`** (`party-1`: pat / pat, planetă / planet): the Romanian words first, then the
  English ones, each column shuffled on its own; a readable word is `OfWordToRead`, a listen-only one
  `OfWord` (its picture). `Tap(word)` says it and selects it (`Round.Current`, its partner
  `Round.Answer`); a tap on its partner while one is selected joins them (a tap on any other word only
  says it and selects it); `Join(word, other)` joins any two words of the two columns, and a line within
  one column or to a joined word is no answer (`Neutral`: the view takes the line back). A join of partners marks both `Done`, places each on the other, and is `Right`
  (the last `Done`), with `twins-done` for twins and `false-friends` for false friends (the view shows
  both pictures); a line to another word is `Wrong` and keeps the first word selected. A tap on a joined word
  says it again. Hints: `twins-hint-pictures` (twins) or `twins-hint-spelled` (new, false friends), cue
  `RevealPictures`; then `twins-hint-sound`; shown: `twins-hint-show` (new), cue `GlowAnswer` on the
  partner. It speaks its lesson's language (Romanian in `party-1`, D1), as `TwoSounds` does.
- **`ActivityRulesFactory`:** four lines. No new number for `LearningRules`: `MostBeats` and
  `MostWindows` are limits of the screens (the drum's row, the rocket), not numbers she meets.
- **Content (`ActivityKinds`, `StepRules`):**
  - `Syllables`: item rule `CountsSyllables` (1 to `MostBeats` syllables); its hint lines.
  - `BuildWord`: `LettersAreChoices` (its extra tiles are taught letters, each once: the human's rule);
    word use `ReadAndPictured` (the design shows its picture); the slow line required; item rule
    `SpellsTheWord` (at most `MostWindows` letters, and the extra letters aren't in the word).
  - `ReadAndMatch`: the slow line required; item rule `PicturesDiffer` (each word once, no two with the
    same picture, so only one picture is right).
  - `WordTwins`: item rule `PairsUp` (the content-model follow-up): every word's twin or false friend is
    in the step, never both (the pairs are one to one), each word once, a false friend one she can read
    (false friends are "spelled the same", so they are shown written), as many Romanian words as English,
    at most 4 pairs.
  - Each kind's hint lines in `ActivityKinds`, so the validator checks them in both languages;
    broken-CSV tests for each new rule.

#### Unity side

- **Views** in `Scripts/UI/Lesson/`, prefabs in `Prefabs/Lesson/`, made from `look`'s art (cards,
  tiles, the rocket, the planets' shading, the badges) and the existing `ChoiceTile` cards by a
  throwaway Editor script (not committed), as before:
  - `SyllablesView` + `Drum` (made of `look`'s pieces: a pink body, a cream head; it bounces on each
    tap), the beat dots (six, pooled), the tick (a 120 px green button with `look`'s tick).
  - `BuildWordView` + `RocketWindow` + `LetterTile` (dragged with uGUI's drag events, as
    `HeldPicture`): the rocket (`look`'s rocket turned on its side) with six windows, eight tiles; let
    go over the rocket: `Place`; elsewhere: it floats back. Done: the rocket flies up and off.
  - `ReadAndMatchView`: the word card (Andika, the letters lit one by one for `SpellOut` from texts
    made when the step begins) and three picture cards (`ChoiceTile`), **without their words written**
    (she would match the text to the text).
  - `WordTwinsView` + `TwinCard` + `JoinLine` (a stretched, rotated image): two columns of four cards,
    the line following her finger, a line per joined pair.
- **`ActivityRegistry.asset`:** four rows, pushed early (`tracing` adds one after); the four kinds off
  "not built yet" (only `TraceLetter` and `TraceWord` stay).
- **Sounds:** the drum's sound, the soft sound of a miss and the rocket's take-off come with
  `sound-feel` (its scope lists the effects, the drum among them, through one `SoundPlayer`); until
  then the drum bounces silently and every line is said as usual.
- **Allocations:** every tile, window, dot, card and line is made with the prefab and reused; drags only
  move a `RectTransform`; the spelled-out texts are made when the step begins. Measured with a temporary
  `ProfilerRecorder` probe in the Editor, as before.

#### Tests

- **EditMode:** `SyllablesRulesTests` (mama 2, apa 2, planet 2, a made-up pe-pe-ne 3; beats, the tick,
  too few, too many, the cap, the hints, the shown circles, through the run), `BuildWordRulesTests`
  (tata with m: a repeated letter, the right tile, a wrong tile, the tap, the hints, the take-off line),
  `ReadAndMatchRulesTests` (never the word before the first miss, then the word; the hints),
  `WordTwinsRulesTests` (columns, taps and joins, a twin, a false friend, a wrong join, the hints); the
  factory over every step of a built kind; `ContentRulesTests` for each new check (a `WordTwins` word
  whose twin isn't in the step fails, a `BuildWord` word longer than the windows fails, an extra tile in
  the word fails, two `ReadAndMatch` words with one picture fail); `ActivityRegistryTests`.
- **PlayMode:** each kind runs to its end in the real Lesson scene (`ro-words-1`'s `Syllables`,
  `BuildWord`, `ReadAndMatch`; `party-1`'s `WordTwins`) with simulated taps and drags, through a hint and
  a shown answer; `LessonScene.FinishLesson` learns the four kinds.
- **Emulator:** each kind at 2000 × 1200 and 1280 × 800, in both languages, with real drags.

#### Another domain

`Syllables` is "tap the drum once per beat": a rhythm (music) or "tap once per star" (numbers) is the
same rules with another subject and count. `BuildWord` is "put the parts in order" (a tune's notes, the
digits of a number). `ReadAndMatch` is "match the written thing to its picture" (a numeral to a number
of stars). `WordTwins` is "join pairs across two columns" (an instrument and its sound). `Places`,
`PlacesFilled`, `Place` and `Join` serve any of them.

#### For the human (decisions inside the brief, and lines to check)

- **`Syllables` asks for a tick when she's done** (the brief says "done on the right count"): without
  it, she couldn't tap too few or too many, so the step could never be missed and the hint would never
  come. The count is judged when she taps the green tick; the dots aren't shown before (they would give
  the count away). New line: „Când ai terminat, atinge bifa verde.” / "When you have finished, tap the
  green tick."
- **The drum in both languages** (the brief); the English lines said "clap": reworded to "Listen to the
  word and tap the drum once for each beat." and "Tap the drum once for each beat."
- **New lines:** „Caută cuvântul care se scrie la fel.” / "Look for the word that is spelled the same."
  (a false friend's hint: its pictures differ, so "they show the same thing" would be untrue);
  „Uite, acesta este cuvântul potrivit. Unește cele două cuvinte!” / "Look, this is the right word. Join
  the two words!" (`WordTwins`' answer shown, worded like `pick-hint-show`). Does she know „bifă” (the
  tick)?
- **`WordTwins`: a tap on a word that isn't the partner only says it** (never a miss), since she may be
  listening; only a line drawn to a wrong word is a miss.
- **No sound effects yet** (the drum, the soft sound of a miss, the rocket's take-off): `sound-feel`
  brings them; the drum bounces without a sound until then.
