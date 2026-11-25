# Task: feature/galaxy-map

> **Status: Done 2026-11-25, released as 0.10.0.**

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

**A profile from before the map** (0.8.1 and earlier) has no `map-progress` yet, but Go
then opened the lessons one after another, so `MapProgress.Before` takes every lesson
before the one in `lesson-run.json` as done: an update keeps where she is.

Read safely: an unknown lesson id is kept and ignored (a later version may know it), a
malformed one dropped, duplicates merged; a damaged file is set aside and her place is
taken from `lesson-run.json` as for a profile from before the map (that file names the next
stop or one already done, so it never claims more than she did); a newer version's file is kept as it is (`SaveStore`'s rules). The
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
states, letters, the mix, nothing playable, all done), `RocketMoveTests`,
`MapLinesTests` (language choice, the greeting, every line exists in both languages),
`MapProgressTests` and `MapProgressDataTests` (the save's shape, unknown and malformed ids,
upgrade, a newer version's file never saved over), `SaveStoreTests` (dated broken copies). PlayMode: Map → Go → finish a lesson →
Map with the rocket one stop further; finishing planet 1 plays the trip; a tap skips it; a
sleepy planet never opens. Emulator: every size in `AGENTS.md`'s table and a cut-out;
the Profiler probe (0 B per frame idle and while scrolling).

### Decisions

- **No button that leads nowhere** (`AGENTS.md` → "main is always playable"): the brief's
  parent button "which does nothing yet" and the bottom bar's placeholders are empty,
  anchored places in the prefab (`AvatarPlace`, `ParentPlace`, `SkyBookPlace`,
  `StickerBookPlace`), so `profiles`, `parent-area` and `rewards` drop their buttons in
  without moving anything; nothing she can tap does nothing.
- **Go's word** ("Hai!" / "Go!", by the app's language in the tablet's settings) is two
  serialized strings on `MapScreen`: the Localization package has no string table yet, and
  one word doesn't justify setting it up; `parent-area` (which brings the app's language
  switch and more words) moves it into a table.
- **The map speaks the next lesson's language** (Design note): in "both" she hears
  Romanian before a Romanian lesson and English before an English one.
- **The stops are 64 dp apart at least,** so a planet and its stops (12 on planet 1 in
  both languages) take most of a screen; the map scrolls, where the design shows three
  planets at once with fewer stops. A planet stands at the start of its stretch and its
  stops trail after its letters, so the rocket never hides a letter.
- **`look`'s whole pictures** (the lead, 2026-11-25): one picture per planet colour and
  state (`Art/Planets/`, face, ring, sparkle, "z z" and shadow drawn in), its ball (60% of
  the picture, 5% above its middle) taking the taps; the rocket is `Art/Trip/rocket-standing`
  on the map, `rocket-flying` while it hops and `rocket-flying-fast` on the trip, past
  `Map/moon` and `Map/earth`. Five colours repeat (gold, pink, green, purple, blue).
- **Tapping an awake or beaming planet** says the greeting again (from the review: nothing
  she can tap does nothing).
- **A known trade-off:** tapping a done stop while the next stop is half-way starts that
  next lesson again later (`lesson-run.json` holds one lesson in progress); her map
  progress is untouched.
- **A known limit of the migration:** before the map, Go wrapped from the last lesson to
  the first, so a child who had finished every lesson before 0.10.0 starts the map with
  nothing done (no one has: planets 1–3 came in 0.8.1).

### Fresh-eyes review (2026-11-25, a second agent, before the push)

Findings and how they were resolved:

1. Medium: a damaged `map-progress` threw her place away. **Fixed:** a set-aside file
   falls back to `lesson-run.json` as a missing one does
   (`MapProgressDataTests.TestLoad_DamagedFileKeepsWhereSheWas`).
2. Medium-low: a migration from an unreadable `lesson-run.json` is saved for good.
   **Recorded** as a limit (only a newer version's file or a locked one; the wrap-around
   edge in "Decisions").
3. Low: tapping another stop drops the half-way lesson's step. **Recorded** in "Decisions".
4. Low: a failed save of the lesson to open still opened the Lesson scene (the wrong
   lesson). **Fixed:** it logs an error and stays on the map.
5. Low: Go during the trip. **Fixed:** Go is ignored while the trip plays.
6. Low: `MapRoute` compared positions from two lists. **Fixed:** stops are compared by
   lesson id and states.
7. Low: an awake or beaming planet's tap did nothing. **Fixed:** it says the greeting.
8. Low: with no content, Go stayed visible and did nothing. **Fixed:** Go is hidden until
   the map is drawn with a go stop.
9. Low: "0.8.1 and earlier" in the migration's doc. **Fixed** (0.9.0, after the rebase).
10. Low: tests. **Added** a newer version's file never saved over; `Before` with a lesson
    outside the order was already tested; retired lessons need synthetic content and are
    covered by `LessonOrder`'s own filter (left).
11. Low: "Încă nu." / "Not yet." has no verb. **Fixed:** "Aici ajungem mai târziu. Hai să
    mergem întâi la rachetă." / "We will get here later. Let's go to the rocket first."


### Pushed

- `ef62757` `content:` the map's stop-ahead and all-planets lines;
- `38a9758` `core:` the map's route, rocket moves and lines (and `MapProgress.Before`);
- `15a82d8` `game:` each damaged save file kept, dated;
- `df79d86` `game:` the map's runner and her `map-progress` save;
- `058f2ca` `ui:` the galaxy map, its stops, the rocket and the trip;
- `007b2b4` `docs:` this Progress; then the close-out (0.10.0).

### Test results

- **Batch mode on the final tree** (rebased on 0.9.0): EditMode 597/597 (was 556), PlayMode 26/26
  (was 20, plus a throwaway probe), `BuildAndroidDebug` exit 0, no `warning CS` and no error in any
  log. The game slice alone (`df79d86`, before the UI) also passed EditMode and PlayMode.
- **Allocations** (Editor Play mode, a throwaway `ProfilerRecorder` "GC Allocated In Frame" probe in a
  PlayMode test, not committed): an empty scene, the map idle, the map dragged by a simulated finger
  and the map coasting all read **290 B per frame on average** (the test harness's own; 362 B at most
  now and then, in the empty scene too): the map adds 0 B. Main thread under 1 ms a frame in the
  Editor. Not measured: the device Profiler, the Frame Debugger (batch mode).
- **Emulator** (`emulator-5602`, a 2560 × 1600 tablet AVD, Android 16, `wm size`): the first start
  (planet 1 awake, a ringed, the rocket on `ro-a`, "Hai!"), a stop ahead tapped (its line in the bubble,
  nothing opened), the map dragged to planets 2 and 3 (asleep, dimmed), the trip (the rocket past the
  Earth, the pink planet coming in) and the landing with its line, every planet done (all beaming,
  the rocket back on the first stop), a damaged `map-progress.json` (set aside as
  `map-progress.broken-20261125-172813.json`, her place kept from `lesson-run.json`), Go opening
  `ro-t`. Sizes: 2000 × 1200, 1280 × 800, 2560 × 1600, 2048 × 1536, 2400 × 1080 and 2000 × 1200 with
  the "tall" cut-out (the HUD moves below it); screenshots in the agent's scratch folder. The phone
  (2400 × 1080) works but Go sits over the path's middle stops (scroll to reach them).
- **Not run:** the Device Simulator and the Editor's Profiler window (batch mode only), listening by
  ear (no clip and no installed voice on the emulator: the lines were read in the bubble), the
  pause-and-resume step on the emulator.


## How to test locally

Prerequisites: the worktree's Unity project (`game/`), `UNITY` set to
`C:\Program Files\Unity\Hub\Editor\6000.3.29f1\Editor\Unity.exe`, `ADB` to the SDK's
`platform-tools/adb.exe`, your emulator as `EMU` (a tablet AVD, landscape, started with
`-feature -Vulkan`), a scratch folder as `S`, and
`F=/storage/emulated/0/Android/data/com.softshack.scolita/files/profiles/child-1`.

### Core slice

1. `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: every test passes, among them `MapRouteTests` (12), `RocketMoveTests` (6),
   `MapLinesTests` (9), `MapProgressTests` (5).
2. What they prove: the next stop is the first lesson not done; planets beam, wake and
   sleep; a letter is met, current or ahead; only her languages' lessons are stops; the trip
   plays only to a later planet; the map greets in the next lesson's language; every map
   line exists in both languages; a profile from before the map keeps its place.

### Game slice

1. The same EditMode run. Expected: `SaveStoreTests` (a damaged file is kept as
   `<name>.broken-<yyyyMMdd-HHmmss>.json`, a second one beside it, never replaced) and
   `MapProgressDataTests` (the save's shape, unknown ids kept, malformed ones dropped, a
   damaged file set aside keeping where she was, a newer version's file never saved over,
   a profile without the file keeps where she was) pass.
2. Unhappy path on the emulator, after the UI slice is installed: write a broken save,
   `"$ADB" -s $EMU shell "echo '{ not json' > $F/map-progress.json"`, start the app.
   Expected: the map keeps her place from `lesson-run.json` (the lessons before the one
   named there are done), and
   `"$ADB" -s $EMU shell ls $F` shows `map-progress.broken-<date-time>.json` beside a new
   `map-progress.json`; do it twice: two dated copies.

### UI slice

1. `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: every test passes, among them `MapFlowTests` (6: the rocket on the next
   lesson, Go → finish → the rocket one stop on, finishing planet 1 plays the trip, a tap
   skips it, "not yet" never opens, a stop done opens again) and `SceneFlowTests` (3).
2. Build and install: `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`,
   then `"$ADB" -s $EMU install -r game/Builds/Android/Scolita-<version>-debug.apk` and
   `"$ADB" -s $EMU shell pm clear com.softshack.scolita`.
3. **First start** (`"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`):
   the purple sky, the rainbow path, planet 1 (gold, awake, its ring) with a m t p on tiles
   below it (a ringed in yellow, the others faint), planet 2 and 3 dimmed and asleep, the
   stops on the path under planet 1 (the first one the big yellow dot), the rocket standing
   over it, the unicorn top left with "Bună! Azi învățăm litera a." in its bubble (the star
   badge), and the yellow "Hai!" at the bottom. Heard (with a Romanian voice installed):
   the same line.
4. **Go:** tap "Hai!". Expected: `ro-a` opens. Finish it, tap "Mai departe": the map comes
   back, the first stop has a green tick, the rocket flies (lit flame) one stop along the
   path to `en-a`, and the bubble says "Hello! Today we are learning the letter a." with the
   blue ringed-planet badge (the next lesson is English).
5. **Scroll:** drag the map left and right: it follows the finger and coasts; the mascot,
   its bubble and "Hai!" stay put.
6. **Not yet:** tap planet 3. Expected: it wobbles and "Planeta aceasta încă doarme.
   Ajungem și acolo curând." is said and shown; nothing opens. Tap a faint stop on planet 1:
   it wobbles, "Aici ajungem mai târziu. Hai să mergem întâi la rachetă."; nothing opens.
7. **A stop done opens again:** tap the ticked stop: `ro-a` opens from its start.
8. **The trip:** push planet 1 done with the rocket last shown on its party:
   `echo '{"version":1,"doneLessons":["ro-a","en-a","ro-m","en-m","ro-t","en-t","ro-p","en-p","ro-words-1","en-words-1","capitals-1","party-1"],"rocketStop":"party-1"}' > $S/mp.json; "$ADB" -s $EMU push $S/mp.json $F/map-progress.json`,
   restart the app. Expected: the trip covers the screen for 4 seconds: the rocket flies
   past the moon and the Earth through the stars and the pink planet comes to meet it; then
   the map with planet 1 beaming (its sparkle), planet 2 awake, the rocket on `ro-s`, and
   "Am aterizat pe o planetă nouă!" then "Bună! Azi învățăm litera s." Push it again and
   tap during the trip: it stops at once, with the same landing.
9. **Every planet done:** push every lesson of planets 1–3 done (`rocketStop` `party-3`).
   Expected: all planets beaming, the rocket on planet 1's first stop (no trip back), and
   "Bravo! Ai fost pe toate planetele. Hai să le vizităm din nou!".
10. **An update keeps her place:** `"$ADB" -s $EMU shell rm $F/map-progress.json`, push a
    `lesson-run.json` naming `ro-t` (`{"version":1,"lessonId":"ro-t","stepNumber":1,"stepsSignature":""}`),
    restart. Expected: a, m done (ticked), the rocket on `ro-t`.
11. **Sizes:** `"$ADB" -s $EMU shell wm size 2000x1200` (then `1280x800`, `2560x1600`,
    `2048x1536`, `2400x1080`), restart each time: the planets, the path, the rocket, the
    mascot and "Hai!" all on screen, nothing under the safe area's edges.
12. **Pause:** press Home during a line and come back: the line is said again.

Cleanup: `"$ADB" -s $EMU shell pm clear com.softshack.scolita`, `"$ADB" -s $EMU shell wm size reset`.

## Completion record

_Finished 2026-11-25 by `galaxy-map`; released as 0.10.0 (tag `v0.10.0`)._

### What was built

- **Pushed:** `ef62757` `content:`, `38a9758` `core:`, `15a82d8` `game:` (dated broken copies), `df79d86` `game:` (the runner and the save), `058f2ca` `ui:` (the map, the prefab and the scene), `007b2b4` `docs:` (Progress), then this close-out (tagged).
- **Core** (`Scripts/Core/Map`): `MapRoute` (a planet per unit with something to play, its stops in
  teaching order, the next stop = the first not done, else the first stop again; planet states
  sleepy / awake / beaming; letter states met / current / ahead), `MapProgress` (the lessons done,
  the stop last shown, and `Before` for a profile from before the map) behind `IDoneLessons`,
  `RocketMove` (placed, a hop, or the trip, decided from the stop last shown), `MapLines` (the
  greeting in the next lesson's language, the landing, the two "not yet"s, all done).
- **Game:** `MapRunner` (the Map scene's composition root), `MapScreenView`, `IMapInput`
  (`IGuideInput` split out of `ILessonScreenInput`, so the guide works on any screen),
  `MapProgressData` / `MapProgressFile` (`map-progress.json`), `LessonRunner` records a finished
  lesson, `ActivityRegistry.PlayableKinds` (one rule for the map and the lesson), `SaveStore` keeps
  each damaged copy dated.
- **UI** (`Scripts/UI/Map`): `MapScreen`, `MapLayout` (+ `MapLayoutSettings` on the prefab),
  `PlanetView` (`look`'s whole pictures), `LetterTile`, `StopView`, `RocketView`, `TripView`,
  `RainbowPath` (one mesh from the Map atlas), `TapArea`, `Wobble`; `MapScreen.prefab` and the
  `Map` scene (with its `MapRunner` and the Voice prefab).
- **Content:** 4 voice lines (stop-ahead, all-planets, both languages).
- **Tests:** EditMode `MapRouteTests`, `RocketMoveTests`, `MapLinesTests`, `MapProgressTests`,
  `MapProgressDataTests`, `SaveStoreTests` (dated copies); PlayMode `MapFlowTests` (6) and the scene
  flow and lesson tests keeping the Editor's `map-progress.json`.

### Conventions for later briefs

- **Ask the map's progress through `IDoneLessons`.** `progress-review` puts `LearnerProgress` behind
  it and migrates `map-progress.json` (version 1: `doneLessons`, `rocketStop`) into its `progress`
  file; keep `rocketStop` (or its equivalent), it is what decides the trip.
- **The map opens a lesson by writing `lesson-run.json`** (unless it already names that lesson), so
  the Lesson scene needs no other hand-off; no static state between scenes.
- **What can be played** is `ActivityRegistry.PlayableKinds()` for both scenes.
- **A screen with the guide** binds `GuideFigure` to an `IGuideInput` and speaks through a
  `LessonVoice` (one line at a time, its bubble, said again on a tap).
- **Places kept on the map:** `SafeArea/AvatarPlace` (`profiles`), `SafeArea/ParentPlace`
  (`parent-area`), `SafeArea/BottomBarPlace/SkyBookPlace` and `StickerBookPlace` (`rewards`): put the
  buttons there; the band itself isn't drawn yet.
- **A planet's look** is a `PlanetLook` (sleepy, awake, beaming pictures) in `MapScreen`'s list; a new
  colour is a new entry. Its ball is 60% of the picture, 5% above its middle.
- **The PlayMode tests keep and restore** the Editor's `map-progress.json` as they do
  `lesson-run.json` (`MapScene.KeepFiles`).

### Decisions and deviations

In Progress → "Decisions" and "Fresh-eyes review": the parent button and the bottom bar are empty
places, not buttons that do nothing (`AGENTS.md`: nothing leads nowhere); Go's word is two
serialized strings, not a Localization table yet; the map speaks the next lesson's language; the
stops are at least 64 dp apart, so the map scrolls more than the design's one screen; a planet leads
its stops so the rocket never hides a letter; `look`'s whole pictures replace the parts (the lead,
2026-11-25); a damaged or missing `map-progress` keeps her place from `lesson-run.json`; the dated
broken copies are built here (the lead), their cleanup is `progress-review`'s.

### Follow-ups

- **`progress-review`:** replace `MapProgress` behind `IDoneLessons`, migrate `map-progress.json`
  (keep its dated broken copies' rule; build their cleanup); the map's stops follow the mix it reads;
  the known limits in Progress (a migration from an unreadable `lesson-run.json`; Go wrapping to the
  first lesson before the map).
- **`rewards`:** the bottom bar's band and buttons in its places (sky book with the star count,
  sticker book; the design's third, trails), the stars of a finished lesson flying to the map, the
  rocket's trail along the path (`RocketView`).
- **`parent-area`:** the parent button in `ParentPlace`; move "Hai!" / "Go!" into a string table
  with the app's language switch.
- **`profiles`:** the avatar in `AvatarPlace`; `MapRunner` and `LessonRunner` still open
  `BuiltInProfiles.FirstChild`.
- **`sound-feel`:** the rocket's take-off and the trip's whoosh; music ducking on the map.
- **`look` (a chore):** delete the old planet parts and rocket in `Art/Map/` (`planet-<colour>`,
  `-face-*`, `-ring-*`, `planet-shadow`, `planet-sleepy-z`, `rocket`, `rocket-flame-*`) and their
  drawing code: the map no longer uses them. The Earth's picture shows a faint square edge on the
  trip.
- **The phone size:** Go covers the path's middle on 2400 × 1080 (it works; a later layout pass).
- **The human:** hear the new lines; the 0.10.0 rows in `docs/manual-testing.md`.
