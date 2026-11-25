# Task: feature/rewards

> **Status: In progress (rewards).**

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

## Progress

_Agent `rewards`, worktree `C:\Projects\scolita-rewards`, emulator `emulator-5602`._

### Design note (a medium change: a new system, three content types, a new save file, four screens)

**Where the logic lives.** Every reward rule is in Core (`Scripts/Core/Rewards`, no
UnityEngine); the Unity side draws it, forwards her taps and saves.

- **`RewardBook`**, her rewards (one per profile, the state only): the stars earned (a total that
  only grows), the lessons whose stars are lit (in the order first lit), how many lessons she has
  finished, the stickers given (in order; a sticker comes again only in a later round), the
  planets whose end was rewarded, the trails earned and the one chosen, and the stickers placed
  on the scene (sticker, x and y from 0 to 1 in the scene, size, in drawing order).
- **`Rewards`**, the one system that gives rewards (`RewardContent`, `RewardRules`, a seeded
  `IRandom`). `LessonFinished(book, lesson, order)`:
  - **stars:** `RewardRules.StarsPerLesson` (3) for every finished lesson, whatever the hints, a
    replay too (the total grows; the constellation lights the lesson's places once);
  - **a sticker** when the count of finished lessons is a multiple of `LessonsPerSticker` (2), or
    when the lesson finishes its planet for the first time (one sticker at most per lesson end):
    picked at random among the stickers not given in the current round (a round ends when every
    sticker that isn't retired has been given), so none repeats until all are given;
  - **a trail** the first time a planet is finished (all of the planet's lessons in her
    `LessonOrder`, so her language mix, have their stars): the next trail in `trails.csv`'s order
    not earned yet; the first one earned is chosen at once;
  - it raises `RewardEarned` (stars, sticker or trail) for each, for `sound-feel`'s sounds, and
    returns them as `LessonRewards` for the lesson's end.
  - Nothing is ever taken away: no method removes stars, stickers or trails, and nothing is
    bought.
- **`SkyChart`**: each planet of the order with its constellation. A constellation has 3 places
  per lesson of its planet (in her mix), spread over its figure by `ConstellationPlaces` (its
  stars first, then points along its lines, shared out by length), so any planet, however many
  lessons it has, fills its own figure; the lessons with stars light the places in the order she
  lit them.
- **The sticker scene's rules** (`RewardBook`): she can place a sticker as many times as it was
  given to her (two copies after a second round); moving or resizing keeps it in the scene (x, y
  clamped to 0..1, the size between the rules' smallest and largest); dropping it back on the
  book takes it off the scene (it stays in the book).
- **`RewardLines`**: what is said, from the content: the stars line of the constellation
  (`<lang>-line-stars-<constellation>`, already there for unicorn, rocket and whale), the
  sticker line (there), and new: a new trail, the constellation's name in the sky book, the sky
  book, the sticker book and the trail picker.

**The content format** (`content/README.md` gets the three files; the importer, the validator
and the ledger of released ids read them; `RewardTables` reads, `RewardContentRules` checks):

- **`stickers.csv`**: `id,retired,notes`, one row per sticker; its art is
  `Assets/Scolita/Art/Stickers/sticker-<id>.png` (`look`'s convention: a picture swapped in under
  the same name needs no other change; a missing picture is an import error). The 18 stickers of
  `look`.
- **`constellations.csv`**: `id,unit,stars,lines,retired,notes`: the planet it belongs to (one
  per unit that isn't retired, an error otherwise), its stars as `x:y` in a 640 × 480 frame
  (`470:56 418:150 …`), its lines as pairs of star numbers from 1 (`1-2 2-3 …`). It is drawn
  from this data (stars and lines), so a new constellation needs no art. Its lines must exist in
  each language of its planet's lessons. Planets 1–3: unicorn, rocket, whale (the stars lines
  already say so; `look`'s cat waits for a later planet, and the human's guitar for planet 4).
- **`trails.csv`**: `id,retired,notes`, in the order they are earned; the art is
  `Assets/Scolita/Art/Map/trail-<id>.png` (`look`'s rainbow, stars and bubbles).
- The importer writes `Content/Rewards/` (`StickerAsset`, `ConstellationAsset`, `TrailAsset` and
  their three catalogs); `RewardContentAssets.Build` turns the catalogs into Core's
  `RewardContent` at run time. The reward content stays beside the `Curriculum`, not in it: no
  change to `Curriculum`, `CurriculumAssets` or their callers (`progress-review` is changing the
  runners at the same time).

**The `rewards` save file** (`profiles/<id>/rewards.json`, through `SaveStore` only, version 1):
`starsEarned`, `litLessons[]`, `lessonsFinished`, `stickers[]` (given, in order),
`rewardedUnits[]`, `trails[]`, `trail`, `placed[]` (`sticker`, `x`, `y`, `size`).

- `Upgraded()` brings a file to the current shape: negative counts become 0, a text that can't
  be an id is dropped, an id the content doesn't know is kept and ignored (a later version may
  know it), a placement out of range is clamped, the chosen trail must be earned. A later
  version adds its migration step there.
- A damaged file is set aside, dated, by `SaveStore`, and the defaults are used.
- **A profile from before rewards** (no file yet): her finished lessons (the map's
  `IDoneLessons`) get their stars and her finished planets their trails, so the sky matches the
  map (no stickers are given for the past).
- It is saved when a lesson's rewards are given, when a sticker is placed, moved, resized or
  taken off, and when a trail is chosen; never per frame.

**The Unity side.**

- `Scripts/Game/Rewards`: `RewardsData` and `RewardsFile` (the save), `RewardRulesAsset`
  (`Settings/RewardRules.asset`: stars per lesson, lessons per sticker, the sticker sizes; beside
  `LearningRules.asset`, which `progress-review` is changing now), `RewardsSession` (a plain
  class the two runners make: the book, the file, the lines, the sprites; it handles the screens'
  taps through `IRewardsInput`).
- **The lesson's end** (design screen 6, `Prefabs/Rewards/LessonEnd.prefab` in
  `LessonScreen.prefab`, replacing the end panel): "Bravo!", the planet's constellation with the
  three new stars flying into their places, the mascot cheering, the sticker popping out of a
  gift box when earned (and the new trail), **Mai departe** and **În cartea mea** (opens the
  sticker book there, in the Lesson scene). `LessonRunner` gets one call at the lesson's end; it
  says the stars line (instead of "Ai terminat lecția"), then the sticker and trail lines.
- **The map's bottom bar** (in `MapScreen.prefab`'s places): the band, **Cerul meu** with the
  star count in a yellow pill, **Abțibilduri** and **Dâre** (a third place, `TrailsPlace`). The
  sticker book shows once she has a sticker and the trails once she has a trail, so no button
  leads to an empty screen.
- **The sky book** (`SkyBook.prefab`): a constellation per planet, lit star by star, each over
  its planet's picture; tapping one says its name.
- **The sticker book and scene** (`StickerBook.prefab`): the Earth on the purple sky, and the
  book (a strip of her stickers, with how many copies are left). Drag one onto the scene, drag it
  around, pinch it with two fingers to resize it, drag it back to the book to take it off; it
  stays where she put it (saved on each release).
- **The trail picker** (`TrailPicker.prefab`): the trails she has; the chosen one has a yellow
  ring and the rocket wears it on the map (`RocketView`'s trail).
- Every overlay has the back button (top left) and its spoken line; `MapRunner` gets one call to
  make the session. Every star, line and sticker image is made when its screen opens, never
  while it runs; the flights and the drags move cached transforms, allocating nothing.
- **Art** (`design/art/draw/`, the agents' drawings, D7): the gift box (box and lid) and the
  three bar icons (constellation, sticker, trail).

**Tests.** EditMode: `RewardsTests` (three stars whatever the hints, a sticker every second
lesson and at a planet's end, no repeat until all are given over a few rounds, retired stickers,
trails in order, nothing lost), `ConstellationPlacesTests`, `SkyChartTests`, the sticker scene's
rules, `RewardTablesTests` and `RewardContentRulesTests`, `RewardsDataTests` (round trip,
`Upgraded`, unknown ids kept, the seed from the map), the importer's and the ledger's tests with
the new files, the rules on the real content. PlayMode: finishing a lesson lights its stars and
shows the sticker; placing a sticker saves it (read back from the file); the bar's buttons open
their screens. Emulator: the end, the sky book and the sticker scene at 2000 × 1200 and
1280 × 800, a two-finger resize with `motionevent`.

**Another domain.** Nothing here knows about letters: a numbers or music domain's units get
their constellations in `constellations.csv`, its lessons give stars, stickers and trails the
same way (`LessonFinished` takes any lesson and its order), and new stickers or trails are rows
and pictures.

### Slices

- **Pushed:** `a02b97a` `core:` (Core/Rewards: `RewardGiver`, `RewardBook`, `SkyChart`,
  `ConstellationPlaces`, `RewardTables`, `RewardContentRules`, `RewardLines`), `240d730`
  `content:` (the three CSVs, 14 voice lines, the asset types and catalogs), `a256f28` `build:`
  (the importer and the ledger read the rewards; `Content/Rewards/` imported).
- **Then:** `game:` (the `rewards` save, `RewardsSession`, the runners' hook, the prefabs, the
  scenes' fields, `RewardRules.asset`) and `ui:` (the end, the bar, the books, the trail), with the
  art (`design/art/draw/`: the gift box and lid, three bar icons, the constellation's line).

### Decisions

- **The reward content sits beside the curriculum** (`RewardContent`), so `Curriculum`,
  `CurriculumAssets` and their callers don't change while `progress-review` changes the runners.
- **Constellations are drawn from data** (`constellations.csv`): `look`'s three constellation
  PNGs are not used (kept for the design; a `look` chore can delete them). Planet 3 is the whale
  (its stars line was already in the content); `look`'s cat and the human's guitar wait for later
  planets.
- **A replay gives 3 stars again** (the total only grows), and its places light once.
- **One sticker at most per lesson end**, also when the second lesson and a planet's end fall
  together.
- **A sticker can be placed as many times as it was given** (copies), so the scene never fills up.
- **The trail is chosen at once when it is the first**; later ones are chosen in the picker.
- **The sticker book and the trails show on the bar once she has one** (no button to an empty
  page).
- **The end says the stars line instead of "Ai terminat lecția. Bravo!"** (it begins with
  "Bravo!"), then the sticker's and the trail's lines.
- **The rules asset is its own** (`Settings/RewardRules.asset`), beside `LearningRules.asset`,
  which `progress-review` changes now; they can merge later.
- **The bar's and the end's words are Romanian only** ("Cerul meu", "Abțibilduri", "Dâre",
  "În cartea mea"), as "Mai departe" is: `parent-area` moves them into the string tables with
  the app's language. The spoken lines follow the map's or the lesson's language.
- **Version** on the map moved up above the bar (the third button needed its place).

### Usability (the new screens)

Every page has the back button top left (88 px, under the mascot), and its line is said as it
opens, with the mascot and the bubble kept on top; buttons are 72–96 px, stickers on 124 px
tiles; nothing can be lost (taking a sticker off puts it back in the book); no numbers but the
star count; the stars land one by one with no timer to beat.

### Measured

- **GC while the end runs and while she drags or pinches a sticker** (a PlayMode probe with the
  `GC Allocated In Frame` counter, in the Editor): 370 B a frame, the same as an idle lesson in the
  Editor's test harness (its own baseline); a few frames 72 B more (the canvas rebuild when a
  star lands). Nothing allocated by the rewards' own frame code.

### Fresh-eyes review

A second agent (`feature-dev:code-reviewer`, not the author) read the core, content, importer and
Unity commits against AGENTS.md. No blocker. Findings and what was done:

1. **A drag out of the strip cut short by closing the book stuck** (the loose copy came back, no
   more drags): fixed, the book drops it when it opens again; PlayMode covers it.
2. **A second finger on the strip drove the first finger's copy:** fixed, `BeginFromTray` says
   whether it started and the strip item only follows its own drag.
3. **A retired sticker or trail could lose its picture** (hers, invisible): fixed, the import
   wants the picture of every row, retired too.
4. **A saved size was not kept within the rules on load:** fixed, `RewardBook.FitSizes` once the
   book is read (and the view clamps); EditMode covers it.
5. **A constellation's lines in a language its planet doesn't have** (a later Romanian-only
   planet on an English map): fixed, the session takes a language the planet's lessons have.
6. **`RewardsData.Upgraded` called the public `ToBook`:** fixed (a private `BuildBook`).
7. **"3 stele" in the stars lines and `StarsPerLesson`:** kept, written in the rules asset's doc and
   in Decisions; change both together.
8. **`RewardLines.CommonKeys` was a writable array:** now read-only.
9. **A sticker dropped from the strip at a size the rules may exclude:** it is placed at the
   rules' size.
10. **Missing cases:** EditMode now covers no sticker and no trail left to give.

The reviewer read the Romanian lines as correct. Its suggestion "Ai primit" for "Ai câștigat"
(a gift rather than a prize) is a question for the human; "Ai câștigat un abțibild!" was already
in the content.

## How to test locally

Prerequisites: the worktree's Unity project (`game/`), `UNITY` set to
`C:\Program Files\Unity\Hub\Editor\6000.3.29f1\Editor\Unity.exe`, `ADB` to the SDK's
`platform-tools/adb.exe` (with `ADB_LOCAL_TRANSPORT_MAX_PORT=5699` and `MSYS_NO_PATHCONV=1` in Git
Bash), your emulator as `EMU` (a tablet AVD, landscape, started with `-feature -Vulkan`), a scratch
folder as `S`, and `F=/storage/emulated/0/Android/data/com.softshack.scolita/files/profiles/child-1`.

### Core slice

1. `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: every test passes, among them `RewardGiverTests` (9), `RewardBookTests` (5),
   `ConstellationPlacesTests` (3), `SkyChartTests` (3), `RewardTablesTests` (3) and
   `RewardContentRulesTests` (5).
2. What they prove: every finished lesson gives 3 stars (a replay too, its places lit once); a
   sticker every second lesson and at a planet's end, none repeated until all are given (three
   rounds of five), never a retired one; the trails in `trails.csv`'s order, once per planet, the
   first worn at once; a profile from before rewards gets the stars of what it did and no
   stickers; a sticker is placed only as often as it was given, kept inside the scene, its size
   within the rules, taken off back into the book; the constellations' places (stars first,
   then along the lines by length); the files' columns, stars and lines; the content rules.

### Content slice

1. The same EditMode run. Expected: `ContentValidatorTests.TestCheck_TheRewardCsvsHaveNoProblem`
   (the repository's `stickers.csv`, `constellations.csv`, `trails.csv` and their lines),
   `TestValidate_EveryContentAssetIsValid` and the freshness test pass.
2. Break one: in `content/constellations.csv` change `planet-3` to `planet-9`, run
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`.
   Expected: exit 1, the log names `constellations.csv, row 4: … planet-9 … units.csv` and
   `the unit planet-3 has no constellation`. Undo it with `git checkout content/constellations.csv`.

### Build slice (the importer)

1. The same EditMode run. Expected: `ContentImporterTests.TestImport_WritesTheRewards` (the assets
   and catalogs under `Content/Rewards/` make the same rewards as the CSVs, a sticker's sprite is
   `sticker-<id>`), `TestMake_RefusesARewardWithNoPicture` and `ReleasedIdLedgerTests` (the reward
   ids are in the ledger's files) pass.
2. Add `no-such-sticker,,` to `content/stickers.csv` and import (above). Expected: exit 1,
   `stickers.csv, row 20: the sticker no-such-sticker has no picture: Assets/Scolita/Art/Stickers/sticker-no-such-sticker.png is missing`.
   Undo it.

### Game and UI slices

1. `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: every test passes, among them `RewardsFlowTests` (3): a finished lesson lights its
   three stars as they land and gives its sticker, "În cartea mea" opens the book; the map's bar
   shows the stars and opens the sky book and the trail picker; a sticker dragged onto the scene,
   pinched to twice its size and moved is saved and still there when the map opens again.
   EditMode's `RewardsDataTests` (6) and `RewardRulesAssetTests` (1) pass.
2. Build and install: `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`,
   `"$ADB" -s $EMU install -r game/Builds/Android/Scolita-<version>-debug.apk`,
   `"$ADB" -s $EMU shell pm clear com.softshack.scolita`, then start it:
   `"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`.
3. **A new profile:** the map's bottom bar shows only **Cerul meu** with **0** in its yellow pill
   (no sticker or trail yet, so their buttons wait).
4. **The end of a lesson:** tap "Hai!", finish `ro-a`. Expected: "Bravo!" top left, the unicorn's
   constellation on the right (its faint lines, dotted rings), three gold stars flying one after
   another from "Bravo!" into the first three places, each lighting as it lands; the mascot
   cheering; heard and shown: "Bravo! Ai aprins 3 stele în constelația Unicornului." No gift box
   (the first lesson). "Mai departe": the map, the bar now **3**.
5. **A sticker:** finish `en-a` (Go). Expected: the stars fly (English: "Well done! You lit 3
   stars in the Unicorn constellation."), then the purple gift box opens, its lid lifts and a
   sticker pops out on a cream card; then "You got a sticker!"; **În cartea mea** beside
   **Mai departe**.
6. **The sticker book:** tap **În cartea mea**. Expected: the Earth on the purple sky, the strip
   at the bottom with the new sticker; "Drag your stickers onto the Earth. …" said. Drag the sticker
   up onto the Earth: it stays there, the strip's sticker fades (no copy left). Drag it around;
   pinch it with two fingers on the emulator (Ctrl + drag) or on a tablet: bigger, smaller (half to
   two and a half times). Drag it back onto the strip: it leaves the scene, the strip's sticker
   is bright again. Put it back on the Earth, tap the back button (top left), then "Mai departe".
7. **It stays:** on the map tap **Abțibilduri** (now shown): the sticker is where she left it,
   at the size she left it. Kill the app (`"$ADB" -s $EMU shell am force-stop com.softshack.scolita`)
   and start it again: still there.
8. **The sky book:** tap **Cerul meu**. Expected: three cards (the unicorn over the gold planet,
   the rocket over the pink one dimmed, the whale over the green one dimmed), the unicorn with
   six stars lit; "These are your constellations. …" (or in Romanian, the map's language). Tap
   the unicorn: "This is the Unicorn constellation." Back closes it.
9. **A planet's end and a trail:** push every lesson of planet 1 done but the last, in both
   files: `echo '{"version":1,"doneLessons":["ro-a","en-a","ro-m","en-m","ro-t","en-t","ro-p","en-p","ro-words-1","en-words-1","capitals-1"],"rocketStop":"capitals-1"}' > $S/mp.json; "$ADB" -s $EMU push $S/mp.json $F/map-progress.json`,
   `"$ADB" -s $EMU shell rm $F/rewards.json` (so they catch up from the map), restart, Go
   (`party-1`), finish it. Expected: the stars fly, the gift box gives a sticker, the rainbow
   trail's card appears, and "Ai câștigat un abțibild! Ai câștigat o dâră nouă pentru rachetă!";
   the unicorn complete. On the map (after the trip) the rocket wears the rainbow trail and
   **Dâre** is shown; it opens the picker with the rainbow ringed.
10. **Unhappy paths:** `"$ADB" -s $EMU shell "echo '{ not json' > $F/rewards.json"`, restart.
    Expected: the map shows the stars of the lessons done on the map (caught up again), and
    `"$ADB" -s $EMU shell ls $F` shows `rewards.broken-<date-time>.json`. A sticker the content
    no longer has (edit `rewards.json` to give `no-such-sticker`) is kept in the file and not
    shown.
11. **Sizes:** `"$ADB" -s $EMU shell wm size 2000x1200` (then `1280x800`, `2560x1600`,
    `2048x1536`, `2400x1080`), restart each time and open the end, the sky book, the sticker book
    and the trail picker: everything on screen, the back button and the strip clear of the edges.

Cleanup: `"$ADB" -s $EMU shell pm clear com.softshack.scolita`, `"$ADB" -s $EMU shell wm size reset`.
