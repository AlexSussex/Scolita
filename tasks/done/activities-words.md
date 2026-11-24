# Task: feature/activities-words

> **Status: Done 2026-11-24, released as 0.6.0** (agent `words`). The human, 2026-11-24: wrong
> choices and extra tiles are only letters already taught; `WordTwins` words pair up.

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
- **Views slice pushed:** `416a587`, `6efa205` `ui:` the four views; `9d58180`, `719bae6` `game:` the prefabs, the registry's rows, the PlayMode tests, and the second review's fixes (below).
- **Closed out** as release 0.6.0 (see "Completion record").

### The views slice

- `ui:` `SyllablesView` + `SyllableDrum` (the drum takes the beat on the finger's touch, so quick beats keep
  their rhythm), `BuildWordView` + `RocketWindow` + `DraggedTile` (on the same object as a `ChoiceTile`: a tap is
  the tile's own, a drag places it), `ReadAndMatchView`, `WordTwinsView` + `TwinCard` + `JoinLine`, and
  `DiscGraphic` (a round shape drawn as a mesh, lit from the top left like `StarGraphic`: the beats, the drum's
  head, the windows, the rocket's nose and flame, crisp at any size with no new art). `ChoiceTile.Restyle` (a
  text with the lit letter, a picture revealed); `ChoiceTileFeedback` fades the missed choice on `SpellOut` too.
- `game:` `Prefabs/Lesson/SyllablesView.prefab`, `BuildWordView.prefab`, `ReadAndMatchView.prefab`,
  `WordTwinsView.prefab` (made by a throwaway Editor script from `look`'s cards, tiles, badges and the existing
  `ChoiceTile` cards, not committed), four rows in `ActivityRegistry.asset` and the four kinds off its "not built
  yet" list (only `TraceLetter` and `TraceWord` stay); `LessonScene.FinishLesson` answers the four kinds; the
  PlayMode tests `WordActivitiesTests`.

### Decisions while building

- **The rocket is built from `look`'s pieces** (a long white body, a pink round nose, purple fins, an orange
  flame, round windows), not the map's upright rocket: the design's rocket lies on its side with a long body,
  and the windows (120 reference pixels, smaller for a word of 5 or 6 letters) need room in a row. `look` may
  draw a proper one (a follow-up).
- **The drum** is a pink body, a cream head, gold rims and two sticks, from the same pieces.
- **`ReadAndMatch` lights the word's letters at a steady pace** (one every 0.8 s, after 1.6 s for "Citește
  cuvântul sunet cu sunet."), not in step with the voice's sounds: the view doesn't hear the voice's parts. A
  follow-up for the lesson's voice (letting a view follow `VoiceRouter.PartStarted`).
- **`WordTwins`' cards** show a readable word written (its picture beside it once revealed or joined) and a
  listen-only word as its picture; the word in play gets a yellow ring; each pair joined gets a green line.
- **A tile or a picture that is done leaves the screen** (`BuildWord`'s tile flies to its window); the extra
  tiles stay until the step ends.

### Usability (the new screens)

- **Spoken:** every instruction, hint, shown answer and praise is said and shown in the bubble; a tile tapped says
  its sound; a word card tapped says the word (in `ReadAndMatch` only after her first try); the picture card
  says its word again (`Syllables`, `BuildWord`); the mascot repeats the last line.
- **Touch targets** (1280 × 800 reference): the drum 320 × 360 (its head and body take the tap), the tick 150 ×
  150, letter tiles 130 × 130 (the design's size), windows up to 120, picture cards 260 × 240 to 400 × 340,
  twin cards 300 × 120, the word card 480 × 200.
- **Kind feedback:** a wrong count clears the dots and the picture card wobbles; a wrong tile floats back and
  wobbles; a wrong picture wobbles and fades; a wrong line disappears and the card wobbles; the hints show (the
  word in syllables, the letters lit, the pictures revealed), the answer shown glows (or, in `Syllables`, the
  empty circles pulse); no red, no buzzer, no score.
- **Nothing gives the answer away:** no word written on `BuildWord`'s picture card or on `ReadAndMatch`'s
  pictures; `Syllables`' circles only once the answer is shown.
- **The language in two ways:** every card, tile and picture carries its star "RO" or ringed planet "EN", and the
  two `WordTwins` columns carry both.
- **Nothing under the HUD:** the cards, the rocket and the drum stay below the mascot's bubble at 2000 × 1200 and
  1280 × 800 (checked on the emulator).

### Test results (2026-11-24, at the views slice)

- **Batch mode:** EditMode **413/413**, PlayMode **14/14** (`WordActivitiesTests`: the four kinds with taps, real
  drag events and lines, through a hint and the answer shown), `BuildAndroidDebug` exit 0; no `warning CS` or
  `error CS` in any log.
- **Emulator** (`emulator-5602`, AVD `Scolita_Tablet_lesson`, no TTS voice, so every line showed in the bubble):
  - **2000 × 1200**, `ro-words-1` from its start: `Syllables` (mama): one beat and the tick cleared the dot and
    said "Ascultă cum spun cuvântul pe silabe. ma-ma."; three beats and the tick showed two pulsing circles
    ("ma-ma. Bate toba o dată pentru fiecare silabă."); two beats filled them and the tick finished; apa the same.
    `BuildWord` (tata with m) with real drags (`adb shell input swipe`): m to the rocket floated back, then a
    made t glow ("Litera care urmează strălucește."); t, a, t, a filled the windows ("t…", "a…") and the rocket
    flew off ("Racheta pornește! t-a-t-a. tata"); pat the same. `ReadAndMatch` (mama): a wrong picture faded,
    "Citește cuvântul sunet cu sunet. m-a-m-a", and the letters lit pink one by one; the right picture, a praise.
    `party-1`'s `WordTwins`: the Romanian "pat" and the planet picture on the left, the English ones on the right;
    a line from "pat" to the English planet: the line followed the finger, then both "pat" pictures appeared; a
    second wrong line: the English "pat" glowed, "Uite, acesta este cuvântul potrivit. Unește cele două
    cuvinte!"; a tap on it joined them (a green line); a line between the planets: "Sunt gemeni!".
  - **1280 × 800**, `en-words-1` in English: `Syllables` (planet, its picture only, being listen-only; "Listen to
    the word and tap the drum once for each beat. planet. When you have finished, tap the green tick."),
    `BuildWord` (map with t, mat with p, real drags), `ReadAndMatch` (map); `party-1`'s `WordTwins` with a real
    line: the same layouts, nothing under the bubble.
  - **Logcat:** the information line names only `TraceLetter` and `TraceWord`; no error.
  - Screenshots in the scratch folder (`emu-syll-2000.png`, `emu-syll-shown.png`, `emu-build-2000.png`,
    `emu-build-shown.png`, `emu-build-two.png`, `emu-read-spell.png`, `emu-twins-shown.png`, `emu-twins-done.png`,
    `emu-syll-1280-en.png`, `emu-build-1280-en.png`, `emu-read-1280-en.png`, `emu-twins-1280.png`).
- **Allocations** (Editor Play mode, a temporary `ProfilerRecorder` "GC Allocated In Frame" probe in a PlayMode
  test, not committed; 150 frames each after 90 to settle): `Syllables`, `BuildWord`, `BuildWord` while dragging a
  tile every frame, `ReadAndMatch`, `ReadAndMatch` while its letters light, `WordTwins` and `WordTwins` while
  drawing a line every frame all read **288 B per frame on average (290 B at most)**: the test harness's own, as
  on the map in earlier briefs, so the views add **0 B**. Not measured on a device.
- **Screen sizes:** 2000 × 1200 and 1280 × 800 (the brief's). Not checked: the large, 4:3, phone and cut-out
  sizes (the views are laid out from the activity area as every lesson screen so far; a follow-up with `look`'s
  final screens).
- **Not run:** the Editor's Device Simulator (batch mode; `wm size` stood in), listening by ear (no voice on the
  emulator; the human hears the lines with this release's rows), the device Profiler.

### Fresh-eyes review of the views slice (2026-11-24)

A second agent that didn't write it read `f5d6ce8..9d58180` against AGENTS.md, this note and the earlier
briefs' conventions. No blockers; it found Core still owning every rule (the views draw `Places`,
`PlacesFilled`, `PlacedOn`, `StateOf`, `Current`, `Answer`), a reused view resetting fully, nothing giving the
answer away, nothing allocated per frame or per drag, the touch targets large enough, the prefabs wired and the
registry rows right, and `LessonScene.FinishLesson` answering the four kinds right. Its 10 findings, and what was
done:

1. **Every drop on the rocket was also a tap** (the tile is a `ChoiceTile` too; uGUI clicks before the drop ends):
   a `ChoiceTile` now ignores the click when it was dragged away from where she pressed it (a shaky tap still
   counts).
2. **Nothing that moves had a nested canvas:** the rocket, the tiles, the drum, the beats and the lines each have
   their own `Canvas` now (with a `GraphicRaycaster` where they take taps); checked on the emulator.
3. **A line drawn from a word is silent until it lands** (only a tap says a word): kept, a decision for the human
   (saying it on the line's start would join it at once when the word in play is its partner); a tap says it.
4. **`ReadAndMatch`'s letters light at a steady pace, not with the voice:** kept (a follow-up for the lesson's
   voice); any later answer now stops it and puts the plain word back.
5. **The `BuildWord` test didn't prove a drop off the rocket is no miss:** it counts the hints now (none after a tap
   and a drop off the rocket, one after the wrong tile on it).
6. **Test gaps:** `WordTwins` now checks a line let go on nothing, a line within a column, a line from a joined word
   and the pictures shown once joined. The `ReadAndMatch` test still waits for the letters to light (about 5
   seconds of real time): the rule is about time.
7. **Room at the limits:** the tiles' spacing shrinks to fit the screen (eight tiles on a 4:3 tablet); four pairs
   of twins sit in rows of 124 reference pixels, under the bubble. The phone size stays a follow-up.
8. **No feedback for the tick before any beat:** the drum bounces (with "Bate toba o dată pentru fiecare
   silabă."). A wrong line wobbles the card it starts from only: kept.
9. **The take-off's numbers were in code:** serialized fields now.
10. **A revealed picture could overlap a long written word:** the word's box narrows beside its picture.

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
- **A line drawn from a word is silent until it lands** (a tap says a word); saying it as the line starts would
  join it at once when the word in play is its partner. Agreed?

## How to test locally

Commands are for Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb"`, `EMU=emulator-5602` (your own emulator),
`S=<your scratch folder, written C:/...>` and `export MSYS_NO_PATHCONV=1 ADB_LOCAL_TRANSPORT_MAX_PORT=5699`.
Close your Editor on this project before any batch-mode run. Start the app with
`"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`;
reset its saves with `"$ADB" -s $EMU shell pm clear com.softshack.scolita`. The lesson file is
`F=/storage/emulated/0/Android/data/com.softshack.scolita/files/profiles/child-1/lesson-run.json`.
(On this PC a Unity build can stop the adb server: start it again detached with PowerShell's
`$env:ADB_LOCAL_TRANSPORT_MAX_PORT='5699'; Start-Process adb start-server` if `adb devices` shows nothing.)

**To open a lesson:** stop the app, push `{"version":1,"lessonId":"ro-words-1","stepNumber":1,"stepsSignature":""}`
to `$F`, start the app and tap **Hai!**. `ro-words-1` plays `Syllables` (mama, then apa), `BuildWord` (tata, then
pat), `ReadAndMatch` (twice); `en-words-1` the same in English (planet; map, mat; map). **To open a later step**
(`party-1`'s `WordTwins` is step 5): open the lesson once as above, read the signature it saved
(`"$ADB" -s $EMU shell cat $F`), then push the same file with the step number and that signature, start the app
and tap **Hai!** (a wrong signature starts the lesson from its first step, by design). `TraceWord` steps are
skipped until `handwriting` builds them. Drags: `"$ADB" -s $EMU shell input swipe <x1> <y1> <x2> <y2> 600`.

### Core slice

1. **EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: every test passes (413 at this slice), no `warning CS` or `error CS` in the log. Among them:
   - `SyllablesRulesTests`: mama, apa and planet have 2 places, a made-up pe-pe-ne 3; drum taps are beats (never
     an answer), the tick with no beat asks for the beats, the right count then the tick is done and says
     "ma-ma", a wrong count is a miss that starts again, 7 beats for 6 syllables is still a miss; the hints
     ("Ascultă cum spun cuvântul pe silabe." + ma-ma), the circles shown (`ShowPlaces`); through the run: Wrong,
     Wrong, Practised.
   - `BuildWordRulesTests` (tata with m): t a t a m tiles, four windows, either t fills the first, a wrong tile is
     a miss, a tap says the tile's sound, the last tile says "Racheta pornește!", t-a-t-a, tata; the slow-word hint,
     the next tile glowing; English map with t.
   - `ReadAndMatchRulesTests`: the word is never said before her first miss (the prompt is the instruction), then
     the prompt is the word; the spell-out hint, then "tap the word", then the blend; English map in `party-1`.
   - `WordTwinsRulesTests` (`party-1`): Romanian words first, each column shuffled; a tap selects, a tap on the
     partner joins, a tap on another word only says it (never a miss), a line to a wrong word is a miss; twins say
     "Sunt gemeni!", false friends "Se scriu la fel, dar înseamnă altceva!"; the hints (spelled for false friends,
     pictures for twins), the partner glowing.
   - `ContentRulesTests`: a `Syllables` word of 7 syllables, a `BuildWord` word of 8 letters, an extra tile in the
     word or not taught yet, a word without its slow line, two `ReadAndMatch` words with one picture, `WordTwins`
     words that aren't partners, uneven columns, a word listed twice, a word with both its twin and its false friend,
     and a listen-only false friend all fail.
2. **A `WordTwins` step whose words don't pair up fails the import:** add
   `party-1,WordTwins,,,ro-mama en-map,,` at the end of `content/steps.csv` and run
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`.
   Expected: exit 1, and the log says the step's "words pair up, and the twin or false friend of ro-mama isn't in the
   step". Undo with `git checkout content/steps.csv`.

### Content slice

1. In `content/voice-lines.csv`: `syll-tick`, `twins-hint-spelled` and `twins-hint-show` in both languages; the
   English `syll` and `syll-hint-once` say "tap the drum". The import (Core slice, step 2, without the added row)
   exits 0, "imported, with 17 warnings", and `git status` shows nothing changed.

### Game and UI slices

1. **PlayMode tests:**
   `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: 14/14, among them `WordActivitiesTests` (the four kinds run to their ends with taps, drags and lines,
   through a hint and the answer shown). They keep the Editor's own `child-1` files as they were.
2. **The debug build:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`,
   then `"$ADB" -s $EMU install -r <worktree, written C:/...>/game/Builds/Android/Scolita-<version>-debug.apk`; then
   `"$ADB" -s $EMU shell wm size 2000x1200; "$ADB" -s $EMU shell wm density 240`.
3. **`Syllables` (`ro-words-1`, step 1):** the picture card (a coloured circle, "mama" under it, a speaker, the gold
   "RO" star), a pink drum with a cream head and two sticks, and a green tick bottom right; no dots yet. Said:
   "Ascultă cuvântul și bate toba pentru fiecare silabă. mama. Când ai terminat, atinge bifa verde."
   - Tap the tick first: "Bate toba o dată pentru fiecare silabă." (no miss).
   - Tap the drum once (it bounces, a yellow dot lights under the card), then the tick: the dot goes, the mascot
     thinks, "Ascultă cum spun cuvântul pe silabe. ma-ma."
   - Tap the drum three times, then the tick: two glowing white circles under the card, "ma-ma. Bate toba o dată
     pentru fiecare silabă."
   - Tap the drum twice (the circles fill yellow), then the tick: a sparkle, a praise, then apa.
   - Tap the picture card: "mama" again.
4. **`BuildWord` (`ro-words-1`, step 3):** the picture card (no word written), the rocket lying on its side with
   four round windows (the first one's rim glowing yellow), and five tiles t a t a m (shuffled), each with its star.
   Said: "Ascultă cuvântul și pune literele în ferestrele rachetei. tata"
   - Tap a tile: it bounces and says its sound. Drag a tile and let go off the rocket: it floats back, no miss.
   - Drag m to the rocket: it floats back and wobbles, "Ascultă cuvântul încet: t-a-t-a." (said sound by sound).
     Drag an a: t glows, "Litera care urmează strălucește."
   - Drag t, a, t, a in turn: each fills the next window ("t…", "a…"), the next window's rim glows; the last makes
     the rocket fly up and off, "Racheta pornește! t-a-t-a. tata", then a praise.
5. **`ReadAndMatch` (`ro-words-1`, step 5):** the word "mama" on a cream card, and three picture cards **without
   words**. Said: "Citește cuvântul și atinge poza potrivită." **The word is never said before her first try:** tap
   the word card: the instruction again, not the word.
   - Tap a wrong picture: it wobbles and fades, "Citește cuvântul sunet cu sunet. m-a-m-a" (said sound by sound),
     and after about two seconds the word's letters turn pink one by one.
   - Tap the word card: now "mama" is said. Tap another wrong picture: the right one glows, "Unește sunetele:
     m-a-m-a… mama!". Tap it: a sparkle, a praise.
6. **`WordTwins` (`party-1`, step 5):** "pat" with its star on the left above the picture of a planet (listen-only:
   a picture, no word); "pat" with the ringed "EN" planet and the planet picture on the right (the columns are
   shuffled). Said: "Unește cuvintele care seamănă: unul în română, unul în engleză."
   - Tap the Romanian "pat": a yellow ring, "pat" said. Tap the English planet picture: "planet" said, the ring moves
     to it (no miss).
   - Drag a line from the Romanian "pat" to the English planet picture: a yellow line follows the finger; let go: it
     goes, the card wobbles, the pictures of both "pat" cards appear, "Caută cuvântul care se scrie la fel." Again:
     the English "pat" glows, "Uite, acesta este cuvântul potrivit. Unește cele două cuvinte!".
   - Tap the English "pat" (the Romanian one still ringed): a green line joins them, "Se scriu la fel, dar înseamnă
     altceva!". Drag a line between the two planet pictures: a second green line, "Sunt gemeni!", a praise, the end.
7. **The other size and English:** `"$ADB" -s $EMU shell wm size 1280x800; "$ADB" -s $EMU shell wm density 160`, open
   `en-words-1`: the same screens with the ringed "EN" badges and the English lines ("Listen to the word and tap the
   drum once for each beat. planet. When you have finished, tap the green tick."; the planet shown as its picture
   only, being listen-only), nothing under the bubble. Reset with `wm size reset; wm density reset`.
8. **The Console stays clean** (`"$ADB" -s $EMU logcat -d -s Unity`): the information line names only `TraceLetter`
   and `TraceWord` as not built yet; no error but the development build's socket lines and, without installed
   voices, the voice's "no installed voice" warning.

### Cleanup

`"$ADB" -s $EMU shell pm clear com.softshack.scolita`; `wm size reset; wm density reset`.

## Completion record

_Finished 2026-11-24 by `words`; released as 0.6.0 (tag `v0.6.0`)._

### What was built

- **Pushed:** `82cca71` the claim; `f871ead` the Design note;
  - `d8f39d7` `content:` the tick, twins hint and shown lines (the English "clap" lines reworded to the drum);
  - `f862054` `core:` the four kinds' rules, the round's places, the new gestures, looks and cues, the validator's word rules;
  - `fa272c0` `content:` and `11df670` `core:` the first review's fixes; `f5d6ce8` `docs:`;
  - `416a587` `ui:` the four views; `9d58180` `game:` the prefabs, the registry rows, the PlayMode tests;
  - `6efa205` `ui:` and `719bae6` `game:` the second review's fixes (nested canvases); `462ab22` `docs:`;
  - the close-out.
- **Core** (`Scripts/Core`): `Activities/SyllablesRules`, `BuildWordRules`, `ReadAndMatchRules`,
  `WordTwinsRules`; `ActivityRound.Places`, `PlacesFilled`, `ShuffleRange`; `ActivityAnswer.Place` and `Join`
  (`Other`); `ActivityChoiceLook.Word` and `Control` (`ActivityChoice.OfWordToRead`, `OfControl`); `HintCue.SpellOut`,
  `RevealPictures`, `ShowPlaces`; `TwoSoundsRules` refuses the new gestures; `Content/ActivityLimits`; the four
  kinds' rows in `ActivityKinds` and the item rules `CountsSyllables`, `SpellsTheWord`, `PicturesDiffer`, `PairsUp`
  in `StepRules`.
- **Unity side:** `Scripts/UI/Lesson/SyllablesView`, `SyllableDrum`, `BuildWordView`, `RocketWindow`,
  `DraggedTile`, `ReadAndMatchView`, `WordTwinsView`, `TwinCard`, `JoinLine`, `DiscGraphic`; `ChoiceTile.Restyle`;
  `Prefabs/Lesson/SyllablesView.prefab`, `BuildWordView.prefab`, `ReadAndMatchView.prefab`, `WordTwinsView.prefab`;
  four rows in `ActivityRegistry.asset` (only `TraceLetter` and `TraceWord` left "not built yet").
- **Content:** `syll-tick`, `twins-hint-spelled`, `twins-hint-show` in both languages; the English `syll` and
  `syll-hint-once` reworded to the drum.
- **Tests:** EditMode 413 (was 388): `SyllablesRulesTests`, `BuildWordRulesTests`, `ReadAndMatchRulesTests`,
  `WordTwinsRulesTests`, new `ContentRulesTests`; the fake content gained a twin pair. PlayMode 14 (was 10):
  `WordActivitiesTests`; `LessonScene.FinishLesson` answers the four kinds.

### Conventions for later briefs

- **A round that fills places** (windows, beats) says so with `Places` and `PlacesFilled`; `PlacedOn` holds the
  place (or the choice) a choice went to. `ShowPlaces` shows the empty places as the answer.
- **Gestures:** `Place(choice)` (a dragged choice to the place waiting for it) and `Join(choice, other)` (a line
  between two choices) join `Tap` and `Drop`; every kind refuses the ones it doesn't take.
- **A word written to read** is `ActivityChoiceLook.Word` (its picture only when revealed); a listen-only word is
  never written: it is its picture (`Picture`). A view's own controls (a drum, a tick) are `Control` choices.
- **Exploring is never a miss:** a tap that only listens (a twin that isn't the partner, a tile, the tick before
  any beat) is `Neutral`; only a real attempt (a count, a placed tile, a drawn line, a picture tapped) is judged.
- **Screen limits** the content is checked against live in `ActivityLimits` (6 syllables, 6 windows, 4 pairs);
  the views hold that many places.
- **Round shapes drawn in code** use `DiscGraphic` (needs a `CanvasRenderer` added before it when built from an
  Editor script); pulse them with `canvasRenderer.SetAlpha`, not `color`, so their mesh isn't rebuilt each frame.

### How it was tested

Batch mode at the pushed commits (EditMode 413/413, PlayMode 14/14, `BuildAndroidDebug`, no compiler warning),
the import refusing a `WordTwins` step whose words don't pair up, the emulator at 2000 × 1200 and 1280 × 800 in
both languages (the four kinds, hints, answers shown, real drags and lines), the allocations in the Editor (the
views add 0 B per frame), and two fresh-eyes reviews (Progress). Not run: the Device Simulator, the device
Profiler, listening by ear.

### Decisions and deviations

- **`Syllables` judges the count when she taps a green tick** (the brief: "done on the right count"), so a
  wrong count can be missed and hinted; the dots aren't shown before the answer.
- **The drum in both languages:** the English "clap" lines reworded to the drum.
- **No sound effects** (the drum, the miss, the take-off): `sound-feel`'s scope.
- **`ReadAndMatch` lights the letters at a steady pace**, not in step with the voice.
- **`WordTwins`: a tap on a word that isn't the partner only says it**; only a drawn line is judged wrong.
- **New lines** `syll-tick`, `twins-hint-spelled`, `twins-hint-show` (for the human).
- **The rocket and the drum** are built from `look`'s pieces and mesh discs, not new art.
- **A line drawn from a word is silent until it lands** (a tap says a word): for the human.

### Follow-ups

- **For the human:** the lines and choices in row 0.6.0-6.
- **`sound-feel`:** the drum's sound on each beat (and the hint's beats), the soft sound of a miss, the rocket's
  take-off.
- **`look`:** a drawn rocket for `BuildWord` (long, on its side, room for six windows) and a drum; the large, 4:3,
  phone and cut-out sizes of these screens.
- **The lesson's voice:** let a view follow the parts said (`VoicePlanner`'s sounds), so `ReadAndMatch`'s letters
  light exactly with their sounds; a bubble mixing the languages (`WordTwins` says English words in a Romanian
  step) shows one badge.
- **`content-letters-1`:** planets 2–3's word lessons: `Syllables` words up to 6 syllables, `BuildWord` words up
  to 6 letters with 1–2 taught extra tiles and their slow lines, `ReadAndMatch` pictures that differ, and the
  twins and false friends of curriculum §6 (pairs one to one, false friends readable).
- **`handwriting`:** `TraceWord`, the last word kind.
