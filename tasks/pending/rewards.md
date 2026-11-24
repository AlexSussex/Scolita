# Task: feature/rewards

> **Status: Ready after `galaxy-map`.**

Read `AGENTS.md` first, then `design/README.md` and `design/screens.md` (build the screens as designed there) (especially "Usability" → no pressure and no
tricks), and the completion records in `tasks/done/`.

**Feature:** `feature/rewards`
**Implements:** PRD §3.6.
**Depends on:** `galaxy-map`.

## Scope

1. **Core's `Rewards`:** three stars per finished lesson (hints never take
   one away); a sticker every second lesson and at every planet's end,
   from a sticker list in the content (animals, instruments, planets,
   space things), never a duplicate until all are given; a rocket trail
   colour per finished planet. Nothing can be lost or bought. Raises
   `RewardEarned`.
2. **The end of a lesson:** the stars fly into the planet's constellation
   in the sky; a sticker pops out of a gift box when earned.
3. **The sky book:** one constellation per planet (a unicorn, a rocket, a
   whale, a guitar …, from the content), lit star by star.
4. **The sticker book:** the stickers she has, and a big space scene
   (the Earth at first) where she drags them, resizes them with two
   fingers, and they stay where she put them (saved).
5. **Rocket trails:** she picks the rocket's trail from those earned, on
   the map.
6. **The `rewards` save file** per profile, versioned.

## Out of scope — do not build

- Coins, a shop, streaks, daily rewards, anything that can be lost.
- The drawing pad.

## Acceptance criteria

- Every finished lesson gives three stars, whatever the hints.
- A sticker placed on the scene is still there after closing the app.
- No sticker repeats until every sticker has been given.

## Tests required

- EditMode: the rewards rules (stars, sticker order without repeats,
  trails), the save file.
- PlayMode: finishing a lesson lights its stars; placing a sticker saves.
- Emulator: the sky book and the sticker scene at 2000 × 1200 and
  1280 × 800; two-finger resize.

## Touches

`Scripts/Core/Rewards`, `Scripts/UI/Rewards`, the map's bottom bar, new
prefabs, `content/stickers.csv` (new), the `rewards` save file (new).

## Screens

The lesson's end, the sky book, the sticker book and scene, the trail
picker.

## Voice lines

"Ai câștigat un abțibild!" / "You got a sticker!", the constellation
names, both languages.

## Before closing out

- Release (the next MINOR).
