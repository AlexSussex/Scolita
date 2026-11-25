// The constellations she lights with her stars (rewards): a faint figure per planet and the lit
// and empty star. The stickers are ChatGPT's (drafts.json, chosen 2026-11-25).

import { Drawing, palette, starPath } from '../lib/svg.mjs';

const constellations = {
  unicorn: {
    stars: [[470, 56], [418, 150], [356, 146], [282, 182], [196, 250], [190, 336], [270, 360], [344, 304], [380, 214]],
    lines: [[0, 1], [1, 2], [2, 3], [3, 4], [4, 5], [5, 6], [6, 7], [7, 8], [8, 1]],
  },
  rocket: {
    stars: [[320, 50], [262, 168], [378, 168], [262, 318], [378, 318], [196, 404], [444, 404], [320, 432]],
    lines: [[0, 1], [0, 2], [1, 3], [2, 4], [3, 5], [4, 6], [3, 7], [7, 4]],
  },
  cat: {
    stars: [[226, 78], [270, 142], [350, 142], [394, 78], [384, 224], [424, 400], [216, 400], [236, 224], [530, 320]],
    lines: [[0, 1], [1, 2], [2, 3], [3, 4], [4, 5], [5, 6], [6, 7], [7, 0], [5, 8]],
  },
};

/** The star places of each constellation, in its 640 × 480 picture (for lighting them later). */
export function constellationStars() {
  return Object.fromEntries(Object.entries(constellations).map(([id, c]) => [id, c.stars.map((s) => [...s])]));
}

function emptyRing(x, y, r = 16) {
  return `<circle cx="${x}" cy="${y}" r="${r}" fill="none" stroke="${palette.sparkle}" stroke-width="3.5" stroke-dasharray="5 6" stroke-linecap="round" opacity="0.85"/>`;
}

/** A constellation's faint figure: lilac lines joining its star places, each a dotted ring (640 × 480). */
function constellation(id) {
  const { stars, lines } = constellations[id];
  const d = new Drawing(640, 480, `cn${id}`);
  const lineMarkup = lines
    .map(([a, b]) => `<path d="M ${stars[a][0]} ${stars[a][1]} L ${stars[b][0]} ${stars[b][1]}" stroke="#D8CCFF" stroke-width="4" stroke-linecap="round" opacity="0.35"/>`)
    .join('');
  return d.render(lineMarkup + stars.map(([x, y]) => emptyRing(x, y)).join(''));
}

/** A lit star: glowing gold with a soft halo (96 × 96). */
function starLit() {
  const d = new Drawing(96, 96, 'sl');
  return d.render(
    `<circle cx="48" cy="48" r="34" fill="${palette.yellow}" opacity="0.45" filter="${d.blur(9)}"/>` +
      `<path d="${starPath(48, 50, 34, 16)}" fill="${d.shade(palette.yellow, { light: 0.65 })}" stroke="#E0A800" stroke-width="4" stroke-linejoin="round"/>` +
      d.gloss(40, 38, 7, 4, 0.7),
  );
}

/** An empty star place: a dotted ring (96 × 96). */
function starEmpty() {
  const d = new Drawing(96, 96, 'se');
  return d.render(emptyRing(48, 48, 30));
}

/** Every constellation sprite, by its path under Art/. */
export function sprites() {
  const list = [];
  for (const id of Object.keys(constellations)) {
    list.push({ path: `Constellations/constellation-${id}`, width: 640, height: 480, svg: constellation(id) });
  }
  list.push({ path: 'Constellations/star-lit', width: 96, height: 96, svg: starLit() });
  list.push({ path: 'Constellations/star-empty', width: 96, height: 96, svg: starEmpty() });
  return list;
}
