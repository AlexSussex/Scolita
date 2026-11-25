# Where Școlița's art comes from

- **Drawn by ChatGPT for the human** (its image tool, 2026-11-24, from the prompts in the
  human's `local/art-prompts/`), chosen by the human and her on 2026-11-25. The pictures belong
  to the human under OpenAI's terms for generated images. They were cut from the human's sheets
  (kept in `local/art-inbox/`, never committed) by `design/art/import-drafts.mjs`, as
  `design/art/drafts.json` lists them:
  - `Sky/sky.png`;
  - `Planets/`: `planet-<gold|pink|green|purple|blue>-<beaming|awake|sleepy>.png` (blue is the
    purple planet recoloured);
  - `Trip/`: `rocket-*.png`, `moon-crescent.png`, `comet.png`;
  - `Map/moon.png`, `Map/earth.png`;
  - `Guides/guide-cat/` (all nine frames; `sleepy-1` repeats `sleepy-0`);
  - `Avatars/`: every avatar but `avatar-kitty.png` (`avatar-cat.png` is ChatGPT's cat set on
    a disc by the script);
  - `Stickers/` (all 18);
  - `Icon/icon-adaptive-foreground.png`;
  - the word pictures named in `content/pictures.csv` (`content/pictures/`).
- **Drawn by agents in code** (`design/art/build.mjs`), everything else: the guide's stand-in
  (`Guides/guide-unicorn/`, until the human's art for ChatGPT's unicorn with a full body comes),
  Pisicuța (`Avatars/avatar-kitty.png`, from design screen 11), the badges, the board, the UI
  kit, the rainbow path, the stops, the trails, the particles, the sparkles, the constellations,
  the icon's background, the rainbow wave (`Path/`), and in `Map/` the planets in parts and
  the first rocket. The parts stay until `galaxy-map` has switched to the chosen pictures.
  `Map/planet-gold.png` and `Map/planet-purple.png` are also `TwoSounds`' glow shape and the
  lesson's picture for a missing picture.
- **Reproducing them:**
  - The agents' drawings come out the same from the same code and the same Chrome
    (`build.mjs` prints its version; these were drawn with Chrome 158.0.8127.36).
  - The ChatGPT parts can be cut again only on the human's PC, because the sheets stay in
    `local/art-inbox/`, never in the repo.
