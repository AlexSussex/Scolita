// Cuts the chosen parts of the human's ChatGPT sheets into the Unity project, as drafts.json lists
// them, and makes the avatar discs that are put together from a cut part. Parts named content/...
// are word pictures: they go to content/pictures/ (then Școlița → Import content brings them in).
//
//   node design/art/import-drafts.mjs [<inbox folder>]   (default C:/Projects/Scolita/local/art-inbox)
//
// The raw sheets never enter the repo; run this again after a sheet is replaced in the inbox.

import { existsSync, readFileSync } from 'node:fs';
import { dirname, join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { cutSheet } from './cut-sheet.mjs';
import { chromeVersion, readFont, renderAll, writeFile } from './lib/render.mjs';

const here = dirname(fileURLToPath(import.meta.url));
const repo = resolve(here, '..', '..');
const artRoot = join(repo, 'game', 'Assets', 'Scolita', 'Art');

/** An avatar disc: a pale circle with a white rim, and the part inside it, cut off at the circle's edge. */
function avatarDisc(png, colour) {
  const image = `data:image/png;base64,${png.toString('base64')}`;
  return (
    '<svg xmlns="http://www.w3.org/2000/svg" width="320" height="320" viewBox="0 0 320 320"><defs>' +
    '<clipPath id="c"><circle cx="160" cy="160" r="146"/></clipPath>' +
    `<radialGradient id="g" cx="38%" cy="32%" r="70%"><stop offset="0" stop-color="#FFFFFF"/><stop offset="0.6" stop-color="${colour}"/><stop offset="1" stop-color="${colour}"/></radialGradient>` +
    '</defs>' +
    '<circle cx="160" cy="160" r="150" fill="#FFFFFF"/>' +
    '<circle cx="160" cy="160" r="146" fill="url(#g)"/>' +
    `<g clip-path="url(#c)"><image href="${image}" x="14" y="34" width="292" height="292"/></g>` +
    '</svg>'
  );
}

/** Keeps every output once: a second entry for the same path is a mistake in drafts.json. */
function put(written, out, bytes, entry) {
  if (!bytes) throw new Error(`drafts.json: ${entry} names a part that was not cut.`);
  if (written.has(out)) throw new Error(`drafts.json: ${out} is written twice (again by ${entry}).`);
  written.set(out, bytes);
}

function main() {
  const inbox = resolve(process.argv[2] ?? 'C:/Projects/Scolita/local/art-inbox');
  const list = JSON.parse(readFileSync(join(here, 'drafts.json'), 'utf8'));
  const written = new Map();
  for (const sheet of list.sheets) {
    const path = join(inbox, sheet.file);
    if (!existsSync(path)) throw new Error(`${sheet.file} is not in ${inbox}.`);
    const names = Object.keys(sheet.parts);
    let parts;
    try {
      parts = cutSheet(path, { ...sheet.cut, grid: sheet.cut.grid ? sheet.cut.grid.split('x').map(Number) : null, names });
    } catch (e) {
      throw new Error(`drafts.json, ${sheet.file}: ${e.message}`);
    }
    if (parts.size !== names.length) throw new Error(`${sheet.file} gave ${parts.size} parts, drafts.json names ${names.length}.`);
    for (const name of names) {
      if (sheet.parts[name]) put(written, sheet.parts[name], parts.get(name), `${sheet.file} → ${name}`);
    }
    for (const [out, name] of Object.entries(sheet.copies ?? {})) put(written, out, parts.get(name), `${sheet.file} copies → ${name}`);
  }
  const discs = list.avatarDiscs.map((disc) => {
    if (!written.has(disc.from)) throw new Error(`drafts.json: the avatar disc ${disc.out} takes ${disc.from}, which no sheet gives.`);
    return { name: disc.out, width: 320, height: 320, svg: avatarDisc(written.get(disc.from), disc.colour) };
  });
  const font = readFont(join(repo, 'game', 'Assets', 'ThirdParty', 'Baloo2', 'Baloo2-Variable.ttf'));
  for (const [name, bytes] of renderAll(discs, font)) put(written, name, bytes, `the avatar disc ${name}`);
  for (const [out, from] of Object.entries(list.reused ?? {})) put(written, out, written.get(from), `reused → ${from}`);
  for (const [out, bytes] of written) writeFile(out.startsWith('content/') ? join(repo, `${out}.png`) : join(artRoot, `${out}.png`), bytes);
  console.log(`Wrote ${written.size} parts from ${list.sheets.length} sheets (Chrome ${chromeVersion()}).`);
}

try {
  main();
} catch (e) {
  console.error(e.message);
  process.exit(1);
}
