# Task: feature/sound-feel

> **Status: In progress (sound-feel).** The last MVP brief.

Read `AGENTS.md` first (especially "Audio", "Performance"), and the
completion records in `tasks/done/`.

**Feature:** `feature/sound-feel`
**Implements:** PRD §3.1–§3.6 (sound and effects); `AGENTS.md` →
"Performance" (the budgets).
**Depends on:** `rewards`, `profiles`, `content-letters-1`, `look`.

## Scope

1. **Music:** a calm space theme on the map and a lighter loop in lessons
   (CC0 or made for the app, licences in `ThirdParty/`), streamed; the
   music ducks under the voice (`IVoice`'s events).
2. **Effects:** taps, pops, the boing of a miss, the sparkle of a right
   answer, the rocket's take-off, stickers, the drum; decompressed on load;
   all through one `SoundPlayer` with the sounds volume.
3. **Haptics:** a light tick on a right answer and a sticker (vibration
   switch respected).
4. **Effects polish:** stardust, the trail, the planet faces' small
   animations, within the "one effect at a time" rule.
5. **The performance pass** on the Tab A7 (with the human: a development
   APK and what to note, as SeliSnake's stress test did): 60 fps on the
   map and in every activity, a lesson ready within 2 seconds, the app
   ready within 5 seconds, the APK size; fixes for whatever misses a
   budget.

## Out of scope — do not build

- Music lessons (`domain-music`).

## Acceptance criteria

- The voice is never covered by music or effects (listening check of a
  whole lesson).
- With sound off in the parent area, nothing plays and every activity
  still works from its pictures.
- The budgets hold on the Tab A7 (the human's numbers recorded in
  Progress).

## Tests required

- EditMode: the ducking rule, `SoundPlayer`'s volume routing.
- PlayMode: a lesson with sound off runs to its end.
- Emulator and the Tab A7: the performance pass.

## Touches

`Scripts/Game/Audio`, `Assets/Scolita/Audio`, prefabs that play sounds
(listed when claimed), `ThirdParty/`.

## Screens

None new.

## Voice lines

None new.

## Before closing out

- Release (the next MINOR): **the MVP is complete.** Update "Where we are"
  with the after-MVP order agreed with the human.
