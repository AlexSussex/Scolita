// The writing board, the UI pieces (buttons with their lip, cards, tiles, the bubble, the
// home button) and the app icon's background (its foreground is ChatGPT's unicorn, drafts.json). Pieces meant to stretch are drawn for 9-slicing: their
// border sizes are in UI_BORDERS, which the Unity importer applies.

import { Drawing, palette, ink, darker, lighter } from '../lib/svg.mjs';

/** 9-slice borders (left, bottom, right, top, in pixels) for the stretchable pieces, by path. */
export const UI_BORDERS = {
  'Board/board-slate': [80, 80, 80, 80],
  'UI/button-yellow': [64, 76, 64, 52],
  'UI/button-cream': [64, 76, 64, 52],
  'UI/card-cream': [72, 72, 72, 72],
  'UI/tile-cream': [56, 64, 56, 48],
  'UI/bubble': [56, 56, 56, 56],
  'UI/panel-translucent': [48, 48, 48, 48],
};

function roundedRect(x, y, w, h, r) {
  return `M ${x + r} ${y} H ${x + w - r} Q ${x + w} ${y} ${x + w} ${y + r} V ${y + h - r} Q ${x + w} ${y + h} ${x + w - r} ${y + h} H ${x + r} Q ${x} ${y + h} ${x} ${y + h - r} V ${y + r} Q ${x} ${y} ${x + r} ${y} Z`;
}

/** The writing board: a slate with an inner rim, rounded (256 × 256, 9-sliced). */
export function boardSlate() {
  const d = new Drawing(256, 256, 'bs');
  return d.render(
    `<path d="${roundedRect(0, 0, 256, 256, 70)}" fill="${palette.slateRim}"/>` +
      `<path d="${roundedRect(8, 8, 240, 240, 62)}" fill="${d.linear([[0, lighter(palette.slate, 0.06)], [1, palette.slate]])}"/>`,
  );
}

/** A dash of the board's dashed middle line (32 × 8). */
export function boardDash() {
  const d = new Drawing(32, 8, 'bd');
  return d.render(`<rect x="2" y="1" width="20" height="6" rx="3" fill="${palette.lines}"/>`);
}

/** A round dot of the dotted guide (24 × 24). */
export function guideDot() {
  const d = new Drawing(24, 24, 'gd');
  return d.render(`<circle cx="12" cy="12" r="8" fill="${palette.lines}" opacity="0.9"/>`);
}

/** The start dot: green with a white rim (the stroke's number is drawn on it in text) (96 × 96). */
export function startDot(next = false) {
  const d = new Drawing(96, 96, `sd${next}`);
  if (next) {
    return d.render(`<circle cx="48" cy="48" r="36" fill="none" stroke="${palette.green}" stroke-width="8"/><circle cx="48" cy="48" r="28" fill="${palette.slate}" opacity="0.6"/>`);
  }
  return d.render(
    `<circle cx="48" cy="48" r="44" fill="${palette.green}" opacity="0.35" filter="${d.blur(4)}"/>` +
      `<circle cx="48" cy="48" r="38" fill="#FFFFFF"/>` +
      `<circle cx="48" cy="48" r="31" fill="${d.shade(palette.green)}"/>`,
  );
}

/** The yellow arrow head that shows the way (96 × 96, pointing right; rotated by code). */
export function arrowHead() {
  const d = new Drawing(96, 96, 'ah');
  return d.render(
    `<path d="M 20 18 L 82 48 L 20 78 Q 30 48 20 18 Z" fill="${d.shade(palette.yellow, { light: 0.5, dark: 0.15 })}" stroke="${darker(palette.yellow, 0.18)}" stroke-width="5" stroke-linejoin="round"/>`,
  );
}

/** A soft round brush for her finger's trail, white (tinted by code) (64 × 64). */
export function trailBrush() {
  const d = new Drawing(64, 64, 'tb');
  return d.render(`<circle cx="32" cy="32" r="24" fill="#FFFFFF" filter="${d.blur(3)}"/><circle cx="32" cy="32" r="20" fill="#FFFFFF"/>`);
}

/** The soft glow where her finger is (128 × 128). */
export function fingerGlow() {
  const d = new Drawing(128, 128, 'fg');
  return d.render(`<circle cx="64" cy="64" r="52" fill="${d.radial([[0, '#FFFFFF', 0.9], [0.5, '#FFFFFF', 0.4], [1, '#FFFFFF', 0]])}"/>`);
}

/** A pill button with its lip (256 × 128; the face 96 tall, the lip under it). */
export function pillButton(colour, lip) {
  const d = new Drawing(256, 128, `pb${colour}`);
  return d.render(
    `<path d="${roundedRect(4, 14, 248, 104, 52)}" fill="#000000" opacity="0.25" filter="${d.blur(5)}"/>` +
      `<path d="${roundedRect(4, 12, 248, 96, 48)}" fill="${lip}"/>` +
      `<path d="${roundedRect(4, 2, 248, 96, 48)}" fill="${d.linear([[0, lighter(colour, 0.25)], [0.5, colour], [1, darker(colour, 0.04)]])}"/>`,
  );
}

/** A cream card, 48-px corners at the reference size (192 × 192, 9-sliced). */
export function card() {
  const d = new Drawing(192, 192, 'card');
  return d.render(
    `<path d="${roundedRect(6, 8, 180, 180, 66)}" fill="#000000" opacity="0.18" filter="${d.blur(4)}"/>` +
      `<path d="${roundedRect(4, 2, 184, 182, 66)}" fill="${palette.cream}"/>`,
  );
}

/** An answer tile: cream with a lip (160 × 160). */
export function tile() {
  const d = new Drawing(160, 160, 'tile');
  return d.render(
    `<path d="${roundedRect(4, 14, 152, 142, 44)}" fill="#D8CDBB"/>` + `<path d="${roundedRect(4, 4, 152, 140, 44)}" fill="${palette.cream}"/>`,
  );
}

/** The speech bubble: cream, rounded, one sharp corner bottom left pointing at the guide (160 × 160). */
export function bubble() {
  const d = new Drawing(160, 160, 'bub');
  const path = 'M 50 4 H 110 Q 156 4 156 50 V 110 Q 156 156 110 156 H 10 Q 4 156 4 150 V 50 Q 4 4 50 4 Z';
  return d.render(`<path d="${path}" fill="#000000" opacity="0.16" transform="translate(2 4)" filter="${d.blur(3)}"/><path d="${path}" fill="${palette.cream}"/>`);
}

/** A translucent panel for the map's bottom bar buttons (128 × 128). */
export function translucentPanel() {
  const d = new Drawing(128, 128, 'tp');
  return d.render(`<path d="${roundedRect(2, 2, 124, 124, 38)}" fill="#FFFFFF" fill-opacity="0.12" stroke="#FFFFFF" stroke-opacity="0.3" stroke-width="3"/>`);
}

/** The home button: a cream rounded square with a house (128 × 128). */
export function homeButton() {
  const d = new Drawing(128, 128, 'home');
  return d.render(
    `<path d="${roundedRect(6, 14, 116, 108, 32)}" fill="#D8CDBB"/>` +
      `<path d="${roundedRect(6, 4, 116, 108, 32)}" fill="${palette.cream}"/>` +
      `<path d="M 38 58 L 64 36 L 90 58 M 46 52 V 82 H 82 V 52" stroke="${ink}" stroke-width="8" fill="none" stroke-linecap="round" stroke-linejoin="round"/>` +
      `<rect x="58" y="64" width="12" height="18" rx="3" fill="${palette.pink}"/>`,
  );
}

/** Line icons in the look's style: rounded, 2.2–2.4 px at 24 (64 × 64, white, tinted by code). */
export function icon(kind) {
  const d = new Drawing(64, 64, `ic${kind}`);
  const s = 'stroke="#FFFFFF" stroke-width="6" fill="none" stroke-linecap="round" stroke-linejoin="round"';
  const shapes = {
    play: `<path d="M 22 14 v 36 a 3 3 0 0 0 4.5 2.6 l 28 -18 a 3 3 0 0 0 0 -5.2 l -28 -18 A 3 3 0 0 0 22 14 Z" fill="#FFFFFF"/>`,
    speaker: `<path d="M 10 26 H 20 L 32 14 V 50 L 20 38 H 10 Z" fill="#FFFFFF"/><path d="M 40 22 Q 48 32 40 42 M 46 14 Q 60 32 46 50" ${s}/>`,
    lock: `<rect x="14" y="30" width="36" height="26" rx="6" ${s}/><path d="M 22 30 V 22 a 10 10 0 0 1 20 0 V 30" ${s}/>`,
    back: `<path d="M 38 14 L 20 32 L 38 50" ${s}/>`,
    replay: `<path d="M 48 32 a 16 16 0 1 1 -5 -11.5" ${s}/><path d="M 46 10 V 22 H 34" ${s}/>`,
    pencil: `<path d="M 14 50 L 16 40 L 42 14 L 50 22 L 24 48 Z" ${s}/><path d="M 36 20 L 44 28" ${s}/>`,
    constellation: `<path d="M 12 46 L 26 22 L 44 30 L 54 12" stroke="#FFFFFF" stroke-width="4" fill="none" stroke-linecap="round" stroke-linejoin="round" opacity="0.7"/><circle cx="12" cy="46" r="6" fill="#FFFFFF"/><circle cx="26" cy="22" r="5" fill="#FFFFFF"/><circle cx="44" cy="30" r="5" fill="#FFFFFF"/><circle cx="54" cy="12" r="6" fill="#FFFFFF"/>`,
    sticker: `<path d="M 14 10 H 50 a 4 4 0 0 1 4 4 V 36 L 36 54 H 14 a 4 4 0 0 1 -4 -4 V 14 a 4 4 0 0 1 4 -4 Z" ${s}/><path d="M 54 36 H 40 a 4 4 0 0 0 -4 4 V 54" ${s}/><circle cx="26" cy="28" r="6" fill="#FFFFFF"/>`,
    trail: `<path d="M 8 46 Q 20 30 32 40 T 56 26" ${s}/><path d="M 8 56 Q 20 42 32 50 T 56 38" ${s} opacity="0.6"/><path d="M 50 6 Q 52 14 60 16 Q 52 18 50 26 Q 48 18 40 16 Q 48 14 50 6 Z" fill="#FFFFFF"/>`,
  };
  return d.render(shapes[kind]);
}

/** The gift box a new sticker pops out of (rewards): a purple box with a yellow ribbon, its lid apart (192 × 160). */
export function giftBox() {
  const d = new Drawing(192, 160, 'gbox');
  return d.render(
    `<ellipse cx="98" cy="150" rx="78" ry="8" fill="#000000" opacity="0.18" filter="${d.blur(3)}"/>` +
      `<path d="${roundedRect(20, 30, 152, 118, 18)}" fill="${d.shade(palette.purple, { light: 0.35 })}"/>` +
      `<rect x="84" y="30" width="24" height="118" fill="${palette.yellow}"/>` +
      `<rect x="20" y="30" width="152" height="14" fill="#000000" opacity="0.12"/>` +
      d.gloss(52, 62, 16, 10, 0.35),
  );
}

/** The gift box's lid with its bow (rewards), lifted when the sticker pops out (208 × 112). */
export function giftLid() {
  const d = new Drawing(208, 112, 'glid');
  return d.render(
    `<path d="M 104 52 C 70 10 44 22 58 44 C 66 56 92 56 104 52 Z" fill="${palette.yellow}" stroke="#E0A800" stroke-width="4"/>` +
      `<path d="M 104 52 C 138 10 164 22 150 44 C 142 56 116 56 104 52 Z" fill="${palette.yellow}" stroke="#E0A800" stroke-width="4"/>` +
      `<path d="${roundedRect(8, 50, 192, 48, 16)}" fill="${d.shade(palette.purple, { light: 0.45 })}"/>` +
      `<rect x="92" y="50" width="24" height="48" fill="${palette.yellow}"/>` +
      d.gloss(44, 64, 18, 7, 0.4),
  );
}

/** The adaptive launcher icon's background: the purple sky with three soft stars (minimum API 26: every launcher uses the adaptive icon). */
function iconSky(d, size) {
  const k = size / 432;
  return (
    `<rect width="${size}" height="${size}" fill="${d.linear([[0, palette.skyTop], [1, palette.skyBottom]])}"/>` +
    `<circle cx="${90 * k}" cy="${92 * k}" r="${5 * k}" fill="#FFFFFF" opacity="0.8"/>` +
    `<circle cx="${350 * k}" cy="${120 * k}" r="${4 * k}" fill="#FFFFFF" opacity="0.7"/>` +
    `<circle cx="${330 * k}" cy="${350 * k}" r="${5 * k}" fill="#FFFFFF" opacity="0.6"/>` +
    `<path transform="translate(${360 * k} ${70 * k}) scale(${k})" d="M 0 -18 Q 4 -4 18 0 Q 4 4 0 18 Q -4 4 -18 0 Q -4 -4 0 -18 Z" fill="${palette.sparkle}"/>`
  );
}

/** The adaptive icon's background layer (432 × 432). */
export function iconAdaptiveBackground() {
  const d = new Drawing(432, 432, 'iab');
  return d.render(iconSky(d, 432));
}

/** Every board, UI and icon sprite. */
export function sprites() {
  return [
    { path: 'Board/board-slate', width: 256, height: 256, svg: boardSlate() },
    { path: 'Board/board-dash', width: 32, height: 8, svg: boardDash() },
    { path: 'Board/guide-dot', width: 24, height: 24, svg: guideDot() },
    { path: 'Board/start-dot', width: 96, height: 96, svg: startDot(false) },
    { path: 'Board/start-dot-next', width: 96, height: 96, svg: startDot(true) },
    { path: 'Board/arrow-head', width: 96, height: 96, svg: arrowHead() },
    { path: 'Board/trail-brush', width: 64, height: 64, svg: trailBrush() },
    { path: 'Board/finger-glow', width: 128, height: 128, svg: fingerGlow() },
    { path: 'UI/button-yellow', width: 256, height: 128, svg: pillButton(palette.yellow, '#D9A800') },
    { path: 'UI/button-cream', width: 256, height: 128, svg: pillButton(palette.cream, '#D8CDBB') },
    { path: 'UI/card-cream', width: 192, height: 192, svg: card() },
    { path: 'UI/tile-cream', width: 160, height: 160, svg: tile() },
    { path: 'UI/bubble', width: 160, height: 160, svg: bubble() },
    { path: 'UI/panel-translucent', width: 128, height: 128, svg: translucentPanel() },
    { path: 'UI/home-button', width: 128, height: 128, svg: homeButton() },
    { path: 'UI/gift-box', width: 192, height: 160, svg: giftBox() },
    { path: 'UI/gift-lid', width: 208, height: 112, svg: giftLid() },
    ...['play', 'speaker', 'lock', 'back', 'replay', 'pencil', 'constellation', 'sticker', 'trail'].map((k) => ({ path: `UI/icon-${k}`, width: 64, height: 64, svg: icon(k) })),
    { path: 'Icon/icon-adaptive-background', width: 432, height: 432, svg: iconAdaptiveBackground() },
  ];
}
