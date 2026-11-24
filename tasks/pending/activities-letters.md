# Task: feature/activities-letters

> **Status: Ready after `lesson-engine`** (can run beside `tracing` and
> `galaxy-map`).

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
