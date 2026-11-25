# Task: feature/parent-area

> **Status: In progress (parent-area).**

Read `AGENTS.md` first, then `design/README.md` and `design/screens.md` (build the screens as designed there) (especially "Security and privacy" → the parent
gate, "Fresh-eyes review"), and the completion records in `tasks/done/`.

**Feature:** `feature/parent-area`
**Implements:** PRD §3.7; `docs/curriculum.md` §1 (little and often).
**Depends on:** `progress-review`.

## Scope

1. **`ParentGate`:** press and hold the parent button for 3 seconds (a
   ring fills), then the app says a number from 11 to 19 and shows three
   numbers written as words in the app's language; the right one opens
   the area, a wrong one closes the gate quietly. Numbers in the rules
   asset. A fresh-eyes review checks a child can't open it by tapping
   around.
2. **What she has learned:** a grid of the track's letters by language
   (new: grey; learning: half-filled; learned: full, with its star or
   circle badge), the days played in the last two weeks, the next lessons.
3. **Settings:** the language mix (D1), the app's own language, letter
   case first (D2), the session limit (D8: off, 10, 15, 20 minutes), music,
   sounds and voice volume, vibration, the version.
4. **The session limit:** when reached, the current lesson finishes, then
   the mascot yawns ("Racheta se odihnește" / "The rocket needs a rest")
   and the map sleeps until the next calendar day or until a parent opens
   it again through the gate.
5. **Voice status:** recorded lines per language, out of the total.
6. **Reset progress:** asks twice, saying what will be lost; keeps the
   settings.
7. **Fill in "The basics"** in `docs/manual-testing.md`: a whole lesson
   from the map and back, the gate, a setting changed.
8. Android Back does what the on-screen Back does on every screen.

## Out of scope — do not build

- Managing profiles (`profiles`, next: it adds the profile list, "Who's
  playing?" and the tester profile to this area), exporting progress,
  notifications. Build this area so the settings marked "per profile" in
  PRD §3.8 (the language mix, letter case first, the session limit) are
  read from the current profile's folder, and the rest from the tablet's
  settings.

## Acceptance criteria

- The gate opens only with the hold and the right number; random taps for
  a minute never open it (a PlayMode monkey test).
- Changing the mix to English only hides the Romanian lessons at once.
- The session limit never ends a lesson in the middle.
- Every screen meets "Usability": 64 dp targets, the parent area's text at
  least 18 sp (it is for adults), a way back everywhere.

## Tests required

- EditMode: the gate's rules, the session limit, the grid's states.
- PlayMode: the gate (right, wrong, the monkey test), each setting takes
  effect, reset.
- Emulator: every screen at every size in the table, in both app
  languages.

## Touches

`Scripts/Core/Parent`, `Scripts/UI/Parent`, the map's parent button, new
prefabs, the `settings` save file (new fields, migrated), the string
tables.

## Screens

The gate, the progress grid, the settings, reset.

## Voice lines

The gate's numbers, the session's end, both languages.

## Before closing out

- Release (the next MINOR). The first APK for her comes with `profiles`. Add to
  `local/for-human-next-steps.md`: install it, run "The basics" (on a
  test profile once `profiles` exists).
