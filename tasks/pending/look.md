# Task: feature/look

> **Status: In progress (look).** Decisions answered
> 2026-11-24: D5 the unicorn astronaut, named by her, with her purple cat
> as the co-pilot; build the guide as data (its poses and moods as art in
> a catalog), so more guides later are art only (`feature/guides`); D6 the
> gold star "RO" and the blue ringed planet "EN"; D7 agents draw, AI tools
> where they help, some of her drawings as stickers.

Read `AGENTS.md` first, `design/README.md`, `design/screens.md` and the design canvas
(source in `design/canvas/`), and the completion records in `tasks/done/`. SeliSnake's
`design/research/ai-art-prompts.md` and its art decisions
(`C:\Projects\SeliSnake\local\todo\03-decisions.md` → D3) show what worked
there.

**Feature:** `feature/look`
**Implements:** `design/README.md`; PRD §3.1 (the mascot), §3.6 (stickers).
**Depends on:** `project-skeleton`.

## Scope

1. **A comparison canvas** of the mascot (two or three takes on the
   unicorn astronaut, with and without the purple cat co-pilot), the
   planets, and the two badges, for the human and her to choose from
   (decisions D5, D6). Her own drawings, if the human adds any to
   `local/potential-art/`, are a source; they stay in `local/`.
2. **The chosen art,** as sprites in sprite atlases: the mascot in its four
   moods (a few frames each, or a skeletal 2D rig if the 2D Animation
   package is agreed first), three planets with their faces in three
   states, the rocket and its trails, the sky, the rainbow path, the moon
   and the Earth, the badges, the writing board, 30 stickers (animals,
   instruments, planets, space things), the three constellations, and the 14 profile avatars (decision D9, chosen
   with her; her purple cat among them).
3. **`design/README.md` updated** with what was chosen and the final
   palette; licences in `ThirdParty/` for anything not drawn by agents.

## Out of scope — do not build

- Code beyond swapping placeholder sprites for the real ones in prefabs
  (each prefab listed in Touches).
- Pictures for words (`content-letters-1`, in this style).

## Acceptance criteria

- The human (and she) chose the mascot and the badges (recorded with the
  date).
- Every sprite in an atlas; the map draws in at most 20 batches (Frame
  Debugger).
- The badges are told apart in greyscale (a screenshot in Progress).

## Tests required

- EditMode: the content validator (every sprite reference resolves).
- Emulator: the map, a lesson and the sticker book with the new art at
  2000 × 1200 and 1280 × 800.

## Touches

`Assets/Scolita/Art`, the atlases, the prefabs that show the art
(listed when claimed), `design/`.

## Screens

All that show the art.

## Voice lines

None.

## Before closing out

- `local/for-human-next-steps.md`: "choose the mascot with her" while the
  canvas is up; "name the mascot" (her choice).
- Release (the next MINOR).

## Progress

_Kept by `look` while the brief is in flight._

### Status

- **Claimed** in `61ed045` (worktree `C:\Projects\scolita-look`, emulator `emulator-5604`).
- **Slice 1 (the art, its pipeline, the atlases, the placeholder prefabs, the launcher
  icon):** see "Slice 1" below for its commits and tests.
- **The human chose, 2026-11-25** (`local/look/answers.txt`):
  - ChatGPT's art for the planets, avatars, stickers, rocket, moon and Earth,
    sky and icon.
  - The guide is ChatGPT's unicorn (D) with a full astronaut body, and the cat
    is ChatGPT's (D, in a helmet).
  - Keep one badge pair for now.
  - The word pictures are applied, each to its own word.

  Slice 2 is in progress. `MapScreen.prefab`, `LessonHud.prefab` and
  `ProjectSettings.asset` are released for `galaxy-map`: the chosen art replaces
  the PNGs under the same GUIDs, so they need no change.
- ~~Waiting for the human and her~~ (the choosing page,
  `C:\Projects\Scolita\local\look\choose.html`): the guide's take (A, B or C by the
  agent, D by ChatGPT), whether the cat sits beside it, the cat (the agent's or ChatGPT's),
  the badges (A or B), and for the rocket, planets, moon, Earth and sky the agent's or
  ChatGPT's. Also the unicorn's name (hers). Until then the app shows take A and badges A.
- **Next, after the choice:** slice 2 swaps in the chosen takes (cut with `cut-sheet.mjs`
  if ChatGPT's), rebuilds the atlases, re-checks the map's batches and the greyscale badges,
  and records the choice with its date. Then the close-out (the next MINOR).

### Slice 1 (pushed 2026-11-24)

- **Commits:**
  - `bece56d` `build:` the import rules and the atlas builder;
  - `cb41425` `content:` the art (135 PNGs, 10 atlases), its drawing tools in
    `design/art/`, `sprite-borders.json`;
  - `15beb3c` `build:` the adaptive launcher icon and `ArtTests`;
  - `aeb299a` `ui:` the placeholder prefabs;
  - the `docs:` commit with this Progress and `design/README.md`.
- **What's in it** (`game/Assets/Scolita/Art/`):
  - `Sky/`: the sky and a sparkle.
  - `Map/`: 4 planets, their 12 faces, ring halves and shadow, "z z z", the
    stops, the rainbow band and wave, the rocket, 2 flame frames, 3 trails, the
    moon and the Earth.
  - `Effects/`: the particles.
  - `Guides/`: `guide-unicorn` and `guide-cat`, 9 frames each.
  - `Badges/`: RO and EN.
  - `Board/`: the slate, a dash, the guide dot, the start dots, the arrow head,
    the trail brush and the finger glow.
  - `UI/`: 2 pill buttons, the card, the tile, the bubble, the panel, the home
    button and 6 icons.
  - `Stickers/`: 30 stickers.
  - `Constellations/`: 3 constellations, a lit star and an empty one (their star
    places are in `design/art/draw/constellations.mjs` → `constellationStars()`).
  - `Avatars/`: the 14 avatars.
  - `Icon/`: the adaptive icon's two layers.
  - `Atlases/`: one atlas per folder.
- **ChatGPT drafts:** all 25 of the human's sheets (`local/art-inbox/`) are on
  the choosing page as takes of their own, cut by `design/art/cut-sheet.mjs`.
  The cut list is `local/look/inbox.json`, with notes on what can't be used as
  it is. None is committed: only the chosen ones will be, after cleaning.

### Test results (2026-11-24, at the slice's commits, rebased on `b830308`, the last `origin/main`)

- **EditMode** 159/159 (154 + 5 `ArtTests`), **PlayMode** 3/3, in batch mode. No `warning CS` or
  `error CS` in any log.
- **`BuildAndroidDebug`:** exit 0, `Scolita-0.1.0-debug.apk` (47 MB; `aapt` shows the adaptive icon `mipmap-anydpi-v26/app_icon.xml` and no `INTERNET`).
- **Emulator** (`emulator-5604`, `Scolita_Tablet_look`: Pixel Tablet, Android
  16, x86_64, `-force-gles`):
  - Map → Lesson → Map at 2000 × 1200 and 1280 × 800.
  - The sky, the guide, the yellow pill, the home button and the cream card are
    drawn as designed, with nothing cut off.
  - `logcat` shows only the three development-build socket errors.
  - In the app drawer, "Școlița" has the unicorn's helmet on purple; after the
    foreground was enlarged, the helmet fills the safe zone.
  - Screenshots are in the agent's scratch folder: `emu-map-2000.png`,
    `emu-lesson-2000.png`, `emu-map-1280.png`, `emu-lesson-1280.png` and
    `emu-drawer-1280.png`.
- **The badges in greyscale:** `badges1.png` in scratch, also on the choosing
  page. In grey the star and the ringed planet are told apart by shape alone, at
  128 px and at bubble size (40 px).
- **Batches:** the Frame Debugger needs the Editor's window, and agents run in
  batch mode, where the Editor's render statistics read 0 (tried with a
  throwaway PlayMode probe). Counted from the hierarchy instead:
  - **The placeholder Map:** 6 batches: the sky, the guide, the title, the pill,
    its label and the version.
  - **The Lesson:** 5.
  - **The real map** (`galaxy-map`): every planet part, stop, rocket and path
    piece is in the single `Map` atlas. So it costs about the sky + the Map atlas
    (1–2 batches if its pieces aren't interleaved with text) + the guide + the UI
    + the text, well under 20.
  - **Still to do:** the Frame Debugger check itself, in the Editor (the human,
    or `galaxy-map`).
- **Screen sizes:** 2000 × 1200 and 1280 × 800, as the brief asks. The art is
  stretched or 9-sliced in layouts that already scale from 1280 × 800, so the
  other sizes add nothing until the real screens come.
- **Not run:** the sticker book (it doesn't exist until `rewards`), the
  Editor's Play mode and the Device Simulator (batch mode only), the Profiler
  (no activity runs yet), listening (no voice lines).

### Follow-ups

- **The guide's catalog asset** (`GuideAsset` with its id, the voice line for its
  name, and frames by mood, in a `GuideCatalog`). It goes with `content-model`'s
  asset types once the human agrees its Design note, or in `feature/guides`.
- **The constellations' star places** come from `constellationStars()` in
  `design/art/draw/constellations.mjs`; `rewards` should read them from data (for
  example, exported to a JSON by `build.mjs`).
- **The unicorn constellation** is the weakest drawing. ChatGPT's unicorn
  constellation (on the choosing page) is clearer, and its star places could be
  traced into the agent's.
- **Palette matching of ChatGPT parts:** `cut-sheet.mjs` keys, despills, trims and
  scales. It does not recolour. If ChatGPT's takes are chosen next to the
  agent's, slice 2 brings the two styles closer (by redrawing the agent's parts,
  or by tinting).
- **The Frame Debugger batch check** in the Editor (see "Test results").

### Slice 2 (2026-11-25): the chosen art

- **The choice** (the human and her mother, 2026-11-25, `local/look/answers.txt`; given to the
  lead in chat):
  1. **The guide:** ChatGPT's unicorn (D) with a full astronaut body like B. B's head looked
     odd in its helmet; C's fitted best.
  2. **The cat:** ChatGPT's cat in a helmet (D), the same design on every screen.
  3. **The badges:** none loved, not important yet; keep one.
  4. **ChatGPT's** planets, avatars (the lavender bicycle; the cat in a helmet), stickers,
     rocket, moon and Earth, sky and icon.
  5. **The word pictures:** all applied.
  6. **The ideas and the constellations:** liked, for later.
- **The guide's full body: route (b).** Agents can't use ChatGPT, and a body drawn in code under
  ChatGPT's painted head would not match it. So:
  - the human has a prompt set in `local/art-prompts/look-guide-body.md` (step 8 in
    `local/for-human-next-steps.md`): the unicorn's full body in the 9 frames, the cat sitting in
    the 9 frames, the two together;
  - until it comes, `Guides/guide-unicorn/` is the agent's take C (`guides.mjs`), beside
    ChatGPT's cat. When the sheets arrive, adding them to `drafts.json` and running
    `import-drafts.mjs` swaps both in under the same file names (the same GUIDs, so no prefab
    changes).
- **What changed:**
  - **The art pipeline:**
    - `design/art/drafts.json` lists which part of which ChatGPT sheet becomes which sprite.
    - `design/art/import-drafts.mjs` cuts them into `Art/`, and the word pictures into
      `content/pictures/`.
    - `cut-sheet.mjs` gained: one part per grid cell (a neighbour's sliver over the edge is
      left out), padding, a centre crop to an aspect ratio, and a hue shift for one colour
      range. The blue planet is the purple one with only its purples turned blue: the cheeks
      and the gold stars stay as they were.
  - **The drawing code:** the drawings ChatGPT's art replaced are deleted from
    `design/art/draw/`. Left are:
    - take C, the stand-in;
    - Pisicuța;
    - badge take A;
    - the board, the UI kit, the path, the stops, the trails and the particles;
    - the constellations (now `constellations.mjs`);
    - the icon's background;
    - the planets in parts and the first rocket (kept for `galaxy-map`; two of the balls are also TwoSounds' glow and the missing-picture fallback).
  - **Removed:** `choose.mjs` (its page stays in `local/look/`) and my 30 stickers (nothing
    used them).
  - **Kept for `galaxy-map`** (the fresh-eyes review found its work in progress uses them by
    path and GUID):
    - the planet parts (bodies, faces, rings, shadow, z z z), the old rocket and its flames,
      all in `Map/`, still drawn by `world.mjs`;
    - they go once `galaxy-map` has switched to the chosen pictures (a follow-up for it).
  - **New folders, so each atlas fits one page:**
    - `Planets/`: the chosen whole planets;
    - `Trip/`: the rocket's six pictures, the crescent, the comet;
    - `Path/`: the rainbow wave, moved out of `Map/` with its GUID (nothing loads it by path).
    - The moon and the Earth stay in `Map/` at their old size (256 px), because `galaxy-map`
      loads them by path.
  - **New sprites:**
    - `Planets/`: `planet-<gold|pink|green|purple|blue>-<beaming|awake|sleepy>` (whole
      pictures);
    - `Trip/`: `rocket-<standing|flying|flying-fast|landing|friends|asleep>`,
      `moon-crescent`, `comet`;
    - `Stickers/`: 18 new ids (`rainbow`, `unicorn`, `guitar`, `bicycle`, `palette`,
      `moon-star`, `helmet-heart`, `whale-rainbow`, `tambourine`, `violin`, `telescope`,
      `comet`, `rocket-heart`, `earth`, `xylophone`, `sunflower`, `star-kite`,
      `dino-astronaut`).
  - **Replaced in place** (same names, same GUIDs, so the prefabs that use them need no
    change): `Sky/sky` (ChatGPT's starfield cropped to 16:10), `Map/moon`, `Map/earth`,
    `Guides/guide-cat/*`, 13 avatars (+ the cat on a disc), `Guides/guide-unicorn/*` (take C),
    `Icon/icon-adaptive-foreground` (ChatGPT's unicorn inside the safe zone).
  - **`TwoSoundsView.prefab`:**
    - each planet's `Planet` image is the whole ChatGPT sprite (gold awake for Romanian;
      blue awake for English, which answers `activities-letters`' "a blue planet"), sized so
      its ball fills the old ball;
    - its `Face`, `RingBack` and `RingFront` objects are removed (`SoundPlanet` never used
      them).
  - **`content/pictures.csv`:** 40 pictures have art (`content/pictures/`, imported into
    `Assets/Scolita/Content/Pictures/`):
    - 36 from the four word sheets;
    - `rocket`, `planet`, `moon` (the crescent) and `sleeping-moon` (the sleepy full moon)
      from the app's own art.
  - **`Art/SOURCE.md`** says which art is ChatGPT's (the human's, under OpenAI's terms) and
    which the agents drew.
- **Still placeholders: 48 pictures,** with ChatGPT prompts for them in
  `local/art-prompts/look-guide-body.md` (sheets F–K):
  - plane, bee, music, drum, apple, astronaut, tiger, pan-flute, pineapple, footprint, thorn,
    sieve, cork, watermelon;
  - drawing, dance, sled, sun, island, ring, cloud, glasses, eye, dolphin, elephant,
    helicopter, pan, ant, nap, sip;
  - mint-leaf, pants, pot, spinning-top, net, pet, nodding, tent, nest, pond, spot, mops,
    insect, igloo, octopus, orange, egg, patting.
- **To redraw** (applied for now, prompts written): `nose` (reads as "a boy") and `dot` (reads
  as a sweet). `ten` (ten stars in two rows), `star` (one star with a face) and `spot` (still a
  placeholder) stay apart. `sleeping-moon` and `moon` read as such.
- **The stickers are 18, not 30.** The human chose ChatGPT's, and 12 more are in the prompt
  file (sheets D and E). `rewards` can start with 18.
- **The badges stay take A** (the clearest in grey). Ideas for a pair she can tell apart
  without reading are in the follow-ups.

### Fresh-eyes review (2026-11-25)

A second agent read slice 1's code and slice 2's diff against AGENTS.md. It found no C# rule
broken. Findings and what was done:

1. **Blocker: `galaxy-map`'s work in progress uses the planet parts and the old rocket**
   (scene and prefab references, and paths in its scratch builder), which slice 2 had deleted.
   - **Done:** they are all kept at their paths and GUIDs, still drawn by `world.mjs`.
   - The chosen art is in new folders (`Planets/`, `Trip/`), and the Status line is
     corrected.
   - `galaxy-map` is told through the lead and in the follow-ups; the parts go once it has
     switched.
2. **The `Map/` atlas no longer fit one 2048 page.** Fixed:
   - the new planets are in `Planets/`, the new rocket pictures in `Trip/`, and the rainbow
     wave in `Path/` (its GUID kept; `galaxy-map` loads only the band);
   - the moon and the Earth are cut at their old 256 px;
   - `ArtTests.TestRebuildAll_EachFolderFitsOnePage` checks every folder (at most 85% of a
     page, padding included).
3. **Commit order and staging:** the commits are ordered so each one passes the tests:
   - the prefab switch comes before anything it stops using is removed, and nothing it uses
     is removed;
   - the seven stickers that kept their names keep their `.meta`, so their GUIDs don't change.
4. **The reference test was too narrow.** `TestGUIDToAssetPath_EveryReferenceResolves` now
   checks every GUID in every scene, prefab and asset under `Assets/Scolita`. The old test,
   named after no method, is replaced. Atlas discovery now works from the folders on disk, so
   each commit passes on its own.
5. **Stale docs and comments** fixed:
   - `cut-sheet.mjs` says it is called from `import-drafts.mjs`;
   - the paths to `constellations.mjs`;
   - the How to test counts (79 drawn, 107 cut);
   - the choosing page's step.
6. **"Deterministic" was too strong.** It now reads "the same code and the same Chrome". Both
   tools print Chrome's version, and `SOURCE.md` records it (158.0.8127.36) and says the
   ChatGPT parts can be re-cut only on the human's PC.
7. **Tool inputs are checked:**
   - `cutSheet()` checks `key`, `align`, `grid` and `size`;
   - `import-drafts.mjs` names any entry that refers to a part not cut, and any output path
     written twice.
8. **Nits:**
   - the importer now depends on `sprite-borders.json`, so a border change reimports
     (version 2);
   - `SpriteBorderTable` refuses an empty or repeated path;
   - "55% opacity" is spelt out;
   - the moon and the Earth stay at 256 px;
   - `guide-cat/sleepy-1` repeats `sleepy-0` (`SOURCE.md`) until the human's round;
   - `guide-unicorn` is take C now, a stand-in, so the map's guide changed from slice 1;
   - the 18 stickers keep the ids in `drafts.json` (7 kept slice 1's names, now with new
     art), so `rewards` uses those;
   - `TwoSoundsView`'s glow and drop area were checked on a render: the ball fills the old
     ball, the glow behind it.

### Design note (medium change: the art pipeline and the guide as data)

- **Where it lives:** all of it on the Unity side and in `design/`. No Core or learning
  code, and no `Scripts/Content` change, since `content`'s Design note is still waiting for
  the human.
  - **The drawings:** JavaScript that writes SVG (`design/art/draw/*.mjs`, the helpers in
    `lib/svg.mjs`). One run of the PC's headless Chrome renders them to PNG
    (`lib/render.mjs`): a page draws each SVG on a canvas and prints the PNGs into its DOM.
    That needs no npm package. SeliSnake drew its art in C# in the Editor; here SVG gives
    gradients, blur and text, so the shaded style is reachable in code.
  - **The import rules:** `Scolita.Editor.Art.ArtTextureImporter`, an AssetPostprocessor.
    Every PNG under `Art/` is a single full-rect sprite with no mipmaps and its 9-slice
    border from `sprite-borders.json`. The borders are written by `build.mjs` from the
    drawings' own table, so they are defined once.
  - **The atlases:** `ArtAtlases`, one Sprite Atlas V2 per folder, packing the folder (so
    new art needs no code). There is no rotation and no tight packing, because UI Images
    draw the sprite's rect; padding is 4 px, maximum 2048, high-quality compression.
  - **The launcher icon:** adaptive only (background and foreground layers). The minimum is
    API 26, so every launcher uses it.
- **The data:**
  - **The guide is data:** `Art/Guides/<guide-id>/<frame>.png`, with nine frames named by
    mood, the same for every guide. `feature/guides` adds a folder.
  - **The guide's catalog asset** waits for `content-model`'s asset types (a follow-up), so
    the two don't overlap. It should be `GuideAsset` (id, name voice line, frames by mood)
    in `GuideCatalog`.
- **Tests:** EditMode `Tests/EditMode/Art/ArtTests.cs` checks that:
  - every art PNG is a sprite;
  - each sprite is in exactly one atlas (`SpriteAtlas.CanBindTo`);
  - the borders match;
  - every sprite reference in the prefabs resolves;
  - the adaptive icon has both layers.
  The emulator shows the map and the lesson with the art.
- **Another domain:** music or numbers add folders (instruments, number tiles) that their
  atlases pack. The guide, the sky, the board and the UI kit serve them as they are.

### Decisions and deviations

- **Drawn in code, not bought or generated in the repo.** No third-party art, so there is no
  licence to add in `ThirdParty/`. ChatGPT drafts are welcome candidates (the lead's prompt
  pack); only cleaned parts would be committed.
- **The sky is 2016 × 1260 and the rainbow wave 2032 × 635,** not 2048 wide: a sprite plus
  the atlas's padding must fit in 2048.
- **The planets are parts** (body, face, ring back and front, shadow, z z z), so 4 colours ×
  3 faces cost 4 bodies and 12 small faces, and the ring passes behind the planet.
- **A small UI kit** (yellow and cream pill buttons with their lip, the cream card, the
  answer tile, the bubble, the translucent panel, the home button, six line icons) beyond
  the brief's list. The look rules ask for these on every screen, and the placeholder
  prefabs needed the yellow pill and the home button.
- **The moon avatar:** the canvas's crescent path has two arcs bulging the same way, so it
  draws only an outline. It is now a filled crescent (the face moved into it).
- **Prefabs changed (art only):**
  - `MapScreen`: the sky as the background, the yellow pill on "Hai!", and the guide
    (happy) top left, 150 × 150, a new `Guide` Image with no raycast;
  - `LessonHud`: the sky, the cream card (9-sliced), the home button's house sprite (its
    "Acasă" label hidden, as the design's home button is an icon).
- **No sticker book yet:** it comes with `rewards`, so the brief's emulator check of the
  sticker book waits for it (the stickers are in their atlas, checked by the tests).
- **Device Simulator not available** (batch mode only, as `project-skeleton`): screen sizes
  on the emulator with `wm size`.

## How to test locally

Commands are for Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb"`, `EMU=emulator-5604`
(your own emulator), `S=<your scratch folder>`, and
`export MSYS_NO_PATHCONV=1 ADB_LOCAL_TRANSPORT_MAX_PORT=5699`. The last variable
lets a restarted adb server find emulators above port 5585. Close your Editor on
this project before any batch-mode run.

### Content slice (the art and its tools)

1. **The art redraws the same.** `node design/art/build.mjs`.
   Expected: "Drew 79 sprites into …\game\Assets\Scolita\Art (Chrome <version>)." (about a
   minute), and `git status` shows no changed PNG. Same code and the same Chrome give the same
   pictures; after a Chrome update a few pixels may change (the version is printed). A
   warning that Chrome still holds a temporary folder is harmless.
2. **One group only:** `node design/art/build.mjs Map/ --out $S/art`.
   Expected: the agent's map sprites in `$S/art/Map/`. Open
   `planet-pink.png` and `planet-pink-face-awake.png`: a pink ball lit from
   the top left, and a face on a transparent square (the parts `galaxy-map` started from).
3. **Cutting a ChatGPT sheet:**
   `node design/art/cut-sheet.mjs "C:/Projects/Scolita/local/art-inbox/unicorn-astronaut-sheet-01.png" $S/cut --grid 4x2 --align bottom --names happy,talk,oh,cheering,thinking,pointing,sleepy,blink`.
   Expected: "Cut 8 parts". Each PNG is a 384 × 384 square with the unicorn
   standing on its bottom edge, no green left round the edges, and the
   thought bubble and the sparkle kept with their unicorn. A sheet on magenta
   takes `--key magenta`. A sheet whose parts nearly touch still gives one
   part per cell.
4. **The choosing page** (done 2026-11-25): it stays in `C:\Projects\Scolita\local\look\choose.html`;
   its generator was removed once the human had chosen.

### Build slice (the import rules and the atlases)

1. **EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: every test passes. `ArtTests` checks that:
   - every art PNG is a sprite;
   - each sprite is in exactly one atlas;
   - the 9-slice borders match `sprite-borders.json`;
   - no prefab shows a missing sprite;
   - the adaptive launcher icon has both layers.
   The log has no `warning CS` and no `error CS`.
2. **The atlases rebuild the same:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.Art.ArtAtlases.RebuildAll -logFile $S/atlases.log`.
   Expected: "Sprite atlases rebuilt: Sky, Map, Effects, Guides, Badges,
   Board, UI, Stickers, Constellations, Avatars." in the log, and no change
   under `game/Assets/Scolita/Art/Atlases` in `git status`.
3. **A new sprite is packed by itself:** copy any PNG to
   `game/Assets/Scolita/Art/Stickers/sticker-test.png` and run step 1.
   Expected: still green (the Stickers atlas packs its folder). Delete the
   file and its `.meta` afterwards.

### Game slice (the art on screen)

1. **PlayMode tests:**
   `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: 3 passed (Boot → Map → Lesson → Map still work with the new art).
2. **Debug APK:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`.
   Expected: exit 0, about 47 MB, no `warning CS`.
3. **On the emulator at the Tab A7's size:**
   - Run `"$ADB" -s $EMU shell wm size 2000x1200`, then `wm density 240`.
   - Install with `"$ADB" -s $EMU install -r game/Builds/Android/Scolita-<version>-debug.apk`.
   - Start it with `"$ADB" -s $EMU shell am start -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`.
   - If "Viewing full screen" shows, tap "Got it".

   Expected:
   - **The Map:**
     - the purple sky with star dust and pale sparkles, not the flat purple;
     - the unicorn astronaut (happy) top left;
     - "Școlița" in white;
     - **Hai!** on a yellow pill with a darker lip under it;
     - the version bottom right.
   - **Tap Hai!:** the Lesson. The same sky, the letters on a cream card with
     round corners, and the home button top left: a cream rounded square with
     a house and no "Acasă" text.
   - **Tap the home button:** the Map.
   - `logcat -s Unity` has only the three development-build socket errors.
4. **At 1280 × 800:** `wm size 1280x800`, `wm density 160`, force-stop and
   start again, then repeat step 3. Expected: the same screens, scaled, with
   nothing cut off. Afterwards run `wm size reset` and `wm density reset`.
5. **The launcher icon:** press Home and swipe up for the app drawer.
   Expected: "Școlița" with the unicorn's helmet on the purple sky, filling
   the launcher's circle, with its horn inside it.

### Content slice 2 (the chosen art and the word pictures)

1. **The ChatGPT parts cut in the same way:** `node design/art/import-drafts.mjs` (the
   human's sheets must be in `C:/Projects/Scolita/local/art-inbox/`). Expected: "Wrote 107
   parts from 19 sheets", and `git status` shows no changed PNG under `game/Assets/Scolita/Art`
   or `content/pictures`. This needs the human's sheets in `local/`, so only an agent on this PC
   can run it. Then `node design/art/build.mjs`. Expected: "Drew 79 sprites", no changed PNG. The two scripts never write the same file.
2. **The import picks up the pictures:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log`.
   Expected: "imported, with 88 warnings": 48 pictures without art and 40 draft shapes. No
   change in `git status`. The EditMode run's freshness test agrees.
3. **The pictures:** open `content/pictures/` and check the 40 pictures each match their word
   in `pictures.csv`'s notes (for example `bed`: a bed with a purple blanket, `pug`: a pug).
   `nose` and `dot` are the two to redraw.
4. **TwoSounds** (the human, in the Editor; or the emulator when a lesson reaches it, planet 1's
   `ro-a`/`en-a` two-sounds step):
   - The Romanian planet is ChatGPT's gold planet, awake, with its ring.
   - The English one is the same planet in blue, its stars still gold.
   - Neither is too big for its place: the planet stands above its tray, its badge and speaker
     on it.
   - Dropping a picture on the wrong one makes it wobble; the right one glows.
5. **On the emulator** (Build slice's step 2, then the Game slice's step 3):
   - the Map shows ChatGPT's starfield, the round unicorn (the stand-in) and "Hai!";
   - the lesson shows the same sky;
   - the app drawer shows ChatGPT's unicorn in the icon's circle.

### Cleanup

- `"$ADB" -s $EMU uninstall com.softshack.scolita` removes the app.
- `game/Builds/` is ignored by git; delete it to free space.

## Completion record

_Finished 2026-11-25 by `look`; released as 0.9.0 (tag `v0.9.0`)._

### What was built

- **The art,** in `game/Assets/Scolita/Art/`, one Sprite Atlas V2 per folder
  (`Art/Atlases/`). `Art/SOURCE.md` says which art is whose.
  - ChatGPT's, chosen by the human (2026-11-25): the sky, the planets in five colours and
    three moods, the rocket's six pictures, the moon, the crescent, the Earth, the comet,
    the cat co-pilot's nine frames, 13 avatars and the cat's disc, 18 stickers, and the
    icon's unicorn.
  - The agents': the guide's stand-in (take C, nine frames), Pisicuța, the badges, the
    writing board, the UI kit, the rainbow path, the stops, the trails, the particles, the
    sparkles, the constellations, the icon's background, and the planets in parts with the first rocket (kept for `galaxy-map`).
- **The word pictures:** 40 in `content/pictures/` (`pictures.csv` → `art`), imported into
  `Assets/Scolita/Content/Pictures/`.
- **The tools:**
  - `design/art/build.mjs` (the agents' drawings, `design/art/draw/*.mjs`, rendered by the
    PC's Chrome);
  - `design/art/cut-sheet.mjs` (cleans an image tool's sheet);
  - `design/art/import-drafts.mjs` with `design/art/drafts.json` (the chosen parts);
  - `Scolita.Editor.Art`: `ArtTextureImporter` (the import rules), `ArtAtlases` (Școlița →
    Rebuild sprite atlases), `SpriteBorderTable`, `ArtPaths`;
  - the tests, `Tests/EditMode/Art/ArtTests.cs`.
- **The prefabs:**
  - `MapScreen` (the sky, the guide, the yellow pill);
  - `LessonHud` (the sky, the cream card, the house home button);
  - `TwoSoundsView` (whole planets, gold and blue);
  - the adaptive launcher icon (`ProjectSettings.asset`).
- **`design/README.md`:** what was chosen, and how the art is made.

### Conventions for later briefs

- **A sprite's file name is its contract.** A redrawn or re-chosen picture replaces the PNG
  under the same name (the same GUID), so prefabs never change for an art swap. New art goes
  in one of the atlas folders and is packed with no other change.
- **ChatGPT art comes only through `drafts.json`** and `import-drafts.mjs`. Raw sheets stay in
  `local/art-inbox/`. A sprite is either drawn (`build.mjs`) or cut (`drafts.json`), never
  both.
- **Planets are whole pictures** (`Planets/planet-<colour>-<mood>`). The ball is about 60% of the
  picture's width, its centre about 5% above the middle. Size a planet from the ball you want.
- **The guide is data:** `Guides/<guide-id>/<frame>` with the nine frames. `guide-unicorn` is
  the stand-in until the human's ChatGPT round. Then its frames are cut in under the same
  names.
- **9-sliced pieces** (`sprite-borders.json`) are shown with "Pixels Per Unit Multiplier"
  1.6.

### How it was tested

- **In batch mode:** EditMode, PlayMode and `BuildAndroidDebug`, with no `warning CS` or
  `error CS`.
- **Emulator** (`emulator-5604`): the map, a lesson and the launcher at 2000 × 1200 and
  1280 × 800.
- **`TwoSoundsView`:** rendered from the prefab in the Editor (batch mode).
- **The badges:** read by shape in greyscale.
- **The fresh-eyes review:** see Progress.
- **Not run:**
  - the Frame Debugger's batch count (batch mode can't);
  - the sticker book (it comes with `rewards`);
  - the Device Simulator.

### Decisions and deviations

These are all in Progress ("Decisions and deviations", "Slice 2"). In short:

- **The guide's full body is route (b),** a ChatGPT round for the human, with take C as the
  stand-in.
- **18 stickers, not 30,** until the human's next round.
- **The badges stay take A.**
- **The planets moved from parts to whole pictures.** The old parts stay in `Map/` until
  `galaxy-map` has switched.
- **No art pack,** so there is no `ThirdParty/` licence: `Art/SOURCE.md` records where the
  art comes from instead.
- **Five `content-letters-1` items** need view code, so they became follow-ups.

### Follow-ups

- **The human: the guide's full body,** the cat sitting, 12 more stickers and the 48 missing
  word pictures. This is step 8 in `local/for-human-next-steps.md`, with the prompts in
  `local/art-prompts/look-guide-body.md`. Then a content chore:
  - add the new sheets to `drafts.json` and run `import-drafts.mjs`;
  - fill the pictures' `art` cells, and run the content import.
- **The badges, for a child who can't read yet.** The idea is a pair she tells apart without
  letters and without being told:
  1. **The guides speak the languages:** the unicorn's face on the Romanian badge and the
     cat's on the English one. She hears the unicorn speak Romanian and the cat English, so
     the faces mean the languages.
  2. **Home and away:** a little house (with a gold door) for Romanian, the language of home,
     and a rocket or a planet for English, the language from far away.
  3. **A sound and a colour of their own:** tapping a badge plays a short sign (a bell for
     Romanian, a whistle for English), with the gold and blue kept. The shapes stay simple:
     a round coin and a ringed planet.

  Choose with her (`feature/badges` or `sound-feel`).
- **`content-letters-1`'s items for `look`** need the activity views' code, so they belong
  with the lesson screens (`activities-letters`, `handwriting`; a `lesson-polish` brief or
  `sound-feel`):
  - **The listen button reads "Ascultă" in English lessons:** the label is fixed text in
    `FirstSoundView`, `LetterHuntView` and `ListenAndPickView`. It should come from the
    step's language ("Listen" in English), through the string tables.
  - **FirstSound shows no word under its picture:** its view should write the word, as
    MeetLetter does.
  - **The bubble breaks „leag-o" at its hyphen:** use a no-break hyphen (U+2011, added to
    `FontCharacterSet` and the fonts rebuilt), or TextMeshPro's `<nobr>` round hyphenated
    words when the line is shown.
  - **At phone size the bubble covers the board's badge:** move the badge inside the
    board's safe corner, or shrink the bubble's width at narrow aspect ratios.
  - **The screen-8 "tipar / de mână" pills:** draw them from the UI kit (`UI/button-cream`
    and `UI/button-yellow` with `UI/icon-pencil`) when the handwriting screen gets its pill
    pair.
- **`galaxy-map`:**
  - Switch to the chosen pictures:
    - `Planets/planet-<gold|pink|green|purple|blue>-<beaming|awake|sleepy>`: whole
      pictures with the face, the ring (awake), the sparkle, the "z z" and the shadow drawn
      in. Size one from its ball: about 60% of the width, its centre 5% above the middle.
    - `Trip/rocket-<standing|flying|flying-fast|landing|friends|asleep>`, `Trip/comet`,
      `Trip/moon-crescent`.
    - `Map/moon` and `Map/earth` are ChatGPT's now, under the same paths and size.
  - Then delete the old parts from `Map/` (bodies, faces, rings, shadow, z z z, `rocket`,
    `rocket-flame-*`) and their drawing code in `world.mjs`. `ArtTests` catches any that are
    still referenced.
  - The rainbow wave is `Path/rainbow-wave`.
  - Check the Frame
  Debugger: the map's art is one atlas plus the sky.
- **`rewards`:**
  - The 18 stickers are `Stickers/sticker-<id>`; their ids are in `drafts.json`.
  - The constellations' star places are in `design/art/draw/constellations.mjs` →
    `constellationStars()`. Export them as data when `rewards` reads them.
  - ChatGPT's guitar, unicorn and rocket constellations (local) are clearer than the agents'
    unicorn. Their star places can be traced in.
- **`feature/guides`:** ChatGPT's six cosmic animal guides and four later planets (local, in
  the ideas) need their nine frames or three moods.
- **`profiles`:** the 14 avatars are `Avatars/avatar-<id>`, with the same ids as design
  screen 11.
- **The guide's catalog asset** (`GuideAsset` / `GuideCatalog`) goes with `feature/guides`.
- **Redraw `nose` and `dot`** (prompted in the same file).
- **`Settings/Renderer2D.asset`** (from the template) refers to a probe-volume debug shader
  that `AssetDatabase` can't resolve (`probeVolumeDebugShader`, GUID `e5c6678e…`).
  - It is harmless in 2D.
  - `ArtTests`' reference check covers only the prefabs, scenes, content and art, so it
    doesn't trip on it.
  - A build chore could clear the field.
- **`Assets/ScolitaImportTest-*`** folders can appear while the content tests run (they
  clean up after themselves). Never commit one; a `.gitignore` line would make sure.
