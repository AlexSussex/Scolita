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
