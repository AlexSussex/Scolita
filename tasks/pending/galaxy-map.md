# Task: feature/galaxy-map

> **Status: In progress (galaxy-map)** (since 2026-11-25). Decision D5 (the
> mascot) is not needed: a placeholder mascot until `look`.

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

## Progress

_Agent `galaxy-map`, worktree `C:\Projects\scolita-map`, emulator `emulator-5602`._

### Design note (a medium change: a new screen, a new save file)

**Where the logic lives.** Everything the map decides is in Core (`Scripts/Core/Map`, no
UnityEngine), the Unity side only draws it and forwards taps:

- `IDoneLessons` (`IsDone(lessonId)`): the one question the map asks of her progress.
  `MapProgress` answers it now (from the `map-progress` save); `progress-review` puts its
  `LearnerProgress` behind the same interface and migrates the file.
- `MapRoute`, built from the domain, the existing `LessonOrder` (her language mix, the
  steps that can be played) and `IDoneLessons`:
  - **planets** = the domain's units that are not retired and have at least one lesson
    she can play, in content order (a planet per unit, never hard-coded);
  - **stops** = a planet's playable lessons in teaching order, each `Done`, `Next` or
    `Ahead`;
  - **the next stop** (where the rocket stands and what Go opens) = the first stop not
    done; when every stop is done, the first stop again (as `LessonOrder.After` already
    does after the last lesson), with the "every planet" line;
  - **planet states:** `Beaming` when all its stops are done, `Awake` for the next stop's
    planet, `Sleepy` after it;
  - **a planet's letters** = the letters its stops introduce, in order, each `Met`,
    `Current` (the next stop's letter) or `Ahead`;
  - **taps:** a done stop and the next stop open their lesson; a stop ahead and a sleepy
    planet never open, they say "not yet".
- `RocketMove`: from the stop where the map last showed the rocket (saved) to the next
  stop: `Placed` (first visit, or backwards), `Hop` (one or more stops on the same planet)
  or `Trip` (to a later planet: the trip plays). Deciding it from the saved stop, not from
  "a lesson just ended", means a trip killed half-way or a lesson finished from a replay
  still ends right.
- `MapLines`: what the mascot says on the map, all from `voice-lines.csv`: the greeting
  (`today-letter` with the next stop's letter, else `hello`), `planet-landed` after a trip,
  `planet-asleep` and a new `stop-ahead` for "not yet", and a new `all-planets` when every
  stop is done. **The map speaks the next lesson's language** (an English lesson is greeted
  in English, "Hello! Today we are learning the letter m."), and the app's language for a
  lesson in both. `MapLines.All` lists them for the bubble's words, the clips to load and an
  EditMode test that every one exists in both languages.

**Go and the Lesson scene.** No static state is passed between scenes: the map writes the
lesson it opens into `lesson-run.json` (the file the Lesson scene already resumes from)
unless that file already names the same lesson, so the lesson in progress still resumes at
its step. `LessonRunner` adds one thing: when a lesson ends it records it done in
`map-progress`.

**The save** (`profiles/<id>/map-progress.json`, through `SaveStore` only):

```json
{ "version": 1, "doneLessons": ["ro-a", "en-a"], "rocketStop": "ro-m" }
```

Read safely: an unknown lesson id is kept and ignored (a later version may know it), a
malformed one dropped, duplicates merged; a damaged file is set aside and the map starts
from the first lesson; a newer version's file is kept as it is (`SaveStore`'s rules). The
human's rule for damaged files (each copy kept, dated `<name>.broken-<date-time>.json`) is
added to `SaveStore` itself, so every file gets it; the cleanup of old copies stays with
`progress-review`.

**The Unity side** (`Scripts/Game/Map`, `Scripts/UI/Map`): `MapRunner` is the Map scene's
composition root (as `LessonRunner`): it builds the curriculum, the voice and the route,
hands the screen what to draw, and opens the lesson. `MapScreen` draws the sky, the rainbow
path (one mesh, built once), the planets from `look`'s parts (body, face by state, ring on
the awake one, shadow, "z z z"), the letter tiles, the stops (at least 64 dp to tap), the
rocket, the guide and its bubble, and Go ("Hai!" / "Go!" by the app's language). It scrolls
sideways by drag (`ScrollRect`) and opens on the rocket. `TripView` is the trip: a
full-screen flight past the moon and the Earth for 4 seconds, skipped by a tap. The avatar,
the parent button and the bottom bar get empty, anchored places only (see "Decisions").
Nothing allocates per frame: positions are computed when the map is built, the animations
move cached transforms.

**Another domain.** Nothing here knows about letters except the planet's tiles: a numbers
or music domain is units and lessons too, so it gets its planets, stops, trip and Go for
free; its tiles would show what its lessons introduce (a `PlanetLetter` becomes a general
"item" when a second domain needs it, not before).

**Tests.** EditMode: `MapRouteTests` (the next stop from `map-progress`, planet and stop
states, letters, the mix, retired and unplayable lessons, all done), `RocketMoveTests`,
`MapLinesTests` (language choice, the greeting, every line exists in both languages),
`MapProgressTests` and `MapProgressDataTests` (the save's shape, unknown and malformed ids,
upgrade), `SaveStoreTests` (dated broken copies). PlayMode: Map → Go → finish a lesson →
Map with the rocket one stop further; finishing planet 1 plays the trip; a tap skips it; a
sleepy planet never opens. Emulator: every size in `AGENTS.md`'s table and a cut-out;
the Profiler probe (0 B per frame idle and while scrolling).

### Decisions

- **No button that leads nowhere** (`AGENTS.md` → "main is always playable"): the brief's
  parent button "which does nothing yet" and the bottom bar's placeholders are empty,
  anchored places in the prefab (`AvatarPlace`, `ParentPlace`, `SkyBookPlace`,
  `StickerBookPlace`), so `profiles`, `parent-area` and `rewards` drop their buttons in
  without moving anything; nothing she can tap does nothing.
- **The stops are 64 dp apart at least,** so a planet and its stops (12 on planet 1 in
  both languages) take most of a screen; the map scrolls, where the design shows three
  planets at once with fewer stops.
