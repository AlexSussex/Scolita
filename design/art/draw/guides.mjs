// The guide: the unicorn astronaut (decision D5), take C (the round one), the stand-in until
// the chosen art arrives (ChatGPT's unicorn with a full astronaut body). Every guide has four
// moods (happy, cheering, thinking, sleepy), a few frames each, so a guide is art only.

import { Drawing, palette, ink, lighter, darker, sparkle, cheek, label, roundStar } from '../lib/svg.mjs';

/** The frames every guide has, in the order the catalog lists them. */
export const frames = [
  { id: 'happy-idle', mood: 'happy', eyes: 'open', mouth: 'smile', arms: 'down' },
  { id: 'happy-blink', mood: 'happy', eyes: 'blink', mouth: 'smile', arms: 'down' },
  { id: 'happy-talk', mood: 'happy', eyes: 'open', mouth: 'talk', arms: 'wave' },
  { id: 'cheering-0', mood: 'cheering', eyes: 'happy', mouth: 'open-smile', arms: 'up', sparkles: 0 },
  { id: 'cheering-1', mood: 'cheering', eyes: 'happy', mouth: 'open-smile', arms: 'up-high', sparkles: 1 },
  { id: 'thinking-idle', mood: 'thinking', eyes: 'up', mouth: 'small', arms: 'chin', bubble: true },
  { id: 'thinking-talk', mood: 'thinking', eyes: 'up', mouth: 'talk', arms: 'chin', bubble: true },
  { id: 'sleepy-0', mood: 'sleepy', eyes: 'sleepy', mouth: 'small', arms: 'down', z: 0 },
  { id: 'sleepy-1', mood: 'sleepy', eyes: 'sleepy', mouth: 'yawn', arms: 'down', z: 1 },
];

const face = '#FFF8F2';
const snoutPink = '#FFE3EC';
const suitWhite = '#F4F1FB';
const suitLilac = '#C9B6FF';
const collar = '#C9D3E8';

function eyes(f, lx, rx, y, r) {
  const highlight = (x, yy) =>
    `<circle cx="${x + r * 0.38}" cy="${yy - r * 0.38}" r="${r * 0.4}" fill="#FFFFFF"/>` +
    `<circle cx="${x - r * 0.3}" cy="${yy + r * 0.42}" r="${r * 0.16}" fill="#FFFFFF" opacity="0.8"/>`;
  if (f.eyes === 'open') {
    return [lx, rx].map((x) => `<ellipse cx="${x}" cy="${y}" rx="${r}" ry="${r * 1.12}" fill="${ink}"/>` + highlight(x, y)).join('');
  }
  if (f.eyes === 'up') {
    return [lx, rx]
      .map((x) => `<ellipse cx="${x + r * 0.25}" cy="${y - r * 0.3}" rx="${r}" ry="${r * 1.12}" fill="${ink}"/>` + highlight(x + r * 0.45, y - r * 0.55))
      .join('');
  }
  if (f.eyes === 'blink') {
    return [lx, rx].map((x) => `<path d="M ${x - r} ${y + r * 0.2} Q ${x} ${y + r * 0.9} ${x + r} ${y + r * 0.2}" stroke="${ink}" stroke-width="${r * 0.5}" fill="none" stroke-linecap="round"/>`).join('');
  }
  if (f.eyes === 'happy') {
    return [lx, rx].map((x) => `<path d="M ${x - r * 1.1} ${y + r * 0.4} Q ${x} ${y - r * 1.1} ${x + r * 1.1} ${y + r * 0.4}" stroke="${ink}" stroke-width="${r * 0.5}" fill="none" stroke-linecap="round"/>`).join('');
  }
  return [lx, rx].map((x) => `<path d="M ${x - r} ${y} Q ${x} ${y + r * 0.8} ${x + r} ${y}" stroke="${ink}" stroke-width="${r * 0.45}" fill="none" stroke-linecap="round"/>`).join('');
}

function mouth(f, x, y, w) {
  if (f.mouth === 'smile') {
    return `<path d="M ${x - w} ${y} Q ${x} ${y + w * 0.9} ${x + w} ${y}" stroke="${ink}" stroke-width="${w * 0.32}" fill="none" stroke-linecap="round"/>`;
  }
  if (f.mouth === 'talk') {
    return (
      `<ellipse cx="${x}" cy="${y + w * 0.3}" rx="${w * 0.62}" ry="${w * 0.55}" fill="#7A2848"/>` +
      `<ellipse cx="${x}" cy="${y + w * 0.6}" rx="${w * 0.38}" ry="${w * 0.22}" fill="#FF8FB0"/>`
    );
  }
  if (f.mouth === 'open-smile') {
    return (
      `<path d="M ${x - w * 1.1} ${y - w * 0.1} Q ${x} ${y + w * 1.6} ${x + w * 1.1} ${y - w * 0.1} Z" fill="#7A2848" stroke="${ink}" stroke-width="${w * 0.2}" stroke-linejoin="round"/>` +
      `<ellipse cx="${x}" cy="${y + w * 0.75}" rx="${w * 0.5}" ry="${w * 0.25}" fill="#FF8FB0"/>`
    );
  }
  if (f.mouth === 'yawn') {
    return `<ellipse cx="${x}" cy="${y + w * 0.35}" rx="${w * 0.45}" ry="${w * 0.6}" fill="#7A2848"/>`;
  }
  return `<path d="M ${x - w * 0.45} ${y + w * 0.1} Q ${x} ${y + w * 0.5} ${x + w * 0.45} ${y + w * 0.1}" stroke="${ink}" stroke-width="${w * 0.3}" fill="none" stroke-linecap="round"/>`;
}

/** The unicorn's head, drawn in the design canvas's 120 × 120 space (helmet glass not included). */
function unicornHead(d, f) {
  const strand = (path, colour, width) =>
    `<path d="${path}" stroke="${darker(colour, 0.12)}" stroke-width="${width + 1.4}" fill="none" stroke-linecap="round"/>` +
    `<path d="${path}" stroke="${colour}" stroke-width="${width}" fill="none" stroke-linecap="round"/>` +
    `<path d="${path}" stroke="${lighter(colour, 0.5)}" stroke-width="${width * 0.3}" fill="none" stroke-linecap="round" opacity="0.7" transform="translate(-1 -1)"/>`;
  return (
    `<path d="M 35 42 L 39 21 L 51 35 Z" fill="${d.shade(face, { light: 0.4, dark: 0.12 })}" stroke="${darker(face, 0.12)}" stroke-width="1.2" stroke-linejoin="round"/>` +
    `<path d="M 85 42 L 81 21 L 69 35 Z" fill="${d.shade(face, { light: 0.4, dark: 0.12 })}" stroke="${darker(face, 0.12)}" stroke-width="1.2" stroke-linejoin="round"/>` +
    `<path d="M 39 38 L 41 27 L 47 35 Z" fill="#FFB3D1"/><path d="M 81 38 L 79 27 L 73 35 Z" fill="#FFB3D1"/>` +
    strand('M 33 52 C 21 62, 23 80, 33 92', palette.pink, 6.5) +
    strand('M 39 44 C 25 56, 27 78, 37 96', palette.yellow, 6.5) +
    strand('M 46 38 C 31 50, 33 74, 43 98', palette.blue, 6.5) +
    `<circle cx="60" cy="62" r="30" fill="${d.shade(face, { light: 0.6, dark: 0.1 })}" stroke="${darker(face, 0.1)}" stroke-width="1"/>` +
    strand('M 52 36 C 46 40, 44 46, 47 52', palette.purple, 5.5) +
    strand('M 58 34 C 52 38, 52 44, 55 48', palette.pink, 5) +
    `<path d="M 60 6 L 52.5 33 L 67.5 33 Z" fill="${d.shade(palette.yellow, { light: 0.5, dark: 0.2 })}" stroke="#E0A800" stroke-width="1.4" stroke-linejoin="round"/>` +
    `<path d="M 55.5 24 L 64.5 21 M 54 29 L 66 26 M 57 18 L 62.5 16" stroke="#E0A800" stroke-width="1.5" stroke-linecap="round"/>` +
    `<ellipse cx="62" cy="79" rx="18" ry="12" fill="${d.shade(snoutPink, { light: 0.5, dark: 0.12 })}"/>` +
    `<ellipse cx="56" cy="76.5" rx="2.4" ry="1.8" fill="#E58BA8"/><ellipse cx="68" cy="76.5" rx="2.4" ry="1.8" fill="#E58BA8"/>` +
    mouth(f, 62, 83, 5) +
    eyes(f, 51, 72, 59, 5) +
    cheek(d, 43, 70, 5, 3) +
    cheek(d, 81, 70, 5, 3)
  );
}

/** A white glove (a round hoof in a glove) at (x, y), radius r. */
function glove(d, x, y, r) {
  return `<ellipse cx="${x}" cy="${y}" rx="${r}" ry="${r * 0.9}" fill="${d.shade(suitWhite, { light: 0.5, dark: 0.16 })}" stroke="${darker(suitWhite, 0.2)}" stroke-width="${r * 0.12}"/>`;
}

/** A thought bubble with three dots, top right. */
function thoughtBubble(d, x, y, s) {
  return (
    `<circle cx="${x - s * 1.4}" cy="${y + s * 1.5}" r="${s * 0.22}" fill="#FFFFFF" opacity="0.95"/>` +
    `<circle cx="${x - s * 0.9}" cy="${y + s * 0.95}" r="${s * 0.34}" fill="#FFFFFF" opacity="0.95"/>` +
    `<ellipse cx="${x}" cy="${y}" rx="${s}" ry="${s * 0.72}" fill="${d.shade(palette.cream, { light: 0.3, dark: 0.08 })}"/>` +
    [-0.45, 0, 0.45].map((k) => `<circle cx="${x + s * k}" cy="${y}" r="${s * 0.12}" fill="${palette.purple}"/>`).join('')
  );
}

/** The mood's extras: sparkles for cheering, z z z for sleepy, the bubble for thinking. */
function extras(d, f, box) {
  let s = '';
  if (f.mood === 'cheering') {
    const a = f.sparkles === 0 ? [[0.86, 0.18, 0.07], [0.12, 0.3, 0.05]] : [[0.9, 0.3, 0.05], [0.1, 0.16, 0.07], [0.8, 0.08, 0.04]];
    for (const [x, y, r] of a) s += sparkle(box * x, box * y, box * r, palette.yellow);
  }
  if (f.mood === 'sleepy') {
    const up = f.z === 0 ? 0 : -box * 0.04;
    s += label(box * 0.78, box * 0.26 + up, box * 0.1, 'z', palette.lines, { extra: 'opacity="0.9"' });
    s += label(box * 0.87, box * 0.15 + up, box * 0.075, 'z', palette.lines, { extra: 'opacity="0.7"' });
  }
  if (f.bubble) s += thoughtBubble(d, box * 0.84, box * 0.14, box * 0.1);
  return s;
}

/** The round one: a chubby round suit with the face in its big window. */
function unicornRound(f, size = 384) {
  const d = new Drawing(size, size, `uC${f.id}`);
  const k = size / 140;
  const hand = (x, y) => glove(d, x, y, 9);
  let arms;
  if (f.arms === 'up') arms = hand(10, 64) + hand(130, 64);
  else if (f.arms === 'up-high') arms = hand(14, 50) + hand(126, 50);
  else if (f.arms === 'chin') arms = hand(10, 104) + hand(54, 108);
  else if (f.arms === 'wave') arms = hand(10, 104) + hand(130, 66);
  else arms = hand(12, 104) + hand(128, 104);
  const body =
    `<g transform="translate(0 ${2 * k}) scale(${k})">` +
    d.groundShadow(72, 134, 40, 4, 0.18) +
    `<ellipse cx="52" cy="128" rx="13" ry="8" fill="${d.shade(palette.purple)}"/>` +
    `<ellipse cx="90" cy="128" rx="13" ry="8" fill="${d.shade(palette.purple)}"/>` +
    `<circle cx="70" cy="78" r="56" fill="${d.shade(suitWhite, { light: 0.5, dark: 0.18 })}" stroke="${darker(suitWhite, 0.2)}" stroke-width="1.5"/>` +
    `<path d="M 22 104 Q 70 132 118 104" stroke="${suitLilac}" stroke-width="7" fill="none" stroke-linecap="round"/>` +
    roundStar(70, 120, 6, palette.yellow, { strokeWidth: 1.6 }) +
    `<g transform="translate(16 12) scale(0.9)">` +
    `<circle cx="60" cy="62" r="44" fill="${d.radial([[0, '#3C2A78'], [1, '#2B1B5A']])}"/>` +
    `<g clip-path="${d.clip('<circle cx="60" cy="62" r="44"/>')}">${unicornHead(d, f)}</g>` +
    `<circle cx="60" cy="62" r="44" fill="${d.radial([[0, '#FFFFFF', 0.05], [0.8, '#DDF1FF', 0.2], [1, '#BFE3FF', 0.5]], { cx: '40%', cy: '35%', r: '60%' })}"/>` +
    `<circle cx="60" cy="62" r="45" fill="none" stroke="${collar}" stroke-width="5"/>` +
    `<path d="M 30 44 Q 38 28 54 23" stroke="#FFFFFF" stroke-width="4" fill="none" stroke-linecap="round" opacity="0.85"/>` +
    `</g>` +
    `<path d="M 70 0 L 63 22 L 77 22 Z" fill="${d.shade(palette.yellow, { light: 0.5, dark: 0.2 })}" stroke="#E0A800" stroke-width="1.4" stroke-linejoin="round"/>` +
    `<path d="M 66 12 L 74 10 M 65 17 L 75 15" stroke="#E0A800" stroke-width="1.4" stroke-linecap="round"/>` +
    arms +
    `</g>` +
    extras(d, f, size);
  return d.render(body);
}

/** The unicorn in one frame. Called by sprites. */
function unicorn(frameId, size = 384) {
  return unicornRound(frames.find((x) => x.id === frameId), size);
}

/**
 * The guide's sprites: Guides/guide-unicorn/<frame>.png, 384 × 384. This is take C, the stand-in
 * until the human's art for ChatGPT's unicorn with a full body arrives (local/art-prompts); the
 * co-pilot cat is ChatGPT's (drafts.json).
 */
export function sprites() {
  return frames.map((f) => ({ path: `Guides/guide-unicorn/${f.id}`, width: 384, height: 384, svg: unicorn(f.id) }));
}
