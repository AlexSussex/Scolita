# Task: feature/galaxy-map

> **Status: Ready after `lesson-engine`** (can run beside `tracing` and the
> activities briefs). Decision D5 (the mascot) is not needed: a
> placeholder mascot until `look`.

Read `AGENTS.md` first, then `design/README.md` and `design/screens.md` (build the screens as designed there) (especially "Usability" → one tap to continue,
"Performance"), `design/README.md`, and the completion records in
`tasks/done/`.

**Feature:** `feature/galaxy-map`
**Implements:** PRD §3.1.
**Depends on:** `lesson-engine`.

## Scope

1. **The Map scene:** the sky, the rainbow path, the planets of the
   domain's track (from the content: a planet per unit, with its letters
   on its surface), the stops per planet (lessons), the rocket on the next
   stop, a big "Hai! / Go!" (in the app's language) that opens it. The map
   scrolls sideways by drag when there are more planets than fit.
2. **Planet states:** sleepy (ahead: dimmed, "zzz"), awake (the current
   one), beaming (finished). Any visited planet's stops can be opened
   again; planets ahead can be tapped and say "not yet" kindly.
3. **The trip:** finishing a planet's last lesson flies the rocket to the
   next planet (3–5 seconds, past the moon and the Earth, skippable with a
   tap).
4. **The mascot** (placeholder) greets her on arrival ("Bună! Hai să
   învățăm litera m!" / "Hi! Let's learn m!") and speaks when tapped.
5. **Where she is:** which lessons are done is read from a small
   `map-progress` save (per profile); `progress-review` will replace the
   source behind the same interface.
6. **Room for the avatar button** (top left, beside "Școlița"), filled by
   `profiles`.
7. **The parent button:** a small button, top right, which does nothing
   yet (the gate comes with `parent-area`).
8. **Bottom bar placeholders** for the sky book and the sticker book
   (filled by `rewards`).

## Out of scope — do not build

- Stars, stickers, trails (`rewards`); mastery and review
  (`progress-review`); the parent area.

## Acceptance criteria

- The app opens on the map with the rocket on the next lesson; one tap on
  Go starts it; finishing it returns to the map with the rocket one stop
  further.
- Finishing planet 1 plays the trip to planet 2 (skippable).
- 60 fps on the map in the Profiler at 2000 × 1200; 0 B allocated per
  frame while idle and while scrolling.
- A planet ahead says "not yet" and never opens.

## Tests required

- EditMode: the next-lesson rule from `map-progress`; planet states.
- PlayMode: Map → Go → (finish a lesson) → Map with the rocket moved;
  finishing a planet plays the trip; skipping it.
- Emulator: every size in `AGENTS.md`'s table (it is the home screen);
  the cut-out profile.

## Touches

`Scripts/Core/Map`, `Scripts/UI/Map`, the `Map` scene, new prefabs, a new
save file (`map-progress`).

## Screens

The galaxy map, the trip.

## Voice lines

Greetings, planet arrivals, "not yet", both languages.

## Before closing out

- Release (the next MINOR): the whole loop she can play alone.
