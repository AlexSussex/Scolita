# Task: feature/activities-letters

> **Status: In progress (letters).**

Read `AGENTS.md` first, then `design/README.md` and `design/screens.md` (build the screens as designed there) (especially "The five seams" → 3, "Usability",
"Learning"), and the completion records in `tasks/done/`, above all
`lesson-engine`'s.

**Feature:** `feature/activities-letters`
**Implements:** PRD §3.2; `docs/curriculum.md` §4.2 (`LetterHunt`,
`FirstSound`, `TwoSounds`).
**Depends on:** `lesson-engine`.

## Scope

For each kind: its Core rules (`IActivityRules`), its view prefab, its row
in `ActivityRegistry`, its validator checks, its voice lines.

1. **`LetterHunt`:** 8–12 stars float slowly across the screen (never too
   fast to tap: a speed from the learning rules), some with the letter
   (both cases), the rest with 2 other taught letters; she pops the right
   ones; a wrong pop bounces back gently; done when all right ones are
   popped. Hint: the right stars twinkle.
2. **`FirstSound`:** a picture and its word heard; 2–4 letter choices;
   hint: the word again with its first sound stretched (a voice line per
   word, "mmm-ama").
3. **`TwoSounds`:** one letter on two little planets with the star and the
   circle badges; she taps each to hear its sound (both must be heard
   before sorting starts); then 4–6 pictures come one by one and she drags
   each to the planet whose sound it starts with. Hint: both sounds again,
   one after the other.

## Out of scope — do not build

- Word activities (`activities-words`), new content beyond test items.

## Acceptance criteria

- Each kind runs in a lesson on the emulator with planet 1–2's items and
  follows the kind-feedback rules (hint, then show).
- `TwoSounds` doesn't start sorting until both sounds have been heard.
- `LetterHunt`'s stars never overlap the home button or the mascot, and
  never move faster than the rule allows on any screen size.

## Tests required

- EditMode: each kind's rules with real items (right, wrong, hint, show,
  done); the validator checks (a `TwoSounds` step on a letter whose two
  sounds are the same fails).
- PlayMode: each kind runs to its end with simulated taps and drags.
- Emulator: each at 2000 × 1200 and 1280 × 800, both languages heard.

## Touches

`Scripts/Core/Activities/{LetterHunt,FirstSound,TwoSounds}`,
`Scripts/Game/Activities/...`, three new prefabs, `ActivityRegistry`
(three rows), `voice-lines.csv` (new rows).

## Screens

The three activities.

## Voice lines

Instructions and hints for the three kinds, both languages; the stretched
first sounds for planet 1–2 words.

## Before closing out

- Release (the next MINOR).

## Progress

_Kept by `letters` while the brief is in flight._

### Status

- **Claimed** in `9c82bb7` (worktree `C:\Projects\scolita-letters`, emulator `emulator-5602`
  on the AVD `Scolita_Tablet_lesson`, reused).
- **Design note** below (a medium change: three activity kinds). Nothing in it needs the
  human before code; the lines to check are listed under "For the human".

### Core slice (pushed, see the hashes below)

- `content:` `ro-a` and `en-a` lose `LetterHunt` and `FirstSound`; `ro-m` and `en-m` hunt and
  pick m among a only; `two-hint-show` added (**the human's rule, 2026-11-24: wrong choices are
  only taught letters**, as `lesson-engine` did for `ListenAndPick`). Two EditMode tests that read
  planet 1's steps travel with it (`CurriculumAssetsTests`, `LessonVoiceLinesTests`), so the commit
  passes on its own.
- `core:` the three kinds' rules, the round's `Subject` / `StateOf` / `Current`,
  `ActivityAnswer.Drop`, `HintCue.TwinkleAnswers`, `LetterHuntTuning` in `LearningRules`, the
  `stretched` voice line kind, the validator's rules, with their EditMode tests.
  `LearningRulesAsset.cs` (Game) travels with it: the constructor it calls changed, and each commit
  must compile.
- `build:` the recording kit plans stretched words.
- `content:` the six stretched lines of planet 1's `FirstSound` words.
- `core:` a `FirstSound` word needs its stretched line (after the lines exist, so every commit passes).
- `content:` `content/README.md` describes the stretched lines and the steps' letters.
- The fresh-eyes review's fixes (below): `core:`, `content:` (the planet hint reworded), `game:` (the
  hunt numbers written into `LearningRules.asset`).

### Fresh-eyes review of the Core slice (2026-11-24)

A second agent that didn't write it read `a21ca62..abacfdf` against AGENTS.md, this note and
`lesson-engine`'s conventions. No blockers; it found the rules true to the note, the ladder still
owned by `LessonRun`, nothing allocated per answer (the `Dictionary<ContentId, …>` lookups don't
box), Core free of Unity, the commit order sound and the new kind safely stored as its code. Its
12 findings, and what was done:

1. **`ListenAndPick` and `MeetLetter` ignored the gesture:** both now refuse a `Drop` (tests);
   every kind refuses the gestures it doesn't take, so `tracing`'s stroke is never misread.
2. **The hunt numbers weren't in `LearningRules.asset`:** written in a `game:` commit, checked by
   `LearningRulesAssetTests`.
3. **The tablet's voice would read "t-t-tata" as letter names:** a stretched line without a clip is
   said as the word's first sound, a pause, then the word (`VoicePlanner`, as slow words are said
   from their sounds; `TestSpeak_StretchedWordFromItsFirstSound`). The bubble still shows
   "t-t-tata". The first letter is found by `WordStarts` (moved out of `FirstSoundRules`, shared).
4. **`LearningRules`' constructor grows with `tracing`'s:** pushed early; `tracing` adds its tuning
   object after `letterHunt`, and tests build rules with `TestLearningRules` (relayed to the lead).
5. **A missing stretched line fell back silently:** the factory now throws (the validator makes it
   impossible).
6. **8 to 12 stars:** `LetterHuntTuning` refuses fewer than 8 (`FewestStars`), the asset's range too.
7. **`Round.Answer` went stale in `LetterHunt`:** it now points at a star still to find.
8. **Two commits each touch two areas:** kept, so every commit compiles and passes (recorded above).
9. **The note's names:** updated to `LearningRules.LetterHunt.{Stars, RightStars, FastestSpeed}`.
10. **`TwoSounds` validator gaps:** the same word twice is refused now; whether the letter is taught
    in both tracks by then is a follow-up (`en-a` follows `ro-a`, so it holds today).
11. **Test names:** `TestAnswer_HintThenShowThroughTheRun`.
12. **Romanian:** reworded to "Uite, aceasta este planeta potrivită. Du poza acolo!", like
    `pick-hint-show` (English: "Look, this is the right planet. Take the picture there!").

### The views slice

- `game:` `ActivityViewContext` carries the `LearningRules` (the stars' speed); `LessonRunner` builds
  one rules object for the run and the views.
- `ui:` `LetterHuntView`, `HuntStar`, `StarGraphic` (a five-pointed star drawn as a mesh, lit from the
  top left: crisp at any size, no new art), `FirstSoundView`, `TwoSoundsView`, `SoundPlanet`,
  `HeldPicture`; `ChoiceTile.ShowSubject` (a subject card asks for the prompt again) and
  `ChoiceTileFeedback` (the cards' feedback, shared by `ListenAndPick` and `FirstSound`).
- `game:` `Prefabs/Lesson/LetterHuntView.prefab`, `FirstSoundView.prefab`, `TwoSoundsView.prefab` (made
  by a throwaway Editor script from `look`'s art and the existing cards, not committed), three rows in
  `ActivityRegistry.asset` and the three kinds off its "not built yet" list; the PlayMode tests
  `LetterActivitiesTests` and the shared `LessonScene` helper (`LessonFlowTests` uses it; its `ro-t`
  run now plays on through `LetterHunt` and `FirstSound`).

### Decisions while building

- **The answer shown in `LetterHunt` glows every star still to find,** not one: one could be half
  out of the field at its edge, and each is the answer. The stars keep drifting (the first try
  stopped them, and a half-hidden glowing star could then never come into view). The glow ends at
  the next star found.
- **The picture in her hand has no language badge** (design screen 4 shows none): the badge would
  tell her which planet it goes to, and she is to listen. It gets its badge once sorted, in the
  planet's tray. Every other card and both planets carry theirs.
- **The English planet** is `look`'s purple planet tinted blue, with its ring, until `look` draws a
  blue one (follow-up). The ticks are the map's "done" stop.
- **Stars** are 150 reference pixels (about 150 dp on the Tab A7), in two lanes at 1280 × 800
  (three when the field is taller), 10 of them; the field is the activity area less 250 reference
  pixels at the top (the home button, the dots, the mascot and its bubble) and 130 at the bottom
  ("Ascultă"), clipped at its edges.
- **Layout** checked against the mascot's bubble: `FirstSound`'s picture card and `TwoSounds`' planets
  and letter sit below its bottom edge.

### Usability (the new screens)

- **Spoken:** every instruction, hint, shown answer and praise is said and shown in the bubble; a
  found star says its sound; each picture's word is said when it comes; tapping the picture (or the
  letter card, or "Ascultă") says the question again, and tapping the mascot the last line.
- **Touch targets** (1280 × 800 reference): stars 150 × 150 (the whole square takes the tap), letter
  cards 190 × 190, the picture card 420 × 380, the planets 240 × 240 (a drop counts over the planet or
  its tray, 340 × 480), the picture in hand 190 × 190, "Ascultă" 320 × 96.
- **Kind feedback:** a missed star bounces, a missed letter wobbles and fades, a picture on the wrong
  planet floats back and the planet wobbles; the hint twinkles, the answer glows yellow; no red, no
  buzzer, no score.
- **The language in two ways:** every star, letter card, picture card and planet carries the star
  "RO" or the ringed planet "EN" (the picture in hand aside, above).
- **Nothing covers the HUD:** the stars stay in their field (checked by `LetterActivitiesTests` on
  the real screen: the field's top under the mascot's and the home button's bottoms).

### Test results (2026-11-24, at the views slice)

- **Batch mode:** EditMode **388/388**, PlayMode **10/10** (`LetterActivitiesTests`: the hunt with a
  miss, the twinkle, the second miss, every m glowing, the pops, the field under the top bar and the
  speed never above 60 reference pixels a second over 30 frames; `FirstSound` with the picture tapped,
  a fade, the glow and the end; `TwoSounds` with no picture before both ticks, two drops on the wrong
  planet, the glow and four pictures in their trays), `BuildAndroidDebug` exit 0; no `warning CS` or
  `error CS` in any log.
- **Emulator** (`emulator-5602`, AVD `Scolita_Tablet_lesson`, Android 16, no TTS voice, so every line
  showed in the bubble):
  - **2000 × 1200:** `ro-m` from its start: `MeetLetter`, `ListenAndPick` (m, a), then `LetterHunt`:
    ten stars in two lanes under the bubble, drifting both ways; a missed A bounced, the m stars
    twinkled and the mascot thought ("Stelele cu litera m strălucesc. Prinde-le!"); a second miss
    showed "Uite, aceasta este litera m. Atinge-o!" (found here: only one star glowed, at the field's
    edge, and everything stopped; fixed as above and checked again on `en-m`: all four m stars glowed
    and kept moving, "Look, this is the letter m. Tap it!"); popped stars said "m…". `FirstSound`
    (`ro-m`): the circle without its word, m and a with their stars, "Cu ce sunet începe cuvântul?
    mama. Atinge litera cu care începe cuvântul."; a faded with "Ascultă primul sunet: mmmama.", then m
    glowed; m ended the lesson ("Ai terminat lecția. Bravo!", "Mai departe"). `FirstSound` (`en-m`) in
    English with the ringed planets. `TwoSounds` (`en-a`): the gold planet with its star and the blue
    ringed one, "The same letter has two sounds! Tap each planet to hear its sound."; each tap ticked
    its planet; the second brought the picture ("a… Listen to the word and take the picture to the
    planet with the same sound. albină"); a real drag (`adb shell input swipe`) to the blue planet
    floated back, a second made the gold planet glow ("Look, this is the right planet. Take the
    picture there!"), a drag to it put the picture in its tray with its star.
  - **1280 × 800:** the three views: the same layout, the bubble clear of the cards and planets.
  - **Logcat:** no error (the development build's socket lines aside).
- **Allocations** (Editor Play mode, a temporary `ProfilerRecorder` "GC Allocated In Frame" probe in a
  PlayMode test, not committed; 150 frames each after 90 to settle): the map, `ListenAndPick`,
  `LetterHunt` (stars moving), `FirstSound`, `TwoSounds` and `TwoSounds` while dragging the picture
  every frame all read **291 B per frame on average (488 B at most)**: the same as the map, so the
  views add **0 B**; the 291 B are the test harness's own (as `lesson-engine` found). Not measured on
  a device.
- **Screen sizes:** 2000 × 1200 and 1280 × 800 (the brief's). Not checked: the large, 4:3, phone and
  cut-out sizes (the field and the cards are laid out from the activity area, as every lesson screen
  so far; a follow-up with `look`'s final screens).
- **Not run:** the Editor's Device Simulator (batch mode; `wm size` stood in), listening by ear (no
  voice on the emulator; the human hears the lines with this release's rows), the device Profiler.

### Fresh-eyes review of the views slice (2026-11-24)

A second agent that didn't write it read `48c542d..8c7b8f4` against AGENTS.md, this note and
`lesson-engine`'s conventions. No blockers; it found the seams held (the views only draw the round
and forward taps and drops), the drag's event order right, nothing allocated in `Update`, drags or
effects, the speed cap sound (0.79 × the fastest at most) and the commits each one area. Its 14
findings, and what was done:

1. **A public method called another public one** (`ChoiceTile.OnPointerClick` → `Bounce`,
   `SoundPlanet.Show` → `EmptyTray`): private `StartBounce` and `ClearTray` now.
2. **`LetterHunt`'s answer shown differs from the note** (all stars glow and keep moving): kept, as
   decided on the emulator (above, "Decisions while building"); `LetterHuntRules`' doc now says the
   view glows every star still to find.
3. **`TwoSoundsView` repeated Core's sorting rule to fill the trays** (and fell back to planet 0
   silently): Core's round now says where each sorted picture went (`ActivityRound.PlacedOn`, set by
   `TwoSoundsRules`, tested); the view only draws it.
4. **`LessonScene.AnswerRight` would misanswer `TraceLetter`** once `tracing` lands: it lists the kinds
   it knows and fails clearly on another ("add it to `LessonScene.AnswerRight`"); the sorting loop is
   bounded. **For `tracing`:** add `TraceLetter` there.
5. **The moving stars would redraw the whole lesson canvas each frame:** the star field is a nested
   canvas now (its own `Canvas` and `GraphicRaycaster`, still clipped by its `RectMask2D`, checked on
   the emulator). The frame time on the Tab A7 is for the human's tablet (the emulator is no
   measure).
6. **The field test checked only the stars' centres:** it checks their whole height now.
7. **The picture in her hand has no badge on purpose:** recorded for the human (below) and asserted
   by the test, so a later prefab can't add one back.
8. **Unused members** (`HeldPicture.IsDragged`, `IsHome`, `HuntStar.Text`): removed; `HeldPicture.Shown`
   is now checked by the test.
9. **The star size in two places:** the view reads it from its stars.
10. **`bobHeight` 0 would divide by zero:** `[Min(1f)]`.
11. **`?.` on Unity objects** in `LetterHuntView`: an index guard instead.
12. **A tap on the picture in her hand went through `Answer`** (and reset its float home): it asks
    for the prompt (`RepeatPrompt`) now, which is its word.
13. **The tests checked little of the taps that aren't answers:** the `TwoSounds` test now taps the
    letter card and the picture and checks neither is an answer.
14. **The throwaway Editor script** was deleted before the commits (never committed).

Batch mode after the fixes: EditMode 388/388, PlayMode 10/10, `BuildAndroidDebug` exit 0, no
`warning CS` or `error CS`; the emulator at 2000 × 1200: the hunt clipped at the field's edges as
before, `TwoSounds` sorted all four pictures by real drags into their trays and ended the lesson
("You finished the lesson. Well done!").

#### For the human (the views)

- **The picture in her hand has no RO/EN badge** in `TwoSounds` (design screen 4 shows none): the
  badge would tell her the planet, and she is to sort by sound. It gets its badge once sorted. Agreed?
- **The third hint of `TwoSounds`** ("The planet with the star is for Romanian, and the planet with
  the ring is for English.") helps only if she knows the word's language; it comes only with 3 or more
  tries (the rules give 2), so it isn't said today.
- **A bubble that mixes the languages** (`TwoSounds`: the English instruction, then the Romanian word
  "albină") shows the badge of its first line only; showing each part with its own badge is a
  follow-up for the lesson's voice (`LessonVoice`).

### Design note (medium change: three activity kinds)

#### In plain words

1. **Each kind is one rules class in Core and one view prefab,** found through
   `ActivityRegistry`, exactly as `lesson-engine` laid out; the "hint, then show" ladder stays in
   `LessonRun`, written once. Each kind only says what is right, what its hint is and what its
   shown answer is.
2. **`LetterHunt`:** stars drift slowly in lanes across the activity area, under the top bar (so
   never over the home button, the dots or the mascot); about four carry the letter (small and
   capital), the rest the step's other letters (also in both cases, so a capital never gives the
   answer away). A right star pops and its sound is said; a wrong one bounces back; the first miss
   makes the right stars twinkle; the second makes them glow (built: all of them) and says "Uite,
   aceasta este litera m. Atinge-o!".
3. **`FirstSound`:** the picture (no word written under it, so the first letter isn't shown), its
   word heard, and 2–4 letter cards. The first miss says the word again with its first sound
   stretched ("Ascultă primul sunet: mmmama."); the second glows the answer.
4. **`TwoSounds`:** the letter at the top, the gold Romanian planet (star badge) on the left and the
   blue English planet (ringed badge) on the right. She taps each to hear its sound (a tick shows
   each one heard); only then the first picture comes, its word said, and she drags it to the
   planet whose sound it starts with. A miss sends it back and says both sounds again, one after
   the other; the second glows the right planet.
5. **The human's rule "wrong choices are only taught letters"** now also holds for `LetterHunt`
   and `FirstSound`, so (as `lesson-engine` did for `ListenAndPick`) `ro-a` and `en-a` lose their
   `LetterHunt` and `FirstSound` steps (only a is taught there), and `ro-m` and `en-m` find m among
   a only (t is not taught yet there).
6. **`TwoSounds` only on a letter whose sounds differ** (`vowel-differs`, `sound-differs`), with
   pictures of both languages, each starting with the letter: the validator fails otherwise.

#### Core (`Scripts/Core`, no engine reference)

- **The round grows three small things** (`ActivityRound`, all set by the rules, read by the
  views):
  - `Subject`: what the question is about, shown but not an answer (`FirstSound`'s picture,
    `TwoSounds`' letter); a tap on it says the prompt again (`IActivityInput.RepeatPrompt`).
  - `StateOf(choice)`: `Open`, `Heard` or `Done` (`LetterHunt`'s popped stars, `TwoSounds`' heard
    planets and sorted pictures), so a view draws Core's state and never keeps its own.
  - `Current`: the choice in play (`TwoSounds`' picture in her hand; -1 for none), with `Answer`
    the planet it belongs to.
- **`ActivityAnswer.Drop(target)`** beside `Tap(choice)` (a `Gesture` on the answer): `TwoSounds`'
  picture let go on a planet. A tap on a planet is hearing it, never an answer.
- **`HintCue.TwinkleAnswers`**: the remaining right choices twinkle (`LetterHunt`'s hint).
- **`LetterHuntRules`** (`ro-m`: `m a`): the step's first letter is the one to find, its others the
  wrong ones (all taught). `LearningRules.LetterHunt.Stars` stars (10), `RightStars` (4) with the
  letter, half of them capitals; the others spread over the other letters, alternating the case;
  shuffled by `IRandom`. Each right star is a part (`Right`, the last `Done`), said with its sound;
  a popped star tapped again is `Neutral`. Instruction: `hunt` + `hunt-capitals`; prompt: `hunt`,
  then `hunt-hint-more` once one is found. Hints: `hunt-hint-twinkle` (`hunt-hint-capital` when
  only capitals are left), then `hunt-hint-not`, both twinkling; shown: one right star glows,
  `pick-hint-show`.
- **`FirstSoundRules`** (`ro-m`: mama among `m a`; `ro-p`: pat among `p a m t`): the letters are
  the choices (all taught, each once), shuffled; the answer is the letter the word starts with (the
  longest, so a later group like `ch` wins over `c`); the picture is the subject. Instruction:
  `first-sound` + the word + `first-sound-tap`; prompt: the word. Hints: `first-hint-stretch` with
  the word's stretched line, then `first-hint-which` + the sound; shown: `pick-hint-show`.
- **`TwoSoundsRules`** (`en-a`: avion, albină, apple, astronaut): choices 0 and 1 are the letter
  in Romanian and in English (the planets), then the pictures, shuffled among themselves (the order
  they come in). Until both planets are `Heard`, `Current` is -1 and a drop is ignored; the second
  tap says the sound, then `two` and the first word. A drop on the planet of the word's language is
  `Right` (the next picture comes, its word said; the last is `Done`), on the other `Wrong`.
  Instruction: `two-same-letter` + `two-tap`; prompt: `two-tap`, then the word in her hand. Hints:
  `two-hint-again` + both sounds, then `two-hint-start` + the word, then `two-hint-badges`; shown:
  the planet glows, `two-hint-show` (new).
  - Every kind refuses the gestures it doesn't take (a `Drop` on `ListenAndPick`, say), so a new
    gesture (`tracing`'s stroke) is never misread.
  - **The language it speaks:** a `TwoSounds` step is in both languages, but its instructions are
    in its lesson's language (`en-a`: English), or in Romanian (D1, the anchor) in a `both` lesson.
    `IActivityRules.Language` becomes "the language it speaks"; each choice keeps its own.
- **`ActivityRulesFactory`**: three lines. **`LearningRules`** gains `LetterHunt`, a
  `LetterHuntTuning` with `Stars`, `RightStars` and `FastestSpeed` (the fastest a star drifts, in
  the lesson screen's 1280 × 800 reference pixels per second), checked on creation (8 to 12 stars,
  at least one right and one wrong).
- **Content (`ActivityKinds`, `StepRules`):**
  - `LetterHunt` and `FirstSound` set `LettersAreChoices` (taught, each once); `LetterHunt` takes
    2–3 letters (it needs a wrong one).
  - `FirstSound` needs each word's **stretched line**: a new voice line kind `stretched`
    (`ro-stretched-mama`, "mmmama"), `Word.StretchedLine`, loaded per lesson by
    `LessonVoiceLines`, said by the tablet's voice until recorded, and planned by the recording
    kit beside the slow words (`RecordingSessionPlanner`, `RecordingScriptPage`,
    `VoiceClipStandard`). It is an additive code in an existing column, like `slow`: no change to
    the model's shape (the `{stretched}` slot and `VoiceSlot.Stretched` exist already). The
    fresh-eyes review covers it. Until a stretched line is recorded, the voice says the word's
    first sound, a pause, then the word (as a slow word is said from its sounds): the tablet's
    voice can't hold a sound and would read "t-t-tata" as letter names.
  - `TwoSounds` gets the item rule `SortsByTheLettersTwoSounds`: its letter's sounds differ, its
    words are in both languages and each starts with the letter. Broken-CSV tests for each.
  - Each kind's hint lines are listed in `ActivityKinds`, so the validator checks them in both
    languages.

#### Unity side

- **Views** in `Scripts/UI/Lesson/`, prefabs in `Prefabs/Lesson/`, made from `look`'s art (cards,
  tiles, icons, the planets, the badges) by a throwaway Editor script, as `lesson-engine` did:
  - `LetterHuntView` + `HuntStar`: 12 stars in the prefab (the most the rules allow), in three
    lanes inside the activity area, drifting left and right, wrapping round, at `HuntStarSpeed` in
    canvas units (so the same speed relative to the screen on every size); a popped star sparkles
    and goes, a missed one bounces, the hint twinkles, the answer shown glows (see "Decisions while building"); an "Ascultă"
    button says the prompt.
  - `FirstSoundView`: the picture card (tap: the word again), up to four `ChoiceTile` letter cards
    with their badges, "Ascultă".
  - `TwoSoundsView`: the letter card, the two planets (badge, speaker, a green tick once heard),
    two trays, and the picture in her hand (dragged with uGUI's drag events; let go over a planet:
    `Drop`; elsewhere: it floats back). Taps on the planets say their sounds.
- **`ActivityViewContext`** also carries the `LearningRules` (the stars' speed).
- **`ActivityRegistry.asset`**: three rows; the three kinds taken off "not built yet".
- **Allocations:** every star, card, planet and picture is made with the prefab and reused; the
  movement and the animations are arithmetic in `Update` on pre-sized arrays; a drag only moves a
  `RectTransform`. Measured with a temporary `ProfilerRecorder` probe in the Editor, as
  `lesson-engine` did.

#### Tests

- **EditMode:** `LetterHuntRulesTests` (`ro-m`), `FirstSoundRulesTests` (`ro-m`, `ro-p`),
  `TwoSoundsRulesTests` (`en-a`): the round, right, wrong, neutral, the parts, the hints, the shown
  answer, the lines, and through `LessonRun` (hint, then show); the factory over every step of a
  built kind; `ContentRulesTests` (a `TwoSounds` on m fails, one with words of one language fails,
  a word that doesn't start with its letter fails, an untaught `LetterHunt` or `FirstSound` letter
  fails, a missing stretched line fails); `LearningRulesTests`; `ActivityRegistryTests`.
- **PlayMode:** each kind runs to its end in the real Lesson scene (`ro-m`'s `LetterHunt` and
  `FirstSound`, `en-a`'s `TwoSounds`) with simulated taps and drags, through a hint and a shown
  answer; the stars stay inside the activity area and under the speed.
- **Emulator:** each kind at 2000 × 1200 and 1280 × 800, both languages.

#### Another domain

`LetterHunt` is "find every one of these among those", `TwoSounds` "sort into two by a property",
both on choices, not letters: "find every 3" (numbers) or "high or low" (music) are the same rules
with other choices once the content gives steps those items. `FirstSound` is letter-specific.
`Subject`, `StateOf`, `Current` and `Drop` serve any hunt or sort.

#### For the human (lines to check)

- **New:** "Uite, aceasta este planeta potrivită. Du poza acolo!" / "Look, this is the right
  planet. Take the picture there!" (`two-hint-show`, worded like `pick-hint-show`).
- **The stretched words:** a sound that can be held is held ("mmmama", "mmmoon"); one that can't
  (t, p) is bounced, as phonics teachers do ("t-t-tata", "p-p-pat", "t-t-tiger"). Recorded clips
  will replace the tablet's voice here; it may say these oddly until then.

## How to test locally

Commands are for Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb"`, `EMU=emulator-5602` (your own emulator),
`S=<your scratch folder>` and `export MSYS_NO_PATHCONV=1 ADB_LOCAL_TRANSPORT_MAX_PORT=5699`. Close
your Editor on this project before any batch-mode run. Start the app with
`"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`;
reset its saves with `"$ADB" -s $EMU shell pm clear com.softshack.scolita`. The lesson file is
`F=/storage/emulated/0/Android/data/com.softshack.scolita/files/profiles/child-1/lesson-run.json`.
(On this PC a Unity build or the tool's shell can stop the adb server: start it again detached with
PowerShell's `Start-Process adb start-server` if `adb devices` shows nothing.)

**To open a lesson at a step:** stop the app, push
`{"version":1,"lessonId":"ro-m","stepNumber":1,"stepsSignature":""}` to `$F`, start the app, tap
**Hai!**, then read the signature it saved (`"$ADB" -s $EMU shell cat $F`); push the same file with
the step number you want and that signature (`ro-m`: `LetterHunt` is step 3, `FirstSound` step 6;
`en-m`: 2 and 4; `en-a`: `TwoSounds` is step 2), start the app again and tap **Hai!**. (A wrong
signature starts the lesson from its first step, by design.) `TraceLetter` steps are skipped until
`tracing` builds them.

### Core slice

1. **EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: every test passes (388 at this slice), no `warning CS` or `error CS` in the log. Among
   them:
   - `LetterHuntRulesTests` (`ro-m`: ten stars, m m M M among a A a A a A, each m a part said with
     /m/, a popped star again is no answer, the twinkle then "another letter" hints, the capital hint
     once only capitals are left, the answer shown on a star still to find, through the run: Right,
     Wrong, Wrong, Practised, Right, Right);
   - `FirstSoundRulesTests` (`ro-m`: mama among m and a; `ro-p`: pat among p a m t, shuffled; the
     stretched hint "mmmama", then "Atinge litera care face acest sunet." + /m/; the longest letter a
     word starts with);
   - `TwoSoundsRulesTests` (`en-a`: the planets first, the pictures shuffled among themselves; a drop
     before both sounds is ignored; each picture to the planet of its language; the hints; English
     instructions in `en-a`, Romanian in a lesson of both languages);
   - `ContentRulesTests`: an untaught letter in a `LetterHunt` or a `FirstSound` fails, a hunt of one
     letter fails, a `TwoSounds` on m fails ("m is shared-close"), one with words of one language or a
     word not starting with its letter fails, a `FirstSound` word without its stretched line fails;
   - `VoiceRouterTests.TestSpeak_StretchedWordFromItsFirstSound`: a stretched line with no clip is
     said as the first sound, a pause, then the word.
2. **A `TwoSounds` on a letter whose sounds are the same fails the import:** add
   `en-m,TwoSounds,,m,ro-muzica en-moon ro-muzica en-moon,,` at the end of `content/steps.csv`, run
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`.
   Expected: exit 1, and the log says the step "needs a letter whose sound differs between Romanian
   and English (vowel-differs or sound-differs); m is shared-close". Undo with
   `git checkout content/steps.csv`.

### Content slice

1. In `content/steps.csv`: `ro-a` is `MeetLetter` and two `TraceLetter`; `en-a` is `MeetLetter`,
   `TwoSounds`, `TraceLetter`; `ro-m` and `en-m` hunt and pick m among a only (`m a`). In
   `content/voice-lines.csv`: the six `*-stretched-*` lines and `two-hint-show` in both languages. The
   import (Core slice, step 2, without the added row) exits 0, "imported, with 17 warnings", and
   `git status` shows nothing changed.

### Game and UI slices

1. **PlayMode tests:**
   `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: 10/10, among them `LetterActivitiesTests` (the three kinds run to their ends with taps
   and drags) and `LessonFlowTests` (`ro-t` now plays on through its hunt and first sound). They keep
   the Editor's own `child-1` files as they were.
2. **The debug build:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`,
   then `"$ADB" -s $EMU install -r game/Builds/Android/Scolita-<version>-debug.apk`; then
   `"$ADB" -s $EMU shell wm size 2000x1200; "$ADB" -s $EMU shell wm density 240`.
3. **`LetterHunt` (`ro-m`, step 3):** ten yellow stars with a, A, m, M and a gold "RO" star each, in
   two lanes, the top one drifting right, the bottom one left, slowly, wrapping at the edges; none
   ever goes over the home button, the dots, the mascot or its bubble, or under "Ascultă". Said: "Atinge
   toate stelele care au litera m. Caută și litera mare!"
   - Tap an a star: it bounces back, the four m stars twinkle and grow, the mascot thinks, "Stelele cu
     litera m strălucesc. Prinde-le!".
   - Tap another a or A: every m star still in the sky glows yellow (and keeps drifting), "Uite,
     aceasta este litera m. Atinge-o!".
   - Tap an m star: it spins, grows and fades, "m…"; the glow ends. Tap the others (small and
     capital): the last one brings a praise and the next step.
   - "Ascultă" says "Atinge toate stelele care au litera m." before the first star is found, then "Mai
     sunt stele cu litera m. Caută-le!".
4. **`FirstSound` (`ro-m`, step 6):** the picture card (a coloured circle, its star badge, a speaker,
   **no word written**) and the cards m and a. Said: "Cu ce sunet începe cuvântul? mama. Atinge
   litera cu care începe cuvântul."
   - Tap the picture: it bounces and the word "mama" is said again (not an answer).
   - Tap a: it wobbles and fades, "Ascultă primul sunet: mmmama." (without a recording the tablet says
     /m/, a pause, then "mama").
   - Tap a again: m glows yellow, "Uite, aceasta este litera m. Atinge-o!". Tap m: a sparkle, a praise,
     and the end of the lesson ("Ai terminat lecția. Bravo!", **Mai departe**).
   - The same on `en-m` (step 4) in English, with the blue ringed "EN" badges: "What sound does the
     word start with? moon. Tap the letter the word starts with."
5. **`TwoSounds` (`en-a`, step 2):** the letter a on a cream card between the gold planet (star "RO",
   on the left) and the blue ringed planet ("EN", on the right), each with a speaker and an empty tray
   under it; no picture yet. Said: "The same letter has two sounds! Tap each planet to hear its
   sound."
   - Try to find a picture to drag: there is none until both planets are heard.
   - Tap the gold planet: a green tick, the Romanian /a/. Tap the blue one: a tick, the English /æ/,
     then "Listen to the word and take the picture to the planet with the same sound." and the first
     word; the picture appears in the middle, tilted, with a yellow ring and no badge.
   - Drag it to the wrong planet
     (`"$ADB" -s $EMU shell input swipe 1000 900 <x> 540 600`, x = 400 for the gold planet, 1600 for
     the blue one): it floats back and wobbles, the planet wobbles, "Listen to the two sounds again." and
     both sounds. Again: the right planet glows, "Look, this is the right planet. Take the picture
     there!". Drag it there: it lands in that planet's tray with its badge, and the next word is said.
   - Sort the four (avion and albină to the gold planet, apple and astronaut to the blue one): a
     praise, then the next step.
   - Tap the picture in the middle: its word again. Tap a planet while sorting: its sound again.
6. **The other size:** `"$ADB" -s $EMU shell wm size 1280x800; "$ADB" -s $EMU shell wm density 160`, repeat
   3 to 5: the same layout, nothing under the bubble. Reset with `wm size reset; wm density reset`.
7. **The Console stays clean** (`"$ADB" -s $EMU logcat -d -s Unity`): the information line now names
   only the kinds still to build (`TraceLetter`, `TraceWord`, `Syllables`, `BuildWord`,
   `ReadAndMatch`, `WordTwins`, fewer once `tracing` lands); no error but the development build's
   socket lines and, without installed voices, the voice's "no installed voice" warning.

### Cleanup

`"$ADB" -s $EMU shell pm clear com.softshack.scolita`; `wm size reset; wm density reset`.
