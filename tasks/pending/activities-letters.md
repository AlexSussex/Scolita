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
   makes the right stars twinkle; the second glows one of them and says "Uite, aceasta este litera
   m. Atinge-o!".
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
    and goes, a missed one bounces, the hint twinkles, the shown one glows and stops; an "Ascultă"
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
