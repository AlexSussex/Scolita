# The look

**Bright 2D with soft shading, in a purple space world** (agreed by the
human, 2026-11-23: "I like the design very much"). It is built from her
favourite things: **purple** first, then rainbows, unicorns, the moon,
planet Earth and space, stars, space ships, animals, music, drawing and
painting, cycling.

- **The design canvas** (the source of truth for the screens):
  https://claude.ai/artifact/RVfBkheGcrfB824aq4cEEf
- **Each screen, described for building it:** [`screens.md`](screens.md).
- **The canvas's source files** (one `.dc.html` per screen and
  `canvas.json`, for exact positions, sizes and colours):
  [`canvas/`](canvas/). They render on the canvas, not on their own; read
  them as a spec.

`feature/look` turns these designs into the app's final art (sprites,
atlases, the mascot's moods) and may refine them with her; it never
changes the rules below without the human.

## Purple is hers

Purple is her favourite colour, so it is the app's signature colour, not
just one colour among many:

- **The sky is purple:** a deep violet at the top (`#2B1B5A`) to a royal
  purple at the bottom (`#4B2C8F`). Every child-facing screen sits on it.
- **The rainbow path ends in purple** (`#9B6CFF`, its last and widest-read
  band), and the rocket's fins are purple.
- **The fourth planet is purple** (lilac `#D8CCFF` → `#9B6CFF` →
  `#5B33B5`), the one she sees waiting at the edge of the map.
- **The parent area is lilac and violet:** a lilac page (`#F6F2FF`), white
  cards, violet controls (`#5B33B5`) and the dark ink `#2A2140`.
- **The app icon** (from `look`): the unicorn astronaut's helmet on the
  purple sky.
- **Purple is never a language colour.** Romanian is gold, English is
  blue; purple belongs to the app, so it never makes her think "this is
  one language".

## The world

- **A galaxy at dusk, never black:** the purple sky with soft white star
  dust (dots of 1.5–2.5 px at 40–80%) and a few four-pointed sparkles
  (`#FFF3B0`), so bright colours glow and it never feels scary.
- **A rainbow path** joins the planets on the map: six bands, pink
  `#FF5C8A`, orange `#FF9F43`, yellow `#FFD93D`, green `#4CD97B`, blue
  `#3BA7F0`, purple `#9B6CFF`, each 10 px, drawn as one smooth wave; the
  rocket travels along it.
- **Each planet is a character:** round, lit from the top left (a radial
  shade from a pale highlight at 35% across and 28% down, through the
  planet's colour, to a darker edge), with a soft oval shadow under it and
  a face:
  - **beaming** (finished): eyes closed in happy arcs, a wide smile, pink
    cheeks, a sparkle beside it;
  - **awake** (the current one): open eyes with highlights, a smile, a
    ring around it, its letters standing below it on tiles;
  - **sleepy** (ahead): 55% opacity, closed eyes, a small mouth, "z z z"
    floating up.
- **Planet colours in map order:** gold (`#F5B700`), pink (`#FF5C8A`),
  green (`#4CD97B`), purple (`#9B6CFF`), then the cycle repeats with new
  faces and rings.
- **The moon and the Earth** appear on the trips between planets; the
  Earth is "home" (the sticker scene's first backdrop).

## The mascot (decision D5, agreed 2026-11-24)

- **A unicorn astronaut:** a white unicorn face in a round, glassy helmet
  (pale blue `#DDF1FF` at 35% with a white rim), a golden striped horn
  through the top (`#FFD93D`, stripes `#E0A800`), two little ears, a
  rainbow mane on the left (pink, yellow and blue strands, 6 px), a pink
  snout with two nostrils, dark eyes with white highlights, pink cheeks,
  and a grey-blue collar (`#C9D3E8`) at the bottom of the helmet.
- **It is named by her.** It speaks every instruction from a speech bubble
  (cream `#FFF8EC`, ink `#2A2140`, 22–26 px bold, rounded with one sharp
  corner pointing at the mascot); tapping it repeats the last line.
- **Moods:** happy (open eyes, a slow float), cheering (eyes closed in
  arcs, open smile, a sparkle: the end of a lesson), thinking (a hint: a
  paw on the chin, a bubble), sleepy (a session's end: "Racheta se
  odihnește").
- **The co-pilot:** her purple cat with black spots and a pink scarf (her
  request for SeliSnake), riding along beside the unicorn: purple, so it
  belongs.
- **More guides later:** the guide is data (its poses and moods as art), so
  more characters for her to choose from are art only (`feature/guides`,
  after the MVP).
- **Chosen with her, 2026-11-25:**
  - **The guide:** ChatGPT's unicorn astronaut (the "take D" sheet), with a full
    astronaut body like the agent's take B.
  - **The co-pilot:** ChatGPT's purple cat in a glass helmet, sitting beside the unicorn.
  - **The same design on every screen.**
  - **Until the full-body art arrives** (a ChatGPT round for the human, in
    `local/art-prompts/look-guide-body.md`), the app shows the agent's round unicorn
    (take C) as a stand-in, beside ChatGPT's cat.

## The language badges (decision D6, agreed 2026-11-24)

Every letter, word and voice line shows its language by a colour **and** a
shape, never by colour alone:

| Language | Badge | Colours | Where |
| --- | --- | --- | --- |
| Romanian | A five-pointed **star** with "RO" | Gold `#F5B700`, outline `#7A4D00`, text `#3A2600` | Top right corner of every letter card, word card and picture card; beside the mascot's bubble; the Romanian side of `TwoSounds` |
| English | A **ringed planet** with "EN" | Blue `#3BA7F0`, outline and ring `#114A78`, a light band `#BFE3FF`, text white | The same places |

Badges are 48–64 px on cards, 30–40 px in bubbles and the parent grid.
Text on them is decoration: a child who can't read still tells a star from
a planet. No flags (D6).

**For now (2026-11-25):** none of the takes was loved, and the badges aren't important yet. The
agent's take A, the clearest in grey, stays. What is wanted later is a pair she can tell apart
without reading letters and without being told; picture-based ideas are in the follow-ups of
`tasks/done/look.md`.

## Rules of the look

- **Light from the top left, always** (as SeliSnake): highlights up and to
  the left, soft shadows down and to the right.
- **Round and soft:** no sharp corners on anything she touches: cards 32–48
  px radius, tiles 14–32 px, big buttons fully rounded (pill).
- **Buttons look pressable:** a solid 6–10 px "lip" shadow under them in a
  darker shade of their colour (`#D9A800` under yellow, `#D8CDBB` under
  cream, `#1E6FA8` under blue).
- **One big yellow action per screen:** the main thing to tap ("Hai!",
  "Mai departe") is yellow `#FFD93D` with dark ink, 96 px tall, with an
  icon; everything else is quieter (cream, outlined, or translucent white
  on the sky).
- **Letters to learn are big and clean:** the reading typeface (Andika,
  school print, single-storey ɑ and ɡ), dark ink `#2A2140` on cream
  `#FFF8EC`, never on a busy background, never with an effect on the
  letter while she reads it. A letter's first sound in a word may be
  highlighted in pink `#D9408A`.
- **Answer tiles** are cream squares with a 32 px radius and a lip shadow,
  at least 96 px (130 px in `BuildWord`); the hinted one glows yellow
  (an 8 px `#FFD93D` ring and a soft yellow halo).
- **Effects celebrate, they don't distract:** sparkles and stars after an
  answer, never while she listens; at most one effect at a time.
- **Handwritten letters** ("litere de mână") are drawn from their stroke
  data as an even, rounded pen line (14 px at the reference size): cream
  ink on the writing board, dark ink `#2A2140` on cream cards. A **tipar /
  de mână** pill pair shows which form she is writing; "de mână" has a
  pencil icon.
- **The writing board** is a slate (`#24324A` with a `#3A4C6C` inner rim)
  with three writing lines in `#E8EEF8`: a thin top line, a dashed middle
  line, a thicker baseline; the dotted guide is round dots; her trail is a
  26 px rainbow stroke (purple → blue → green → yellow → orange → pink)
  with stardust sparkles; the start dot is green `#4CD97B` with a white rim
  and its stroke number; the arrow is yellow.
- **Progress through a lesson** is a row of 22 px dots at the top centre:
  green done, yellow current, translucent white to come.
- **The home button** is always top left: an 80 px cream rounded square
  with a house icon.
- **The safe area:** the home button, the mascot and the badges stay clear
  of the display's cut-outs and edges (at least 24 px).
- **Icons** are simple rounded line icons (2.2–2.4 px strokes) or flat
  shapes; never emoji.
- **Text:** Baloo 2 (rounded, friendly) for the app's own words, covering
  ă â î ș ț; 22–30 px on child screens (bold), 18 px and up in the parent
  area.

## Palette

| Thing | Colour |
| --- | --- |
| Sky, top / bottom | `#2B1B5A` / `#4B2C8F` (purple) |
| Star dust / sparkles | `#FFFFFF` at 40–80% / `#FFF3B0` |
| Rainbow path | `#FF5C8A`, `#FF9F43`, `#FFD93D`, `#4CD97B`, `#3BA7F0`, `#9B6CFF` |
| Planets | gold `#F5B700`, pink `#FF5C8A`, green `#4CD97B`, purple `#9B6CFF` |
| Card / tile / bubble | `#FFF8EC`, ink `#2A2140` |
| Main action | `#FFD93D`, lip `#D9A800`, ink `#2A2140` |
| Right answer | `#4CD97B` |
| Hint glow | `#FFD93D` |
| First-sound highlight | `#D9408A` |
| Writing board | slate `#24324A`, rim `#3A4C6C`, lines `#E8EEF8` |
| Romanian | gold `#F5B700`, outline `#7A4D00` (star) |
| English | blue `#3BA7F0`, outline `#114A78` (ringed planet) |
| Parent area | page `#F6F2FF`, cards `#FFFFFF`, controls `#5B33B5`, muted text `#5E5478`, borders `#C9BFE3`, the reset button `#8E1A45` on white |

All text on its background at WCAG AA (4.5:1) or better; the badges are
told apart by shape first.

## The art (`feature/look`, 2026-11-24 and 2026-11-25)

The app's art comes from two sources, listed one by one in `game/Assets/Scolita/Art/SOURCE.md`:

- **ChatGPT's, chosen by the human and her (2026-11-25).** The human made sprite sheets with
  ChatGPT from the prompts in `local/art-prompts/`. Chosen from them: the sky, the planets
  and their faces, the rocket, the moon and the Earth, the cat co-pilot, 13 avatars, the 18
  stickers, the app icon's unicorn and the word pictures. The raw sheets stay in
  `local/art-inbox/`.
  - `design/art/drafts.json` says which part of which sheet becomes which sprite.
  - `node design/art/import-drafts.mjs` cuts them all into `game/Assets/Scolita/Art/`, and the
    word pictures into `content/pictures/`. `cut-sheet.mjs` does the cutting: it removes the flat
    green (or magenta) with a soft edge and no green fringe, finds the parts, and scales every
    part of a sheet alike.
  - When a sheet is replaced in the inbox, run the script again.
- **Drawn by agents in code** (decision D7), for everything else. That is the guide's stand-in,
  Pisicuța, the badges, the writing board, the UI kit, the rainbow path, the stops, the trails,
  the particles, the constellations and the icon's background.
  - The sources are `design/art/draw/*.mjs`, with shared helpers (the top-left light, gloss,
    soft shadows, eyes) in `design/art/lib/svg.mjs`.
  - `node design/art/build.mjs` (or with a folder prefix: `node design/art/build.mjs Map/`)
    renders them with the PC's Chrome into `game/Assets/Scolita/Art/<Folder>/<name>.png`. It
    also writes `sprite-borders.json` (the 9-slice borders).
  - The same code and the same Chrome give the same PNGs (the version is printed). It needs
    Node 18+ and Chrome or Edge
    (`SCOLITA_CHROME` names another).

**In Unity:**

- **Import rules:** every PNG under `Assets/Scolita/Art/` is imported by
  `Scolita.Editor.Art.ArtTextureImporter` as a full-rect sprite, with no mipmaps and its 9-slice
  border.
- **Atlases:** each folder is packed by its own sprite atlas
  (`Art/Atlases/<Folder>.spriteatlasv2`, built by Școlița → Rebuild sprite atlases): Sky, Map,
  Planets, Trip, Path, Effects, Guides, Badges, Board, UI, Stickers, Constellations, Avatars.
  Each fits one 2048 page (`ArtTests` checks it), so a folder that grows too big is split. A new sprite in one of
  these folders is packed with no other change.
- **The launcher icon:** `Art/Icon/` holds the adaptive icon's two layers (minimum API 26, so
  every launcher uses the adaptive icon).
- **Sizes:** sprites are about 1.6× the 1280 × 800 reference size. A 9-sliced piece is shown with
  the Image's "Pixels Per Unit Multiplier" at 1.6.
- **The guide is data:** `Art/Guides/<guide-id>/<frame>.png`, with the same nine frames for
  every guide: `happy-idle`, `happy-blink`, `happy-talk`, `cheering-0`, `cheering-1`,
  `thinking-idle`, `thinking-talk`, `sleepy-0`, `sleepy-1`. `guide-unicorn` is the guide (its
  name is hers to choose) and `guide-cat` the co-pilot. A new guide is a new folder of the same
  frames (`feature/guides`).
- **The planets are whole pictures,** one per colour and mood:
  `Planets/planet-<gold|pink|green|purple|blue>-<beaming|awake|sleepy>`.
  - The face, the ring (awake), the sparkle (beaming), the "z z" (sleepy) and the soft shadow
    are drawn in.
  - The ball is about 60% of the picture's width, its centre about 5% above the middle (so a
    view sizes the picture from the ball it wants).
  - A sleepy planet ahead on the map is also drawn at 55% opacity.
  - `blue` is the purple planet recoloured, for English in `TwoSounds`.
- **The rocket's six pictures:** `Trip/rocket-<standing|flying|flying-fast|landing|friends|asleep>`,
  with `Trip/comet` and `Trip/moon-crescent`; the moon and the Earth are `Map/moon` and `Map/earth`.
- **The old parts** (`Map/planet-<colour>`, `-face-<mood>`, `-ring-back/front`, `planet-shadow`,
  `planet-sleepy-z`, `rocket`, `rocket-flame-*`) stay until `galaxy-map` has switched, then go.
- **The rainbow wave** is `Path/rainbow-wave` (its own atlas, as it is a third of a page).
  "Friends" has the unicorn and the cat in its window.
- **The rocket's trails** (`trail-*`), the stops and the rainbow path stay the agent's.

## The screens

Eleven screens are designed so far, and three more are described as built (details in [`screens.md`](screens.md)):

1. The galaxy map (home).
2. Meet a letter (`MeetLetter`).
3. Trace a letter (`TraceLetter`).
4. One letter, two sounds (`TwoSounds`).
5. Build a word (`BuildWord`).
6. The end of a lesson.
7. The parent area.
8. Write by hand, joined (`TraceLetter` by hand, `TraceWord`).
9. Who's playing? (profiles).
10. Profiles in the parent area (children and the tester).
11. The 14 profile avatars (and the `Avatar` component that draws them).
12. The sky book (`rewards`, as built).
13. The sticker book and scene (`rewards`, as built).
14. The trail picker (`rewards`, as built).

Still to design (in their briefs, in this look): `ListenAndPick`,
`LetterHunt`, `FirstSound`, `Syllables`, `ReadAndMatch`, `WordTwins`, the
trip between planets, the parent gate, the session's end, the tester overlay.
