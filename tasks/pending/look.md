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
    places are in `design/art/draw/stickers.mjs` → `constellationStars()`).
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
  `design/art/draw/stickers.mjs`; `rewards` should read them from data (for
  example, exported to a JSON by `build.mjs`).
- **The unicorn constellation** is the weakest drawing. ChatGPT's unicorn
  constellation (on the choosing page) is clearer, and its star places could be
  traced into the agent's.
- **Palette matching of ChatGPT parts:** `cut-sheet.mjs` keys, despills, trims and
  scales. It does not recolour. If ChatGPT's takes are chosen next to the
  agent's, slice 2 brings the two styles closer (by redrawing the agent's parts,
  or by tinting).
- **The Frame Debugger batch check** in the Editor (see "Test results").

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
   Expected: "Drew 135 sprites into …\game\Assets\Scolita\Art." (about a
   minute), and `git status` shows no changed PNG. Same code, same pictures. A
   warning that Chrome still holds a temporary folder is harmless.
2. **One group only:** `node design/art/build.mjs Map/ --out $S/art`.
   Expected: the 39 map sprites in `$S/art/Map/`. Open
   `planet-pink.png` and `planet-pink-face-awake.png`: a pink ball lit from
   the top left, and a face on a transparent square.
3. **Cutting a ChatGPT sheet:**
   `node design/art/cut-sheet.mjs "C:/Projects/Scolita/local/art-inbox/unicorn-astronaut-sheet-01.png" $S/cut --grid 4x2 --align bottom --names happy,talk,oh,cheering,thinking,pointing,sleepy,blink`.
   Expected: "Cut 8 parts". Each PNG is a 384 × 384 square with the unicorn
   standing on its bottom edge, no green left round the edges, and the
   thought bubble and the sparkle kept with their unicorn. A sheet on magenta
   takes `--key magenta`. A sheet whose parts nearly touch still gives one
   part per cell.
4. **The choosing page:**
   `node design/art/choose.mjs C:/Projects/Scolita/local/look --inbox C:/Projects/Scolita/local/look/inbox.json`,
   then open `C:\Projects\Scolita\local\look\choose.html`. Expected:
   - every take side by side: the guide A–C (the agent's) and D (ChatGPT's),
     the cat, the badges A–C (with grey versions), the planets, the avatars,
     the stickers, the rocket, the moon and the Earth, the sky and the icon;
   - the word pictures, and "Ideas for later", each with notes on what can't
     be used as it is.

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

### Cleanup

- `"$ADB" -s $EMU uninstall com.softshack.scolita` removes the app.
- `game/Builds/` is ignored by git; delete it to free space.
