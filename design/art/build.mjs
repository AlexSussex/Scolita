// Draws Școlița's art and writes it as PNGs into the Unity project.
//
//   node design/art/build.mjs                 every sprite, into game/Assets/Scolita/Art/
//   node design/art/build.mjs Map/ Guides/    only sprites whose path starts with one of these
//   node design/art/build.mjs --out <folder>  write somewhere else (for candidates and reviews)
//   node design/art/choose.mjs <folder>       the choosing page (keep it in local/)
//
// Needs Node 18+ and Chrome (or Edge) on the PC; SCOLITA_CHROME overrides where Chrome is.
// The output is deterministic: the same code gives the same PNGs.

import { dirname, join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { readFont, renderAll, writeFile } from './lib/render.mjs';
import * as world from './draw/world.mjs';
import * as guides from './draw/guides.mjs';
import * as badges from './draw/badges.mjs';
import * as avatars from './draw/avatars.mjs';
import * as ui from './draw/ui.mjs';
import * as stickers from './draw/stickers.mjs';

const here = dirname(fileURLToPath(import.meta.url));
const repo = resolve(here, '..', '..');
const fontPath = join(repo, 'game', 'Assets', 'ThirdParty', 'Baloo2', 'Baloo2-Variable.ttf');

const modules = [world, guides, badges, avatars, ui, stickers];

/** The 9-slice borders the Unity importer applies (Scolita.Editor.Art.ArtTextureImporter), as JSON. */
function spriteBorders() {
  const entries = Object.entries(ui.UI_BORDERS).map(([path, [left, bottom, right, top]]) => ({ path, left, bottom, right, top }));
  return JSON.stringify({ borders: entries }, null, 2) + '\n';
}

function parseArgs(argv) {
  const args = { out: join(repo, 'game', 'Assets', 'Scolita', 'Art'), prefixes: [] };
  for (let i = 0; i < argv.length; i += 1) {
    if (argv[i] === '--out') {
      args.out = resolve(argv[i + 1]);
      i += 1;
    } else {
      args.prefixes.push(argv[i]);
    }
  }
  return args;
}

async function main() {
  const args = parseArgs(process.argv.slice(2));
  const list = modules.flatMap((m) => m.sprites());
  const out = args.out;
  const seen = new Set();
  for (const s of list) {
    if (seen.has(s.path)) throw new Error(`Two sprites share the path ${s.path}.`);
    seen.add(s.path);
  }
  const chosen = args.prefixes.length ? list.filter((s) => args.prefixes.some((p) => s.path.startsWith(p))) : list;
  if (!chosen.length) throw new Error(`No sprite matches ${args.prefixes.join(', ')}.`);
  const font = readFont(fontPath);
  const pngs = renderAll(
    chosen.map((s) => ({ name: s.path, width: s.width, height: s.height, svg: s.svg })),
    font,
  );
  for (const s of chosen) writeFile(join(out, `${s.path}.png`), pngs.get(s.path));
  writeFile(join(out, 'sprite-borders.json'), Buffer.from(spriteBorders(), 'utf8'));
  console.log(`Drew ${chosen.length} sprites into ${out}.`);
}

main().catch((e) => {
  console.error(e.message);
  process.exit(1);
});
