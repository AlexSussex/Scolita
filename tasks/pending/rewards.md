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
